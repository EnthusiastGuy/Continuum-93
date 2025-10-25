using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_AND
    {
        [Fact]
        public void TestEXEC_AND_r_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 0b11111111;
            computer.CPU.REGS.B = 0b11111111;
            computer.CPU.REGS.C = 0b11111111;
            computer.CPU.REGS.D = 0b10101010;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "AND A, 0",
                    "AND B, 0b00100100",
                    "AND C, 0b00001111",
                    "AND D, 0b11111111",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00000000, computer.CPU.REGS.A);
            Assert.Equal(0b00100100, computer.CPU.REGS.B);
            Assert.Equal(0b00001111, computer.CPU.REGS.C);
            Assert.Equal(0b10101010, computer.CPU.REGS.D);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_AND_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 0b11111111;
            computer.CPU.REGS.B = 0b11111111;
            computer.CPU.REGS.C = 0b11111111;
            computer.CPU.REGS.D = 0b10101010;

            computer.CPU.REGS.F = 0;
            computer.CPU.REGS.G = 0b00100100;
            computer.CPU.REGS.H = 0b00001111;
            computer.CPU.REGS.I = 0b11111111;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "AND A, F",
                    "AND B, G",
                    "AND C, H",
                    "AND D, I",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00000000, computer.CPU.REGS.A);
            Assert.Equal(0b00100100, computer.CPU.REGS.B);
            Assert.Equal(0b00001111, computer.CPU.REGS.C);
            Assert.Equal(0b10101010, computer.CPU.REGS.D);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_AND_rr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 0b1111111111111111;
            computer.CPU.REGS.CD = 0b1111111111111111;
            computer.CPU.REGS.EF = 0b1111111111111111;
            computer.CPU.REGS.GH = 0b1111111100000000;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "AND AB, 0",
                    "AND CD, 0b0000001000010000",
                    "AND EF, 0b0000000011111111",
                    "AND GH, 0b1010101010101010",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b0000000000000000, computer.CPU.REGS.AB);
            Assert.Equal(0b0000001000010000, computer.CPU.REGS.CD);
            Assert.Equal(0b0000000011111111, computer.CPU.REGS.EF);
            Assert.Equal(0b1010101000000000, computer.CPU.REGS.GH);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_AND_rr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 0b1111111111111111;
            computer.CPU.REGS.CD = 0b1111111111111111;
            computer.CPU.REGS.EF = 0b1111111111111111;
            computer.CPU.REGS.GH = 0b1111111100000000;

            computer.CPU.REGS.KL = 0;
            computer.CPU.REGS.MN = 0b0000001000010000;
            computer.CPU.REGS.OP = 0b0000000011111111;
            computer.CPU.REGS.QR = 0b1010101010101010;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "AND AB, KL",
                    "AND CD, MN",
                    "AND EF, OP",
                    "AND GH, QR",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b0000000000000000, computer.CPU.REGS.AB);
            Assert.Equal(0b0000001000010000, computer.CPU.REGS.CD);
            Assert.Equal(0b0000000011111111, computer.CPU.REGS.EF);
            Assert.Equal(0b1010101000000000, computer.CPU.REGS.GH);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_AND_rrr_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 0b111111111111111111111111;
            computer.CPU.REGS.DEF = 0b111111111111111111111111;
            computer.CPU.REGS.GHI = 0b111111111111111111111111;
            computer.CPU.REGS.JKL = 0b111111111111000000000000;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "AND ABC, 0",
                    "AND DEF, 0b000000100001000001000001",
                    "AND GHI, 0b000000000000111111111111",
                    "AND JKL, 0b101010101010101010101010",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b000000000000000000000000, (double)computer.CPU.REGS.ABC);
            Assert.Equal(0b000000100001000001000001, (double)computer.CPU.REGS.DEF);
            Assert.Equal(0b000000000000111111111111, (double)computer.CPU.REGS.GHI);
            Assert.Equal(0b101010101010000000000000, (double)computer.CPU.REGS.JKL);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_AND_rrr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 0b111111111111111111111111;
            computer.CPU.REGS.DEF = 0b111111111111111111111111;
            computer.CPU.REGS.GHI = 0b111111111111111111111111;
            computer.CPU.REGS.JKL = 0b111111111111000000000000;

            computer.CPU.REGS.MNO = 0;
            computer.CPU.REGS.PQR = 0b000000100001000001000001;
            computer.CPU.REGS.STU = 0b000000000000111111111111;
            computer.CPU.REGS.VWX = 0b101010101010101010101010;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "AND ABC, MNO",
                    "AND DEF, PQR",
                    "AND GHI, STU",
                    "AND JKL, VWX",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b000000000000000000000000, (double)computer.CPU.REGS.ABC);
            Assert.Equal(0b000000100001000001000001, (double)computer.CPU.REGS.DEF);
            Assert.Equal(0b000000000000111111111111, (double)computer.CPU.REGS.GHI);
            Assert.Equal(0b101010101010000000000000, (double)computer.CPU.REGS.JKL);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_AND_rrrr_nnnn()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0b11111111111111111111111111111111;
            computer.CPU.REGS.EFGH = 0b11111111111111111111111111111111;
            computer.CPU.REGS.IJKL = 0b11111111111111110000000000000000;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "AND ABCD, 0",
                    "AND EFGH, 0b00000010000100000100000100010001",
                    "AND IJKL, 0b10101010101010101010101010101010",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00000000000000000000000000000000, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0b00000010000100000100000100010001, (double)computer.CPU.REGS.EFGH);
            Assert.Equal(0b10101010101010100000000000000000, (double)computer.CPU.REGS.IJKL);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_AND_rrrr_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0b00110011001100110011001100110011;
            computer.CPU.REGS.EFGH = 0b01010101010101010101010101010101;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "AND ABCD, EFGH",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00010001000100010001000100010001, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0b01010101010101010101010101010101, (double)computer.CPU.REGS.EFGH);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_AND_IrrrI_n()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set8bitToRAM(address, 0b11110000);
            computer.CPU.REGS.ABC = address;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "AND (ABC), 0b00111100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00110000, computer.MEMC.Get8bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_AND16_IrrrI_nn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set16bitToRAM(address, 0b1111000011110000);
            computer.CPU.REGS.ABC = address;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "AND16 (ABC), 0b0011110000111100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b0011000000110000, computer.MEMC.Get16bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_AND24_IrrrI_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set24bitToRAM(address, 0b111100001111000011110000);
            computer.CPU.REGS.ABC = address;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "AND24 (ABC), 0b001111000011110000111100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b001100000011000000110000, (double)computer.MEMC.Get24bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_AND32_IrrrI_nnnn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set32bitToRAM(address, 0b11110000111100001111000011110000);
            computer.CPU.REGS.ABC = address;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "AND32 (ABC), 0b00111100001111000011110000111100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00110000001100000011000000110000, (double)computer.MEMC.Get32bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_AND_IrrrI_r()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set8bitToRAM(address, 0b11110000);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.D = 0b00111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "AND (ABC), D",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00110000, computer.MEMC.Get8bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_AND_IrrrI_rr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set16bitToRAM(address, 0b1111000011110000);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DE = 0b0011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "AND (ABC), DE",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b0011000000110000, computer.MEMC.Get16bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_AND_IrrrI_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set24bitToRAM(address, 0b111100001111000011110000);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DEF = 0b001111000011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "AND (ABC), DEF",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b001100000011000000110000, (double)computer.MEMC.Get24bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_AND_IrrrI_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set32bitToRAM(address, 0b11110000111100001111000011110000);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DEFG = 0b00111100001111000011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "AND (ABC), DEFG",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00110000001100000011000000110000, (double)computer.MEMC.Get32bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_AND_r_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set8bitToRAM(address, 0b11110000);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.D = 0b00111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "AND D, (ABC)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11110000, computer.MEMC.Get8bitFromRAM(address));
            Assert.Equal(0b00110000, computer.CPU.REGS.D);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_AND_rr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set16bitToRAM(address, 0b1111000011110000);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DE = 0b0011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "AND DE, (ABC)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b1111000011110000, computer.MEMC.Get16bitFromRAM(address));
            Assert.Equal(0b0011000000110000, computer.CPU.REGS.DE);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_AND_rrr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set24bitToRAM(address, 0b111100001111000011110000);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DEF = 0b001111000011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "AND DEF, (ABC)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b111100001111000011110000, (double)computer.MEMC.Get24bitFromRAM(address));
            Assert.Equal(0b001100000011000000110000, (double)computer.CPU.REGS.DEF);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_AND_rrrr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set32bitToRAM(address, 0b11110000111100001111000011110000);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DEFG = 0b00111100001111000011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "AND DEFG, (ABC)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11110000111100001111000011110000, (double)computer.MEMC.Get32bitFromRAM(address));
            Assert.Equal(0b00110000001100000011000000110000, (double)computer.CPU.REGS.DEFG);
            TUtils.IncrementCountedTests("exec");
        }

        // AND (nnn), n/ nn/ nnn/ nnnn
        [Fact]
        public void TestEXEC_AND_InnnI_n()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set8bitToRAM(address, 0b11110000);

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"AND ({address}), 0b00111100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00110000, computer.MEMC.Get8bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_AND16_InnnI_nn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set16bitToRAM(address, 0b1111000011110000);

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"AND16 ({address}), 0b0011110000111100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b0011000000110000, computer.MEMC.Get16bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_AND24_InnnI_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set24bitToRAM(address, 0b111100001111000011110000);

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"AND24 ({address}), 0b001111000011110000111100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b001100000011000000110000, (double)computer.MEMC.Get24bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_AND32_InnnI_nnnn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set32bitToRAM(address, 0b11110000111100001111000011110000);

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"AND32 ({address}), 0b00111100001111000011110000111100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00110000001100000011000000110000, (double)computer.MEMC.Get32bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        // AND (nnn), r/ rr/ rrr/ rrrr
        [Fact]
        public void TestEXEC_AND_InnnI_r()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set8bitToRAM(address, 0b11110000);
            computer.CPU.REGS.D = 0b00111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"AND ({address}), D",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00110000, computer.MEMC.Get8bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_AND_InnnI_rr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set16bitToRAM(address, 0b1111000011110000);
            computer.CPU.REGS.DE = 0b0011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"AND ({address}), DE",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b0011000000110000, computer.MEMC.Get16bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_AND_InnnI_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set24bitToRAM(address, 0b111100001111000011110000);
            computer.CPU.REGS.DEF = 0b001111000011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"AND ({address}), DEF",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b001100000011000000110000, (double)computer.MEMC.Get24bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_AND_InnnI_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set32bitToRAM(address, 0b11110000111100001111000011110000);
            computer.CPU.REGS.DEFG = 0b00111100001111000011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"AND ({address}), DEFG",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00110000001100000011000000110000, (double)computer.MEMC.Get32bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        // AND r/ rr/ rrr/ rrrr, (nnn)
        [Fact]
        public void TestEXEC_AND_r_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set8bitToRAM(address, 0b11110000);
            computer.CPU.REGS.D = 0b00111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"AND D, ({address})",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11110000, computer.MEMC.Get8bitFromRAM(address));
            Assert.Equal(0b00110000, computer.CPU.REGS.D);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_AND_rr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set16bitToRAM(address, 0b1111000011110000);
            computer.CPU.REGS.DE = 0b0011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"AND DE, ({address})",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b1111000011110000, computer.MEMC.Get16bitFromRAM(address));
            Assert.Equal(0b0011000000110000, computer.CPU.REGS.DE);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_AND_rrr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set24bitToRAM(address, 0b111100001111000011110000);
            computer.CPU.REGS.DEF = 0b001111000011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"AND DEF, ({address})",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b111100001111000011110000, (double)computer.MEMC.Get24bitFromRAM(address));
            Assert.Equal(0b001100000011000000110000, (double)computer.CPU.REGS.DEF);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_AND_rrrr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set32bitToRAM(address, 0b11110000111100001111000011110000);
            computer.CPU.REGS.DEFG = 0b00111100001111000011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"AND DEFG, ({address})",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11110000111100001111000011110000, (double)computer.MEMC.Get32bitFromRAM(address));
            Assert.Equal(0b00110000001100000011000000110000, (double)computer.CPU.REGS.DEFG);
            TUtils.IncrementCountedTests("exec");
        }
    }
}
