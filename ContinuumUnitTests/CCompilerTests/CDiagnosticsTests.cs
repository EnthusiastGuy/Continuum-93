using static ContinuumUnitTests.CCompilerTests.CTestHarness;

namespace ContinuumUnitTests.CCompilerTests
{
    [Collection("CCompiler")]
    public class CDiagnosticsTests
    {
        [Theory]
        // Names and declarations
        [InlineData("int main(void) { return x; }", "Undeclared identifier 'x'")]
        [InlineData("int main(void) { return f(); }", "Implicit declaration of function 'f'")]
        [InlineData("int main(void) { int a; int a; return 0; }", "Redefinition of 'a'")]
        [InlineData("int f(void) { return 1; } int f(void) { return 2; } int main(void) { return 0; }", "Redefinition of function 'f'")]
        [InlineData("int f(int); char f(int); int main(void) { return 0; }", "Conflicting types for 'f'")]
        [InlineData("int g = 1; int g = 2; int main(void) { return 0; }", "Redefinition of 'g'")]
        [InlineData("int main(void) { void v; return 0; }", "declared void")]
        [InlineData("struct S; int main(void) { struct S s; return 0; }", "incomplete type")]
        [InlineData("int main(void) { struct { int a; int a; } s; return 0; }", "Duplicate member 'a'")]
        [InlineData("int helper(void); int main(void) { return helper(); }", "Undefined reference to 'helper'")]
        [InlineData("int x;", "Undefined reference to 'main'")]
        // Types and expressions
        [InlineData("int main(void) { int a; return *a; }", "Cannot dereference non-pointer type 'int'")]
        [InlineData("int main(void) { void *p = 0; return *p; }", "Dereferencing a 'void *' pointer")]
        [InlineData("int main(void) { int *p = 0; int *q = 0; return p + q; }", "Invalid operands to '+'")]
        [InlineData("int main(void) { int *p = 0; char *q = 0; return p - q; }", "Subtracting incompatible pointer types")]
        [InlineData("int main(void) { int *p; char *c = 0; p = c; return 0; }", "Incompatible pointer types in assignment")]
        [InlineData("int main(void) { int *p = 5; return 0; }", "Making a pointer from an integer without a cast")]
        [InlineData("int main(void) { int x = \"str\"; return 0; }", "Making an integer from a pointer without a cast")]
        [InlineData("int main(void) { int *p = 0; return p == 1; }", "Comparison between pointer and integer")]
        [InlineData("int main(void) { 5 = 3; return 0; }", "not assignable")]
        [InlineData("int main(void) { int a[3], b[3]; a = b; return 0; }", "Array type is not assignable")]
        [InlineData("int main(void) { return &5; }", "Cannot take the address of an rvalue")]
        [InlineData("int main(void) { int a; return a.x; }", "is not a struct or union")]
        [InlineData("struct P { int x; }; int main(void) { struct P p; return p.y; }", "No member named 'y'")]
        [InlineData("int main(void) { int a = 1; return a(); }", "Called object is not a function")]
        [InlineData("int f(int a, int b) { return a; } int main(void) { return f(1); }", "Wrong number of arguments to 'f'")]
        [InlineData("int f(int a) { return a; } int main(void) { return f(1, 2); }", "Wrong number of arguments to 'f'")]
        [InlineData("void f(void) { } int main(void) { int x = f(); return 0; }", "Void value not ignored")]
        [InlineData("void f(void) { return 1; } int main(void) { return 0; }", "Returning a value from a void function")]
        [InlineData("int f(void) { return; } int main(void) { return 0; }", "Non-void function must return a value")]
        [InlineData("struct S { int a; }; int main(void) { struct S s; return s + 1; }", "Struct value used where a scalar is required")]
        [InlineData("struct S { int a; }; int main(void) { struct S s; if (s) return 1; return 0; }", "scalar condition")]
        [InlineData("int main(void) { return 1 / 0; }", "Division by zero")]
        [InlineData("int main(void) { int *p = 0; return ~p; }", "Invalid operand to '~'")]
        [InlineData("int main(void) { return sizeof(void (int)); }", "sizeof")]
        // Statements
        [InlineData("int main(void) { break; }", "'break' statement not within a loop or switch")]
        [InlineData("int main(void) { continue; }", "'continue' statement not within a loop")]
        [InlineData("int main(void) { case 1: return 0; }", "'case' label not within a switch")]
        [InlineData("int main(void) { switch (1) { case 1: case 1: break; } return 0; }", "Duplicate case value 1")]
        [InlineData("int main(void) { switch (1) { default: default: break; } return 0; }", "Multiple default labels")]
        [InlineData("int main(void) { int v = 2; switch (v) { case v: break; } return 0; }", "Case label must be an integer constant")]
        [InlineData("int main(void) { goto nowhere; }", "Label 'nowhere' used but not defined")]
        [InlineData("int main(void) { a: a: return 0; }", "Duplicate label 'a'")]
        [InlineData("int main(void) { int n = 3; int arr[n]; return 0; }", "variable-length arrays are not supported")]
        [InlineData("int g = 5; int h = g; int main(void) { return 0; }", "not a compile-time constant")]
        [InlineData("int main(void) { int a[2] = {1, 2, 3}; return 0; }", "Excess elements in initializer")]
        [InlineData("int main(void) { char s[2] = \"long\"; return 0; }", "too long")]
        // Syntax
        [InlineData("int main(void) { return 0 }", "Expected ';'")]
        [InlineData("int main(void) { return (1 + 2; }", "Expected ')'")]
        [InlineData("int main(void) { int x = ; }", "Expected an expression")]
        [InlineData("int main(void) {", "Expected '}' before end of file")]
        [InlineData("int main(void) { if 1 return 0; }", "Expected '('")]
        // Unsupported features give clear messages
        [InlineData("int main(void) { float f = 1; return 0; }", "Floating point types are not supported")]
        [InlineData("int main(void) { long long x = 0; return 0; }", "'long long' is not supported")]
        [InlineData("int main(void) { return 1.5; }", "Floating point is not supported")]
        [InlineData("struct S { int a; }; int f(struct S s) { return 0; } int main(void) { return 0; }", "Passing structs by value is not supported")]
        [InlineData("struct S { int a; }; struct S f(void); int main(void) { return 0; }", "returning structs by value are not supported")]
        [InlineData("int f(int n, ...) { return n; } int main(void) { return 0; }", "variadic functions is not supported")]
        [InlineData("struct S { int a : 3; }; int main(void) { return 0; }", "Bit-fields are not supported")]
        [InlineData("int main(void) { int a[3] = { [1] = 2 }; return 0; }", "Designated initializers are not supported")]
        [InlineData("int main(void) { int *p = (int[]){1, 2}; return 0; }", "Compound literals are not supported")]
        public void ReportsError(string source, string message) => AssertError(source, message);

