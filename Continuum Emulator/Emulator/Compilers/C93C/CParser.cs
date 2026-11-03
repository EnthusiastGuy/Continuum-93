using System;
using System.Collections.Generic;
using System.Linq;

namespace Continuum93.Emulator.Compilers.C93C
{
    /// <summary>
    /// Recursive-descent C parser. Builds a fully typed AST: identifiers are resolved, implicit conversions are
    /// inserted as Cast nodes, pointer arithmetic is scaled, and ++/--/compound assignments/initializers are lowered
    /// into simpler nodes. The first error aborts parsing.
    /// </summary>
    public sealed class CParser
    {
        private sealed class DeclSpec
        {
            public CType Type;
            public bool IsTypedef;
            public bool IsStatic;
            public bool IsExtern;
        }

        private List<CToken> _toks;
        private readonly List<CCompileError> _errors;
        private readonly CTranslationUnit _unit;
        private int _pos;

        private readonly List<Dictionary<string, CSymbol>> _scopes = [new()];
        private readonly List<Dictionary<string, CType>> _tagScopes = [new()];
        private readonly Dictionary<string, CSymbol> _globals = [];

        private CFunction _fn;                       // Function being parsed
        private string _brk;                         // Current break target label
        private string _cont;                        // Current continue target label
        private CNode _switch;                       // Innermost switch
        private Dictionary<string, string> _userLabels;      // goto labels in current function -> unique label
        private List<CNode> _gotos;
        private int _labelCounter;
        private int _staticCounter;
        private bool _libraryMode;                   // Parsing the runtime library: user definitions take precedence

        public CParser(List<CToken> tokens, List<CCompileError> errors, CTranslationUnit unit)
        {
            _toks = tokens;
            _errors = errors;
            _unit = unit;
        }

        public int LabelCounter => _labelCounter;

        /// <summary>Continues parsing another token stream into the same unit and scopes (used for the runtime library).</summary>
        public void ParseAdditional(List<CToken> tokens, bool libraryMode)
        {
            _toks = tokens;
            _pos = 0;
            ParseTranslationUnit(libraryMode);
        }

        // ====================================================================== Top level

        public void ParseTranslationUnit(bool libraryMode = false)
        {
            _libraryMode = libraryMode;
            while (!AtEof())
            {
                if (Accept(";"))
                    continue;

                DeclSpec spec = ParseDeclSpec(true);

                if (spec.IsTypedef)
                {
                    ParseTypedef(spec);
                    continue;
                }

                if (Accept(";"))
                    continue; // e.g. "struct S { ... };"

                CToken nameTok;
                CType ty = ParseDeclarator(spec.Type, out nameTok);
                if (nameTok == null)
                    throw Error(Cur, "Expected an identifier in declaration");

                if (ty.IsFunction && Peek("{"))
                {
                    ParseFunctionDefinition(ty, nameTok, spec);
                    continue;
                }

                // Declarations (possibly several separated by commas)
                while (true)
                {
                    if (ty.IsFunction)
                        DeclareFunction(ty, nameTok, spec, false);
                    else
                        ParseGlobalVariable(ty, nameTok, spec);

                    if (Accept(";"))
                        break;
                    Expect(",");
                    ty = ParseDeclarator(spec.Type, out nameTok);
                    if (nameTok == null)
                        throw Error(Cur, "Expected an identifier in declaration");
                }
            }
        }

        private void ParseTypedef(DeclSpec spec)
        {
            bool first = true;
            while (!Accept(";"))
            {
                if (!first) Expect(",");
                first = false;
                CType ty = ParseDeclarator(spec.Type, out CToken name);
                if (name == null)
                    throw Error(Cur, "Typedef requires a name");
                DeclareInScope(name, new CSymbol { Name = name.Text, Kind = CSymbolKind.Typedef, Type = ty });
            }
        }

        private CSymbol DeclareFunction(CType ty, CToken nameTok, DeclSpec spec, bool isDefinition)
        {
            string name = nameTok.Text;
            if (ty.Base.IsStruct)
                throw Error(nameTok, "Functions returning structs by value are not supported; return a pointer instead");
            if (ty.Base.IsArray)
                throw Error(nameTok, "Functions cannot return arrays");

            if (_globals.TryGetValue(name, out CSymbol existing))
            {
                if (existing.Kind != CSymbolKind.Function)
                    throw Error(nameTok, $"'{name}' redeclared as a different kind of symbol");
                if (!CType.SameType(existing.Type, ty))
                    throw Error(nameTok, $"Conflicting types for '{name}': {existing.Type} vs {ty}");
                if (isDefinition && existing.IsDefined)
                    throw Error(nameTok, $"Redefinition of function '{name}'");
                if (ty.HasPrototype && !existing.Type.HasPrototype)
                    existing.Type = ty;
                return existing;
            }

            var sym = new CSymbol
            {
                Name = name,
                Kind = CSymbolKind.Function,
                Type = ty,
                Label = ".c_" + name,
                IsStatic = spec.IsStatic
            };
            _globals[name] = sym;
            _scopes[0][name] = sym;
            return sym;
        }

        private void ParseFunctionDefinition(CType ty, CToken nameTok, DeclSpec spec)
        {
            if (_libraryMode && _globals.TryGetValue(nameTok.Text, out CSymbol userSym) && userSym.IsDefined)
            {
                SkipBalancedBraces(); // The user's own definition wins over the runtime library
                return;
            }

            if (ty.Variadic)
                throw Error(nameTok, "Defining variadic functions is not supported");

            CSymbol sym = DeclareFunction(ty, nameTok, spec, true);
            sym.IsDefined = true;
            sym.Type = ty;

            var fn = new CFunction
            {
                Name = nameTok.Text,
                Symbol = sym,
                Type = ty,
                ReturnLabel = $".c_{nameTok.Text}__ret"
            };
            sym.Function = fn;
            _fn = fn;
            _userLabels = [];
            _gotos = [];

            EnterScope();

            // Parameters
            if (ty.HasPrototype)
            {
                foreach (CParam p in ty.Params)
                {
                    if (p.Name == null)
                        throw Error(nameTok, $"Parameter name omitted in definition of '{nameTok.Text}'");
                    if (p.Type.IsStruct)
                        throw Error(nameTok, "Passing structs by value is not supported; pass a pointer instead");
                    var ps = new CSymbol { Name = p.Name, Kind = CSymbolKind.Local, Type = p.Type };
                    fn.Params.Add(ps);
                    _scopes[^1][p.Name] = ps;
                }
            }

            fn.Body = ParseCompoundStatement(false);
            LeaveScope();

            // Resolve gotos
            foreach (CNode g in _gotos)
            {
                if (!_userLabels.TryGetValue(g.LabelName, out string unique))
                    throw Error(g.Tok, $"Label '{g.LabelName}' used but not defined");
                g.UniqueLabel = unique;
            }

            // Frame layout: params above the frame pointer, locals below
            for (int i = 0; i < fn.Params.Count; i++)
            {
                int size = fn.Params[i].Type.Size;
                fn.Params[i].Offset = 4 * i + (4 - Math.Min(size, 4));
            }
            int offset = 0;
            foreach (CSymbol local in fn.Locals)
            {
                int size = Math.Max(local.Type.Size, 1);
                offset -= size;
                local.Offset = offset;
            }
            fn.FrameSize = -offset;

            _unit.Functions.Add(fn);
            _fn = null;
        }

        private void SkipBalancedBraces()
        {
            Expect("{");
            int depth = 1;
            while (depth > 0)
            {
                if (AtEof()) throw Error(Cur, "Unexpected end of file");
                if (Peek("{")) depth++;
                else if (Peek("}")) depth--;
                _pos++;
            }
        }

        private void ParseGlobalVariable(CType ty, CToken nameTok, DeclSpec spec)
        {
            string name = nameTok.Text;
            if (ty.IsVoid)
                throw Error(nameTok, $"Variable '{name}' declared void");

            if (_globals.TryGetValue(name, out CSymbol sym))
            {
                if (sym.Kind != CSymbolKind.Global)
                    throw Error(nameTok, $"'{name}' redeclared as a different kind of symbol");
                if (!CType.SameType(sym.Type, ty))
                    throw Error(nameTok, $"Conflicting types for '{name}'");
                if (sym.Type.IsArray && sym.Type.Length < 0 && ty.Length >= 0)
                    sym.Type = ty;
            }
            else
            {
                sym = new CSymbol { Name = name, Kind = CSymbolKind.Global, Type = ty, Label = ".c_" + name, IsStatic = spec.IsStatic };
                _globals[name] = sym;
                _scopes[0][name] = sym;
                _unit.Globals.Add(sym);
            }

            if (Accept("="))
            {
                if (sym.HasInitializer)
                    throw Error(nameTok, $"Redefinition of '{name}'");
                ParseGlobalInitializer(sym);
                sym.HasInitializer = true;
                sym.IsDefined = true;
                sym.IsExtern = false;
            }
            else if (!spec.IsExtern)
            {
                sym.IsDefined = true;
                sym.IsExtern = false;
            }
            else if (!sym.IsDefined)
            {
                sym.IsExtern = true;
            }
        }

        // ====================================================================== Declaration specifiers

        private static readonly HashSet<string> TypeKeywords =
        [
            "void", "_Bool", "char", "short", "int", "long", "signed", "unsigned", "struct", "union", "enum",
            "typedef", "static", "extern", "inline", "const", "volatile", "auto", "register", "restrict",
            "__restrict", "float", "double", "_Noreturn", "__inline"
        ];

        private bool IsTypeName(CToken t)
        {
            if (t.Kind != CTokenKind.Identifier) return false;
            if (TypeKeywords.Contains(t.Text)) return true;
            CSymbol s = Lookup(t.Text);
            return s != null && s.Kind == CSymbolKind.Typedef;
        }

