using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{
    public class TestEXEC_STREGS
    {
        [Fact]
        public void TestEXEC_STREGS_IrrrI_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.A = 0x20;
            computer.CPU.REGS.B = 0x21;
            computer.CPU.REGS.CD = 0x2223;
            computer.CPU.REGS.EFGH = 0x24252627;
            computer.CPU.REGS.I = 0x28;
            computer.CPU.REGS.J = 0x29;
            computer.CPU.REGS.KLM = 0xF0F1F2;

            cp.Build(
                @$"
                    LD XYZ, 50000
                    STREGS (XYZ), A, J
                    BREAK
                "
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            byte[] expected = new byte[] {
                0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0, 0, 0 };
            byte[] actual = computer.GetMemFrom(50000, 13);

            Assert.Equal(expected, actual);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_STREGS_InnnI_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.A = 0x20;
            computer.CPU.REGS.B = 0x21;
            computer.CPU.REGS.CD = 0x2223;
            computer.CPU.REGS.EFGH = 0x24252627;
            computer.CPU.REGS.I = 0x28;
            computer.CPU.REGS.J = 0x29;
            computer.CPU.REGS.KLM = 0xF0F1F2;

            cp.Build(
                @$"
                    STREGS (50000), A, J
                    BREAK
                "
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            byte[] expected = new byte[] {
                0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0, 0, 0 };
            byte[] actual = computer.GetMemFrom(50000, 13);

            Assert.Equal(expected, actual);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_STREGS_InnnI_r_r_inverse()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.A = 0x20;
            computer.CPU.REGS.B = 0x21;
            computer.CPU.REGS.CD = 0x2223;
            computer.CPU.REGS.EFGH = 0x24252627;
            computer.CPU.REGS.I = 0x28;
            computer.CPU.REGS.J = 0x29;
            computer.CPU.REGS.KLM = 0xF0F1F2;

            cp.Build(
                @$"
                    STREGS (50000), J, A
                    BREAK
                "
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            byte[] expected = new byte[] {
                0x29, 0x28, 0x27, 0x26, 0x25, 0x24, 0x23, 0x22, 0x21, 0x20, 0, 0, 0 };
            byte[] actual = computer.GetMemFrom(50000, 13);

            Assert.Equal(expected, actual);

            TUtils.IncrementCountedTests("exec");
        }
    }
}
