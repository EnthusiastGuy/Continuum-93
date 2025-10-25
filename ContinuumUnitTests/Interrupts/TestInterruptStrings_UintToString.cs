using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace Interrupts
{
    public class TestInterruptStrings_UintToString
    {
        // https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings?redirectedfrom=MSDN#code-example

        [Theory]
        [InlineData(055, "", "55")]
        [InlineData(055, "D6", "000055")]
        [InlineData(100, "# 'coins'", "100 coins")]
        [InlineData(31, "X4", "001F")]
        [InlineData(31, "B08", "00011111")]
        [InlineData(2811, "E03", "2.811E+003")]
        [InlineData(2811, "F02", "2811.00")]
        public void TestIntToFloatToString(int value, string format, string expected)
        {
            // Initialize Assembler and Computer
            Assembler cp = new();
            using Computer computer = new();

            // Set the interrupt call to convert float to string
            computer.CPU.REGS.A = 0x01; // 0x01 - FloatToString

            // Build the assembler code, injecting the format string and destination buffer dynamically
            cp.Build($@"
                LD A, 0x02
                LD BCDE, {value}
                LD FGH, .Format
                LD IJK, .Destination
                INT 0x05, A
                BREAK

            .Format
                #DB ""{format}"", 0

            .Destination
                #DB [32] 0x00
            ");

            // Compile and load the code
            byte[] compiled = cp.GetCompiledCode();
            computer.LoadMem(compiled);

            // Run the computer to execute the code
            computer.Run();

            // Retrieve the result from the destination address
            string actual = computer.MEMC.GetStringAt(computer.CPU.REGS.IJK);

            // Assert the result matches the expected value
            Assert.Equal(expected, actual);

            // Increment test count
            TUtils.IncrementCountedTests("interrupts");
        }
    }
}
