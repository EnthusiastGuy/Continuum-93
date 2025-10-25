using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_MUL
    {
        [Fact]
        public void TestEXEC_MUL_r_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 9;
            computer.CPU.REGS.B = 7;
            computer.CPU.REGS.C = 12;
            computer.CPU.REGS.D = 120;
            computer.CPU.REGS.E = 255;
            computer.CPU.REGS.F = 5;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MUL A, 3",
                    "MUL B, 4",
                    "MUL C, 13",
                    "MUL D, 10",
                    "MUL E, 5",
                    "MUL F, 1",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(27, computer.CPU.REGS.A);
            Assert.Equal(28, computer.CPU.REGS.B);
            Assert.Equal(156, computer.CPU.REGS.C);
            Assert.Equal(176, computer.CPU.REGS.D);
            Assert.Equal(251, computer.CPU.REGS.E);
            Assert.Equal(5, computer.CPU.REGS.F);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MUL_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 26;
            computer.CPU.REGS.B = 2;
            computer.CPU.REGS.C = 12;
            computer.CPU.REGS.D = 6;
            computer.CPU.REGS.E = 255;
            computer.CPU.REGS.F = 6;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MUL A, B",
                    "MUL C, D",
                    "MUL E, F",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(52, computer.CPU.REGS.A);
            Assert.Equal(2, computer.CPU.REGS.B);
            Assert.Equal(72, computer.CPU.REGS.C);
            Assert.Equal(6, computer.CPU.REGS.D);
            Assert.Equal(250, computer.CPU.REGS.E);
            Assert.Equal(6, computer.CPU.REGS.F);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MUL_rr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 9;
            computer.CPU.REGS.CD = 7;
            computer.CPU.REGS.EF = 12;
            computer.CPU.REGS.GH = 120;
            computer.CPU.REGS.IJ = 1255;
            computer.CPU.REGS.KL = 10000;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MUL AB, 3",
                    "MUL CD, 4",
                    "MUL EF, 13",
                    "MUL GH, 10",
                    "MUL IJ, 7",
                    "MUL KL, 15",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(27, computer.CPU.REGS.AB);
            Assert.Equal(28, computer.CPU.REGS.CD);
            Assert.Equal(156, computer.CPU.REGS.EF);
            Assert.Equal(1200, computer.CPU.REGS.GH);
            Assert.Equal(8785, computer.CPU.REGS.IJ);
            Assert.Equal(18928, computer.CPU.REGS.KL);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MUL_rr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 2000;
            computer.CPU.REGS.C = 20;
            computer.CPU.REGS.DE = 5000;
            computer.CPU.REGS.F = 6;
            computer.CPU.REGS.GH = 32900;
            computer.CPU.REGS.I = 10;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MUL AB, C",
                    "MUL DE, F",
                    "MUL GH, I",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(40000, computer.CPU.REGS.AB);
            Assert.Equal(20, computer.CPU.REGS.C);
            Assert.Equal(30000, computer.CPU.REGS.DE);
            Assert.Equal(6, computer.CPU.REGS.F);
            Assert.Equal(1320, computer.CPU.REGS.GH);
            Assert.Equal(10, computer.CPU.REGS.I);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MUL_rr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 30;
            computer.CPU.REGS.CD = 300;
            computer.CPU.REGS.EF = 32900;
            computer.CPU.REGS.GH = 1024;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MUL AB, CD",
                    "MUL EF, GH",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(9000, computer.CPU.REGS.AB);
            Assert.Equal(300, computer.CPU.REGS.CD);
            Assert.Equal(4096, computer.CPU.REGS.EF);
            Assert.Equal(1024, computer.CPU.REGS.GH);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MUL_rrr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 0x333333;
            computer.CPU.REGS.DEF = 0x1000;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MUL ABC, 3",
                    "MUL DEF, 4",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x999999, (double)computer.CPU.REGS.ABC);
            Assert.Equal(0x4000, (double)computer.CPU.REGS.DEF);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MUL_rrr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 0x012345;
            computer.CPU.REGS.D = 0x0F;
            computer.CPU.REGS.EFG = 0x123456;
            computer.CPU.REGS.H = 0xFF;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MUL ABC, D",
                    "MUL EFG, H",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x11110B, (double)computer.CPU.REGS.ABC);
            Assert.Equal(0x0F, computer.CPU.REGS.D);
            Assert.Equal(0x2221AA, (double)computer.CPU.REGS.EFG);
            Assert.Equal(0xFF, computer.CPU.REGS.H);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MUL_rrr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 0x750;
            computer.CPU.REGS.DE = 0x700;
            computer.CPU.REGS.FGH = 0x123456;
            computer.CPU.REGS.IJ = 0xFFFF;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MUL ABC, DE",
                    "MUL FGH, IJ",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x333000, (double)computer.CPU.REGS.ABC);
            Assert.Equal(0x700, computer.CPU.REGS.DE);
            Assert.Equal(0x43CBAA, (double)computer.CPU.REGS.FGH);
            Assert.Equal(0xFFFF, computer.CPU.REGS.IJ);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MUL_rrr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 0x750;
            computer.CPU.REGS.DEF = 0x700;
            computer.CPU.REGS.GHI = 0x123456;
            computer.CPU.REGS.JKL = 0xFFFF;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MUL ABC, DEF",
                    "MUL GHI, JKL",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x333000, (double)computer.CPU.REGS.ABC);
            Assert.Equal(0x700, (double)computer.CPU.REGS.DEF);
            Assert.Equal(0x43CBAA, (double)computer.CPU.REGS.GHI);
            Assert.Equal(0xFFFF, (double)computer.CPU.REGS.JKL);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MUL_rrrr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0x100000;
            computer.CPU.REGS.EFGH = 0x10101;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MUL ABCD, 0x200",
                    "MUL EFGH, 0x1000",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x20000000, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0x10101000, (double)computer.CPU.REGS.EFGH);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MUL_rrrr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0x01ABCDEF;
            computer.CPU.REGS.E = 0x56;
            computer.CPU.REGS.FGHI = 0xFFFFFF;
            computer.CPU.REGS.J = 0xFA;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MUL ABCD, E",
                    "MUL FGHI, J",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x8FB72E4A, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0x56, computer.CPU.REGS.E);
            Assert.Equal(0xF9FFFF06, (double)computer.CPU.REGS.FGHI);
            Assert.Equal(0xFA, computer.CPU.REGS.J);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MUL_rrrr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0xCDEF;
            computer.CPU.REGS.EF = 0xABCD;
            computer.CPU.REGS.GHIJ = 0xFFF;
            computer.CPU.REGS.KL = 0xFFFF;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MUL ABCD, EF",
                    "MUL GHIJ, KL",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x8A338D63, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0xABCD, computer.CPU.REGS.EF);
            Assert.Equal(0xFFEF001, (double)computer.CPU.REGS.GHIJ);
            Assert.Equal(0xFFFF, computer.CPU.REGS.KL);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MUL_rrrr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0xDEF;
            computer.CPU.REGS.EFG = 0xF56FA;
            computer.CPU.REGS.HIJK = 0xFF;
            computer.CPU.REGS.LMN = 0x31FAEF;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MUL ABCD, EFG",
                    "MUL HIJK, LMN",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xD5BCE566, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0xF56FA, (double)computer.CPU.REGS.EFG);
            Assert.Equal(0x31C8F411, (double)computer.CPU.REGS.HIJK);
            Assert.Equal(0x31FAEF, (double)computer.CPU.REGS.LMN);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MUL_rrrr_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0xAFFF;
            computer.CPU.REGS.EFGH = 0xBFFF;
            computer.CPU.REGS.IJKL = 0x2;
            computer.CPU.REGS.MNOP = 0x2FFFFFFF;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MUL ABCD, EFGH",
                    "MUL IJKL, MNOP",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x83FE9001, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0xBFFF, (double)computer.CPU.REGS.EFGH);
            Assert.Equal(0x5FFFFFFE, (double)computer.CPU.REGS.IJKL);
            Assert.Equal(0x2FFFFFFF, (double)computer.CPU.REGS.MNOP);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MUL_by_zero_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 9;
            computer.CPU.REGS.BC = 700;
            computer.CPU.REGS.DEF = 120000;
            computer.CPU.REGS.GHIJ = 2000000000;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MUL A, 0",
                    "MUL BC, 0",
                    "MUL DEF, 0",
                    "MUL GHIJ, 0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, computer.CPU.REGS.A);
            Assert.Equal(0, computer.CPU.REGS.BC);
            Assert.Equal(0, (double)computer.CPU.REGS.DEF);
            Assert.Equal(0, (double)computer.CPU.REGS.GHIJ);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MUL_by_zero_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 9;
            computer.CPU.REGS.BC = 700;
            computer.CPU.REGS.DEF = 120000;
            computer.CPU.REGS.GHIJ = 2000000000;
            computer.CPU.REGS.Z = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MUL A, Z",
                    "MUL BC, Z",
                    "MUL DEF, Z",
                    "MUL GHIJ, Z",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, computer.CPU.REGS.A);
            Assert.Equal(0, computer.CPU.REGS.BC);
            Assert.Equal(0, (double)computer.CPU.REGS.DEF);
            Assert.Equal(0, (double)computer.CPU.REGS.GHIJ);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MUL_by_zero_rr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.BC = 700;
            computer.CPU.REGS.DEF = 120000;
            computer.CPU.REGS.GHIJ = 2000000000;
            computer.CPU.REGS.YZ = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MUL BC, YZ",
                    "MUL DEF, YZ",
                    "MUL GHIJ, YZ",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, computer.CPU.REGS.BC);
            Assert.Equal(0, (double)computer.CPU.REGS.DEF);
            Assert.Equal(0, (double)computer.CPU.REGS.GHIJ);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MUL_by_zero_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.DEF = 120000;
            computer.CPU.REGS.GHIJ = 2000000000;
            computer.CPU.REGS.XYZ = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MUL DEF, XYZ",
                    "MUL GHIJ, XYZ",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, (double)computer.CPU.REGS.DEF);
            Assert.Equal(0, (double)computer.CPU.REGS.GHIJ);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MUL_by_zero_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.DEF = 120000;
            computer.CPU.REGS.GHIJ = 2000000000;
            computer.CPU.REGS.WXYZ = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MUL GHIJ, WXYZ",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, (double)computer.CPU.REGS.GHIJ);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MUL_overflow()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.A = 64;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MUL A, 0x0A",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(128, (double)computer.CPU.REGS.A);
            TUtils.IncrementCountedTests("exec");
        }
    }
}
