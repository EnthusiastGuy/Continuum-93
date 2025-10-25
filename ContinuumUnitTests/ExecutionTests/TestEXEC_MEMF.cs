using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_MEMF
    {
        [Fact]
        public void TestEXEC_MEMF_rrr_rrr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                LD ABC, 20000   ; Target
                LD DEF, 50000   ; Length
                LD G, 0xAA      ; Value
                MEMF ABC, DEF, G
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            for (int i = 0; i < 50000; i++)
            {
                if (computer.MEMC.RAM.Data[20000 + i] != 0xAA)
                {
                    Assert.False(true,
                        string.Format("Bad memory value. Expected 0xAA but got {0}", computer.MEMC.RAM.Data[20000 + i])
                    );
                }
            }

            Assert.Equal(0, (double)computer.MEMC.RAM.Data[19999]);
            Assert.Equal(0, (double)computer.MEMC.RAM.Data[70000]);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MEMF_nnn_rrr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                LD DEF, 50000   ; Length
                LD G, 0xAA      ; Value
                MEMF 20000, DEF, G  ; 20000 is the target
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            for (int i = 0; i < 50000; i++)
            {
                if (computer.MEMC.RAM.Data[20000 + i] != 0xAA)
                {
                    Assert.False(true,
                        string.Format("Bad memory value. Expected 0xAA but got {0}", computer.MEMC.RAM.Data[20000 + i])
                    );
                }
            }

            Assert.Equal(0, (double)computer.MEMC.RAM.Data[19999]);
            Assert.Equal(0, (double)computer.MEMC.RAM.Data[70000]);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MEMF_rrr_nnn_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                LD ABC, 20000   ; Target
                LD G, 0xAA      ; Value
                MEMF ABC, 50000, G  ; 50000 is the length
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            for (int i = 0; i < 50000; i++)
            {
                if (computer.MEMC.RAM.Data[20000 + i] != 0xAA)
                {
                    Assert.False(true,
                        string.Format("Bad memory value. Expected 0xAA but got {0}", computer.MEMC.RAM.Data[20000 + i])
                    );
                }
            }

            Assert.Equal(0, (double)computer.MEMC.RAM.Data[19999]);
            Assert.Equal(0, (double)computer.MEMC.RAM.Data[70000]);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MEMF_nnn_nnn_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                LD G, 0xAA      ; Value
                MEMF 20000, 50000, G  ; 20000 is the target, 50000 is the length
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            for (int i = 0; i < 50000; i++)
            {
                if (computer.MEMC.RAM.Data[20000 + i] != 0xAA)
                {
                    Assert.False(true,
                        string.Format("Bad memory value. Expected 0xAA but got {0}", computer.MEMC.RAM.Data[20000 + i])
                    );
                }
            }

            Assert.Equal(0, (double)computer.MEMC.RAM.Data[19999]);
            Assert.Equal(0, (double)computer.MEMC.RAM.Data[70000]);

            TUtils.IncrementCountedTests("exec");
        }


        [Fact]
        public void TestEXEC_MEMF_rrr_rrr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                LD ABC, 20000   ; Target
                LD DEF, 50000   ; Length
                MEMF ABC, DEF, 0xAA ; 0xAA is the value
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            for (int i = 0; i < 50000; i++)
            {
                if (computer.MEMC.RAM.Data[20000 + i] != 0xAA)
                {
                    Assert.False(true,
                        string.Format("Bad memory value. Expected 0xAA but got {0}", computer.MEMC.RAM.Data[20000 + i])
                    );
                }
            }

            Assert.Equal(0, (double)computer.MEMC.RAM.Data[19999]);
            Assert.Equal(0, (double)computer.MEMC.RAM.Data[70000]);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MEMF_nnn_rrr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                LD DEF, 50000   ; Length
                MEMF 20000, DEF, 0xAA  ; 20000 is the target, 0xAA is the value
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            for (int i = 0; i < 50000; i++)
            {
                if (computer.MEMC.RAM.Data[20000 + i] != 0xAA)
                {
                    Assert.False(true,
                        string.Format("Bad memory value. Expected 0xAA but got {0}", computer.MEMC.RAM.Data[20000 + i])
                    );
                }
            }

            Assert.Equal(0, (double)computer.MEMC.RAM.Data[19999]);
            Assert.Equal(0, (double)computer.MEMC.RAM.Data[70000]);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MEMF_rrr_nnn_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                LD ABC, 20000   ; Target
                MEMF ABC, 50000, 0xAA  ; 50000 is the length, 0xAA is the value
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            for (int i = 0; i < 50000; i++)
            {
                if (computer.MEMC.RAM.Data[20000 + i] != 0xAA)
                {
                    Assert.False(true,
                        string.Format("Bad memory value. Expected 0xAA but got {0}", computer.MEMC.RAM.Data[20000 + i])
                    );
                }
            }

            Assert.Equal(0, (double)computer.MEMC.RAM.Data[19999]);
            Assert.Equal(0, (double)computer.MEMC.RAM.Data[70000]);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MEMF_nnn_nnn_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                MEMF 20000, 50000, 0xAA  ; 20000 is the target, 50000 is the length, 0xAA is the value
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            for (int i = 0; i < 50000; i++)
            {
                if (computer.MEMC.RAM.Data[20000 + i] != 0xAA)
                {
                    Assert.False(true,
                        string.Format("Bad memory value. Expected 0xAA but got {0}", computer.MEMC.RAM.Data[20000 + i])
                    );
                }
            }

            Assert.Equal(0, (double)computer.MEMC.RAM.Data[19999]);
            Assert.Equal(0, (double)computer.MEMC.RAM.Data[70000]);

            TUtils.IncrementCountedTests("exec");
        }
    }
}