        private DeclSpec ParseDeclSpec(bool allowStorage)
        {
            var spec = new DeclSpec();
            int voids = 0, bools = 0, chars = 0, shorts = 0, ints = 0, longs = 0, signeds = 0, unsigneds = 0;
            CType other = null;
            CToken start = Cur;

            while (IsTypeName(Cur))
            {
                CToken t = Cur;
                string s = t.Text;

                if (s is "typedef" or "static" or "extern" or "auto" or "register" or "inline" or "__inline" or "_Noreturn")
                {
                    if (!allowStorage)
                        throw Error(t, $"Storage class '{s}' is not allowed here");
                    if (s == "typedef") spec.IsTypedef = true;
                    if (s == "static") spec.IsStatic = true;
                    if (s == "extern") spec.IsExtern = true;
                    _pos++;
                    continue;
                }

                if (s is "const" or "volatile" or "restrict" or "__restrict")
                {
                    _pos++;
                    continue;
                }

                if (s is "float" or "double")
                    throw Error(t, "Floating point types are not supported");

                if (s is "struct" or "union")
                {
                    if (other != null || voids + chars + shorts + ints + longs + signeds + unsigneds + bools > 0)
                        throw Error(t, "Invalid combination of type specifiers");
                    _pos++;
                    other = ParseStructDecl(s == "union");
                    continue;
                }

                if (s == "enum")
                {
                    if (other != null || voids + chars + shorts + ints + longs + signeds + unsigneds + bools > 0)
                        throw Error(t, "Invalid combination of type specifiers");
                    _pos++;
                    other = ParseEnumDecl();
                    continue;
                }

                CSymbol td = TypeKeywords.Contains(s) ? null : Lookup(s);
                if (td != null && td.Kind == CSymbolKind.Typedef)
                {
                    // A typedef name only counts as the type if no other type specifier was seen yet
                    if (other != null || voids + chars + shorts + ints + longs + signeds + unsigneds + bools > 0)
                        break;
                    other = td.Type;
                    _pos++;
                    continue;
                }

                if (other != null)
                    throw Error(t, "Invalid combination of type specifiers");

                switch (s)
                {
                    case "void": voids++; break;
                    case "_Bool": bools++; break;
                    case "char": chars++; break;
                    case "short": shorts++; break;
                    case "int": ints++; break;
                    case "long": longs++; break;
                    case "signed": signeds++; break;
                    case "unsigned": unsigneds++; break;
                }
                _pos++;
            }

            if (longs > 1)
                throw Error(start, "'long long' is not supported (integers are 32-bit)");

            if (other != null)
            {
                spec.Type = other;
                return spec;
            }

            int total = voids + bools + chars + shorts + ints + longs + signeds + unsigneds;
            if (total == 0)
            {
                if (spec.IsTypedef || spec.IsStatic || spec.IsExtern)
                {
                    spec.Type = CType.Int; // "static x;" is implicitly int
                    return spec;
                }
                throw Error(start, $"Expected a type, got '{start}'");
            }

            if (signeds > 0 && unsigneds > 0 || signeds > 1 || unsigneds > 1 || ints > 1 || chars > 1 || shorts > 1)
                throw Error(start, "Invalid combination of type specifiers");

            if (voids > 0)
            {
                if (total != 1) throw Error(start, "Invalid combination of type specifiers");
                spec.Type = CType.Void;
            }
            else if (bools > 0)
            {
                if (total != 1) throw Error(start, "Invalid combination of type specifiers");
                spec.Type = CType.Bool;
            }
            else if (chars > 0)
            {
                if (shorts + ints + longs > 0) throw Error(start, "Invalid combination of type specifiers");
                spec.Type = unsigneds > 0 ? CType.UChar : CType.Char;
            }
            else if (shorts > 0)
            {
                if (longs > 0) throw Error(start, "Invalid combination of type specifiers");
                spec.Type = unsigneds > 0 ? CType.UShort : CType.Short;
            }
            else
            {
                spec.Type = unsigneds > 0 ? CType.UInt : CType.Int;
            }
            return spec;
        }

        private CType ParseStructDecl(bool isUnion)
        {
            CToken tagTok = null;
            if (Cur.Kind == CTokenKind.Identifier)
            {
                tagTok = Cur;
                _pos++;
            }

            if (!Peek("{"))
            {
                if (tagTok == null)
                    throw Error(Cur, "Expected a struct tag or '{'");
                CType found = LookupTag(tagTok.Text);
                if (found != null)
                {
                    if (!found.IsStruct || found.Struct.IsUnion != isUnion)
                        throw Error(tagTok, $"'{tagTok.Text}' defined as wrong kind of tag");
                    return found;
                }
                // Forward declaration: incomplete type in the current scope
                var fwd = new CType { Kind = CTypeKind.Struct, Struct = new CStructInfo { Tag = tagTok.Text, IsUnion = isUnion } };
                _tagScopes[^1][tagTok.Text] = fwd;
                return fwd;
            }

            Expect("{");

            CType ty;
            if (tagTok != null && _tagScopes[^1].TryGetValue(tagTok.Text, out CType existing) && existing.IsStruct && !existing.Struct.Complete)
            {
                if (existing.Struct.IsUnion != isUnion)
                    throw Error(tagTok, $"'{tagTok.Text}' defined as wrong kind of tag");
                ty = existing;
            }
            else
            {
                if (tagTok != null && _tagScopes[^1].TryGetValue(tagTok.Text, out CType prev) && prev.IsStruct && prev.Struct.Complete)
                    throw Error(tagTok, $"Redefinition of '{(isUnion ? "union" : "struct")} {tagTok.Text}'");
                ty = new CType { Kind = CTypeKind.Struct, Struct = new CStructInfo { Tag = tagTok?.Text, IsUnion = isUnion } };
                if (tagTok != null)
                    _tagScopes[^1][tagTok.Text] = ty;
            }

            var members = new List<CMember>();
            while (!Accept("}"))
            {
                DeclSpec spec = ParseDeclSpec(false);
                if (Accept(";"))
                {
                    // Anonymous struct/union member
                    if (spec.Type.IsStruct && spec.Type.Struct.Complete)
                    {
                        members.Add(new CMember { Name = null, Type = spec.Type });
                        continue;
                    }
                    throw Error(Cur, "Declaration does not declare anything");
                }
                bool first = true;
                while (!Accept(";"))
                {
                    if (!first) Expect(",");
                    first = false;
                    CType mty = ParseDeclarator(spec.Type, out CToken mname);
                    if (Peek(":"))
                        throw Error(Cur, "Bit-fields are not supported");
                    if (mname == null)
                        throw Error(Cur, "Expected a member name");
                    if (!mty.IsComplete)
                        throw Error(mname, $"Member '{mname.Text}' has incomplete type");
                    if (members.Any(m => m.Name == mname.Text))
                        throw Error(mname, $"Duplicate member '{mname.Text}'");
                    members.Add(new CMember { Name = mname.Text, Type = mty });
                }
            }

            // Layout (packed)
            int size = 0;
            var flat = new List<CMember>();
            foreach (CMember m in members)
            {
                int off = isUnion ? 0 : size;
                if (m.Name == null)
                {
                    // Hoist anonymous members into this struct
                    foreach (CMember inner in m.Type.Struct.Members)
                        flat.Add(new CMember { Name = inner.Name, Type = inner.Type, Offset = off + inner.Offset });
                }
                else
                {
                    m.Offset = off;
                    flat.Add(m);
                }
                size = isUnion ? Math.Max(size, m.Type.Size) : size + m.Type.Size;
            }

            ty.Struct.Members = flat;
            ty.Struct.Size = size;
            ty.Struct.Complete = true;
            return ty;
        }

        private CType ParseEnumDecl()
        {
            CToken tagTok = null;
            if (Cur.Kind == CTokenKind.Identifier)
            {
                tagTok = Cur;
                _pos++;
            }

            var ty = new CType { Kind = CTypeKind.Int, IsEnum = true };

            if (!Peek("{"))
            {
                if (tagTok == null)
                    throw Error(Cur, "Expected an enum tag or '{'");
                CType found = LookupTag(tagTok.Text);
                if (found == null)
                    throw Error(tagTok, $"Unknown enum '{tagTok.Text}'");
                return found;
            }

            Expect("{");
            long value = 0;
            bool first = true;
            while (!Accept("}"))
            {
                if (!first)
                {
                    Expect(",");
                    if (Accept("}")) break;
                }
                first = false;

                CToken name = ExpectIdentifier();
                if (Accept("="))
                    value = EvalConstExpr(ParseConditional());
                DeclareInScope(name, new CSymbol { Name = name.Text, Kind = CSymbolKind.EnumConst, Type = CType.Int, EnumValue = (int)value });
                value++;
            }

            if (tagTok != null)
                _tagScopes[^1][tagTok.Text] = ty;
            return ty;
        }

        // ====================================================================== Declarators

        private CType ParseDeclarator(CType ty, out CToken name)
        {
            while (Accept("*"))
            {
                ty = CType.PointerTo(ty);
                while (Cur.Kind == CTokenKind.Identifier && Cur.Text is "const" or "volatile" or "restrict" or "__restrict")
                    _pos++;
            }

            if (Peek("(") && IsNestedDeclaratorStart(Peek2))
            {
                // Parse the inner declarator later, once the outer suffix is known
                int start = _pos;
                _pos++;
                ParseDeclarator(CType.Int, out _);   // skip
                Expect(")");
                ty = ParseTypeSuffix(ty);
                int end = _pos;
                _pos = start + 1;
                CType result = ParseDeclarator(ty, out name);
                Expect(")");
                _pos = end;
                return result;
            }

            // The declarator name may coincide with a typedef name (redeclaration or shadowing); the
            // declaration specifiers were already consumed, so any non-keyword identifier here is the name.
            name = null;
            if (Cur.Kind == CTokenKind.Identifier && !IsKeyword(Cur.Text))
            {
                name = Cur;
                _pos++;
            }
            return ParseTypeSuffix(ty);
        }

