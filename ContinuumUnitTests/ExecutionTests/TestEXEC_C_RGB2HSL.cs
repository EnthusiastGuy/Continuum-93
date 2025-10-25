using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{
    public class TestEXEC_C_RGB2HSL
    {
        [Fact]
        public void TestEXEC_RGB2HSL_nnn_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                RGB2HSL 0x64C814, ABCD
                BREAK
            "
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x005D512B, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RGB2HSL_rrr_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD XYZ, 0x64C814
                RGB2HSL XYZ, ABCD
                BREAK
            "
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x005D512B, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RGB2HSL_nnn_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                RGB2HSL 0x64C814, (0x20000)
                BREAK
            "
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x005D512B, computer.MEMC.Get32bitFromRAM(0x20000));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RGB2HSL_rrr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD XYZ, 0x64C814
                RGB2HSL XYZ, (0x20000)
                BREAK
            "
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x005D512B, computer.MEMC.Get32bitFromRAM(0x20000));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RGB2HSL_nnn_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD KLM, 0x20000
                RGB2HSL 0x64C814, (KLM)
                BREAK
            "
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x005D512B, computer.MEMC.Get32bitFromRAM(0x20000));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RGB2HSL_rrr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD XYZ, 0x64C814
                LD KLM, 0x20000
                RGB2HSL XYZ, (KLM)
                BREAK
            "
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x005D512B, computer.MEMC.Get32bitFromRAM(0x20000));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RGB2HSL_InnnI_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.LoadMemAt(0x22000, new byte[] { 0x64, 0xC8, 0x14 });

            cp.Build(@"
                RGB2HSL (0x22000), ABCD
                BREAK
            "
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x005D512B, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RGB2HSL_IrrrI_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.LoadMemAt(0x22000, new byte[] { 0x64, 0xC8, 0x14 });

            cp.Build(@"
                LD KLM, 0x22000
                RGB2HSL (KLM), ABCD
                BREAK
            "
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x005D512B, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RGB2HSL_InnnI_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.LoadMemAt(0x22000, new byte[] { 0x64, 0xC8, 0x14 });

            cp.Build(@"
                RGB2HSL (0x22000), (0x20000)
                BREAK
            "
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x005D512B, computer.MEMC.Get32bitFromRAM(0x20000));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RGB2HSL_IrrrI_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.LoadMemAt(0x22000, new byte[] { 0x64, 0xC8, 0x14 });

            cp.Build(@"
                LD KLM, 0x22000
                RGB2HSL (KLM), (0x20000)
                BREAK
            "
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x005D512B, computer.MEMC.Get32bitFromRAM(0x20000));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RGB2HSL_InnnI_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.LoadMemAt(0x22000, new byte[] { 0x64, 0xC8, 0x14 });

            cp.Build(@"
                LD KLM, 0x20000
                RGB2HSL (0x22000), (KLM)
                BREAK
            "
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x005D512B, computer.MEMC.Get32bitFromRAM(0x20000));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_RGB2HSL_IrrrI_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.LoadMemAt(0x22000, new byte[] { 0x64, 0xC8, 0x14 });

            cp.Build(@"
                LD XYZ, 0x22000
                LD KLM, 0x20000
                RGB2HSL (XYZ), (KLM)
                BREAK
            "
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x005D512B, computer.MEMC.Get32bitFromRAM(0x20000));
            TUtils.IncrementCountedTests("exec");
        }
    }
}
