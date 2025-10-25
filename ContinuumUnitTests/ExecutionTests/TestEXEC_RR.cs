using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_RR
    {
        [Fact]
        public void TestEXEC_RR_r_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 0b10100000;
            computer.CPU.REGS.B = 0b10000000;
            computer.CPU.REGS.C = 0b10000000;
            computer.CPU.REGS.D = 0b11111111;
            computer.CPU.REGS.E = 0b10101010;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "RR A, 3",
                    "RR B, 7",
                    "RR C, 8",
                    "RR D, 4",
                    "RR E, 0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00010100, computer.CPU.REGS.A);
            Assert.Equal(0b00000001, computer.CPU.REGS.B);
            Assert.Equal(0b10000000, computer.CPU.REGS.C);
            Assert.Equal(0b11111111, computer.CPU.REGS.D);
            Assert.Equal(0b10101010, computer.CPU.REGS.E);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RR_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 0b10100000;
            computer.CPU.REGS.B = 0b10000000;
            computer.CPU.REGS.C = 0b10000000;
            computer.CPU.REGS.D = 0b11111111;
            computer.CPU.REGS.E = 0b10101010;

            computer.CPU.REGS.F = 3;
            computer.CPU.REGS.G = 7;
            computer.CPU.REGS.H = 8;
            computer.CPU.REGS.I = 4;
            computer.CPU.REGS.J = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "RR A, F",
                    "RR B, G",
                    "RR C, H",
                    "RR D, I",
                    "RR E, J",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00010100, computer.CPU.REGS.A);
            Assert.Equal(0b00000001, computer.CPU.REGS.B);
            Assert.Equal(0b10000000, computer.CPU.REGS.C);
            Assert.Equal(0b11111111, computer.CPU.REGS.D);
            Assert.Equal(0b10101010, computer.CPU.REGS.E);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RR_rr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 0b1100000000000000;
            computer.CPU.REGS.CD = 0b1000000000000000;
            computer.CPU.REGS.EF = 0b1000000000000000;
            computer.CPU.REGS.GH = 0b1111111111111111;
            computer.CPU.REGS.IJ = 0b1010101010101010;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "RR AB, 6",
                    "RR CD, 15",
                    "RR EF, 16",
                    "RR GH, 8",
                    "RR IJ, 0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b0000001100000000, computer.CPU.REGS.AB);
            Assert.Equal(0b0000000000000001, computer.CPU.REGS.CD);
            Assert.Equal(0b1000000000000000, computer.CPU.REGS.EF);
            Assert.Equal(0b1111111111111111, computer.CPU.REGS.GH);
            Assert.Equal(0b1010101010101010, computer.CPU.REGS.IJ);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RR_rr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 0b1100000000000000;
            computer.CPU.REGS.CD = 0b1000000000000000;
            computer.CPU.REGS.EF = 0b1000000000000000;
            computer.CPU.REGS.GH = 0b1111111111111111;
            computer.CPU.REGS.IJ = 0b1010101010101010;

            computer.CPU.REGS.K = 6;
            computer.CPU.REGS.L = 15;
            computer.CPU.REGS.M = 16;
            computer.CPU.REGS.N = 8;
            computer.CPU.REGS.O = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "RR AB, K",
                    "RR CD, L",
                    "RR EF, M",
                    "RR GH, N",
                    "RR IJ, O",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b0000001100000000, computer.CPU.REGS.AB);
            Assert.Equal(0b0000000000000001, computer.CPU.REGS.CD);
            Assert.Equal(0b1000000000000000, computer.CPU.REGS.EF);
            Assert.Equal(0b1111111111111111, computer.CPU.REGS.GH);
            Assert.Equal(0b1010101010101010, computer.CPU.REGS.IJ);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RR_rrr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 0b110000000000000000000000;
            computer.CPU.REGS.DEF = 0b100000000000000000000000;
            computer.CPU.REGS.GHI = 0b100000000000000000000000;
            computer.CPU.REGS.JKL = 0b111111111111111111111111;
            computer.CPU.REGS.MNO = 0b101010101010101010101010;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "RR ABC, 9",
                    "RR DEF, 23",
                    "RR GHI, 24",
                    "RR JKL, 12",
                    "RR MNO, 0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b000000000110000000000000, (double)computer.CPU.REGS.ABC);
            Assert.Equal(0b000000000000000000000001, (double)computer.CPU.REGS.DEF);
            Assert.Equal(0b100000000000000000000000, (double)computer.CPU.REGS.GHI);
            Assert.Equal(0b111111111111111111111111, (double)computer.CPU.REGS.JKL);
            Assert.Equal(0b101010101010101010101010, (double)computer.CPU.REGS.MNO);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RR_rrr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 0b110000000000000000000000;
            computer.CPU.REGS.DEF = 0b100000000000000000000000;
            computer.CPU.REGS.GHI = 0b100000000000000000000000;
            computer.CPU.REGS.JKL = 0b111111111111111111111111;
            computer.CPU.REGS.MNO = 0b101010101010101010101010;

            computer.CPU.REGS.P = 9;
            computer.CPU.REGS.Q = 23;
            computer.CPU.REGS.R = 24;
            computer.CPU.REGS.S = 12;
            computer.CPU.REGS.T = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "RR ABC, P",
                    "RR DEF, Q",
                    "RR GHI, R",
                    "RR JKL, S",
                    "RR MNO, T",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b000000000110000000000000, (double)computer.CPU.REGS.ABC);
            Assert.Equal(0b000000000000000000000001, (double)computer.CPU.REGS.DEF);
            Assert.Equal(0b100000000000000000000000, (double)computer.CPU.REGS.GHI);
            Assert.Equal(0b111111111111111111111111, (double)computer.CPU.REGS.JKL);
            Assert.Equal(0b101010101010101010101010, (double)computer.CPU.REGS.MNO);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RR_rrrr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0b11000000000000000000000000000000;
            computer.CPU.REGS.EFGH = 0b10000000000000000000000000000000;
            computer.CPU.REGS.IJKL = 0b11111111111111111111111111111111;
            computer.CPU.REGS.MNOP = 0b10101010101010101010101010101010;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "RR ABCD, 12",
                    "RR EFGH, 31",
                    "RR IJKL, 16",
                    "RR MNOP, 0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00000000000011000000000000000000, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0b00000000000000000000000000000001, (double)computer.CPU.REGS.EFGH);
            Assert.Equal(0b11111111111111111111111111111111, (double)computer.CPU.REGS.IJKL);
            Assert.Equal(0b10101010101010101010101010101010, (double)computer.CPU.REGS.MNOP);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RR_rrrr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0b11000000000000000000000000000000;
            computer.CPU.REGS.EFGH = 0b10000000000000000000000000000000;
            computer.CPU.REGS.IJKL = 0b11111111111111111111111111111111;
            computer.CPU.REGS.MNOP = 0b10101010101010101010101010101010;

            computer.CPU.REGS.Q = 12;
            computer.CPU.REGS.R = 31;
            computer.CPU.REGS.S = 16;
            computer.CPU.REGS.T = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "RR ABCD, Q",
                    "RR EFGH, R",
                    "RR IJKL, S",
                    "RR MNOP, T",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00000000000011000000000000000000, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0b00000000000000000000000000000001, (double)computer.CPU.REGS.EFGH);
            Assert.Equal(0b11111111111111111111111111111111, (double)computer.CPU.REGS.IJKL);
            Assert.Equal(0b10101010101010101010101010101010, (double)computer.CPU.REGS.MNOP);
            TUtils.IncrementCountedTests("exec");
        }
    }
}
