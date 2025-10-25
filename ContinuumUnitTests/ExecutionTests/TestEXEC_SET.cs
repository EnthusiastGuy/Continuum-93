using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_SET
    {
        [Fact]
        public void TestEXEC_SET_r_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 0b00100000;
            computer.CPU.REGS.B = 0b00000000;
            computer.CPU.REGS.C = 0b00000000;
            computer.CPU.REGS.D = 0b00000000;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SET A, 0",
                    "SET B, 5",
                    "SET C, 7",
                    "SET D, 8",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00100001, computer.CPU.REGS.A);
            Assert.Equal(0b00100000, computer.CPU.REGS.B);
            Assert.Equal(0b10000000, computer.CPU.REGS.C);
            Assert.Equal(0b00000000, computer.CPU.REGS.D);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SET_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 0b00100000;
            computer.CPU.REGS.B = 0b00000000;
            computer.CPU.REGS.C = 0b00000000;
            computer.CPU.REGS.D = 0b00000000;

            computer.CPU.REGS.F = 0;
            computer.CPU.REGS.G = 5;
            computer.CPU.REGS.H = 7;
            computer.CPU.REGS.I = 8;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SET A, F",
                    "SET B, G",
                    "SET C, H",
                    "SET D, I",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00100001, computer.CPU.REGS.A);
            Assert.Equal(0b00100000, computer.CPU.REGS.B);
            Assert.Equal(0b10000000, computer.CPU.REGS.C);
            Assert.Equal(0b00000000, computer.CPU.REGS.D);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SET_rr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 0b0000000000000000;
            computer.CPU.REGS.CD = 0b0000000000000000;
            computer.CPU.REGS.EF = 0b0000000000000000;
            computer.CPU.REGS.GH = 0b0000000000000000;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SET AB, 0",
                    "SET CD, 10",
                    "SET EF, 15",
                    "SET GH, 16",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b0000000000000001, computer.CPU.REGS.AB);
            Assert.Equal(0b0000010000000000, computer.CPU.REGS.CD);
            Assert.Equal(0b1000000000000000, computer.CPU.REGS.EF);
            Assert.Equal(0b0000000000000000, computer.CPU.REGS.GH);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SET_rr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 0b0000000000000000;
            computer.CPU.REGS.CD = 0b0000000000000000;
            computer.CPU.REGS.EF = 0b0000000000000000;
            computer.CPU.REGS.GH = 0b0000000000000000;

            computer.CPU.REGS.K = 0;
            computer.CPU.REGS.L = 10;
            computer.CPU.REGS.M = 15;
            computer.CPU.REGS.N = 16;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SET AB, K",
                    "SET CD, L",
                    "SET EF, M",
                    "SET GH, N",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b0000000000000001, computer.CPU.REGS.AB);
            Assert.Equal(0b0000010000000000, computer.CPU.REGS.CD);
            Assert.Equal(0b1000000000000000, computer.CPU.REGS.EF);
            Assert.Equal(0b0000000000000000, computer.CPU.REGS.GH);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SET_rrr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 0b000000000000000000000000;
            computer.CPU.REGS.DEF = 0b000000000000000000000000;
            computer.CPU.REGS.GHI = 0b000000000000000000000000;
            computer.CPU.REGS.JKL = 0b000000000000000000000000;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SET ABC, 0",
                    "SET DEF, 15",
                    "SET GHI, 23",
                    "SET JKL, 24",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b000000000000000000000001, (double)computer.CPU.REGS.ABC);
            Assert.Equal(0b000000001000000000000000, (double)computer.CPU.REGS.DEF);
            Assert.Equal(0b100000000000000000000000, (double)computer.CPU.REGS.GHI);
            Assert.Equal(0b000000000000000000000000, (double)computer.CPU.REGS.JKL);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SET_rrr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 0b000000000000000000000000;
            computer.CPU.REGS.DEF = 0b000000000000000000000000;
            computer.CPU.REGS.GHI = 0b000000000000000000000000;
            computer.CPU.REGS.JKL = 0b000000000000000000000000;

            computer.CPU.REGS.P = 0;
            computer.CPU.REGS.Q = 15;
            computer.CPU.REGS.R = 23;
            computer.CPU.REGS.S = 24;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SET ABC, P",
                    "SET DEF, Q",
                    "SET GHI, R",
                    "SET JKL, S",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b000000000000000000000001, (double)computer.CPU.REGS.ABC);
            Assert.Equal(0b000000001000000000000000, (double)computer.CPU.REGS.DEF);
            Assert.Equal(0b100000000000000000000000, (double)computer.CPU.REGS.GHI);
            Assert.Equal(0b000000000000000000000000, (double)computer.CPU.REGS.JKL);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SET_rrrr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0b00000000000000000000000000000000;
            computer.CPU.REGS.EFGH = 0b00000000000000000000000000000000;
            computer.CPU.REGS.IJKL = 0b00000000000000000000000000000000;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SET ABCD, 0",
                    "SET EFGH, 20",
                    "SET IJKL, 31",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00000000000000000000000000000001, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0b00000000000100000000000000000000, (double)computer.CPU.REGS.EFGH);
            Assert.Equal(0b10000000000000000000000000000000, (double)computer.CPU.REGS.IJKL);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SET_rrrr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0b00000000000000000000000000000000;
            computer.CPU.REGS.EFGH = 0b00000000000000000000000000000000;
            computer.CPU.REGS.IJKL = 0b00000000000000000000000000000000;

            computer.CPU.REGS.Q = 0;
            computer.CPU.REGS.R = 20;
            computer.CPU.REGS.S = 31;
            computer.CPU.REGS.T = 32;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SET ABCD, Q",
                    "SET EFGH, R",
                    "SET IJKL, S",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00000000000000000000000000000001, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0b00000000000100000000000000000000, (double)computer.CPU.REGS.EFGH);
            Assert.Equal(0b10000000000000000000000000000000, (double)computer.CPU.REGS.IJKL);
            TUtils.IncrementCountedTests("exec");
        }
    }
}
