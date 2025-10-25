using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace LabelsTests
{
    public class LabelsCALL
    {
        [Fact]
        public void TestLabel_CALL()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@$"
                CALL .Target
                BREAK
            .Target
                LD ABCD, 0x12345678
                RET
                "
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x12345678, (long)computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }
    }
}
