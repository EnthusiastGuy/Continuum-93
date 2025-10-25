using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace LabelsTests
{
    public class LabelsLD_ST_REGS
    {
        [Fact]
        public void TestEXEC_LDREGS()
        {
            Assembler cp = new();
            using Computer computer = new();

            Assert.Equal(0, computer.CPU.REGS.A);
            Assert.Equal(0, computer.CPU.REGS.B);
            Assert.Equal(0, computer.CPU.REGS.CD);
            Assert.Equal(0, (long)computer.CPU.REGS.EFGH);
            Assert.Equal(0, computer.CPU.REGS.I);
            Assert.Equal(0, computer.CPU.REGS.J);
            Assert.Equal(0, (long)computer.CPU.REGS.KLM);

            cp.Build(
                @$"
                    LDREGS A, J, (.Source)
                    BREAK
                .Source
                    #DB 0x0A, 0x0B, 0xABCD, 0x12345678, 0x99, 0x98
                "
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x0A, computer.CPU.REGS.A);
            Assert.Equal(0x0B, computer.CPU.REGS.B);
            Assert.Equal(0xABCD, computer.CPU.REGS.CD);
            Assert.Equal(0x12345678, (long)computer.CPU.REGS.EFGH);
            Assert.Equal(0x99, computer.CPU.REGS.I);
            Assert.Equal(0x98, computer.CPU.REGS.J);
            Assert.Equal(0, (long)computer.CPU.REGS.KLM);
            TUtils.IncrementCountedTests("exec");
        }

        [Fact]
        public void TestEXEC_STREGS_IrrrI_r_r()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.A = 0x20;
            computer.CPU.REGS.B = 0x21;
            computer.CPU.REGS.CD = 0x2223;
            computer.CPU.REGS.EFGH = 0x24252627;
            computer.CPU.REGS.I = 0x28;
            computer.CPU.REGS.J = 0x29;
            computer.CPU.REGS.KLM = 0xF0F1F2;

            cp.Build(
                @$"
                    #ORG 0
                    
                    STREGS (.Data), A, J
                    BREAK
                    
                    #ORG 50000
                .Data
                    #DB [13] 0x00
                "
            );

            List<CodeBlock> cBlocks = cp.BlockManager.GetBlocks();
            for (int i = 0; i < cBlocks.Count; i++)
            {
                computer.LoadMemAt(cBlocks[i].Start, cBlocks[i].Data);
            }
            computer.Run();

            byte[] expected = new byte[] {
                0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0, 0, 0 };
            byte[] actual = computer.GetMemFrom(50000, 13);

            Assert.Equal(expected, actual);

            TUtils.IncrementCountedTests("exec");
        }
    }
}
