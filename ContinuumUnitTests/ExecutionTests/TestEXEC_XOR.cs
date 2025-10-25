using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_XOR
    {
        [Fact]
        public void TestEXEC_XOR_r_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 0b00001111;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XOR A, 0b00100100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00101011, computer.CPU.REGS.A);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XOR_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 0b00001111;
            computer.CPU.REGS.F = 0b00100100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XOR A, F",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00101011, computer.CPU.REGS.A);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XOR_rr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 0b0000000011111111;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XOR AB, 0b0000110000001111",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b0000110011110000, computer.CPU.REGS.AB);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XOR_rr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 0b0000000011111111;
            computer.CPU.REGS.KL = 0b0000110000001111;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XOR AB, KL",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b0000110011110000, computer.CPU.REGS.AB);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XOR_rrr_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 0b000000000000111111111111;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XOR ABC, 0b000000110000000000111111",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b000000110000111111000000, (double)computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XOR_rrr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 0b000000000000111111111111;
            computer.CPU.REGS.MNO = 0b000000110000000000111111;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XOR ABC, MNO",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b000000110000111111000000, (double)computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XOR_rrrr_nnnn()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0b00000000000000001111111111111111;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XOR ABCD, 0b00000000110000000000000011111111",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00000000110000001111111100000000, (double)computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XOR_rrrr_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 0b00000000000000001111111111111111;
            computer.CPU.REGS.MNO = 0b00000000110000000000000011111111;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XOR ABC, MNO",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00000000110000001111111100000000, (double)computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XOR_IrrrI_n()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set8bitToRAM(address, 0b00001111);
            computer.CPU.REGS.ABC = address;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XOR (ABC), 0b00100100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00101011, computer.MEMC.Get8bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XOR16_IrrrI_nn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set16bitToRAM(address, 0b0000000011111111);
            computer.CPU.REGS.ABC = address;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XOR16 (ABC), 0b0000110000001111",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b0000110011110000, computer.MEMC.Get16bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XOR24_IrrrI_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set24bitToRAM(address, 0b000000000000111111111111);
            computer.CPU.REGS.ABC = address;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XOR24 (ABC), 0b000000110000000000111111",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b000000110000111111000000, (double)computer.MEMC.Get24bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XOR32_IrrrI_nnnn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set32bitToRAM(address, 0b00000000000000001111111111111111);
            computer.CPU.REGS.ABC = address;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XOR32 (ABC), 0b00000000110000000000000011111111",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00000000110000001111111100000000, (double)computer.MEMC.Get32bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XOR_IrrrI_r()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set8bitToRAM(address, 0b00001111);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.D = 0b00100100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XOR (ABC), D",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00101011, computer.MEMC.Get8bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XOR_IrrrI_rr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set16bitToRAM(address, 0b0000000011111111);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DE = 0b0000110000001111;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XOR (ABC), DE",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b0000110011110000, computer.MEMC.Get16bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XOR_IrrrI_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set24bitToRAM(address, 0b000000000000111111111111);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DEF = 0b000000110000000000111111;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XOR (ABC), DEF",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b000000110000111111000000, (double)computer.MEMC.Get24bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XOR_IrrrI_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set32bitToRAM(address, 0b00000000000000001111111111111111);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DEFG = 0b00000000110000000000000011111111;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XOR (ABC), DEFG",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00000000110000001111111100000000, (double)computer.MEMC.Get32bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }



        [Fact]
        public void TestEXEC_XOR_r_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set8bitToRAM(address, 0b11110000);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.D = 0b00111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XOR D, (ABC)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11110000, computer.MEMC.Get8bitFromRAM(address));
            Assert.Equal(0b11001100, computer.CPU.REGS.D);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XOR_rr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set16bitToRAM(address, 0b1111000011110000);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DE = 0b0011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XOR DE, (ABC)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b1111000011110000, computer.MEMC.Get16bitFromRAM(address));
            Assert.Equal(0b1100110011001100, computer.CPU.REGS.DE);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XOR_rrr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set24bitToRAM(address, 0b111100001111000011110000);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DEF = 0b001111000011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XOR DEF, (ABC)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b111100001111000011110000, (double)computer.MEMC.Get24bitFromRAM(address));
            Assert.Equal(0b110011001100110011001100, (double)computer.CPU.REGS.DEF);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XOR_rrrr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set32bitToRAM(address, 0b11110000111100001111000011110000);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DEFG = 0b00111100001111000011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "XOR DEFG, (ABC)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11110000111100001111000011110000, (double)computer.MEMC.Get32bitFromRAM(address));
            Assert.Equal(0b11001100110011001100110011001100, (double)computer.CPU.REGS.DEFG);
            TUtils.IncrementCountedTests("exec");
        }

        // XOR (nnn), n/ nn/ nnn/ nnnn
        [Fact]
        public void TestEXEC_XOR_InnnI_n()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set8bitToRAM(address, 0b11110000);

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"XOR ({address}), 0b00111100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11001100, computer.MEMC.Get8bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XOR16_InnnI_nn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set16bitToRAM(address, 0b1111000011110000);

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"XOR16 ({address}), 0b0011110000111100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b1100110011001100, computer.MEMC.Get16bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XOR24_InnnI_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set24bitToRAM(address, 0b111100001111000011110000);

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"XOR24 ({address}), 0b001111000011110000111100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b110011001100110011001100, (double)computer.MEMC.Get24bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XOR32_InnnI_nnnn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set32bitToRAM(address, 0b11110000111100001111000011110000);

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"XOR32 ({address}), 0b00111100001111000011110000111100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11001100110011001100110011001100, (double)computer.MEMC.Get32bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        // XOR (nnn), r/ rr/ rrr/ rrrr
        [Fact]
        public void TestEXEC_XOR_InnnI_r()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set8bitToRAM(address, 0b11110000);
            computer.CPU.REGS.D = 0b00111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"XOR ({address}), D",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11001100, computer.MEMC.Get8bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XOR_InnnI_rr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set16bitToRAM(address, 0b1111000011110000);
            computer.CPU.REGS.DE = 0b0011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"XOR ({address}), DE",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b1100110011001100, computer.MEMC.Get16bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XOR_InnnI_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set24bitToRAM(address, 0b111100001111000011110000);
            computer.CPU.REGS.DEF = 0b001111000011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"XOR ({address}), DEF",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b110011001100110011001100, (double)computer.MEMC.Get24bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XOR_InnnI_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set32bitToRAM(address, 0b11110000111100001111000011110000);
            computer.CPU.REGS.DEFG = 0b00111100001111000011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"XOR ({address}), DEFG",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11001100110011001100110011001100, (double)computer.MEMC.Get32bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        // XOR r/ rr/ rrr/ rrrr, (nnn)
        [Fact]
        public void TestEXEC_XOR_r_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set8bitToRAM(address, 0b11110000);
            computer.CPU.REGS.D = 0b00111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"XOR D, ({address})",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11110000, computer.MEMC.Get8bitFromRAM(address));
            Assert.Equal(0b11001100, computer.CPU.REGS.D);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XOR_rr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set16bitToRAM(address, 0b1111000011110000);
            computer.CPU.REGS.DE = 0b0011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"XOR DE, ({address})",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b1111000011110000, computer.MEMC.Get16bitFromRAM(address));
            Assert.Equal(0b1100110011001100, computer.CPU.REGS.DE);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XOR_rrr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set24bitToRAM(address, 0b111100001111000011110000);
            computer.CPU.REGS.DEF = 0b001111000011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"XOR DEF, ({address})",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b111100001111000011110000, (double)computer.MEMC.Get24bitFromRAM(address));
            Assert.Equal(0b110011001100110011001100, (double)computer.CPU.REGS.DEF);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_XOR_rrrr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set32bitToRAM(address, 0b11110000111100001111000011110000);
            computer.CPU.REGS.DEFG = 0b00111100001111000011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"XOR DEFG, ({address})",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11110000111100001111000011110000, (double)computer.MEMC.Get32bitFromRAM(address));
            Assert.Equal(0b11001100110011001100110011001100, (double)computer.CPU.REGS.DEFG);
            TUtils.IncrementCountedTests("exec");
        }
    }
}
