using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_RL
    {
        [Fact]
        public void TestEXEC_RL_r_n()
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
                    "RL A, 3",
                    "RL B, 7",
                    "RL C, 8",
                    "RL D, 4",
                    "RL E, 0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00000101, computer.CPU.REGS.A);
            Assert.Equal(0b01000000, computer.CPU.REGS.B);
            Assert.Equal(0b10000000, computer.CPU.REGS.C);
            Assert.Equal(0b11111111, computer.CPU.REGS.D);
            Assert.Equal(0b10101010, computer.CPU.REGS.E);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RL_r_r()
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
                    "RL A, F",
                    "RL B, G",
                    "RL C, H",
                    "RL D, I",
                    "RL E, J",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00000101, computer.CPU.REGS.A);
            Assert.Equal(0b01000000, computer.CPU.REGS.B);
            Assert.Equal(0b10000000, computer.CPU.REGS.C);
            Assert.Equal(0b11111111, computer.CPU.REGS.D);
            Assert.Equal(0b10101010, computer.CPU.REGS.E);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RL_rr_n()
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
                    "RL AB, 6",
                    "RL CD, 15",
                    "RL EF, 16",
                    "RL GH, 8",
                    "RL IJ, 0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b0000000000110000, computer.CPU.REGS.AB);
            Assert.Equal(0b0100000000000000, computer.CPU.REGS.CD);
            Assert.Equal(0b1000000000000000, computer.CPU.REGS.EF);
            Assert.Equal(0b1111111111111111, computer.CPU.REGS.GH);
            Assert.Equal(0b1010101010101010, computer.CPU.REGS.IJ);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RL_rr_r()
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
                    "RL AB, K",
                    "RL CD, L",
                    "RL EF, M",
                    "RL GH, N",
                    "RL IJ, O",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b0000000000110000, computer.CPU.REGS.AB);
            Assert.Equal(0b0100000000000000, computer.CPU.REGS.CD);
            Assert.Equal(0b1000000000000000, computer.CPU.REGS.EF);
            Assert.Equal(0b1111111111111111, computer.CPU.REGS.GH);
            Assert.Equal(0b1010101010101010, computer.CPU.REGS.IJ);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RL_rrr_n()
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
                    "RL ABC, 9",
                    "RL DEF, 23",
                    "RL GHI, 24",
                    "RL JKL, 12",
                    "RL MNO, 0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b000000000000000110000000, (double)computer.CPU.REGS.ABC);
            Assert.Equal(0b010000000000000000000000, (double)computer.CPU.REGS.DEF);
            Assert.Equal(0b100000000000000000000000, (double)computer.CPU.REGS.GHI);
            Assert.Equal(0b111111111111111111111111, (double)computer.CPU.REGS.JKL);
            Assert.Equal(0b101010101010101010101010, (double)computer.CPU.REGS.MNO);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RL_rrr_r()
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
                    "RL ABC, P",
                    "RL DEF, Q",
                    "RL GHI, R",
                    "RL JKL, S",
                    "RL MNO, T",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b000000000000000110000000, (double)computer.CPU.REGS.ABC);
            Assert.Equal(0b010000000000000000000000, (double)computer.CPU.REGS.DEF);
            Assert.Equal(0b100000000000000000000000, (double)computer.CPU.REGS.GHI);
            Assert.Equal(0b111111111111111111111111, (double)computer.CPU.REGS.JKL);
            Assert.Equal(0b101010101010101010101010, (double)computer.CPU.REGS.MNO);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RL_rrrr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0b11000000000000000000000000000000;
            computer.CPU.REGS.EFGH = 0b10000000000000000000000000000000;
            computer.CPU.REGS.IJKL = 0b11111111111111111111111111111111;
            computer.CPU.REGS.MNOP = 0b10101010101010101010101010101010;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "RL ABCD, 12",
                    "RL EFGH, 31",
                    "RL IJKL, 16",
                    "RL MNOP, 0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00000000000000000000110000000000, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0b01000000000000000000000000000000, (double)computer.CPU.REGS.EFGH);
            Assert.Equal(0b11111111111111111111111111111111, (double)computer.CPU.REGS.IJKL);
            Assert.Equal(0b10101010101010101010101010101010, (double)computer.CPU.REGS.MNOP);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RL_rrrr_r()
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
                    "RL ABCD, Q",
                    "RL EFGH, R",
                    "RL IJKL, S",
                    "RL MNOP, T",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00000000000000000000110000000000, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0b01000000000000000000000000000000, (double)computer.CPU.REGS.EFGH);
            Assert.Equal(0b11111111111111111111111111111111, (double)computer.CPU.REGS.IJKL);
            Assert.Equal(0b10101010101010101010101010101010, (double)computer.CPU.REGS.MNOP);
            TUtils.IncrementCountedTests("exec");
        }
    }
}
