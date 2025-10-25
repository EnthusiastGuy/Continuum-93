using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{
    public class TestEXEC_C_HSB2RGB
    {
        [Fact]
        public void TestEXEC_HSB2RGB_nnn_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                HSB2RGB 0x005D5A4E, ABC
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x64C613, computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_HSB2RGB_rrrr_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD WXYZ, 0x005D5A4E
                HSB2RGB WXYZ, ABC
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x64C613, computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_HSB2RGB_nnnn_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                HSB2RGB 0x005D5A4E, (0x20000)
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x64C613, computer.MEMC.Get24bitFromRAM(0x20000));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_HSB2RGB_rrr_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD WXYZ, 0x005D5A4E
                HSB2RGB WXYZ, (0x20000)
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x64C613, computer.MEMC.Get24bitFromRAM(0x20000));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_HSB2RGB_nnn_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD KLM, 0x20000
                HSB2RGB 0x005D5A4E, (KLM)
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x64C613, computer.MEMC.Get24bitFromRAM(0x20000));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_HSB2RGB_rrrr_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD WXYZ, 0x005D5A4E
                LD KLM, 0x20000
                HSB2RGB WXYZ, (KLM)
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x64C613, computer.MEMC.Get24bitFromRAM(0x20000));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_HSB2RGB_InnnI_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.LoadMemAt(0x22000, new byte[] { 0x00, 0x5D, 0x5A, 0x4E });

            cp.Build(@"
                HSB2RGB (0x22000), ABC
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x64C613, computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_HSB2RGB_IrrrI_rrrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.LoadMemAt(0x22000, new byte[] { 0x00, 0x5D, 0x5A, 0x4E });

            cp.Build(@"
                LD KLM, 0x22000
                HSB2RGB (KLM), ABC
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x64C613, computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_HSB2RGB_InnnI_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.LoadMemAt(0x22000, new byte[] { 0x00, 0x5D, 0x5A, 0x4E });

            cp.Build(@"
                HSB2RGB (0x22000), (0x20000)
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x64C613, computer.MEMC.Get24bitFromRAM(0x20000));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_HSB2RGB_IrrrI_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.LoadMemAt(0x22000, new byte[] { 0x00, 0x5D, 0x5A, 0x4E });

            cp.Build(@"
                LD KLM, 0x22000
                HSB2RGB (KLM), (0x20000)
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x64C613, computer.MEMC.Get24bitFromRAM(0x20000));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_HSB2RGB_InnnI_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.LoadMemAt(0x22000, new byte[] { 0x00, 0x5D, 0x5A, 0x4E });

            cp.Build(@"
                LD KLM, 0x20000
                HSB2RGB (0x22000), (KLM)
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x64C613, computer.MEMC.Get24bitFromRAM(0x20000));
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_HSB2RGB_IrrrI_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.LoadMemAt(0x22000, new byte[] { 0x00, 0x5D, 0x5A, 0x4E });

            cp.Build(@"
                LD XYZ, 0x22000
                LD KLM, 0x20000
                HSB2RGB (XYZ), (KLM)
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((long)0x64C613, computer.MEMC.Get24bitFromRAM(0x20000));
            TUtils.IncrementCountedTests("exec");
        }
    }
}
