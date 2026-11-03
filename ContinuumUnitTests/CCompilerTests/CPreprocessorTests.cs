using Continuum93.Emulator.Compilers.C93C;
using static ContinuumUnitTests.CCompilerTests.CTestHarness;

namespace ContinuumUnitTests.CCompilerTests
{
    [Collection("CCompiler")]
    public class CPreprocessorTests
    {
        private static string Pp(string src, Dictionary<string, string>? files = null)
        {
            var errors = new List<CCompileError>();
            var pp = new CPreprocessor((name, _) => files != null && files.TryGetValue(name, out var s) ? s : null, errors);
            var toks = pp.Process(src, "t.c");
            Assert.True(errors.Count == 0, string.Join("\n", errors));
            return string.Join(" ", toks.Where(t => t.Kind != CTokenKind.EOF).Select(t => t.Kind == CTokenKind.String ? "\"" + t.Text + "\"" : t.Text));
        }

        private static List<CCompileError> PpErrors(string src)
        {
            var errors = new List<CCompileError>();
            new CPreprocessor((_, _) => null, errors).Process(src, "t.c");
            return errors;
        }

        [Fact]
        public void ObjectLikeMacro() => Assert.Equal("int x = 10 ;", Pp("#define N 10\nint x = N;"));

        [Fact]
        public void MacroExpandsRecursivelyButNotItself() =>
            Assert.Equal("1 + 2 + A", Pp("#define A 1 + B\n#define B 2 + A\nA"));

        [Fact]
        public void FunctionLikeMacro() =>
            Assert.Equal("( ( 3 ) * ( 3 ) )", Pp("#define SQ(x) ((x) * (x))\nSQ(3)"));

        [Fact]
        public void FunctionLikeMacroWithoutParensIsNotExpanded() =>
            Assert.Equal("SQ + 1", Pp("#define SQ(x) ((x)*(x))\nSQ + 1"));

        [Fact]
        public void ObjectLikeMacroWhoseBodyStartsWithParen() =>
            Assert.Equal("( 1 ) ( 2 )", Pp("#define P (1)\nP(2)"));

        [Fact]
        public void NestedParenthesesAndCommasInArguments() =>
            Assert.Equal("f ( a , b ) + ( c )", Pp("#define ADD(x, y) x + y\nADD(f(a, b), (c))"));

        [Fact]
        public void ArgumentsArePreExpanded() =>
            Assert.Equal("( 5 + 1 )", Pp("#define FIVE 5\n#define INC(x) (x + 1)\nINC(FIVE)"));

        [Fact]
        public void Stringification() =>
            Assert.Equal("\"a + b\" \"x\"", Pp("#define STR(x) #x\nSTR(a + b) STR(x)"));

        [Fact]
        public void TokenPasting() =>
            Assert.Equal("int var_12 = 12 ;", Pp("#define CAT(a, b) a ## b\nint CAT(var_, 12) = CAT(1, 2);"));

        [Fact]
        public void VariadicMacro() =>
            Assert.Equal("f ( 1 , 2 , 3 )", Pp("#define CALL(fn, ...) fn(__VA_ARGS__)\nCALL(f, 1, 2, 3)"));

        [Fact]
        public void MacroSpanningLines() =>
            Assert.Equal("1 + 2", Pp("#define ADD(a, b) a + b\nADD(1,\n 2)"));

        [Fact]
        public void Undef() => Assert.Equal("N", Pp("#define N 1\n#undef N\nN"));

