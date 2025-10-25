using Continuum93.Emulator.Interpreter;

namespace CompilerTests
{
    public class SourceParsingTests
    {
        [Fact]
        public void Test_CRLF()
        {
            Assembler cp = new();
            cp.Build(".Start\r\n  LD A, 0\r\n  LD B, 0xFF\n  LD C, 0x80\n\n  JP .Start");

            Assert.Equal(".Start\r\n  LD A, 0\r\n  LD B, 0xFF\r\n  LD C, 0x80\r\n\r\n  JP .Start", cp.FullSource);
        }

        [Fact]
        public void Test_CRLF_with_DB()
        {
            Assembler cp = new();
            cp.Build(".Start\r\n  #DB \"SomeText \\nSome other text\"\n  JP .Start");

            Assert.Equal(".Start\r\n  #DB \"SomeText \\nSome other text\"\r\n  JP .Start", cp.FullSource);
        }
    }
}
