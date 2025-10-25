using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_CALL
    {
        [Fact]
        public void TestEXEC_CALL_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                LD A, 3
                CALL .CallAddress
                CALL .AnotherAddress
                LD C, 0x40
                BREAK
            .CallAddress
                LD A, 0xFF
                LD B, 0x80
                RET" +
                "DB \"Get this, we're coming!\"" +
                @"LD ABC, 0xABCDEF
            .AnotherAddress
                LD D, 0x10
                RET
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xFF, computer.CPU.REGS.A);
            Assert.Equal(0x80, computer.CPU.REGS.B);
            Assert.Equal(0x40, computer.CPU.REGS.C);
            Assert.Equal(0x10, computer.CPU.REGS.D);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CALL_ff_nnn()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();
            computer.CPU.FLAGS.SetValueByName("Z", true);

            cp.Build(@"
                LD A, 3
                CALL Z, .LabelCalled
                CALL NZ, .LabelNotCalled
                LD C, 0x40
                BREAK
            .LabelCalled
                LD A, 0xFF
                RET
            .LabelNotCalled
                LD D, 0x10
                RET
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xFF, computer.CPU.REGS.A);
            Assert.Equal(0, computer.CPU.REGS.B);
            Assert.Equal(0x40, computer.CPU.REGS.C);
            Assert.Equal(0, computer.CPU.REGS.D);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CALL_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                LD A, 3
                LD XYZ, 20
                CALL XYZ
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
                RET
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xFF, computer.CPU.REGS.A);
            Assert.Equal(0x80, computer.CPU.REGS.B);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_CALL_ff_rrr()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.FLAGS.ResetAll();
            computer.CPU.FLAGS.SetValueByName("C", true);

            cp.Build(@"
                LD A, 3
                LD XYZ, 20
                CALL C, XYZ
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
                RET
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
