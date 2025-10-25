using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{
    public class TestEXEC_RES
    {
        [Fact]
        public void TestEXEC_RES_r_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 0b11111111;
            computer.CPU.REGS.B = 0b11111111;
            computer.CPU.REGS.C = 0b11111111;
            computer.CPU.REGS.D = 0b11111111;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "RES A, 0",
                    "RES B, 5",
                    "RES C, 7",
                    "RES D, 8",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11111110, computer.CPU.REGS.A);
            Assert.Equal(0b11011111, computer.CPU.REGS.B);
            Assert.Equal(0b01111111, computer.CPU.REGS.C);
            Assert.Equal(0b11111111, computer.CPU.REGS.D);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RES_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 0b11111111;
            computer.CPU.REGS.B = 0b11111111;
            computer.CPU.REGS.C = 0b11111111;
            computer.CPU.REGS.D = 0b11111111;

            computer.CPU.REGS.F = 0;
            computer.CPU.REGS.G = 5;
            computer.CPU.REGS.H = 7;
            computer.CPU.REGS.I = 8;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "RES A, F",
                    "RES B, G",
                    "RES C, H",
                    "RES D, I",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11111110, computer.CPU.REGS.A);
            Assert.Equal(0b11011111, computer.CPU.REGS.B);
            Assert.Equal(0b01111111, computer.CPU.REGS.C);
            Assert.Equal(0b11111111, computer.CPU.REGS.D);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RES_rr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 0b1111111111111111;
            computer.CPU.REGS.CD = 0b1111111111111111;
            computer.CPU.REGS.EF = 0b1111111111111111;
            computer.CPU.REGS.GH = 0b1111111111111111;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "RES AB, 0",
                    "RES CD, 10",
                    "RES EF, 15",
                    "RES GH, 16",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b1111111111111110, computer.CPU.REGS.AB);
            Assert.Equal(0b1111101111111111, computer.CPU.REGS.CD);
            Assert.Equal(0b0111111111111111, computer.CPU.REGS.EF);
            Assert.Equal(0b1111111111111111, computer.CPU.REGS.GH);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RES_rr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 0b1111111111111111;
            computer.CPU.REGS.CD = 0b1111111111111111;
            computer.CPU.REGS.EF = 0b1111111111111111;
            computer.CPU.REGS.GH = 0b1111111111111111;

            computer.CPU.REGS.K = 0;
            computer.CPU.REGS.L = 10;
            computer.CPU.REGS.M = 15;
            computer.CPU.REGS.N = 16;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "RES AB, K",
                    "RES CD, L",
                    "RES EF, M",
                    "RES GH, N",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b1111111111111110, computer.CPU.REGS.AB);
            Assert.Equal(0b1111101111111111, computer.CPU.REGS.CD);
            Assert.Equal(0b0111111111111111, computer.CPU.REGS.EF);
            Assert.Equal(0b1111111111111111, computer.CPU.REGS.GH);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RES_rrr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 0b111111111111111111111111;
            computer.CPU.REGS.DEF = 0b111111111111111111111111;
            computer.CPU.REGS.GHI = 0b111111111111111111111111;
            computer.CPU.REGS.JKL = 0b111111111111111111111111;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "RES ABC, 0",
                    "RES DEF, 15",
                    "RES GHI, 23",
                    "RES JKL, 24",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b111111111111111111111110, (double)computer.CPU.REGS.ABC);
            Assert.Equal(0b111111110111111111111111, (double)computer.CPU.REGS.DEF);
            Assert.Equal(0b011111111111111111111111, (double)computer.CPU.REGS.GHI);
            Assert.Equal(0b111111111111111111111111, (double)computer.CPU.REGS.JKL);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RES_rrr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 0b111111111111111111111111;
            computer.CPU.REGS.DEF = 0b111111111111111111111111;
            computer.CPU.REGS.GHI = 0b111111111111111111111111;
            computer.CPU.REGS.JKL = 0b111111111111111111111111;

            computer.CPU.REGS.P = 0;
            computer.CPU.REGS.Q = 15;
            computer.CPU.REGS.R = 23;
            computer.CPU.REGS.S = 24;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "RES ABC, P",
                    "RES DEF, Q",
                    "RES GHI, R",
                    "RES JKL, S",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b111111111111111111111110, (double)computer.CPU.REGS.ABC);
            Assert.Equal(0b111111110111111111111111, (double)computer.CPU.REGS.DEF);
            Assert.Equal(0b011111111111111111111111, (double)computer.CPU.REGS.GHI);
            Assert.Equal(0b111111111111111111111111, (double)computer.CPU.REGS.JKL);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RES_rrrr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0b11111111111111111111111111111111;
            computer.CPU.REGS.EFGH = 0b11111111111111111111111111111111;
            computer.CPU.REGS.IJKL = 0b11111111111111111111111111111111;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "RES ABCD, 0",
                    "RES EFGH, 20",
                    "RES IJKL, 31",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11111111111111111111111111111110, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0b11111111111011111111111111111111, (double)computer.CPU.REGS.EFGH);
            Assert.Equal(0b01111111111111111111111111111111, (double)computer.CPU.REGS.IJKL);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RES_rrrr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0b11111111111111111111111111111111;
            computer.CPU.REGS.EFGH = 0b11111111111111111111111111111111;
            computer.CPU.REGS.IJKL = 0b11111111111111111111111111111111;

            computer.CPU.REGS.Q = 0;
            computer.CPU.REGS.R = 20;
            computer.CPU.REGS.S = 31;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "RES ABCD, Q",
                    "RES EFGH, R",
                    "RES IJKL, S",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11111111111111111111111111111110, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0b11111111111011111111111111111111, (double)computer.CPU.REGS.EFGH);
            Assert.Equal(0b01111111111111111111111111111111, (double)computer.CPU.REGS.IJKL);
            TUtils.IncrementCountedTests("exec");
        }
    }
}
