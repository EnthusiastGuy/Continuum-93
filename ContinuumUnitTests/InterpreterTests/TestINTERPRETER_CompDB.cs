
using Continuum93.Emulator.Interpreter;

namespace InterpreterTests
{

    public class TestINTERPRETER_CompDB
    {
        [Fact]
        public void TestInterpretDBDataFailure()
        {
            Assembler cp = new();
            CompileLog.Reset();
            Assert.False(CompileLog.StopBuild);

            cp.Build("#DB   0xABCDEF, , 0b11111110     ");
            Assert.Equal(
                new byte[] { 171, 205, 239, 254 },
                cp.GetCompiledCodeBlock(0));

            Assert.True(CompileLog.StopBuild);
            CompileLog.Reset();
        }

        [Fact]
        public void TestGetInterpretedDBData()
        {
            Assembler cp = new();

            cp.Build("  #DB    0Xff, 0xFF,   0xAA     ");
            Assert.Equal(
                new byte[] { 0xFF, 0xFF, 0xAA },
                cp.GetCompiledCodeBlock(0));

            cp.Build(" #db  0Xff, 0xFF,   0xAA     ");
            Assert.Equal(
                new byte[] { 0xFF, 0xFF, 0xAA },
                cp.GetCompiledCodeBlock(0));

            cp.Build(" #db   \"Hello\", 0, 0Xff, 0xFF,   0xAA     ");
            Assert.Equal(
                new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x00, 0xFF, 0xFF, 0xAA },
                cp.GetCompiledCodeBlock(0));

            cp.Build(" #db   \"Hello,  world\", 0, 0Xff, 0xFF,   0xAA     ");
            byte[] actual = cp.GetCompiledCodeBlock(0);
            Assert.Equal(
                new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x2C, 0x20, 0x20, 0x77, 0x6F, 0x72, 0x6C, 0x64, 0x00, 0xFF, 0xFF, 0xAA },
                actual);

            cp.Build("#DB   0xFFFF, \"Hi\", 0, 0b11111110     ");
            Assert.Equal(
                new byte[] { 0xFF, 0xFF, 0x48, 0x69, 0x00, 0xFE },
                cp.GetCompiledCodeBlock(0));




            cp.Build("#DB  0b111100001     ");
            Assert.Equal(
                new byte[] { 0x01, 0xE1 },
                cp.GetCompiledCodeBlock(0));

            cp.Build("#DB  0b11111111000000001111111100000000     ");
            Assert.Equal(
                new byte[] { 0xFF, 0, 0xFF, 0 },
                cp.GetCompiledCodeBlock(0));

