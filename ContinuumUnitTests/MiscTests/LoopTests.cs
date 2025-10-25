using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace MiscTests
{

    public class LoopTests
    {
        [Fact]
        public void TestInfiniteLoop()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                    #ORG 1000
                .Infinite
                    LD A, 3
                    JP .Infinite
                    BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            TUtils.IncrementCountedTests("exec");
        }
    }
}
