using static ContinuumUnitTests.CCompilerTests.CTestHarness;

namespace ContinuumUnitTests.CCompilerTests
{
    [Collection("CCompiler")]
    public class CStatementTests
    {
        private static int Body(string body) => Eval($"int main(void) {{ {body} }}");

        [Theory]
        [InlineData("int x = 5; if (x > 3) return 1; return 2;", 1)]
        [InlineData("int x = 1; if (x > 3) return 1; else return 2;", 2)]
        [InlineData("int x = 1; if (x) if (0) return 1; else return 2; return 3;", 2)]   // dangling else binds inner if
        [InlineData("int x = 0; if (x) { return 1; } else if (x == 0) { return 5; } else { return 9; }", 5)]
        [InlineData("if (\"non-null pointer\") return 1; return 0;", 1)]
        public void IfElse(string body, int expected) => Assert.Equal(expected, Body(body));

        [Theory]
        [InlineData("int i = 0, s = 0; while (i < 10) { s += i; i++; } return s;", 45)]
        [InlineData("int s = 0; for (int i = 1; i <= 100; i++) s += i; return s;", 5050)]
        [InlineData("int i = 10, n = 0; do { n++; } while (i < 5); return n;", 1)]     // do runs at least once
        [InlineData("int i = 0, n = 0; do { n += i; i++; } while (i < 5); return n;", 10)]
        [InlineData("int n = 0; for (;;) { if (++n == 7) break; } return n;", 7)]
        [InlineData("int s = 0; for (int i = 0; i < 10; i++) { if (i % 2) continue; s += i; } return s;", 20)]
        [InlineData("int i = 0, s = 0; while (1) { i++; if (i > 5) break; if (i == 3) continue; s += i; } return s;", 12)]
        [InlineData("int i = 0, s = 0; do { i++; if (i == 2) continue; s += i; } while (i < 4); return s;", 8)]
        [InlineData("int s = 0; for (int i = 0; i < 4; i++) for (int j = 0; j < 4; j++) { if (j > i) break; s++; } return s;", 10)]
        [InlineData("int i, j, s = 0; for (i = 0, j = 10; i < j; i++, j--) s++; return s;", 5)]
        [InlineData("int n = 0; while (0) n = 99; return n;", 0)]
        public void Loops(string body, int expected) => Assert.Equal(expected, Body(body));

        [Fact]
        public void ForScopedVariableShadowsOuter() =>
            Assert.Equal(100 + 45, Body("int i = 100, s = 0; for (int i = 0; i < 10; i++) s += i; return i + s;"));

        [Fact]
        public void BlockScopeShadowing() =>
            Assert.Equal(3201, Body("int x = 1; int r; { int x = 2; { int x = 3; r = x; } r = r * 10 + x; } return r * 100 + x;"));

        [Theory]
        [InlineData(1, 10)]
        [InlineData(2, 20)]
        [InlineData(3, 35)]      // falls through from case 3 into case 4
        [InlineData(4, 5)]
        [InlineData(9, -1)]      // default
        [InlineData(-7, 70)]     // negative case label
        public void Switch(int value, int expected) =>
            Assert.Equal(expected, Eval($@"
int f(int v) {{
    int r = 0;
    switch (v) {{
        case 1: r = 10; break;
        case 2: {{ r = 20; break; }}
        case 3: r = 30;
        case 4: r += 5; break;
        case -7: return 70;
        default: r = -1;
    }}
    return r;
}}
int main(void) {{ return f({value}); }}"));

        [Fact]
        public void SwitchWithoutDefaultAndNoMatch() =>
            Assert.Equal(3, Body("int r = 3; switch (r) { case 1: r = 0; } return r;"));

        [Fact]
        public void SwitchOnCharAndEnumWithDefaultInMiddle() =>
            Assert.Equal(21, Eval(@"
enum Color { RED, GREEN = 5, BLUE };
int classify(char c) { switch (c) { case 'a': return 1; default: return 2; case 'z': return 3; } }
int color(enum Color c) { switch (c) { case RED: return 0; case GREEN: return 10; case BLUE: return 20; } return -1; }
int main(void) { return classify('q') - classify('a') + color(BLUE); }"));

        [Fact]
        public void NestedSwitchAndLoopBreaks() =>
            Assert.Equal(62, Body(@"
int s = 0;
for (int i = 0; i < 6; i++) {
    switch (i % 3) {
        case 0: s += 1; break;           // breaks the switch, not the loop
        case 1: s += 10; continue;       // continue applies to the loop
        default: s += 20;
    }
    s += 0;
}
return s;"));

        [Fact]
        public void GotoForwardAndBackward() =>
            Assert.Equal(15, Body(@"
int i = 0, s = 0;
again:
    s += i;
    i++;
    if (i <= 5) goto again;
    goto done;
    s = -1000;
done:
    return s;"));

        [Fact]
        public void GotoOutOfNestedLoops() =>
            Assert.Equal(26, Body(@"
int r = 0;
for (int i = 0; i < 10; i++)
    for (int j = 0; j < 10; j++)
        if (i * j == 12) { r = i * 10 + j; goto out; }
out:
return r;"));

        [Fact]
        public void EmptyStatementsAndBlocks() => Assert.Equal(4, Body("; ; {} { ; } int x = 4; {{{ }}} return x;"));

        [Fact]
        public void ManyLocalsAndLargeFrame() =>
            Assert.Equal(4950, Body(@"
int big[1000];
int s = 0;
for (int i = 0; i < 1000; i++) big[i] = i % 100;
for (int i = 0; i < 100; i++) s += big[i + 500];
return s;"));
    }
}
