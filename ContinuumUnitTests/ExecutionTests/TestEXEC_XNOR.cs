using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{
    public class TestEXEC_XNOR
    {
        [Fact]
        public void TestEXEC_XNOR_r_n()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.A = 0b00001111;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XNOR A, 0b00100100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11010100, computer.CPU.REGS.A);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XNOR_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.A = 0b00001111;
            computer.CPU.REGS.F = 0b00100100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XNOR A, F",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11010100, computer.CPU.REGS.A);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XNOR_rr_n()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.AB = 0b0000111100001111;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XNOR AB, 0b0010010000100100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b1101010011010100, computer.CPU.REGS.AB);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XNOR_rr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.AB = 0b0000111100001111;
            computer.CPU.REGS.KL = 0b0010010000100100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XNOR AB, KL",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b1101010011010100, computer.CPU.REGS.AB);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XNOR_rrr_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABC = 0b000011110000111100001111;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XNOR ABC, 0b001001000010010000100100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b110101001101010011010100, (double)computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XNOR_rrr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABC = 0b000011110000111100001111;
            computer.CPU.REGS.MNO = 0b001001000010010000100100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XNOR ABC, MNO",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b110101001101010011010100, (double)computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XNOR_rrrr_nnnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABCD = 0b00001111000011110000111100001111;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XNOR ABCD, 0b00100100001001000010010000100100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11010100110101001101010011010100, (double)computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XNOR_rrrr_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABCD = 0b00001111000011110000111100001111;
            computer.CPU.REGS.MNOP = 0b00100100001001000010010000100100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XNOR ABCD, MNOP",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11010100110101001101010011010100, (double)computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XNOR_IrrrI_n()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set8bitToRAM(address, 0b00001111);
            computer.CPU.REGS.ABC = address;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XNOR (ABC), 0b00100100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11010100, computer.MEMC.Get8bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XNOR16_IrrrI_nn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set16bitToRAM(address, 0b0000111100001111);
            computer.CPU.REGS.ABC = address;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XNOR16 (ABC), 0b0010010000100100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b1101010011010100, computer.MEMC.Get16bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XNOR24_IrrrI_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set24bitToRAM(address, 0b000011110000111100001111);
            computer.CPU.REGS.ABC = address;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XNOR24 (ABC), 0b001001000010010000100100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b110101001101010011010100, (double)computer.MEMC.Get24bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XNOR32_IrrrI_nnnn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set32bitToRAM(address, 0b00001111000011110000111100001111);
            computer.CPU.REGS.ABC = address;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XNOR32 (ABC), 0b00100100001001000010010000100100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11010100110101001101010011010100, (double)computer.MEMC.Get32bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XNOR_IrrrI_r()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set8bitToRAM(address, 0b00001111);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.D = 0b00100100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XNOR (ABC), D",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11010100, computer.MEMC.Get8bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XNOR_IrrrI_rr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set16bitToRAM(address, 0b0000111100001111);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DE = 0b0010010000100100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XNOR (ABC), DE",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b1101010011010100, computer.MEMC.Get16bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XNOR_IrrrI_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set24bitToRAM(address, 0b000011110000111100001111);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DEF = 0b001001000010010000100100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XNOR (ABC), DEF",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b110101001101010011010100, (double)computer.MEMC.Get24bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XNOR_IrrrI_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set32bitToRAM(address, 0b00001111000011110000111100001111);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DEFG = 0b00100100001001000010010000100100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XNOR (ABC), DEFG",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11010100110101001101010011010100, (double)computer.MEMC.Get32bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }




        [Fact]
        public void TestEXEC_XNOR_r_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set8bitToRAM(address, 0b11110000);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.D = 0b00111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XNOR D, (ABC)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11110000, computer.MEMC.Get8bitFromRAM(address));
            Assert.Equal(0b00110011, computer.CPU.REGS.D);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XNOR_rr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set16bitToRAM(address, 0b1111000011110000);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DE = 0b0011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XNOR DE, (ABC)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b1111000011110000, computer.MEMC.Get16bitFromRAM(address));
            Assert.Equal(0b0011001100110011, computer.CPU.REGS.DE);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XNOR_rrr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set24bitToRAM(address, 0b111100001111000011110000);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DEF = 0b001111000011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XNOR DEF, (ABC)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b111100001111000011110000, (double)computer.MEMC.Get24bitFromRAM(address));
            Assert.Equal(0b001100110011001100110011, (double)computer.CPU.REGS.DEF);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XNOR_rrrr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set32bitToRAM(address, 0b11110000111100001111000011110000);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DEFG = 0b00111100001111000011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XNOR DEFG, (ABC)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11110000111100001111000011110000, (double)computer.MEMC.Get32bitFromRAM(address));
            Assert.Equal(0b00110011001100110011001100110011, (double)computer.CPU.REGS.DEFG);
            TUtils.IncrementCountedTests("exec");
        }

        // XOR (nnn), n/ nn/ nnn/ nnnn
        [Fact]
        public void TestEXEC_XNOR_InnnI_n()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set8bitToRAM(address, 0b11110000);

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"XNOR ({address}), 0b00111100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00110011, computer.MEMC.Get8bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XNOR16_InnnI_nn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set16bitToRAM(address, 0b1111000011110000);

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"XNOR16 ({address}), 0b0011110000111100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b0011001100110011, computer.MEMC.Get16bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XNOR24_InnnI_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set24bitToRAM(address, 0b111100001111000011110000);

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"XNOR24 ({address}), 0b001111000011110000111100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b001100110011001100110011, (double)computer.MEMC.Get24bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XNOR32_InnnI_nnnn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set32bitToRAM(address, 0b11110000111100001111000011110000);

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"XNOR32 ({address}), 0b00111100001111000011110000111100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00110011001100110011001100110011, (double)computer.MEMC.Get32bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        // XOR (nnn), r/ rr/ rrr/ rrrr
        [Fact]
        public void TestEXEC_XNOR_InnnI_r()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set8bitToRAM(address, 0b11110000);
            computer.CPU.REGS.D = 0b00111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"XNOR ({address}), D",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00110011, computer.MEMC.Get8bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XNOR_InnnI_rr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set16bitToRAM(address, 0b1111000011110000);
            computer.CPU.REGS.DE = 0b0011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"XNOR ({address}), DE",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b0011001100110011, computer.MEMC.Get16bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XNOR_InnnI_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set24bitToRAM(address, 0b111100001111000011110000);
            computer.CPU.REGS.DEF = 0b001111000011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"XNOR ({address}), DEF",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b001100110011001100110011, (double)computer.MEMC.Get24bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XNOR_InnnI_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set32bitToRAM(address, 0b11110000111100001111000011110000);
            computer.CPU.REGS.DEFG = 0b00111100001111000011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"XNOR ({address}), DEFG",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00110011001100110011001100110011, (double)computer.MEMC.Get32bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        // XOR r/ rr/ rrr/ rrrr, (nnn)
        [Fact]
        public void TestEXEC_XNOR_r_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set8bitToRAM(address, 0b11110000);
            computer.CPU.REGS.D = 0b00111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"XNOR D, ({address})",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11110000, computer.MEMC.Get8bitFromRAM(address));
            Assert.Equal(0b00110011, computer.CPU.REGS.D);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XNOR_rr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set16bitToRAM(address, 0b1111000011110000);
            computer.CPU.REGS.DE = 0b0011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"XNOR DE, ({address})",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b1111000011110000, computer.MEMC.Get16bitFromRAM(address));
            Assert.Equal(0b0011001100110011, computer.CPU.REGS.DE);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XNOR_rrr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set24bitToRAM(address, 0b111100001111000011110000);
            computer.CPU.REGS.DEF = 0b001111000011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"XNOR DEF, ({address})",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b111100001111000011110000, (double)computer.MEMC.Get24bitFromRAM(address));
            Assert.Equal(0b001100110011001100110011, (double)computer.CPU.REGS.DEF);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XNOR_rrrr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set32bitToRAM(address, 0b11110000111100001111000011110000);
            computer.CPU.REGS.DEFG = 0b00111100001111000011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"XNOR DEFG, ({address})",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11110000111100001111000011110000, (double)computer.MEMC.Get32bitFromRAM(address));
            Assert.Equal(0b00110011001100110011001100110011, (double)computer.CPU.REGS.DEFG);
            TUtils.IncrementCountedTests("exec");
        }
    }
}
