using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{
    public class TestEXEC_LDREGS
    {
        [Fact]
        public void TestEXEC_LDREGS_r_r_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.Equal(0, computer.CPU.REGS.A);
            Assert.Equal(0, computer.CPU.REGS.B);
            Assert.Equal(0, computer.CPU.REGS.CD);
            Assert.Equal(0, (long)computer.CPU.REGS.EFGH);
            Assert.Equal(0, computer.CPU.REGS.I);
            Assert.Equal(0, computer.CPU.REGS.J);
            Assert.Equal(0, (long)computer.CPU.REGS.KLM);

            cp.Build(
                @$"
                    LD XYZ, .Source
                    LDREGS A, J, (XYZ)
                    BREAK
                .Source
                    #DB 0x0A, 0x0B, 0xABCD, 0x12345678, 0x99, 0x98
                "
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x0A, computer.CPU.REGS.A);
            Assert.Equal(0x0B, computer.CPU.REGS.B);
            Assert.Equal(0xABCD, computer.CPU.REGS.CD);
            Assert.Equal(0x12345678, (long)computer.CPU.REGS.EFGH);
            Assert.Equal(0x99, computer.CPU.REGS.I);
            Assert.Equal(0x98, computer.CPU.REGS.J);
            Assert.Equal(0, (long)computer.CPU.REGS.KLM);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_LDREGS_r_r_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.Equal(0, computer.CPU.REGS.A);
            Assert.Equal(0, computer.CPU.REGS.B);
            Assert.Equal(0, computer.CPU.REGS.CD);
            Assert.Equal(0, (long)computer.CPU.REGS.EFGH);
            Assert.Equal(0, computer.CPU.REGS.I);
            Assert.Equal(0, computer.CPU.REGS.J);
            Assert.Equal(0, (long)computer.CPU.REGS.KLM);

            cp.Build(
                @$"
                    LDREGS A, J, (.Source)
                    BREAK
                .Source
                    #DB 0x0A, 0x0B, 0xABCD, 0x12345678, 0x99, 0x98
                "
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x0A, computer.CPU.REGS.A);
            Assert.Equal(0x0B, computer.CPU.REGS.B);
            Assert.Equal(0xABCD, computer.CPU.REGS.CD);
            Assert.Equal(0x12345678, (long)computer.CPU.REGS.EFGH);
            Assert.Equal(0x99, computer.CPU.REGS.I);
            Assert.Equal(0x98, computer.CPU.REGS.J);
            Assert.Equal(0, (long)computer.CPU.REGS.KLM);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_LDREGS_r_r_InnnI_inverse()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.Equal(0, computer.CPU.REGS.A);
            Assert.Equal(0, computer.CPU.REGS.B);
            Assert.Equal(0, computer.CPU.REGS.CD);
            Assert.Equal(0, (long)computer.CPU.REGS.EFGH);
            Assert.Equal(0, computer.CPU.REGS.I);
            Assert.Equal(0, computer.CPU.REGS.J);
            Assert.Equal(0, (long)computer.CPU.REGS.KLM);

            cp.Build(
                @$"
                    LDREGS J, A, (.Source)
                    BREAK
                .Source
                    #DB 0x0A, 0x0B, 0xABCD, 0x12345678, 0x99, 0x98
                "
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x98, computer.CPU.REGS.A);
            Assert.Equal(0x99, computer.CPU.REGS.B);
            Assert.Equal(0x7856, computer.CPU.REGS.CD);
            Assert.Equal(0x3412CDAB, (long)computer.CPU.REGS.EFGH);
            Assert.Equal(0x0B, computer.CPU.REGS.I);
            Assert.Equal(0x0A, computer.CPU.REGS.J);
            Assert.Equal(0, (long)computer.CPU.REGS.KLM);
            TUtils.IncrementCountedTests("exec");
        }
    }
}
