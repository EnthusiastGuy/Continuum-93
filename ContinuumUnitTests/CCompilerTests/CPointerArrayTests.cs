using static ContinuumUnitTests.CCompilerTests.CTestHarness;

namespace ContinuumUnitTests.CCompilerTests
{
    [Collection("CCompiler")]
    public class CPointerArrayTests
    {
        private static int Body(string body, string top = "") => Eval($"{top}\nint main(void) {{ {body} }}");

        [Theory]
        [InlineData("int x = 5; int *p = &x; *p = 9; return x;", 9)]
        [InlineData("int x = 5; int *p = &x; int **pp = &p; **pp += 1; return x;", 6)]
        [InlineData("int a[4] = {1, 2, 3, 4}; int *p = a; return p[2] + *(p + 3);", 7)]
        [InlineData("int a[4] = {1, 2, 3, 4}; return 2[a];", 3)]                          // a[i] == i[a]
        [InlineData("int a[5]; int *p = &a[1], *q = &a[4]; return q - p;", 3)]           // pointer difference in elements
        [InlineData("int a[5]; int *p = a + 4; return p - a;", 4)]
        [InlineData("int a[5]; return &a[3] - &a[0] + (&a[3] > &a[1]) * 10;", 13)]
        [InlineData("short s[3] = {10, 20, 30}; short *p = s; p++; return *p + *(p + 1);", 50)]
        [InlineData("char c[3] = {1, 2, 3}; char *p = c + 2; p--; return *p;", 2)]
        [InlineData("int a[3] = {5, 6, 7}; int *p = a; int x = *p++; return x * 10 + *p;", 56)]   // *p++ reads then advances
        [InlineData("int a[3] = {5, 6, 7}; int *p = a; *++p = 60; return a[1];", 60)]
        [InlineData("int a[3] = {5, 6, 7}; int *p = a + 2; int x = *p--; return x * 10 + *p;", 76)]
        [InlineData("int a[3] = {1, 2, 3}; int *p = a; p += 2; p -= 1; return *p;", 2)]
        [InlineData("int *p = 0; return p == 0 && !p;", 1)]
        [InlineData("int x; void *v = &x; int *p = v; *p = 33; return x;", 33)]
        [InlineData("int a[10]; return (int)sizeof(a) + (int)sizeof(&a[0]) * 100;", 440)]
        [InlineData("int a[3] = {1, 2, 3}; int (*pa)[3] = &a; return (*pa)[2] + (int)sizeof(*pa);", 15)]
        public void Pointers(string body, int expected) => Assert.Equal(expected, Body(body));

