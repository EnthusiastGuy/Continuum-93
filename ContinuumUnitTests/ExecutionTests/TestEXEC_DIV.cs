using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_DIV
    {
        [Fact]
        public void TestEXEC_DIV_r_n()
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
                    "DIV A, 3",
                    "DIV B, 4",
                    "DIV C, 13",
                    "DIV D, 10",
                    "DIV E, 5",
                    "DIV F, 1",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(3, computer.CPU.REGS.A);
            Assert.Equal(1, computer.CPU.REGS.B);
            Assert.Equal(0, computer.CPU.REGS.C);
            Assert.Equal(12, computer.CPU.REGS.D);
            Assert.Equal(51, computer.CPU.REGS.E);
            Assert.Equal(5, computer.CPU.REGS.F);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_r_r()
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
                    "DIV A, B",
                    "DIV C, D",
                    "DIV E, F",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(13, computer.CPU.REGS.A);
            Assert.Equal(2, computer.CPU.REGS.B);
            Assert.Equal(2, computer.CPU.REGS.C);
            Assert.Equal(6, computer.CPU.REGS.D);
            Assert.Equal(42, computer.CPU.REGS.E);
            Assert.Equal(6, computer.CPU.REGS.F);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 9;
            computer.CPU.REGS.CD = 7;
            computer.CPU.REGS.EF = 12;
            computer.CPU.REGS.GH = 120;
            computer.CPU.REGS.IJ = 1255;
            computer.CPU.REGS.KL = 65535;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DIV AB, 3",
                    "DIV CD, 4",
                    "DIV EF, 13",
                    "DIV GH, 10",
                    "DIV IJ, 7",
                    "DIV KL, 15",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(3, computer.CPU.REGS.AB);
            Assert.Equal(1, computer.CPU.REGS.CD);
            Assert.Equal(0, computer.CPU.REGS.EF);
            Assert.Equal(12, computer.CPU.REGS.GH);
            Assert.Equal(179, computer.CPU.REGS.IJ);
            Assert.Equal(4369, computer.CPU.REGS.KL);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 49151;
            computer.CPU.REGS.C = 18;
            computer.CPU.REGS.DE = 5000;
            computer.CPU.REGS.F = 6;
            computer.CPU.REGS.GH = 32900;
            computer.CPU.REGS.I = 255;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DIV AB, C",
                    "DIV DE, F",
                    "DIV GH, I",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(2730, computer.CPU.REGS.AB);
            Assert.Equal(18, computer.CPU.REGS.C);
            Assert.Equal(833, computer.CPU.REGS.DE);
            Assert.Equal(6, computer.CPU.REGS.F);
            Assert.Equal(129, computer.CPU.REGS.GH);
            Assert.Equal(255, computer.CPU.REGS.I);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 49151;
            computer.CPU.REGS.CD = 3500;
            computer.CPU.REGS.EF = 32900;
            computer.CPU.REGS.GH = 1024;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DIV AB, CD",
                    "DIV EF, GH",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(14, computer.CPU.REGS.AB);
            Assert.Equal(3500, computer.CPU.REGS.CD);
            Assert.Equal(32, computer.CPU.REGS.EF);
            Assert.Equal(1024, computer.CPU.REGS.GH);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 0x333333;
            computer.CPU.REGS.DEF = 0x777777;
            computer.CPU.REGS.GHI = 0xFFFFFF;
            computer.CPU.REGS.JKL = 0x101010;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DIV ABC, 3",
                    "DIV DEF, 4",
                    "DIV GHI, 0x01FFF",
                    "DIV JKL, 10",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x111111, (double)computer.CPU.REGS.ABC);
            Assert.Equal(0x1DDDDD, (double)computer.CPU.REGS.DEF);
            Assert.Equal(0x800, (double)computer.CPU.REGS.GHI);
            Assert.Equal(0x19B34, (double)computer.CPU.REGS.JKL);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 0xABCDEF;
            computer.CPU.REGS.D = 0xEF;
            computer.CPU.REGS.EFG = 0x123456;
            computer.CPU.REGS.H = 0xFF;
            computer.CPU.REGS.IJK = 0x00FFFF;
            computer.CPU.REGS.L = 0xFF;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DIV ABC, D",
                    "DIV EFG, H",
                    "DIV IJK, L",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xB806, (double)computer.CPU.REGS.ABC);
            Assert.Equal(0xEF, computer.CPU.REGS.D);
            Assert.Equal(0x1246, (double)computer.CPU.REGS.EFG);
            Assert.Equal(0xFF, computer.CPU.REGS.H);
            Assert.Equal(0x101, (double)computer.CPU.REGS.IJK);
            Assert.Equal(0xFF, computer.CPU.REGS.L);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 0xABCDEF;
            computer.CPU.REGS.DE = 0xCDEF;
            computer.CPU.REGS.FGH = 0x123456;
            computer.CPU.REGS.IJ = 0xFFFF;
            computer.CPU.REGS.KLM = 0x00FFFF;
            computer.CPU.REGS.NO = 0x10FF;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DIV ABC, DE",
                    "DIV FGH, IJ",
                    "DIV KLM, NO",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xD5, (double)computer.CPU.REGS.ABC);
            Assert.Equal(0xCDEF, computer.CPU.REGS.DE);
            Assert.Equal(0x12, (double)computer.CPU.REGS.FGH);
            Assert.Equal(0xFFFF, computer.CPU.REGS.IJ);
            Assert.Equal(0xF, (double)computer.CPU.REGS.KLM);
            Assert.Equal(0x10FF, computer.CPU.REGS.NO);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 0xABCDEF;
            computer.CPU.REGS.DEF = 0x01CDEF;
            computer.CPU.REGS.GHI = 0x123456;
            computer.CPU.REGS.JKL = 0xFFFF;
            computer.CPU.REGS.MNO = 0xF0FFFF;
            computer.CPU.REGS.PQR = 0x10FF;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DIV ABC, DEF",
                    "DIV GHI, JKL",
                    "DIV MNO, PQR",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x5F, (double)computer.CPU.REGS.ABC);
            Assert.Equal(0x1CDEF, (double)computer.CPU.REGS.DEF);
            Assert.Equal(0x12, (double)computer.CPU.REGS.GHI);
            Assert.Equal(0xFFFF, (double)computer.CPU.REGS.JKL);
            Assert.Equal(0xE2E, (double)computer.CPU.REGS.MNO);
            Assert.Equal(0x10FF, (double)computer.CPU.REGS.PQR);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrrr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0xFF00FF00;
            computer.CPU.REGS.EFGH = 0x01010101;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DIV ABCD, 0xFF",
                    "DIV EFGH, 0x10",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x1000100, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0x101010, (double)computer.CPU.REGS.EFGH);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrrr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0x89ABCDEF;
            computer.CPU.REGS.E = 0x56;
            computer.CPU.REGS.FGHI = 0xFFFFFFFF;
            computer.CPU.REGS.J = 0xFA;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DIV ABCD, E",
                    "DIV FGHI, J",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x199CFCA, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0x56, computer.CPU.REGS.E);
            Assert.Equal(0x10624DD, (double)computer.CPU.REGS.FGHI);
            Assert.Equal(0xFA, computer.CPU.REGS.J);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrrr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0x89ABCDEF;
            computer.CPU.REGS.EF = 0x56FA;
            computer.CPU.REGS.GHIJ = 0xFFFFFFFF;
            computer.CPU.REGS.KL = 0xFAEF;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DIV ABCD, EF",
                    "DIV GHIJ, KL",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x19535, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0x56FA, computer.CPU.REGS.EF);
            Assert.Equal(0x1052B, (double)computer.CPU.REGS.GHIJ);
            Assert.Equal(0xFAEF, computer.CPU.REGS.KL);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrrr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0x89ABCDEF;
            computer.CPU.REGS.EFG = 0xFF56FA;
            computer.CPU.REGS.HIJK = 0xFFFFFFFF;
            computer.CPU.REGS.LMN = 0x01FAEF;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DIV ABCD, EFG",
                    "DIV HIJK, LMN",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x8A, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0xFF56FA, (double)computer.CPU.REGS.EFG);
            Assert.Equal(0x8147, (double)computer.CPU.REGS.HIJK);
            Assert.Equal(0x1FAEF, (double)computer.CPU.REGS.LMN);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrrr_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0xD9ABCDEF;
            computer.CPU.REGS.EFGH = 0x1FF56FA;
            computer.CPU.REGS.IJKL = 0xFFFFFFFF;
            computer.CPU.REGS.MNOP = 0xDD1FAEF;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DIV ABCD, EFGH",
                    "DIV IJKL, MNOP",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x6C, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0x1FF56FA, (double)computer.CPU.REGS.EFGH);
            Assert.Equal(0x12, (double)computer.CPU.REGS.IJKL);
            Assert.Equal(0xDD1FAEF, (double)computer.CPU.REGS.MNOP);
            TUtils.IncrementCountedTests("exec");
        }

        // With remainder
        [Fact]
        public void TestEXEC_DIV_r_n_r()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.A = 9;
            computer.CPU.REGS.C = 18;
            computer.CPU.REGS.E = 29;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DIV A, 3, B",
                    "DIV C, 13, D",
                    "DIV E, 5, F",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(3, computer.CPU.REGS.A);
            Assert.Equal(0, computer.CPU.REGS.B);
            Assert.Equal(1, computer.CPU.REGS.C);
            Assert.Equal(5, computer.CPU.REGS.D);
            Assert.Equal(5, computer.CPU.REGS.E);
            Assert.Equal(4, computer.CPU.REGS.F);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_r_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.A = 200;
            computer.CPU.REGS.B = 17;
            computer.CPU.REGS.D = 2;
            computer.CPU.REGS.E = 200;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DIV A, B, C",
                    "DIV D, E, F",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(11, computer.CPU.REGS.A);
            Assert.Equal(17, computer.CPU.REGS.B);
            Assert.Equal(13, computer.CPU.REGS.C);
            Assert.Equal(0, computer.CPU.REGS.D);
            Assert.Equal(200, computer.CPU.REGS.E);
            Assert.Equal(2, computer.CPU.REGS.F);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rr_nn_rr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.AB = 0xFFFF;
            computer.CPU.REGS.EF = 0x1234;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DIV AB, 0xFFF, CD",
                    "DIV EF, 0xAA, GH",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x10, computer.CPU.REGS.AB);
            Assert.Equal(15, computer.CPU.REGS.CD);
            Assert.Equal(27, computer.CPU.REGS.EF);
            Assert.Equal(70, computer.CPU.REGS.GH);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rr_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.AB = 0xC000;
            computer.CPU.REGS.C = 7;
            computer.CPU.REGS.D = 0;
            computer.CPU.REGS.EF = 0xABCD;
            computer.CPU.REGS.G = 200;
            computer.CPU.REGS.H = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DIV AB, C, D",
                    "DIV EF, G, H",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x1B6D, computer.CPU.REGS.AB);
            Assert.Equal(7, computer.CPU.REGS.C);
            Assert.Equal(5, computer.CPU.REGS.D);
            Assert.Equal(0xDB, computer.CPU.REGS.EF);
            Assert.Equal(200, computer.CPU.REGS.G);
            Assert.Equal(0xB5, computer.CPU.REGS.H);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rr_rr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.AB = 0xFCFC;
            computer.CPU.REGS.CD = 0x400;
            computer.CPU.REGS.EF = 0;
            computer.CPU.REGS.GH = 0xABCD;
            computer.CPU.REGS.IJ = 0x8000;
            computer.CPU.REGS.KL = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DIV AB, CD, EF",
                    "DIV GH, IJ, KL",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x3F, computer.CPU.REGS.AB);
            Assert.Equal(0x400, computer.CPU.REGS.CD);
            Assert.Equal(0xFC, computer.CPU.REGS.EF);
            Assert.Equal(1, computer.CPU.REGS.GH);
            Assert.Equal(0x8000, computer.CPU.REGS.IJ);
            Assert.Equal(0x2BCD, computer.CPU.REGS.KL);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrr_nn_rr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABC = 0xFFFAFF;
            computer.CPU.REGS.GHI = 0x1234;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DIV ABC, 0x1FFF, DE",
                    "DIV GHI, 0xAA, JK",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x800, (double)computer.CPU.REGS.ABC);
            Assert.Equal(0x2FF, computer.CPU.REGS.DE);
            Assert.Equal(0x1B, (double)computer.CPU.REGS.GHI);
            Assert.Equal(0x46, computer.CPU.REGS.JK);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrr_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABC = 0xFAFAFF;
            computer.CPU.REGS.D = 13;
            computer.CPU.REGS.E = 0;
            computer.CPU.REGS.FGH = 0xABCDEF;
            computer.CPU.REGS.I = 200;
            computer.CPU.REGS.J = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DIV ABC, D, E",
                    "DIV FGH, I, J",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x134E62, (double)computer.CPU.REGS.ABC);
            Assert.Equal(13, computer.CPU.REGS.D);
            Assert.Equal(5, computer.CPU.REGS.E);
            Assert.Equal(0xDBE8, (double)computer.CPU.REGS.FGH);
            Assert.Equal(200, computer.CPU.REGS.I);
            Assert.Equal(0xAF, computer.CPU.REGS.J);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrr_rr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABC = 0xFAFAFF;
            computer.CPU.REGS.DE = 0xABCD;
            computer.CPU.REGS.FG = 0;
            computer.CPU.REGS.HIJ = 0xABCDEF;
            computer.CPU.REGS.KL = 0x1000;
            computer.CPU.REGS.MN = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DIV ABC, DE, FG",
                    "DIV HIJ, KL, MN",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x175, (double)computer.CPU.REGS.ABC);
            Assert.Equal(0xABCD, computer.CPU.REGS.DE);
            Assert.Equal(0xA94E, computer.CPU.REGS.FG);
            Assert.Equal(0xABC, (double)computer.CPU.REGS.HIJ);
            Assert.Equal(0x1000, computer.CPU.REGS.KL);
            Assert.Equal(0xDEF, computer.CPU.REGS.MN);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrr_rrr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABC = 0xFAFAFF;
            computer.CPU.REGS.DEF = 0xABCDF;
            computer.CPU.REGS.GHI = 0;
            computer.CPU.REGS.JKL = 0xABCDEF;
            computer.CPU.REGS.MNO = 0x10000;
            computer.CPU.REGS.PQR = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DIV ABC, DEF, GHI",
                    "DIV JKL, MNO, PQR",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x17, (double)computer.CPU.REGS.ABC);
            Assert.Equal(0xABCDF, (double)computer.CPU.REGS.DEF);
            Assert.Equal(0x402F6, (double)computer.CPU.REGS.GHI);
            Assert.Equal(0xAB, (double)computer.CPU.REGS.JKL);
            Assert.Equal(0x10000, (double)computer.CPU.REGS.MNO);
            Assert.Equal(0xCDEF, (double)computer.CPU.REGS.PQR);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrrr_nn_rr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABCD = 0xFAFAFBFB;
            computer.CPU.REGS.GHIJ = 0x12345678;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DIV ABCD, 0xFFFF, EF",
                    "DIV GHIJ, 0x1999, KL",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xFAFB, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0xF6F6, computer.CPU.REGS.EF);
            Assert.Equal(0xB60F, (double)computer.CPU.REGS.GHIJ);
            Assert.Equal(0x1081, computer.CPU.REGS.KL);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrrr_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABCD = 0xFAFAFFFF;
            computer.CPU.REGS.E = 13;
            computer.CPU.REGS.F = 0;
            computer.CPU.REGS.GHIJ = 0xABCDEF12;
            computer.CPU.REGS.K = 200;
            computer.CPU.REGS.L = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DIV ABCD, E, F",
                    "DIV GHIJ, K, L",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x134E6276, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(13, computer.CPU.REGS.E);
            Assert.Equal(1, computer.CPU.REGS.F);
            Assert.Equal(0xDBE8E0, (double)computer.CPU.REGS.GHIJ);
            Assert.Equal(200, computer.CPU.REGS.K);
            Assert.Equal(0x12, computer.CPU.REGS.L);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrrr_rr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABCD = 0xFAFAFFFF;
            computer.CPU.REGS.EF = 0x80FF;
            computer.CPU.REGS.GH = 0;
            computer.CPU.REGS.IJKL = 0xABCDEF12;
            computer.CPU.REGS.MN = 0x1000;
            computer.CPU.REGS.OP = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DIV ABCD, EF, GH",
                    "DIV IJKL, MN, OP",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x1F215, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0x80FF, computer.CPU.REGS.EF);
            Assert.Equal(0x5D14, computer.CPU.REGS.GH);
            Assert.Equal(0xABCDE, (double)computer.CPU.REGS.IJKL);
            Assert.Equal(0x1000, computer.CPU.REGS.MN);
            Assert.Equal(0xF12, computer.CPU.REGS.OP);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrrr_rrr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABCD = 0xFAFAFFFF;
            computer.CPU.REGS.EFG = 0x180FF;
            computer.CPU.REGS.HIJ = 0;
            computer.CPU.REGS.KLMN = 0xABCDEF12;
            computer.CPU.REGS.OPQ = 0x11000;
            computer.CPU.REGS.RST = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DIV ABCD, EFG, HIJ",
                    "DIV KLMN, OPQ, RST",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xA6E3, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0x180FF, (double)computer.CPU.REGS.EFG);
            Assert.Equal(0x43E2, (double)computer.CPU.REGS.HIJ);
            Assert.Equal(0xA1B2, (double)computer.CPU.REGS.KLMN);
            Assert.Equal(0x11000, (double)computer.CPU.REGS.OPQ);
            Assert.Equal(0xCF12, (double)computer.CPU.REGS.RST);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DIV_rrrr_rrrr_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.ABCD = 0xFAFAFFFF;
            computer.CPU.REGS.EFGH = 0xA1180FF;
            computer.CPU.REGS.IJKL = 0;
            computer.CPU.REGS.MNOP = 0xABCDEF12;
            computer.CPU.REGS.QRST = 0xD11000;
            computer.CPU.REGS.UVWX = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DIV ABCD, EFGH, IJKL",
                    "DIV MNOP, QRST, UVWX",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x18, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0xA1180FF, (double)computer.CPU.REGS.EFGH);
            Assert.Equal(0x956E817, (double)computer.CPU.REGS.IJKL);
            Assert.Equal(0xD2, (double)computer.CPU.REGS.MNOP);
            Assert.Equal(0xD11000, (double)computer.CPU.REGS.QRST);
            Assert.Equal(0x4ECF12, (double)computer.CPU.REGS.UVWX);
            TUtils.IncrementCountedTests("exec");
        }
    }
}
