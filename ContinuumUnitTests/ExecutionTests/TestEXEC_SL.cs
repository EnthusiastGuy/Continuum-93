using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_SL
    {
        [Fact]
        public void TestEXEC_SL_r_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 9;
            computer.CPU.REGS.B = 1;
            computer.CPU.REGS.C = 1;
            computer.CPU.REGS.D = 255;
            computer.CPU.REGS.E = 0b10101010;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SL A, 3",
                    "SL B, 7",
                    "SL C, 8",
                    "SL D, 4",
                    "SL E, 0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(9 * 2 * 2 * 2, computer.CPU.REGS.A);
            Assert.Equal(128, computer.CPU.REGS.B);
            Assert.Equal(0, computer.CPU.REGS.C);
            Assert.Equal(240, computer.CPU.REGS.D);
            Assert.Equal(0b10101010, computer.CPU.REGS.E);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SL_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 9;
            computer.CPU.REGS.B = 1;
            computer.CPU.REGS.C = 1;
            computer.CPU.REGS.D = 255;
            computer.CPU.REGS.E = 0b10101010;

            computer.CPU.REGS.F = 3;
            computer.CPU.REGS.G = 7;
            computer.CPU.REGS.H = 8;
            computer.CPU.REGS.I = 4;
            computer.CPU.REGS.J = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SL A, F",
                    "SL B, G",
                    "SL C, H",
                    "SL D, I",
                    "SL E, J",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(9 * 2 * 2 * 2, computer.CPU.REGS.A);
            Assert.Equal(128, computer.CPU.REGS.B);
            Assert.Equal(0, computer.CPU.REGS.C);
            Assert.Equal(240, computer.CPU.REGS.D);
            Assert.Equal(0b10101010, computer.CPU.REGS.E);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SL_rr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 0b0000000000001100;
            computer.CPU.REGS.CD = 1;
            computer.CPU.REGS.EF = 1;
            computer.CPU.REGS.GH = 0b1111111111111111;
            computer.CPU.REGS.IJ = 0b1010101010101010;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SL AB, 6",
                    "SL CD, 15",
                    "SL EF, 16",
                    "SL GH, 8",
                    "SL IJ, 0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b0000001100000000, computer.CPU.REGS.AB);
            Assert.Equal(0b1000000000000000, computer.CPU.REGS.CD);
            Assert.Equal(0, computer.CPU.REGS.EF);
            Assert.Equal(0b1111111100000000, computer.CPU.REGS.GH);
            Assert.Equal(0b1010101010101010, computer.CPU.REGS.IJ);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SL_rr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 0b0000000000001100;
            computer.CPU.REGS.CD = 1;
            computer.CPU.REGS.EF = 1;
            computer.CPU.REGS.GH = 0b1111111111111111;
            computer.CPU.REGS.IJ = 0b1010101010101010;

            computer.CPU.REGS.K = 6;
            computer.CPU.REGS.L = 15;
            computer.CPU.REGS.M = 16;
            computer.CPU.REGS.N = 8;
            computer.CPU.REGS.O = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SL AB, K",
                    "SL CD, L",
                    "SL EF, M",
                    "SL GH, N",
                    "SL IJ, O",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b0000001100000000, computer.CPU.REGS.AB);
            Assert.Equal(0b1000000000000000, computer.CPU.REGS.CD);
            Assert.Equal(0, computer.CPU.REGS.EF);
            Assert.Equal(0b1111111100000000, computer.CPU.REGS.GH);
            Assert.Equal(0b1010101010101010, computer.CPU.REGS.IJ);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SL_rrr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 0b000000000000000000001100;
            computer.CPU.REGS.DEF = 1;
            computer.CPU.REGS.GHI = 1;
            computer.CPU.REGS.JKL = 0b111111111111111111111111;
            computer.CPU.REGS.MNO = 0b101010101010101010101010;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SL ABC, 9",
                    "SL DEF, 23",
                    "SL GHI, 24",
                    "SL JKL, 12",
                    "SL MNO, 0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b000000000001100000000000, (double)computer.CPU.REGS.ABC);
            Assert.Equal(0b100000000000000000000000, (double)computer.CPU.REGS.DEF);
            Assert.Equal(0, (double)computer.CPU.REGS.GHI);
            Assert.Equal(0b111111111111000000000000, (double)computer.CPU.REGS.JKL);
            Assert.Equal(0b101010101010101010101010, (double)computer.CPU.REGS.MNO);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SL_rrr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 0b000000000000000000001100;
            computer.CPU.REGS.DEF = 1;
            computer.CPU.REGS.GHI = 1;
            computer.CPU.REGS.JKL = 0b111111111111111111111111;
            computer.CPU.REGS.MNO = 0b101010101010101010101010;

            computer.CPU.REGS.P = 9;
            computer.CPU.REGS.Q = 23;
            computer.CPU.REGS.R = 24;
            computer.CPU.REGS.S = 12;
            computer.CPU.REGS.T = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SL ABC, P",
                    "SL DEF, Q",
                    "SL GHI, R",
                    "SL JKL, S",
                    "SL MNO, T",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b000000000001100000000000, (double)computer.CPU.REGS.ABC);
            Assert.Equal(0b100000000000000000000000, (double)computer.CPU.REGS.DEF);
            Assert.Equal(0, (double)computer.CPU.REGS.GHI);
            Assert.Equal(0b111111111111000000000000, (double)computer.CPU.REGS.JKL);
            Assert.Equal(0b101010101010101010101010, (double)computer.CPU.REGS.MNO);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SL_rrrr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0b00000000000000000000000000001100;
            computer.CPU.REGS.EFGH = 1;
            computer.CPU.REGS.IJKL = 0b11111111111111111111111111111111;
            computer.CPU.REGS.MNOP = 0b10101010101010101010101010101010;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SL ABCD, 12",
                    "SL EFGH, 31",
                    "SL IJKL, 16",
                    "SL MNOP, 0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00000000000000001100000000000000, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0b10000000000000000000000000000000, (double)computer.CPU.REGS.EFGH);
            Assert.Equal(0b11111111111111110000000000000000, (double)computer.CPU.REGS.IJKL);
            Assert.Equal(0b10101010101010101010101010101010, (double)computer.CPU.REGS.MNOP);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SL_rrrr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0b00000000000000000000000000001100;
            computer.CPU.REGS.EFGH = 1;
            computer.CPU.REGS.IJKL = 0b11111111111111111111111111111111;
            computer.CPU.REGS.MNOP = 0b10101010101010101010101010101010;

            computer.CPU.REGS.Q = 12;
            computer.CPU.REGS.R = 31;
            computer.CPU.REGS.S = 16;
            computer.CPU.REGS.T = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SL ABCD, Q",
                    "SL EFGH, R",
                    "SL IJKL, S",
                    "SL MNOP, T",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00000000000000001100000000000000, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0b10000000000000000000000000000000, (double)computer.CPU.REGS.EFGH);
            Assert.Equal(0b11111111111111110000000000000000, (double)computer.CPU.REGS.IJKL);
            Assert.Equal(0b10101010101010101010101010101010, (double)computer.CPU.REGS.MNOP);
            TUtils.IncrementCountedTests("exec");
        }
    }
}
