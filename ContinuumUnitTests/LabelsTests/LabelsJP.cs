using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace LabelsTests
{
    public class LabelsJP
    {
        [Fact]
        public void TestLabel_JP()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@$"
                JP .Target
                BREAK
            .Target
                LD ABCD, 0x12345678
                BREAK
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
