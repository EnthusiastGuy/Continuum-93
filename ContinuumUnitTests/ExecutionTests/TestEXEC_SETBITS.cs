using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;

namespace ExecutionTests
{
    public class TestEXEC_SETBITS
    {
        [Theory]
        [InlineData(0b11111111, 803, 7, new byte[] { 0b00011111, 0b11000000, 0, 0, 0 })]
        [InlineData(0b11111111, 803, 8, new byte[] { 0b00011111, 0b11100000, 0, 0, 0 })]
        [InlineData(0b11111111, 800, 8, new byte[] { 0b11111111, 0, 0, 0, 0 })]
        public void TestEXEC_SETBITS_rrrr_r_n(byte regValue, uint bitAddress, byte numBits, byte[] expected)
        {
            Assembler cp = new();
            using Computer computer = new();

            // Build and run the assembly
            cp.Build($@"
                LD A, {regValue}
                LD WXYZ, {bitAddress}
                SETBITS WXYZ, A, {numBits}
                BREAK
            ");
            byte[] compiled = cp.GetCompiledCode();
            computer.LoadMem(compiled);
            computer.Run();

            byte[] actualMem = computer.MEMC.RAM.GetMemoryAt(100, 5);

            // Assert result
            Assert.Equal(expected, actualMem);
            TUtils.IncrementCountedTests("exec");

            // Cleanup
            computer.Stop();
            computer.Clear();
        }

        [Theory]
        [InlineData(0b1111111111111111, 803, 10, new byte[] { 0b00011111, 0b11111000, 0, 0, 0 })]
        [InlineData(0b1111111111111111, 803, 16, new byte[] { 0b00011111, 0b11111111, 0b11100000, 0, 0 })]
        [InlineData(0b1111111111111111, 800, 16, new byte[] { 0b11111111, 0b11111111, 0, 0, 0 })]
        public void TestEXEC_SETBITS_rrrr_rr_n(ushort regValue, uint bitAddress, byte numBits, byte[] expected)
        {
            Assembler cp = new();
            using Computer computer = new();

            // Build and run the assembly
            cp.Build($@"
                LD AB, {regValue}
                LD WXYZ, {bitAddress}
                SETBITS WXYZ, AB, {numBits}
                BREAK
            ");
            byte[] compiled = cp.GetCompiledCode();
            computer.LoadMem(compiled);
            computer.Run();

            byte[] actualMem = computer.MEMC.RAM.GetMemoryAt(100, 5);

            // Assert result
            Assert.Equal(expected, actualMem);
            TUtils.IncrementCountedTests("exec");

            // Cleanup
            computer.Stop();
            computer.Clear();
        }

        [Theory]
        [InlineData(0b111111111111111111111111, 803, 20, new byte[] { 0b00011111, 0b11111111, 0b11111110, 0, 0 })]
        [InlineData(0b111111111111111111111111, 803, 24, new byte[] { 0b00011111, 0b11111111, 0b11111111, 0b11100000, 0 })]
        [InlineData(0b111111111111111111111111, 800, 24, new byte[] { 0b11111111, 0b11111111, 0b11111111, 0, 0 })]
        public void TestEXEC_SETBITS_rrrr_rrr_n(uint regValue, uint bitAddress, byte numBits, byte[] expected)
        {
            Assembler cp = new();
            using Computer computer = new();

            // Build and run the assembly
            cp.Build($@"
                LD ABC, {regValue}
                LD WXYZ, {bitAddress}
                SETBITS WXYZ, ABC, {numBits}
                BREAK
            ");
            byte[] compiled = cp.GetCompiledCode();
            computer.LoadMem(compiled);
            computer.Run();

            byte[] actualMem = computer.MEMC.RAM.GetMemoryAt(100, 5);

            // Assert result
            Assert.Equal(expected, actualMem);
            TUtils.IncrementCountedTests("exec");

            // Cleanup
            computer.Stop();
            computer.Clear();
        }

        [Theory]
        [InlineData(0b11111111111111111111111111111111, 803, 30, new byte[] { 0b00011111, 0b11111111, 0b11111111, 0b11111111, 0b10000000 })]
        [InlineData(0b11111111111111111111111111111111, 803, 32, new byte[] { 0b00011111, 0b11111111, 0b11111111, 0b11111111, 0b11100000 })]
        [InlineData(0b11111111111111111111111111111111, 800, 32, new byte[] { 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0 })]
        public void TestEXEC_SETBITS_rrrr_rrrr_n(uint regValue, uint bitAddress, byte numBits, byte[] expected)
        {
            Assembler cp = new();
            using Computer computer = new();

            // Build and run the assembly
            cp.Build($@"
                LD ABCD, {regValue}
                LD WXYZ, {bitAddress}
                SETBITS WXYZ, ABCD, {numBits}
                BREAK
            ");
            byte[] compiled = cp.GetCompiledCode();
            computer.LoadMem(compiled);
            computer.Run();

            byte[] actualMem = computer.MEMC.RAM.GetMemoryAt(100, 5);

            // Assert result
            Assert.Equal(expected, actualMem);
            TUtils.IncrementCountedTests("exec");

            // Cleanup
            computer.Stop();
            computer.Clear();
        }


        [Theory]
        [InlineData(0b11111111, 803, 7, new byte[] { 0b00011111, 0b11000000, 0, 0, 0 })]
        [InlineData(0b11111111, 803, 8, new byte[] { 0b00011111, 0b11100000, 0, 0, 0 })]
        [InlineData(0b11111111, 800, 8, new byte[] { 0b11111111, 0, 0, 0, 0 })]
        public void TestEXEC_SETBITS_rrrr_r_r(byte regValue, uint bitAddress, byte numBits, byte[] expected)
        {
            Assembler cp = new();
            using Computer computer = new();

            // Build and run the assembly
            cp.Build($@"
                LD A, {regValue}
                LD F, {numBits}
                LD WXYZ, {bitAddress}
                SETBITS WXYZ, A, F
                BREAK
            ");
            byte[] compiled = cp.GetCompiledCode();
            computer.LoadMem(compiled);
            computer.Run();

            byte[] actualMem = computer.MEMC.RAM.GetMemoryAt(100, 5);

            // Assert result
            Assert.Equal(expected, actualMem);
            TUtils.IncrementCountedTests("exec");

            // Cleanup
            computer.Stop();
            computer.Clear();
        }

        [Theory]
        [InlineData(0b1111111111111111, 803, 10, new byte[] { 0b00011111, 0b11111000, 0, 0, 0 })]
        [InlineData(0b1111111111111111, 803, 16, new byte[] { 0b00011111, 0b11111111, 0b11100000, 0, 0 })]
        [InlineData(0b1111111111111111, 800, 16, new byte[] { 0b11111111, 0b11111111, 0, 0, 0 })]
        public void TestEXEC_SETBITS_rrrr_rr_r(ushort regValue, uint bitAddress, byte numBits, byte[] expected)
        {
            Assembler cp = new();
            using Computer computer = new();

            // Build and run the assembly
            cp.Build($@"
                LD AB, {regValue}
                LD F, {numBits}
                LD WXYZ, {bitAddress}
                SETBITS WXYZ, AB, F
                BREAK
            ");
            byte[] compiled = cp.GetCompiledCode();
            computer.LoadMem(compiled);
            computer.Run();

            byte[] actualMem = computer.MEMC.RAM.GetMemoryAt(100, 5);

            // Assert result
            Assert.Equal(expected, actualMem);
            TUtils.IncrementCountedTests("exec");

            // Cleanup
            computer.Stop();
            computer.Clear();
        }

        [Theory]
        [InlineData(0b111111111111111111111111, 803, 20, new byte[] { 0b00011111, 0b11111111, 0b11111110, 0, 0 })]
        [InlineData(0b111111111111111111111111, 803, 24, new byte[] { 0b00011111, 0b11111111, 0b11111111, 0b11100000, 0 })]
        [InlineData(0b111111111111111111111111, 800, 24, new byte[] { 0b11111111, 0b11111111, 0b11111111, 0, 0 })]
        public void TestEXEC_SETBITS_rrrr_rrr_r(uint regValue, uint bitAddress, byte numBits, byte[] expected)
        {
            Assembler cp = new();
            using Computer computer = new();

            // Build and run the assembly
            cp.Build($@"
                LD ABC, {regValue}
                LD F, {numBits}
                LD WXYZ, {bitAddress}
                SETBITS WXYZ, ABC, F
                BREAK
            ");
            byte[] compiled = cp.GetCompiledCode();
            computer.LoadMem(compiled);
            computer.Run();

            byte[] actualMem = computer.MEMC.RAM.GetMemoryAt(100, 5);

            // Assert result
            Assert.Equal(expected, actualMem);
            TUtils.IncrementCountedTests("exec");

            // Cleanup
            computer.Stop();
            computer.Clear();
        }

        [Theory]
        [InlineData(0b11111111111111111111111111111111, 803, 30, new byte[] { 0b00011111, 0b11111111, 0b11111111, 0b11111111, 0b10000000 })]
        [InlineData(0b11111111111111111111111111111111, 803, 32, new byte[] { 0b00011111, 0b11111111, 0b11111111, 0b11111111, 0b11100000 })]
        [InlineData(0b11111111111111111111111111111111, 800, 32, new byte[] { 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0 })]
        public void TestEXEC_SETBITS_rrrr_rrrr_r(uint regValue, uint bitAddress, byte numBits, byte[] expected)
        {
            Assembler cp = new();
            using Computer computer = new();

            // Build and run the assembly
            cp.Build($@"
                LD ABCD, {regValue}
                LD F, {numBits}
                LD WXYZ, {bitAddress}
                SETBITS WXYZ, ABCD, F
                BREAK
            ");
            byte[] compiled = cp.GetCompiledCode();
            computer.LoadMem(compiled);
            computer.Run();

            byte[] actualMem = computer.MEMC.RAM.GetMemoryAt(100, 5);

            // Assert result
            Assert.Equal(expected, actualMem);
            TUtils.IncrementCountedTests("exec");

            // Cleanup
            computer.Stop();
            computer.Clear();
        }
    }
}
