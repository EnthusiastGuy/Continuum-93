using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_TAN
    {
        [Fact]
        public void TestEXEC_TAN_fr()
        {
            for (byte i = 0; i < 16; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                float radians = 1.0f;
                computer.CPU.FREGS.SetRegister(i, radians);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        $"TAN F{i}",
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                Assert.Equal(MathF.Tan(radians), computer.CPU.FREGS.GetRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_TAN_fr_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            float radians = 1.0f;
            computer.CPU.FREGS.SetRegister(0, radians);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "TAN F1, F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(MathF.Tan(radians), computer.CPU.FREGS.GetRegister(1));
            Assert.Equal(radians, computer.CPU.FREGS.GetRegister(0));
            TUtils.IncrementCountedTests("exec");
        }
    }
}
