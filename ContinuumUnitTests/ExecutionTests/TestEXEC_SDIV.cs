using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_SDIV
    {
        [Fact]
        public void TestEXEC_SDIV_r_n()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.A = TUtils.GetUnsignedByte(-9);
            computer.CPU.REGS.B = 7;
            computer.CPU.REGS.C = TUtils.GetUnsignedByte(-12);
            computer.CPU.REGS.D = TUtils.GetUnsignedByte(-120);
            computer.CPU.REGS.E = 25;
            computer.CPU.REGS.F = TUtils.GetUnsignedByte(-5);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SDIV A, 3",
                    "SDIV B, -4",
                    "SDIV C, -13",
                    "SDIV D, 10",
                    "SDIV E, -5",
                    "SDIV F, -1",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsignedByte(-3), computer.CPU.REGS.A);
            Assert.Equal(TUtils.GetUnsignedByte(-1), computer.CPU.REGS.B);
            Assert.Equal(0, computer.CPU.REGS.C);
            Assert.Equal(TUtils.GetUnsignedByte(-12), computer.CPU.REGS.D);
            Assert.Equal(TUtils.GetUnsignedByte(-5), computer.CPU.REGS.E);
            Assert.Equal(5, computer.CPU.REGS.F);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SDIV_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.A = TUtils.GetUnsignedByte(-26);
            computer.CPU.REGS.B = 2;
            computer.CPU.REGS.C = 12;
            computer.CPU.REGS.D = TUtils.GetUnsignedByte(-6);
            computer.CPU.REGS.E = TUtils.GetUnsignedByte(-6);
            computer.CPU.REGS.F = 6;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SDIV A, B",
                    "SDIV C, D",
                    "SDIV E, F",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsignedByte(-13), computer.CPU.REGS.A);
            Assert.Equal(2, computer.CPU.REGS.B);
            Assert.Equal(TUtils.GetUnsignedByte(-2), computer.CPU.REGS.C);
            Assert.Equal(TUtils.GetUnsignedByte(-6), computer.CPU.REGS.D);
            Assert.Equal(TUtils.GetUnsignedByte(-1), computer.CPU.REGS.E);
            Assert.Equal(6, computer.CPU.REGS.F);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SDIV_rr_n()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.AB = TUtils.GetUnsignedShort(-9);
            computer.CPU.REGS.CD = 7;
            computer.CPU.REGS.EF = 12;
            computer.CPU.REGS.GH = TUtils.GetUnsignedShort(-120);
            computer.CPU.REGS.IJ = 1255;
            computer.CPU.REGS.KL = TUtils.GetUnsignedShort(-32000);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SDIV AB, 3",
                    "SDIV CD, -4",
                    "SDIV EF, 13",
                    "SDIV GH, -10",
                    "SDIV IJ, 7",
                    "SDIV KL, 15",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsignedShort(-3), computer.CPU.REGS.AB);
            Assert.Equal(TUtils.GetUnsignedShort(-1), computer.CPU.REGS.CD);
            Assert.Equal(0, computer.CPU.REGS.EF);
            Assert.Equal(12, computer.CPU.REGS.GH);
            Assert.Equal(179, computer.CPU.REGS.IJ);
            Assert.Equal(TUtils.GetUnsignedShort(-2133), computer.CPU.REGS.KL);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SDIV_rr_r()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.AB = TUtils.GetUnsignedShort(-29000);
            computer.CPU.REGS.C = 18;
            computer.CPU.REGS.DE = 5000;
            computer.CPU.REGS.F = 6;
            computer.CPU.REGS.GH = 16200;
            computer.CPU.REGS.I = TUtils.GetUnsignedByte(-55);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SDIV AB, C",
                    "SDIV DE, F",
                    "SDIV GH, I",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsignedShort(-1611), computer.CPU.REGS.AB);
            Assert.Equal(18, computer.CPU.REGS.C);
            Assert.Equal(833, computer.CPU.REGS.DE);
            Assert.Equal(6, computer.CPU.REGS.F);
            Assert.Equal(TUtils.GetUnsignedShort(-294), computer.CPU.REGS.GH);
            Assert.Equal(TUtils.GetUnsignedByte(-55), computer.CPU.REGS.I);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SDIV_rr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.AB = TUtils.GetUnsignedShort(-32000);
            computer.CPU.REGS.CD = 320;
            computer.CPU.REGS.EF = TUtils.GetUnsignedShort(-32000);
            computer.CPU.REGS.GH = TUtils.GetUnsignedShort(-1000);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SDIV AB, CD",
                    "SDIV EF, GH",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsignedShort(-100), computer.CPU.REGS.AB);
            Assert.Equal(320, computer.CPU.REGS.CD);
            Assert.Equal(32, computer.CPU.REGS.EF);
            Assert.Equal(TUtils.GetUnsignedShort(-1000), computer.CPU.REGS.GH);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SDIV_rrr_n()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.ABC = TUtils.GetUnsigned24BitInt(-0x100000);
            computer.CPU.REGS.DEF = 0x777777;
            computer.CPU.REGS.GHI = 0x7FFFFF;
            computer.CPU.REGS.JKL = 0x101010;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SDIV ABC, 3",
                    "SDIV DEF, 4",
                    "SDIV GHI, -511",
                    "SDIV JKL, 10",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsigned24BitInt(-0x55555), (double)computer.CPU.REGS.ABC);
            Assert.Equal(0x1DDDDD, (double)computer.CPU.REGS.DEF);
            Assert.Equal(TUtils.GetUnsigned24BitInt(-16416), (double)computer.CPU.REGS.GHI);
            Assert.Equal(0x19B34, (double)computer.CPU.REGS.JKL);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SDIV_rrr_r()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.ABC = 0x7BCDEF;
            computer.CPU.REGS.D = TUtils.GetUnsignedByte(-100);
            computer.CPU.REGS.EFG = TUtils.GetUnsigned24BitInt(-1193046);
            computer.CPU.REGS.H = TUtils.GetUnsignedByte(-12);
            computer.CPU.REGS.IJK = 0x00FFFF;
            computer.CPU.REGS.L = TUtils.GetUnsignedByte(-1);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SDIV ABC, D",
                    "SDIV EFG, H",
                    "SDIV IJK, L",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsigned24BitInt(-81136), (double)computer.CPU.REGS.ABC);
            Assert.Equal(TUtils.GetUnsignedByte(-100), computer.CPU.REGS.D);
            Assert.Equal(99420, (double)computer.CPU.REGS.EFG);
            Assert.Equal(TUtils.GetUnsignedByte(-12), computer.CPU.REGS.H);
            Assert.Equal(TUtils.GetUnsigned24BitInt(-65535), (double)computer.CPU.REGS.IJK);
            Assert.Equal(TUtils.GetUnsignedByte(-1), computer.CPU.REGS.L);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SDIV_rrr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.ABC = TUtils.GetUnsigned24BitInt(-1193046);
            computer.CPU.REGS.DE = 30000;
            computer.CPU.REGS.FGH = 0x123456;
            computer.CPU.REGS.IJ = TUtils.GetUnsignedShort(-16000);
            computer.CPU.REGS.KLM = 0x00FFFF;
            computer.CPU.REGS.NO = 0x10FF;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SDIV ABC, DE",
                    "SDIV FGH, IJ",
                    "SDIV KLM, NO",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsigned24BitInt(-39), (double)computer.CPU.REGS.ABC);
            Assert.Equal(30000, computer.CPU.REGS.DE);
            Assert.Equal(TUtils.GetUnsigned24BitInt(-74), (double)computer.CPU.REGS.FGH);
            Assert.Equal(TUtils.GetUnsignedShort(-16000), computer.CPU.REGS.IJ);
            Assert.Equal(0xF, (double)computer.CPU.REGS.KLM);
            Assert.Equal(0x10FF, computer.CPU.REGS.NO);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SDIV_rrr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.ABC = TUtils.GetUnsigned24BitInt(-7065071);
            computer.CPU.REGS.DEF = 0x01CDEF;
            computer.CPU.REGS.GHI = 0x123456;
            computer.CPU.REGS.JKL = 0xFFFF;
            computer.CPU.REGS.MNO = 0x0FFFFF;
            computer.CPU.REGS.PQR = TUtils.GetUnsigned24BitInt(-4351);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SDIV ABC, DEF",
                    "SDIV GHI, JKL",
                    "SDIV MNO, PQR",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsigned24BitInt(-59), (double)computer.CPU.REGS.ABC);
            Assert.Equal(0x1CDEF, (double)computer.CPU.REGS.DEF);
            Assert.Equal(0x12, (double)computer.CPU.REGS.GHI);
            Assert.Equal(0xFFFF, (double)computer.CPU.REGS.JKL);
            Assert.Equal(TUtils.GetUnsigned24BitInt(-240), (double)computer.CPU.REGS.MNO);
            Assert.Equal(TUtils.GetUnsigned24BitInt(-4351), (double)computer.CPU.REGS.PQR);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SDIV_rrrr_n()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.ABCD = TUtils.GetUnsigned32BitInt(-1610612736);
            computer.CPU.REGS.EFGH = 0x01010101;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SDIV ABCD, 2500",
                    "SDIV EFGH, -10",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsigned32BitInt(-644245), (double)computer.CPU.REGS.ABCD);
            Assert.Equal(TUtils.GetUnsigned32BitInt(-1684300), (double)computer.CPU.REGS.EFGH);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SDIV_rrrr_r()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.ABCD = TUtils.GetUnsigned32BitInt(-2041302511);
            computer.CPU.REGS.E = 0x56;
            computer.CPU.REGS.FGHI = 0x3FFFFFFF;
            computer.CPU.REGS.J = TUtils.GetUnsignedByte(-30);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SDIV ABCD, E",
                    "SDIV FGHI, J",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsigned32BitInt(-23736075), (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0x56, computer.CPU.REGS.E);
            Assert.Equal(TUtils.GetUnsigned32BitInt(-35791394), (double)computer.CPU.REGS.FGHI);
            Assert.Equal(TUtils.GetUnsignedByte(-30), computer.CPU.REGS.J);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SDIV_rrrr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.ABCD = TUtils.GetUnsigned32BitInt(-2041302511);
            computer.CPU.REGS.EF = 0x56FA;
            computer.CPU.REGS.GHIJ = 0x1FFFFFFF;
            computer.CPU.REGS.KL = TUtils.GetUnsignedShort(-10000);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SDIV ABCD, EF",
                    "SDIV GHIJ, KL",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsigned32BitInt(-91678), (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0x56FA, computer.CPU.REGS.EF);
            Assert.Equal(TUtils.GetUnsigned32BitInt(-53687), (double)computer.CPU.REGS.GHIJ);
            Assert.Equal(TUtils.GetUnsignedShort(-10000), computer.CPU.REGS.KL);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SDIV_rrrr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.ABCD = TUtils.GetUnsigned32BitInt(-2041302511);
            computer.CPU.REGS.EFG = 0x7F56FA;
            computer.CPU.REGS.HIJK = 0x7FFFFFFF;
            computer.CPU.REGS.LMN = TUtils.GetUnsigned24BitInt(-129775);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SDIV ABCD, EFG",
                    "SDIV HIJK, LMN",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsigned32BitInt(-244), (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0x7F56FA, (double)computer.CPU.REGS.EFG);
            Assert.Equal(TUtils.GetUnsigned32BitInt(-16547), (double)computer.CPU.REGS.HIJK);
            Assert.Equal(TUtils.GetUnsigned24BitInt(-129775), (double)computer.CPU.REGS.LMN);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SDIV_rrrr_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.ABCD = TUtils.GetUnsigned32BitInt(-699125231);
            computer.CPU.REGS.EFGH = 0x1FF56FA;
            computer.CPU.REGS.IJKL = 0x7FFFFFFF;
            computer.CPU.REGS.MNOP = TUtils.GetUnsigned32BitInt(-231865071);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SDIV ABCD, EFGH",
                    "SDIV IJKL, MNOP",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsigned32BitInt(-20), (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0x1FF56FA, (double)computer.CPU.REGS.EFGH);
            Assert.Equal(TUtils.GetUnsigned32BitInt(-9), (double)computer.CPU.REGS.IJKL);
            Assert.Equal(TUtils.GetUnsigned32BitInt(-231865071), (double)computer.CPU.REGS.MNOP);
            TUtils.IncrementCountedTests("exec");
        }

        // With remainder
        [Fact]
        public void TestEXEC_SDIV_r_n_r()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.A = 9;
            computer.CPU.REGS.C = 18;
            computer.CPU.REGS.E = TUtils.GetUnsignedByte(-29);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SDIV A, -3, B",
                    "SDIV C, -13, D",
                    "SDIV E, -5, F",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsignedByte(-3), computer.CPU.REGS.A);
            Assert.Equal(0, computer.CPU.REGS.B);
            Assert.Equal(TUtils.GetUnsignedByte(-1), computer.CPU.REGS.C);
            Assert.Equal(5, computer.CPU.REGS.D);
            Assert.Equal(5, computer.CPU.REGS.E);
            Assert.Equal(TUtils.GetUnsignedByte(-4), computer.CPU.REGS.F);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SDIV_r_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.A = TUtils.GetUnsignedByte(-100);
            computer.CPU.REGS.B = 17;
            computer.CPU.REGS.D = TUtils.GetUnsignedByte(-2);
            computer.CPU.REGS.E = TUtils.GetUnsignedByte(-10);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SDIV A, B, C",
                    "SDIV D, E, F",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsignedByte(-5), computer.CPU.REGS.A);
            Assert.Equal(17, computer.CPU.REGS.B);
            Assert.Equal(TUtils.GetUnsignedByte(-15), computer.CPU.REGS.C);
            Assert.Equal(0, computer.CPU.REGS.D);
            Assert.Equal(TUtils.GetUnsignedByte(-10), computer.CPU.REGS.E);
            Assert.Equal(TUtils.GetUnsignedByte(-2), computer.CPU.REGS.F);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SDIV_rr_nn_rr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.AB = TUtils.GetUnsignedShort(-30000);
            computer.CPU.REGS.EF = TUtils.GetUnsignedShort(-20000);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SDIV AB, 512, CD",
                    "SDIV EF, -501, GH",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsignedShort(-58), computer.CPU.REGS.AB);
            Assert.Equal(TUtils.GetUnsignedShort(-304), computer.CPU.REGS.CD);
            Assert.Equal(39, computer.CPU.REGS.EF);
            Assert.Equal(TUtils.GetUnsignedShort(-461), computer.CPU.REGS.GH);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SDIV_rr_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.AB = TUtils.GetUnsignedShort(-28690);
            computer.CPU.REGS.C = 7;
            computer.CPU.REGS.D = 0;
            computer.CPU.REGS.EF = 30000;
            computer.CPU.REGS.G = TUtils.GetUnsignedByte(-101);
            computer.CPU.REGS.H = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SDIV AB, C, D",
                    "SDIV EF, G, H",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsignedShort(-4098), computer.CPU.REGS.AB);
            Assert.Equal(7, computer.CPU.REGS.C);
            Assert.Equal(TUtils.GetUnsignedByte(-4), computer.CPU.REGS.D);
            Assert.Equal(TUtils.GetUnsignedShort(-297), computer.CPU.REGS.EF);
            Assert.Equal(TUtils.GetUnsignedByte(-101), computer.CPU.REGS.G);
            Assert.Equal(3, computer.CPU.REGS.H);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SDIV_rr_rr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.AB = TUtils.GetUnsignedShort(-28690);
            computer.CPU.REGS.CD = 400;
            computer.CPU.REGS.EF = 0;
            computer.CPU.REGS.GH = 30000;
            computer.CPU.REGS.IJ = TUtils.GetUnsignedShort(-1900);
            computer.CPU.REGS.KL = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SDIV AB, CD, EF",
                    "SDIV GH, IJ, KL",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsignedShort(-71), computer.CPU.REGS.AB);
            Assert.Equal(400, computer.CPU.REGS.CD);
            Assert.Equal(TUtils.GetUnsignedShort(-290), computer.CPU.REGS.EF);
            Assert.Equal(TUtils.GetUnsignedShort(-15), computer.CPU.REGS.GH);
            Assert.Equal(TUtils.GetUnsignedShort(-1900), computer.CPU.REGS.IJ);
            Assert.Equal(1500, computer.CPU.REGS.KL);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SDIV_rrr_nn_rr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.ABC = TUtils.GetUnsigned24BitInt(-4193023);
            computer.CPU.REGS.GHI = 200029;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SDIV ABC, 4000, DE",
                    "SDIV GHI, -400, JK",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsigned24BitInt(-1048), (double)computer.CPU.REGS.ABC);
            Assert.Equal(TUtils.GetUnsignedShort(-1023), computer.CPU.REGS.DE);
            Assert.Equal(TUtils.GetUnsigned24BitInt(-500), (double)computer.CPU.REGS.GHI);
            Assert.Equal(29, computer.CPU.REGS.JK);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SDIV_rrr_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.ABC = TUtils.GetUnsigned24BitInt(-4193023);
            computer.CPU.REGS.D = 13;
            computer.CPU.REGS.E = 0;
            computer.CPU.REGS.FGH = 8113647;
            computer.CPU.REGS.I = TUtils.GetUnsignedByte(-101);
            computer.CPU.REGS.J = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SDIV ABC, D, E",
                    "SDIV FGH, I, J",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsigned24BitInt(-322540), (double)computer.CPU.REGS.ABC);
            Assert.Equal(13, computer.CPU.REGS.D);
            Assert.Equal(TUtils.GetUnsignedByte(-3), computer.CPU.REGS.E);
            Assert.Equal(TUtils.GetUnsigned24BitInt(-80333), (double)computer.CPU.REGS.FGH);
            Assert.Equal(TUtils.GetUnsignedByte(-101), computer.CPU.REGS.I);
            Assert.Equal(14, computer.CPU.REGS.J);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SDIV_rrr_rr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.ABC = TUtils.GetUnsigned24BitInt(-4193023);
            computer.CPU.REGS.DE = 30000;
            computer.CPU.REGS.FG = 0;
            computer.CPU.REGS.HIJ = TUtils.GetUnsigned24BitInt(4193023);
            computer.CPU.REGS.KL = TUtils.GetUnsignedShort(-30000);
            computer.CPU.REGS.MN = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SDIV ABC, DE, FG",
                    "SDIV HIJ, KL, MN",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsigned24BitInt(-139), (double)computer.CPU.REGS.ABC);
            Assert.Equal(30000, computer.CPU.REGS.DE);
            Assert.Equal(TUtils.GetUnsignedShort(-23023), computer.CPU.REGS.FG);
            Assert.Equal(TUtils.GetUnsigned24BitInt(-139), (double)computer.CPU.REGS.HIJ);
            Assert.Equal(TUtils.GetUnsignedShort(-30000), computer.CPU.REGS.KL);
            Assert.Equal(23023, computer.CPU.REGS.MN);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SDIV_rrr_rrr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.ABC = TUtils.GetUnsigned24BitInt(-4193023);
            computer.CPU.REGS.DEF = 703711;
            computer.CPU.REGS.GHI = 0;
            computer.CPU.REGS.JKL = 4193023;
            computer.CPU.REGS.MNO = TUtils.GetUnsigned24BitInt(-703711);
            computer.CPU.REGS.PQR = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SDIV ABC, DEF, GHI",
                    "SDIV JKL, MNO, PQR",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsigned24BitInt(-5), (double)computer.CPU.REGS.ABC);
            Assert.Equal(703711, (double)computer.CPU.REGS.DEF);
            Assert.Equal(TUtils.GetUnsigned24BitInt(-674468), (double)computer.CPU.REGS.GHI);
            Assert.Equal(TUtils.GetUnsigned24BitInt(-5), (double)computer.CPU.REGS.JKL);
            Assert.Equal(TUtils.GetUnsigned24BitInt(-703711), (double)computer.CPU.REGS.MNO);
            Assert.Equal(674468, (double)computer.CPU.REGS.PQR);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SDIV_rrrr_nn_rr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.ABCD = TUtils.GetUnsigned32BitInt(-2063268859);
            computer.CPU.REGS.GHIJ = 2063268859;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SDIV ABCD, 30000, EF",
                    "SDIV GHIJ, -30000, KL",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsigned32BitInt(-68775), (double)computer.CPU.REGS.ABCD);
            Assert.Equal(TUtils.GetUnsignedShort(-18859), computer.CPU.REGS.EF);
            Assert.Equal(TUtils.GetUnsigned32BitInt(-68775), (double)computer.CPU.REGS.GHIJ);
            Assert.Equal(18859, computer.CPU.REGS.KL);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SDIV_rrrr_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.ABCD = TUtils.GetUnsigned32BitInt(-2063268859);
            computer.CPU.REGS.E = 80;
            computer.CPU.REGS.F = 0;
            computer.CPU.REGS.GHIJ = 2063268859;
            computer.CPU.REGS.K = TUtils.GetUnsignedByte(-80);
            computer.CPU.REGS.L = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SDIV ABCD, E, F",
                    "SDIV GHIJ, K, L",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsigned32BitInt(-25790860), (double)computer.CPU.REGS.ABCD);
            Assert.Equal(80, computer.CPU.REGS.E);
            Assert.Equal(TUtils.GetUnsignedByte(-59), computer.CPU.REGS.F);
            Assert.Equal(TUtils.GetUnsigned32BitInt(-25790860), (double)computer.CPU.REGS.GHIJ);
            Assert.Equal(TUtils.GetUnsignedByte(-80), computer.CPU.REGS.K);
            Assert.Equal(59, computer.CPU.REGS.L);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SDIV_rrrr_rr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.ABCD = TUtils.GetUnsigned32BitInt(-25790860);
            computer.CPU.REGS.EF = 30000;
            computer.CPU.REGS.GH = 0;
            computer.CPU.REGS.IJKL = 25790860;
            computer.CPU.REGS.MN = TUtils.GetUnsignedShort(-30000);
            computer.CPU.REGS.OP = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SDIV ABCD, EF, GH",
                    "SDIV IJKL, MN, OP",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsigned32BitInt(-859), (double)computer.CPU.REGS.ABCD);
            Assert.Equal(30000, computer.CPU.REGS.EF);
            Assert.Equal(TUtils.GetUnsignedShort(-20860), computer.CPU.REGS.GH);
            Assert.Equal(TUtils.GetUnsigned32BitInt(-859), (double)computer.CPU.REGS.IJKL);
            Assert.Equal(TUtils.GetUnsignedShort(-30000), computer.CPU.REGS.MN);
            Assert.Equal(20860, computer.CPU.REGS.OP);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SDIV_rrrr_rrr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.ABCD = TUtils.GetUnsigned32BitInt(-25790860);
            computer.CPU.REGS.EFG = 600000;
            computer.CPU.REGS.HIJ = 0;
            computer.CPU.REGS.KLMN = 25790860;
            computer.CPU.REGS.OPQ = TUtils.GetUnsigned24BitInt(-600000);
            computer.CPU.REGS.RST = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SDIV ABCD, EFG, HIJ",
                    "SDIV KLMN, OPQ, RST",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsigned32BitInt(-42), (double)computer.CPU.REGS.ABCD);
            Assert.Equal(600000, (double)computer.CPU.REGS.EFG);
            Assert.Equal(TUtils.GetUnsigned24BitInt(-590860), (double)computer.CPU.REGS.HIJ);
            Assert.Equal(TUtils.GetUnsigned32BitInt(-42), (double)computer.CPU.REGS.KLMN);
            Assert.Equal(TUtils.GetUnsigned24BitInt(-600000), (double)computer.CPU.REGS.OPQ);
            Assert.Equal(590860, (double)computer.CPU.REGS.RST);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SDIV_rrrr_rrrr_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            computer.CPU.REGS.ABCD = TUtils.GetUnsigned32BitInt(-1890307567);
            computer.CPU.REGS.EFGH = 19079697;
            computer.CPU.REGS.IJKL = 0;
            computer.CPU.REGS.MNOP = 1890307567;
            computer.CPU.REGS.QRST = TUtils.GetUnsigned32BitInt(-19079697);
            computer.CPU.REGS.UVWX = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SDIV ABCD, EFGH, IJKL",
                    "SDIV MNOP, QRST, UVWX",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(TUtils.GetUnsigned32BitInt(-99), (double)computer.CPU.REGS.ABCD);
            Assert.Equal(19079697, (double)computer.CPU.REGS.EFGH);
            Assert.Equal(TUtils.GetUnsigned32BitInt(-1417564), (double)computer.CPU.REGS.IJKL);
            Assert.Equal(TUtils.GetUnsigned32BitInt(-99), (double)computer.CPU.REGS.MNOP);
            Assert.Equal(TUtils.GetUnsigned32BitInt(-19079697), (double)computer.CPU.REGS.QRST);
            Assert.Equal(1417564, (double)computer.CPU.REGS.UVWX);
            TUtils.IncrementCountedTests("exec");
        }
    }
}
