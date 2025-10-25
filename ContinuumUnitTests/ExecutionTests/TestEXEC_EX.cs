using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_EX
    {
        [Fact]
        public void TestEXEC_EX_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 0b00001111;
            computer.CPU.REGS.B = 0b11001100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "EX A, B",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11001100, computer.CPU.REGS.A);
            Assert.Equal(0b00001111, computer.CPU.REGS.B);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_EX_rr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 0b0101010101010101;
            computer.CPU.REGS.CD = 0b1100110011001100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "EX AB, CD",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b1100110011001100, computer.CPU.REGS.AB);
            Assert.Equal(0b0101010101010101, computer.CPU.REGS.CD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_EX_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 0b010101010101010101010101;
            computer.CPU.REGS.DEF = 0b110011001100110011001100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "EX ABC, DEF",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b110011001100110011001100, (double)computer.CPU.REGS.ABC);
            Assert.Equal(0b010101010101010101010101, (double)computer.CPU.REGS.DEF);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_EX_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0b01010101010101010101010101010101;
            computer.CPU.REGS.EFGH = 0b11001100110011001100110011001100;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "EX ABCD, EFGH",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0b11001100110011001100110011001100, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0b01010101010101010101010101010101, (double)computer.CPU.REGS.EFGH);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_EX_fr()
        {
            Assembler cp = new();
            using Computer computer = new();



            computer.CPU.FREGS.SetRegister(0, (float)Math.Tau);
            computer.CPU.FREGS.SetRegister(1, (float)Math.PI);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "EX F0, F1",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((float)Math.PI, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal((float)Math.Tau, computer.CPU.FREGS.GetRegister(1));
            TUtils.IncrementCountedTests("exec");
        }
    }
}
