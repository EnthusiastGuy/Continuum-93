using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{
    public class TestEXEC_IMPLY
    {
        public const byte LEFT_8BIT = 0b00110011;
        public const ushort LEFT_16BIT = (LEFT_8BIT << 8) + LEFT_8BIT;
        public const uint LEFT_24BIT = (LEFT_16BIT << 8) + LEFT_8BIT;
        public const uint LEFT_32BIT = (LEFT_24BIT << 8) + LEFT_8BIT;

        public const byte RIGHT_8BIT = 0b01010101;
        public const ushort RIGHT_16BIT = (RIGHT_8BIT << 8) + RIGHT_8BIT;
        public const uint RIGHT_24BIT = (RIGHT_16BIT << 8) + RIGHT_8BIT;
        public const uint RIGHT_32BIT = (RIGHT_24BIT << 8) + RIGHT_8BIT;

        public const byte EXPECTED_8BIT = 0b11011101;
        public const ushort EXPECTED_16BIT = (EXPECTED_8BIT << 8) + EXPECTED_8BIT;
        public const uint EXPECTED_24BIT = (EXPECTED_16BIT << 8) + EXPECTED_8BIT;
        public const uint EXPECTED_32BIT = (EXPECTED_24BIT << 8) + EXPECTED_8BIT;

        [Fact]
        public void TestEXEC_IMPLY_r_n()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.A = LEFT_8BIT;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"IMPLY A, {RIGHT_8BIT}",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(EXPECTED_8BIT, computer.CPU.REGS.A);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_IMPLY_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.A = LEFT_8BIT;
            computer.CPU.REGS.F = RIGHT_8BIT;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "IMPLY A, F",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(EXPECTED_8BIT, computer.CPU.REGS.A);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_IMPLY_rr_n()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.AB = LEFT_16BIT;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"IMPLY AB, {RIGHT_16BIT}",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(EXPECTED_16BIT, computer.CPU.REGS.AB);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_IMPLY_rr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.AB = LEFT_16BIT;
            computer.CPU.REGS.KL = RIGHT_16BIT;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "IMPLY AB, KL",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(EXPECTED_16BIT, computer.CPU.REGS.AB);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_IMPLY_rrr_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABC = LEFT_24BIT;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"IMPLY ABC, {RIGHT_24BIT}",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(EXPECTED_24BIT, (double)computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_IMPLY_rrr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABC = LEFT_24BIT;
            computer.CPU.REGS.MNO = RIGHT_24BIT;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "IMPLY ABC, MNO",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(EXPECTED_24BIT, (double)computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_IMPLY_rrrr_nnnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABCD = LEFT_32BIT;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"IMPLY ABCD, {RIGHT_32BIT}",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(EXPECTED_32BIT, (double)computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_IMPLY_rrrr_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABCD = LEFT_32BIT;
            computer.CPU.REGS.MNOP = RIGHT_32BIT;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "IMPLY ABCD, MNOP",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(EXPECTED_32BIT, (double)computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_IMPLY_IrrrI_n()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set8bitToRAM(address, LEFT_8BIT);
            computer.CPU.REGS.ABC = address;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"IMPLY (ABC), {RIGHT_8BIT}",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(EXPECTED_8BIT, computer.MEMC.Get8bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_IMPLY16_IrrrI_nn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set16bitToRAM(address, LEFT_16BIT);
            computer.CPU.REGS.ABC = address;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"IMPLY16 (ABC), {RIGHT_16BIT}",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(EXPECTED_16BIT, computer.MEMC.Get16bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_IMPLY24_IrrrI_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set24bitToRAM(address, LEFT_24BIT);
            computer.CPU.REGS.ABC = address;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"IMPLY24 (ABC), {RIGHT_24BIT}",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(EXPECTED_24BIT, (double)computer.MEMC.Get24bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_IMPLY32_IrrrI_nnnn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set32bitToRAM(address, LEFT_32BIT);
            computer.CPU.REGS.ABC = address;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"IMPLY32 (ABC), {RIGHT_32BIT}",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(EXPECTED_32BIT, (double)computer.MEMC.Get32bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_IMPLY_IrrrI_r()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set8bitToRAM(address, LEFT_8BIT);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.D = RIGHT_8BIT;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "IMPLY (ABC), D",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(EXPECTED_8BIT, computer.MEMC.Get8bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_IMPLY_IrrrI_rr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set16bitToRAM(address, LEFT_16BIT);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DE = RIGHT_16BIT;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "IMPLY (ABC), DE",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(EXPECTED_16BIT, computer.MEMC.Get16bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_IMPLY_IrrrI_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set24bitToRAM(address, LEFT_24BIT);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DEF = RIGHT_24BIT;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "IMPLY (ABC), DEF",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(EXPECTED_24BIT, (double)computer.MEMC.Get24bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_IMPLY_IrrrI_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set32bitToRAM(address, LEFT_32BIT);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DEFG = RIGHT_32BIT;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "IMPLY (ABC), DEFG",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(EXPECTED_32BIT, (double)computer.MEMC.Get32bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_IMPLY_r_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set8bitToRAM(address, RIGHT_8BIT);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.D = LEFT_8BIT;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "IMPLY D, (ABC)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(RIGHT_8BIT, computer.MEMC.Get8bitFromRAM(address));
            Assert.Equal(EXPECTED_8BIT, computer.CPU.REGS.D);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_IMPLY_rr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set16bitToRAM(address, RIGHT_16BIT);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DE = LEFT_16BIT;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "IMPLY DE, (ABC)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(RIGHT_16BIT, computer.MEMC.Get16bitFromRAM(address));
            Assert.Equal(EXPECTED_16BIT, computer.CPU.REGS.DE);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_IMPLY_rrr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set24bitToRAM(address, RIGHT_24BIT);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DEF = LEFT_24BIT;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "IMPLY DEF, (ABC)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(RIGHT_24BIT, (double)computer.MEMC.Get24bitFromRAM(address));
            Assert.Equal(EXPECTED_24BIT, (double)computer.CPU.REGS.DEF);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_IMPLY_rrrr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;


            computer.MEMC.Set32bitToRAM(address, RIGHT_32BIT);
            computer.CPU.REGS.ABC = address;
            computer.CPU.REGS.DEFG = LEFT_32BIT;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "IMPLY DEFG, (ABC)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(RIGHT_32BIT, (double)computer.MEMC.Get32bitFromRAM(address));
            Assert.Equal(EXPECTED_32BIT, (double)computer.CPU.REGS.DEFG);
            TUtils.IncrementCountedTests("exec");
        }

        // IMPLY (nnn), n/ nn/ nnn/ nnnn
        [Fact]
        public void TestEXEC_IMPLY_InnnI_n()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set8bitToRAM(address, LEFT_8BIT);

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"IMPLY ({address}), {RIGHT_8BIT}",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(EXPECTED_8BIT, computer.MEMC.Get8bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_IMPLY16_InnnI_nn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set16bitToRAM(address, LEFT_16BIT);

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"IMPLY16 ({address}), {RIGHT_16BIT}",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(EXPECTED_16BIT, computer.MEMC.Get16bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_IMPLY24_InnnI_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set24bitToRAM(address, LEFT_24BIT);

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"IMPLY24 ({address}), {RIGHT_24BIT}",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(EXPECTED_24BIT, (double)computer.MEMC.Get24bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_IMPLY32_InnnI_nnnn()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set32bitToRAM(address, LEFT_32BIT);

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"IMPLY32 ({address}), {RIGHT_32BIT}",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(EXPECTED_32BIT, (double)computer.MEMC.Get32bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        // IMPLY (nnn), r/ rr/ rrr/ rrrr
        [Fact]
        public void TestEXEC_IMPLY_InnnI_r()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set8bitToRAM(address, LEFT_8BIT);
            computer.CPU.REGS.D = RIGHT_8BIT;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"IMPLY ({address}), D",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(EXPECTED_8BIT, computer.MEMC.Get8bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_IMPLY_InnnI_rr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set16bitToRAM(address, LEFT_16BIT);
            computer.CPU.REGS.DE = RIGHT_16BIT;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"IMPLY ({address}), DE",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(EXPECTED_16BIT, computer.MEMC.Get16bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_IMPLY_InnnI_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set24bitToRAM(address, LEFT_24BIT);
            computer.CPU.REGS.DEF = RIGHT_24BIT;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"IMPLY ({address}), DEF",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(EXPECTED_24BIT, (double)computer.MEMC.Get24bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_IMPLY_InnnI_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set32bitToRAM(address, LEFT_32BIT);
            computer.CPU.REGS.DEFG = RIGHT_32BIT;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"IMPLY ({address}), DEFG",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(EXPECTED_32BIT, (double)computer.MEMC.Get32bitFromRAM(address));
            TUtils.IncrementCountedTests("exec");
        }

        // IMPLY r/ rr/ rrr/ rrrr, (nnn)
        [Fact]
        public void TestEXEC_IMPLY_r_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set8bitToRAM(address, RIGHT_8BIT);
            computer.CPU.REGS.D = LEFT_8BIT;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"IMPLY D, ({address})",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(RIGHT_8BIT, computer.MEMC.Get8bitFromRAM(address));
            Assert.Equal(EXPECTED_8BIT, computer.CPU.REGS.D);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_IMPLY_rr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set16bitToRAM(address, RIGHT_16BIT);
            computer.CPU.REGS.DE = LEFT_16BIT;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"IMPLY DE, ({address})",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(RIGHT_16BIT, computer.MEMC.Get16bitFromRAM(address));
            Assert.Equal(EXPECTED_16BIT, computer.CPU.REGS.DE);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_IMPLY_rrr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set24bitToRAM(address, RIGHT_24BIT);
            computer.CPU.REGS.DEF = LEFT_24BIT;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"IMPLY DEF, ({address})",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(RIGHT_24BIT, (double)computer.MEMC.Get24bitFromRAM(address));
            Assert.Equal(EXPECTED_24BIT, (double)computer.CPU.REGS.DEF);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_IMPLY_rrrr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();
            uint address = 50;

            computer.MEMC.Set32bitToRAM(address, RIGHT_32BIT);
            computer.CPU.REGS.DEFG = LEFT_32BIT;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"IMPLY DEFG, ({address})",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(RIGHT_32BIT, (double)computer.MEMC.Get32bitFromRAM(address));
            Assert.Equal(EXPECTED_32BIT, (double)computer.CPU.REGS.DEFG);
            TUtils.IncrementCountedTests("exec");
        }
    }
}