        [Theory]
        [InlineData("#if 1\na\n#else\nb\n#endif", "a")]
        [InlineData("#if 0\na\n#else\nb\n#endif", "b")]
        [InlineData("#if 0\na\n#elif 2 > 1\nb\n#else\nc\n#endif", "b")]
        [InlineData("#if 0\na\n#elif 0\nb\n#else\nc\n#endif", "c")]
        [InlineData("#define X\n#ifdef X\na\n#endif", "a")]
        [InlineData("#ifndef X\na\n#endif", "a")]
        [InlineData("#define V 3\n#if V * 2 == 6 && defined(V) && !defined W\nok\n#endif", "ok")]
        [InlineData("#if UNDEFINED_NAME == 0\nzero\n#endif", "zero")]
        [InlineData("#if (1 ? 2 : 3) == 2 && (7 % 4) == 3 && (1 << 4) == 16\nok\n#endif", "ok")]
        [InlineData("#if 0\n#if 1\nx\n#endif\ny\n#else\nz\n#endif", "z")]
        [InlineData("#if 0\n#error never reached\n#endif\nok", "ok")]
        [InlineData("#if -1 < 0\nneg\n#endif", "neg")]
        public void Conditionals(string src, string expected) => Assert.Equal(expected, Pp(src));

        [Fact]
        public void IncludeFromResolver()
        {
            var files = new Dictionary<string, string> { ["a.h"] = "#define FROM_A 7\nint a;", ["b.h"] = "#include \"a.h\"\nint b = FROM_A;" };
            Assert.Equal("int a ; int b = 7 ;", Pp("#include \"b.h\"", files));
        }

        [Fact]
        public void IncludeGuardsPreventDoubleInclusion()
        {
            var files = new Dictionary<string, string> { ["g.h"] = "#ifndef G_H\n#define G_H\nint g;\n#endif" };
            Assert.Equal("int g ;", Pp("#include \"g.h\"\n#include \"g.h\"", files));
        }

        [Fact]
        public void PragmaOnce()
        {
            var files = new Dictionary<string, string> { ["o.h"] = "#pragma once\nint o;" };
            Assert.Equal("int o ;", Pp("#include \"o.h\"\n#include <o.h>", files));
        }

        [Fact]
        public void BuiltinMacroIsDefined() => Assert.Equal("yes", Pp("#ifdef __C93__\nyes\n#endif"));

        [Theory]
        [InlineData("#error stop here", "#error stop here")]
        [InlineData("#include \"missing.h\"", "Cannot find include file 'missing.h'")]
        [InlineData("#if 1\nx", "Unterminated conditional")]
        [InlineData("#endif", "#endif without #if")]
        [InlineData("#else", "#else without #if")]
        [InlineData("#if 1\n#else\n#else\n#endif", "Duplicate #else")]
        [InlineData("#define F(a, b) a\nF(1)", "expects 2 argument(s) but got 1")]
        [InlineData("#define F(a) a\nF(1", "Unterminated argument list")]
        [InlineData("#foo", "Unknown preprocessor directive")]
        [InlineData("#if 1 / 0\n#endif", "Division by zero")]
        [InlineData("#define\n", "#define expects a macro name")]
        [InlineData("#define F(a,\n", "Missing ')'")]
        public void PreprocessorErrors(string src, string message)
        {
            var errors = PpErrors(src);
            Assert.True(errors.Any(e => e.Message.Contains(message)), $"Expected '{message}', got: {string.Join("; ", errors)}");
        }

        [Fact]
        public void IncludeRecursionIsBounded()
        {
            var errors = new List<CCompileError>();
            new CPreprocessor((n, _) => "#include \"self.h\"", errors).Process("#include \"self.h\"", "t.c");
            Assert.Contains(errors, e => e.Message.Contains("nested too deeply"));
        }

        [Fact]
        public void MacrosWorkInsidePrograms()
        {
            const string src = @"
#define MAX(a, b) ((a) > (b) ? (a) : (b))
#define SQUARE(x) ((x) * (x))
#define ARRAY_LEN(a) (sizeof(a) / sizeof((a)[0]))
#define SWAP(t, a, b) do { t _tmp = (a); (a) = (b); (b) = _tmp; } while (0)
int main(void) {
    int arr[] = { 3, 9, 4 };
    int x = 2, y = 5;
    SWAP(int, x, y);
    return MAX(arr[0], arr[1]) * 1000 + SQUARE(x) * 10 + (int)ARRAY_LEN(arr) + y * 0;
}";
            Assert.Equal(9000 + 250 + 3, Eval(src));
        }
    }
}