        private bool IsNestedDeclaratorStart(CToken next) =>
            next.Is("*") || next.Is("(") || (next.Kind == CTokenKind.Identifier && !IsTypeName(next));

        private CType ParseAbstractTypeName()
        {
            DeclSpec spec = ParseDeclSpec(false);
            CType ty = ParseDeclarator(spec.Type, out CToken name);
            if (name != null)
                throw Error(name, "Unexpected identifier in type name");
            return ty;
        }

        private CType ParseTypeSuffix(CType ty)
        {
            if (Accept("("))
                return ParseFunctionParams(ty);

            if (Accept("["))
            {
                int length = -1;
                while (Cur.Kind == CTokenKind.Identifier && Cur.Text is "static" or "const" or "restrict")
                    _pos++;
                if (!Accept("]"))
                {
                    CToken at = Cur;
                    long n = EvalConstExpr(ParseConditional(), "Array size must be a constant expression (variable-length arrays are not supported)");
                    if (n < 0) throw Error(at, "Array size is negative");
                    length = (int)n;
                    Expect("]");
                }
                CType elem = ParseTypeSuffix(ty);
                if (!elem.IsComplete)
                    throw Error(Cur, $"Array has incomplete element type '{elem}'");
                return CType.ArrayOf(elem, length);
            }

            return ty;
        }

        private CType ParseFunctionParams(CType ret)
        {
            if (Accept(")"))
                return CType.FunctionOf(ret, [], false, false);

            if (Cur.Is("void") && Peek2.Is(")"))
            {
                _pos += 2;
                return CType.FunctionOf(ret, [], true, false);
            }

            var ps = new List<CParam>();
            bool variadic = false;
            while (true)
            {
                if (Accept("..."))
                {
                    variadic = true;
                    Expect(")");
                    break;
                }

                DeclSpec spec = ParseDeclSpec(false);
                CType pty = ParseDeclarator(spec.Type, out CToken pname);
                if (pty.IsVoid)
                    throw Error(Cur, "Parameter has void type");
                // Array and function parameters are adjusted to pointers
                if (pty.IsArray) pty = CType.PointerTo(pty.Base);
                else if (pty.IsFunction) pty = CType.PointerTo(pty);
                ps.Add(new CParam { Name = pname?.Text, Type = pty });

                if (Accept(")")) break;
                Expect(",");
            }

            return CType.FunctionOf(ret, ps, true, variadic);
        }

        // ====================================================================== Statements

        private CNode ParseCompoundStatement(bool newScope = true)
        {
            CToken tok = Expect("{");
            var node = new CNode(CNodeKind.Block, tok) { Body = [] };
            if (newScope) EnterScope();

            while (!Accept("}"))
            {
                if (AtEof())
                    throw Error(Cur, "Expected '}' before end of file");

                if (IsTypeName(Cur) && !Peek2.Is(":"))
                {
                    DeclSpec spec = ParseDeclSpec(true);
                    if (spec.IsTypedef)
                    {
                        ParseTypedef(spec);
                        continue;
                    }
                    node.Body.Add(ParseLocalDeclaration(spec));
                    continue;
                }

                node.Body.Add(ParseStatement());
            }

            if (newScope) LeaveScope();
            return node;
        }

        private CNode ParseLocalDeclaration(DeclSpec spec)
        {
            CToken tok = Cur;
            var block = new CNode(CNodeKind.Block, tok) { Body = [] };
            bool first = true;

            while (!Accept(";"))
            {
                if (!first) Expect(",");
                first = false;

                CType ty = ParseDeclarator(spec.Type, out CToken name);
                if (name == null)
                    throw Error(Cur, "Expected an identifier in declaration");

                if (ty.IsFunction)
                {
                    DeclareFunction(ty, name, spec, false);
                    _scopes[^1][name.Text] = _globals[name.Text];
                    continue;
                }

                if (ty.IsVoid)
                    throw Error(name, $"Variable '{name.Text}' declared void");

                if (spec.IsExtern)
                {
                    if (!_globals.TryGetValue(name.Text, out CSymbol g))
                    {
                        g = new CSymbol { Name = name.Text, Kind = CSymbolKind.Global, Type = ty, Label = ".c_" + name.Text, IsExtern = true };
                        _globals[name.Text] = g;
                        _scopes[0][name.Text] = g;
                        _unit.Globals.Add(g);
                    }
                    _scopes[^1][name.Text] = g;
                    continue;
                }

                if (spec.IsStatic)
                {
                    var s = new CSymbol
                    {
                        Name = name.Text,
                        Kind = CSymbolKind.Global,
                        Type = ty,
                        Label = $".c_{_fn.Name}__{name.Text}_{++_staticCounter}",
                        IsStatic = true,
                        IsDefined = true
                    };
                    if (Accept("="))
                        ParseGlobalInitializer(s);
                    else
                        s.InitData = new byte[Math.Max(ty.Size, 0)];
                    if (!s.Type.IsComplete)
                        throw Error(name, $"Variable '{name.Text}' has incomplete type");
                    DeclareInScope(name, s);
                    _unit.Globals.Add(s);
                    continue;
                }

                CSymbol local = NewLocal(name.Text, ty);
                DeclareInScope(name, local);

                if (Accept("="))
                {
                    CNode init = ParseLocalInitializer(local, name);
                    block.Body.Add(init);
                }

                if (!local.Type.IsComplete)
                    throw Error(name, $"Variable '{name.Text}' has incomplete type '{local.Type}'");
            }

            return block;
        }

        private CNode ParseStatement()
        {
            CToken tok = Cur;

            if (Cur.Kind == CTokenKind.Identifier && Peek2.Is(":") && !IsKeyword(Cur.Text))
            {
                _pos += 2;
                if (_userLabels.ContainsKey(tok.Text))
                    throw Error(tok, $"Duplicate label '{tok.Text}'");
                string unique = NewLabel();
                _userLabels[tok.Text] = unique;
                var lbl = new CNode(CNodeKind.Label, tok) { LabelName = tok.Text, UniqueLabel = unique };
                lbl.BodyStmt = Peek("}") ? new CNode(CNodeKind.Empty, tok) : ParseStatement();
                return lbl;
            }

            if (Peek("{"))
                return ParseCompoundStatement();

            if (Accept(";"))
                return new CNode(CNodeKind.Empty, tok);

            if (tok.Kind == CTokenKind.Identifier)
            {
                switch (tok.Text)
                {
                    case "return":
                        {
                            _pos++;
                            var node = new CNode(CNodeKind.Return, tok);
                            if (!Accept(";"))
                            {
                                CNode e = ParseExpr();
                                Expect(";");
                                if (_fn.Type.Base.IsVoid)
                                    throw Error(tok, "Returning a value from a void function");
                                node.Lhs = ConvertForAssignment(e, _fn.Type.Base, tok, "return");
                            }
                            else if (!_fn.Type.Base.IsVoid && _fn.Name != "main")
                            {
                                throw Error(tok, "Non-void function must return a value");
                            }
                            return node;
                        }

                    case "if":
                        {
                            _pos++;
                            var node = new CNode(CNodeKind.If, tok);
                            Expect("(");
                            node.Cond = ToCondition(ParseExpr());
                            Expect(")");
                            node.Then = ParseStatement();
                            if (Accept("else"))
                                node.Els = ParseStatement();
                            return node;
                        }

                    case "while":
                        {
                            _pos++;
                            var node = new CNode(CNodeKind.For, tok);
                            Expect("(");
                            node.Cond = ToCondition(ParseExpr());
                            Expect(")");
                            ParseLoopBody(node);
                            return node;
                        }

                    case "do":
                        {
                            _pos++;
                            var node = new CNode(CNodeKind.Do, tok);
                            ParseLoopBody(node);
                            if (!Accept("while"))
                                throw Error(Cur, "Expected 'while' after do-statement body");
                            Expect("(");
                            node.Cond = ToCondition(ParseExpr());
                            Expect(")");
                            Expect(";");
                            return node;
                        }

                    case "for":
                        {
                            _pos++;
                            var node = new CNode(CNodeKind.For, tok);
                            Expect("(");
                            EnterScope();
                            if (IsTypeName(Cur))
                            {
                                DeclSpec spec = ParseDeclSpec(true);
                                node.Init = ParseLocalDeclaration(spec);
                            }
                            else if (!Accept(";"))
                            {
                                node.Init = new CNode(CNodeKind.ExprStmt, Cur) { Lhs = ParseExpr() };
                                Expect(";");
                            }
                            if (!Peek(";"))
                                node.Cond = ToCondition(ParseExpr());
                            Expect(";");
                            if (!Peek(")"))
                                node.Inc = ParseExpr();
                            Expect(")");
                            ParseLoopBody(node);
                            LeaveScope();
                            return node;
                        }

                    case "switch":
                        {
                            _pos++;
                            var node = new CNode(CNodeKind.Switch, tok) { Cases = [] };
                            Expect("(");
                            CNode cond = ParseExpr();
                            if (!cond.Type.IsInteger)
                                throw Error(tok, "Switch quantity is not an integer");
                            node.Cond = Cast(cond, Promote(cond.Type));
                            Expect(")");

                            CNode prevSwitch = _switch;
                            string prevBrk = _brk;
                            _switch = node;
                            node.BreakLabel = _brk = NewLabel();
                            node.Then = ParseStatement();
                            _switch = prevSwitch;
                            _brk = prevBrk;
                            return node;
                        }

                    case "case":
                        {
                            _pos++;
                            if (_switch == null)
                                throw Error(tok, "'case' label not within a switch statement");
                            long v = EvalConstExpr(ParseConditional(), "Case label must be an integer constant");
                            Expect(":");
                            v = Normalize(v, _switch.Cond.Type);
                            if (_switch.Cases.Any(c => c.CaseValue == v))
                                throw Error(tok, $"Duplicate case value {v}");
                            var node = new CNode(CNodeKind.Case, tok) { CaseValue = v, UniqueLabel = NewLabel() };
                            _switch.Cases.Add(node);
                            node.BodyStmt = Peek("}") ? new CNode(CNodeKind.Empty, tok) : ParseStatement();
                            return node;
                        }

                    case "default":
                        {
                            _pos++;
                            if (_switch == null)
                                throw Error(tok, "'default' label not within a switch statement");
                            Expect(":");
                            if (_switch.DefaultCase != null)
                                throw Error(tok, "Multiple default labels in one switch");
                            var node = new CNode(CNodeKind.Case, tok) { UniqueLabel = NewLabel() };
                            _switch.DefaultCase = node;
                            node.BodyStmt = Peek("}") ? new CNode(CNodeKind.Empty, tok) : ParseStatement();
                            return node;
                        }

                    case "break":
                        {
                            _pos++;
                            Expect(";");
                            if (_brk == null)
                                throw Error(tok, "'break' statement not within a loop or switch");
                            return new CNode(CNodeKind.Goto, tok) { UniqueLabel = _brk };
                        }

                    case "continue":
                        {
                            _pos++;
                            Expect(";");
                            if (_cont == null)
                                throw Error(tok, "'continue' statement not within a loop");
                            return new CNode(CNodeKind.Goto, tok) { UniqueLabel = _cont };
                        }

                    case "goto":
                        {
                            _pos++;
                            CToken name = ExpectIdentifier();
                            Expect(";");
                            var node = new CNode(CNodeKind.Goto, tok) { LabelName = name.Text };
                            _gotos.Add(node);
                            return node;
                        }

                    case "asm":
                    case "__asm":
                    case "__asm__":
                        {
                            _pos++;
                            while (Cur.Is("volatile") || Cur.Is("__volatile__")) _pos++;
                            Expect("(");
                            if (Cur.Kind != CTokenKind.String)
                                throw Error(Cur, "asm expects a string literal");
                            string text = "";
                            while (Cur.Kind == CTokenKind.String)
                            {
                                text += Cur.Text;
                                _pos++;
                            }
                            Expect(")");
                            Expect(";");
                            return new CNode(CNodeKind.Asm, tok) { AsmText = text };
                        }
                }
            }

            CNode expr = ParseExpr();
            Expect(";");
            return new CNode(CNodeKind.ExprStmt, tok) { Lhs = expr };
        }