            cp.Build("#DB  0b_11111111_00000000_11111111_00000000     ");
            Assert.Equal(
                new byte[] { 0xFF, 0, 0xFF, 0 },
                cp.GetCompiledCodeBlock(0));

        }

        [Fact]
        public void TestGetInterpretedDBNegativeDataHex()
        {
            Assembler cp = new();

            cp.Build("  #DB    -0x2");
            Assert.Equal(new byte[] { 0xFE }, cp.GetCompiledCodeBlock(0));

            cp.Build("  #DB    -0x02");
            Assert.Equal(new byte[] { 0xFE }, cp.GetCompiledCodeBlock(0));

            cp.Build("  #DB    -0x002");
            Assert.Equal(new byte[] { 0xFF, 0xFE }, cp.GetCompiledCodeBlock(0));

            cp.Build("  #DB    -0x0002");
            Assert.Equal(new byte[] { 0xFF, 0xFE }, cp.GetCompiledCodeBlock(0));

            cp.Build("  #DB    -0x00002");
            Assert.Equal(new byte[] { 0xFF, 0xFF, 0xFE }, cp.GetCompiledCodeBlock(0));

            cp.Build("  #DB    -0x000002");
            Assert.Equal(new byte[] { 0xFF, 0xFF, 0xFE }, cp.GetCompiledCodeBlock(0));

            cp.Build("  #DB    -0x0000002");
            Assert.Equal(new byte[] { 0xFF, 0xFF, 0xFF, 0xFE }, cp.GetCompiledCodeBlock(0));

            cp.Build("  #DB    -0x00000002");
            Assert.Equal(new byte[] { 0xFF, 0xFF, 0xFF, 0xFE }, cp.GetCompiledCodeBlock(0));
        }

        [Fact]
        public void TestGetInterpretedDBNegativeDataBin()
        {
            Assembler cp = new();

            cp.Build("  #DB    -0b00000010");
            Assert.Equal(new byte[] { 0xFE }, cp.GetCompiledCodeBlock(0));

            cp.Build("  #DB    -0b00000000_00000010");
            Assert.Equal(new byte[] { 0xFF, 0xFE }, cp.GetCompiledCodeBlock(0));

            cp.Build("  #DB    -0b00000000_00000000_00000010");
            Assert.Equal(new byte[] { 0xFF, 0xFF, 0xFE }, cp.GetCompiledCodeBlock(0));

            cp.Build("  #DB    -0b00000000_00000000_00000000_00000010");
            Assert.Equal(new byte[] { 0xFF, 0xFF, 0xFF, 0xFE }, cp.GetCompiledCodeBlock(0));
        }

        [Fact]
        public void TestGetInterpretedDBNegativeDataOct()
        {
            Assembler cp = new();

            cp.Build("  #DB    -0o02");
            Assert.Equal(new byte[] { 0xFE }, cp.GetCompiledCodeBlock(0));

            cp.Build("  #DB    -0o002");
            Assert.Equal(new byte[] { 0xFF, 0xFE }, cp.GetCompiledCodeBlock(0));

            cp.Build("  #DB    -0o0002");
            Assert.Equal(new byte[] { 0xFF, 0xFF, 0xFE }, cp.GetCompiledCodeBlock(0));

            cp.Build("  #DB    -0o00002");
            Assert.Equal(new byte[] { 0xFF, 0xFF, 0xFF, 0xFE }, cp.GetCompiledCodeBlock(0));
        }

        [Fact]
        public void TestGetInterpretedDBNegativeDataDec()
        {
            Assembler cp = new();

            cp.Build("  #DB    -2");
            Assert.Equal(new byte[] { 0xFE }, cp.GetCompiledCodeBlock(0));

            cp.Build("  #DB    -02");
            Assert.Equal(new byte[] { 0xFE }, cp.GetCompiledCodeBlock(0));

            cp.Build("  #DB    -002");
            Assert.Equal(new byte[] { 0xFF, 0xFE }, cp.GetCompiledCodeBlock(0));

            cp.Build("  #DB    -0002");
            Assert.Equal(new byte[] { 0xFF, 0xFF, 0xFE }, cp.GetCompiledCodeBlock(0));

            cp.Build("  #DB    -00002");
            Assert.Equal(new byte[] { 0xFF, 0xFF, 0xFF, 0xFE }, cp.GetCompiledCodeBlock(0));

            cp.Build("  #DB    -32767");
            Assert.Equal(new byte[] { 0x80, 0x01 }, cp.GetCompiledCodeBlock(0));

            cp.Build("  #DB    -32768");
            Assert.Equal(new byte[] { 0x80, 0x00 }, cp.GetCompiledCodeBlock(0));

            cp.Build("  #DB    -32769");
            Assert.Equal(new byte[] { 0x7F, 0xFF }, cp.GetCompiledCodeBlock(0));

            cp.Build("  #DB    32767");
            Assert.Equal(new byte[] { 0x7F, 0xFF }, cp.GetCompiledCodeBlock(0));

            cp.Build("  #DB    -8388608");
            Assert.Equal(new byte[] { 0x80, 0x00, 0x00 }, cp.GetCompiledCodeBlock(0));

            cp.Build("  #DB    -8388609");
            Assert.Equal(new byte[] { 0x7F, 0xFF, 0xFF }, cp.GetCompiledCodeBlock(0));
        }
    }
}