        [Fact]
        public void PointerArithmeticScalesByElementSize()
        {
            int r = Body(@"
struct P { int x; char tag; short y; };   /* 7 bytes, packed */
struct P arr[3];
char *base = (char *)arr;
return (int)((char *)&arr[2] - base) * 100 + (int)((char *)(arr + 1) - base);");
            Assert.Equal(1407, r);
        }

        [Fact]
        public void MultiDimensionalArrays() =>
            Assert.Equal(1 + 6 + 12 + 1000 * 12, Body(@"
int m[3][4];
for (int i = 0; i < 3; i++)
    for (int j = 0; j < 4; j++)
        m[i][j] = i * j;
int (*row)[4] = m + 1;
return m[1][1] + m[2][3] + row[1][2] * 3 + 0 * row[0][0] + (int)sizeof(m) / 4 * 1000;"));

        [Fact]
        public void ArrayInitializers() =>
            Assert.Equal(1 + 0 + 30 + 4 * 100 + 3 * 1000, Body(@"
int a[5] = {1, 2};          /* rest zero-filled */
int b[] = {10, 20, 30, 40}; /* size from initializer */
int m[2][3] = {{1, 2, 3}, {4}};
int n[2][2] = {1, 2, 3};    /* brace elision */
return a[0] + a[4] + b[2] + (int)(sizeof(b) / sizeof(b[0])) * 100 + n[1][0] * 1000 + m[1][1];"));

        [Fact]
        public void StringsAndCharArrays()
        {
            int r = Body(@"
char s[] = ""hello"";
char t[10] = ""hi"";
const char *p = ""world"";
int len = 0;
while (p[len]) len++;
s[0] = 'J';
return s[0] * 1000000 + (int)sizeof(s) * 10000 + t[1] * 100 + t[5] + len;");
            Assert.Equal('J' * 1000000 + 6 * 10000 + 'i' * 100 + 0 + 5, r);
        }

        [Fact]
        public void StringLiteralConcatenationAndEscapes() =>
            Assert.Equal(1, Body(@"
const char *s = ""ab"" ""c\n"" ""\x41"";
return s[0] == 'a' && s[2] == 'c' && s[3] == 10 && s[4] == 'A' && s[5] == 0;"));

        [Fact]
        public void ArraysPassedToFunctionsDecayToPointers() =>
            Assert.Equal(15 + 4 * 100, Eval(@"
int sum(int a[], int n) { int s = 0; for (int i = 0; i < n; i++) s += a[i]; return s + (int)sizeof(a) * 100; }
int main(void) { int v[5] = {1, 2, 3, 4, 5}; return sum(v, 5); }"));

        [Fact]
        public void FunctionsWritingThroughPointers() =>
            Assert.Equal(54321, Eval(@"
void swap(int *a, int *b) { int t = *a; *a = *b; *b = t; }
void reverse(int *a, int n) { for (int i = 0, j = n - 1; i < j; i++, j--) swap(&a[i], &a[j]); }
int main(void) {
    int d[5] = {1, 2, 3, 4, 5};
    reverse(d, 5);
    int r = 0;
    for (int i = 0; i < 5; i++) r = r * 10 + d[i];
    return r;
}"));

        [Fact]
        public void FunctionPointers() =>
            Assert.Equal(7 + 12 + 1 + 100, Eval(@"
int add(int a, int b) { return a + b; }
int mul(int a, int b) { return a * b; }
int sub(int a, int b) { return a - b; }
typedef int (*binop)(int, int);
int apply(binop f, int a, int b) { return f(a, b); }
int (*pick(int which))(int, int) { return which ? mul : add; }
int main(void) {
    binop table[3] = { add, mul, sub };
    int (*fp)(int, int) = &sub;
    return table[0](3, 4) + apply(table[1], 3, 4) + (*fp)(5, 4) + pick(1)(10, 10);
}"));

        [Fact]
        public void PointerToPointerAndArrayOfPointers() =>
            Assert.Equal(3 + 'z' * 10, Eval(@"
const char *words[] = { ""alpha"", ""beta"", ""zeta"" };
int count(const char **w, int n, char c) { int k = 0; for (int i = 0; i < n; i++) if (w[i][0] == c || w[i][1] == c || w[i][3] == c) k++; return k; }
int main(void) { return count(words, 3, 'a') + words[2][0] * 10; }"));

        [Fact]
        public void PointerComparisonsAreUnsigned() =>
            Assert.Equal(1, Body("char *lo = (char *)0x100; char *hi = (char *)0xF00000; return lo < hi && hi > lo && lo != hi;"));

        [Fact]
        public void CastsBetweenPointersAndIntegers() =>
            Assert.Equal(0x123458, Body("int *p = (int *)0x123450; p += 2; return (int)p;"));

        [Fact]
        public void ByteViewOfIntIsBigEndian() =>
            Assert.Equal(0x12 * 1000 + 0x78, Body("unsigned int v = 0x12345678u; unsigned char *b = (unsigned char *)&v; return b[0] * 1000 + b[3];"));

        [Fact]
        public void PointerIncrementOfStructPointer() =>
            Assert.Equal(30, Body("struct S { int a; int b; } s[3] = {{1, 2}, {10, 20}, {100, 200}}; struct S *p = s; p++; return p->a + p->b;"));
    }
}
