using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{
    public class TestEXEC_GETBITS
    {
        [Theory]
        [InlineData(803, 7, 0b1001011)] // Address 803, 7 bits, expected result 0b1001011
        [InlineData(815, 3, 0b101)]     // Address 815, 3 bits, expected result 0b101
        [InlineData(812, 5, 0b01110)]
        [InlineData(800, 50, 0b11010010)]
        [InlineData(800, 0, 0b11010010)]
        public void TestEXEC_GETBITS_r_rrrr_n(uint bitAddress, byte numBits, byte expected)
        {
            Assembler cp = new();
            using Computer computer = new();

            // Load initial memory for testing
            computer.LoadMemAt(100, new byte[] { 0b11010010, 0b11010111, 0b01010101, 0b00001111 });

            // Build and run the assembly
            cp.Build($@"
                LD WXYZ, {bitAddress}
                GETBITS A, WXYZ, {numBits}
                BREAK
            ");
            byte[] compiled = cp.GetCompiledCode();
            computer.LoadMem(compiled);
            computer.Run();

            // Assert result
            Assert.Equal(expected, computer.CPU.REGS.A);
            TUtils.IncrementCountedTests("exec");

            // Cleanup
            computer.Stop();
            computer.Clear();
        }

        [Theory]
        [InlineData(803, 10, 0b1001011010)]
        [InlineData(815, 16, 0b1010101010000111)]
        [InlineData(812, 5, 0b01110)]
        [InlineData(800, 50, 0b1101001011010111)]
        [InlineData(800, 0, 0b1101001011010111)]
        public void TestEXEC_GETBITS_rr_rrrr_n(uint bitAddress, byte numBits, ushort expected)
        {
            Assembler cp = new();
            using Computer computer = new();

            // Load initial memory for testing
            computer.LoadMemAt(100, new byte[] { 0b11010010, 0b11010111, 0b01010101, 0b00001111 });

            // Build and run the assembly
            cp.Build($@"
                LD WXYZ, {bitAddress}
                GETBITS AB, WXYZ, {numBits}
                BREAK
            ");
            byte[] compiled = cp.GetCompiledCode();
            computer.LoadMem(compiled);
            computer.Run();

            // Assert result
            Assert.Equal(expected, computer.CPU.REGS.AB);
            TUtils.IncrementCountedTests("exec");

            // Cleanup
            computer.Stop();
            computer.Clear();
        }

        [Theory]
        [InlineData(803, 20, 0b10010110101110101010)]
        [InlineData(815, 24, 0b101010101000011111100110)]
        [InlineData(800, 50, 0b110100101101011101010101)]
        [InlineData(800, 0, 0b110100101101011101010101)]
        public void TestEXEC_GETBITS_rrr_rrrr_n(uint bitAddress, byte numBits, uint expected)
        {
            Assembler cp = new();
            using Computer computer = new();

            // Load initial memory for testing
            computer.LoadMemAt(100, new byte[] { 0b11010010, 0b11010111, 0b01010101, 0b00001111, 0b11001100 });

            // Build and run the assembly
            cp.Build($@"
                LD WXYZ, {bitAddress}
                GETBITS ABC, WXYZ, {numBits}
                BREAK
            ");
            byte[] compiled = cp.GetCompiledCode();
            computer.LoadMem(compiled);
            computer.Run();

            // Assert result
            Assert.Equal(expected, computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");

            // Cleanup
            computer.Stop();
            computer.Clear();
        }

        [Theory]
        [InlineData(803, 30, 0b100101101011101010101000011111)]
        [InlineData(815, 32, 0b10101010100001111110011000110100)]
        [InlineData(800, 50, 0b11010010110101110101010100001111)]
        [InlineData(800, 0, 0b11010010110101110101010100001111)]
        public void TestEXEC_GETBITS_rrrr_rrrr_n(uint bitAddress, byte numBits, uint expected)
        {
            Assembler cp = new();
            using Computer computer = new();

            // Load initial memory for testing
            computer.LoadMemAt(100, new byte[] { 0b11010010, 0b11010111, 0b01010101, 0b00001111, 0b11001100, 0b01101001 });

            // Build and run the assembly
            cp.Build($@"
                LD WXYZ, {bitAddress}
                GETBITS ABCD, WXYZ, {numBits}
                BREAK
            ");
            byte[] compiled = cp.GetCompiledCode();
            computer.LoadMem(compiled);
            computer.Run();

            // Assert result
            Assert.Equal(expected, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");

            // Cleanup
            computer.Stop();
            computer.Clear();
        }


        [Theory]
        [InlineData(803, 7, 0b1001011)] // Address 803, 7 bits, expected result 0b1001011
        [InlineData(815, 3, 0b101)]     // Address 815, 3 bits, expected result 0b101
        [InlineData(812, 5, 0b01110)]
        [InlineData(800, 50, 0b11010010)]
        [InlineData(800, 0, 0b11010010)]
        public void TestEXEC_GETBITS_r_rrrr_r(uint bitAddress, byte numBits, byte expected)
        {
            Assembler cp = new();
            using Computer computer = new();

            // Load initial memory for testing
            computer.LoadMemAt(100, new byte[] { 0b11010010, 0b11010111, 0b01010101, 0b00001111 });

            // Build and run the assembly
            cp.Build($@"
                LD F, {numBits}
                LD WXYZ, {bitAddress}
                GETBITS A, WXYZ, F
                BREAK
            ");
            byte[] compiled = cp.GetCompiledCode();
            computer.LoadMem(compiled);
            computer.Run();

            // Assert result
            Assert.Equal(expected, computer.CPU.REGS.A);
            TUtils.IncrementCountedTests("exec");

            // Cleanup
            computer.Stop();
            computer.Clear();
        }

        [Theory]
        [InlineData(803, 10, 0b1001011010)]
        [InlineData(815, 16, 0b1010101010000111)]
        [InlineData(812, 5, 0b01110)]
        [InlineData(800, 50, 0b1101001011010111)]
        [InlineData(800, 0, 0b1101001011010111)]
        public void TestEXEC_GETBITS_rr_rrrr_r(uint bitAddress, byte numBits, ushort expected)
        {
            Assembler cp = new();
            using Computer computer = new();

            // Load initial memory for testing
            computer.LoadMemAt(100, new byte[] { 0b11010010, 0b11010111, 0b01010101, 0b00001111 });

            // Build and run the assembly
            cp.Build($@"
                LD F, {numBits}
                LD WXYZ, {bitAddress}
                GETBITS AB, WXYZ, F
                BREAK
            ");
            byte[] compiled = cp.GetCompiledCode();
            computer.LoadMem(compiled);
            computer.Run();

            // Assert result
            Assert.Equal(expected, computer.CPU.REGS.AB);
            TUtils.IncrementCountedTests("exec");

            // Cleanup
            computer.Stop();
            computer.Clear();
        }

        [Theory]
        [InlineData(803, 20, 0b10010110101110101010)]
        [InlineData(815, 24, 0b101010101000011111100110)]
        [InlineData(800, 50, 0b110100101101011101010101)]
        [InlineData(800, 0, 0b110100101101011101010101)]
        public void TestEXEC_GETBITS_rrr_rrrr_r(uint bitAddress, byte numBits, uint expected)
        {
            Assembler cp = new();
            using Computer computer = new();

            // Load initial memory for testing
            computer.LoadMemAt(100, new byte[] { 0b11010010, 0b11010111, 0b01010101, 0b00001111, 0b11001100 });

            // Build and run the assembly
            cp.Build($@"
                LD F, {numBits}
                LD WXYZ, {bitAddress}
                GETBITS ABC, WXYZ, F
                BREAK
            ");
            byte[] compiled = cp.GetCompiledCode();
            computer.LoadMem(compiled);
            computer.Run();

            // Assert result
            Assert.Equal(expected, computer.CPU.REGS.ABC);
            TUtils.IncrementCountedTests("exec");

            // Cleanup
            computer.Stop();
            computer.Clear();
        }

        [Theory]
        [InlineData(803, 30, 0b100101101011101010101000011111)]
        [InlineData(815, 32, 0b10101010100001111110011000110100)]
        [InlineData(800, 50, 0b11010010110101110101010100001111)]
        [InlineData(800, 0, 0b11010010110101110101010100001111)]
        public void TestEXEC_GETBITS_rrrr_rrrr_r(uint bitAddress, byte numBits, uint expected)
        {
            Assembler cp = new();
            using Computer computer = new();

            // Load initial memory for testing
            computer.LoadMemAt(100, new byte[] { 0b11010010, 0b11010111, 0b01010101, 0b00001111, 0b11001100, 0b01101001 });

            // Build and run the assembly
            cp.Build($@"
                LD F, {numBits}
                LD WXYZ, {bitAddress}
                GETBITS ABCD, WXYZ, F
                BREAK
            ");
            byte[] compiled = cp.GetCompiledCode();
            computer.LoadMem(compiled);
            computer.Run();

            // Assert result
            Assert.Equal(expected, computer.CPU.REGS.ABCD);
            TUtils.IncrementCountedTests("exec");

            // Cleanup
            computer.Stop();
            computer.Clear();
        }
    }
}
