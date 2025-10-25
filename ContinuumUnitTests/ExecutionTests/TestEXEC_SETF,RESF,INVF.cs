using Continuum93.Emulator;
using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_SETF_RESF_INVF
    {
        [Fact]
        public void TestEXEC_SETF_ff()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 15; i++)
            {
                computer.Clear();
                computer.CPU.FLAGS.ResetAll();

                string flagName = Flags.GetFlagNameByIndex(i);
                cp.Build(@$"SETF {flagName}
                            BREAK");

                computer.LoadMem(cp.GetCompiledCode());
                computer.Run();

                Assert.True(computer.CPU.FLAGS.GetValueByName(flagName));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_RESF_ff()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 15; i++)
            {
                computer.Clear();
                computer.CPU.FLAGS.SetAll();

                string flagName = Flags.GetFlagNameByIndex(i);
                cp.Build(@$"RESF {flagName}
                            BREAK");

                computer.LoadMem(cp.GetCompiledCode());
                computer.Run();

                Assert.False(computer.CPU.FLAGS.GetValueByName(flagName));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_INVF_ff()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 15; i++)
            {
                string flagName = Flags.GetFlagNameByIndex(i);
                computer.Clear();

                bool initialValue = computer.CPU.FLAGS.GetValueByName(flagName);

                cp.Build(@$"INVF {flagName}
                            BREAK");

                computer.LoadMem(cp.GetCompiledCode());
                computer.Run();

                bool resultValue = computer.CPU.FLAGS.GetValueByName(flagName);

                Assert.NotEqual(initialValue, resultValue);
                TUtils.IncrementCountedTests("exec");
            }
        }
    }
}
