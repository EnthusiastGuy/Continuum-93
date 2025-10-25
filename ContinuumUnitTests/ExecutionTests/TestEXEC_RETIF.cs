using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{
    public class TestEXEC_RETIF
    {
        [Fact]
        public void TestEXEC_RETIF_ff()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@$"
                LD A, 128
                LD B, 5

                CALLR .TestLabel
                CALLR .TestLabel2
                
                BREAK

            .TestLabel
                CP B, 5
                RETIF Z
                LD A, 100
                RET

            .TestLabel2
                CP A, 128
                RETIF NZ
                LD B, 100
                RET

                BREAK"
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(128, computer.CPU.REGS.A);
            Assert.Equal(100, computer.CPU.REGS.B);
            TUtils.IncrementCountedTests("exec");
        }
    }
}