        private void ParseLoopBody(CNode loop)
        {
            string prevBrk = _brk, prevCont = _cont;
            loop.BreakLabel = _brk = NewLabel();
            loop.ContinueLabel = _cont = NewLabel();
            loop.Then = ParseStatement();
            _brk = prevBrk;
            _cont = prevCont;
        }

        private static readonly HashSet<string> Keywords =
        [
            "return", "if", "else", "while", "do", "for", "switch", "case", "default", "break", "continue", "goto",
            "sizeof", "asm", "__asm", "__asm__"
        ];

        private bool IsKeyword(string s) => Keywords.Contains(s) || TypeKeywords.Contains(s);

        // ====================================================================== Initializers

        private abstract class Init { public CToken Tok; }
        private sealed class ExprInit : Init { public CNode Expr; }
        private sealed class ListInit : Init { public List<Init> Items = []; }

        private Init ParseInitializerSyntax()
        {
            CToken tok = Cur;
            if (Accept("{"))
            {
                var list = new ListInit { Tok = tok };
                while (!Accept("}"))
                {
                    if (Peek(".") || Peek("["))
                        throw Error(Cur, "Designated initializers are not supported");
                    list.Items.Add(ParseInitializerSyntax());
                    if (!Accept(","))
                    {
                        Expect("}");
                        break;
                    }
                }
                return list;
            }
            return new ExprInit { Tok = tok, Expr = ParseAssign() };
        }

        /// <summary>Flattens a (possibly brace-elided) initializer against a type into (offset, type, expr) leaves.</summary>
        private void FlattenInit(CType ty, Init init, int baseOffset, List<(int Offset, CType Type, CNode Expr)> leaves, ref int cursorIndex, List<Init> siblings)
        {
            // Helper consumes either the explicit init or, with brace elision, items from siblings
            if (ty.IsArray || ty.IsStruct)
            {
                // char array from string literal
                if (ty.IsArray && ty.Base.Kind == CTypeKind.Char && init is ExprInit se && se.Expr.Kind == CNodeKind.StrLabel)
                {
                    byte[] bytes = StringBytes(se.Expr);
                    if (ty.Length < 0)
                        ty.Length = bytes.Length;   // char s[] = "abc" has 4 elements (caller copied the type)
                    else if (bytes.Length - 1 > ty.Length)
                        throw Error(se.Tok, "Initializer string is too long for the array");
                    int n = Math.Min(ty.Length, bytes.Length);
                    for (int i = 0; i < n; i++)
                        leaves.Add((baseOffset + i, ty.Base, Num(bytes[i], se.Tok)));
                    return;
                }

                if (init is ListInit li)
                {
                    int idx = 0;
                    FillAggregate(ty, li.Items, ref idx, baseOffset, leaves, li.Tok);
                    if (idx < li.Items.Count)
                        throw Error(li.Items[idx].Tok, "Excess elements in initializer");
                    return;
                }

                if (siblings != null)
                {
                    // Brace elision: consume as many sibling items as the aggregate needs
                    FillAggregate(ty, siblings, ref cursorIndex, baseOffset, leaves, init.Tok);
                    cursorIndex--; // caller increments
                    return;
                }

                // Struct initialized from an expression of the same struct type (locals only)
                if (ty.IsStruct && init is ExprInit ei && ei.Expr.Type.IsStruct)
                {
                    leaves.Add((baseOffset, ty, ei.Expr));
                    return;
                }

                throw Error(init.Tok, $"Invalid initializer for type '{ty}'");
            }

            if (init is ListInit scalarList)
            {
                if (scalarList.Items.Count == 0)
                    throw Error(scalarList.Tok, "Empty scalar initializer");
                if (scalarList.Items.Count > 1)
                    throw Error(scalarList.Items[1].Tok, "Excess elements in scalar initializer");
                int dummy = 0;
                FlattenInit(ty, scalarList.Items[0], baseOffset, leaves, ref dummy, null);
                return;
            }

            leaves.Add((baseOffset, ty, ((ExprInit)init).Expr));
        }

        private void FillAggregate(CType ty, List<Init> items, ref int idx, int baseOffset, List<(int, CType, CNode)> leaves, CToken tok)
        {
            if (ty.IsArray)
            {
                int i = 0;
                while (idx < items.Count && (ty.Length < 0 || i < ty.Length))
                {
                    int cursor = idx;
                    FlattenInit(ty.Base, items[idx], baseOffset + i * ty.Base.Size, leaves, ref cursor, items);
                    idx = cursor + 1;
                    i++;
                }
                if (ty.Length < 0)
                    ty.Length = i; // Completes "int a[] = {...}" (caller copies type first)
                return;
            }

            foreach (CMember m in ty.Struct.Members)
            {
                if (idx >= items.Count) break;
                int cursor = idx;
                FlattenInit(m.Type, items[idx], baseOffset + m.Offset, leaves, ref cursor, items);
                idx = cursor + 1;
                if (ty.Struct.IsUnion) break;
            }
        }

        private List<(int Offset, CType Type, CNode Expr)> ParseInitializerFor(ref CType ty, CToken nameTok)
        {
            Init init = ParseInitializerSyntax();
            if (ty.IsArray && ty.Length < 0)
                ty = ty.Copy(); // Length gets completed by flattening
            var leaves = new List<(int, CType, CNode)>();
            int cursor = 0;
            FlattenInit(ty, init, 0, leaves, ref cursor, null);
            if (ty.IsArray && ty.Length < 0)
                throw Error(nameTok, "Array size missing in declaration");
            return leaves;
        }

        private CNode ParseLocalInitializer(CSymbol local, CToken nameTok)
        {
            CType ty = local.Type;
            List<(int Offset, CType Type, CNode Expr)> leaves = ParseInitializerFor(ref ty, nameTok);
            local.Type = ty;

            var block = new CNode(CNodeKind.Block, nameTok) { Body = [] };
            if (ty.IsArray || ty.IsStruct)
            {
                block.Body.Add(new CNode(CNodeKind.ExprStmt, nameTok)
                {
                    Lhs = new CNode(CNodeKind.MemZero, nameTok) { Var = local, Type = CType.Void }
                });
            }

            foreach (var (offset, lty, expr) in leaves)
            {
                // *(lty*)((char*)&local + offset) = expr
                CNode addr = new CNode(CNodeKind.Addr, nameTok) { Lhs = VarNode(local, nameTok), Type = CType.PointerTo(local.Type) };
                CNode bytePtr = Cast(addr, CType.PointerTo(CType.Char));
                CNode at = offset == 0 ? bytePtr : NewAddRaw(bytePtr, Num(offset, nameTok), CType.PointerTo(CType.Char));
                CNode target = Deref(Cast(at, CType.PointerTo(lty)), nameTok);
                block.Body.Add(new CNode(CNodeKind.ExprStmt, nameTok) { Lhs = NewAssign(target, expr, nameTok) });
            }
            return block;
        }

