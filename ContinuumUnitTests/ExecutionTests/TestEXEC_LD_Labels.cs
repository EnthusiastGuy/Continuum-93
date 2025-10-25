using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_LD_Labels
    {
        [Fact]
        public void TestEXEC_LD_r_nnn_Label()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                    #ORG 20000
                    LD ABC, .TestLabel
                .TestLabel
                    #DB 0, 0, 0, 0
                    BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMemAt(20000, compiled);
            computer.Run();

            TUtils.IncrementCountedTests("exec");
        }
        [Fact]
        public void TestEXEC_LD_r_InnnI_Label()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                    #ORG 20000
                    LD ABC, (.TestLabel)
                    BREAK
                .TestLabel
                    #DB 0xAA, 0xBB, 0xCC, 0xDD
                    BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMemAt(20000, compiled);
            computer.Run();

            Assert.Equal(0xAABBCC, (long)computer.CPU.REGS.ABC);

            TUtils.IncrementCountedTests("exec");
        }
    }
}
