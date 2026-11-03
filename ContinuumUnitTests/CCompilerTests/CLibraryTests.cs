using static ContinuumUnitTests.CCompilerTests.CTestHarness;

namespace ContinuumUnitTests.CCompilerTests
{
    [Collection("CCompiler")]
    public class CLibraryTests
    {
        private static int Lib(string body) => Eval($@"
#include <string.h>
#include <stdlib.h>
#include <ctype.h>
#include <stdint.h>
#include <stdbool.h>
#include <limits.h>
int main(void) {{ {body} }}");

        [Theory]
        [InlineData("return strlen(\"\") + strlen(\"hello\") * 10;", 50)]
        [InlineData("return strcmp(\"abc\", \"abc\") == 0 && strcmp(\"abc\", \"abd\") < 0 && strcmp(\"b\", \"abc\") > 0 && strcmp(\"ab\", \"abc\") < 0;", 1)]
        [InlineData("return strncmp(\"abcX\", \"abcY\", 3) == 0 && strncmp(\"abcX\", \"abcY\", 4) < 0;", 1)]
        [InlineData("return strcmp(\"\\xFF\", \"a\") > 0;", 1)]                                     // compares as unsigned char
        [InlineData("char b[16]; strcpy(b, \"foo\"); strcat(b, \"bar\"); return strcmp(b, \"foobar\") == 0 && strlen(b) == 6;", 1)]
        [InlineData("char b[8]; strncpy(b, \"hi\", 5); return b[0] == 'h' && b[2] == 0 && b[4] == 0;", 1)]
        [InlineData("const char *s = \"hello\"; return strchr(s, 'l') - s + (strchr(s, 'z') == 0) * 10 + (strchr(s, 0) - s) * 100;", 512)]
        [InlineData("char b[10]; memset(b, 'x', 10); b[9] = 0; return strlen(b) == 9 && b[4] == 'x';", 1)]
        [InlineData("int a[4] = {1, 2, 3, 4}, c[4]; memcpy(c, a, sizeof(a)); return c[3] == 4 && memcmp(a, c, 16) == 0;", 1)]
        [InlineData("char s[] = \"abcdef\"; memmove(s + 2, s, 4); return strcmp(s, \"ababcd\") == 0;", 1)]     // overlapping, forward
        [InlineData("char s[] = \"abcdef\"; memmove(s, s + 2, 4); return strcmp(s, \"cdefef\") == 0;", 1)]     // overlapping, backward
        [InlineData("return memcmp(\"abc\", \"abd\", 3) < 0 && memcmp(\"abc\", \"abd\", 2) == 0;", 1)]
        public void StringFunctions(string body, int expected) => Assert.Equal(expected, Lib(body));

        [Theory]
        [InlineData("return abs(-5) + abs(5) * 10 + abs(0);", 55)]
        [InlineData("return atoi(\"  -1234\") == -1234 && atoi(\"+42xyz\") == 42 && atoi(\"0\") == 0 && atoi(\"\") == 0;", 1)]
        [InlineData("srand(1); int a = rand(); srand(1); int b = rand(); return a == b && a >= 0 && a <= RAND_MAX;", 1)]
        [InlineData("int *p = malloc(sizeof(int) * 4); int *q = malloc(4); return (char *)q - (char *)p == 16 && p != 0;", 1)]
        [InlineData("int *p = calloc(8, sizeof(int)); int s = 0; for (int i = 0; i < 8; i++) s += p[i]; free(p); return s;", 0)]
        public void StdlibFunctions(string body, int expected) => Assert.Equal(expected, Lib(body));

        [Fact]
        public void RandMatchesReferenceLcg()
        {
            int r = Lib("srand(12345); int x = 0; for (int i = 0; i < 5; i++) x = x * 31 + rand(); return x;");
            uint state = 12345;
            int expected = 0;
            for (int i = 0; i < 5; i++)
            {
                state = state * 1103515245u + 12345u;
                expected = unchecked(expected * 31 + (int)((state >> 16) & 0x7FFF));
            }
            Assert.Equal(expected, r);
        }

        [Theory]
        [InlineData("return isdigit('7') && !isdigit('a') && isalpha('Q') && !isalpha('1') && isalnum('z') && isspace(' ') && isspace('\\n') && !isspace('x');", 1)]
        [InlineData("return toupper('a') == 'A' && toupper('A') == 'A' && tolower('Z') == 'z' && tolower('5') == '5' && isupper('B') && islower('b');", 1)]
        public void CtypeFunctions(string body, int expected) => Assert.Equal(expected, Lib(body));

        [Theory]
        [InlineData("return sizeof(int8_t) == 1 && sizeof(uint16_t) == 2 && sizeof(int32_t) == 4 && sizeof(size_t) == 4;", 1)]
        [InlineData("int8_t s = -1; uint8_t u = 255; return s == -1 && u == UINT8_MAX && INT32_MIN < 0 && INT32_MAX == 2147483647;", 1)]
        [InlineData("bool b = 7; bool f = false; return b == true && !f && sizeof(bool) == 1;", 1)]
        [InlineData("return INT_MAX + INT_MIN == -1 && UINT_MAX == 4294967295u && CHAR_BIT == 8 && SHRT_MIN == -32768;", 1)]
        [InlineData("int *p = NULL; return p == 0;", 1)]
        public void StandardHeaders(string body, int expected) => Assert.Equal(expected, Lib(body));

        [Fact]
        public void UserDefinitionsOverrideLibraryFunctions() =>
            Assert.Equal(1234, Eval(@"
#include <string.h>
size_t strlen(const char *s) { (void)s; return 1234; }
int main(void) { return (int)strlen(""abc""); }"));

        [Fact]
        public void UserCanDefineLibraryNameWithoutIncludingHeader() =>
            Assert.Equal(-9, Eval("int abs(int x) { return -x; } int main(void) { return abs(9); }"));

        [Fact]
        public void HeapAllocationsDoNotOverlapProgramData()
        {
            var r = Run(@"
#include <stdlib.h>
int marker = 0x5A5A5A5A;
unsigned int first;
int main(void) {
    char *p = malloc(64);
    for (int i = 0; i < 64; i++) p[i] = (char)0xEE;
    first = (unsigned int)p;
    return marker == 0x5A5A5A5A;
}");
            Assert.Equal(1, r.ReturnValue);
            Assert.True(r.GlobalUInt("first") >= r.Labels[".c___heap_start"]);
        }
    }
}
