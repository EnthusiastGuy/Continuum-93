using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_JP
    {
        [Fact]
        public void TestEXEC_JP_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                LD A, 3
                JP .JumpAddress
                LD C, 0x40
                BREAK
            .JumpAddress
                LD A, 0xFF
                LD B, 0x80
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xFF, computer.CPU.REGS.A);
            Assert.Equal(0x80, computer.CPU.REGS.B);
            Assert.Equal(0x0, computer.CPU.REGS.C);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_JP_ff_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();
            computer.CPU.FLAGS.SetValueByName("Z", true);

            cp.Build(@"
                LD A, 3
                JP Z, .LabelJump
                LD C, 0x40
                BREAK
            .LabelJump
                LD A, 0xFF
                BREAK 
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xFF, computer.CPU.REGS.A);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_JP_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                LD A, 3
                LD XYZ, 20
                JP XYZ
                BREAK
                NOP
                NOP
                NOP
                NOP
                NOP
                NOP
                NOP
                NOP
                NOP
                NOP
                NOP
                NOP
                NOP
                LD A, 0xFF
                LD B, 0x80
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xFF, computer.CPU.REGS.A);
            Assert.Equal(0x80, computer.CPU.REGS.B);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_JP_ff_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();
            computer.CPU.FLAGS.SetValueByName("C", true);

            cp.Build(@"
                LD A, 3
                LD XYZ, 20
                JP C, XYZ
                LD B, 0xAA
                BREAK
                NOP
                NOP
                NOP
                NOP
                NOP
                NOP
                NOP
                NOP
                NOP
                NOP
                NOP
                NOP
                NOP
                LD A, 0xFF
                LD B, 0x80
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xFF, computer.CPU.REGS.A);
            Assert.Equal(0x80, computer.CPU.REGS.B);
            TUtils.IncrementCountedTests("exec");
        }
    }
}
