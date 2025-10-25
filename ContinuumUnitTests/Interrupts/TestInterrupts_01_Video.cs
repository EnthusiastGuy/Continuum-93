using Continuum93.Emulator;
using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace Interrupts
{

    public class TestInterrupts_Video
    {
        [Fact]
        public void TestInt_Video_Resolution()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.Z = 0; // 0x00 - ReadVideoResolution

            cp.Build(
                TUtils.GetFormattedAsm(
                    "INT 1, Z",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(480, computer.CPU.REGS.AB);
            Assert.Equal(270, computer.CPU.REGS.CD);
            TUtils.IncrementCountedTests("interrupts");
        }

        [Fact]
        public void TestInt_Video_PagesCount()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.Z = 1; // 0x01 - ReadVideoPagesCount

            cp.Build(
                TUtils.GetFormattedAsm(
                    "INT 1, Z",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(2, computer.CPU.REGS.Z);
            TUtils.IncrementCountedTests("interrupts");
        }

        [Fact]
        public void TestInt_Video_SetPagesCount()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.A = 2; // 0x02 - SetVideoPagesCount
            computer.CPU.REGS.B = 5; // Set 5 pages

            cp.Build(
                TUtils.GetFormattedAsm(
                    "INT 1, A",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(5, computer.GRAPHICS.VRAM_PAGES);
            TUtils.IncrementCountedTests("interrupts");
        }

        [Fact]
        public void TestInt_Video_ReadVideoAddress()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.A = 3; // 0x03 - ReadVideoAddress
            computer.CPU.REGS.B = 1; // Read second page address

            cp.Build(
                TUtils.GetFormattedAsm(
                    "INT 1, A",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xFC0B80, (double)computer.CPU.REGS.BCD);
            TUtils.IncrementCountedTests("interrupts");
        }

        [Fact]
        public void TestInt_Video_ReadVideoPaletteAddress()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.GRAPHICS.SetVramPages(2);
            computer.CPU.REGS.A = 4; // 0x04 - ReadVideoPaletteAddress
            computer.CPU.REGS.B = 1; // Read for the second page

            cp.Build(
                TUtils.GetFormattedAsm(
                    "INT 1, A",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xFC0580, (double)computer.CPU.REGS.BCD);
            TUtils.IncrementCountedTests("interrupts");
        }

        [Fact]
        public void TestInt_Video_ClearVideoPage()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.GRAPHICS.SetVramPages(2);
            computer.CPU.REGS.A = 5; // 0x05 - ClearVideoPage
            computer.CPU.REGS.B = 0; // Clears first page
            computer.CPU.REGS.C = 0xAA; // ... with color 0xAA

            cp.Build(
                TUtils.GetFormattedAsm(
                    "INT 1, A",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xAA, computer.MEMC.RAM[0x1000000 - Constants.V_SIZE]);
            Assert.Equal(0xAA, computer.MEMC.RAM[0x1000000 - 1]);
            Assert.NotEqual(0xAA, computer.MEMC.RAM[0x1000000 - Constants.V_SIZE - 1]);
            TUtils.IncrementCountedTests("interrupts");
        }

        [Fact]
        public void TestInt_Video_DrawTileMapSprite()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.GRAPHICS.SetVramPages(2);

            computer.LoadMemAt(0x2000, new byte[] {
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 12, 2, 2, 2, 2, 2, 5, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 2, 7, 6, 6, 3, 2, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 2, 6, 3, 3, 6, 2, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 2, 6, 3, 3, 6, 2, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 2, 3, 6, 6, 3, 2, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 2, 4, 4, 4, 4, 2, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 2, 4, 4, 4, 4, 2, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 2, 2, 2, 2, 2, 9, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
            });    // testDir


            computer.CPU.REGS.A = 0x0E;      // 0x0E - DrawTileMapSprite
            computer.CPU.REGS.BCD = 0x2000;  // Address of the tilemap
            computer.CPU.REGS.EF = 16;       // Tilemap width
            computer.CPU.REGS.GH = 4;        // Sprite x position on tilemap
            computer.CPU.REGS.IJ = 3;        // Sprite y position on tilemap
            computer.CPU.REGS.KL = 6;        // Sprite width
            computer.CPU.REGS.MN = 8;        // Sprite height

            computer.CPU.REGS.O = 0;         // First video page
            computer.CPU.REGS.PQ = 50;       // Target x coordinate
            computer.CPU.REGS.RS = 100;      // Target y coordinate

            cp.Build(
                TUtils.GetFormattedAsm(
                    "INT 1, A",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint vAddress = 0xFE05C0 + 100 * Constants.V_WIDTH + 50;

            Assert.Equal(12, computer.MEMC.RAM[vAddress]);
            Assert.Equal(2, computer.MEMC.RAM[vAddress + 1]);
            Assert.Equal(7, computer.MEMC.RAM[vAddress + Constants.V_WIDTH + 1]);
            Assert.Equal(9, computer.MEMC.RAM[vAddress + Constants.V_WIDTH * 7 + 5]);
            TUtils.IncrementCountedTests("interrupts");
        }
    }
}
