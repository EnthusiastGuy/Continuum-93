using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_MIN
    {
        // MIN [r, rr, rrr, rrrr], [r, rr, rrr, rrrr]
        [Fact]
        public void TestEXEC_MIN_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 57;
            computer.CPU.REGS.B = 3;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MIN A, B",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(3, computer.CPU.REGS.A);
            Assert.Equal(3, computer.CPU.REGS.B);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MIN_rr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 0xEFAB;
            computer.CPU.REGS.CD = 0xABCD;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MIN AB, CD",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xABCD, computer.CPU.REGS.AB);
            Assert.Equal(0xABCD, computer.CPU.REGS.CD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MIN_rrr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 0xCDEFAB;
            computer.CPU.REGS.DEF = 0xABCDEF;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MIN ABC, DEF",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xABCDEF, (double)computer.CPU.REGS.ABC);
            Assert.Equal(0xABCDEF, (double)computer.CPU.REGS.DEF);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MIN_rrrr_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0xABCDEFAB;
            computer.CPU.REGS.EFGH = 0x12131415;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MIN ABCD, EFGH",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x12131415, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0x12131415, (double)computer.CPU.REGS.EFGH);
            TUtils.IncrementCountedTests("exec");
        }

        // MIN [r, rr, rrr, rrrr], [n, nn, nnn, nnnn]
        [Fact]
        public void TestEXEC_MIN_r_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 57;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MIN A, 45",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(45, computer.CPU.REGS.A);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MIN_rr_nn()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 0xABCD;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MIN AB, 0x1234",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x1234, computer.CPU.REGS.AB);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MIN_rrr_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 0xABCDEF;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MIN ABC, 0x123456",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x123456, (double)computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MIN_rrrr_nnnn()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0xABCDEF12;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MIN ABCD, 0x12345678",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x12345678, (double)computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        // MIN (float)
        [Fact]
        public void TestEXEC_MIN_fr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 57.2f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MIN F0, 45.0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(45, computer.CPU.FREGS.GetRegister(0));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MIN_fr_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 57.2f);
            computer.CPU.FREGS.SetRegister(1, 45.0f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MIN F0, F1",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(45, computer.CPU.FREGS.GetRegister(0));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MIN_r_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 57;
            computer.CPU.FREGS.SetRegister(1, 45.2f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MIN A, F1",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(45, computer.CPU.REGS.A);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MIN_r_fr_larger_float()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 57;
            computer.CPU.FREGS.SetRegister(1, 320.2f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MIN A, F1",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(57, computer.CPU.REGS.A);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MIN_rr_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 570;
            computer.CPU.FREGS.SetRegister(1, 450.2f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MIN AB, F1",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(450, computer.CPU.REGS.AB);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MIN_rr_fr_larger_float()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 570;
            computer.CPU.FREGS.SetRegister(1, 81320.2f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MIN AB, F1",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(570, computer.CPU.REGS.AB);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MIN_rrr_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 12000000;
            computer.CPU.FREGS.SetRegister(1, 11000000);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MIN ABC, F1",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((uint)11000000, computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MIN_rrr_fr_larger_float()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 12000000;
            computer.CPU.FREGS.SetRegister(1, 81000320.2f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MIN ABC, F1",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((uint)12000000, computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MIN_rrrr_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 120_000_000;
            computer.CPU.FREGS.SetRegister(1, 90_000_000);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MIN ABCD, F1",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((uint)90_000_000, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MIN_rrrr_fr_larger_float()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 120_000_000;
            computer.CPU.FREGS.SetRegister(1, 810_000_320.2f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MIN ABCD, F1",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((uint)120_000_000, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MIN_fr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 45.2f);
            computer.CPU.REGS.A = 22;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MIN F0, A",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(22f, computer.CPU.FREGS.GetRegister(0));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MIN_fr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 2450.2f);
            computer.CPU.REGS.AB = 1222;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MIN F0, AB",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1222f, computer.CPU.FREGS.GetRegister(0));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MIN_fr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 240_050.2f);
            computer.CPU.REGS.ABC = 122_200;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MIN F0, ABC",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(122_200f, computer.CPU.FREGS.GetRegister(0));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MIN_fr_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 240_050_000.2f);
            computer.CPU.REGS.ABCD = 122_200_000;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MIN F0, ABCD",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(122_200_000f, computer.CPU.FREGS.GetRegister(0));
            TUtils.IncrementCountedTests("exec");
        }
    }
}
