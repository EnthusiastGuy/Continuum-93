using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace LabelsTests
{
    public class LabelsINC
    {
        [Fact]
        public void TestLabel_INC()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint dataAddress = 0x001000;

            cp.Build(@$"
                #ORG 0x000000

                INC (.Data)
                INC (.Data)

                BREAK

                #ORG {dataAddress}
            .Data
                #DB 0x10, 0x20, 0x30, 0x40, 0x50
                "
            );

            List<CodeBlock> cBlocks = cp.BlockManager.GetBlocks();
            for (int i = 0; i < cBlocks.Count; i++)
            {
                computer.LoadMemAt(cBlocks[i].Start, cBlocks[i].Data);
            }
            computer.Run();

            Assert.Equal(0x12, computer.MEMC.Get8bitFromRAM(dataAddress));
            Assert.Equal(0x20, computer.MEMC.Get8bitFromRAM(dataAddress + 1));
            Assert.Equal(0x30, computer.MEMC.Get8bitFromRAM(dataAddress + 2));
            Assert.Equal(0x40, computer.MEMC.Get8bitFromRAM(dataAddress + 3));
            Assert.Equal(0x50, computer.MEMC.Get8bitFromRAM(dataAddress + 4));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestLabel_INC16()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint dataAddress = 0x001000;

            cp.Build(@$"
                #ORG 0x000000

                INC16 (.Data)
                INC16 (.Data)

                BREAK

                #ORG {dataAddress}
            .Data
                #DB 0xFFFF, 0x30, 0x40, 0x50
                "
            );

            List<CodeBlock> cBlocks = cp.BlockManager.GetBlocks();
            for (int i = 0; i < cBlocks.Count; i++)
            {
                computer.LoadMemAt(cBlocks[i].Start, cBlocks[i].Data);
            }
            computer.Run();

            Assert.Equal(0x0001, computer.MEMC.Get16bitFromRAM(dataAddress));
            Assert.Equal(0x30, computer.MEMC.Get8bitFromRAM(dataAddress + 2));
            Assert.Equal(0x40, computer.MEMC.Get8bitFromRAM(dataAddress + 3));
            Assert.Equal(0x50, computer.MEMC.Get8bitFromRAM(dataAddress + 4));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestLabel_INC24()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint dataAddress = 0x001000;

            cp.Build(@$"
                #ORG 0x000000

                INC24 (.Data)
                INC24 (.Data)

                BREAK

                #ORG {dataAddress}
            .Data
                #DB 0xFFFFFF, 0x40, 0x50
                "
            );

            List<CodeBlock> cBlocks = cp.BlockManager.GetBlocks();
            for (int i = 0; i < cBlocks.Count; i++)
            {
                computer.LoadMemAt(cBlocks[i].Start, cBlocks[i].Data);
            }
            computer.Run();

            Assert.Equal(0x000001, (long)computer.MEMC.Get24bitFromRAM(dataAddress));
            Assert.Equal(0x40, computer.MEMC.Get8bitFromRAM(dataAddress + 3));
            Assert.Equal(0x50, computer.MEMC.Get8bitFromRAM(dataAddress + 4));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestLabel_INC32()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint dataAddress = 0x001000;

            cp.Build(@$"
                #ORG 0x000000

                INC32 (.Data)
                INC32 (.Data)

                BREAK

                #ORG {dataAddress}
            .Data
                #DB 0xFFFFFFFF, 0x50
                "
            );

            List<CodeBlock> cBlocks = cp.BlockManager.GetBlocks();
            for (int i = 0; i < cBlocks.Count; i++)
            {
                computer.LoadMemAt(cBlocks[i].Start, cBlocks[i].Data);
            }
            computer.Run();

            Assert.Equal(0x00000001, (long)computer.MEMC.Get32bitFromRAM(dataAddress));
            Assert.Equal(0x50, computer.MEMC.Get8bitFromRAM(dataAddress + 4));
            TUtils.IncrementCountedTests("exec");
        }
    }
}
