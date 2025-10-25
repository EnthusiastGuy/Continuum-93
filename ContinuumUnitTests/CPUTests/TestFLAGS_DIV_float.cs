using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace CPUTests
{

    public class TestFLAGS_DIV_float
    {
        [Fact]
        public void TestDiv_fr_fr_positive()
        {
            Assembler cp = new();

            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetSignNegative(true);
            Assert.True(computer.CPU.FLAGS.IsNegative());

            computer.CPU.FREGS.SetRegister(0, 100f);
            computer.CPU.FREGS.SetRegister(1, 1.5f);

            cp.Build(@"
                DIV F0,F1
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsNegative());
        }

        [Fact]
        public void TestDiv_fr_fr_negative()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsNegative());

            computer.CPU.FREGS.SetRegister(0, 100f);
            computer.CPU.FREGS.SetRegister(1, -1.0f);

            cp.Build(@"
                DIV F0,F1
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsNegative());
        }

        [Fact]
        public void TestDiv_fr_nnn_positive()
        {
            Assembler cp = new();

            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetSignNegative(true);
            Assert.True(computer.CPU.FLAGS.IsNegative());

            computer.CPU.FREGS.SetRegister(0, 100f);

            cp.Build(@"
                DIV F0,0.25
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsNegative());
        }

        [Fact]
        public void TestDiv_fr_nnn_negative()
        {
            Assembler cp = new();

            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsNegative());

            computer.CPU.FREGS.SetRegister(0, 100f);

            cp.Build(@"
                DIV F0,-2.2
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsNegative());
        }

        [Fact]
        public void TestDiv_fr_r()
        {
            Assembler cp = new();

            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetSignNegative(true);
            Assert.True(computer.CPU.FLAGS.IsNegative());

            computer.CPU.FREGS.SetRegister(0, 100f);
            computer.CPU.REGS.Set8BitRegister(0, 2);

            cp.Build(@"
                DIV F0,A
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsNegative());
        }

        [Fact]
        public void TestDiv_fr_rr()
        {
            Assembler cp = new();

            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetSignNegative(true);
            Assert.True(computer.CPU.FLAGS.IsNegative());

            computer.CPU.FREGS.SetRegister(0, 100f);
            computer.CPU.REGS.Set16BitRegister(0, 2);

            cp.Build(@"
                DIV F0,AB
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsNegative());
        }

        [Fact]
        public void TestDiv_fr_rrr()
        {
            Assembler cp = new();

            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetSignNegative(true);
            Assert.True(computer.CPU.FLAGS.IsNegative());

            computer.CPU.FREGS.SetRegister(0, 100f);
            computer.CPU.REGS.Set24BitRegister(0, 2);

            cp.Build(@"
                DIV F0,ABC
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsNegative());
        }

        [Fact]
        public void TestDiv_fr_rrrr()
        {
            Assembler cp = new();

            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetSignNegative(true);
            Assert.True(computer.CPU.FLAGS.IsNegative());

            computer.CPU.FREGS.SetRegister(0, 100f);
            computer.CPU.REGS.Set32BitRegister(0, 2);

            cp.Build(@"
                DIV F0,ABCD
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsNegative());
        }

        [Fact]
        public void TestDiv_r_fr_unaffected()
        {
            Assembler cp = new();

            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetSignNegative(true);

            computer.CPU.FREGS.SetRegister(0, 100f);
            computer.CPU.REGS.Set8BitRegister(0, 200);

            cp.Build(@"
                DIV A,F0
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsNegative());
        }

        [Fact]
        public void TestDiv_rr_fr_unaffected()
        {
            Assembler cp = new();

            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetSignNegative(true);

            computer.CPU.FREGS.SetRegister(0, 100f);
            computer.CPU.REGS.Set16BitRegister(0, 20000);

            cp.Build(@"
                DIV AB,F0
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsNegative());
        }

        [Fact]
        public void TestDiv_rrr_fr_unaffected()
        {
            Assembler cp = new();

            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetSignNegative(true);

            computer.CPU.FREGS.SetRegister(0, 100f);
            computer.CPU.REGS.Set24BitRegister(0, 200000);

            cp.Build(@"
                DIV ABC,F0
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsNegative());
        }

        [Fact]
        public void TestDiv_rrrr_fr_unaffected()
        {
            Assembler cp = new();

            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetSignNegative(true);

            computer.CPU.FREGS.SetRegister(0, 100f);
            computer.CPU.REGS.Set32BitRegister(0, 20_000_000);

            cp.Build(@"
                DIV ABCD,F0
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsNegative());
        }

        [Fact]
        public void TestDiv_fr_InnnI_positive()
        {
            Assembler cp = new();

            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetSignNegative(true);
            Assert.True(computer.CPU.FLAGS.IsNegative());

            computer.CPU.FREGS.SetRegister(0, 100f);
            computer.MEMC.SetFloatToRam(0x2000, 5.0f);

            cp.Build(@"
                DIV F0,(0x2000)
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsNegative());
        }

        [Fact]
        public void TestDiv_fr_InnnI_negative()
        {
            Assembler cp = new();

            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsNegative());

            computer.CPU.FREGS.SetRegister(0, 100f);
            computer.MEMC.SetFloatToRam(0x2000, -5.0f);

            cp.Build(@"
                DIV F0,(0x2000)
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsNegative());
        }

        [Fact]
        public void TestDiv_fr_IrrrI_positive()
        {
            Assembler cp = new();

            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetSignNegative(true);
            Assert.True(computer.CPU.FLAGS.IsNegative());

            computer.CPU.FREGS.SetRegister(0, 100f);
            computer.CPU.REGS.Set24BitRegister(0, 0x2000);
            computer.MEMC.SetFloatToRam(0x2000, 5.0f);

            cp.Build(@"
                DIV F0,(ABC)
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsNegative());
        }

        [Fact]
        public void TestDiv_fr_IrrrI_negative()
        {
            Assembler cp = new();

            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsNegative());

            computer.CPU.FREGS.SetRegister(0, 100f);
            computer.CPU.REGS.Set24BitRegister(0, 0x2000);
            computer.MEMC.SetFloatToRam(0x2000, -5.0f);

            cp.Build(@"
                DIV F0,(ABC)
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsNegative());
        }

        [Fact]
        public void TestDiv_InnnI_fr_positive()
        {
            Assembler cp = new();

            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetSignNegative(true);
            Assert.True(computer.CPU.FLAGS.IsNegative());

            computer.MEMC.SetFloatToRam(0x2000, 500.0f);
            computer.CPU.FREGS.SetRegister(0, 100f);

            cp.Build(@"
                DIV (0x2000),F0
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsNegative());
        }

        [Fact]
        public void TestDiv_InnnI_fr_negative()
        {
            Assembler cp = new();

            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsNegative());

            computer.MEMC.SetFloatToRam(0x2000, 500.0f);
            computer.CPU.FREGS.SetRegister(0, -100f);

            cp.Build(@"
                DIV (0x2000),F0
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsNegative());
        }

        [Fact]
        public void TestDiv_IrrrI_fr_positive()
        {
            Assembler cp = new();

            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            computer.CPU.FLAGS.SetSignNegative(true);
            Assert.True(computer.CPU.FLAGS.IsNegative());

            computer.MEMC.SetFloatToRam(0x2000, 500.0f);
            computer.CPU.REGS.Set24BitRegister(0, 0x2000);
            computer.CPU.FREGS.SetRegister(0, 100f);

            cp.Build(@"
                DIV (ABC),F0
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.False(computer.CPU.FLAGS.IsNegative());
        }

        [Fact]
        public void TestDiv_IrrrI_fr_negative()
        {
            Assembler cp = new();

            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsNegative());

            computer.MEMC.SetFloatToRam(0x2000, 500.0f);
            computer.CPU.REGS.Set24BitRegister(0, 0x2000);
            computer.CPU.FREGS.SetRegister(0, -100f);

            cp.Build(@"
                DIV (ABC),F0
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.True(computer.CPU.FLAGS.IsNegative());
        }
    }
}
