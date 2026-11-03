using static ContinuumUnitTests.CCompilerTests.CTestHarness;

namespace ContinuumUnitTests.CCompilerTests
{
    [Collection("CCompiler")]
    public class CGlobalTests
    {
        [Fact]
        public void GlobalInitializersAreLaidOutInMemory()
        {
            var r = Run(@"
int counter = 42;
int negative = -7;
unsigned char bytes[4] = { 1, 2, 0xFF };
short halves[2] = { -2, 300 };
char greeting[] = ""Hi!"";
int zeroed[3];
int main(void) { return 0; }");

            Assert.Equal(42, r.GlobalInt("counter"));
            Assert.Equal(-7, r.GlobalInt("negative"));
            Assert.Equal(new byte[] { 1, 2, 0xFF, 0 }, r.Read(r.AddressOf("bytes"), 4));
            Assert.Equal(new byte[] { 0xFF, 0xFE, 0x01, 0x2C }, r.Read(r.AddressOf("halves"), 4));
            Assert.Equal("Hi!", r.GlobalString("greeting"));
            Assert.Equal(0, r.GlobalInt("zeroed", 2));
        }

        [Fact]
        public void ProgramsCanWriteGlobalsReadByTheHost()
        {
            var r = Run(@"
int squares[10];
int main(void) { for (int i = 0; i < 10; i++) squares[i] = i * i; return 0; }");
            for (int i = 0; i < 10; i++)
                Assert.Equal(i * i, r.GlobalInt("squares", i));
        }

        [Fact]
        public void AddressConstantsInGlobalInitializers() =>
            Assert.Equal(1, Eval(@"
int target = 5;
int arr[4] = { 10, 20, 30, 40 };
int *p = &target;
int *mid = &arr[2];
int *end = arr + 4;
const char *msg = ""text"";
struct Pair { int *a; int b; } pair = { &arr[1], 9 };
int (*fn)(void);
int seven(void) { return 7; }
int (*fptr)(void) = seven;
int main(void) {
    return *p == 5 && *mid == 30 && end - arr == 4 && end[-1] == 40 && msg[2] == 'x'
        && *pair.a == 20 && pair.b == 9 && fn == 0 && fptr() == 7;
}"));

        [Fact]
        public void ConstantExpressionsInInitializersAndArraySizes() =>
            Assert.Equal(1, Eval(@"
#define N 4
enum { SIZE = N * 2 + 1 };
int table[SIZE];
int computed = (1 << 10) | (0x0F & 3) + sizeof(int) * 2;
unsigned int mask = ~0u >> 4;
char c = 'A' + 2;
int conditional = SIZE > 5 ? 100 : 200;
int main(void) {
    return sizeof(table) == 36 && computed == (1024 | 11) && mask == 0x0FFFFFFFu && c == 'C' && conditional == 100;
}"));

        [Fact]
        public void StaticLocalsPersistAcrossCalls() =>
            Assert.Equal(1 + 2 + 3 + 100 + 1000, Eval(@"
int next(void) { static int n; return ++n; }
int other(void) { static int n = 99; return ++n; }   /* separate storage despite the same name */
int main(void) {
    int a = next(), b = next(), c = next();
    static int once = 1000;
    return a + b + c + other() + once;
}"));

        [Fact]
        public void ExternDeclarationsAndTentativeDefinitions() =>
            Assert.Equal(13, Eval(@"
extern int later;
int twice;
int twice;                  /* tentative definitions merge */
int use(void) { extern int later; return later + twice; }
int later = 10;
int main(void) { twice = 3; return use(); }"));

        [Fact]
        public void FunctionPrototypesAllowForwardCallsAndMutualRecursion() =>
            Assert.Equal(1, Eval(@"
int is_even(int n);
int is_odd(int n) { return n == 0 ? 0 : is_even(n - 1); }
int is_even(int n) { return n == 0 ? 1 : is_odd(n - 1); }
int main(void) { return is_even(10) && is_odd(7) && !is_odd(4); }"));

        [Fact]
        public void UnusedFunctionsAreNotEmitted()
        {
            var r = Run(@"
int used(void) { return 1; }
int unused(void) { return 2; }
int main(void) { return used(); }");
            Assert.True(r.Labels.ContainsKey(".c_used"));
            Assert.False(r.Labels.ContainsKey(".c_unused"));
            Assert.False(r.Labels.ContainsKey(".c_strlen"));   // library functions too
        }

        [Fact]
        public void MainReceivesZeroArgcArgv() =>
            Assert.Equal(0, Eval("int main(int argc, char **argv) { return argc + (argv != 0); }"));

        [Fact]
        public void InlineAssemblyCanAccessGlobals()
        {
            var r = Run(@"
int value = 5;
int main(void) {
    asm(""LD ABCD, (.c_value)\nADD32 ABCD, 10\nLD (.c_value), ABCD"");
    __asm__ volatile (""LD EFGH, 0"" "" ; adjacent strings concatenate"");
    return value;
}");
            Assert.Equal(15, r.ReturnValue);
            Assert.Equal(15, r.GlobalInt("value"));
        }

        [Fact]
        public void InlineAssemblyCanCallCFunctionsByLabel() =>
            Assert.Equal(99, Eval(@"
int result;
void set(void) { result = 99; }
int main(void) { asm(""CALL .c_set""); return result; }"));

        [Fact]
        public void InlineAssemblyCanUseMachineInterrupts()
        {
            // INT 0x00 / 0x03: read the machine clock (milliseconds) into memory; just checks it runs end to end
            var r = Run(@"
unsigned int clock_ms;
int main(void) {
    asm(""LD A, 0x03\nLD B, 0x00\nLD CDE, .c_clock_ms\nINT 0x00, A"");
    return 1;
}");
            Assert.Equal(1, r.ReturnValue);
        }
    }
}