        private void ParseGlobalInitializer(CSymbol sym)
        {
            CToken tok = Cur;
            CType ty = sym.Type;
            List<(int Offset, CType Type, CNode Expr)> leaves = ParseInitializerFor(ref ty, tok);
            sym.Type = ty;

            byte[] data = new byte[Math.Max(ty.Size, 0)];
            sym.Relocations = [];
            foreach (var (offset, lty, expr) in leaves)
            {
                if (lty.IsStruct)
                    throw Error(expr.Tok, "Initializer element is not a compile-time constant");
                CNode e = ConvertForAssignment(expr, lty, expr.Tok, "initialization");
                long value = EvalConstWithLabel(e, out string label, "Initializer element is not a compile-time constant");
                if (label != null)
                {
                    if (lty.Size != 4)
                        throw Error(expr.Tok, "Address constant does not fit in the initialized type");
                    sym.Relocations.Add(new CRelocation { Offset = offset, TargetLabel = label, Addend = value });
                    continue;
                }
                WriteBigEndian(data, offset, lty.Size, value);
            }
            sym.InitData = data;
        }

        private static void WriteBigEndian(byte[] data, int offset, int size, long value)
        {
            for (int i = 0; i < size; i++)
                data[offset + i] = (byte)(value >> (8 * (size - 1 - i)));
        }

        // ====================================================================== Expressions

        private CNode ParseExpr()
        {
            CNode node = ParseAssign();
            while (Peek(","))
            {
                CToken tok = Cur;
                _pos++;
                CNode rhs = ParseAssign();
                node = new CNode(CNodeKind.Comma, tok) { Lhs = node, Rhs = rhs, Type = rhs.Type.Decay() };
            }
            return node;
        }

        private static readonly Dictionary<string, CNodeKind> CompoundOps = new()
        {
            ["+="] = CNodeKind.Add, ["-="] = CNodeKind.Sub, ["*="] = CNodeKind.Mul, ["/="] = CNodeKind.Div,
            ["%="] = CNodeKind.Mod, ["&="] = CNodeKind.BitAnd, ["|="] = CNodeKind.BitOr, ["^="] = CNodeKind.BitXor,
            ["<<="] = CNodeKind.Shl, [">>="] = CNodeKind.Shr
        };

        private CNode ParseAssign()
        {
            CNode lhs = ParseConditional();
            CToken tok = Cur;

            if (Accept("="))
                return NewAssign(lhs, ParseAssign(), tok);

            if (tok.Kind == CTokenKind.Punct && CompoundOps.TryGetValue(tok.Text, out CNodeKind op))
            {
                _pos++;
                return NewCompoundAssign(lhs, op, ParseAssign(), tok);
            }

            return lhs;
        }

        private CNode ParseConditional()
        {
            CNode cond = ParseBinary(0);
            CToken tok = Cur;
            if (!Accept("?"))
                return cond;

            CNode a = ParseExpr();
            Expect(":");
            CNode b = ParseConditional();

            var node = new CNode(CNodeKind.Cond, tok) { Cond = ToCondition(cond) };
            CType ta = a.Type.Decay(), tb = b.Type.Decay();

            if (ta.IsVoid || tb.IsVoid)
            {
                node.Type = CType.Void;
            }
            else if (ta.IsInteger && tb.IsInteger)
            {
                CType common = Common(ta, tb);
                a = Cast(a, common);
                b = Cast(b, common);
                node.Type = common;
            }
            else if (ta.IsPointer && tb.IsPointer)
            {
                node.Type = ta.Base.IsVoid ? tb : ta;
            }
            else if (ta.IsPointer && IsNullConstant(b))
            {
                node.Type = ta;
                b = Cast(b, ta);
            }
            else if (tb.IsPointer && IsNullConstant(a))
            {
                node.Type = tb;
                a = Cast(a, tb);
            }
            else if (ta.IsStruct && tb.IsStruct && ReferenceEquals(ta.Struct, tb.Struct))
            {
                node.Type = ta;
            }
            else
            {
                throw Error(tok, $"Type mismatch in conditional expression ('{ta}' and '{tb}')");
            }

            node.Then = a;
            node.Els = b;
            return node;
        }

        private static readonly string[][] BinaryLevels =
        [
            ["||"], ["&&"], ["|"], ["^"], ["&"], ["==", "!="], ["<", ">", "<=", ">="], ["<<", ">>"], ["+", "-"], ["*", "/", "%"]
        ];

        private CNode ParseBinary(int level)
        {
            if (level == BinaryLevels.Length)
                return ParseCast();

            CNode lhs = ParseBinary(level + 1);
            while (true)
            {
                CToken tok = Cur;
                if (tok.Kind != CTokenKind.Punct || !BinaryLevels[level].Contains(tok.Text))
                    return lhs;
                _pos++;
                CNode rhs = ParseBinary(level + 1);
                lhs = NewBinaryOp(tok.Text, lhs, rhs, tok);
            }
        }

        private CNode NewBinaryOp(string op, CNode lhs, CNode rhs, CToken tok)
        {
            switch (op)
            {
                case "+": return NewAdd(lhs, rhs, tok);
                case "-": return NewSub(lhs, rhs, tok);
                case "*": return NewArith(CNodeKind.Mul, lhs, rhs, tok);
                case "/": return NewArith(CNodeKind.Div, lhs, rhs, tok);
                case "%": return NewArith(CNodeKind.Mod, lhs, rhs, tok);
                case "&": return NewArith(CNodeKind.BitAnd, lhs, rhs, tok);
                case "|": return NewArith(CNodeKind.BitOr, lhs, rhs, tok);
                case "^": return NewArith(CNodeKind.BitXor, lhs, rhs, tok);
                case "<<": return NewShift(CNodeKind.Shl, lhs, rhs, tok);
                case ">>": return NewShift(CNodeKind.Shr, lhs, rhs, tok);
                case "==": return NewCompare(CNodeKind.Eq, lhs, rhs, tok);
                case "!=": return NewCompare(CNodeKind.Ne, lhs, rhs, tok);
                case "<": return NewCompare(CNodeKind.Lt, lhs, rhs, tok);
                case "<=": return NewCompare(CNodeKind.Le, lhs, rhs, tok);
                case ">": return NewCompare(CNodeKind.Lt, rhs, lhs, tok);
                case ">=": return NewCompare(CNodeKind.Le, rhs, lhs, tok);
                case "&&":
                    return new CNode(CNodeKind.LogAnd, tok) { Lhs = ToCondition(lhs), Rhs = ToCondition(rhs), Type = CType.Int };
                case "||":
                    return new CNode(CNodeKind.LogOr, tok) { Lhs = ToCondition(lhs), Rhs = ToCondition(rhs), Type = CType.Int };
            }
            throw Error(tok, $"Unknown operator '{op}'");
        }

        private CNode ParseCast()
        {
            if (Peek("(") && IsTypeName(Peek2))
            {
                CToken tok = Cur;
                int save = _pos;
                _pos++;
                CType ty = ParseAbstractTypeName();
                Expect(")");

                if (Peek("{"))
                    throw Error(Cur, "Compound literals are not supported");

                CNode operand = ParseCast();
                return ExplicitCast(operand, ty, tok);
            }
            return ParseUnary();
        }

        private CNode ParseUnary()
        {
            CToken tok = Cur;

            if (Accept("+"))
            {
                CNode e = RValue(ParseCast(), tok);
                if (!e.Type.IsInteger) throw Error(tok, "Invalid operand to unary '+'");
                return Cast(e, Promote(e.Type));
            }

            if (Accept("-"))
            {
                CNode e = RValue(ParseCast(), tok);
                if (!e.Type.IsInteger) throw Error(tok, "Invalid operand to unary '-'");
                CType t = Promote(e.Type);
                return new CNode(CNodeKind.Neg, tok) { Lhs = Cast(e, t), Type = t };
            }

            if (Accept("~"))
            {
                CNode e = RValue(ParseCast(), tok);
                if (!e.Type.IsInteger) throw Error(tok, "Invalid operand to '~'");
                CType t = Promote(e.Type);
                return new CNode(CNodeKind.BitNot, tok) { Lhs = Cast(e, t), Type = t };
            }

            if (Accept("!"))
            {
                CNode e = ToCondition(ParseCast());
                return new CNode(CNodeKind.LogNot, tok) { Lhs = e, Type = CType.Int };
            }

            if (Accept("&"))
            {
                CNode e = ParseCast();
                return AddressOf(e, tok);
            }

            if (Accept("*"))
            {
                CNode e = ParseCast();
                return Deref(e, tok);
            }

            if (Accept("++"))
                return NewCompoundAssign(ParseUnary(), CNodeKind.Add, Num(1, tok), tok);

            if (Accept("--"))
                return NewCompoundAssign(ParseUnary(), CNodeKind.Sub, Num(1, tok), tok);

            if (Accept("sizeof"))
            {
                if (Peek("(") && IsTypeName(Peek2))
                {
                    _pos++;
                    CType ty = ParseAbstractTypeName();
                    Expect(")");
                    return SizeOf(ty, tok);
                }
                CNode e = ParseUnary();
                return SizeOf(e.Type, tok);
            }

            return ParsePostfix();
        }

        private CNode SizeOf(CType ty, CToken tok)
        {
            if (ty.IsFunction) throw Error(tok, "Invalid application of 'sizeof' to a function type");
            if (!ty.IsComplete && !ty.IsVoid) throw Error(tok, $"Invalid application of 'sizeof' to incomplete type '{ty}'");
            return new CNode(CNodeKind.Num, tok) { Value = ty.Size, Type = CType.UInt };
        }

