using System.Collections.Generic;
using System.Text;

namespace Continuum93.Emulator.Compilers.C93C
{
    public enum CTokenKind
    {
        Identifier,
        Number,
        Char,
        String,
        Punct,
        Newline,    // Only produced for the preprocessor; stripped before parsing
        EOF
    }

    public sealed class CToken
    {
        public CTokenKind Kind;
        public string Text;          // Raw text (for String/Char this is the decoded value)
        public string File;
        public int Line;
        public int Column;
        public bool LeadingSpace;    // Whitespace preceded this token (needed for macro definitions/stringify)
        public HashSet<string> HideSet;  // Macros that must not expand this token again

        public CToken(CTokenKind kind, string text, string file, int line, int column)
        {
            Kind = kind;
            Text = text;
            File = file;
            Line = line;
            Column = column;
        }

        public CToken Clone()
        {
            return new CToken(Kind, Text, File, Line, Column)
            {
                LeadingSpace = LeadingSpace,
                HideSet = HideSet
            };
        }

        public bool Is(string punctOrIdent) =>
            (Kind == CTokenKind.Punct || Kind == CTokenKind.Identifier) && Text == punctOrIdent;

        public override string ToString() => Kind switch
        {
            CTokenKind.String => "\"" + Text + "\"",
            CTokenKind.Char => "'" + Text + "'",
            CTokenKind.EOF => "end of file",
            CTokenKind.Newline => "end of line",
            _ => Text
        };
    }

    /// <summary>
    /// Converts C source text into tokens. Newlines are kept as tokens because the preprocessor is line oriented.
    /// Comments and line continuations are removed here.
    /// </summary>
    public sealed class CLexer
    {
        private static readonly string[] Puncts =
        [
            "...", "<<=", ">>=",
            "->", "++", "--", "<<", ">>", "<=", ">=", "==", "!=", "&&", "||",
            "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=", "##",
            "[", "]", "(", ")", "{", "}", ".", "&", "*", "+", "-", "~", "!", "/", "%",
            "<", ">", "^", "|", "?", ":", ";", "=", ",", "#"
        ];

        private readonly string _src;
        private readonly string _file;
        private readonly List<CCompileError> _errors;
        private int _pos;
        private int _line = 1;
        private int _col = 1;

        public CLexer(string source, string file, List<CCompileError> errors)
        {
            // Normalize line endings and splice backslash-newline continuations
            _src = (source ?? "").Replace("\r\n", "\n").Replace('\r', '\n');
            _file = file;
            _errors = errors;
        }

        public List<CToken> Tokenize()
        {
            var tokens = new List<CToken>();
            bool leadingSpace = false;

            while (true)
            {
                SkipContinuations();
                if (_pos >= _src.Length)
                    break;

                char c = _src[_pos];

                if (c == '\n')
                {
                    tokens.Add(new CToken(CTokenKind.Newline, "\n", _file, _line, _col));
                    Advance();
                    leadingSpace = false;
                    continue;
                }

                if (c == ' ' || c == '\t' || c == '\f' || c == '\v')
                {
                    Advance();
                    leadingSpace = true;
                    continue;
                }

                if (c == '/' && Peek(1) == '/')
                {
                    while (_pos < _src.Length && _src[_pos] != '\n')
                        Advance();
                    leadingSpace = true;
                    continue;
                }

                if (c == '/' && Peek(1) == '*')
                {
                    int startLine = _line, startCol = _col;
                    Advance(); Advance();
                    bool closed = false;
                    while (_pos < _src.Length)
                    {
                        if (_src[_pos] == '*' && Peek(1) == '/')
                        {
                            Advance(); Advance();
                            closed = true;
                            break;
                        }
                        Advance();
                    }
                    if (!closed)
                        Error(startLine, startCol, "Unterminated block comment");
                    leadingSpace = true;
                    continue;
                }

                CToken tok = LexToken();
                if (tok != null)
                {
                    tok.LeadingSpace = leadingSpace;
                    tokens.Add(tok);
                }
                leadingSpace = false;
            }

            tokens.Add(new CToken(CTokenKind.Newline, "\n", _file, _line, _col));
            tokens.Add(new CToken(CTokenKind.EOF, "", _file, _line, _col));
            return tokens;
        }

