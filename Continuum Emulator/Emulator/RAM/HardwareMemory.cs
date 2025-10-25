using System;

namespace Continuum93.Emulator.RAM
{
    public class HardwareMemory(uint size)
    {
        public uint ERROR_HANDLER_ADDRESS = size - 1;   // Last variable in the memory
        public uint ERROR_ID = size - 2;

        private readonly uint[] _data = new uint[size];
        
        public uint this[uint addr]
        {
            get
            {
                if (addr >= _data.Length)
                    throw new ArgumentOutOfRangeException(nameof(addr), "Address out of bounds");
                return _data[addr];
            }
            set
            {
                if (addr >= _data.Length)
                    throw new ArgumentOutOfRangeException(nameof(addr), "Address out of bounds");
                _data[addr] = value;
            }
        }

        public void Clear()
        {
            Array.Clear(_data, 0, _data.Length);
        }
    }
}
