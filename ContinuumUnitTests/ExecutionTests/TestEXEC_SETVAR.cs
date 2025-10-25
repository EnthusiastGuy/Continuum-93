using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{
    public class TestEXEC_SETVAR
    {
        [Fact]
        public void TestEXEC_SETVAR_n_n()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@$"
                SETVAR 100, 0x12345678
                BREAK
            ");

            computer.LoadMem(cp.GetCompiledCode());
            computer.Run();

            Assert.Equal((double)0x12345678, computer.MEMC.HMEM[100]);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_SETVAR_n_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                computer.CPU.FLAGS.ResetAll();

                cp.Build(@$"
                    LD {TUtils.Get32bitRegisterString(i)}, 0x12345678
                    SETVAR 100, {TUtils.Get32bitRegisterString(i)}
                    BREAK
                ");

                computer.LoadMem(cp.GetCompiledCode());
                computer.Run();

                Assert.Equal((double)0x12345678, computer.MEMC.HMEM[100]);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_SETVAR_rrrr_n()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 26; i++)
            {
                computer.Clear();
                computer.CPU.FLAGS.ResetAll();

                cp.Build(@$"
                    LD {TUtils.Get32bitRegisterString(i)}, 100
                    SETVAR {TUtils.Get32bitRegisterString(i)}, 0x12345678
                    BREAK
                ");

                computer.LoadMem(cp.GetCompiledCode());
                computer.Run();

                Assert.Equal((double)0x12345678, computer.MEMC.HMEM[100]);
                TUtils.IncrementCountedTests("exec");
            }
        }

        [Fact]
        public void TestEXEC_SETVAR_rrrr_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            for (byte i = 0; i < 13; i++)
            {
                for (byte j = 16; j < 23; j++)
                {
                    computer.Clear();
                    computer.CPU.FLAGS.ResetAll();

                    cp.Build(@$"
                        LD {TUtils.Get32bitRegisterString(i)}, 100
                        LD {TUtils.Get32bitRegisterString(j)}, 0x12345678
                        SETVAR {TUtils.Get32bitRegisterString(i)}, {TUtils.Get32bitRegisterString(j)}
                        BREAK
                    ");

                    computer.LoadMem(cp.GetCompiledCode());
                    computer.Run();

                    var expected = 0x12345678;
                    var actual = computer.MEMC.HMEM[100];

                    Assert.True(
                        actual == (double)expected,
                        $"Assertion failed for i={i}, j={j}. Expected: 0x{expected:X8}, Actual: 0x{actual:X8}"
                    );
                    TUtils.IncrementCountedTests("exec");
                }
            }
        }
    }
}
