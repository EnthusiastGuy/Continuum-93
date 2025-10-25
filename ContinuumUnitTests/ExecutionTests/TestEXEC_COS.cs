using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_COS
    {
        [Fact]
        public void TestEXEC_COS_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            float radians = 1.0f;
            computer.CPU.FREGS.SetRegister(0, radians);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "COS F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((float)Math.Cos(radians), computer.CPU.FREGS.GetRegister(0));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_COS_fr_fr()
        {
            for (byte i = 1; i < 16; i++)
            {
                Assembler cp = new();
                using Computer computer = new();

                float radians = 1.0f;
                computer.CPU.FREGS.SetRegister(0, radians);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        $"COS F{i}, F0",
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                Assert.Equal((float)Math.Cos(radians), computer.CPU.FREGS.GetRegister(i));
                Assert.Equal(radians, computer.CPU.FREGS.GetRegister(0));
                TUtils.IncrementCountedTests("exec");
            }

        }
    }
}
