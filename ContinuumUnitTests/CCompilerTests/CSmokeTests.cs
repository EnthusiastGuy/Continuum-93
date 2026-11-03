using static ContinuumUnitTests.CCompilerTests.CTestHarness;

namespace ContinuumUnitTests.CCompilerTests
{
    [Collection("CCompiler")]
    public class CSmokeTests
    {
        [Fact]
        public void ReturnsConstant() => Assert.Equal(42, Eval("int main(void) { return 42; }"));

        [Fact]
        public void FallingOffMainReturnsZero() => Assert.Equal(0, Eval("int main(void) { }"));

        [Fact]
        public void LocalsAndArithmetic() =>
            Assert.Equal(17, Eval("int main(void) { int a = 3; int b = 4; return a * b + 5; }"));

        [Fact]
        public void FunctionCall() =>
            Assert.Equal(9, Eval("int add(int a, int b) { return a + b; } int main(void) { return add(4, 5); }"));

        [Fact]
        public void RecursiveFibonacci() =>
            Assert.Equal(610, Eval("int fib(int n) { return n < 2 ? n : fib(n - 1) + fib(n - 2); } int main(void) { return fib(15); }"));
    }
}
