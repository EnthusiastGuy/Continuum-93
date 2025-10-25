using System;

namespace Continuum93.Emulator.RAM
{
    public class StackMemory
    {
        private readonly uint[] _data;

        public StackMemory(uint size)
        {
            _data = new uint[size];
        }

        public uint this[uint addr]
        {
            get => _data[addr];
            set => _data[addr] = value;
        }

        public uint[] GetMemoryAt(uint address, int length)
        {
            uint[] response = new uint[length];
            Array.Copy(_data, address, response, 0, length);

            return response;
        }

        public void Clear()
        {
            Array.Clear(_data, 0, _data.Length);
        }
    }
}