        private CNode ParsePostfix()
        {
            CNode node = ParsePrimary();

            while (true)
            {
                CToken tok = Cur;

                if (Accept("["))
                {
                    CNode idx = ParseExpr();
                    Expect("]");
                    node = Deref(NewAdd(node, idx, tok), tok);
                    continue;
                }

                if (Accept("("))
                {
                    node = ParseCall(node, tok);
                    continue;
                }

                if (Accept("."))
                {
                    node = MemberAccess(node, ExpectIdentifier(), tok);
                    continue;
                }

                if (Accept("->"))
                {
                    node = MemberAccess(Deref(node, tok), ExpectIdentifier(), tok);
                    continue;
                }

                if (Accept("++"))
                {
                    node = NewPostIncDec(node, CNodeKind.Add, tok);
                    continue;
                }

                if (Accept("--"))
                {
                    node = NewPostIncDec(node, CNodeKind.Sub, tok);
                    continue;
                }

                return node;
            }
        }

        private CNode ParseCall(CNode fn, CToken tok)
        {
            CType fty = fn.Type.IsFunction ? fn.Type : fn.Type.IsPointer && fn.Type.Base.IsFunction ? fn.Type.Base : null;
            if (fty == null)
                throw Error(tok, "Called object is not a function or function pointer");

            var args = new List<CNode>();
            if (!Accept(")"))
            {
                while (true)
                {
                    args.Add(ParseAssign());
                    if (Accept(")")) break;
                    Expect(",");
                }
            }

            string name = fn.Kind == CNodeKind.Var ? fn.Var.Name : "function";

            if (fty.HasPrototype)
            {
                if (args.Count < fty.Params.Count || (!fty.Variadic && args.Count > fty.Params.Count))
                    throw Error(tok, $"Wrong number of arguments to '{name}': expected {fty.Params.Count}, got {args.Count}");
            }

            for (int i = 0; i < args.Count; i++)
            {
                CNode a = args[i];
                if (a.Type.IsStruct)
                    throw Error(a.Tok, "Passing structs by value is not supported; pass a pointer instead");
                if (fty.HasPrototype && i < fty.Params.Count)
                    args[i] = ConvertForAssignment(a, fty.Params[i].Type, a.Tok, $"argument {i + 1} of '{name}'");
                else
                {
                    a = RValue(a, a.Tok);
                    args[i] = a.Type.IsInteger ? Cast(a, Promote(a.Type)) : a;
                }
            }

            var node = new CNode(CNodeKind.Call, tok)
            {
                Lhs = fn,
                Args = args,
                FuncType = fty,
                Type = fty.Base
            };

            if (fn.Kind == CNodeKind.Var && fn.Var.Kind == CSymbolKind.Function)
            {
                node.DirectCallee = fn.Var;
                _fn?.CalledFunctions.Add(fn.Var.Label);
            }
            return node;
        }

        private CNode ParsePrimary()
        {
            CToken tok = Cur;

            if (Accept("("))
            {
                if (Peek("{"))
                    throw Error(tok, "Statement expressions are not supported");
                CNode e = ParseExpr();
                Expect(")");
                return e;
            }

            if (tok.Kind == CTokenKind.Number)
            {
                _pos++;
                if (!CConstExpr.TryParseInt(tok.Text, out uint v, out bool uns, out string err))
                    throw Error(tok, err);
                return new CNode(CNodeKind.Num, tok) { Value = uns ? v : (int)v, Type = uns ? CType.UInt : CType.Int };
            }

            if (tok.Kind == CTokenKind.Char)
            {
                _pos++;
                int c = tok.Text.Length > 0 ? (sbyte)(byte)tok.Text[0] : 0;
                return new CNode(CNodeKind.Num, tok) { Value = c, Type = CType.Int };
            }

            if (tok.Kind == CTokenKind.String)
            {
                // Adjacent string literals concatenate
                string s = "";
                while (Cur.Kind == CTokenKind.String)
                {
                    s += Cur.Text;
                    _pos++;
                }
                byte[] bytes = new byte[s.Length + 1];
                for (int i = 0; i < s.Length; i++)
                    bytes[i] = (byte)s[i];
                string label = $".c__str{_unit.Strings.Count}";
                _unit.Strings.Add((label, bytes));
                return new CNode(CNodeKind.StrLabel, tok) { Label = label, Type = CType.ArrayOf(CType.Char, bytes.Length) };
            }

            if (tok.Kind == CTokenKind.Identifier)
            {
                if (IsKeyword(tok.Text))
                    throw Error(tok, $"Expected an expression, got '{tok.Text}'");
                _pos++;

                CSymbol sym = Lookup(tok.Text);
                if (sym == null)
                {
                    if (Peek("("))
                        throw Error(tok, $"Implicit declaration of function '{tok.Text}' (declare it or include the right header)");
                    throw Error(tok, $"Undeclared identifier '{tok.Text}'");
                }

                switch (sym.Kind)
                {
                    case CSymbolKind.EnumConst:
                        return new CNode(CNodeKind.Num, tok) { Value = sym.EnumValue, Type = CType.Int };
                    case CSymbolKind.Typedef:
                        throw Error(tok, $"Unexpected type name '{tok.Text}'");
                    case CSymbolKind.Function:
                        _fn?.CalledFunctions.Add(sym.Label);
                        sym.IsReferenced = true;
                        return VarNode(sym, tok);
                    case CSymbolKind.Global:
                        _fn?.CalledFunctions.Add(sym.Label);
                        sym.IsReferenced = true;
                        return VarNode(sym, tok);
                    default:
                        return VarNode(sym, tok);
                }
            }

            throw Error(tok, $"Expected an expression, got '{tok}'");
        }

        // ====================================================================== Typed node construction

        private static CNode VarNode(CSymbol sym, CToken tok) => new(CNodeKind.Var, tok) { Var = sym, Type = sym.Type };

        private static CNode Num(long v, CToken tok) => new(CNodeKind.Num, tok) { Value = v, Type = CType.Int };

        /// <summary>Integer promotion: everything narrower than int becomes int.</summary>
        private static CType Promote(CType t) => t.Kind == CTypeKind.Int ? (t.IsEnum ? CType.Int : t) : CType.Int;

        /// <summary>Usual arithmetic conversions for 32-bit int targets.</summary>
        private static CType Common(CType a, CType b)
        {
            CType pa = Promote(a), pb = Promote(b);
            return pa.Unsigned || pb.Unsigned ? CType.UInt : CType.Int;
        }

        private CNode RValue(CNode n, CToken tok)
        {
            if (n.Type.IsVoid)
                throw Error(tok, "Void value not ignored as it ought to be");
            if (n.Type.IsStruct)
                throw Error(tok, "Struct value used where a scalar is required");
            return n;
        }

        private CNode ToCondition(CNode n)
        {
            CType t = n.Type.Decay();
            if (!t.IsScalar)
                throw Error(n.Tok, $"Used '{n.Type}' where a scalar condition is required");
            return n;
        }

        internal static CNode Cast(CNode n, CType to)
        {
            CType from = n.Type.Decay();
            if (CType.SameType(from, to) && !to.IsBool)
                return n;
            // Folding constants keeps constant expressions (e.g. initializers) simple
            if (n.Kind == CNodeKind.Num && to.IsInteger)
                return new CNode(CNodeKind.Num, n.Tok) { Value = Normalize(n.Value, to), Type = to };
            return new CNode(CNodeKind.Cast, n.Tok) { Lhs = n, Type = to };
        }

        internal static long Normalize(long v, CType t)
        {
            if (t.IsBool) return v != 0 ? 1 : 0;
            return t.Size switch
            {
                1 => t.Unsigned ? (byte)v : (sbyte)v,
                2 => t.Unsigned ? (ushort)v : (short)v,
                _ => t.Unsigned || t.IsPointer ? (uint)v : (int)v
            };
        }

        private CNode ExplicitCast(CNode e, CType to, CToken tok)
        {
            if (to.IsVoid)
                return new CNode(CNodeKind.Cast, tok) { Lhs = e, Type = CType.Void };
            if (to.IsStruct || to.IsArray || to.IsFunction)
                throw Error(tok, $"Cannot cast to '{to}'");
            CType from = e.Type.Decay();
            if (!from.IsScalar)
                throw Error(tok, $"Cannot cast '{e.Type}' to '{to}'");
            return Cast(e, to);
        }

        private static bool IsNullConstant(CNode n) =>
            n.Kind == CNodeKind.Num && n.Value == 0 ||
            n.Kind == CNodeKind.Cast && n.Type.IsPointer && n.Type.Base.IsVoid && IsNullConstant(n.Lhs);

        private CNode ConvertForAssignment(CNode e, CType to, CToken tok, string context)
        {
            CType from = e.Type.Decay();

            if (to.IsStruct)
            {
                if (from.IsStruct && ReferenceEquals(from.Struct, to.Struct))
                    return e;
                throw Error(tok, $"Incompatible types in {context}: '{from}' to '{to}'");
            }

            if (from.IsVoid)
                throw Error(tok, "Void value not ignored as it ought to be");
            if (from.IsStruct)
                throw Error(tok, $"Incompatible types in {context}: '{from}' to '{to}'");

            if (to.IsPointer)
            {
                if (from.IsPointer)
                {
                    bool ok = to.Base.IsVoid || from.Base.IsVoid || CType.SameType(to.Base, from.Base) ||
                              (to.Base.IsInteger && from.Base.IsInteger && to.Base.Size == from.Base.Size);
                    if (!ok)
                        throw Error(tok, $"Incompatible pointer types in {context}: '{from}' to '{to}'");
                    return Cast(e, to);
                }
                if (IsNullConstant(e))
                    return Cast(e, to);
                if (from.IsInteger)
                    throw Error(tok, $"Making a pointer from an integer without a cast in {context}");
            }

            if (to.IsInteger && from.IsPointer && !to.IsBool)
                throw Error(tok, $"Making an integer from a pointer without a cast in {context}");

            return Cast(e, to);
        }

