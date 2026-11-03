using System.Collections.Generic;

namespace Continuum93.Emulator.Compilers.C93C
{
    /// <summary>
    /// Integer literal parsing and the preprocessor's #if expression evaluator.
    /// </summary>
    public static class CConstExpr
    {
        /// <summary>
        /// Parses a C integer literal (decimal, 0x hex, 0b binary, leading-0 octal) with optional u/U/l/L suffixes.
        /// Follows C typing rules for a 32-bit int target: decimal literals that don't fit int are an error
        /// unless suffixed unsigned; hex/octal/binary literals that don't fit int become unsigned int.
        /// </summary>
        public static bool TryParseInt(string text, out uint value, out bool isUnsigned, out string error)
        {
            value = 0;
            isUnsigned = false;
            error = null;

            string s = text;
            bool hasU = false;
            int lCount = 0;
            while (s.Length > 0)
            {
                char last = char.ToLowerInvariant(s[^1]);
                if (last == 'u' && !hasU) { hasU = true; s = s[..^1]; }
                else if (last == 'l') { lCount++; s = s[..^1]; }
                else break;
            }

            if (lCount > 1)
            {
                error = "'long long' is not supported (integers are 32-bit)";
                return false;
            }

            if (s.Length == 0)
            {
                error = $"Invalid integer literal '{text}'";
                return false;
            }

            int radix = 10;
            string digits = s;
            if (s.Length > 1 && s[0] == '0' && (s[1] == 'x' || s[1] == 'X')) { radix = 16; digits = s[2..]; }
            else if (s.Length > 1 && s[0] == '0' && (s[1] == 'b' || s[1] == 'B')) { radix = 2; digits = s[2..]; }
            else if (s.Length > 1 && s[0] == '0') { radix = 8; digits = s[1..]; }

            if (digits.Length == 0)
            {
                error = $"Invalid integer literal '{text}'";
                return false;
            }

            ulong acc = 0;
            foreach (char ch in digits)
            {
                int d;
                if (ch >= '0' && ch <= '9') d = ch - '0';
                else if (ch >= 'a' && ch <= 'f') d = ch - 'a' + 10;
                else if (ch >= 'A' && ch <= 'F') d = ch - 'A' + 10;
                else
                {
                    error = text.Contains('.') || text.Contains('e') && radix == 10
                        ? "Floating point is not supported"
                        : $"Invalid digit '{ch}' in integer literal '{text}'";
                    return false;
                }
                if (d >= radix)
                {
                    error = $"Invalid digit '{ch}' in integer literal '{text}'";
                    return false;
                }
                acc = acc * (ulong)radix + (ulong)d;
                if (acc > uint.MaxValue)
                {
                    error = $"Integer literal '{text}' is too large (integers are 32-bit)";
                    return false;
                }
            }

            value = (uint)acc;
            if (hasU)
                isUnsigned = true;
            else if (acc > int.MaxValue)
            {
                if (radix == 10)
                {
                    error = $"Integer literal '{text}' is too large for int; add a 'u' suffix";
                    return false;
                }
                isUnsigned = true;
            }
            return true;
        }

        // ---------------------------------------------------------------- #if evaluator

        public static long EvaluatePreprocessor(List<CToken> tokens, List<CCompileError> errors)
        {
            var p = new PPParser(tokens, errors);
            long v = p.ParseTernary();
            if (p.Pos < tokens.Count && errors.Count == 0)
                p.Error(tokens[p.Pos], $"Unexpected token '{tokens[p.Pos]}' in #if expression");
            return v;
        }

        private sealed class PPParser
        {
            private readonly List<CToken> _t;
            private readonly List<CCompileError> _errors;
            public int Pos;

            public PPParser(List<CToken> tokens, List<CCompileError> errors)
            {
                _t = tokens;
                _errors = errors;
            }

            private bool Accept(string p)
            {
                if (Pos < _t.Count && _t[Pos].Kind == CTokenKind.Punct && _t[Pos].Text == p)
                {
                    Pos++;
                    return true;
                }
                return false;
            }

            public void Error(CToken t, string message)
            {
                if (_errors.Count == 0)
                    _errors.Add(new CCompileError(t.File, t.Line, t.Column, message));
            }

            public long ParseTernary()
            {
                long c = ParseBinary(0);
                if (Accept("?"))
                {
                    long a = ParseTernary();
                    if (!Accept(":"))
                        Error(Pos < _t.Count ? _t[Pos] : _t[^1], "Expected ':' in #if expression");
                    long b = ParseTernary();
                    return c != 0 ? a : b;
                }
                return c;
            }

            private static readonly string[][] Levels =
            [
                ["||"], ["&&"], ["|"], ["^"], ["&"], ["==", "!="], ["<", ">", "<=", ">="], ["<<", ">>"], ["+", "-"], ["*", "/", "%"]
            ];

            private long ParseBinary(int level)
            {
                if (level == Levels.Length)
                    return ParseUnary();

                long left = ParseBinary(level + 1);
                while (true)
                {
                    string op = null;
                    foreach (string o in Levels[level])
                    {
                        if (Accept(o)) { op = o; break; }
                    }
                    if (op == null)
                        return left;

                    CToken at = _t[Pos - 1];
                    long right = ParseBinary(level + 1);
                    left = op switch
                    {
                        "||" => (left != 0 || right != 0) ? 1 : 0,
                        "&&" => (left != 0 && right != 0) ? 1 : 0,
                        "|" => left | right,
                        "^" => left ^ right,
                        "&" => left & right,
                        "==" => left == right ? 1 : 0,
                        "!=" => left != right ? 1 : 0,
                        "<" => left < right ? 1 : 0,
                        ">" => left > right ? 1 : 0,
                        "<=" => left <= right ? 1 : 0,
                        ">=" => left >= right ? 1 : 0,
                        "<<" => left << (int)(right & 63),
                        ">>" => left >> (int)(right & 63),
                        "+" => left + right,
                        "-" => left - right,
                        "*" => left * right,
                        "/" => right == 0 ? DivZero(at) : left / right,
                        "%" => right == 0 ? DivZero(at) : left % right,
                        _ => 0
                    };
                }
            }

            private long DivZero(CToken at)
            {
                Error(at, "Division by zero in #if expression");
                return 0;
            }

            private long ParseUnary()
            {
                if (Accept("+")) return ParseUnary();
                if (Accept("-")) return -ParseUnary();
                if (Accept("!")) return ParseUnary() == 0 ? 1 : 0;
                if (Accept("~")) return ~ParseUnary();
                if (Accept("("))
                {
                    long v = ParseTernary();
                    if (!Accept(")"))
                        Error(Pos < _t.Count ? _t[Pos] : _t[^1], "Expected ')' in #if expression");
                    return v;
                }

                if (Pos >= _t.Count)
                {
                    Error(_t[^1], "Unexpected end of #if expression");
                    return 0;
                }

                CToken t = _t[Pos++];
                if (t.Kind == CTokenKind.Number)
                {
                    if (TryParseInt(t.Text, out uint v, out bool uns, out string err))
                        return uns ? v : (int)v;
                    Error(t, err);
                    return 0;
                }
                if (t.Kind == CTokenKind.Char)
                    return t.Text.Length > 0 ? (sbyte)(byte)t.Text[0] : 0;

                Error(t, $"Unexpected token '{t}' in #if expression");
                return 0;
            }
        }
    }
}
