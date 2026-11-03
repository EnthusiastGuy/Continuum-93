using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Continuum93.Emulator.Compilers.C93C
{
    /// <summary>
    /// Token based C preprocessor.
    /// Supports: #define (object-like and function-like, #, ##, __VA_ARGS__), #undef, #include "..." / &lt;...&gt;,
    /// #if / #ifdef / #ifndef / #elif / #else / #endif with defined(), #error, #warning, #pragma (ignored), #line (ignored).
    /// </summary>
    public sealed class CPreprocessor
    {
        private sealed class Macro
        {
            public string Name;
            public bool FunctionLike;
            public List<string> Params = [];
            public bool Variadic;
            public List<CToken> Body = [];
        }

        private sealed class CondState
        {
            public bool ParentActive;
            public bool Active;       // Currently emitting tokens
            public bool AnyTaken;     // A branch of this group was already taken
            public bool SeenElse;
        }

        private const int MaxIncludeDepth = 32;

        private readonly Dictionary<string, Macro> _macros = [];
        private readonly Func<string, string, string> _includeResolver; // (name, includingFile) -> source or null
        private readonly List<CCompileError> _errors;
        private readonly HashSet<string> _pragmaOnce = [];
        private int _includeDepth;

        /// <summary>Values of "#pragma c93 &lt;name&gt; &lt;value&gt;" directives (e.g. origin, stack).</summary>
        public Dictionary<string, CToken> Pragmas { get; } = [];

        public CPreprocessor(Func<string, string, string> includeResolver, List<CCompileError> errors)
        {
            _includeResolver = includeResolver;
            _errors = errors;
            Define("__C93__", "1");
            Define("__STDC__", "1");
        }

        public void Define(string name, string value)
        {
            var lexErrors = new List<CCompileError>();
            var body = new CLexer(value, "<builtin>", lexErrors).Tokenize()
                .Where(t => t.Kind != CTokenKind.Newline && t.Kind != CTokenKind.EOF).ToList();
            _macros[name] = new Macro { Name = name, Body = body };
        }

        public List<CToken> Process(string source, string file)
        {
            var output = new List<CToken>();
            ProcessFile(source, file, output);
            output.Add(new CToken(CTokenKind.EOF, "", file, output.Count > 0 ? output[^1].Line : 1, 1));
            return output;
        }

        private void ProcessFile(string source, string file, List<CToken> output)
        {
            List<CToken> tokens = new CLexer(source, file, _errors).Tokenize();
            var conds = new Stack<CondState>();
            int i = 0;

            // Collect text lines; each directive line is handled separately, other lines are macro-expanded in runs
            var pending = new List<CToken>();

            while (i < tokens.Count && tokens[i].Kind != CTokenKind.EOF)
            {
                // Read a single line
                int lineStart = i;
                while (tokens[i].Kind != CTokenKind.Newline && tokens[i].Kind != CTokenKind.EOF)
                    i++;
                List<CToken> line = tokens.GetRange(lineStart, i - lineStart);
                if (tokens[i].Kind == CTokenKind.Newline)
                    i++;

                bool active = conds.Count == 0 || conds.Peek().Active;

                if (line.Count > 0 && line[0].Is("#") && line[0].Kind == CTokenKind.Punct)
                {
                    // Flush text collected so far (macro invocations may not span directives)
                    if (pending.Count > 0)
                    {
                        output.AddRange(Expand(pending));
                        pending.Clear();
                    }
                    HandleDirective(line, file, conds, active, output);
                    continue;
                }

                if (active)
                    pending.AddRange(line);
            }

            if (pending.Count > 0)
                output.AddRange(Expand(pending));

            if (conds.Count > 0)
                Error(tokens[^1], "Unterminated conditional directive (#if without #endif)");
        }

        private void HandleDirective(List<CToken> line, string file, Stack<CondState> conds, bool active, List<CToken> output)
        {
            if (line.Count == 1)
                return; // Null directive

            CToken dirTok = line[1];
            string dir = dirTok.Text;
            List<CToken> args = line.Skip(2).ToList();

            switch (dir)
            {
                case "if":
                    {
                        bool value = active && EvaluateCondition(args, dirTok);
                        conds.Push(new CondState { ParentActive = active, Active = value, AnyTaken = value });
                        return;
                    }
                case "ifdef":
                case "ifndef":
                    {
                        bool value = false;
                        if (active)
                        {
                            if (args.Count == 0 || args[0].Kind != CTokenKind.Identifier)
                                Error(dirTok, $"#{dir} expects a macro name");
                            else
                                value = _macros.ContainsKey(args[0].Text) == (dir == "ifdef");
                        }
                        conds.Push(new CondState { ParentActive = active, Active = value, AnyTaken = value });
                        return;
                    }
                case "elif":
                    {
                        if (conds.Count == 0) { Error(dirTok, "#elif without #if"); return; }
                        CondState s = conds.Peek();
                        if (s.SeenElse) Error(dirTok, "#elif after #else");
                        if (!s.ParentActive || s.AnyTaken)
                        {
                            s.Active = false;
                        }
                        else
                        {
                            s.Active = EvaluateCondition(args, dirTok);
                            s.AnyTaken = s.Active;
                        }
                        return;
                    }
                case "else":
                    {
                        if (conds.Count == 0) { Error(dirTok, "#else without #if"); return; }
                        CondState s = conds.Peek();
                        if (s.SeenElse) Error(dirTok, "Duplicate #else");
                        s.SeenElse = true;
                        s.Active = s.ParentActive && !s.AnyTaken;
                        s.AnyTaken = true;
                        return;
                    }
                case "endif":
                    if (conds.Count == 0) Error(dirTok, "#endif without #if");
                    else conds.Pop();
                    return;
            }

            if (!active)
                return;

            switch (dir)
            {
                case "define":
                    HandleDefine(args, dirTok);
                    break;
                case "undef":
                    if (args.Count == 0 || args[0].Kind != CTokenKind.Identifier)
                        Error(dirTok, "#undef expects a macro name");
                    else
                        _macros.Remove(args[0].Text);
                    break;
                case "include":
                    HandleInclude(args, dirTok, file, output);
                    break;
                case "error":
                    Error(dirTok, "#error " + JoinTokens(args));
                    break;
                case "warning":
                    break;
                case "pragma":
                    if (args.Count > 0 && args[0].Text == "once")
                        _pragmaOnce.Add(file);
                    else if (args.Count >= 3 && args[0].Text == "c93" && args[1].Kind == CTokenKind.Identifier)
                        Pragmas[args[1].Text] = args[2];   // e.g. #pragma c93 origin 0x000000
                    break;
                case "line":
                    break;
                default:
                    Error(dirTok, $"Unknown preprocessor directive '#{dir}'");
                    break;
            }
        }

        private void HandleDefine(List<CToken> args, CToken dirTok)
        {
            if (args.Count == 0 || args[0].Kind != CTokenKind.Identifier)
            {
                Error(dirTok, "#define expects a macro name");
                return;
            }

            var macro = new Macro { Name = args[0].Text };
            int i = 1;

            // Function-like only if '(' immediately follows the name (no whitespace)
            if (i < args.Count && args[i].Is("(") && !args[i].LeadingSpace)
            {
                macro.FunctionLike = true;
                i++;
                bool expectParam = true;
                while (true)
                {
                    if (i >= args.Count)
                    {
                        Error(dirTok, "Missing ')' in macro parameter list");
                        return;
                    }
                    CToken t = args[i++];
                    if (t.Is(")"))
                        break;
                    if (t.Is(",") && !expectParam) { expectParam = true; continue; }
                    if (t.Is("...") && expectParam)
                    {
                        macro.Variadic = true;
                        macro.Params.Add("__VA_ARGS__");
                        expectParam = false;
                        continue;
                    }
                    if (t.Kind == CTokenKind.Identifier && expectParam)
                    {
                        macro.Params.Add(t.Text);
                        expectParam = false;
                        continue;
                    }
                    Error(t, $"Invalid token '{t}' in macro parameter list");
                    return;
                }
            }

            macro.Body = args.Skip(i).Select(t => t.Clone()).ToList();
            _macros[macro.Name] = macro;
        }

        private void HandleInclude(List<CToken> args, CToken dirTok, string file, List<CToken> output)
        {
            string name = null;
            if (args.Count == 1 && args[0].Kind == CTokenKind.String)
            {
                name = args[0].Text;
            }
            else if (args.Count >= 2 && args[0].Is("<"))
            {
                var sb = new StringBuilder();
                int i = 1;
                for (; i < args.Count && !args[i].Is(">"); i++)
                    sb.Append(args[i].Text);
                if (i < args.Count)
                    name = sb.ToString();
            }

            if (name == null)
            {
                Error(dirTok, "#include expects \"FILENAME\" or <FILENAME>");
                return;
            }

            if (_includeDepth >= MaxIncludeDepth)
            {
                Error(dirTok, "#include nested too deeply");
                return;
            }

            string src = _includeResolver?.Invoke(name, file);
            if (src == null)
            {
                Error(dirTok, $"Cannot find include file '{name}'");
                return;
            }

            if (_pragmaOnce.Contains(name))
                return;

            _includeDepth++;
            ProcessFile(src, name, output);
            _includeDepth--;
        }

        // ---------------------------------------------------------------- #if evaluation

        private bool EvaluateCondition(List<CToken> args, CToken dirTok)
        {
            // Replace defined X / defined(X) before macro expansion
            var replaced = new List<CToken>();
            for (int i = 0; i < args.Count; i++)
            {
                if (args[i].Kind == CTokenKind.Identifier && args[i].Text == "defined")
                {
                    string name = null;
                    if (i + 1 < args.Count && args[i + 1].Kind == CTokenKind.Identifier)
                    {
                        name = args[i + 1].Text;
                        i += 1;
                    }
                    else if (i + 3 < args.Count && args[i + 1].Is("(") && args[i + 2].Kind == CTokenKind.Identifier && args[i + 3].Is(")"))
                    {
                        name = args[i + 2].Text;
                        i += 3;
                    }
                    else
                    {
                        Error(args[i], "Malformed 'defined' expression");
                        return false;
                    }
                    replaced.Add(new CToken(CTokenKind.Number, _macros.ContainsKey(name) ? "1" : "0", args[i].File, args[i].Line, args[i].Column));
                    continue;
                }
                replaced.Add(args[i]);
            }

            List<CToken> expanded = Expand(replaced);

            // Remaining identifiers evaluate to 0
            for (int i = 0; i < expanded.Count; i++)
            {
                if (expanded[i].Kind == CTokenKind.Identifier)
                {
                    var t = expanded[i].Clone();
                    t.Kind = CTokenKind.Number;
                    t.Text = expanded[i].Text == "true" ? "1" : "0";
                    expanded[i] = t;
                }
            }

            if (expanded.Count == 0)
            {
                Error(dirTok, "#if with no expression");
                return false;
            }

            var errors = new List<CCompileError>();
            long value = CConstExpr.EvaluatePreprocessor(expanded, errors);
            if (errors.Count > 0)
            {
                _errors.AddRange(errors);
                return false;
            }
            return value != 0;
        }

        // ---------------------------------------------------------------- Macro expansion

        private List<CToken> Expand(List<CToken> input)
        {
            var output = new List<CToken>();
            var queue = new LinkedList<CToken>(input);

            while (queue.Count > 0)
            {
                CToken tok = queue.First.Value;
                queue.RemoveFirst();

                if (tok.Kind != CTokenKind.Identifier || !_macros.TryGetValue(tok.Text, out Macro macro)
                    || (tok.HideSet != null && tok.HideSet.Contains(tok.Text)))
                {
                    output.Add(tok);
                    continue;
                }

                if (!macro.FunctionLike)
                {
                    var hs = Union(tok.HideSet, macro.Name);
                    List<CToken> body = Substitute(macro, null, hs, tok);
                    PushFront(queue, body);
                    continue;
                }

                // Function-like macro: needs '(' next, otherwise it's a plain identifier
                if (queue.Count == 0 || !queue.First.Value.Is("("))
                {
                    output.Add(tok);
                    continue;
                }

                queue.RemoveFirst(); // '('
                List<List<CToken>> actuals = ReadArguments(queue, tok, out CToken closeParen);
                if (actuals == null)
                    return output;

                if (macro.Params.Count == 0 && actuals.Count == 1 && actuals[0].Count == 0)
                    actuals.Clear();

                if (macro.Variadic)
                {
                    int fixedCount = macro.Params.Count - 1;
                    if (actuals.Count < fixedCount)
                    {
                        Error(tok, $"Macro '{macro.Name}' requires at least {fixedCount} argument(s)");
                        continue;
                    }
                    // Merge extra arguments into __VA_ARGS__
                    var va = new List<CToken>();
                    for (int a = fixedCount; a < actuals.Count; a++)
                    {
                        if (a > fixedCount)
                            va.Add(new CToken(CTokenKind.Punct, ",", tok.File, tok.Line, tok.Column));
                        va.AddRange(actuals[a]);
                    }
                    actuals = actuals.Take(fixedCount).ToList();
                    actuals.Add(va);
                }
                else if (actuals.Count != macro.Params.Count)
                {
                    Error(tok, $"Macro '{macro.Name}' expects {macro.Params.Count} argument(s) but got {actuals.Count}");
                    continue;
                }

                var hideSet = Union(Intersect(tok.HideSet, closeParen.HideSet), macro.Name);
                List<CToken> result = Substitute(macro, actuals, hideSet, tok);
                PushFront(queue, result);
            }

            return output;
        }

        private List<List<CToken>> ReadArguments(LinkedList<CToken> queue, CToken macroTok, out CToken closeParen)
        {
            var args = new List<List<CToken>> { new() };
            int depth = 0;
            closeParen = null;

            while (true)
            {
                if (queue.Count == 0)
                {
                    Error(macroTok, $"Unterminated argument list invoking macro '{macroTok.Text}'");
                    return null;
                }

                CToken t = queue.First.Value;
                queue.RemoveFirst();

                if (t.Is("(")) depth++;
                else if (t.Is(")"))
                {
                    if (depth == 0)
                    {
                        closeParen = t;
                        return args;
                    }
                    depth--;
                }
                else if (t.Is(",") && depth == 0)
                {
                    args.Add([]);
                    continue;
                }

                args[^1].Add(t);
            }
        }

        private List<CToken> Substitute(Macro macro, List<List<CToken>> actuals, HashSet<string> hideSet, CToken site)
        {
            var result = new List<CToken>();
            List<CToken> body = macro.Body;

            int ParamIndex(CToken t) =>
                actuals != null && t.Kind == CTokenKind.Identifier ? macro.Params.IndexOf(t.Text) : -1;

            for (int i = 0; i < body.Count; i++)
            {
                CToken t = body[i];

                // Stringification: # param
                if (macro.FunctionLike && t.Is("#") && i + 1 < body.Count && ParamIndex(body[i + 1]) >= 0)
                {
                    List<CToken> arg = actuals[ParamIndex(body[i + 1])];
                    result.Add(new CToken(CTokenKind.String, Stringify(arg), site.File, site.Line, site.Column) { LeadingSpace = t.LeadingSpace });
                    i++;
                    continue;
                }

                // Token pasting: X ## Y
                if (i + 1 < body.Count && body[i + 1].Is("##"))
                {
                    List<CToken> left = ParamIndex(t) >= 0 ? actuals[ParamIndex(t)].Select(x => x.Clone()).ToList() : [t.Clone()];
                    i += 2;
                    while (true)
                    {
                        if (i >= body.Count)
                        {
                            Error(site, "'##' cannot appear at the end of a macro expansion");
                            break;
                        }
                        CToken r = body[i];
                        List<CToken> right = ParamIndex(r) >= 0 ? actuals[ParamIndex(r)].Select(x => x.Clone()).ToList() : [r.Clone()];
                        left = Paste(left, right, site);
                        if (i + 1 < body.Count && body[i + 1].Is("##"))
                        {
                            i += 2;
                            continue;
                        }
                        break;
                    }
                    foreach (var x in left)
                        result.Add(Relocate(x, hideSet, site));
                    continue;
                }

                int p = ParamIndex(t);
                if (p >= 0)
                {
                    // Arguments are fully macro-expanded before substitution
                    List<CToken> expandedArg = Expand(actuals[p].Select(x => x.Clone()).ToList());
                    bool first = true;
                    foreach (var x in expandedArg)
                    {
                        var c = Relocate(x, hideSet, site);
                        if (first) c.LeadingSpace = t.LeadingSpace;
                        first = false;
                        result.Add(c);
                    }
                    continue;
                }

                result.Add(Relocate(t, hideSet, site));
            }

            return result;
        }

        private List<CToken> Paste(List<CToken> left, List<CToken> right, CToken site)
        {
            if (left.Count == 0) return right;
            if (right.Count == 0) return left;

            string text = left[^1].Text + right[0].Text;
            var lexErrors = new List<CCompileError>();
            var toks = new CLexer(text, site.File, lexErrors).Tokenize()
                .Where(t => t.Kind != CTokenKind.Newline && t.Kind != CTokenKind.EOF).ToList();
            if (toks.Count != 1 || lexErrors.Count > 0)
                Error(site, $"Pasting \"{left[^1].Text}\" and \"{right[0].Text}\" does not give a valid token");

            var result = left.Take(left.Count - 1).ToList();
            result.AddRange(toks);
            result.AddRange(right.Skip(1));
            return result;
        }

        private static CToken Relocate(CToken t, HashSet<string> hideSet, CToken site)
        {
            CToken c = t.Clone();
            c.File = site.File;
            c.Line = site.Line;
            c.Column = site.Column;
            c.HideSet = Union(t.HideSet, hideSet);
            return c;
        }

        private static string Stringify(List<CToken> tokens)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < tokens.Count; i++)
            {
                if (i > 0 && tokens[i].LeadingSpace)
                    sb.Append(' ');
                CToken t = tokens[i];
                if (t.Kind == CTokenKind.String)
                    sb.Append('"').Append(Escape(t.Text)).Append('"');
                else if (t.Kind == CTokenKind.Char)
                    sb.Append('\'').Append(Escape(t.Text)).Append('\'');
                else
                    sb.Append(t.Text);
            }
            return sb.ToString();
        }

        private static string Escape(string s) => s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n");

        private static string JoinTokens(List<CToken> tokens) => string.Join(" ", tokens.Select(t => t.ToString()));

        private static void PushFront(LinkedList<CToken> queue, List<CToken> tokens)
        {
            for (int i = tokens.Count - 1; i >= 0; i--)
                queue.AddFirst(tokens[i]);
        }

        private static HashSet<string> Union(HashSet<string> set, string name)
        {
            var r = set == null ? new HashSet<string>() : new HashSet<string>(set);
            r.Add(name);
            return r;
        }

        private static HashSet<string> Union(HashSet<string> a, HashSet<string> b)
        {
            if (a == null || a.Count == 0) return b;
            if (b == null || b.Count == 0) return a;
            var r = new HashSet<string>(a);
            r.UnionWith(b);
            return r;
        }

        private static HashSet<string> Intersect(HashSet<string> a, HashSet<string> b)
        {
            if (a == null || b == null) return null;
            var r = new HashSet<string>(a);
            r.IntersectWith(b);
            return r;
        }

        private void Error(CToken at, string message) =>
            _errors.Add(new CCompileError(at.File, at.Line, at.Column, message));
    }
}
