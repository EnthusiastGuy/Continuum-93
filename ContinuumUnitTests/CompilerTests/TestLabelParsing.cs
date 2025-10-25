using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace CompilerTests
{
    public class TestLabelParsing
    {
        [Fact]
        public void Test_MultiLabels()
        {
            Assembler cp = new();
            using Computer computer = new();

            cp.Build(@"
                LD A, (ABC - XYZ)
                LD ABC, .FirstValue
                LD DEF, .SecondValue 
                ADD (.FirstValue),(.SecondValue)

            .FirstValue
                #DB 100, 0x00, 0x00, 0x00
            .SecondValue
                #DB 25, 0x00, 0x00, 0x00
            ");


            

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            uint firstValueAddress = computer.CPU.REGS.ABC;
            uint secondValueAddress = computer.CPU.REGS.DEF;
            byte actualFirstValue = computer.MEMC.Get8bitFromRAM(firstValueAddress);
            byte actualSecondValue = computer.MEMC.Get8bitFromRAM(secondValueAddress);

            Assert.Equal(125, actualFirstValue);
            Assert.Equal(25, actualSecondValue);
        }
    }
}
