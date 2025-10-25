using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{
    public class TestEXEC_NAND
    {
        [Fact]
        public void TestEXEC_NAND_r_n()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.A = 0b00110011;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NAND A, 0b10101010",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11011101, computer.CPU.REGS.A);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NAND_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.A = 0b00110011;
            computer.CPU.REGS.B = 0b10101010;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NAND A, B",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11011101, computer.CPU.REGS.A);
            Assert.Equal(0b10101010, computer.CPU.REGS.B);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NAND_rr_n()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.AB = 0b0011001100110011;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NAND AB, 0b1010101010101010",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b1101110111011101, computer.CPU.REGS.AB);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NAND_rr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.AB = 0b0011001100110011;
            computer.CPU.REGS.CD = 0b1010101010101010;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NAND AB, CD",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b1101110111011101, computer.CPU.REGS.AB);
            Assert.Equal(0b1010101010101010, computer.CPU.REGS.CD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NAND_rrr_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABC = 0b001100110011001100110011;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NAND ABC, 0b101010101010101010101010",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b110111011101110111011101, (double)computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NAND_rrr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABC = 0b001100110011001100110011;
            computer.CPU.REGS.DEF = 0b101010101010101010101010;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NAND ABC, DEF",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b110111011101110111011101, (double)computer.CPU.REGS.ABC);
            Assert.Equal(0b101010101010101010101010, (double)computer.CPU.REGS.DEF);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NAND_rrrr_nnnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABCD = 0b00110011001100110011001100110011;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NAND ABCD, 0b10101010101010101010101010101010",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11011101110111011101110111011101, (double)computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NAND_rrrr_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABCD = 0b00110011001100110011001100110011;
            computer.CPU.REGS.EFGH = 0b10101010101010101010101010101010;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NAND ABCD, EFGH",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11011101110111011101110111011101, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0b10101010101010101010101010101010, (double)computer.CPU.REGS.EFGH);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NAND_IrrrI_n()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set8bitToRAM(address, 0b00110011);
            computer.CPU.REGS.ABC = address;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NAND (ABC), 0b10101010",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11011101, computer.MEMC.Get8bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NAND16_IrrrI_nn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set16bitToRAM(address, 0b0011001100110011);
            computer.CPU.REGS.ABC = address;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NAND16 (ABC), 0b1010101010101010",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b1101110111011101, computer.MEMC.Get16bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NAND24_IrrrI_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set24bitToRAM(address, 0b001100110011001100110011);
            computer.CPU.REGS.ABC = address;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NAND24 (ABC), 0b101010101010101010101010",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b110111011101110111011101, (double)computer.MEMC.Get24bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NAND32_IrrrI_nnnn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set32bitToRAM(address, 0b00110011001100110011001100110011);
            computer.CPU.REGS.ABC = address;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NAND32 (ABC), 0b10101010101010101010101010101010",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11011101110111011101110111011101, (double)computer.MEMC.Get32bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NAND_IrrrI_r()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set8bitToRAM(address, 0b00110011);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.D = 0b10101010;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NAND (ABC), D",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11011101, computer.MEMC.Get8bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NAND_IrrrI_rr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set16bitToRAM(address, 0b0011001100110011);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DE = 0b1010101010101010;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NAND (ABC), DE",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b1101110111011101, computer.MEMC.Get16bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NAND_IrrrI_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set24bitToRAM(address, 0b001100110011001100110011);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DEF = 0b101010101010101010101010;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NAND (ABC), DEF",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b110111011101110111011101, (double)computer.MEMC.Get24bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NAND_IrrrI_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set32bitToRAM(address, 0b00110011001100110011001100110011);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DEFG = 0b10101010101010101010101010101010;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NAND (ABC), DEFG",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11011101110111011101110111011101, (double)computer.MEMC.Get32bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NAND_r_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set8bitToRAM(address, 0b11110000);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.D = 0b00111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NAND D, (ABC)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11110000, computer.MEMC.Get8bitFromRAM(address));
            Assert.Equal(0b11001111, computer.CPU.REGS.D);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NAND_rr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set16bitToRAM(address, 0b1111000011110000);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DE = 0b0011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NAND DE, (ABC)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b1111000011110000, computer.MEMC.Get16bitFromRAM(address));
            Assert.Equal(0b1100111111001111, computer.CPU.REGS.DE);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NAND_rrr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set24bitToRAM(address, 0b111100001111000011110000);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DEF = 0b001111000011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NAND DEF, (ABC)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b111100001111000011110000, (double)computer.MEMC.Get24bitFromRAM(address));
            Assert.Equal(0b110011111100111111001111, (double)computer.CPU.REGS.DEF);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NAND_rrrr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set32bitToRAM(address, 0b11110000111100001111000011110000);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DEFG = 0b00111100001111000011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NAND DEFG, (ABC)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11110000111100001111000011110000, (double)computer.MEMC.Get32bitFromRAM(address));
            Assert.Equal(0b11001111110011111100111111001111, (double)computer.CPU.REGS.DEFG);
            TUtils.IncrementCountedTests("exec");
        }

        // AND (nnn), n/ nn/ nnn/ nnnn
        [Fact]
        public void TestEXEC_NAND_InnnI_n()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set8bitToRAM(address, 0b11110000);

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"NAND ({address}), 0b00111100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11001111, computer.MEMC.Get8bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NAND16_InnnI_nn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set16bitToRAM(address, 0b1111000011110000);

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"NAND16 ({address}), 0b0011110000111100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b1100111111001111, computer.MEMC.Get16bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NAND24_InnnI_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set24bitToRAM(address, 0b111100001111000011110000);

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"NAND24 ({address}), 0b001111000011110000111100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b110011111100111111001111, (double)computer.MEMC.Get24bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NAND32_InnnI_nnnn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set32bitToRAM(address, 0b11110000111100001111000011110000);

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"NAND32 ({address}), 0b00111100001111000011110000111100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11001111110011111100111111001111, (double)computer.MEMC.Get32bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        // AND (nnn), r/ rr/ rrr/ rrrr
        [Fact]
        public void TestEXEC_NAND_InnnI_r()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set8bitToRAM(address, 0b11110000);
            computer.CPU.REGS.D = 0b00111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"NAND ({address}), D",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11001111, computer.MEMC.Get8bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NAND_InnnI_rr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set16bitToRAM(address, 0b1111000011110000);
            computer.CPU.REGS.DE = 0b0011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"NAND ({address}), DE",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b1100111111001111, computer.MEMC.Get16bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NAND_InnnI_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set24bitToRAM(address, 0b111100001111000011110000);
            computer.CPU.REGS.DEF = 0b001111000011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"NAND ({address}), DEF",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b110011111100111111001111, (double)computer.MEMC.Get24bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NAND_InnnI_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set32bitToRAM(address, 0b11110000111100001111000011110000);
            computer.CPU.REGS.DEFG = 0b00111100001111000011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"NAND ({address}), DEFG",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11001111110011111100111111001111, (double)computer.MEMC.Get32bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        // AND r/ rr/ rrr/ rrrr, (nnn)
        [Fact]
        public void TestEXEC_NAND_r_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set8bitToRAM(address, 0b11110000);
            computer.CPU.REGS.D = 0b00111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"NAND D, ({address})",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11110000, computer.MEMC.Get8bitFromRAM(address));
            Assert.Equal(0b11001111, computer.CPU.REGS.D);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NAND_rr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set16bitToRAM(address, 0b1111000011110000);
            computer.CPU.REGS.DE = 0b0011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"NAND DE, ({address})",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b1111000011110000, computer.MEMC.Get16bitFromRAM(address));
            Assert.Equal(0b1100111111001111, computer.CPU.REGS.DE);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NAND_rrr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set24bitToRAM(address, 0b111100001111000011110000);
            computer.CPU.REGS.DEF = 0b001111000011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"NAND DEF, ({address})",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b111100001111000011110000, (double)computer.MEMC.Get24bitFromRAM(address));
            Assert.Equal(0b110011111100111111001111, (double)computer.CPU.REGS.DEF);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NAND_rrrr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set32bitToRAM(address, 0b11110000111100001111000011110000);
            computer.CPU.REGS.DEFG = 0b00111100001111000011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"NAND DEFG, ({address})",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11110000111100001111000011110000, (double)computer.MEMC.Get32bitFromRAM(address));
            Assert.Equal(0b11001111110011111100111111001111, (double)computer.CPU.REGS.DEFG);
            TUtils.IncrementCountedTests("exec");
        }
    }
}
