using Continuum93.Emulator;
using Continuum93.Emulator.RAM;

namespace Code_Analysis
{

    public class MemoryDebugControllerTests
    {
        // Register stack
        [Fact]
        public void TestGetRegisterStackVeryLarge()
        {
            using Computer computer = new();


            byte stackItems = 250;

            PushToRegisterStack(Enumerable.Range(1, stackItems).Select(x => (byte)x).ToArray(), computer);

            byte[] actual = MemoryDebugController.GetRegisterStack(computer, 20);
            byte[] expected = Enumerable.Range(231, 20).Select(x => (byte)x).ToArray();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestGetRegisterStackLarge()
        {
            using Computer computer = new();

            byte stackItems = 25;

            PushToRegisterStack(Enumerable.Range(1, stackItems).Select(x => (byte)x).ToArray(), computer);

            byte[] actual = MemoryDebugController.GetRegisterStack(computer, 10);
            byte[] expected = new byte[] { 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestGetRegisterStackShort()
        {
            using Computer computer = new();


            byte stackItems = 4;

            PushToRegisterStack(Enumerable.Range(1, stackItems).Select(x => (byte)x).ToArray(), computer);

            byte[] actual = MemoryDebugController.GetRegisterStack(computer, 10);
            byte[] expected = new byte[] { 1, 2, 3, 4 };

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestGetRegisterStackEmpty()
        {
            using Computer computer = new();


            byte[] actual = MemoryDebugController.GetRegisterStack(computer, 10);

            Assert.Equal(Array.Empty<byte>(), actual);
        }

        private static void PushToRegisterStack(byte[] values, Computer computer)
        {
            for (int i = 0; i < values.Length; i++)
                computer.MEMC.RSRAM[computer.CPU.REGS.SPR++] = (byte)(i + 1);
        }

        // Call stack

        [Fact]
        public void TestCallStackVeryLarge()
        {
            using Computer computer = new();


            int stackItems = 4000;

            PushToCallStack(Enumerable.Range(1, stackItems).Select(x => (uint)x).ToArray(), computer);

            uint[] actual = MemoryDebugController.GetCallStack(computer, 30);
            uint[] expected = Enumerable.Range(3971, 30).Select(x => (uint)x).ToArray();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestCallStackShort()
        {
            using Computer computer = new();


            byte stackItems = 4;

            PushToCallStack(Enumerable.Range(1, stackItems).Select(x => (uint)x).ToArray(), computer);

            uint[] actual = MemoryDebugController.GetCallStack(computer, 10);
            uint[] expected = new uint[] { 1, 2, 3, 4 };

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestGetCallStackEmpty()
        {
            using Computer computer = new();


            uint[] actual = MemoryDebugController.GetCallStack(computer, 10);

            Assert.Equal(Array.Empty<uint>(), actual);
        }

        private static void PushToCallStack(uint[] values, Computer computer)
        {
            for (uint i = 0; i < values.Length; i++)
                computer.MEMC.SetToCallStack(computer.CPU.REGS.SPC++, i + 1);
        }
    }
}
