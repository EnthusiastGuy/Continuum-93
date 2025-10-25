using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace LabelsTests
{
    public class LabelsDEC
    {
        [Fact]
        public void TestLabel_DEC()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint dataAddress = 0x001000;

            cp.Build(@$"
                #ORG 0x000000

                DEC (.Data)
                DEC (.Data)

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

            Assert.Equal(0x0E, computer.MEMC.Get8bitFromRAM(dataAddress));
            Assert.Equal(0x20, computer.MEMC.Get8bitFromRAM(dataAddress + 1));
            Assert.Equal(0x30, computer.MEMC.Get8bitFromRAM(dataAddress + 2));
            Assert.Equal(0x40, computer.MEMC.Get8bitFromRAM(dataAddress + 3));
            Assert.Equal(0x50, computer.MEMC.Get8bitFromRAM(dataAddress + 4));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestLabel_DEC16()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint dataAddress = 0x001000;

            cp.Build(@$"
                #ORG 0x000000

                DEC16 (.Data)
                DEC16 (.Data)

                BREAK

                #ORG {dataAddress}
            .Data
                #DB 0x0001, 0x30, 0x40, 0x50
                "
            );

            List<CodeBlock> cBlocks = cp.BlockManager.GetBlocks();
            for (int i = 0; i < cBlocks.Count; i++)
            {
                computer.LoadMemAt(cBlocks[i].Start, cBlocks[i].Data);
            }
            computer.Run();

            Assert.Equal(0xFFFF, computer.MEMC.Get16bitFromRAM(dataAddress));
            Assert.Equal(0x30, computer.MEMC.Get8bitFromRAM(dataAddress + 2));
            Assert.Equal(0x40, computer.MEMC.Get8bitFromRAM(dataAddress + 3));
            Assert.Equal(0x50, computer.MEMC.Get8bitFromRAM(dataAddress + 4));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestLabel_DEC24()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint dataAddress = 0x001000;

            cp.Build(@$"
                #ORG 0x000000

                DEC24 (.Data)
                DEC24 (.Data)

                BREAK

                #ORG {dataAddress}
            .Data
                #DB 0x000001, 0x40, 0x50
                "
            );

            List<CodeBlock> cBlocks = cp.BlockManager.GetBlocks();
            for (int i = 0; i < cBlocks.Count; i++)
            {
                computer.LoadMemAt(cBlocks[i].Start, cBlocks[i].Data);
            }
            computer.Run();

            Assert.Equal(0xFFFFFF, (long)computer.MEMC.Get24bitFromRAM(dataAddress));
            Assert.Equal(0x40, computer.MEMC.Get8bitFromRAM(dataAddress + 3));
            Assert.Equal(0x50, computer.MEMC.Get8bitFromRAM(dataAddress + 4));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestLabel_DEC32()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint dataAddress = 0x001000;

            cp.Build(@$"
                #ORG 0x000000

                DEC32 (.Data)
                DEC32 (.Data)

                BREAK

                #ORG {dataAddress}
            .Data
                #DB 0x00000001, 0x50
                "
            );

            List<CodeBlock> cBlocks = cp.BlockManager.GetBlocks();
            for (int i = 0; i < cBlocks.Count; i++)
            {
                computer.LoadMemAt(cBlocks[i].Start, cBlocks[i].Data);
            }
            computer.Run();

            Assert.Equal(0xFFFFFFFF, (long)computer.MEMC.Get32bitFromRAM(dataAddress));
            Assert.Equal(0x50, computer.MEMC.Get8bitFromRAM(dataAddress + 4));
            TUtils.IncrementCountedTests("exec");
        }
    }
}
