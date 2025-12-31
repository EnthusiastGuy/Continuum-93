using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Continuum93.Emulator.Compilers.C93Basic
{
    /// <summary>
    /// Lexer for tokenizing BASIC source code.
    /// </summary>
    public class Lexer
    {
        public List<CompileError> Errors { get; private set; } = new List<CompileError>();

        private static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>(StringComparer.OrdinalIgnoreCase)
        {
            // Control flow
            { "LET", TokenType.LET },
            { "DIM", TokenType.DIM },
            { "IF", TokenType.IF },
            { "THEN", TokenType.THEN },
            { "ELSE", TokenType.ELSE },
            { "ELSEIF", TokenType.ELSEIF },
            { "END", TokenType.END },
            { "ENDIF", TokenType.ENDIF },
            { "FOR", TokenType.FOR },
            { "TO", TokenType.TO },
            { "STEP", TokenType.STEP },
            { "NEXT", TokenType.NEXT },
            { "WHILE", TokenType.WHILE },
            { "WEND", TokenType.WEND },
            { "REPEAT", TokenType.REPEAT },
            { "UNTIL", TokenType.UNTIL },
            { "GOTO", TokenType.GOTO },
            { "GOSUB", TokenType.GOSUB },
            { "RETURN", TokenType.RETURN },
            { "EXIT", TokenType.EXIT },
            { "CONTINUE", TokenType.CONTINUE },
            { "REM", TokenType.REM },
            { "DATA", TokenType.DATA },
            { "STOP", TokenType.STOP },

            // I/O
            { "PRINT", TokenType.PRINT },
            { "INPUT", TokenType.INPUT },
            { "INKEY", TokenType.INKEY },
            { "MOUSE", TokenType.MOUSE },
            { "BUTTON", TokenType.BUTTON },
            { "X", TokenType.X },
            { "Y", TokenType.Y },
            { "LOAD", TokenType.LOAD },
            { "SAVE", TokenType.SAVE },
            { "OPEN", TokenType.OPEN },
            { "CLOSE", TokenType.CLOSE },
            { "READ", TokenType.READ },
            { "WRITE", TokenType.WRITE },
            { "AT", TokenType.AT },
            { "USING", TokenType.USING },
            { "TAB", TokenType.TAB },

            // Graphics
            { "CLS", TokenType.CLS },
            { "SCREEN", TokenType.SCREEN },
            { "VIDEO", TokenType.VIDEO },
            { "PLOT", TokenType.PLOT },
            { "LINE", TokenType.LINE },
            { "RECTANGLE", TokenType.RECTANGLE },
            { "CIRCLE", TokenType.CIRCLE },
            { "ELLIPSE", TokenType.ELLIPSE },
            { "FILLED", TokenType.FILLED },
            { "INK", TokenType.INK },
            { "PAPER", TokenType.PAPER },
            { "LOCATE", TokenType.LOCATE },
            { "FONT", TokenType.FONT },
            { "COLOR", TokenType.COLOR },
            { "FLAGS", TokenType.FLAGS },
            { "MAXWIDTH", TokenType.MAXWIDTH },
            { "OUTLINE", TokenType.OUTLINE },
            { "LAYER", TokenType.LAYER },
            { "SHOW", TokenType.SHOW },
            { "HIDE", TokenType.HIDE },
            { "VISIBILITY", TokenType.VISIBILITY },
            { "SPRITE", TokenType.SPRITE },

            // Audio
            { "BEEP", TokenType.BEEP },
            { "PLAY", TokenType.PLAY },

            // Memory
            { "PEEK", TokenType.PEEK },
            { "POKE", TokenType.POKE },
            { "PEEK16", TokenType.PEEK16 },
            { "POKE16", TokenType.POKE16 },
            { "PEEK24", TokenType.PEEK24 },
            { "POKE24", TokenType.POKE24 },
            { "PEEK32", TokenType.PEEK32 },
            { "POKE32", TokenType.POKE32 },
            { "MEMCOPY", TokenType.MEMCOPY },
            { "MEMFILL", TokenType.MEMFILL },
            { "VARPTR", TokenType.VARPTR },

            // System
            { "WAIT", TokenType.WAIT },
            { "SLEEP", TokenType.SLEEP },
            { "TIME", TokenType.TIME },
            { "TICKS", TokenType.TICKS },
            { "ON", TokenType.ON },
            { "ERROR", TokenType.ERROR },
            { "RESUME", TokenType.RESUME },
            { "ERR", TokenType.ERR },
            { "ERL", TokenType.ERL },

            // Operators
            { "MOD", TokenType.MODULO },
            { "AND", TokenType.AND },
            { "OR", TokenType.OR },
            { "XOR", TokenType.XOR },
            { "NOT", TokenType.NOT },
            { "IMPLY", TokenType.IMPLY },
            { "NAND", TokenType.NAND },
            { "NOR", TokenType.NOR },
            { "XNOR", TokenType.XNOR },
            { "SHL", TokenType.SHL },
            { "SHR", TokenType.SHR },
            { "ROL", TokenType.ROL },
            { "ROR", TokenType.ROR },

            // Math functions
            { "ABS", TokenType.ABS },
            { "SGN", TokenType.SGN },
            { "INT", TokenType.INT },
            { "FIX", TokenType.FIX },
            { "FLOOR", TokenType.FLOOR },
            { "CEIL", TokenType.CEIL },
            { "ROUND", TokenType.ROUND },
            { "SIN", TokenType.SIN },
            { "COS", TokenType.COS },
            { "TAN", TokenType.TAN },
            { "ASIN", TokenType.ASIN },
            { "ACOS", TokenType.ACOS },
            { "ATAN", TokenType.ATAN },
            { "ATAN2", TokenType.ATAN2 },
            { "EXP", TokenType.EXP },
            { "LOG", TokenType.LOG },
            { "LOG10", TokenType.LOG10 },
            { "SQR", TokenType.SQR },
            { "POW", TokenType.POW },
            { "ISQR", TokenType.ISQR },
            { "MIN", TokenType.MIN },
            { "MAX", TokenType.MAX },
            { "RND", TokenType.RND },
            { "RANDOMIZE", TokenType.RANDOMIZE },
            { "PI", TokenType.PI },
            { "E", TokenType.E },

            // String functions
            { "LEN", TokenType.LEN },
            { "LEFT$", TokenType.LEFT },
            { "RIGHT$", TokenType.RIGHT },
            { "MID$", TokenType.MID },
            { "CHR$", TokenType.CHR },
            { "ASC", TokenType.ASC },
            { "VAL", TokenType.VAL },
            { "STR$", TokenType.STR },
            { "STRING$", TokenType.STRING_FUNC },
            { "INSTR", TokenType.INSTR },
            { "UCASE$", TokenType.UCASE },
            { "LCASE$", TokenType.LCASE },
            { "TRIM$", TokenType.TRIM },
            { "LTRIM$", TokenType.LTRIM },
            { "RTRIM$", TokenType.RTRIM },
        };

        public List<Token> Tokenize(string source)
        {
            Errors.Clear();
            List<Token> tokens = new List<Token>();
            
            string[] lines = source.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            
            for (int lineNum = 0; lineNum < lines.Length; lineNum++)
            {
                string line = lines[lineNum];
                int col = 0;
                int lineNumber = lineNum + 1;

                // Check for label at start of line (only if the entire line is just the label)
                string trimmedLine = line.Trim();
                if (trimmedLine.EndsWith(":") && trimmedLine.IndexOf(':') == trimmedLine.Length - 1)
                {
                    // This is a label-only line
                    string label = trimmedLine.TrimEnd(':').Trim();
                    if (!string.IsNullOrWhiteSpace(label))
                    {
                        tokens.Add(new Token(TokenType.IDENTIFIER, label, lineNumber, 0));
                        tokens.Add(new Token(TokenType.COLON, ":", lineNumber, label.Length));
                        tokens.Add(new Token(TokenType.NEWLINE, "\n", lineNumber, trimmedLine.Length));
                    }
                    continue;
                }

                while (col < line.Length)
                {
                    // Skip whitespace
                    if (char.IsWhiteSpace(line[col]))
                    {
                        col++;
                        continue;
                    }

                    // Comment
                    if (line[col] == '\'' || (col < line.Length - 2 && line.Substring(col, 3).ToUpper() == "REM"))
                    {
                        if (line[col] == '\'')
                        {
                            tokens.Add(new Token(TokenType.COMMENT, line.Substring(col), lineNumber, col));
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.REM, "REM", lineNumber, col));
                            tokens.Add(new Token(TokenType.COMMENT, line.Substring(col + 3), lineNumber, col + 3));
                        }
                        break;
                    }

                    // String literal
                    if (line[col] == '"')
                    {
                        int start = col;
                        col++;
                        StringBuilder sb = new StringBuilder();
                        sb.Append('"');
                        
                        while (col < line.Length && line[col] != '"')
                        {
                            if (line[col] == '\\' && col + 1 < line.Length)
                            {
                                col++;
                                switch (line[col])
                                {
                                    case 'n': sb.Append('\n'); break;
                                    case 't': sb.Append('\t'); break;
                                    case 'r': sb.Append('\r'); break;
                                    case '\\': sb.Append('\\'); break;
                                    case '"': sb.Append('"'); break;
                                    default: sb.Append('\\').Append(line[col]); break;
                                }
                            }
                            else
                            {
                                sb.Append(line[col]);
                            }
                            col++;
                        }
                        
                        if (col < line.Length)
                        {
                            sb.Append('"');
                            tokens.Add(new Token(TokenType.STRING, sb.ToString(), lineNumber, start));
                            col++;
                        }
                        else
                        {
                            Errors.Add(new CompileError(lineNumber, start, "Unterminated string literal"));
                        }
                        continue;
                    }

                    // Numbers
                    if (char.IsDigit(line[col]) || (line[col] == '-' && col + 1 < line.Length && char.IsDigit(line[col + 1])))
                    {
                        int start = col;
                        bool isNegative = line[col] == '-';
                        if (isNegative) col++;
                        
                        StringBuilder num = new StringBuilder();
                        if (isNegative) num.Append('-');
                        
                        bool isFloat = false;
                        bool isHex = false;
                        bool isBinary = false;
                        
                        // Check for hex (0x or 0X)
                        if (col + 1 < line.Length && line[col] == '0' && (line[col + 1] == 'x' || line[col + 1] == 'X'))
                        {
                            isHex = true;
                            num.Append(line[col]).Append(line[col + 1]);
                            col += 2;
                        }
                        // Check for binary (0b or 0B)
                        else if (col + 1 < line.Length && line[col] == '0' && (line[col + 1] == 'b' || line[col + 1] == 'B'))
                        {
                            isBinary = true;
                            num.Append(line[col]).Append(line[col + 1]);
                            col += 2;
                        }
                        
                        while (col < line.Length)
                        {
                            char c = line[col];
                            if (isHex && ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f')))
                            {
                                num.Append(c);
                                col++;
                            }
                            else if (isBinary && (c == '0' || c == '1'))
                            {
                                num.Append(c);
                                col++;
                            }
                            else if (char.IsDigit(c))
                            {
                                num.Append(c);
                                col++;
                            }
                            else if (c == '.' && !isFloat && !isHex && !isBinary)
                            {
                                isFloat = true;
                                num.Append(c);
                                col++;
                            }
                            else if ((c == 'e' || c == 'E') && !isHex && !isBinary)
                            {
                                isFloat = true;
                                num.Append(c);
                                col++;
                                if (col < line.Length && (line[col] == '+' || line[col] == '-'))
                                {
                                    num.Append(line[col]);
                                    col++;
                                }
                            }
                            else if (c == '#' && col + 1 < line.Length && char.IsWhiteSpace(line[col + 1]))
                            {
                                // Float suffix
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                        
                        string numStr = num.ToString();
                        if (isFloat || numStr.Contains(".") || numStr.Contains("e") || numStr.Contains("E"))
                        {
                            tokens.Add(new Token(TokenType.FLOAT, numStr, lineNumber, start));
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.INTEGER, numStr, lineNumber, start));
                        }
                        continue;
                    }

                    // Operators and punctuation
                    char ch = line[col];
                    switch (ch)
                    {
                        case '+': tokens.Add(new Token(TokenType.PLUS, "+", lineNumber, col)); col++; break;
                        case '-': tokens.Add(new Token(TokenType.MINUS, "-", lineNumber, col)); col++; break;
                        case '*': tokens.Add(new Token(TokenType.MULTIPLY, "*", lineNumber, col)); col++; break;
                        case '/': tokens.Add(new Token(TokenType.DIVIDE, "/", lineNumber, col)); col++; break;
                        case '\\': tokens.Add(new Token(TokenType.INT_DIVIDE, "\\", lineNumber, col)); col++; break;
                        case '^': tokens.Add(new Token(TokenType.POWER, "^", lineNumber, col)); col++; break;
                        case '=': tokens.Add(new Token(TokenType.EQUAL, "=", lineNumber, col)); col++; break;
                        case '<':
                            if (col + 1 < line.Length && line[col + 1] == '>')
                            {
                                tokens.Add(new Token(TokenType.NOT_EQUAL, "<>", lineNumber, col));
                                col += 2;
                            }
                            else if (col + 1 < line.Length && line[col + 1] == '=')
                            {
                                tokens.Add(new Token(TokenType.LESS_EQUAL, "<=", lineNumber, col));
                                col += 2;
                            }
                            else
                            {
                                tokens.Add(new Token(TokenType.LESS_THAN, "<", lineNumber, col));
                                col++;
                            }
                            break;
                        case '>':
                            if (col + 1 < line.Length && line[col + 1] == '=')
                            {
                                tokens.Add(new Token(TokenType.GREATER_EQUAL, ">=", lineNumber, col));
                                col += 2;
                            }
                            else
                            {
                                tokens.Add(new Token(TokenType.GREATER_THAN, ">", lineNumber, col));
                                col++;
                            }
                            break;
                        case ',': tokens.Add(new Token(TokenType.COMMA, ",", lineNumber, col)); col++; break;
                        case ';': tokens.Add(new Token(TokenType.SEMICOLON, ";", lineNumber, col)); col++; break;
                        case ':': tokens.Add(new Token(TokenType.COLON, ":", lineNumber, col)); col++; break;
                        case '(': tokens.Add(new Token(TokenType.LPAREN, "(", lineNumber, col)); col++; break;
                        case ')': tokens.Add(new Token(TokenType.RPAREN, ")", lineNumber, col)); col++; break;
                        case '[': tokens.Add(new Token(TokenType.LBRACKET, "[", lineNumber, col)); col++; break;
                        case ']': tokens.Add(new Token(TokenType.RBRACKET, "]", lineNumber, col)); col++; break;
                        default:
                            // Identifier or keyword
                            int identStart = col;
                            StringBuilder ident = new StringBuilder();
                            
                            while (col < line.Length && (char.IsLetterOrDigit(line[col]) || line[col] == '_' || line[col] == '$' || line[col] == '#'))
                            {
                                ident.Append(line[col]);
                                col++;
                            }
                            
                            string identStr = ident.ToString();
                            if (identStr.Length > 0)
                            {
                                // Check for string function with $ suffix
                                if (identStr.EndsWith("$") && identStr.Length > 1)
                                {
                                    string baseName = identStr.Substring(0, identStr.Length - 1) + "$";
                                    if (Keywords.ContainsKey(baseName))
                                    {
                                        tokens.Add(new Token(Keywords[baseName], baseName, lineNumber, identStart));
                                    }
                                    else
                                    {
                                        tokens.Add(new Token(TokenType.IDENTIFIER, identStr, lineNumber, identStart));
                                    }
                                }
                                else if (Keywords.ContainsKey(identStr))
                                {
                                    tokens.Add(new Token(Keywords[identStr], identStr, lineNumber, identStart));
                                }
                                else
                                {
                                    tokens.Add(new Token(TokenType.IDENTIFIER, identStr, lineNumber, identStart));
                                }
                            }
                            else
                            {
                                Errors.Add(new CompileError(lineNumber, col, $"Unexpected character: {ch}"));
                                col++;
                            }
                            break;
                    }
                }
                
                tokens.Add(new Token(TokenType.NEWLINE, "\n", lineNumber, line.Length));
            }
            
            tokens.Add(new Token(TokenType.EOF, "", lines.Length, 0));
            return tokens;
        }
    }

    public class CompileError
    {
        public int Line { get; set; }
        public int Column { get; set; }
        public string Message { get; set; }

        public CompileError(int line, int column, string message)
        {
            Line = line;
            Column = column;
            Message = message;
        }
    }
}

