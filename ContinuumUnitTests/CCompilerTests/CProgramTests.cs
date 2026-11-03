using static ContinuumUnitTests.CCompilerTests.CTestHarness;

namespace ContinuumUnitTests.CCompilerTests
{
    /// <summary>Whole programs whose results are checked against known answers or C# re-implementations.</summary>
    [Collection("CCompiler")]
    public class CProgramTests
    {
        [Fact]
        public void SieveOfEratosthenes()
        {
            var r = Run(@"
#define LIMIT 2000
char composite[LIMIT + 1];
int primes[400];
int main(void) {
    int count = 0;
    for (int i = 2; i <= LIMIT; i++) {
        if (composite[i]) continue;
        primes[count++] = i;
        for (int j = i * i; j <= LIMIT; j += i) composite[j] = 1;
    }
    return count;
}");
            Assert.Equal(303, r.ReturnValue);                 // π(2000) = 303
            Assert.Equal(1999, r.GlobalInt("primes", 302));
            Assert.Equal(2, r.GlobalInt("primes", 0));
        }

        [Fact]
        public void QuicksortAndInsertionSortAgreeWithDotNet()
        {
            var r = Run(@"
#define N 200
int a[N], b[N];
unsigned int seed = 2024;
int next_rand(void) { seed = seed * 1664525u + 1013904223u; return (int)(seed >> 8) % 10000 - 5000; }
void swap(int *x, int *y) { int t = *x; *x = *y; *y = t; }
void quicksort(int *v, int lo, int hi) {
    if (lo >= hi) return;
    int pivot = v[(lo + hi) / 2], i = lo, j = hi;
    while (i <= j) {
        while (v[i] < pivot) i++;
        while (v[j] > pivot) j--;
        if (i <= j) { swap(&v[i], &v[j]); i++; j--; }
    }
    quicksort(v, lo, j);
    quicksort(v, i, hi);
}
void insertion_sort(int *v, int n) {
    for (int i = 1; i < n; i++) {
        int key = v[i], j = i - 1;
        while (j >= 0 && v[j] > key) { v[j + 1] = v[j]; j--; }
        v[j + 1] = key;
    }
}
int main(void) {
    for (int i = 0; i < N; i++) a[i] = b[i] = next_rand();
    quicksort(a, 0, N - 1);
    insertion_sort(b, N);
    for (int i = 0; i < N; i++) if (a[i] != b[i]) return -1;
    return 1;
}", maxSteps: 50_000_000);

            Assert.Equal(1, r.ReturnValue);
            uint seed = 2024;
            var expected = new List<int>();
            for (int i = 0; i < 200; i++)
            {
                seed = seed * 1664525u + 1013904223u;
                expected.Add((int)(seed >> 8) % 10000 - 5000);
            }
            expected.Sort();
            for (int i = 0; i < 200; i++)
                Assert.Equal(expected[i], r.GlobalInt("a", i));
        }

        [Fact]
        public void EightQueensHas92Solutions() =>
            Assert.Equal(92, Eval(@"
int cols[8];
int safe(int row, int col) {
    for (int r = 0; r < row; r++) {
        int c = cols[r];
        if (c == col || c - col == r - row || c - col == row - r) return 0;
    }
    return 1;
}
int solve(int row) {
    if (row == 8) return 1;
    int n = 0;
    for (int c = 0; c < 8; c++) if (safe(row, c)) { cols[row] = c; n += solve(row + 1); }
    return n;
}
int main(void) { return solve(0); }"));

        [Fact]
        public void Crc32MatchesReference()
        {
            var r = Run(@"
#include <string.h>
unsigned int table[256];
void make_table(void) {
    for (unsigned int n = 0; n < 256; n++) {
        unsigned int c = n;
        for (int k = 0; k < 8; k++) c = (c & 1) ? 0xEDB88320u ^ (c >> 1) : c >> 1;
        table[n] = c;
    }
}
unsigned int crc32(const unsigned char *buf, unsigned int len) {
    unsigned int c = 0xFFFFFFFFu;
    for (unsigned int i = 0; i < len; i++) c = table[(c ^ buf[i]) & 0xFF] ^ (c >> 8);
    return c ^ 0xFFFFFFFFu;
}
unsigned int result;
int main(void) {
    const char *text = ""The quick brown fox jumps over the lazy dog"";
    make_table();
    result = crc32((const unsigned char *)text, strlen(text));
    return 0;
}", maxSteps: 50_000_000);
            Assert.Equal(0x414FA339u, r.GlobalUInt("result"));   // well-known CRC-32 of this sentence
        }

        [Fact]
        public void TowersOfHanoiMoveCount() =>
            Assert.Equal(1023, Eval(@"
int moves;
void hanoi(int n, int from, int to, int via) {
    if (n == 0) return;
    hanoi(n - 1, from, via, to);
    moves++;
    hanoi(n - 1, via, to, from);
}
int main(void) { hanoi(10, 1, 3, 2); return moves; }"));

        [Fact]
        public void AckermannDeepRecursion() =>
            Assert.Equal(61, Eval(@"
int ack(int m, int n) {
    if (m == 0) return n + 1;
    if (n == 0) return ack(m - 1, 1);
    return ack(m - 1, ack(m, n - 1));
}
int main(void) { return ack(2, 3) * 0 + ack(3, 3); }"));

        [Fact]
        public void GcdLcmAndCollatz() =>
            Assert.Equal(1, Eval(@"
unsigned int gcd(unsigned int a, unsigned int b) { while (b) { unsigned int t = a % b; a = b; b = t; } return a; }
unsigned int lcm(unsigned int a, unsigned int b) { return a / gcd(a, b) * b; }
int collatz_steps(unsigned int n) { int s = 0; while (n != 1) { n = (n & 1) ? 3 * n + 1 : n / 2; s++; } return s; }
int main(void) {
    return gcd(1071, 462) == 21 && lcm(4, 6) == 12 && gcd(4000000000u, 3000000000u) == 1000000000u && collatz_steps(27) == 111;
}"));

        [Fact]
        public void StringReverseWordCountAndPalindromes()
        {
            var r = Run(@"
#include <string.h>
#include <ctype.h>
char buf[64];
void reverse(char *s) { for (int i = 0, j = strlen(s) - 1; i < j; i++, j--) { char t = s[i]; s[i] = s[j]; s[j] = t; } }
int words(const char *s) { int n = 0, in = 0; for (; *s; s++) { if (isspace(*s)) in = 0; else if (!in) { in = 1; n++; } } return n; }
int is_palindrome(const char *s) {
    const char *e = s + strlen(s) - 1;
    while (s < e) {
        if (!isalpha(*s)) { s++; continue; }
        if (!isalpha(*e)) { e--; continue; }
        if (tolower(*s++) != tolower(*e--)) return 0;
    }
    return 1;
}
int main(void) {
    strcpy(buf, ""Continuum 93"");
    reverse(buf);
    return words(""  the quick  brown fox "") * 10 + is_palindrome(""A man, a plan, a canal: Panama"") + is_palindrome(""nope"") * 100;
}");
            Assert.Equal(41, r.ReturnValue);
            Assert.Equal("39 muunitnoC", r.GlobalString("buf"));
        }

        [Fact]
        public void MatrixMultiplication()
        {
            var r = Run(@"
#define N 6
int A[N][N], B[N][N], C[N][N];
int main(void) {
    for (int i = 0; i < N; i++)
        for (int j = 0; j < N; j++) { A[i][j] = i + j; B[i][j] = i - j; }
    for (int i = 0; i < N; i++)
        for (int j = 0; j < N; j++) {
            int s = 0;
            for (int k = 0; k < N; k++) s += A[i][k] * B[k][j];
            C[i][j] = s;
        }
    return C[N - 1][0];
}");
            int[,] a = new int[6, 6], b = new int[6, 6];
            for (int i = 0; i < 6; i++)
                for (int j = 0; j < 6; j++) { a[i, j] = i + j; b[i, j] = i - j; }
            for (int i = 0; i < 6; i++)
                for (int j = 0; j < 6; j++)
                {
                    int s = 0;
                    for (int k = 0; k < 6; k++) s += a[i, k] * b[k, j];
                    Assert.Equal(s, r.GlobalInt("C", i * 6 + j));
                }
        }

        [Fact]
        public void BinarySearchAndBitCounting() =>
            Assert.Equal(1, Eval(@"
int bsearch_idx(const int *v, int n, int key) {
    int lo = 0, hi = n - 1;
    while (lo <= hi) {
        int mid = lo + (hi - lo) / 2;
        if (v[mid] == key) return mid;
        if (v[mid] < key) lo = mid + 1; else hi = mid - 1;
    }
    return -1;
}
int popcount(unsigned int x) { int n = 0; while (x) { x &= x - 1; n++; } return n; }
unsigned int reverse_bits(unsigned int x) { unsigned int r = 0; for (int i = 0; i < 32; i++) { r = (r << 1) | (x & 1); x >>= 1; } return r; }
int main(void) {
    int v[] = { -40, -3, 0, 7, 19, 42, 100, 1000 };
    return bsearch_idx(v, 8, 42) == 5 && bsearch_idx(v, 8, -40) == 0 && bsearch_idx(v, 8, 5) == -1
        && popcount(0xFFFFFFFFu) == 32 && popcount(0x80000001u) == 2 && reverse_bits(1u) == 0x80000000u
        && reverse_bits(0x0000FFFFu) == 0xFFFF0000u;
}"));

        [Fact]
        public void StateMachineTokenizer() =>
            Assert.Equal(6 * 100 + 3 + 4, Eval(@"      /* tokens: abc 123 de 4 5 f; numbers: 123 4 5 */
enum State { START, IN_NUMBER, IN_WORD };
int main(void) {
    const char *input = ""abc 123 de 4 5f"";
    enum State st = START;
    int numbers = 0, tokens = 0;
    for (const char *p = input; ; p++) {
        char c = *p;
        int digit = c >= '0' && c <= '9', alpha = c >= 'a' && c <= 'z';
        switch (st) {
            case START:
                if (digit) { st = IN_NUMBER; tokens++; numbers++; }
                else if (alpha) { st = IN_WORD; tokens++; }
                break;
            case IN_NUMBER:
                if (alpha) { st = IN_WORD; tokens++; }    /* '5f' splits into a number and a word */
                else if (!digit) st = START;
                break;
            case IN_WORD:
                if (!alpha && !digit) st = START;
                break;
        }
        if (!c) break;
    }
    return tokens * 100 + numbers + 4;
}"));

        [Fact]
        public void BytecodeInterpreter() =>
            Assert.Equal(3628800, Eval(@"
/* A tiny stack VM computing 10! with a loop */
enum Op { PUSH, LOAD, STORE, MUL, SUBI, JNZ, HALT };
int code[] = {
    PUSH, 1, STORE, 0,          /* acc = 1  */
    PUSH, 10, STORE, 1,         /* n = 10   */
    LOAD, 0, LOAD, 1, MUL, STORE, 0,   /* acc *= n (pc 8) */
    LOAD, 1, SUBI, 1, STORE, 1, /* n -= 1   */
    LOAD, 1, JNZ, 8,            /* loop while n != 0 */
    LOAD, 0, HALT
};
int run(void) {
    int stack[16], sp = 0, vars[2], pc = 0;
    for (;;) {
        switch (code[pc++]) {
            case PUSH: stack[sp++] = code[pc++]; break;
            case LOAD: stack[sp++] = vars[code[pc++]]; break;
            case STORE: vars[code[pc++]] = stack[--sp]; break;
            case MUL: sp--; stack[sp - 1] *= stack[sp]; break;
            case SUBI: stack[sp - 1] -= code[pc++]; break;
            case JNZ: { int target = code[pc++]; if (stack[--sp]) pc = target; break; }
            case HALT: return stack[sp - 1];
        }
    }
}
int main(void) { return run(); }"));
    }
}
