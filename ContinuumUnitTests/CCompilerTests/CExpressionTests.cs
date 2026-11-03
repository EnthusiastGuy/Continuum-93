using static ContinuumUnitTests.CCompilerTests.CTestHarness;

namespace ContinuumUnitTests.CCompilerTests
{
    [Collection("CCompiler")]
    public class CExpressionTests
    {
        /// <summary>Evaluates an int expression with a few variables in scope (so operands aren't all constants).</summary>
        private static int E(string expr, string decls = "") => Eval($@"
int main(void) {{
    int a = 7, b = -3, c = 100, z = 0, big = 2147483647;
    unsigned int ua = 7u, ub = 4294967293u, umax = 4294967295u;
    char ch = -5; unsigned char uch = 250; short sh = -1234; unsigned short ush = 60000;
    {decls}
    return (int)({expr});
}}");

        [Theory]
        [InlineData("a + b", 4)]
        [InlineData("a - b", 10)]
        [InlineData("a * b", -21)]
        [InlineData("c / a", 14)]
        [InlineData("c % a", 2)]
        [InlineData("-c / a", -14)]           // truncation toward zero
        [InlineData("-c % a", -2)]            // remainder has the sign of the dividend
        [InlineData("c / b", -33)]
        [InlineData("c % b", 1)]
        [InlineData("b / b", 1)]
        [InlineData("big + 1", int.MinValue)] // wraps (two's complement)
        [InlineData("-big - 2", int.MaxValue)]
        [InlineData("a * 65536 * 65536", 0)]
        [InlineData("1 + 2 * 3 - 4 / 2", 5)]
        [InlineData("(1 + 2) * (3 - 4) / 2", -1)]
        [InlineData("10 - 3 - 2", 5)]          // left associativity
        [InlineData("100 / 10 / 5", 2)]
        [InlineData("-a", -7)]
        [InlineData("+b", -3)]
        [InlineData("- -a", 7)]
        public void SignedArithmetic(string expr, int expected) => Assert.Equal(expected, E(expr));

        [Theory]
        [InlineData("ub / 2u", 2147483646)]     // 4294967293 / 2 (unsigned division)
        [InlineData("ub % 10u", 3)]
        [InlineData("umax / ua", 613566756)]
        [InlineData("ub > ua", 1)]              // unsigned comparison
        [InlineData("b < ua", 0)]               // -3 converts to a huge unsigned value
        [InlineData("b < (int)ua", 1)]
        [InlineData("umax + 1u", 0)]
        [InlineData("0u - 1u == umax", 1)]
        [InlineData("ub >> 28", 15)]            // logical shift for unsigned
        [InlineData("(int)(umax * umax)", 1)]
        [InlineData("ua * 3u", 21)]
        [InlineData("-ua == 4294967289u", 1)]
        public void UnsignedArithmetic(string expr, int expected) => Assert.Equal(expected, E(expr));

        [Theory]
        [InlineData("1 << 31", int.MinValue)]
        [InlineData("a << 4", 112)]
        [InlineData("b >> 1", -2)]              // arithmetic shift right for signed values
        [InlineData("b >> 31", -1)]
        [InlineData("-256 >> 4", -16)]
        [InlineData("c >> 2", 25)]
        [InlineData("a << z", 7)]
        [InlineData("c >> a", 0)]
        [InlineData("b << 2", -12)]
        [InlineData("(int)(0x80000000u >> 31)", 1)]
        [InlineData("1 << a", 128)]
        [InlineData("big >> 30", 1)]
        public void Shifts(string expr, int expected) => Assert.Equal(expected, E(expr));

        [Theory]
        [InlineData("a & 3", 3)]
        [InlineData("a | 8", 15)]
        [InlineData("a ^ 5", 2)]
        [InlineData("~a", -8)]
        [InlineData("~0", -1)]
        [InlineData("b & 0xFF", 253)]
        [InlineData("(int)(umax ^ 0xF0F0F0F0u)", 0x0F0F0F0F)]
        [InlineData("a & b | c ^ 1", 101)]      // precedence: & then ^ then |
        public void Bitwise(string expr, int expected) => Assert.Equal(expected, E(expr));

        [Theory]
        [InlineData("a == 7", 1)]
        [InlineData("a != 7", 0)]
        [InlineData("a < b", 0)]
        [InlineData("b < a", 1)]
        [InlineData("a <= 7", 1)]
        [InlineData("a > 7", 0)]
        [InlineData("a >= 7", 1)]
        [InlineData("b >= -3", 1)]
        [InlineData("big > -big", 1)]
        [InlineData("(a < c) + (c < a) * 2", 1)]
        [InlineData("1 < 2 == 1", 1)]
        public void Comparisons(string expr, int expected) => Assert.Equal(expected, E(expr));

        [Theory]
        [InlineData("a && b", 1)]
        [InlineData("a && z", 0)]
        [InlineData("z || b", 1)]
        [InlineData("z || z", 0)]
        [InlineData("!a", 0)]
        [InlineData("!z", 1)]
        [InlineData("!!c", 1)]
        [InlineData("a > 0 && b < 0 && c == 100", 1)]
        [InlineData("a > 0 && (b > 0 || c == 100)", 1)]
        [InlineData("!(a > 0) || z", 0)]
        public void Logical(string expr, int expected) => Assert.Equal(expected, E(expr));

        [Fact]
        public void LogicalOperatorsShortCircuit() =>
            Assert.Equal(1010, Eval(@"
int calls;
int touch(int v) { calls++; return v; }
int main(void) {
    int r = 0;
    calls = 0;
    if (touch(0) && touch(1)) r += 1;   // second call skipped
    if (touch(1) || touch(1)) r += 10;  // second call skipped
    if (touch(1) && touch(0)) r += 100; // both called
    return r + calls * 250;             // 10 + 4 calls * 250
}"));

        [Theory]
        [InlineData("a > 0 ? 1 : 2", 1)]
        [InlineData("z ? 1 : 2", 2)]
        [InlineData("a > c ? a : b > 0 ? b : c", 100)]   // right associative
        [InlineData("(a, b, c)", 100)]
        [InlineData("z ? ua : b", -3)]                    // common type is unsigned; converts back to int
        public void ConditionalAndComma(string expr, int expected) => Assert.Equal(expected, E(expr));

        [Fact]
        public void ConditionalOnlyEvaluatesOneBranch() =>
            Assert.Equal(1, Eval(@"
int n;
int bump(int v) { n += v; return v; }
int main(void) { int x = 1 ? bump(1) : bump(100); return n + x - 1; }"));

        [Theory]
        [InlineData("ch", -5)]
        [InlineData("uch", 250)]
        [InlineData("sh", -1234)]
        [InlineData("ush", 60000)]
        [InlineData("(char)300", 44)]
        [InlineData("(char)200", -56)]
        [InlineData("(unsigned char)-1", 255)]
        [InlineData("(short)70000", 4464)]
        [InlineData("(short)40000", -25536)]
        [InlineData("(unsigned short)-2", 65534)]
        [InlineData("(unsigned char)ch", 251)]
        [InlineData("(signed char)uch", -6)]
        [InlineData("(unsigned short)sh", 64302)]
        [InlineData("(short)ush", -5536)]
        [InlineData("ch + uch", 245)]                 // promoted to int before adding
        [InlineData("ch * 2", -10)]
        [InlineData("uch * uch", 62500)]              // no 8-bit overflow: promoted to int
        [InlineData("(_Bool)5 + (_Bool)0", 1)]
        [InlineData("(_Bool)256", 1)]                 // _Bool normalizes rather than truncates
        [InlineData("(int)(char)(unsigned char)128", -128)]
        public void ConversionsAndPromotions(string expr, int expected) => Assert.Equal(expected, E(expr));

        [Theory]
        [InlineData("ch = 130", -126)]                // assignment converts to the target type
        [InlineData("uch = 300", 44)]
        [InlineData("sh = 32768", -32768)]
        [InlineData("ush = 70000", 4464)]
        [InlineData("a = b = 9", 9)]
        [InlineData("(a = 3) * 2", 6)]
        public void AssignmentValueHasTargetType(string expr, int expected) => Assert.Equal(expected, E(expr));

        [Theory]
        [InlineData("a += 3", "a", 10)]
        [InlineData("a -= 10", "a", -3)]
        [InlineData("a *= b", "a", -21)]
        [InlineData("c /= a", "c", 14)]
        [InlineData("c %= a", "c", 2)]
        [InlineData("a <<= 2", "a", 28)]
        [InlineData("b >>= 1", "b", -2)]
        [InlineData("a &= 5", "a", 5)]
        [InlineData("a |= 8", "a", 15)]
        [InlineData("a ^= 1", "a", 6)]
        [InlineData("uch += 10", "uch", 4)]           // wraps in unsigned char
        [InlineData("ch -= 125", "ch", 126)]          // -130 wraps to 126 in signed char
        [InlineData("ush *= 2", "ush", 54464)]
        [InlineData("ub /= 2u", "(int)ub", 2147483646)]
        public void CompoundAssignment(string stmt, string result, int expected) =>
            Assert.Equal(expected, E(result, stmt + ";"));

        [Theory]
        [InlineData("a++", 7, 8)]
        [InlineData("++a", 8, 8)]
        [InlineData("a--", 7, 6)]
        [InlineData("--a", 6, 6)]
        public void IncrementDecrement(string expr, int value, int after)
        {
            Assert.Equal(value, E("r", $"int r = {expr};"));
            Assert.Equal(after, E("a", $"int r = {expr};"));
        }

        [Theory]
        [InlineData("uch", "uch++", 250, 251)]
        [InlineData("x", "x++", 255, 0)]               // unsigned char wraps
        [InlineData("y", "y--", -128, 127)]            // signed char wraps
        public void IncrementOfNarrowTypes(string var, string expr, int value, int after)
        {
            string decls = "unsigned char x = 255; signed char y = -128;";
            Assert.Equal(value, E("r", decls + $"int r = {expr};"));
            Assert.Equal(after, E(var, decls + $"{expr};"));
        }

        [Theory]
        [InlineData("sizeof(char)", 1)]
        [InlineData("sizeof(short)", 2)]
        [InlineData("sizeof(int)", 4)]
        [InlineData("sizeof(long)", 4)]
        [InlineData("sizeof(int *)", 4)]
        [InlineData("sizeof(char[10])", 10)]
        [InlineData("sizeof(int[3][4])", 48)]
        [InlineData("sizeof a", 4)]
        [InlineData("sizeof ch", 1)]
        [InlineData("sizeof(ch + 1)", 4)]              // promoted
        [InlineData("sizeof \"hello\"", 6)]
        [InlineData("sizeof(_Bool)", 1)]
        [InlineData("sizeof(struct { char c; int i; })", 5)]   // structs are packed
        public void Sizeof(string expr, int expected) => Assert.Equal(expected, E(expr));

        [Fact]
        public void SizeofDoesNotEvaluateItsOperand() =>
            Assert.Equal(7, E("a + (int)sizeof(a++) - 4"));

        [Theory]
        [InlineData("'A'", 65)]
        [InlineData("'\\n'", 10)]
        [InlineData("'\\xFF'", -1)]          // plain char is signed
        [InlineData("0x7fffffff", int.MaxValue)]
        [InlineData("0xFFFFFFFF == umax", 1)]
        [InlineData("0xFFFFFFFF > 0", 1)]   // hex literal is unsigned int
        [InlineData("-1 < 0u", 0)]          // -1 converts to unsigned
        [InlineData("017", 15)]
        [InlineData("0b101", 5)]
        public void Literals(string expr, int expected) => Assert.Equal(expected, E(expr));

        [Fact]
        public void LargeImmediatesAreNotTruncated() =>
            Assert.Equal(1, E("(a + 0x12345678) == 0x1234567F && (c * 100000) == 10000000 && (c - 70000) == -69900 && (ua | 0x10000000u) == 0x10000007u"));

        [Fact]
        public void DeepExpressionNesting()
        {
            // ((((1+1)+1)+...)) 200 levels deep and a right-leaning chain that exercises the register stack
            string left = string.Concat(Enumerable.Repeat("(", 200)) + "1" + string.Concat(Enumerable.Repeat("+1)", 200));
            string right = string.Join("+(", Enumerable.Repeat("a", 60)) + new string(')', 59);
            Assert.Equal(201, E(left));
            Assert.Equal(420, E(right));
        }
    }
}
