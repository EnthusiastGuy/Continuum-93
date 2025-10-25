using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_MAX
    {
        // MAX [r, rr, rrr, rrrr], [r, rr, rrr, rrrr]
        [Fact]
        public void TestEXEC_MAX_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 3;
            computer.CPU.REGS.B = 57;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MAX A, B",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(57, computer.CPU.REGS.A);
            Assert.Equal(57, computer.CPU.REGS.B);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MAX_rr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 0xABCD;
            computer.CPU.REGS.CD = 0xEFAB;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MAX AB, CD",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xEFAB, computer.CPU.REGS.AB);
            Assert.Equal(0xEFAB, computer.CPU.REGS.CD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MAX_rrr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 0xABCDEF;
            computer.CPU.REGS.DEF = 0xCDEFAB;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MAX ABC, DEF",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xCDEFAB, (double)computer.CPU.REGS.ABC);
            Assert.Equal(0xCDEFAB, (double)computer.CPU.REGS.DEF);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MAX_rrrr_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0x12131415;
            computer.CPU.REGS.EFGH = 0xABCDEFAB;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MAX ABCD, EFGH",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xABCDEFAB, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0xABCDEFAB, (double)computer.CPU.REGS.EFGH);
            TUtils.IncrementCountedTests("exec");
        }

        // MAX [r, rr, rrr, rrrr], [n, nn, nnn, nnnn]
        [Fact]
        public void TestEXEC_MAX_r_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 57;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MAX A, 45",
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
        public void TestEXEC_MAX_rr_nn()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 0xABCD;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MAX AB, 0x1234",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xABCD, computer.CPU.REGS.AB);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MAX_rrr_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 0xABCDEF;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MAX ABC, 0x123456",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xABCDEF, (double)computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MAX_rrrr_nnnn()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0xABCDEF12;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MAX ABCD, 0x12345678",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xABCDEF12, (double)computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        // MAX (float)
        [Fact]
        public void TestEXEC_MAX_fr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 22.0f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MAX F0, 45.0",
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
        public void TestEXEC_MAX_fr_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 22.0f);
            computer.CPU.FREGS.SetRegister(1, 45.0f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MAX F0, F1",
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
        public void TestEXEC_MAX_r_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 22;
            computer.CPU.FREGS.SetRegister(1, 45.2f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MAX A, F1",
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
        public void TestEXEC_MAX_r_fr_larger_float()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 57;
            computer.CPU.FREGS.SetRegister(1, 320.2f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MAX A, F1",
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
        public void TestEXEC_MAX_rr_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 222;
            computer.CPU.FREGS.SetRegister(1, 450.2f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MAX AB, F1",
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
        public void TestEXEC_MAX_rr_fr_larger_float()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 570;
            computer.CPU.FREGS.SetRegister(1, 81320.2f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MAX AB, F1",
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
        public void TestEXEC_MAX_rrr_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 9000000;
            computer.CPU.FREGS.SetRegister(1, 11000000);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MAX ABC, F1",
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
        public void TestEXEC_MAX_rrr_fr_larger_float()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 12000000;
            computer.CPU.FREGS.SetRegister(1, 81000320.2f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MAX ABC, F1",
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
        public void TestEXEC_MAX_rrrr_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 80_000_000;
            computer.CPU.FREGS.SetRegister(1, 90_000_000);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MAX ABCD, F1",
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
        public void TestEXEC_MAX_rrrr_fr_larger_float()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 120_000_000;
            computer.CPU.FREGS.SetRegister(1, 810_000_320_000.2f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MAX ABCD, F1",
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
        public void TestEXEC_MAX_fr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 45.2f);
            computer.CPU.REGS.A = 55;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MAX F0, A",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(55f, computer.CPU.FREGS.GetRegister(0));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MAX_fr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 2450.2f);
            computer.CPU.REGS.AB = 4222;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MAX F0, AB",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(4222f, computer.CPU.FREGS.GetRegister(0));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MAX_fr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 240_050.2f);
            computer.CPU.REGS.ABC = 522_200;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MAX F0, ABC",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(522_200f, computer.CPU.FREGS.GetRegister(0));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MAX_fr_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 240_050_000.2f);
            computer.CPU.REGS.ABCD = 522_200_000;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "MAX F0, ABCD",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(522_200_000f, computer.CPU.FREGS.GetRegister(0));
            TUtils.IncrementCountedTests("exec");
        }
    }
}
