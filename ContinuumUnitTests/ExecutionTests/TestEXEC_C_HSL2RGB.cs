using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{
    public class TestEXEC_C_HSL2RGB
    {
        [Fact]
        public void TestEXEC_HSL2RGB_nnn_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                HSL2RGB 0x005D512B, ABC
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x64C614, computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_HSL2RGB_rrrr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD WXYZ, 0x005D512B
                HSL2RGB WXYZ, ABC
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x64C614, computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_HSL2RGB_nnnn_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                HSL2RGB 0x005D512B, (0x20000)
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x64C614, computer.MEMC.Get24bitFromRAM(0x20000));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_HSL2RGB_rrr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD WXYZ, 0x005D512B
                HSL2RGB WXYZ, (0x20000)
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x64C614, computer.MEMC.Get24bitFromRAM(0x20000));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_HSL2RGB_nnn_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD KLM, 0x20000
                HSL2RGB 0x005D512B, (KLM)
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x64C614, computer.MEMC.Get24bitFromRAM(0x20000));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_HSL2RGB_rrrr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD WXYZ, 0x005D512B
                LD KLM, 0x20000
                HSL2RGB WXYZ, (KLM)
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x64C614, computer.MEMC.Get24bitFromRAM(0x20000));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_HSL2RGB_InnnI_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.LoadMemAt(0x22000, new byte[] { 0x00, 0x5D, 0x51, 0x2B });

            cp.Build(@"
                HSL2RGB (0x22000), ABC
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x64C614, computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_HSL2RGB_IrrrI_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.LoadMemAt(0x22000, new byte[] { 0x00, 0x5D, 0x51, 0x2B });

            cp.Build(@"
                LD KLM, 0x22000
                HSL2RGB (KLM), ABC
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x64C614, computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_HSL2RGB_InnnI_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.LoadMemAt(0x22000, new byte[] { 0x00, 0x5D, 0x51, 0x2B });

            cp.Build(@"
                HSL2RGB (0x22000), (0x20000)
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x64C614, computer.MEMC.Get24bitFromRAM(0x20000));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_HSL2RGB_IrrrI_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.LoadMemAt(0x22000, new byte[] { 0x00, 0x5D, 0x51, 0x2B });

            cp.Build(@"
                LD KLM, 0x22000
                HSL2RGB (KLM), (0x20000)
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x64C614, computer.MEMC.Get24bitFromRAM(0x20000));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_HSL2RGB_InnnI_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.LoadMemAt(0x22000, new byte[] { 0x00, 0x5D, 0x51, 0x2B });

            cp.Build(@"
                LD KLM, 0x20000
                HSL2RGB (0x22000), (KLM)
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x64C614, computer.MEMC.Get24bitFromRAM(0x20000));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_HSL2RGB_IrrrI_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.LoadMemAt(0x22000, new byte[] { 0x00, 0x5D, 0x51, 0x2B });

            cp.Build(@"
                LD XYZ, 0x22000
                LD KLM, 0x20000
                HSL2RGB (XYZ), (KLM)
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x64C614, computer.MEMC.Get24bitFromRAM(0x20000));
            TUtils.IncrementCountedTests("exec");
        }
    }
}
