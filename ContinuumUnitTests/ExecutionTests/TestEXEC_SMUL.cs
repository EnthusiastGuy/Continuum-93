using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_SMUL
    {
        [Fact]
        public void TestEXEC_SMUL_r_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = TUtils.GetUnsignedByte(-17);
            computer.CPU.REGS.B = TUtils.GetUnsignedByte(-17);
            computer.CPU.REGS.C = 11;
            computer.CPU.REGS.D = TUtils.GetUnsignedByte(-120);
            computer.CPU.REGS.E = TUtils.GetUnsignedByte(-2);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SMUL A, -3",
                    "SMUL B, 5",
                    "SMUL C, -11",
                    "SMUL D, 10",
                    "SMUL E, -60",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(51, computer.CPU.REGS.A);
            Assert.Equal(TUtils.GetUnsignedByte(-85), computer.CPU.REGS.B);
            Assert.Equal(TUtils.GetUnsignedByte(-121), computer.CPU.REGS.C);
            Assert.Equal(80, computer.CPU.REGS.D);
            Assert.Equal(120, computer.CPU.REGS.E);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SMUL_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = TUtils.GetUnsignedByte(-26);
            computer.CPU.REGS.B = 2;
            computer.CPU.REGS.C = 12;
            computer.CPU.REGS.D = TUtils.GetUnsignedByte(-6);
            computer.CPU.REGS.E = 255;
            computer.CPU.REGS.F = 6;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SMUL A, B",
                    "SMUL C, D",
                    "SMUL E, F",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsignedByte(-52), computer.CPU.REGS.A);
            Assert.Equal(2, computer.CPU.REGS.B);
            Assert.Equal(TUtils.GetUnsignedByte(-72), computer.CPU.REGS.C);
            Assert.Equal(TUtils.GetUnsignedByte(-6), computer.CPU.REGS.D);
            Assert.Equal(250, computer.CPU.REGS.E);
            Assert.Equal(6, computer.CPU.REGS.F);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SMUL_rr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = TUtils.GetUnsignedShort(-9);
            computer.CPU.REGS.CD = 7;
            computer.CPU.REGS.EF = 12;
            computer.CPU.REGS.GH = TUtils.GetUnsignedShort(-120);
            computer.CPU.REGS.IJ = 1255;
            computer.CPU.REGS.KL = 10000;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SMUL AB, 3",
                    "SMUL CD, -4",
                    "SMUL EF, 13",
                    "SMUL GH, 10",
                    "SMUL IJ, 7",
                    "SMUL KL, 15",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsignedShort(-27), computer.CPU.REGS.AB);
            Assert.Equal(TUtils.GetUnsignedShort(-28), computer.CPU.REGS.CD);
            Assert.Equal(156, computer.CPU.REGS.EF);
            Assert.Equal(TUtils.GetUnsignedShort(-1200), computer.CPU.REGS.GH);
            Assert.Equal(8785, computer.CPU.REGS.IJ);
            Assert.Equal(18928, computer.CPU.REGS.KL);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SMUL_rr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = TUtils.GetUnsignedShort(-1000);
            computer.CPU.REGS.C = 20;
            computer.CPU.REGS.DE = 5000;
            computer.CPU.REGS.F = TUtils.GetUnsignedByte(-6);
            computer.CPU.REGS.GH = 32900;
            computer.CPU.REGS.I = 10;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SMUL AB, C",
                    "SMUL DE, F",
                    "SMUL GH, I",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsignedShort(-20000), computer.CPU.REGS.AB);
            Assert.Equal(20, computer.CPU.REGS.C);
            Assert.Equal(TUtils.GetUnsignedShort(-30000), computer.CPU.REGS.DE);
            Assert.Equal(TUtils.GetUnsignedByte(-6), computer.CPU.REGS.F);
            Assert.Equal(1320, computer.CPU.REGS.GH);
            Assert.Equal(10, computer.CPU.REGS.I);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SMUL_rr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = TUtils.GetUnsignedShort(-30);
            computer.CPU.REGS.CD = 300;
            computer.CPU.REGS.EF = 32900;
            computer.CPU.REGS.GH = TUtils.GetUnsignedShort(-1024);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SMUL AB, CD",
                    "SMUL EF, GH",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsignedShort(-9000), computer.CPU.REGS.AB);
            Assert.Equal(300, computer.CPU.REGS.CD);
            Assert.Equal(TUtils.GetUnsignedShort(-4096), computer.CPU.REGS.EF);
            Assert.Equal(TUtils.GetUnsignedShort(-1024), computer.CPU.REGS.GH);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SMUL_rrr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = TUtils.GetUnsigned24BitInt(-3355443);
            computer.CPU.REGS.DEF = 0x1000;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SMUL ABC, 3",
                    "SMUL DEF, -4",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsigned24BitInt(-10066329), (double)computer.CPU.REGS.ABC);
            Assert.Equal(TUtils.GetUnsigned24BitInt(-16384), (double)computer.CPU.REGS.DEF);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SMUL_rrr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = TUtils.GetUnsigned24BitInt(-100000);
            computer.CPU.REGS.D = 10;
            computer.CPU.REGS.EFG = 0x123456;
            computer.CPU.REGS.H = TUtils.GetUnsignedByte(-6);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SMUL ABC, D",
                    "SMUL EFG, H",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsigned24BitInt(-1000000), (double)computer.CPU.REGS.ABC);
            Assert.Equal(10, computer.CPU.REGS.D);
            Assert.Equal(TUtils.GetUnsigned24BitInt(-7158276), (double)computer.CPU.REGS.EFG);
            Assert.Equal(TUtils.GetUnsignedByte(-6), computer.CPU.REGS.H);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SMUL_rrr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = TUtils.GetUnsigned24BitInt(-0x750);
            computer.CPU.REGS.DE = 0x700;
            computer.CPU.REGS.FGH = 0x123456;
            computer.CPU.REGS.IJ = TUtils.GetUnsignedShort(-6);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SMUL ABC, DE",
                    "SMUL FGH, IJ",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsigned24BitInt(-0x333000), (double)computer.CPU.REGS.ABC);
            Assert.Equal(0x700, computer.CPU.REGS.DE);
            Assert.Equal(TUtils.GetUnsigned24BitInt(-7158276), (double)computer.CPU.REGS.FGH);
            Assert.Equal(TUtils.GetUnsignedShort(-6), computer.CPU.REGS.IJ);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SMUL_rrr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = TUtils.GetUnsigned24BitInt(-0x750);
            computer.CPU.REGS.DEF = 0x700;
            computer.CPU.REGS.GHI = 0x1234;
            computer.CPU.REGS.JKL = TUtils.GetUnsigned24BitInt(-0x100);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SMUL ABC, DEF",
                    "SMUL GHI, JKL",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsigned24BitInt(-0x333000), (double)computer.CPU.REGS.ABC);
            Assert.Equal(0x700, (double)computer.CPU.REGS.DEF);
            Assert.Equal(TUtils.GetUnsigned24BitInt(-0x123400), (double)computer.CPU.REGS.GHI);
            Assert.Equal(TUtils.GetUnsigned24BitInt(-0x100), (double)computer.CPU.REGS.JKL);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SMUL_rrrr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = TUtils.GetUnsigned32BitInt(-0x100000);
            computer.CPU.REGS.EFGH = 100000;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SMUL ABCD, 0x200",
                    "SMUL EFGH, -800",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsigned32BitInt(-0x20000000), (double)computer.CPU.REGS.ABCD);
            Assert.Equal(TUtils.GetUnsigned32BitInt(-80000000), (double)computer.CPU.REGS.EFGH);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SMUL_rrrr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = TUtils.GetUnsigned32BitInt(-0x01ABCDEF);
            computer.CPU.REGS.E = 0x46;
            computer.CPU.REGS.FGHI = 0xFFFFFF;
            computer.CPU.REGS.J = TUtils.GetUnsignedByte(-6);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SMUL ABCD, E",
                    "SMUL FGHI, J",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsigned32BitInt(-0x74FA4F5A), (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0x46, computer.CPU.REGS.E);
            Assert.Equal(TUtils.GetUnsigned32BitInt(-0x5FFFFFA), (double)computer.CPU.REGS.FGHI);
            Assert.Equal(TUtils.GetUnsignedByte(-6), computer.CPU.REGS.J);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SMUL_rrrr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = TUtils.GetUnsigned32BitInt(-0x7DEF);
            computer.CPU.REGS.EF = 0x7BCD;
            computer.CPU.REGS.GHIJ = 0xFFF;
            computer.CPU.REGS.KL = TUtils.GetUnsignedShort(-0x3FFF);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SMUL ABCD, EF",
                    "SMUL GHIJ, KL",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsigned32BitInt(-0x3CE6AD63), (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0x7BCD, computer.CPU.REGS.EF);
            Assert.Equal(TUtils.GetUnsigned32BitInt(-0x3FFB001), (double)computer.CPU.REGS.GHIJ);
            Assert.Equal(TUtils.GetUnsignedShort(-0x3FFF), computer.CPU.REGS.KL);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SMUL_rrrr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = TUtils.GetUnsigned32BitInt(-0xDEF);
            computer.CPU.REGS.EFG = 0x756FA;
            computer.CPU.REGS.HIJK = 0xFF;
            computer.CPU.REGS.LMN = TUtils.GetUnsigned24BitInt(-0x31FAEF);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SMUL ABCD, EFG",
                    "SMUL HIJK, LMN",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsigned32BitInt(-0x6644E566), (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0x756FA, (double)computer.CPU.REGS.EFG);
            Assert.Equal(TUtils.GetUnsigned32BitInt(-0x31C8F411), (double)computer.CPU.REGS.HIJK);
            Assert.Equal(TUtils.GetUnsigned24BitInt(-0x31FAEF), (double)computer.CPU.REGS.LMN);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SMUL_rrrr_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = TUtils.GetUnsigned32BitInt(-0x7FFF);
            computer.CPU.REGS.EFGH = 0x3FFF;
            computer.CPU.REGS.IJKL = 0x2;
            computer.CPU.REGS.MNOP = TUtils.GetUnsigned32BitInt(-0x2FFFFFFF);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SMUL ABCD, EFGH",
                    "SMUL IJKL, MNOP",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsigned32BitInt(-0x1FFF4001), (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0x3FFF, (double)computer.CPU.REGS.EFGH);
            Assert.Equal(TUtils.GetUnsigned32BitInt(-0x5FFFFFFE), (double)computer.CPU.REGS.IJKL);
            Assert.Equal(TUtils.GetUnsigned32BitInt(-0x2FFFFFFF), (double)computer.CPU.REGS.MNOP);
            TUtils.IncrementCountedTests("exec");
        }
    }
}
