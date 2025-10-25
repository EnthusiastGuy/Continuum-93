using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{

    public class TestEXEC_INT
    {
        [Fact]
        public void TestEXEC_INT_r_0x01_1()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "INT 1, A",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(480, computer.CPU.REGS.BC);
            Assert.Equal(270, computer.CPU.REGS.DE);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_INT_r_0x01_2()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "INT 1, Z",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(480, computer.CPU.REGS.AB);
            Assert.Equal(270, computer.CPU.REGS.CD);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_INT_r_0x01_Mode0_Stop()
        {
            // TODO insert some form of failsafe code on clear
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.Z = 0;
            computer.CPU.REGS.A = 0;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "INT 0, Z",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);

            Assert.True(computer.IsRunning);
            computer.Run();
            Assert.False(computer.IsRunning);


            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_INT_r_0x01_Mode1_Clear()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.A = 1;
            computer.CPU.REGS.BCDE = 0xABCDEFAB;
            computer.CPU.REGS.FGHI = 0xABCDEFAB;
            computer.CPU.REGS.JKLM = 0xABCDEFAB;
            computer.CPU.REGS.NOPQ = 0xABCDEFAB;
            computer.CPU.REGS.RSTU = 0xABCDEFAB;
            computer.CPU.REGS.VWXY = 0xABCDEFAB;

            cp.Build(
                TUtils.GetFormattedAsm(
                    "INT 0, A",
                    "BREAK"
                )
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0, (long)computer.CPU.REGS.BCDE);
            Assert.Equal(0, (long)computer.CPU.REGS.FGHI);

            TUtils.IncrementCountedTests("exec");
        }
    }
}
