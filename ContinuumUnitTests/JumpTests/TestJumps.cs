using Continuum93.Emulator.Interpreter;

namespace TestJumps
{

    public class TestJumpsLong
    {
        [Fact]
        public void TestLongRelativeJumps()
        {
            Assembler cp = new();

            cp.Build(@"
                #ORG 1000
            .Block1
                LD A, 12
                LD BC, 0x0203
                LD A, C
                JR .Block2
                RET

                #ORG 100000
            .Block2
                LD A, 34
                LD BC, 0x0405
                LD A, C
                JR .Block1
                RET
            ");

            List<CodeBlock> cBlocks = cp.BlockManager.GetBlocks();

            Assert.Equal(2, cp.BlockManager.BlocksCount());
            //Assert.Equal(new byte[] { 1, 0, 0, 12, 1, 16, 1, 2, 3, 1, 4, 2, 24, 0, 1, 130, 172, 255 }, cBlocks[0].Data);
            //Assert.Equal(new byte[] { 1, 0, 0, 34, 1, 16, 1, 4, 5, 1, 4, 2, 24, 0, 254, 125, 60, 255 }, cBlocks[1].Data);
            Assert.Equal(1000, (int)cBlocks[0].Start);
            Assert.Equal(100000, (int)cBlocks[1].Start);
            Assert.False(cp.BlockManager.HasCollisions());
            TUtils.IncrementCountedTests("exec");
        }
    }
}
