using System.Collections.Generic;

namespace Continuum93.Emulator.Compilers.C93C
{
    public enum CSymbolKind
    {
        Global,     // File-scope or static-local object, addressed by label
        Local,      // Automatic variable or parameter, addressed relative to the frame pointer
        Function,
        EnumConst,
        Typedef
    }

    public sealed class CSymbol
    {
        public string Name;
        public CSymbolKind Kind;
        public CType Type;
        public string Label;            // Globals and functions
        public int Offset;              // Locals/params: offset from the frame pointer (QRS)
        public long EnumValue;
        public bool IsDefined;          // Function has a body / global has storage in this unit
        public bool IsStatic;
        public bool IsExtern;
        public bool IsReferenced;

        // Global storage
        public bool HasInitializer;
        public byte[] InitData;
        public List<CRelocation> Relocations = [];

        // Function definition
        public CFunction Function;
    }

    /// <summary>Address that must be patched into a global's initial data at startup: *(label+Offset) = &amp;Target + Addend.</summary>
    public sealed class CRelocation
    {
        public int Offset;
        public string TargetLabel;
        public long Addend;
    }

    public sealed class CFunction
    {
        public string Name;
        public CSymbol Symbol;
        public CType Type;
        public List<CSymbol> Params = [];
        public List<CSymbol> Locals = [];
        public CNode Body;
        public int FrameSize;
        public string ReturnLabel;
        public HashSet<string> CalledFunctions = [];  // Labels of functions/globals referenced (for dead code elimination)
    }

    public enum CNodeKind
    {
        // Expressions
        Num,
        Var,
        StrLabel,       // Address of a string literal (Label)
        FuncAddr,       // Address of a function (Var holds the symbol)
        Deref,
        Addr,
        Neg,
        BitNot,
        LogNot,
        Add, Sub, Mul, Div, Mod, Shl, Shr, BitAnd, BitOr, BitXor,
        Eq, Ne, Lt, Le,
        LogAnd, LogOr,
        Cond,
        Comma,
        Assign,
        Member,
        Cast,
        Call,
        MemZero,        // Zero-fill a local variable (Var)

        // Statements
        Block,
        ExprStmt,
        If,
        For,            // Also while
        Do,
        Switch,
        Case,
        Goto,
        Label,
        Return,
        Asm,
        Empty
    }

    public sealed class CNode
    {
        public CNodeKind Kind;
        public CType Type;
        public CToken Tok;

        public CNode Lhs;
        public CNode Rhs;

        // Control flow
        public CNode Cond;
        public CNode Then;
        public CNode Els;
        public CNode Init;
        public CNode Inc;
        public CNode BodyStmt;
        public List<CNode> Body;            // Block statements
        public string BreakLabel;
        public string ContinueLabel;

        // Switch / case
        public List<CNode> Cases;
        public CNode DefaultCase;
        public long CaseValue;

        // Goto / label: user label name and the unique assembly label
        public string LabelName;
        public string UniqueLabel;

        // Values
        public long Value;                  // Num
        public CSymbol Var;                 // Var, FuncAddr, MemZero
        public string Label;                // StrLabel
        public CMember Member;              // Member
        public string AsmText;              // Asm

        // Calls
        public List<CNode> Args;
        public CType FuncType;              // Callee's function type
        public CSymbol DirectCallee;        // Set when calling a named function directly

        public CNode(CNodeKind kind, CToken tok)
        {
            Kind = kind;
            Tok = tok;
        }
    }

    public sealed class CTranslationUnit
    {
        public List<CSymbol> Globals = [];      // In definition order
        public List<CFunction> Functions = [];
        public List<(string Label, byte[] Bytes)> Strings = [];
    }
}
