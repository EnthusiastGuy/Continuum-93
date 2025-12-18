namespace Continuum93.Emulator.Compilers.C93Basic
{
    /// <summary>
    /// Represents a token in the BASIC source code.
    /// </summary>
    public class Token
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }

        public Token(TokenType type, string value, int line, int column)
        {
            Type = type;
            Value = value;
            Line = line;
            Column = column;
        }

        public override string ToString()
        {
            return $"{Type}({Value}) at {Line}:{Column}";
        }
    }

    public enum TokenType
    {
        // Literals
        INTEGER,
        FLOAT,
        STRING,
        IDENTIFIER,

        // Operators
        PLUS,           // +
        MINUS,          // -
        MULTIPLY,       // *
        DIVIDE,         // /
        INT_DIVIDE,     // \
        MODULO,         // MOD
        POWER,          // ^
        ASSIGN,         // =
        EQUAL,          // =
        NOT_EQUAL,      // <>
        LESS_THAN,      // <
        GREATER_THAN,   // >
        LESS_EQUAL,     // <=
        GREATER_EQUAL,  // >=

        // Logical operators
        AND,            // AND
        OR,             // OR
        XOR,            // XOR
        NOT,            // NOT
        IMPLY,          // IMPLY
        NAND,           // NAND
        NOR,            // NOR
        XNOR,           // XNOR

        // Bitwise operators
        SHL,            // SHL
        SHR,            // SHR
        ROL,            // ROL
        ROR,            // ROR

        // Punctuation
        COMMA,          // ,
        SEMICOLON,      // ;
        COLON,          // :
        LPAREN,         // (
        RPAREN,         // )
        LBRACKET,       // [
        RBRACKET,       // ]

        // Keywords
        LET,
        DIM,
        IF,
        THEN,
        ELSE,
        ELSEIF,
        END,
        ENDIF,
        FOR,
        TO,
        STEP,
        NEXT,
        WHILE,
        WEND,
        REPEAT,
        UNTIL,
        GOTO,
        GOSUB,
        RETURN,
        EXIT,
        CONTINUE,
        REM,
        DATA,
        PRINT,
        INPUT,
        CLS,
        SCREEN,
        VIDEO,
        PLOT,
        LINE,
        RECTANGLE,
        CIRCLE,
        ELLIPSE,
        FILLED,
        INK,
        PAPER,
        LOCATE,
        FONT,
        COLOR,
        FLAGS,
        MAXWIDTH,
        OUTLINE,
        LAYER,
        SHOW,
        HIDE,
        VISIBILITY,
        SPRITE,
        BEEP,
        PLAY,
        INKEY,
        MOUSE,
        BUTTON,
        LOAD,
        SAVE,
        OPEN,
        CLOSE,
        READ,
        WRITE,
        PEEK,
        POKE,
        PEEK16,
        POKE16,
        PEEK24,
        POKE24,
        PEEK32,
        POKE32,
        MEMCOPY,
        MEMFILL,
        VARPTR,
        WAIT,
        SLEEP,
        TIME,
        TICKS,
        ON,
        ERROR,
        RESUME,
        ERR,
        ERL,
        STOP,
        AT,
        USING,
        TAB,

        // Math functions
        ABS, SGN, INT, FIX, FLOOR, CEIL, ROUND,
        SIN, COS, TAN, ASIN, ACOS, ATAN, ATAN2,
        EXP, LOG, LOG10,
        SQR, POW, ISQR,
        MIN, MAX,
        RND, RANDOMIZE,
        PI, E,

        // String functions
        LEN, LEFT, RIGHT, MID, CHR, ASC, VAL, STR,
        STRING_FUNC, INSTR, UCASE, LCASE, TRIM, LTRIM, RTRIM,

        // Special
        EOF,
        NEWLINE,
        COMMENT
    }
}

