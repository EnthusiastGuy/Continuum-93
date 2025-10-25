using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{
    public class TestEXEC_CALLR
    {
        [Fact]
        public void TestEXEC_CALLR_nnn_compile()
        {
            Assembler cp = new();

            cp.Build(@"
                #ORG 20000
                CALLR .CallAddress
                LD A, A
            .CallAddress
                RET
            ");

            byte[] compiled = cp.GetCompiledCode();

            Assert.Equal([22, 0, 0, 0, 9, 1, 1, 0, 0, 255], compiled);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CALLR_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                #ORG 20000
                LD A, 0
                CALLR .CallAddress
                LD A, A
                BREAK
            .CallAddress
                LD A, 45
                RET
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(45, computer.CPU.REGS.A);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CALLR_ff_nnn_compile()
        {
            Assembler cp = new();

            cp.Build(@"
                #ORG 20000
                CALLR Z, .LabelCalled
                LD A, A
            .LabelCalled
                RET
            ");

            byte[] compiled = cp.GetCompiledCode();

            Assert.Equal([22, 40, 0, 0, 9, 1, 1, 0, 0, 255], compiled);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CALLR_ff_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                #ORG 20000
                LD A, 2
                CP A, 2
                CALLR Z, .LabelCalled
                LD A, A
                BREAK
            .LabelCalled
                LD A, 88
                RET
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(88, computer.CPU.REGS.A);
            TUtils.IncrementCountedTests("exec");
        }
    }
}