        private CToken LexToken()
        {
            int line = _line, col = _col;
            char c = _src[_pos];

            if (char.IsLetter(c) || c == '_')
            {
                var sb = new StringBuilder();
                while (_pos < _src.Length && (char.IsLetterOrDigit(_src[_pos]) || _src[_pos] == '_'))
                {
                    sb.Append(_src[_pos]);
                    Advance();
                    SkipContinuations();
                }
                return new CToken(CTokenKind.Identifier, sb.ToString(), _file, line, col);
            }

            if (char.IsDigit(c) || (c == '.' && char.IsDigit(Peek(1))))
            {
                // pp-number: digits, letters, underscores, dots, and a sign directly after an exponent letter
                var sb = new StringBuilder();
                while (_pos < _src.Length)
                {
                    char d = _src[_pos];
                    char prev = sb.Length > 0 ? char.ToLowerInvariant(sb[^1]) : '\0';
                    bool isHex = sb.Length > 1 && sb[0] == '0' && (sb[1] == 'x' || sb[1] == 'X');
                    bool exponentSign = (d == '+' || d == '-') && (prev == 'p' || (prev == 'e' && !isHex));
                    if (!(char.IsLetterOrDigit(d) || d == '_' || d == '.' || exponentSign))
                        break;
                    sb.Append(d);
                    Advance();
                    SkipContinuations();
                }
                return new CToken(CTokenKind.Number, sb.ToString(), _file, line, col);
            }

            if (c == '"')
                return new CToken(CTokenKind.String, ReadQuoted('"', line, col), _file, line, col);

            if (c == '\'')
            {
                string value = ReadQuoted('\'', line, col);
                if (value.Length == 0)
                    Error(line, col, "Empty character constant");
                else if (value.Length > 1)
                    Error(line, col, "Multi-character constants are not supported");
                return new CToken(CTokenKind.Char, value, _file, line, col);
            }

            foreach (string p in Puncts)
            {
                if (string.CompareOrdinal(_src, _pos, p, 0, p.Length) == 0)
                {
                    for (int i = 0; i < p.Length; i++)
                        Advance();
                    return new CToken(CTokenKind.Punct, p, _file, line, col);
                }
            }

            Error(line, col, $"Unexpected character '{c}'");
            Advance();
            return null;
        }

        private string ReadQuoted(char quote, int line, int col)
        {
            Advance(); // opening quote
            var sb = new StringBuilder();
            while (true)
            {
                SkipContinuations();
                if (_pos >= _src.Length || _src[_pos] == '\n')
                {
                    Error(line, col, quote == '"' ? "Unterminated string literal" : "Unterminated character constant");
                    return sb.ToString();
                }

                char c = _src[_pos];
                if (c == quote)
                {
                    Advance();
                    return sb.ToString();
                }

                if (c == '\\')
                {
                    Advance();
                    sb.Append(ReadEscape(line, col));
                    continue;
                }

                sb.Append(c);
                Advance();
            }
        }

        private char ReadEscape(int line, int col)
        {
            if (_pos >= _src.Length)
                return '\\';

            char e = _src[_pos];
            Advance();
            switch (e)
            {
                case 'n': return '\n';
                case 't': return '\t';
                case 'r': return '\r';
                case '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7':
                    {
                        int value = e - '0';
                        for (int i = 0; i < 2 && _pos < _src.Length && _src[_pos] >= '0' && _src[_pos] <= '7'; i++)
                        {
                            value = value * 8 + (_src[_pos] - '0');
                            Advance();
                        }
                        return (char)(value & 0xFF);
                    }
                case 'x':
                    {
                        int value = 0, digits = 0;
                        while (_pos < _src.Length && IsHex(_src[_pos]))
                        {
                            value = value * 16 + HexVal(_src[_pos]);
                            Advance();
                            digits++;
                        }
                        if (digits == 0)
                            Error(line, col, "\\x used with no following hex digits");
                        return (char)(value & 0xFF);
                    }
                case 'a': return '\a';
                case 'b': return '\b';
                case 'f': return '\f';
                case 'v': return '\v';
                case 'e': return (char)27;
                case '\\': return '\\';
                case '\'': return '\'';
                case '"': return '"';
                case '?': return '?';
                default:
                    Error(line, col, $"Unknown escape sequence '\\{e}'");
                    return e;
            }
        }

        private static bool IsHex(char c) => char.IsDigit(c) || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
        private static int HexVal(char c) => char.IsDigit(c) ? c - '0' : (char.ToLowerInvariant(c) - 'a' + 10);

        private void SkipContinuations()
        {
            while (_pos + 1 < _src.Length && _src[_pos] == '\\' && _src[_pos + 1] == '\n')
            {
                _pos += 2;
                _line++;
                _col = 1;
            }
        }

        private char Peek(int offset) => _pos + offset < _src.Length ? _src[_pos + offset] : '\0';

        private void Advance()
        {
            if (_src[_pos] == '\n')
            {
                _line++;
                _col = 1;
            }
            else
            {
                _col++;
            }
            _pos++;
        }

        private void Error(int line, int col, string message) =>
            _errors.Add(new CCompileError(_file, line, col, message));
    }
}
