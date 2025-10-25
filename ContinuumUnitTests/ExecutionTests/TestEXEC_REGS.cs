using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{
    public class TestEXEC_REGS
    {
        [Fact]
        public void TestEXEC_REGS_n()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(@"
                LD ABCD, 0xAABBCCDD
                REGS 1
                LD ABCD, 0x12345678
                REGS 2
                LD ABCD, 0x456789AB
                REGS 0
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xAABBCCDD, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_REGS_n_variation()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(@"
                LD ABCD, 0xAABBCCDD
                REGS 1
                LD ABCD, 0x12345678
                REGS 2
                LD ABCD, 0x456789AB
                REGS 1
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x12345678, (long)computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_REGS_InnnI()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(@"
                REGS 1
                LD ABCD, 0x12345678
                REGS (20000)    ; should equival with REGS 0
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((double)0x00000000, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_REGS_r()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(@"
                LD Z, 1
                LD ABCD, 0xAABBCCDD
                REGS Z
                LD ABCD, 0x12345678
                INC Z
                REGS Z
                LD ABCD, 0x456789AB
                LD Z, 0
                REGS Z
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xAABBCCDD, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_REGS_IrrrI()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.Equal(0, (double)computer.CPU.REGS.SPR);

            cp.Build(@"
                REGS 1
                LD XYZ, 20000
                LD ABCD, 0x12345678
                REGS (XYZ)    ; should equival with REGS 0
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal((double)0x00000000, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");
        }
    }
}