        private CNode Deref(CNode e, CToken tok)
        {
            CType t = e.Type.Decay();
            if (!t.IsPointer)
                throw Error(tok, $"Cannot dereference non-pointer type '{e.Type}'");
            if (t.Base.IsFunction)
                return e; // *fp is the function itself
            if (t.Base.IsVoid)
                throw Error(tok, "Dereferencing a 'void *' pointer");
            return new CNode(CNodeKind.Deref, tok) { Lhs = e, Type = t.Base };
        }

        private CNode AddressOf(CNode e, CToken tok)
        {
            if (e.Kind == CNodeKind.Var && e.Var.Kind == CSymbolKind.Function)
                return new CNode(CNodeKind.Addr, tok) { Lhs = e, Type = CType.PointerTo(e.Type) };
            if (!IsLValue(e))
                throw Error(tok, "Cannot take the address of an rvalue");
            return new CNode(CNodeKind.Addr, tok) { Lhs = e, Type = CType.PointerTo(e.Type) };
        }

        private static bool IsLValue(CNode e) =>
            e.Kind switch
            {
                CNodeKind.Var => e.Var.Kind != CSymbolKind.Function,
                CNodeKind.Deref => true,
                CNodeKind.Member => IsLValue(e.Lhs),
                CNodeKind.StrLabel => true,
                _ => false
            };

        private CNode MemberAccess(CNode obj, CToken name, CToken tok)
        {
            if (!obj.Type.IsStruct)
                throw Error(tok, $"Member reference base type '{obj.Type}' is not a struct or union");
            if (!obj.Type.Struct.Complete)
                throw Error(tok, $"Incomplete type '{obj.Type}'");
            CMember m = obj.Type.Struct.Find(name.Text);
            if (m == null)
                throw Error(name, $"No member named '{name.Text}' in '{obj.Type}'");
            return new CNode(CNodeKind.Member, tok) { Lhs = obj, Member = m, Type = m.Type };
        }

        private CNode NewArith(CNodeKind kind, CNode lhs, CNode rhs, CToken tok)
        {
            CType lt = RValue(lhs, tok).Type.Decay(), rt = RValue(rhs, tok).Type.Decay();
            if (!lt.IsInteger || !rt.IsInteger)
                throw Error(tok, $"Invalid operands to binary operator ('{lt}' and '{rt}')");
            CType common = Common(lt, rt);
            if ((kind == CNodeKind.Div || kind == CNodeKind.Mod) && rhs.Kind == CNodeKind.Num && rhs.Value == 0)
                throw Error(tok, "Division by zero");
            return new CNode(kind, tok) { Lhs = Cast(lhs, common), Rhs = Cast(rhs, common), Type = common };
        }

        private CNode NewShift(CNodeKind kind, CNode lhs, CNode rhs, CToken tok)
        {
            CType lt = RValue(lhs, tok).Type.Decay(), rt = RValue(rhs, tok).Type.Decay();
            if (!lt.IsInteger || !rt.IsInteger)
                throw Error(tok, $"Invalid operands to shift ('{lt}' and '{rt}')");
            CType t = Promote(lt);
            return new CNode(kind, tok) { Lhs = Cast(lhs, t), Rhs = Cast(rhs, CType.Int), Type = t };
        }

        private CNode NewCompare(CNodeKind kind, CNode lhs, CNode rhs, CToken tok)
        {
            CType lt = RValue(lhs, tok).Type.Decay(), rt = RValue(rhs, tok).Type.Decay();
            if (lt.IsInteger && rt.IsInteger)
            {
                CType common = Common(lt, rt);
                return new CNode(kind, tok) { Lhs = Cast(lhs, common), Rhs = Cast(rhs, common), Type = CType.Int };
            }
            if (lt.IsPointer && rt.IsPointer)
                return new CNode(kind, tok) { Lhs = Cast(lhs, CType.UInt), Rhs = Cast(rhs, CType.UInt), Type = CType.Int };
            if ((lt.IsPointer && IsNullConstant(rhs)) || (rt.IsPointer && IsNullConstant(lhs)))
                return new CNode(kind, tok) { Lhs = Cast(lhs, CType.UInt), Rhs = Cast(rhs, CType.UInt), Type = CType.Int };
            if (lt.IsPointer && rt.IsInteger || lt.IsInteger && rt.IsPointer)
                throw Error(tok, "Comparison between pointer and integer");
            throw Error(tok, $"Invalid operands to comparison ('{lt}' and '{rt}')");
        }

        private CNode NewAddRaw(CNode ptr, CNode offset, CType resultType) =>
            new(CNodeKind.Add, ptr.Tok) { Lhs = Cast(ptr, resultType), Rhs = Cast(offset, CType.Int), Type = resultType };

        private CNode Scale(CNode idx, int size, CToken tok)
        {
            idx = Cast(idx, CType.Int);
            if (size == 1) return idx;
            if (idx.Kind == CNodeKind.Num)
                return new CNode(CNodeKind.Num, tok) { Value = (int)(idx.Value * size), Type = CType.Int };
            return new CNode(CNodeKind.Mul, tok) { Lhs = idx, Rhs = Num(size, tok), Type = CType.Int };
        }

        private CNode NewAdd(CNode lhs, CNode rhs, CToken tok)
        {
            CType lt = RValue(lhs, tok).Type.Decay(), rt = RValue(rhs, tok).Type.Decay();

            if (lt.IsInteger && rt.IsInteger)
                return NewArith(CNodeKind.Add, lhs, rhs, tok);

            if (lt.IsInteger && rt.IsPointer)
            {
                (lhs, rhs) = (rhs, lhs);
                (lt, rt) = (rt, lt);
            }

            if (lt.IsPointer && rt.IsInteger)
            {
                if (lt.Base.IsFunction || !lt.Base.IsComplete && !lt.Base.IsVoid)
                    throw Error(tok, $"Arithmetic on a pointer to an incomplete type '{lt.Base}'");
                return new CNode(CNodeKind.Add, tok) { Lhs = Cast(lhs, lt), Rhs = Scale(rhs, lt.Base.Size, tok), Type = lt };
            }

            throw Error(tok, $"Invalid operands to '+' ('{lt}' and '{rt}')");
        }

        private CNode NewSub(CNode lhs, CNode rhs, CToken tok)
        {
            CType lt = RValue(lhs, tok).Type.Decay(), rt = RValue(rhs, tok).Type.Decay();

            if (lt.IsInteger && rt.IsInteger)
                return NewArith(CNodeKind.Sub, lhs, rhs, tok);

            if (lt.IsPointer && rt.IsInteger)
            {
                if (lt.Base.IsFunction || !lt.Base.IsComplete && !lt.Base.IsVoid)
                    throw Error(tok, $"Arithmetic on a pointer to an incomplete type '{lt.Base}'");
                return new CNode(CNodeKind.Sub, tok) { Lhs = Cast(lhs, lt), Rhs = Scale(rhs, lt.Base.Size, tok), Type = lt };
            }

            if (lt.IsPointer && rt.IsPointer)
            {
                if (!CType.SameType(lt.Base, rt.Base))
                    throw Error(tok, $"Subtracting incompatible pointer types ('{lt}' and '{rt}')");
                var diff = new CNode(CNodeKind.Sub, tok) { Lhs = Cast(lhs, CType.Int), Rhs = Cast(rhs, CType.Int), Type = CType.Int };
                int size = lt.Base.Size;
                if (size == 1) return diff;
                return new CNode(CNodeKind.Div, tok) { Lhs = diff, Rhs = Num(size, tok), Type = CType.Int };
            }

            throw Error(tok, $"Invalid operands to '-' ('{lt}' and '{rt}')");
        }

        private CNode NewAssign(CNode lhs, CNode rhs, CToken tok)
        {
            if (!IsLValue(lhs) || lhs.Kind == CNodeKind.StrLabel)
                throw Error(tok, "Expression is not assignable");
            if (lhs.Type.IsArray)
                throw Error(tok, "Array type is not assignable");
            if (lhs.Type.IsStruct && !lhs.Type.Struct.Complete)
                throw Error(tok, "Assignment to incomplete type");
            CNode value = ConvertForAssignment(rhs, lhs.Type, tok, "assignment");
            return new CNode(CNodeKind.Assign, tok) { Lhs = lhs, Rhs = value, Type = lhs.Type };
        }

        private static bool IsSimpleVar(CNode n) => n.Kind == CNodeKind.Var && n.Var.Kind != CSymbolKind.Function;

        /// <summary>A op= B  becomes  A = A op B for plain variables, otherwise  (tmp = &amp;A, *tmp = *tmp op B).</summary>
        private CNode NewCompoundAssign(CNode lhs, CNodeKind op, CNode rhs, CToken tok)
        {
            if (!IsLValue(lhs) || lhs.Type.IsArray || lhs.Type.IsStruct)
                throw Error(tok, "Expression is not assignable");

            if (IsSimpleVar(lhs))
                return NewAssign(lhs, NewOpFor(op, VarNode(lhs.Var, tok), rhs, tok), tok);

            CSymbol tmp = NewLocal("", CType.PointerTo(lhs.Type));
            CNode setTmp = new(CNodeKind.Assign, tok) { Lhs = VarNode(tmp, tok), Rhs = AddressOf(lhs, tok), Type = tmp.Type };
            CNode target() => new(CNodeKind.Deref, tok) { Lhs = VarNode(tmp, tok), Type = lhs.Type };
            CNode assign = NewAssign(target(), NewOpFor(op, target(), rhs, tok), tok);
            return new CNode(CNodeKind.Comma, tok) { Lhs = setTmp, Rhs = assign, Type = assign.Type };
        }

