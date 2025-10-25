using Continuum93.Emulator.Controls;
using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace Interrupts
{

    public class TestInterrupts_02_Input
    {
        [Fact]
        public void TestInt_ReadKeyboardStateAsBits()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.FillMemoryAt(0x1000, 40, 255);

            cp.Build(
                @"
                    LD A, 0     ;  0x00 - Read keyboard state as bits
                    LD BCD, 0x1000
                    INT 2, A
                    BREAK
                "
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            byte[] actual = computer.GetMemFrom(0x1000, 40);

            Assert.Equal(new byte[] {
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                255, 255, 255, 255, 255, 255, 255, 255
            }, actual);

            Assert.Equal(32, computer.CPU.REGS.A);

            TUtils.IncrementCountedTests("interrupts");
        }

        [Fact]
        public void TestInt_ReadKeyboardStateAsBytes()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.FillMemoryAt(0x1000, 40, 255);

            bool[] testState = new bool[256];

            testState[32] = true;
            testState[64] = true;
            testState[65] = true;
            testState[80] = true;


            KeyboardStateExt.SetStateForTests(testState);

            cp.Build(
                @"
                    LD A, 0x10  ; 0x10 - Read keyboard state as code bytes
                    LD BCD, 0x1000
                    INT 2, A
                    BREAK
                "
            );

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            byte[] actual = computer.GetMemFrom(0x1000, 40);

            Assert.Equal(new byte[] {
                32, 64, 65, 80, 255, 255, 255, 255,
                255, 255, 255, 255, 255, 255, 255, 255,
                255, 255, 255, 255, 255, 255, 255, 255,
                255, 255, 255, 255, 255, 255, 255, 255,
                255, 255, 255, 255, 255, 255, 255, 255
            }, actual);

            Assert.Equal(4, computer.CPU.REGS.A);

            TUtils.IncrementCountedTests("interrupts");
        }
    }
}
