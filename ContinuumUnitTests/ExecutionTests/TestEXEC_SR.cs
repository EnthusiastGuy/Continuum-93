using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_SR
    {
        [Fact]
        public void TestEXEC_SR_r_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 0b10010000;
            computer.CPU.REGS.B = 0b10000000;
            computer.CPU.REGS.C = 0b10000000;
            computer.CPU.REGS.D = 0b11111111;
            computer.CPU.REGS.E = 0b10101010;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SR A, 3",
                    "SR B, 7",
                    "SR C, 8",
                    "SR D, 4",
                    "SR E, 0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00010010, computer.CPU.REGS.A);
            Assert.Equal(0b00000001, computer.CPU.REGS.B);
            Assert.Equal(0, computer.CPU.REGS.C);
            Assert.Equal(0b00001111, computer.CPU.REGS.D);
            Assert.Equal(0b10101010, computer.CPU.REGS.E);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SR_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 0b10010000;
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
                    "SR A, F",
                    "SR B, G",
                    "SR C, H",
                    "SR D, I",
                    "SR E, J",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00010010, computer.CPU.REGS.A);
            Assert.Equal(0b00000001, computer.CPU.REGS.B);
            Assert.Equal(0, computer.CPU.REGS.C);
            Assert.Equal(0b00001111, computer.CPU.REGS.D);
            Assert.Equal(0b10101010, computer.CPU.REGS.E);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SR_rr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 0b1001000000000000;
            computer.CPU.REGS.CD = 0b1000000000000000;
            computer.CPU.REGS.EF = 0b1000000000000000;
            computer.CPU.REGS.GH = 0b1111111111111111;
            computer.CPU.REGS.IJ = 0b1010101010101010;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SR AB, 6",
                    "SR CD, 15",
                    "SR EF, 16",
                    "SR GH, 8",
                    "SR IJ, 0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b0000001001000000, computer.CPU.REGS.AB);
            Assert.Equal(0b0000000000000001, computer.CPU.REGS.CD);
            Assert.Equal(0, computer.CPU.REGS.EF);
            Assert.Equal(0b0000000011111111, computer.CPU.REGS.GH);
            Assert.Equal(0b1010101010101010, computer.CPU.REGS.IJ);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SR_rr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 0b1001000000000000;
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
                    "SR AB, K",
                    "SR CD, L",
                    "SR EF, M",
                    "SR GH, N",
                    "SR IJ, O",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b0000001001000000, computer.CPU.REGS.AB);
            Assert.Equal(0b0000000000000001, computer.CPU.REGS.CD);
            Assert.Equal(0, computer.CPU.REGS.EF);
            Assert.Equal(0b0000000011111111, computer.CPU.REGS.GH);
            Assert.Equal(0b1010101010101010, computer.CPU.REGS.IJ);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SR_rrr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 0b100100000000000000000000;
            computer.CPU.REGS.DEF = 0b100000000000000000000000;
            computer.CPU.REGS.GHI = 0b100000000000000000000000;
            computer.CPU.REGS.JKL = 0b111111111111111111111111;
            computer.CPU.REGS.MNO = 0b101010101010101010101010;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SR ABC, 9",
                    "SR DEF, 23",
                    "SR GHI, 24",
                    "SR JKL, 12",
                    "SR MNO, 0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b000000000100100000000000, (double)computer.CPU.REGS.ABC);
            Assert.Equal(0b000000000000000000000001, (double)computer.CPU.REGS.DEF);
            Assert.Equal(0, (double)computer.CPU.REGS.GHI);
            Assert.Equal(0b000000000000111111111111, (double)computer.CPU.REGS.JKL);
            Assert.Equal(0b101010101010101010101010, (double)computer.CPU.REGS.MNO);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SR_rrr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 0b100100000000000000000000;
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
                    "SR ABC, P",
                    "SR DEF, Q",
                    "SR GHI, R",
                    "SR JKL, S",
                    "SR MNO, T",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b000000000100100000000000, (double)computer.CPU.REGS.ABC);
            Assert.Equal(0b000000000000000000000001, (double)computer.CPU.REGS.DEF);
            Assert.Equal(0, (double)computer.CPU.REGS.GHI);
            Assert.Equal(0b000000000000111111111111, (double)computer.CPU.REGS.JKL);
            Assert.Equal(0b101010101010101010101010, (double)computer.CPU.REGS.MNO);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SR_rrrr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0b10010000000000000000000000000000;
            computer.CPU.REGS.EFGH = 0b10000000000000000000000000000000;
            computer.CPU.REGS.IJKL = 0b11111111111111111111111111111111;
            computer.CPU.REGS.MNOP = 0b10101010101010101010101010101010;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SR ABCD, 12",
                    "SR EFGH, 31",
                    "SR IJKL, 16",
                    "SR MNOP, 0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00000000000010010000000000000000, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0b00000000000000000000000000000001, (double)computer.CPU.REGS.EFGH);
            Assert.Equal(0b00000000000000001111111111111111, (double)computer.CPU.REGS.IJKL);
            Assert.Equal(0b10101010101010101010101010101010, (double)computer.CPU.REGS.MNOP);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SR_rrrr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0b10010000000000000000000000000000;
            computer.CPU.REGS.EFGH = 0b10000000000000000000000000000000;
            computer.CPU.REGS.IJKL = 0b11111111111111111111111111111111;
            computer.CPU.REGS.MNOP = 0b10101010101010101010101010101010;

            computer.CPU.REGS.Q = 12;
            computer.CPU.REGS.R = 31;
            computer.CPU.REGS.S = 16;
            computer.CPU.REGS.T = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SR ABCD, Q",
                    "SR EFGH, R",
                    "SR IJKL, S",
                    "SR MNOP, T",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b00000000000010010000000000000000, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0b00000000000000000000000000000001, (double)computer.CPU.REGS.EFGH);
            Assert.Equal(0b00000000000000001111111111111111, (double)computer.CPU.REGS.IJKL);
            Assert.Equal(0b10101010101010101010101010101010, (double)computer.CPU.REGS.MNOP);
            TUtils.IncrementCountedTests("exec");
        }
    }
}
