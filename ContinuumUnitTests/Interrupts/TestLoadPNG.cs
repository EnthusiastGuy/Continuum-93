using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace Interrupts
{
    public class TestLoadPNG
    {
        [Fact]
        public void TestPNGValid()
        {
            Assembler cp = new();
            using Computer computer = new();

            ushort expectedWidth = 4800;
            ushort expectedHeight = 1120;
            ushort expectedColorCount = 32;

            // Fle must reside in "bin\Debug\net6.0\Data\filesystem"
            cp.Build(@"
                MEMF 20000, 6000000, 128    ; Fill the target memory with some dummy data to reveal margins
                LD A, 0x33          ; Load 8 bit PNG
                LD BCD, .Filename   ; Filename address
                LD EFG, 20000       ; Target address for the palette and transparency data
                LD HIJ, 21024       ; Target address for the pixel data
                INT 4, A
                BREAK
              .Filename
                #DB ""Seymour-at-the-movies-map.png"", 0
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            byte[] colors = computer.GetMemFrom(20000, (uint)(expectedColorCount * 3));
            //byte[] transparency = computer.GetMemFrom((uint)(20000 + expectedColorCount * 3), (uint)(expectedColorCount + 1));
            byte[] imageDataStart = computer.GetMemFrom(21024, 16);
            byte[] imageDataEnd = computer.GetMemFrom((uint)(21024 + expectedWidth * expectedHeight - 16), 24);

            Assert.Equal(0, computer.CPU.REGS.A);
            Assert.Equal(expectedWidth, computer.CPU.REGS.BC);
            Assert.Equal(expectedHeight, computer.CPU.REGS.DE);
            Assert.Equal(expectedColorCount, computer.CPU.REGS.F);

            Assert.Equal(
                new byte[] {
                    79, 104, 255,
                    0, 0, 0,
                    159, 0, 0,
                    255, 168, 0,
                    207, 88, 0, 0,
                    184, 255, 255,
                    200, 0, 207, 200, 207, 255, 0, 0, 159, 152, 159, 0, 136,
                    0, 255, 248, 255, 111, 104, 111, 0, 184, 0, 255, 255, 255, 63, 72, 191,
                    127, 152, 255, 255, 0, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                colors);

            /*Assert.Equal(
                new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
                    255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
                128},   // Outside of the transparency
                transparency);*/

            Assert.Equal(
                new byte[] { 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 }, imageDataStart);

            Assert.Equal(
                new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    128, 128, 128, 128, 128, 128, 128, 128 }, imageDataEnd);
        }

        [Fact]
        public void TestPNGInvalidNotFound()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                MEMF 20000, 6000000, 128    ; Fill the target memory with some dummy data to reveal margins
                LD A, 0x33          ; Load 8 bit PNG
                LD BCD, .Filename   ; Filename address
                LD EFG, 20000       ; Target address for the palette and transparency data
                LD HIJ, 21024       ; Target address for the pixel data
                INT 4, A
                BREAK
              .Filename
                #DB ""DoesNotExist.png"", 0
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1, computer.CPU.REGS.A);       // File not found
        }

        [Fact]
        public void TestPNGInvalidWrongFormat()
        {
            Assembler cp = new();
            using Computer computer = new();

            // Fle must reside in "bin\Debug\net6.0\Data\filesystem"
            cp.Build(@"
                MEMF 20000, 6000000, 128    ; Fill the target memory with some dummy data to reveal margins
                LD A, 0x33          ; Load 8 bit PNG
                LD BCD, .Filename   ; Filename address
                LD EFG, 20000       ; Target address for the palette and transparency data
                LD HIJ, 21024       ; Target address for the pixel data
                INT 4, A
                BREAK
              .Filename
                #DB ""dummy.txt"", 0
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(2, computer.CPU.REGS.A);       // Not a PNG file
        }

        [Fact]
        public void TestPNGInvalidLarge()
        {
            Assembler cp = new();
            using Computer computer = new();

            // Fle must reside in "bin\Debug\net6.0\Data\filesystem"
            cp.Build(@"
                MEMF 20000, 6000000, 128    ; Fill the target memory with some dummy data to reveal margins
                LD A, 0x33          ; Load 8 bit PNG
                LD BCD, .Filename   ; Filename address
                LD EFG, 20000       ; Target address for the palette and transparency data
                LD HIJ, 21024       ; Target address for the pixel data
                INT 4, A
                BREAK
              .Filename
                #DB ""ReallyLongPNG.png"", 0
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(3, computer.CPU.REGS.A);               // Width or height over 65535
        }

        [Fact]
        public void TestPNGInvalidNonPalette()
        {
            Assembler cp = new();
            using Computer computer = new();

            // Fle must reside in "bin\Debug\net6.0\Data\filesystem"
            cp.Build(@"
                MEMF 20000, 6000000, 128    ; Fill the target memory with some dummy data to reveal margins
                LD A, 0x33          ; Load 8 bit PNG
                LD BCD, .Filename   ; Filename address
                LD EFG, 20000       ; Target address for the palette and transparency data
                LD HIJ, 21024       ; Target address for the pixel data
                INT 4, A
                BREAK
              .Filename
                #DB ""nonPalettePNG.png"", 0
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(5, computer.CPU.REGS.A);   // Not a palette based PNG
        }
    }
}
