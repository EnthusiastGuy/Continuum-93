using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;
using Continuum93.Tools;

namespace ExecutionTests
{

    public class TestEXEC_PUSH
    {
        [Fact]
        public void TestEXEC_PUSH_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD A, 0x44",
                    "PUSH A",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            byte stackVal = computer.MEMC.Get8BitFromRegStack(computer.CPU.REGS.SPR - 1);
            Assert.Equal(1, (double)computer.CPU.REGS.SPR);
            Assert.Equal(0x44, stackVal);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_PUSH_rr()
        {
            Assembler cp = new();
            using Computer computer = new();


            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD AB, 0xABCD",
                    "PUSH AB",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            ushort stackVal = computer.MEMC.Get16BitFromRegStack(computer.CPU.REGS.SPR - 2);
            Assert.Equal(2, (double)computer.CPU.REGS.SPR);
            Assert.Equal(0xABCD, stackVal);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_PUSH_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD ABC, 0xABCDEF",
                    "PUSH ABC",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint stackVal = computer.MEMC.Get24BitFromRegStack(computer.CPU.REGS.SPR - 3);
            Assert.Equal(3, (double)computer.CPU.REGS.SPR);
            Assert.Equal(0xABCDEF, (double)stackVal);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_PUSH_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD ABCD, 0xABCDEFAB",
                    "PUSH ABCD",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint stackVal = computer.MEMC.Get32BitFromRegStack(computer.CPU.REGS.SPR - 4);
            Assert.Equal(4, (double)computer.CPU.REGS.SPR);
            Assert.Equal(0xABCDEFAB, (double)stackVal);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_PUSH_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD ABCD, 0x01020304",
                    "LD EFGH, 0x05060708",
                    "PUSH D,G",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint stackVal = computer.MEMC.Get32BitFromRegStack(computer.CPU.REGS.SPR - 4);
            Assert.Equal(4, (double)computer.CPU.REGS.SPR);
            Assert.Equal(0x04050607, (double)stackVal);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_PUSH_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 1.125",
                    "PUSH F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint stackVal = computer.MEMC.Get32BitFromRegStack(computer.CPU.REGS.SPR - 4);
            float floatVal = FloatPointUtils.UintToFloat(stackVal);
            Assert.Equal(4, (double)computer.CPU.REGS.SPR);
            Assert.Equal(1.125f, floatVal);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_PUSH_fr_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 1.0",
                    "LD F1, 2.0",
                    "LD F2, 3.0",
                    "LD F3, 4.0",
                    "LD F4, 5.0",
                    "LD F5, 6.0",
                    "PUSH F0, F5",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(24, (double)computer.CPU.REGS.SPR);

            for (int i = 0; i < 6; i++)
            {
                uint stackVal = computer.MEMC.Get32BitFromRegStack((uint)(computer.CPU.REGS.SPR - 4 * (i + 1)));
                float floatVal = FloatPointUtils.UintToFloat(stackVal);
                Assert.Equal(6 - i, floatVal);
            }

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_PUSH_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(@"
                LD (0x80000), 0x12345678, 4;
                PUSH (0x80000)
                BREAK"
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            byte stackVal = computer.MEMC.Get8BitFromRegStack(computer.CPU.REGS.SPR - 1);
            Assert.Equal(1, (double)computer.CPU.REGS.SPR);
            Assert.Equal(0x12, stackVal);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_PUSH_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(@"
                LD ABC, 0x80000
                LD (0x80000), 0x12345678, 4;
                PUSH (ABC)
                BREAK"
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            byte stackVal = computer.MEMC.Get8BitFromRegStack(computer.CPU.REGS.SPR - 1);
            Assert.Equal(1, (double)computer.CPU.REGS.SPR);
            Assert.Equal(0x12, stackVal);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_PUSH16_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(@"
                LD (0x80000), 0x12345678, 4;
                PUSH16 (0x80000)
                BREAK"
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            ushort stackVal = computer.MEMC.Get16BitFromRegStack(computer.CPU.REGS.SPR - 2);
            Assert.Equal(2, (double)computer.CPU.REGS.SPR);
            Assert.Equal(0x1234, stackVal);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_PUSH16_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(@"
                LD ABC, 0x80000
                LD (0x80000), 0x12345678, 4;
                PUSH16 (ABC)
                BREAK"
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            ushort stackVal = computer.MEMC.Get16BitFromRegStack(computer.CPU.REGS.SPR - 2);
            Assert.Equal(2, (double)computer.CPU.REGS.SPR);
            Assert.Equal(0x1234, stackVal);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_PUSH24_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(@"
                LD (0x80000), 0x12345678, 4;
                PUSH24 (0x80000)
                BREAK"
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint stackVal = computer.MEMC.Get24BitFromRegStack(computer.CPU.REGS.SPR - 3);
            Assert.Equal(3, (double)computer.CPU.REGS.SPR);
            Assert.Equal((long)0x123456, stackVal);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_PUSH24_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(@"
                LD ABC, 0x80000
                LD (0x80000), 0x12345678, 4;
                PUSH24 (ABC)
                BREAK"
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint stackVal = computer.MEMC.Get24BitFromRegStack(computer.CPU.REGS.SPR - 3);
            Assert.Equal(3, (double)computer.CPU.REGS.SPR);
            Assert.Equal((long)0x123456, stackVal);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_PUSH32_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(@"
                LD (0x80000), 0x12345678, 4;
                PUSH32 (0x80000)
                BREAK"
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint stackVal = computer.MEMC.Get32BitFromRegStack(computer.CPU.REGS.SPR - 4);
            Assert.Equal(4, (double)computer.CPU.REGS.SPR);
            Assert.Equal((long)0x12345678, stackVal);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_PUSH32_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(@"
                LD ABC, 0x80000
                LD (0x80000), 0x12345678, 4;
                PUSH32 (ABC)
                BREAK"
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint stackVal = computer.MEMC.Get32BitFromRegStack(computer.CPU.REGS.SPR - 4);
            Assert.Equal(4, (double)computer.CPU.REGS.SPR);
            Assert.Equal((long)0x12345678, stackVal);

            TUtils.IncrementCountedTests("exec");
        }
    }
}
