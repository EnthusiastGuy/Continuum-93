using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{
    /*
     * Overlap tests will not be a part of this test suite since we are using Array.Copy(). As per the documentation
     * provided by Microsoft
     * https://learn.microsoft.com/en-us/dotnet/api/system.array.copy?redirectedfrom=MSDN&view=net-7.0#System_Array_Copy_System_Array_System_Int32_System_Array_System_Int32_System_Int32_
     * 
     * "If sourceArray and destinationArray overlap, this method behaves as if the original values of
     * sourceArray were preserved in a temporary location before destinationArray is overwritten."
     */


    public class TestEXEC_MEMC
    {
        [Fact]
        public void TestEXEC_MEMC_rrr_rrr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            byte[] randomData = TUtils.GetRandomByteArray(1000);
            computer.LoadMemAt(20000, randomData);

            cp.Build(@"
                LD ABC, 20000   ; Source
                LD DEF, 50000   ; Target
                LD GHI, 1000    ; Length
                MEMC ABC, DEF, GHI
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            for (int i = 0; i < 1000; i++)
            {
                if (computer.MEMC.RAM.Data[50000 + i] != randomData[i])
                {
                    Assert.False(true,
                        string.Format(
                            "Bad memory value. Expected {0} but got {1}",
                            randomData[i],
                            computer.MEMC.RAM.Data[50000 + i])
                    );
                }
            }

            Assert.Equal(0, (double)computer.MEMC.RAM.Data[49999]);
            Assert.Equal(0, (double)computer.MEMC.RAM.Data[51000]);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MEMC_nnn_rrr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            byte[] randomData = TUtils.GetRandomByteArray(1000);
            computer.LoadMemAt(20000, randomData);

            cp.Build(@"
                LD DEF, 50000   ; Target
                LD GHI, 1000    ; Length
                MEMC 20000, DEF, GHI    ; 20000 is the source
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            for (int i = 0; i < 1000; i++)
            {
                if (computer.MEMC.RAM.Data[50000 + i] != randomData[i])
                {
                    Assert.False(true,
                        string.Format(
                            "Bad memory value. Expected {0} but got {1}",
                            randomData[i],
                            computer.MEMC.RAM.Data[50000 + i])
                    );
                }
            }

            Assert.Equal(0, (double)computer.MEMC.RAM.Data[49999]);
            Assert.Equal(0, (double)computer.MEMC.RAM.Data[51000]);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MEMC_rrr_nnn_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            byte[] randomData = TUtils.GetRandomByteArray(1000);
            computer.LoadMemAt(20000, randomData);

            cp.Build(@"
                LD ABC, 20000   ; Source
                LD GHI, 1000    ; Length
                MEMC ABC, 50000, GHI    ; 50000 is the target
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            for (int i = 0; i < 1000; i++)
            {
                if (computer.MEMC.RAM.Data[50000 + i] != randomData[i])
                {
                    Assert.False(true,
                        string.Format(
                            "Bad memory value. Expected {0} but got {1}",
                            randomData[i],
                            computer.MEMC.RAM.Data[50000 + i])
                    );
                }
            }

            Assert.Equal(0, (double)computer.MEMC.RAM.Data[49999]);
            Assert.Equal(0, (double)computer.MEMC.RAM.Data[51000]);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MEMC_nnn_nnn_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            byte[] randomData = TUtils.GetRandomByteArray(1000);
            computer.LoadMemAt(20000, randomData);

            cp.Build(@"
                LD GHI, 1000    ; Length
                MEMC 20000, 50000, GHI    ; 20000 is the source, 50000 is the target
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            for (int i = 0; i < 1000; i++)
            {
                if (computer.MEMC.RAM.Data[50000 + i] != randomData[i])
                {
                    Assert.False(true,
                        string.Format(
                            "Bad memory value. Expected {0} but got {1}",
                            randomData[i],
                            computer.MEMC.RAM.Data[50000 + i])
                    );
                }
            }

            Assert.Equal(0, (double)computer.MEMC.RAM.Data[49999]);
            Assert.Equal(0, (double)computer.MEMC.RAM.Data[51000]);

            TUtils.IncrementCountedTests("exec");
        }


        [Fact]
        public void TestEXEC_MEMC_rrr_rrr_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();


            byte[] randomData = TUtils.GetRandomByteArray(1000);
            computer.LoadMemAt(20000, randomData);

            cp.Build(@"
                LD ABC, 20000   ; Source
                LD DEF, 50000   ; Target
                MEMC ABC, DEF, 1000     ; 1000 is the length
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            for (int i = 0; i < 1000; i++)
            {
                if (computer.MEMC.RAM.Data[50000 + i] != randomData[i])
                {
                    Assert.False(true,
                        string.Format(
                            "Bad memory value. Expected {0} but got {1}",
                            randomData[i],
                            computer.MEMC.RAM.Data[50000 + i])
                    );
                }
            }

            Assert.Equal(0, (double)computer.MEMC.RAM.Data[49999]);
            Assert.Equal(0, (double)computer.MEMC.RAM.Data[51000]);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MEMC_nnn_rrr_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();


            byte[] randomData = TUtils.GetRandomByteArray(1000);
            computer.LoadMemAt(20000, randomData);

            cp.Build(@"
                LD DEF, 50000   ; Target
                MEMC 20000, DEF, 1000    ; 20000 is the source, 1000 is the length
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            for (int i = 0; i < 1000; i++)
            {
                if (computer.MEMC.RAM.Data[50000 + i] != randomData[i])
                {
                    Assert.False(true,
                        string.Format(
                            "Bad memory value. Expected {0} but got {1}",
                            randomData[i],
                            computer.MEMC.RAM.Data[50000 + i])
                    );
                }
            }

            Assert.Equal(0, (double)computer.MEMC.RAM.Data[49999]);
            Assert.Equal(0, (double)computer.MEMC.RAM.Data[51000]);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MEMC_rrr_nnn_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();


            byte[] randomData = TUtils.GetRandomByteArray(1000);
            computer.LoadMemAt(20000, randomData);

            cp.Build(@"
                LD ABC, 20000   ; Source
                MEMC ABC, 50000, 1000    ; 50000 is the target, 1000 is the length
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            for (int i = 0; i < 1000; i++)
            {
                if (computer.MEMC.RAM.Data[50000 + i] != randomData[i])
                {
                    Assert.False(true,
                        string.Format(
                            "Bad memory value. Expected {0} but got {1}",
                            randomData[i],
                            computer.MEMC.RAM.Data[50000 + i])
                    );
                }
            }

            Assert.Equal(0, (double)computer.MEMC.RAM.Data[49999]);
            Assert.Equal(0, (double)computer.MEMC.RAM.Data[51000]);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_MEMC_nnn_nnn_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();


            byte[] randomData = TUtils.GetRandomByteArray(1000);
            computer.LoadMemAt(20000, randomData);

            cp.Build(@"
                MEMC 20000, 50000, 1000    ; 20000 is the source, 50000 is the target, 1000 is the length
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            for (int i = 0; i < 1000; i++)
            {
                if (computer.MEMC.RAM.Data[50000 + i] != randomData[i])
                {
                    Assert.False(true,
                        string.Format(
                            "Bad memory value. Expected {0} but got {1}",
                            randomData[i],
                            computer.MEMC.RAM.Data[50000 + i])
                    );
                }
            }

            Assert.Equal(0, (double)computer.MEMC.RAM.Data[49999]);
            Assert.Equal(0, (double)computer.MEMC.RAM.Data[51000]);

            TUtils.IncrementCountedTests("exec");
        }
    }
}
