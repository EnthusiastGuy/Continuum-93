using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_DEC
    {
        [Fact]
        public void TestEXEC_DEC_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 12;
            computer.CPU.REGS.B = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DEC A",
                    "DEC A",
                    "DEC B",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(10, computer.CPU.REGS.A);
            Assert.Equal(255, computer.CPU.REGS.B);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DEC_rr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 1200;
            computer.CPU.REGS.CD = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DEC AB",
                    "DEC AB",
                    "DEC CD",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1198, computer.CPU.REGS.AB);
            Assert.Equal(0xFFFF, computer.CPU.REGS.CD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DEC_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 0xA00000;
            computer.CPU.REGS.DEF = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DEC ABC",
                    "DEC ABC",
                    "DEC DEF",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x9FFFFE, (double)computer.CPU.REGS.ABC);
            Assert.Equal(0xFFFFFF, (double)computer.CPU.REGS.DEF);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DEC_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0xA0000000;
            computer.CPU.REGS.EFGH = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DEC ABCD",
                    "DEC ABCD",
                    "DEC EFGH",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x9FFFFFFE, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0xFFFFFFFF, (double)computer.CPU.REGS.EFGH);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DEC_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint address1 = 5000;
            uint address2 = 5010;
            computer.MEMC.Set8bitToRAM(address1, 12);
            computer.MEMC.Set8bitToRAM(address2, 0);

            computer.CPU.REGS.ABC = address1;
            computer.CPU.REGS.DEF = address2;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DEC (ABC)",
                    "DEC (ABC)",
                    "DEC (DEF)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(10, computer.MEMC.Get8bitFromRAM(address1));
            Assert.Equal(0xFF, computer.MEMC.Get8bitFromRAM(address2));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DEC16_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint address1 = 5000;
            uint address2 = 5010;
            computer.MEMC.Set16bitToRAM(address1, 1200);
            computer.MEMC.Set16bitToRAM(address2, 0);

            computer.CPU.REGS.ABC = address1;
            computer.CPU.REGS.DEF = address2;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DEC16 (ABC)",
                    "DEC16 (ABC)",
                    "DEC16 (DEF)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1198, computer.MEMC.Get16bitFromRAM(address1));
            Assert.Equal(0xFFFF, computer.MEMC.Get16bitFromRAM(address2));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DEC24_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint address1 = 5000;
            uint address2 = 5010;
            computer.MEMC.Set24bitToRAM(address1, 0xA00000);
            computer.MEMC.Set24bitToRAM(address2, 0);

            computer.CPU.REGS.ABC = address1;
            computer.CPU.REGS.DEF = address2;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DEC24 (ABC)",
                    "DEC24 (ABC)",
                    "DEC24 (DEF)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x9FFFFE, (double)computer.MEMC.Get24bitFromRAM(address1));
            Assert.Equal(0xFFFFFF, (double)computer.MEMC.Get24bitFromRAM(address2));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DEC32_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint address1 = 5000;
            uint address2 = 5010;
            computer.MEMC.Set32bitToRAM(address1, 0xA0000000);
            computer.MEMC.Set32bitToRAM(address2, 0);

            computer.CPU.REGS.ABC = address1;
            computer.CPU.REGS.DEF = address2;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "DEC32 (ABC)",
                    "DEC32 (ABC)",
                    "DEC32 (DEF)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x9FFFFFFE, (double)computer.MEMC.Get32bitFromRAM(address1));
            Assert.Equal(0xFFFFFFFF, (double)computer.MEMC.Get32bitFromRAM(address2));
            TUtils.IncrementCountedTests("exec");
        }


        [Fact]
        public void TestEXEC_DEC_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint address1 = 5000;
            uint address2 = 5010;
            computer.MEMC.Set8bitToRAM(address1, 12);
            computer.MEMC.Set8bitToRAM(address2, 0);

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"DEC ({address1})",
                    $"DEC ({address1})",
                    $"DEC ({address2})",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(10, computer.MEMC.Get8bitFromRAM(address1));
            Assert.Equal(0xFF, computer.MEMC.Get8bitFromRAM(address2));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DEC16_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint address1 = 5000;
            uint address2 = 5010;
            computer.MEMC.Set16bitToRAM(address1, 1200);
            computer.MEMC.Set16bitToRAM(address2, 0);

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"DEC16 ({address1})",
                    $"DEC16 ({address1})",
                    $"DEC16 ({address2})",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1198, computer.MEMC.Get16bitFromRAM(address1));
            Assert.Equal(0xFFFF, computer.MEMC.Get16bitFromRAM(address2));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DEC24_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint address1 = 5000;
            uint address2 = 5010;
            computer.MEMC.Set24bitToRAM(address1, 0xA00000);
            computer.MEMC.Set24bitToRAM(address2, 0);

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"DEC24 ({address1})",
                    $"DEC24 ({address1})",
                    $"DEC24 ({address2})",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x9FFFFE, (double)computer.MEMC.Get24bitFromRAM(address1));
            Assert.Equal(0xFFFFFF, (double)computer.MEMC.Get24bitFromRAM(address2));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_DEC32_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint address1 = 5000;
            uint address2 = 5010;
            computer.MEMC.Set32bitToRAM(address1, 0xA0000000);
            computer.MEMC.Set32bitToRAM(address2, 0);

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"DEC32 ({address1})",
                    $"DEC32 ({address1})",
                    $"DEC32 ({address2})",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x9FFFFFFE, (double)computer.MEMC.Get32bitFromRAM(address1));
            Assert.Equal(0xFFFFFFFF, (double)computer.MEMC.Get32bitFromRAM(address2));
            TUtils.IncrementCountedTests("exec");
        }
    }
}
