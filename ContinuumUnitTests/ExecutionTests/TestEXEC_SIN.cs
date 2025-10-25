using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_SIN
    {
        [Fact]
        public void TestEXEC_SIN_fr()
        {
            for (byte i = 0; i < 16; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                float radians = 1.0f;
                computer.CPU.FREGS.SetRegister(i, radians);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        $"SIN F{i}",
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                Assert.Equal((float)Math.Sin(radians), computer.CPU.FREGS.GetRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_SIN_fr_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            float radians = 1.0f;
            computer.CPU.FREGS.SetRegister(0, radians);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "SIN F1, F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((float)Math.Sin(radians), computer.CPU.FREGS.GetRegister(1));
            Assert.Equal(radians, computer.CPU.FREGS.GetRegister(0));
            TUtils.IncrementCountedTests("exec");
        }
    }
}
