using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_POW
    {
        [Fact]
        public void TestEXEC_POW_fr_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 2.0f);
            computer.CPU.FREGS.SetRegister(1, 3.0f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "POW F0, F1",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(8.0f, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal(3.0f, computer.CPU.FREGS.GetRegister(1));

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_POW_fr_r()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 2.0f);
            computer.CPU.REGS.A = 3;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "POW F0, A",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(8.0f, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal(3, computer.CPU.REGS.A);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_POW_fr_rr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 1.01f);
            computer.CPU.REGS.AB = 300;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "POW F0, AB",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(19.7884102f, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal(300, computer.CPU.REGS.AB);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_POW_fr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 1.0001f);
            computer.CPU.REGS.ABC = 80_000;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "POW F0, ABC",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(2983.72388f, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal((uint)80_000, computer.CPU.REGS.ABC);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_POW_fr_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 1.0000001f);
            computer.CPU.REGS.ABCD = 40_000_000;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "POW F0, ABCD",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(117.727341f, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal((uint)40_000_000, computer.CPU.REGS.ABCD);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_POW_r_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 1.5f);
            computer.CPU.REGS.A = 3;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "POW A, F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1.5f, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal(5, computer.CPU.REGS.A);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_POW_rr_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 1.5f);
            computer.CPU.REGS.AB = 300;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "POW AB, F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1.5f, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal(5196, computer.CPU.REGS.AB);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_POW_rrr_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 1.25f);
            computer.CPU.REGS.ABC = 80_000;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "POW ABC, F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1.25f, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal((uint)1_345_434, computer.CPU.REGS.ABC);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_POW_rrrr_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 1.1f);
            computer.CPU.REGS.ABCD = 20_000_000;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "POW ABCD, F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1.1f, computer.CPU.FREGS.GetRegister(0));
            Assert.Equal((uint)107_431_880, computer.CPU.REGS.ABCD);

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_POW_fr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 1.4f);
            computer.CPU.REGS.ABC = 0x2000;
            computer.MEMC.SetFloatToRam(0x2000, 1.5f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "POW F0, (ABC)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1.65650225f, computer.CPU.FREGS.GetRegister(0));

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_POW_IrrrI_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 1.4f);
            computer.CPU.REGS.ABC = 0x2000;
            computer.MEMC.SetFloatToRam(0x2000, 1.5f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "POW (ABC), F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1.76411853f, computer.MEMC.GetFloatFromRAM(0x2000));

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_POW_fr_n()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 1.4f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "POW F0, 1.5",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1.65650225f, computer.CPU.FREGS.GetRegister(0));

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_POW_fr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 1.4f);
            computer.MEMC.SetFloatToRam(0x2000, 1.5f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "POW F0, (0x2000)",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1.65650225f, computer.CPU.FREGS.GetRegister(0));

            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_POW_InnnI_fr()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.FREGS.SetRegister(0, 1.4f);
            computer.MEMC.SetFloatToRam(0x2000, 1.5f);

            cp.Build(
                TUtils.GetFormattedAsm(
                    "POW (0x2000), F0",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(1.76411853f, computer.MEMC.GetFloatFromRAM(0x2000));

            TUtils.IncrementCountedTests("exec");
        }
    }
}
