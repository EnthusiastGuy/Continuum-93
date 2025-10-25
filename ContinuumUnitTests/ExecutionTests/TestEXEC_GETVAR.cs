using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{
    public class TestEXEC_GETVAR
    {
        [Fact]
        public void TestEXEC_SETVAR_n_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                computer.CPU.FLAGS.ResetAll();
                computer.MEMC.HMEM[100] = 0x567890AB;

                cp.Build(@$"
                    GETVAR 100, {TUtils.Get32bitRegisterString(i)}
                    BREAK
                ");

                computer.LoadMem(cp.GetCompiledCode());
                computer.Run();

                Assert.Equal((double)0x567890AB, computer.CPU.REGS.Get32BitRegister(i));
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_SETVAR_rrrr_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 10; i++)
            {
                for (byte j = 13; j < 23; j++)
                {
                    computer.Clear();
                    computer.CPU.FLAGS.ResetAll();
                    computer.MEMC.HMEM[100] = 0x567890AB;

                    cp.Build(@$"
                        LD {TUtils.Get32bitRegisterString(i)}, 100
                        GETVAR {TUtils.Get32bitRegisterString(i)}, {TUtils.Get32bitRegisterString(j)}
                        BREAK
                    ");

                    computer.LoadMem(cp.GetCompiledCode());
                    computer.Run();

                    Assert.Equal((double)0x567890AB, computer.CPU.REGS.Get32BitRegister(j));
                    TUtils.IncrementCountedTests("exec");
                }
            }
        }
    }
}