        [Fact]
        public void ErrorsCarryFileLineAndColumn()
        {
            var errors = CompileErrors("int main(void) {\n    int x = 1;\n    return y;\n}");
            var e = Assert.Single(errors);
            Assert.Equal("main.c", e.File);
            Assert.Equal(3, e.Line);
            Assert.Equal(12, e.Column);
            Assert.Equal("main.c:3:12: error: Undeclared identifier 'y'", e.ToString());
        }

        [Fact]
        public void ErrorsInIncludedFilesReportTheHeader()
        {
            var errors = CompileErrors("#include \"bad.h\"\nint main(void) { return 0; }", new() { ["bad.h"] = "int ok;\nint broken = ;" });
            Assert.Contains(errors, e => e.File == "bad.h" && e.Line == 2);
        }

        [Fact]
        public void ErrorsInMacroExpansionsPointAtTheInvocation()
        {
            var errors = CompileErrors("#define BAD(x) (x + undefined_name)\nint main(void) {\n  return BAD(1);\n}");
            var e = Assert.Single(errors);
            Assert.Equal(3, e.Line);
        }

        [Fact]
        public void CompileLogContainsAllErrors()
        {
            var compiler = new Continuum93.Emulator.Compilers.C93C.CCompiler();
            string asm = compiler.Compile("#error first\n#error second\nint main(void) { return 0; }");
            Assert.Equal("", asm);
            Assert.Equal(2, compiler.Errors);
            Assert.Contains("first", compiler.Log);
            Assert.Contains("second", compiler.Log);
        }
    }
}
