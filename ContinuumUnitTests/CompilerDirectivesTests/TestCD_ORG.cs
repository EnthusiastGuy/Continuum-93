using Continuum93.Emulator.Interpreter;

namespace CompilerDirectivesTests
{

    public class TestCD_ORG
    {
        [Fact]
        public void TestMultipleOrgDirectives()
        {
            Assembler cp = new();

            cp.Build(@"
                #ORG 10
                LD A, 12
                LD BC, 0x0203
                LD A, C
                RET

                #ORG 40
                LD A, 34
                LD BC, 0x0405
                LD A, C
                RET
            ");

            List<CodeBlock> cBlocks = cp.BlockManager.GetBlocks();

            Assert.Equal(2, cp.BlockManager.BlocksCount());
            Assert.Equal([1, 0, 0, 12, 1, 13, 1, 2, 3, 1, 1, 0, 2, 255], cBlocks[0].Data);
            Assert.Equal([1, 0, 0, 34, 1, 13, 1, 4, 5, 1, 1, 0, 2, 255], cBlocks[1].Data);
            Assert.Equal(10, (int)cBlocks[0].Start);
            Assert.Equal(40, (int)cBlocks[1].Start);
            Assert.False(cp.BlockManager.HasCollisions());
            TUtils.IncrementCountedTests("directives");
        }

        [Fact]
        public void TestMultipleOrgDirectives_Collisions()
        {
            Assembler cp = new();

            cp.Build(@"
                #ORG 10
                LD A, 12
                LD BC, 0x0203
                LD A, C
                RET

                #ORG 20
                LD A, 34
                LD BC, 0x0405
                LD A, C
                RET
            ");

            List<string> collisions = cp.BlockManager.GetCollisions();

            Assert.Equal(2, cp.BlockManager.BlocksCount());
            Assert.Single(collisions);
            Assert.True(cp.BlockManager.HasCollisions());
            Assert.Equal("Blocks starting at addresses 20 and 10 collide for 4 bytes", collisions[0]);
            TUtils.IncrementCountedTests("directives");
        }

        [Fact]
        public void TestMultipleOrgDirectives_Collisions2()
        {
            Assembler cp = new();

            cp.Build(@"
                #ORG 10
                LD A, 12
                LD BC, 0x0203
                LD A, C
                RET

                #ORG 20
                LD A, 34
                LD BC, 0x0405
                LD A, C
                RET

                #ORG 15
                LD A, 34
                LD BC, 0x0405
                LD A, C
                RET
            ");

            List<string> collisions = cp.BlockManager.GetCollisions();

            Assert.Equal(3, cp.BlockManager.BlocksCount());
            Assert.Equal(3, collisions.Count);
            Assert.True(cp.BlockManager.HasCollisions());
            Assert.Equal(new string[] {
                "Blocks starting at addresses 20 and 10 collide for 4 bytes",
                "Blocks starting at addresses 15 and 10 collide for 9 bytes",
                "Blocks starting at addresses 15 and 20 collide for 19 bytes"
            }, collisions);
            TUtils.IncrementCountedTests("directives");
        }

        [Fact]
        public void TestMultipleOrgDirectives_Collisions3()
        {
            Assembler cp = new();

            cp.Build(@"
                #ORG 10
                LD A, 12
                LD BC, 0x0203
                LD A, C
                LD D, A
                LD X, D
                LD A, X
                RET

                #ORG 20
                LD A, 34
                LD BC, 0x0405
                LD A, C
                LD D, A
                LD X, D
                LD A, X
                RET

                #ORG 15
                LD A, 34
                LD BC, 0x0405
                LD A, C
                LD D, A
                LD X, D
                LD A, X
                RET
            ");

            List<string> collisions = cp.BlockManager.GetCollisions();

            Assert.Equal(3, cp.BlockManager.BlocksCount());
            Assert.Equal(3, collisions.Count);
            Assert.True(cp.BlockManager.HasCollisions());
            Assert.Equal(new string[] {
                "Blocks starting at addresses 20 and 10 collide for 16 bytes",
                "Blocks starting at addresses 15 and 10 collide for 21 bytes",
                "Blocks starting at addresses 15 and 20 collide for 31 bytes"
            }, collisions);
            TUtils.IncrementCountedTests("directives");
        }

        [Fact]
        public void TestMultipleOrgDirectives_Collisions4()
        {
            Assembler cp = new();

            cp.Build(@"
                #ORG 10
                LD A, 12
                LD BC, 0x0203
                LD A, C
                LD D, A
                LD X, D
                LD A, X
                INC X
                INC A
                DEC D
                RET

                #ORG 12
                LD A, C
                LD D, A
                RET
            ");

            List<string> collisions = cp.BlockManager.GetCollisions();

            Assert.Equal(2, cp.BlockManager.BlocksCount());
            Assert.Single(collisions);
            Assert.True(cp.BlockManager.HasCollisions());
            Assert.Equal(new string[] {
                "Blocks starting at address 10 completely includes block starting at 12 (9 bytes)"
            }, collisions);
            TUtils.IncrementCountedTests("directives");
        }
    }
}
