using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace Interrupts
{
    public class TestInterruptStrings_FloatToString
    {
        [Theory]
        [InlineData(3.1415f, "", "3.1415")]
        [InlineData(3.1415f, "0000.00", "0003.14")]
        [InlineData(123.45f, "0000.00", "0123.45")]
        [InlineData(0.5f, "0.00", "0.50")]
        [InlineData(-2.718f, "000.00", "-002.72")]
        [InlineData(1234.5678f, "00000.0", "01234.6")]
        [InlineData(0.1245f, "P02", "12.45%")]
        [InlineData(545912f, "N00", "545,912")]
        public void TestFloatToString(float value, string format, string expected)
        {
            // Initialize Assembler and Computer
            Assembler cp = new();
            using Computer computer = new();

            string floatStrValue = value.ToString();
            if (!floatStrValue.Contains('.'))
            {
                floatStrValue += ".0";
            }

            if (value < 0)
            {
                int xy = 0;
            }

            // Set the interrupt call to convert float to string
            computer.CPU.REGS.A = 0x01; // 0x01 - FloatToString
            string source = $@"
                    LD F2, {floatStrValue}
                    LD A, 0x01
                    LD B, 2
                    LD CDE, .Format
                    LD FGH, .Destination
                    INT 0x05, A
                    BREAK

                .Format
                    #DB ""{format}"", 0

                .Destination
                    #DB [32] 0x00
                ";

            // Build the assembler code, injecting the format string and destination buffer dynamically
            cp.Build(source);

            // Compile and load the code
            byte[] compiled = cp.GetCompiledCode();
            computer.LoadMem(compiled);

            // Run the computer to execute the code
            computer.Run();

            // Retrieve the result from the destination address
            string actual = computer.MEMC.GetStringAt(computer.CPU.REGS.FGH);

            // Assert the result matches the expected value
            Assert.Equal(expected, actual);

            // Increment test count
            TUtils.IncrementCountedTests("interrupts");
        }

        
    }
}
