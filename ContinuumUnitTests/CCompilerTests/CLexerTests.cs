using Continuum93.Emulator.Compilers.C93C;

namespace ContinuumUnitTests.CCompilerTests
{
    [Collection("CCompiler")]
    public class CLexerTests
    {
        private static List<CToken> Lex(string src, out List<CCompileError> errors)
        {
            errors = [];
            return new CLexer(src, "t.c", errors).Tokenize()
                .Where(t => t.Kind != CTokenKind.Newline && t.Kind != CTokenKind.EOF).ToList();
        }

        private static List<CToken> Lex(string src)
        {
            var toks = Lex(src, out var errors);
            Assert.Empty(errors);
            return toks;
        }

        [Fact]
        public void IdentifiersNumbersAndPunctuators()
        {
            var t = Lex("int x_1 = 0x1F + 42u;");
            Assert.Equal(["int", "x_1", "=", "0x1F", "+", "42u", ";"], t.Select(k => k.Text));
            Assert.Equal(CTokenKind.Identifier, t[1].Kind);
            Assert.Equal(CTokenKind.Number, t[3].Kind);
            Assert.Equal(CTokenKind.Punct, t[4].Kind);
        }

        [Fact]
        public void LongestMatchPunctuators()
        {
            var t = Lex("a<<=b>>=c->d++--e...f&&g||h##i");
            Assert.Equal(["a", "<<=", "b", ">>=", "c", "->", "d", "++", "--", "e", "...", "f", "&&", "g", "||", "h", "##", "i"], t.Select(k => k.Text));
        }

        [Fact]
        public void CommentsAreSkipped()
        {
            var t = Lex("a /* block \n comment */ b // line comment\n c");
            Assert.Equal(["a", "b", "c"], t.Select(k => k.Text));
        }

        [Fact]
        public void LineAndColumnTracking()
        {
            var t = Lex("a\n  bb\n\tc");
            Assert.Equal((1, 1), (t[0].Line, t[0].Column));
            Assert.Equal((2, 3), (t[1].Line, t[1].Column));
            Assert.Equal((3, 2), (t[2].Line, t[2].Column));
        }

        [Theory]
        [InlineData(@"""\n""", "\n")]
        [InlineData(@"""\t\r""", "\t\r")]
        [InlineData(@"""\x41\x62""", "Ab")]
        [InlineData(@"""\101\0""", "A\0")]
        [InlineData(@"""\\\""""", "\\\"")]
        [InlineData(@"""it's""", "it's")]
        public void StringEscapes(string src, string expected)
        {
            var t = Lex(src);
            Assert.Single(t);
            Assert.Equal(CTokenKind.String, t[0].Kind);
            Assert.Equal(expected, t[0].Text);
        }

        [Theory]
        [InlineData(@"'a'", 'a')]
        [InlineData(@"'\n'", '\n')]
        [InlineData(@"'\''", '\'')]
        [InlineData(@"'\xFF'", (char)255)]
        [InlineData(@"'\0'", '\0')]
        public void CharacterConstants(string src, char expected)
        {
            var t = Lex(src);
            Assert.Equal(CTokenKind.Char, t[0].Kind);
            Assert.Equal(expected, t[0].Text[0]);
        }

        [Fact]
        public void LineContinuationsAreSpliced()
        {
            var t = Lex("ab\\\ncd + 1\\\n2");
            Assert.Equal(["abcd", "+", "12"], t.Select(k => k.Text));
            Assert.Equal(2, t[1].Line); // tokens after a continuation report their physical line
        }

        [Fact]
        public void LeadingSpaceIsRecorded()
        {
            var t = Lex("a (b");
            Assert.False(t[0].LeadingSpace);
            Assert.True(t[1].LeadingSpace);
            Assert.False(t[2].LeadingSpace);
        }

        [Theory]
        [InlineData("\"abc", "Unterminated string literal")]
        [InlineData("'a", "Unterminated character constant")]
        [InlineData("/* never closed", "Unterminated block comment")]
        [InlineData("a @ b", "Unexpected character '@'")]
        [InlineData("'ab'", "Multi-character constants are not supported")]
        [InlineData("''", "Empty character constant")]
        [InlineData(@"""\q""", "Unknown escape sequence")]
        public void LexicalErrors(string src, string message)
        {
            Lex(src, out var errors);
            Assert.Contains(errors, e => e.Message.Contains(message));
        }

        [Theory]
        [InlineData("0", 0u, false)]
        [InlineData("2147483647", 2147483647u, false)]
        [InlineData("0x7FFFFFFF", 0x7FFFFFFFu, false)]
        [InlineData("0x80000000", 0x80000000u, true)]       // hex that doesn't fit int becomes unsigned
        [InlineData("0xFFFFFFFF", 0xFFFFFFFFu, true)]
        [InlineData("4294967295u", 4294967295u, true)]
        [InlineData("017", 15u, false)]
        [InlineData("0b1011", 11u, false)]
        [InlineData("10U", 10u, true)]
        [InlineData("10L", 10u, false)]
        [InlineData("10ul", 10u, true)]
        public void IntegerLiteralTyping(string text, uint value, bool unsigned)
        {
            Assert.True(CConstExpr.TryParseInt(text, out uint v, out bool u, out string err), err);
            Assert.Equal(value, v);
            Assert.Equal(unsigned, u);
        }

        [Theory]
        [InlineData("2147483648", "too large for int")]   // decimal without suffix must fit int
        [InlineData("4294967296", "too large")]
        [InlineData("10LL", "long long")]
        [InlineData("09", "Invalid digit")]
        [InlineData("1.5", "Floating point is not supported")]
        [InlineData("0x", "Invalid integer literal")]
        public void IntegerLiteralErrors(string text, string message)
        {
            Assert.False(CConstExpr.TryParseInt(text, out _, out _, out string err));
            Assert.Contains(message, err);
        }
    }
}
