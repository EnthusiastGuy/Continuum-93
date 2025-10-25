using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{
    public class TestEXEC_NOR
    {
        [Fact]
        public void TestEXEC_NOR_r_n()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.A = 0b00110011;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NOR A, 0b01010101",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b10001000, computer.CPU.REGS.A);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NOR_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.A = 0b00110011;
            computer.CPU.REGS.B = 0b01010101;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NOR A, B",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b10001000, computer.CPU.REGS.A);
            Assert.Equal(0b01010101, computer.CPU.REGS.B);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NOR_rr_n()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.AB = 0b0011001100110011;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NOR AB, 0b0101010101010101",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b1000100010001000, computer.CPU.REGS.AB);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NOR_rr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.AB = 0b0011001100110011;
            computer.CPU.REGS.CD = 0b0101010101010101;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NOR AB, CD",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b1000100010001000, computer.CPU.REGS.AB);
            Assert.Equal(0b0101010101010101, computer.CPU.REGS.CD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NOR_rrr_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABC = 0b001100110011001100110011;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NOR ABC, 0b010101010101010101010101",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b100010001000100010001000, (double)computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NOR_rrr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABC = 0b001100110011001100110011;
            computer.CPU.REGS.DEF = 0b010101010101010101010101;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NOR ABC, DEF",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b100010001000100010001000, (double)computer.CPU.REGS.ABC);
            Assert.Equal(0b010101010101010101010101, (double)computer.CPU.REGS.DEF);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NOR_rrrr_nnnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABCD = 0b00110011001100110011001100110011;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NOR ABCD, 0b01010101010101010101010101010101",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b10001000100010001000100010001000, (double)computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NOR_rrrr_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABCD = 0b00110011001100110011001100110011;
            computer.CPU.REGS.EFGH = 0b01010101010101010101010101010101;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NOR ABCD, EFGH",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b10001000100010001000100010001000, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0b01010101010101010101010101010101, (double)computer.CPU.REGS.EFGH);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NOR_IrrrI_n()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set8bitToRAM(address, 0b00110011);
            computer.CPU.REGS.ABC = address;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NOR (ABC), 0b01010101",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b10001000, computer.MEMC.Get8bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NOR16_IrrrI_nn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set16bitToRAM(address, 0b0011001100110011);
            computer.CPU.REGS.ABC = address;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NOR16 (ABC), 0b0101010101010101",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b1000100010001000, computer.MEMC.Get16bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NOR24_IrrrI_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set24bitToRAM(address, 0b001100110011001100110011);
            computer.CPU.REGS.ABC = address;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NOR24 (ABC), 0b010101010101010101010101",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b100010001000100010001000, (double)computer.MEMC.Get24bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NOR32_IrrrI_nnnn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set32bitToRAM(address, 0b00110011001100110011001100110011);
            computer.CPU.REGS.ABC = address;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NOR32 (ABC), 0b01010101010101010101010101010101",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b10001000100010001000100010001000, (double)computer.MEMC.Get32bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NOR_IrrrI_r()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set8bitToRAM(address, 0b00110011);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.D = 0b01010101;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NOR (ABC), D",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b10001000, computer.MEMC.Get8bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NOR_IrrrI_rr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set16bitToRAM(address, 0b0011001100110011);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DE = 0b0101010101010101;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NOR (ABC), DE",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b1000100010001000, computer.MEMC.Get16bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NOR_IrrrI_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set24bitToRAM(address, 0b001100110011001100110011);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DEF = 0b010101010101010101010101;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NOR (ABC), DEF",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b100010001000100010001000, (double)computer.MEMC.Get24bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NOR_IrrrI_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set32bitToRAM(address, 0b00110011001100110011001100110011);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DEFG = 0b01010101010101010101010101010101;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NOR (ABC), DEFG",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b10001000100010001000100010001000, (double)computer.MEMC.Get32bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }


        // new
        [Fact]
        public void TestEXEC_NOR_r_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set8bitToRAM(address, 0b11110000);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.D = 0b00111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NOR D, (ABC)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11110000, computer.MEMC.Get8bitFromRAM(address));
            Assert.Equal(0b00000011, computer.CPU.REGS.D);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NOR_rr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set16bitToRAM(address, 0b1111000011110000);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DE = 0b0011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NOR DE, (ABC)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b1111000011110000, computer.MEMC.Get16bitFromRAM(address));
            Assert.Equal(0b0000001100000011, computer.CPU.REGS.DE);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NOR_rrr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set24bitToRAM(address, 0b111100001111000011110000);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DEF = 0b001111000011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NOR DEF, (ABC)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b111100001111000011110000, (double)computer.MEMC.Get24bitFromRAM(address));
            Assert.Equal(0b000000110000001100000011, (double)computer.CPU.REGS.DEF);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NOR_rrrr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set32bitToRAM(address, 0b11110000111100001111000011110000);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DEFG = 0b00111100001111000011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "NOR DEFG, (ABC)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11110000111100001111000011110000, (double)computer.MEMC.Get32bitFromRAM(address));
            Assert.Equal(0b00000011000000110000001100000011, (double)computer.CPU.REGS.DEFG);
            TUtils.IncrementCountedTests("exec");
        }

        // OR (nnn), n/ nn/ nnn/ nnnn
        [Fact]
        public void TestEXEC_NOR_InnnI_n()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set8bitToRAM(address, 0b11110000);

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"NOR ({address}), 0b00111100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00000011, computer.MEMC.Get8bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NOR16_InnnI_nn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set16bitToRAM(address, 0b1111000011110000);

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"NOR16 ({address}), 0b0011110000111100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b0000001100000011, computer.MEMC.Get16bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NOR24_InnnI_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set24bitToRAM(address, 0b111100001111000011110000);

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"NOR24 ({address}), 0b001111000011110000111100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b000000110000001100000011, (double)computer.MEMC.Get24bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NOR32_InnnI_nnnn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set32bitToRAM(address, 0b11110000111100001111000011110000);

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"NOR32 ({address}), 0b00111100001111000011110000111100",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00000011000000110000001100000011, (double)computer.MEMC.Get32bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        // OR (nnn), r/ rr/ rrr/ rrrr
        [Fact]
        public void TestEXEC_NOR_InnnI_r()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set8bitToRAM(address, 0b11110000);
            computer.CPU.REGS.D = 0b00111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"NOR ({address}), D",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00000011, computer.MEMC.Get8bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NOR_InnnI_rr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set16bitToRAM(address, 0b1111000011110000);
            computer.CPU.REGS.DE = 0b0011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"NOR ({address}), DE",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b0000001100000011, computer.MEMC.Get16bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NOR_InnnI_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set24bitToRAM(address, 0b111100001111000011110000);
            computer.CPU.REGS.DEF = 0b001111000011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"NOR ({address}), DEF",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b000000110000001100000011, (double)computer.MEMC.Get24bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NOR_InnnI_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set32bitToRAM(address, 0b11110000111100001111000011110000);
            computer.CPU.REGS.DEFG = 0b00111100001111000011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"NOR ({address}), DEFG",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00000011000000110000001100000011, (double)computer.MEMC.Get32bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        // OR r/ rr/ rrr/ rrrr, (nnn)
        [Fact]
        public void TestEXEC_NOR_r_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set8bitToRAM(address, 0b11110000);
            computer.CPU.REGS.D = 0b00111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"NOR D, ({address})",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11110000, computer.MEMC.Get8bitFromRAM(address));
            Assert.Equal(0b00000011, computer.CPU.REGS.D);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NOR_rr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set16bitToRAM(address, 0b1111000011110000);
            computer.CPU.REGS.DE = 0b0011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"NOR DE, ({address})",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b1111000011110000, computer.MEMC.Get16bitFromRAM(address));
            Assert.Equal(0b0000001100000011, computer.CPU.REGS.DE);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NOR_rrr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set24bitToRAM(address, 0b111100001111000011110000);
            computer.CPU.REGS.DEF = 0b001111000011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"NOR DEF, ({address})",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b111100001111000011110000, (double)computer.MEMC.Get24bitFromRAM(address));
            Assert.Equal(0b000000110000001100000011, (double)computer.CPU.REGS.DEF);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_NOR_rrrr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set32bitToRAM(address, 0b11110000111100001111000011110000);
            computer.CPU.REGS.DEFG = 0b00111100001111000011110000111100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"NOR DEFG, ({address})",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11110000111100001111000011110000, (double)computer.MEMC.Get32bitFromRAM(address));
            Assert.Equal(0b00000011000000110000001100000011, (double)computer.CPU.REGS.DEFG);
            TUtils.IncrementCountedTests("exec");
        }
    }
}
