using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_INC
    {
        [Fact]
        public void TestEXEC_INC_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 12;
            computer.CPU.REGS.B = 255;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "INC A",
                    "INC A",
                    "INC B",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(14, computer.CPU.REGS.A);
            Assert.Equal(0, computer.CPU.REGS.B);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_INC_rr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.AB = 1200;
            computer.CPU.REGS.CD = 0xFFFF;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "INC AB",
                    "INC AB",
                    "INC CD",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1202, computer.CPU.REGS.AB);
            Assert.Equal(0, computer.CPU.REGS.CD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_INC_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABC = 0xA00000;
            computer.CPU.REGS.DEF = 0xFFFFFF;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "INC ABC",
                    "INC ABC",
                    "INC DEF",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xA00002, (double)computer.CPU.REGS.ABC);
            Assert.Equal(0, (double)computer.CPU.REGS.DEF);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_INC_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.ABCD = 0xA0000000;
            computer.CPU.REGS.EFGH = 0xFFFFFFFF;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "INC ABCD",
                    "INC ABCD",
                    "INC EFGH",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xA0000002, (double)computer.CPU.REGS.ABCD);
            Assert.Equal(0, (double)computer.CPU.REGS.EFGH);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_INC_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint address1 = 5000;
            uint address2 = 5010;
            computer.MEMC.Set8bitToRAM(address1, 12);
            computer.MEMC.Set8bitToRAM(address2, 0xFF);

            computer.CPU.REGS.ABC = address1;
            computer.CPU.REGS.DEF = address2;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "INC (ABC)",
                    "INC (ABC)",
                    "INC (DEF)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(14, computer.MEMC.Get8bitFromRAM(address1));
            Assert.Equal(0, computer.MEMC.Get8bitFromRAM(address2));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_INC16_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint address1 = 5000;
            uint address2 = 5010;
            computer.MEMC.Set16bitToRAM(address1, 1200);
            computer.MEMC.Set16bitToRAM(address2, 0xFFFF);

            computer.CPU.REGS.ABC = address1;
            computer.CPU.REGS.DEF = address2;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "INC16 (ABC)",
                    "INC16 (ABC)",
                    "INC16 (DEF)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1202, computer.MEMC.Get16bitFromRAM(address1));
            Assert.Equal(0, computer.MEMC.Get16bitFromRAM(address2));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_INC24_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint address1 = 5000;
            uint address2 = 5010;
            computer.MEMC.Set24bitToRAM(address1, 0xA00000);
            computer.MEMC.Set24bitToRAM(address2, 0xFFFFFF);

            computer.CPU.REGS.ABC = address1;
            computer.CPU.REGS.DEF = address2;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "INC24 (ABC)",
                    "INC24 (ABC)",
                    "INC24 (DEF)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xA00002, (double)computer.MEMC.Get24bitFromRAM(address1));
            Assert.Equal(0, (double)computer.MEMC.Get24bitFromRAM(address2));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_INC32_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint address1 = 5000;
            uint address2 = 5010;
            computer.MEMC.Set32bitToRAM(address1, 0xA0000000);
            computer.MEMC.Set32bitToRAM(address2, 0xFFFFFFFF);

            computer.CPU.REGS.ABC = address1;
            computer.CPU.REGS.DEF = address2;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "INC32 (ABC)",
                    "INC32 (ABC)",
                    "INC32 (DEF)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xA0000002, (double)computer.MEMC.Get32bitFromRAM(address1));
            Assert.Equal(0, (double)computer.MEMC.Get32bitFromRAM(address2));
            TUtils.IncrementCountedTests("exec");
        }

        // new
        [Fact]
        public void TestEXEC_INC_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint address1 = 5000;
            uint address2 = 5010;
            computer.MEMC.Set8bitToRAM(address1, 12);
            computer.MEMC.Set8bitToRAM(address2, 0xFF);

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"INC ({address1})",
                    $"INC ({address1})",
                    $"INC ({address2})",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(14, computer.MEMC.Get8bitFromRAM(address1));
            Assert.Equal(0, computer.MEMC.Get8bitFromRAM(address2));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_INC16_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint address1 = 5000;
            uint address2 = 5010;
            computer.MEMC.Set16bitToRAM(address1, 1200);
            computer.MEMC.Set16bitToRAM(address2, 0xFFFF);

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"INC16 ({address1})",
                    $"INC16 ({address1})",
                    $"INC16 ({address2})",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1202, computer.MEMC.Get16bitFromRAM(address1));
            Assert.Equal(0, computer.MEMC.Get16bitFromRAM(address2));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_INC24_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint address1 = 5000;
            uint address2 = 5010;
            computer.MEMC.Set24bitToRAM(address1, 0xA00000);
            computer.MEMC.Set24bitToRAM(address2, 0xFFFFFF);

            computer.CPU.REGS.ABC = address1;
            computer.CPU.REGS.DEF = address2;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"INC24 ({address1})",
                    $"INC24 ({address1})",
                    $"INC24 ({address2})",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xA00002, (double)computer.MEMC.Get24bitFromRAM(address1));
            Assert.Equal(0, (double)computer.MEMC.Get24bitFromRAM(address2));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_INC32_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            uint address1 = 5000;
            uint address2 = 5010;
            computer.MEMC.Set32bitToRAM(address1, 0xA0000000);
            computer.MEMC.Set32bitToRAM(address2, 0xFFFFFFFF);

            computer.CPU.REGS.ABC = address1;
            computer.CPU.REGS.DEF = address2;

            cp.Build(
                TUtils.GetFormattedAsm(
                    $"INC32 ({address1})",
                    $"INC32 ({address1})",
                    $"INC32 ({address2})",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xA0000002, (double)computer.MEMC.Get32bitFromRAM(address1));
            Assert.Equal(0, (double)computer.MEMC.Get32bitFromRAM(address2));
            TUtils.IncrementCountedTests("exec");
        }
    }
}
