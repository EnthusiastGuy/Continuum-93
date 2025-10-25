using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_ABS
    {
        [Fact]
        public void TestEXEC_ABS_fr()
        {
            using Computer computer = new();
            Assembler cp = new();

            for (byte i = 0; i < 16; i++)
            {
                computer.Clear();
                float value = -1.0f;
                computer.CPU.FREGS.SetRegister(i, value);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        $"ABS F{i}",
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                Assert.Equal(MathF.Abs(value), computer.CPU.FREGS.GetRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_ABS_fr_fr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.Clear();
            float value = -1.0f;
            computer.CPU.FREGS.SetRegister(0, value);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "ABS F1, F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(MathF.Abs(value), computer.CPU.FREGS.GetRegister(1));
            Assert.Equal(value, computer.CPU.FREGS.GetRegister(0));
            TUtils.IncrementCountedTests("exec");
        }
    }
}
