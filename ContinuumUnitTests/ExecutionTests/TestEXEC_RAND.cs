using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_RAND
    {
        [Fact]
        public void TestEXEC_RAND_r()
        {
            Assembler cp = new();
            using Computer computer = new();

            int total = 0;

            for (int i = 0; i < 20; i++)
            {


                Assert.Equal(0, (double)computer.CPU.REGS.SPR);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        "LD A, 0x05",
                        "RAND A",
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                total += computer.CPU.REGS.A;

                Assert.True(computer.CPU.REGS.A < 0x05);

                TUtils.IncrementCountedTests("exec");
            }

            Assert.True(total < 100);

        }

        [Fact]
        public void TestEXEC_RAND_r_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            int total = 0;

            for (int i = 0; i < 20; i++)
            {
                Assert.Equal(0, (double)computer.CPU.REGS.SPR);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        "LD A, 0x0",
                        "RAND A",
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                total += computer.CPU.REGS.A;
            }

            Assert.True(total > 20);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RAND_rr()
        {
            Assembler cp = new();
            using Computer computer = new();

            int total = 0;

            for (int i = 0; i < 20; i++)
            {


                Assert.Equal(0, (double)computer.CPU.REGS.SPR);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        "LD AB, 0x05",
                        "RAND AB",
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                total += computer.CPU.REGS.AB;

                Assert.True(computer.CPU.REGS.AB < 0x05);

                TUtils.IncrementCountedTests("exec");
            }

            Assert.True(total < 100);

        }

        [Fact]
        public void TestEXEC_RAND_rr_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            int total = 0;

            for (int i = 0; i < 20; i++)
            {


                Assert.Equal(0, (double)computer.CPU.REGS.SPR);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        "LD AB, 0x0",
                        "RAND AB",
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                total += computer.CPU.REGS.AB;


            }

            Assert.True(total > 100);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RAND_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint total = 0;

            for (int i = 0; i < 20; i++)
            {


                Assert.Equal(0, (double)computer.CPU.REGS.SPR);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        "LD ABC, 0x05",
                        "RAND ABC",
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                total += computer.CPU.REGS.ABC;

                Assert.True(computer.CPU.REGS.ABC < 0x05);

                TUtils.IncrementCountedTests("exec");
            }

            Assert.True(total < 100);

        }

        [Fact]
        public void TestEXEC_RAND_rrr_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint total = 0;

            for (int i = 0; i < 20; i++)
            {


                Assert.Equal(0, (double)computer.CPU.REGS.SPR);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        "LD ABC, 0x0",
                        "RAND ABC",
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                total += computer.CPU.REGS.ABC;


            }

            Assert.True(total > 100);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RAND_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint total = 0;

            for (int i = 0; i < 20; i++)
            {


                Assert.Equal(0, (double)computer.CPU.REGS.SPR);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        "LD ABCD, 0x05",
                        "RAND ABCD",
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                total += computer.CPU.REGS.ABCD;

                Assert.True(computer.CPU.REGS.ABCD < 0x05);

                TUtils.IncrementCountedTests("exec");
            }

            Assert.True(total < 100);

        }

        [Fact]
        public void TestEXEC_RAND_rrrr_zero()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint total = 0;

            for (int i = 0; i < 20; i++)
            {


                Assert.Equal(0, (double)computer.CPU.REGS.SPR);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        "LD ABCD, 0x0",
                        "RAND ABCD",
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                total += computer.CPU.REGS.ABCD;


            }

            Assert.True(total > 100);
            TUtils.IncrementCountedTests("exec");
        }

        // RAND [r, rr, rrr, rrrr], [n, nn, nnn, nnnn]
        [Fact]
        public void TestEXEC_RAND_r_n()
        {
            Assembler cp = new();
            using Computer computer = new();

            int total = 0;

            for (int i = 0; i < 20; i++)
            {


                Assert.Equal(0, (double)computer.CPU.REGS.SPR);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        "RAND A, 0x05",
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                total += computer.CPU.REGS.A;

                Assert.True(computer.CPU.REGS.A < 0x05);

                TUtils.IncrementCountedTests("exec");
            }

            Assert.True(total < 100);

        }

        [Fact]
        public void TestEXEC_RAND_rr_nn()
        {
            Assembler cp = new();
            using Computer computer = new();

            int total = 0;

            for (int i = 0; i < 20; i++)
            {


                Assert.Equal(0, (double)computer.CPU.REGS.SPR);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        "RAND AB, 1000",
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                total += computer.CPU.REGS.AB;

                Assert.True(computer.CPU.REGS.AB < 1000);

                TUtils.IncrementCountedTests("exec");
            }

            Assert.True(total < 20000);
            Assert.True(total > 100);

        }

        [Fact]
        public void TestEXEC_RAND_rrr_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint total = 0;

            for (int i = 0; i < 20; i++)
            {


                Assert.Equal(0, (double)computer.CPU.REGS.SPR);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        "RAND ABC, 100000",
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                total += computer.CPU.REGS.ABC;

                Assert.True(computer.CPU.REGS.ABC < 100000);

                TUtils.IncrementCountedTests("exec");
            }

            Assert.True(total < 2000000);
            Assert.True(total > 1000);

        }

        [Fact]
        public void TestEXEC_RAND_rrrr_nnnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint total = 0;

            for (int i = 0; i < 20; i++)
            {


                Assert.Equal(0, (double)computer.CPU.REGS.SPR);

                cp.Build(
                    TUtils.GetFormattedAsm(
                        "RAND ABCD, 1000000",
                        "BREAK"
                    )
                );

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                total += computer.CPU.REGS.ABCD;

                Assert.True(computer.CPU.REGS.ABCD < 1000000);

                TUtils.IncrementCountedTests("exec");
            }

            Assert.True(total < 20000000);
            Assert.True(total > 10000);

        }

        // Floating point
        [Fact]
        public void TestEXEC_RAND_fr()
        {
            Assembler cp = new();
            using Computer computer = new();

            float total = 0;

            for (int i = 0; i < 2000; i++)
            {
                cp.Build(@"
                    RAND F0
                    BREAK
                ");

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMem(compiled);
                computer.Run();

                total += computer.CPU.FREGS.GetRegister(0);

                TUtils.IncrementCountedTests("exec");
            }

            Assert.True(total > 1.0f);
        }

        [Fact]
        public void TestEXEC_RAND_fr_nnnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                RAND F0, 0x043A7FD1
                RAND F1
                RAND F2
                RAND F3
                RAND F4, 0x043A7FD1
                RAND F5
                RAND F6
                RAND F7
                RAND F8, 0x70515D23
                RAND F9
                RAND F10
                RAND F11
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            float f0 = computer.CPU.FREGS.GetRegister(0);
            float f1 = computer.CPU.FREGS.GetRegister(1);
            float f2 = computer.CPU.FREGS.GetRegister(2);
            float f3 = computer.CPU.FREGS.GetRegister(3);
            float f4 = computer.CPU.FREGS.GetRegister(4);
            float f5 = computer.CPU.FREGS.GetRegister(5);
            float f6 = computer.CPU.FREGS.GetRegister(6);
            float f7 = computer.CPU.FREGS.GetRegister(7);
            float f8 = computer.CPU.FREGS.GetRegister(8);
            float f9 = computer.CPU.FREGS.GetRegister(9);
            float f10 = computer.CPU.FREGS.GetRegister(10);
            float f11 = computer.CPU.FREGS.GetRegister(11);

            TUtils.IncrementCountedTests("exec");

            Assert.Equal(f4, f0);
            Assert.Equal(f5, f1);
            Assert.Equal(f6, f2);
            Assert.Equal(f7, f3);

            Assert.NotEqual(f8, f0);
            Assert.NotEqual(f9, f1);
            Assert.NotEqual(f10, f2);
            Assert.NotEqual(f11, f3);

            Assert.Equal(0.957685173f, f0);
            Assert.Equal(0.857129037f, f1);
            Assert.Equal(0.207400933f, f2);
            Assert.Equal(0.586846113f, f3);

            Assert.Equal(0.891778231f, f8);
            Assert.Equal(0.763478816f, f9);
            Assert.Equal(0.507753193f, f10);
            Assert.Equal(0.357524365f, f11);
        }

        [Fact]
        public void TestEXEC_RAND_fr_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD ABCD, 0x043A7FD1
                LD EFGH, 0x70515D23
                RAND F0, ABCD
                RAND F1
                RAND F2
                RAND F3
                RAND F4, ABCD
                RAND F5
                RAND F6
                RAND F7
                RAND F8, EFGH
                RAND F9
                RAND F10
                RAND F11
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            float f0 = computer.CPU.FREGS.GetRegister(0);
            float f1 = computer.CPU.FREGS.GetRegister(1);
            float f2 = computer.CPU.FREGS.GetRegister(2);
            float f3 = computer.CPU.FREGS.GetRegister(3);
            float f4 = computer.CPU.FREGS.GetRegister(4);
            float f5 = computer.CPU.FREGS.GetRegister(5);
            float f6 = computer.CPU.FREGS.GetRegister(6);
            float f7 = computer.CPU.FREGS.GetRegister(7);
            float f8 = computer.CPU.FREGS.GetRegister(8);
            float f9 = computer.CPU.FREGS.GetRegister(9);
            float f10 = computer.CPU.FREGS.GetRegister(10);
            float f11 = computer.CPU.FREGS.GetRegister(11);

            TUtils.IncrementCountedTests("exec");

            Assert.Equal(f4, f0);
            Assert.Equal(f5, f1);
            Assert.Equal(f6, f2);
            Assert.Equal(f7, f3);

            Assert.NotEqual(f8, f0);
            Assert.NotEqual(f9, f1);
            Assert.NotEqual(f10, f2);
            Assert.NotEqual(f11, f3);

            Assert.Equal(0.957685173f, f0);
            Assert.Equal(0.857129037f, f1);
            Assert.Equal(0.207400933f, f2);
            Assert.Equal(0.586846113f, f3);

            Assert.Equal(0.891778231f, f8);
            Assert.Equal(0.763478816f, f9);
            Assert.Equal(0.507753193f, f10);
            Assert.Equal(0.357524365f, f11);
        }
    }
}