        private CNode NewOpFor(CNodeKind op, CNode a, CNode b, CToken tok) =>
            op switch
            {
                CNodeKind.Add => NewAdd(a, b, tok),
                CNodeKind.Sub => NewSub(a, b, tok),
                CNodeKind.Shl or CNodeKind.Shr => NewShift(op, a, b, tok),
                _ => NewArith(op, a, b, tok)
            };

        /// <summary>A++  becomes  (typeof A)((A += 1) - 1).</summary>
        private CNode NewPostIncDec(CNode lhs, CNodeKind op, CToken tok)
        {
            if (!IsLValue(lhs) || !lhs.Type.IsScalar)
                throw Error(tok, "Expression is not assignable");
            CNode updated = NewCompoundAssign(lhs, op, Num(1, tok), tok);
            CNode undone = op == CNodeKind.Add ? NewSub(updated, Num(1, tok), tok) : NewAdd(updated, Num(1, tok), tok);
            return Cast(undone, lhs.Type);
        }

        // ====================================================================== Constant evaluation

        private long EvalConstExpr(CNode n, string message = "Expected an integer constant expression")
        {
            long v = EvalConstWithLabel(n, out string label, message);
            if (label != null)
                throw Error(n.Tok, message);
            return v;
        }

        /// <summary>Evaluates a constant expression. Address constants return the label plus the value as addend.</summary>
        private long EvalConstWithLabel(CNode n, out string label, string message)
        {
            label = null;
            switch (n.Kind)
            {
                case CNodeKind.Num:
                    return n.Value;

                case CNodeKind.StrLabel:
                    label = n.Label;
                    return 0;

                case CNodeKind.Var:
                    if (n.Var.Kind == CSymbolKind.Function || n.Var.Kind == CSymbolKind.Global && (n.Type.IsArray))
                    {
                        label = n.Var.Label;
                        return 0;
                    }
                    break;

                case CNodeKind.Addr:
                    return EvalAddress(n.Lhs, out label, message);

                case CNodeKind.Cast:
                    {
                        long v = EvalConstWithLabel(n.Lhs, out label, message);
                        if (label != null)
                        {
                            if (n.Type.Size != 4) throw Error(n.Tok, message);
                            return v;
                        }
                        return n.Type.IsVoid ? 0 : Normalize(v, n.Type);
                    }

                case CNodeKind.Add:
                case CNodeKind.Sub:
                    {
                        long a = EvalConstWithLabel(n.Lhs, out string la, message);
                        long b = EvalConstWithLabel(n.Rhs, out string lb, message);
                        if (lb != null) throw Error(n.Tok, message);
                        label = la;
                        long r = n.Kind == CNodeKind.Add ? a + b : a - b;
                        return la != null ? r : Normalize(r, n.Type);
                    }

                case CNodeKind.Cond:
                    {
                        long c = EvalConstExpr(n.Cond, message);
                        return EvalConstWithLabel(c != 0 ? n.Then : n.Els, out label, message);
                    }

                case CNodeKind.Neg: return Normalize(-EvalConstExpr(n.Lhs, message), n.Type);
                case CNodeKind.BitNot: return Normalize(~EvalConstExpr(n.Lhs, message), n.Type);
                case CNodeKind.LogNot: return EvalConstExpr(n.Lhs, message) == 0 ? 1 : 0;
                case CNodeKind.LogAnd: return EvalConstExpr(n.Lhs, message) != 0 && EvalConstExpr(n.Rhs, message) != 0 ? 1 : 0;
                case CNodeKind.LogOr: return EvalConstExpr(n.Lhs, message) != 0 || EvalConstExpr(n.Rhs, message) != 0 ? 1 : 0;

                case CNodeKind.Mul:
                case CNodeKind.Div:
                case CNodeKind.Mod:
                case CNodeKind.BitAnd:
                case CNodeKind.BitOr:
                case CNodeKind.BitXor:
                case CNodeKind.Shl:
                case CNodeKind.Shr:
                case CNodeKind.Eq:
                case CNodeKind.Ne:
                case CNodeKind.Lt:
                case CNodeKind.Le:
                    {
                        long a = EvalConstExpr(n.Lhs, message);
                        long b = EvalConstExpr(n.Rhs, message);
                        bool uns = n.Lhs.Type.Decay().Unsigned;
                        long ua = (uint)a, ub = (uint)b;
                        long r;
                        switch (n.Kind)
                        {
                            case CNodeKind.Mul: r = a * b; break;
                            case CNodeKind.Div:
                                if (b == 0) throw Error(n.Tok, "Division by zero in constant expression");
                                r = uns ? ua / ub : (int)a / (int)b; break;
                            case CNodeKind.Mod:
                                if (b == 0) throw Error(n.Tok, "Division by zero in constant expression");
                                r = uns ? ua % ub : (int)a % (int)b; break;
                            case CNodeKind.BitAnd: r = a & b; break;
                            case CNodeKind.BitOr: r = a | b; break;
                            case CNodeKind.BitXor: r = a ^ b; break;
                            case CNodeKind.Shl: r = (uint)a << (int)(b & 31); break;
                            case CNodeKind.Shr: r = uns ? (uint)a >> (int)(b & 31) : (int)a >> (int)(b & 31); break;
                            case CNodeKind.Eq: return (uint)a == (uint)b ? 1 : 0;
                            case CNodeKind.Ne: return (uint)a != (uint)b ? 1 : 0;
                            case CNodeKind.Lt: return (uns ? ua < ub : (int)a < (int)b) ? 1 : 0;
                            case CNodeKind.Le: return (uns ? ua <= ub : (int)a <= (int)b) ? 1 : 0;
                            default: r = 0; break;
                        }
                        return Normalize(r, n.Type);
                    }
            }

            throw Error(n.Tok, message);
        }

        private long EvalAddress(CNode n, out string label, string message)
        {
            switch (n.Kind)
            {
                case CNodeKind.Var:
                    if (n.Var.Kind == CSymbolKind.Global || n.Var.Kind == CSymbolKind.Function)
                    {
                        label = n.Var.Label;
                        return 0;
                    }
                    break;
                case CNodeKind.StrLabel:
                    label = n.Label;
                    return 0;
                case CNodeKind.Deref:
                    return EvalConstWithLabel(n.Lhs, out label, message);
                case CNodeKind.Member:
                    return EvalAddress(n.Lhs, out label, message) + n.Member.Offset;
            }
            throw Error(n.Tok, message);
        }

        private byte[] StringBytes(CNode str) => _unit.Strings.First(s => s.Label == str.Label).Bytes;

        // ====================================================================== Scopes & symbols

        private CSymbol NewLocal(string name, CType ty)
        {
            var sym = new CSymbol { Name = name, Kind = CSymbolKind.Local, Type = ty };
            _fn.Locals.Add(sym);
            return sym;
        }

        private void EnterScope()
        {
            _scopes.Add(new Dictionary<string, CSymbol>());
            _tagScopes.Add(new Dictionary<string, CType>());
        }

        private void LeaveScope()
        {
            _scopes.RemoveAt(_scopes.Count - 1);
            _tagScopes.RemoveAt(_tagScopes.Count - 1);
        }

        private void DeclareInScope(CToken name, CSymbol sym)
        {
            var scope = _scopes[^1];
            if (scope.TryGetValue(name.Text, out CSymbol existing))
            {
                bool sameTypedef = existing.Kind == CSymbolKind.Typedef && sym.Kind == CSymbolKind.Typedef && CType.SameType(existing.Type, sym.Type);
                if (!sameTypedef)
                    throw Error(name, $"Redefinition of '{name.Text}'");
            }
            scope[name.Text] = sym;
        }

        private CSymbol Lookup(string name)
        {
            for (int i = _scopes.Count - 1; i >= 0; i--)
                if (_scopes[i].TryGetValue(name, out CSymbol s))
                    return s;
            return null;
        }

        private CType LookupTag(string name)
        {
            for (int i = _tagScopes.Count - 1; i >= 0; i--)
                if (_tagScopes[i].TryGetValue(name, out CType t))
                    return t;
            return null;
        }

        public IEnumerable<CSymbol> AllGlobals => _globals.Values;

        // ====================================================================== Token helpers

        private CToken Cur => _toks[_pos];
        private CToken Peek2 => _pos + 1 < _toks.Count ? _toks[_pos + 1] : _toks[^1];
        private bool AtEof() => Cur.Kind == CTokenKind.EOF;
        private bool Peek(string s) => Cur.Is(s) && Cur.Kind != CTokenKind.String;

        private bool Accept(string s)
        {
            if ((Cur.Kind == CTokenKind.Punct || Cur.Kind == CTokenKind.Identifier) && Cur.Text == s)
            {
                _pos++;
                return true;
            }
            return false;
        }

        private CToken Expect(string s)
        {
            if (!Accept(s))
                throw Error(Cur, $"Expected '{s}' but found '{Cur}'");
            return _toks[_pos - 1];
        }

        private CToken ExpectIdentifier()
        {
            if (Cur.Kind != CTokenKind.Identifier || IsKeyword(Cur.Text))
                throw Error(Cur, $"Expected an identifier but found '{Cur}'");
            return _toks[_pos++];
        }

        private string NewLabel() => $".c__L{_labelCounter++}";

        private CCompileAbort Error(CToken at, string message)
        {
            _errors.Add(new CCompileError(at.File, at.Line, at.Column, message));
            return new CCompileAbort();
        }
    }
}
