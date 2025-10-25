using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_POP
    {
        [Fact]
        public void TestEXEC_POP_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD A, 0x44",
                    "PUSH A",
                    "POP G",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, (double)computer.CPU.REGS.SPR);
            Assert.Equal(0x44, computer.CPU.REGS.G);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_POP_rr()
        {
            Assembler cp = new();
            using Computer computer = new();


            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD AB, 0xABCD",
                    "PUSH AB",
                    "POP GH",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, (double)computer.CPU.REGS.SPR);
            Assert.Equal(0xABCD, computer.CPU.REGS.GH);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_POP_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD ABC, 0xABCDEF",
                    "PUSH ABC",
                    "POP GHI",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, (double)computer.CPU.REGS.SPR);
            Assert.Equal(0xABCDEF, (double)computer.CPU.REGS.GHI);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_POP_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD ABCD, 0xABCDEFAB",
                    "PUSH ABCD",
                    "POP GHIJ",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, (double)computer.CPU.REGS.SPR);
            Assert.Equal(0xABCDEFAB, (double)computer.CPU.REGS.GHIJ);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_POP_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD G, 0x12",
                    "LD H, 0x34",
                    "LD I, 0x56",
                    "LD J, 0x78",
                    "PUSH G,J",
                    "POP A,D",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, (double)computer.CPU.REGS.SPR);
            Assert.Equal(0x12, (double)computer.CPU.REGS.A);
            Assert.Equal(0x34, (double)computer.CPU.REGS.B);
            Assert.Equal(0x56, (double)computer.CPU.REGS.C);
            Assert.Equal(0x78, (double)computer.CPU.REGS.D);
            Assert.Equal(0x00, (double)computer.CPU.REGS.E);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_POP_r_r_all()
        {
            Assembler cp = new();
            using Computer computer = new();


            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD G, 0x12",
                    "LD H, 0x34",
                    "LD I, 0x56",
                    "LD J, 0x78",
                    "PUSH A,Z",
                    "INC A",
                    "INC H",
                    "INC J",
                    "INC Z",
                    "POP A,Z",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, (double)computer.CPU.REGS.SPR);
            Assert.Equal(0x00, (double)computer.CPU.REGS.A);
            Assert.Equal(0x12, (double)computer.CPU.REGS.G);
            Assert.Equal(0x34, (double)computer.CPU.REGS.H);
            Assert.Equal(0x56, (double)computer.CPU.REGS.I);
            Assert.Equal(0x78, (double)computer.CPU.REGS.J);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_POP_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 3.14",
                    "LD F1, 6.2",
                    "PUSH F0",
                    "PUSH F1",
                    "POP F6",
                    "POP F5",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, (double)computer.CPU.REGS.SPR);
            Assert.Equal(3.14f, (double)computer.CPU.FREGS.GetRegister(0));
            Assert.Equal(6.2f, (double)computer.CPU.FREGS.GetRegister(1));
            Assert.Equal(3.14f, (double)computer.CPU.FREGS.GetRegister(5));
            Assert.Equal(6.2f, (double)computer.CPU.FREGS.GetRegister(6));

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_POP_fr_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "LD F0, 3.14",
                    "LD F1, 6.2",
                    "LD F2, 12.45",
                    "LD F3, 100.0",
                    "LD F4, -25.25",
                    "PUSH F0, F4",
                    "POP F10, F14",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, (double)computer.CPU.REGS.SPR);
            Assert.Equal(3.14f, (double)computer.CPU.FREGS.GetRegister(0));
            Assert.Equal(6.2f, (double)computer.CPU.FREGS.GetRegister(1));
            Assert.Equal(12.45f, (double)computer.CPU.FREGS.GetRegister(2));
            Assert.Equal(100.0f, (double)computer.CPU.FREGS.GetRegister(3));
            Assert.Equal(-25.25f, (double)computer.CPU.FREGS.GetRegister(4));

            Assert.Equal(3.14f, (double)computer.CPU.FREGS.GetRegister(10));
            Assert.Equal(6.2f, (double)computer.CPU.FREGS.GetRegister(11));
            Assert.Equal(12.45f, (double)computer.CPU.FREGS.GetRegister(12));
            Assert.Equal(100.0f, (double)computer.CPU.FREGS.GetRegister(13));
            Assert.Equal(-25.25f, (double)computer.CPU.FREGS.GetRegister(14));

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_POP_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(@"
                LD ABCD, 0x12345678
                PUSH ABCD
                POP (0x80000)
                BREAK"
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint memVal = computer.MEMC.Get32bitFromRAM(0x80000);
            Assert.Equal(3, (double)computer.CPU.REGS.SPR);
            Assert.Equal((long)0x78000000, memVal);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_POP_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(@"
                LD ABCD, 0x12345678
                PUSH ABCD
                LD EFG, 0x80000
                POP (EFG)
                BREAK"
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint memVal = computer.MEMC.Get32bitFromRAM(0x80000);
            Assert.Equal(3, (double)computer.CPU.REGS.SPR);
            Assert.Equal((long)0x78000000, memVal);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_POP16_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(@"
                LD ABCD, 0x12345678
                PUSH ABCD
                POP16 (0x80000)
                BREAK"
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint memVal = computer.MEMC.Get32bitFromRAM(0x80000);
            Assert.Equal(2, (double)computer.CPU.REGS.SPR);
            Assert.Equal((long)0x56780000, memVal);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_POP16_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(@"
                LD ABCD, 0x12345678
                PUSH ABCD
                LD EFG, 0x80000
                POP16 (EFG)
                BREAK"
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint memVal = computer.MEMC.Get32bitFromRAM(0x80000);
            Assert.Equal(2, (double)computer.CPU.REGS.SPR);
            Assert.Equal((long)0x56780000, memVal);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_POP24_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(@"
                LD ABCD, 0x12345678
                PUSH ABCD
                POP24 (0x80000)
                BREAK"
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint memVal = computer.MEMC.Get32bitFromRAM(0x80000);
            Assert.Equal(1, (double)computer.CPU.REGS.SPR);
            Assert.Equal((long)0x34567800, memVal);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_POP24_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(@"
                LD ABCD, 0x12345678
                PUSH ABCD
                LD EFG, 0x80000
                POP24 (EFG)
                BREAK"
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint memVal = computer.MEMC.Get32bitFromRAM(0x80000);
            Assert.Equal(1, (double)computer.CPU.REGS.SPR);
            Assert.Equal((long)0x34567800, memVal);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_POP32_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(@"
                LD ABCD, 0x12345678
                PUSH ABCD
                POP32 (0x80000)
                BREAK"
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint memVal = computer.MEMC.Get32bitFromRAM(0x80000);
            Assert.Equal(0, (double)computer.CPU.REGS.SPR);
            Assert.Equal((long)0x12345678, memVal);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_POP32_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(@"
                LD ABCD, 0x12345678
                PUSH ABCD
                LD EFG, 0x80000
                POP32 (EFG)
                BREAK"
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint memVal = computer.MEMC.Get32bitFromRAM(0x80000);
            Assert.Equal(0, (double)computer.CPU.REGS.SPR);
            Assert.Equal((long)0x12345678, memVal);

            TUtils.IncrementCountedTests("exec");
        }
    }
}
