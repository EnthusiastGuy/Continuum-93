using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace LabelsTests
{
    public class LabelsOR
    {
        [Fact]
        public void TestLabel_OR()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint dataAddress = 0x001000;

            cp.Build(@$"
                #ORG 0x000000

                OR (.Data), 0b11100010
                BREAK

                #ORG {dataAddress}
            .Data
                #DB 0b00110011, 0b01010101, 0b11000110, 0b01110000, 0b00100100
            ");

            List<CodeBlock> cBlocks = cp.BlockManager.GetBlocks();
            for (int i = 0; i < cBlocks.Count; i++)
            {
                computer.LoadMemAt(cBlocks[i].Start, cBlocks[i].Data);
            }
            computer.Run();

            Assert.Equal(0b11110011, computer.MEMC.Get8bitFromRAM(dataAddress));
            Assert.Equal(0b01010101, computer.MEMC.Get8bitFromRAM(dataAddress + 1));
            Assert.Equal(0b11000110, computer.MEMC.Get8bitFromRAM(dataAddress + 2));
            Assert.Equal(0b01110000, computer.MEMC.Get8bitFromRAM(dataAddress + 3));
            Assert.Equal(0b00100100, computer.MEMC.Get8bitFromRAM(dataAddress + 4));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestLabel_OR16()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint dataAddress = 0x001000;

            cp.Build(@$"
                #ORG 0x000000

                OR16 (.Data), 0b1110001011100010
                BREAK

                #ORG {dataAddress}
            .Data
                #DB 0b0011001101010101, 0b11000110, 0b01110000, 0b00100100
            ");

            List<CodeBlock> cBlocks = cp.BlockManager.GetBlocks();
            for (int i = 0; i < cBlocks.Count; i++)
            {
                computer.LoadMemAt(cBlocks[i].Start, cBlocks[i].Data);
            }
            computer.Run();

            Assert.Equal(0b1111001111110111, computer.MEMC.Get16bitFromRAM(dataAddress));
            Assert.Equal(0b11000110, computer.MEMC.Get8bitFromRAM(dataAddress + 2));
            Assert.Equal(0b01110000, computer.MEMC.Get8bitFromRAM(dataAddress + 3));
            Assert.Equal(0b00100100, computer.MEMC.Get8bitFromRAM(dataAddress + 4));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestLabel_OR24()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint dataAddress = 0x001000;

            cp.Build(@$"
                #ORG 0x000000

                OR24 (.Data), 0b111000101110001011100010
                BREAK

                #ORG {dataAddress}
            .Data
                #DB 0b001100110101010111000110, 0b01110000, 0b00100100
            ");

            List<CodeBlock> cBlocks = cp.BlockManager.GetBlocks();
            for (int i = 0; i < cBlocks.Count; i++)
            {
                computer.LoadMemAt(cBlocks[i].Start, cBlocks[i].Data);
            }
            computer.Run();

            Assert.Equal(0b111100111111011111100110,
                (long)computer.MEMC.Get24bitFromRAM(dataAddress));
            Assert.Equal(0b01110000, computer.MEMC.Get8bitFromRAM(dataAddress + 3));
            Assert.Equal(0b00100100, computer.MEMC.Get8bitFromRAM(dataAddress + 4));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestLabel_OR32()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint dataAddress = 0x001000;

            cp.Build(@$"
                #ORG 0x000000

                OR32 (.Data), 0b11100010111000101110001011100010
                BREAK

                #ORG {dataAddress}
            .Data
                #DB 0b00110011010101011100011001110000, 0b00100100
            ");

            List<CodeBlock> cBlocks = cp.BlockManager.GetBlocks();
            for (int i = 0; i < cBlocks.Count; i++)
            {
                computer.LoadMemAt(cBlocks[i].Start, cBlocks[i].Data);
            }
            computer.Run();

            Assert.Equal(0b11110011111101111110011011110010,
                (long)computer.MEMC.Get32bitFromRAM(dataAddress));
            Assert.Equal(0b00100100, computer.MEMC.Get8bitFromRAM(dataAddress + 4));
            TUtils.IncrementCountedTests("exec");
        }
    }
}
