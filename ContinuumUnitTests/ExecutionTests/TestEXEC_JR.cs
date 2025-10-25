using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_JR
    {
        [Fact]
        public void TestEXEC_JR_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                #ORG 20000
                JR .JumpAddress
            .JumpAddress
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            Assert.Equal(new byte[] { 24, 0, 0, 0, 5, 253 }, compiled);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_JR_nnn_longer_jump()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 3;
            computer.CPU.REGS.B = 57;

            cp.Build(@"
                #ORG 20000
                JR .JumpAddress
                LD A, 0x7F
                LD B, A
                DEC B
            .JumpAddress
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMemAt(20000, compiled);
            computer.Run(20000);

            Assert.Equal(3, computer.CPU.REGS.A);
            Assert.Equal(57, computer.CPU.REGS.B);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_JR_nnn_longer_cond_jump()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 3;
            computer.CPU.REGS.B = 57;

            cp.Build(@"
                #ORG 20000
                LD E, 5
                CP E, 6
                JR LT, .JumpAddress
                LD A, 0x7F
                LD B, A
                DEC B
            .JumpAddress
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMemAt(20000, compiled);
            computer.Run(20000);

            Assert.Equal(3, computer.CPU.REGS.A);
            Assert.Equal(57, computer.CPU.REGS.B);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_JR_ff_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                #ORG 20000
                JR Z, .JumpAddress
            .JumpAddress
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            Assert.Equal(new byte[] { 24, 40, 0, 0, 5, 253 }, compiled);
            TUtils.IncrementCountedTests("exec");
        }
    }
}
