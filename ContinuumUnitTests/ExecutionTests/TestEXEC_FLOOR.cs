using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_FLOOR
    {
        [Fact]
        public void TestEXEC_FLOOR_fr()
        {
            for (byte i = 0; i < 16; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                float value = 1.0f + i / 16.0f;
                computer.CPU.FREGS.SetRegister(i, value);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        $"FLOOR F{i}",
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                Assert.Equal(MathF.Floor(value), computer.CPU.FREGS.GetRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_FLOOR_fr_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 1.2f);
            computer.CPU.FREGS.SetRegister(2, 1.6f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "FLOOR F1, F0",
                    "FLOOR F3, F2",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1.0f, computer.CPU.FREGS.GetRegister(1));
            Assert.Equal(1.2f, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal(1.0f, computer.CPU.FREGS.GetRegister(3));
            Assert.Equal(1.6f, computer.CPU.FREGS.GetRegister(2));
            TUtils.IncrementCountedTests("exec");
        }
    }
}
