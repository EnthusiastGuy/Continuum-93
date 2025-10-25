using Continuum93.Emulator;
using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace Interrupts
{
    public class TestInterrupts_00_Machine_Designations
    {
        [Theory]
        [InlineData(125, "Titan II")]
        [InlineData(2, "Nova I")]
        [InlineData(10, "Comet")]
        [InlineData(18, "Comet Prime")]
        [InlineData(90, "Phoenix II")]
        [InlineData(100, "Phoenix Keanu")]
        [InlineData(200, "Vulcan I")]
        [InlineData(400, "Atlas Keanu")]
        [InlineData(1000, "Odin II")]
        [InlineData(1100, "Beyond Odin +2")]
        [InlineData(1200, "Beyond Odin +3")]
        public void TestCPUDesignationAssociation(ushort frequency, string expectedDesignation)
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.FillMemoryAt(0x1000, 40, 255);

            cp.Build($@"
                LD A, 0xF0              ; 0xF0 - GetCPUDesignationByFrequency
                LD BC, {frequency}      ; {frequency} MHz
                LD DEF, .TargetDesignation
                INT 0x00, A
                BREAK

            .TargetDesignation
                #DB [64] 0
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            string actualDesignation = computer.MEMC.GetStringAt(computer.CPU.REGS.DEF);

            Assert.Equal(expectedDesignation, actualDesignation);

            TUtils.IncrementCountedTests("interrupts");
        }
    }
}
