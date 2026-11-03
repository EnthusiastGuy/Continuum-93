using System.Collections.Generic;
using System.Linq;

namespace Continuum93.Emulator.Compilers.C93C
{
    public enum CTypeKind
    {
        Void,
        Char,       // 8-bit
        Short,      // 16-bit
        Int,        // 32-bit (also long and enums)
        Pointer,    // 32-bit storage, 24-bit address space
        Array,
        Struct,     // Also unions (see CStructInfo.IsUnion)
        Function
    }

    public sealed class CMember
    {
        public string Name;
        public CType Type;
        public int Offset;
    }

    /// <summary>Shared between all CType instances of the same struct tag so forward declarations complete later.</summary>
    public sealed class CStructInfo
    {
        public string Tag;
        public bool IsUnion;
        public bool Complete;
        public List<CMember> Members = [];
        public int Size;

        public CMember Find(string name) => Members.FirstOrDefault(m => m.Name == name);
    }

    public sealed class CParam
    {
        public string Name;
        public CType Type;
    }

    /// <summary>
    /// A C type. Integers are 8/16/32-bit; there is no floating point and no 64-bit integer.
    /// Structs are packed (alignment 1) because the target has no alignment requirements.
    /// </summary>
    public sealed class CType
    {
        public CTypeKind Kind;
        public bool Unsigned;
        public CType Base;              // Pointee, array element or function return type
        public int Length = -1;         // Array length; -1 when incomplete (e.g. int a[])
        public CStructInfo Struct;
        public List<CParam> Params;
        public bool HasPrototype;       // false for "int f()" (old-style, unknown parameters)
        public bool Variadic;
        public bool IsEnum;
        public bool IsBool;             // _Bool: 8-bit, conversions normalize to 0/1

        public static readonly CType Void = new() { Kind = CTypeKind.Void };
        public static readonly CType Bool = new() { Kind = CTypeKind.Char, Unsigned = true, IsBool = true };
        public static readonly CType Char = new() { Kind = CTypeKind.Char };
        public static readonly CType UChar = new() { Kind = CTypeKind.Char, Unsigned = true };
        public static readonly CType Short = new() { Kind = CTypeKind.Short };
        public static readonly CType UShort = new() { Kind = CTypeKind.Short, Unsigned = true };
        public static readonly CType Int = new() { Kind = CTypeKind.Int };
        public static readonly CType UInt = new() { Kind = CTypeKind.Int, Unsigned = true };

        public static CType PointerTo(CType t) => new() { Kind = CTypeKind.Pointer, Base = t, Unsigned = true };
        public static CType ArrayOf(CType t, int length) => new() { Kind = CTypeKind.Array, Base = t, Length = length };

        public static CType FunctionOf(CType ret, List<CParam> ps, bool hasPrototype, bool variadic) =>
            new() { Kind = CTypeKind.Function, Base = ret, Params = ps, HasPrototype = hasPrototype, Variadic = variadic };

        public bool IsInteger => Kind is CTypeKind.Char or CTypeKind.Short or CTypeKind.Int;
        public bool IsPointer => Kind == CTypeKind.Pointer;
        public bool IsArray => Kind == CTypeKind.Array;
        public bool IsStruct => Kind == CTypeKind.Struct;
        public bool IsFunction => Kind == CTypeKind.Function;
        public bool IsVoid => Kind == CTypeKind.Void;
        public bool IsScalar => IsInteger || IsPointer;
        public bool IsPointerLike => IsPointer || IsArray;   // Has a Base and supports indexing

        public bool IsComplete =>
            Kind switch
            {
                CTypeKind.Void => false,
                CTypeKind.Array => Length >= 0 && Base.IsComplete,
                CTypeKind.Struct => Struct.Complete,
                CTypeKind.Function => false,
                _ => true
            };

        public int Size =>
            Kind switch
            {
                CTypeKind.Void => 1,        // GNU-style: arithmetic on void* uses size 1
                CTypeKind.Char => 1,
                CTypeKind.Short => 2,
                CTypeKind.Int => 4,
                CTypeKind.Pointer => 4,
                CTypeKind.Array => Length < 0 ? 0 : Length * Base.Size,
                CTypeKind.Struct => Struct.Size,
                CTypeKind.Function => 1,
                _ => 4
            };

        /// <summary>Arrays decay to pointers to their first element, functions to function pointers.</summary>
        public CType Decay() =>
            Kind switch
            {
                CTypeKind.Array => PointerTo(Base),
                CTypeKind.Function => PointerTo(this),
                _ => this
            };

        public CType Copy() => (CType)MemberwiseClone();

        public static bool SameType(CType a, CType b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a.Kind != b.Kind) return false;
            switch (a.Kind)
            {
                case CTypeKind.Void:
                    return true;
                case CTypeKind.Char:
                case CTypeKind.Short:
                case CTypeKind.Int:
                    return a.Unsigned == b.Unsigned && a.IsBool == b.IsBool;
                case CTypeKind.Pointer:
                    return SameType(a.Base, b.Base);
                case CTypeKind.Array:
                    return SameType(a.Base, b.Base) && (a.Length < 0 || b.Length < 0 || a.Length == b.Length);
                case CTypeKind.Struct:
                    return ReferenceEquals(a.Struct, b.Struct);
                case CTypeKind.Function:
                    if (!SameType(a.Base, b.Base)) return false;
                    if (!a.HasPrototype || !b.HasPrototype) return true;
                    if (a.Variadic != b.Variadic || a.Params.Count != b.Params.Count) return false;
                    for (int i = 0; i < a.Params.Count; i++)
                        if (!SameType(a.Params[i].Type, b.Params[i].Type)) return false;
                    return true;
            }
            return false;
        }

        public override string ToString()
        {
            switch (Kind)
            {
                case CTypeKind.Void: return "void";
                case CTypeKind.Char: return IsBool ? "_Bool" : Unsigned ? "unsigned char" : "char";
                case CTypeKind.Short: return Unsigned ? "unsigned short" : "short";
                case CTypeKind.Int: return IsEnum ? "enum" : Unsigned ? "unsigned int" : "int";
                case CTypeKind.Pointer: return Base.ToString() + "*";
                case CTypeKind.Array: return $"{Base}[{(Length >= 0 ? Length.ToString() : "")}]";
                case CTypeKind.Struct: return (Struct.IsUnion ? "union " : "struct ") + (Struct.Tag ?? "<anonymous>");
                case CTypeKind.Function:
                    string ps = !HasPrototype ? "" : Params.Count == 0 && !Variadic ? "void" : string.Join(", ", Params.Select(p => p.Type.ToString()));
                    if (Variadic) ps += ", ...";
                    return $"{Base}({ps})";
            }
            return "?";
        }
    }
}
