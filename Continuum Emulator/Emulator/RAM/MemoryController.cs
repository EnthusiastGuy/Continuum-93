using Continuum93.Tools;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Continuum93.Emulator.RAM
{
    public class MemoryController(Computer computer)
    {
        private const uint SIZE_32K = 32 * 1024;
        private const uint SIZE_64K = 64 * 1024;
        private const uint SIZE_128K = 128 * 1024;
        private const uint SIZE_1MB = 1024 * 1024;
        private const uint SIZE_4MB = 4 * 1024 * 1024;
        private const uint SIZE_16MB = 16 * 1024 * 1024;

        public Memory RAM = new(SIZE_16MB + 3);     // general purpose RAM (ROM data, RAM, VRAM)
        public Memory RSRAM = new(SIZE_4MB);        // RAM reserved for the register stack
        public StackMemory CSRAM = new(SIZE_1MB);   // RAM reserved for the call stack
        public HardwareMemory HMEM = new(SIZE_64K); // Memory reserved for hardware internals

        private readonly Computer _computer = computer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte Fetch()
        {
            byte value = Get8bitFromRAM(_computer.CPU.REGS.IPO);
            //Log.WriteLine(string.Format("Fetching 8 bits ({0}) from RAM at {1}", value, _computer.CPU.REGS.IPO));
            _computer.CPU.REGS.IPO++;
            return value;
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte FetchSigned()
        {
            sbyte value = GetSigned8bitFromRAM(_computer.CPU.REGS.IPO);
            //Log.WriteLine(string.Format("Fetching signed 8 bits ({0}) from RAM at {1}", value, _computer.CPU.REGS.IPO));
            _computer.CPU.REGS.IPO++;
            return value;
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort Fetch16()
        {
            ushort value = Get16bitFromRAM(_computer.CPU.REGS.IPO);
            //Log.WriteLine(string.Format("Fetching 16-bits ({0}) from RAM at {1}", value, _computer.CPU.REGS.IPO));
            _computer.CPU.REGS.IPO += 2;
            return value;
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short Fetch16Signed()
        {
            short value = GetSigned16bitFromRAM(_computer.CPU.REGS.IPO);
            //Log.WriteLine(string.Format("Fetching signed 16-bits signed ({0}) from RAM at {1}", value, _computer.CPU.REGS.IPO));
            _computer.CPU.REGS.IPO += 2;
            return value;
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Fetch24()
        {
            uint value = Get24bitFromRAM(_computer.CPU.REGS.IPO);
            //Log.WriteLine(string.Format("Fetching 24 bits ({0}) from RAM at {1}", value, _computer.CPU.REGS.IPO));
            _computer.CPU.REGS.IPO += 3;
            return value;
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Fetch24Signed()
        {
            int value = GetSigned24bitFromRAM(_computer.CPU.REGS.IPO);
            //Log.WriteLine(string.Format("Fetching signed 24 bits signed ({0}) from RAM at {1}", value, _computer.CPU.REGS.IPO));
            _computer.CPU.REGS.IPO += 3;
            return value;
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Fetch32()
        {
            uint value = Get32bitFromRAM(_computer.CPU.REGS.IPO);
            //Log.WriteLine(string.Format("Fetching 32 bits ({0}) from RAM at {1}", value, _computer.CPU.REGS.IPO));
            _computer.CPU.REGS.IPO += 4;
            return value;
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Fetch32Signed()
        {
            int value = GetSigned32bitFromRAM(_computer.CPU.REGS.IPO);
            //Log.WriteLine(string.Format("Fetching signed 32 bits ({0}) from RAM at {1}", value, _computer.CPU.REGS.IPO));
            _computer.CPU.REGS.IPO += 4;
            return value;
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte GetSafe8bitFromRAM(uint address) => RAM[address % 0x1000000];

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort GetSafe16bitFromRAM(uint address) =>
            Get16bitFromRAM(address % 0x1000000);

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint GetSafe24bitFromRAM(uint address) =>
            Get24bitFromRAM(address % 0x1000000);

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint GetSafe32bitFromRAM(uint address) =>
            Get32bitFromRAM(address % 0x1000000);

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte Get8bitFromRAM(uint address) => RAM[address];

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte GetSigned8bitFromRAM(uint address) => (sbyte)RAM[address];

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort Get16bitFromRAM(uint address) => (ushort)((RAM[address] << 8) + RAM[address + 1]);

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short GetSigned16bitFromRAM(uint address) => (short)((RAM[address] << 8) + RAM[address + 1]);

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Get24bitFromRAM(uint address) => (uint)((RAM[address] << 16) + (RAM[address + 1] << 8) + RAM[address + 2]);

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetSigned24bitFromRAM(uint address)
        {
            int value = (RAM[address] << 16) + (RAM[address + 1] << 8) + RAM[address + 2];

            if ((value & 0x00800000) == 0)
                value &= 0x00FFFFFF;
            else
                value = (int)(value | 0xFF000000);

            return value;
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Get32bitFromRAM(uint address) => (uint)((RAM[address] << 24) + (RAM[address + 1] << 16) + (RAM[address + 2] << 8) + RAM[address + 3]);

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetSigned32bitFromRAM(uint address) => (RAM[address] << 24) + (RAM[address + 1] << 16) + (RAM[address + 2] << 8) + RAM[address + 3];

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetFloatFromRAM(uint address) => FloatPointUtils.BytesToFloat(
            [
                RAM[address % 0x1000000], RAM[(address + 1) % 0x1000000], RAM[(address + 2) % 0x1000000], RAM[(address + 3) % 0x1000000]
            ]);

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set8bitToRAM(uint address, byte value) => RAM[address] = value;

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set16bitToRAM(uint address, ushort value)
        {
            ushort val = (ushort)(value & 0b1111111111111111);
            RAM[address] = (byte)(val >> 8);
            RAM[address + 1] = (byte)(val & 0b11111111);
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set24bitToRAM(uint address, uint value)
        {
            uint val = value & 0b111111111111111111111111;
            RAM[address] = (byte)(val >> 16);
            RAM[address + 1] = (byte)(val >> 8 & 0b11111111);
            RAM[address + 2] = (byte)(val & 0b11111111);
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set32bitToRAM(uint address, uint value)
        {
            RAM[address] = (byte)(value >> 24);
            RAM[address + 1] = (byte)(value >> 16 & 0b11111111);
            RAM[address + 2] = (byte)(value >> 8 & 0b11111111);
            RAM[address + 3] = (byte)(value & 0b11111111);
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetFloatToRam(uint address, float value)
        {
            byte[] bytes = FloatPointUtils.FloatToBytes(value);
            RAM[address] = bytes[0];
            RAM[address + 1] = bytes[1];
            RAM[address + 2] = bytes[2];
            RAM[address + 3] = bytes[3];
        }

        // Gets a zero terminated string starting at given address
        public string GetStringAt(uint address)
        {
            int index = Array.IndexOf(RAM.Data, (byte)0, (int)address);
            return index >= 0
                ? Encoding.UTF8.GetString(RAM.Data, (int)address, index - (int)address)
                : string.Empty;
        }

        public void SetStringAt(string str, uint address)
        {
            // Convert the string to a UTF8 byte array
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);

            // Copy each byte from the string to the RAM array starting at the given address
            for (int i = 0; i < bytes.Length; i++)
            {
                RAM[address + (uint)i] = bytes[i];
            }

            // Add a null terminator at the end of the string
            RAM[address + (uint)bytes.Length] = 0;
        }

        public byte[] GetStringBytesAt(uint address)
        {
            int length = 0;
            for (int i = 0; i < RAM.Size; i++)
            {
                if (RAM[address + (uint)i] == 0)
                {
                    length = i;
                    break;
                }
            }

            byte[] response = new byte[length];
            Array.Copy(RAM.Data, (int)address, response, 0, length);

            return response;
        }

        public void ClearAllRAM() => RAM.Clear();

        public void ClearHMEM() => HMEM.Clear();

        public void ResetAllStacks()
        {
            RSRAM.Clear();
            _computer.CPU.REGS.SPR = 0;
            CSRAM.Clear();
            _computer.CPU.REGS.SPC = 0;
        }

        public uint GetRegStack(uint address) => RSRAM[address];

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint GetFromCallStack(uint address)
        {
            uint val = CSRAM[address];
            return val;
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetToCallStack(uint address, uint value) => CSRAM[address] = value;

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte Get8BitFromRegStack(uint address) => RSRAM[address];

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set8BitToRegStack(uint address, byte value) => RSRAM[address] = value;

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort Get16BitFromRegStack(uint address) => (ushort)((RSRAM[address] << 8) + RSRAM[address + 1]);

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set16BitToRegStack(uint address, ushort value)
        {
            RSRAM[address] = (byte)(value >> 8);
            RSRAM[address + 1] = (byte)(value & 0b11111111);
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Get24BitFromRegStack(uint address) => (uint)((RSRAM[address] << 16) + (RSRAM[address + 1] << 8) + RSRAM[address + 2]);

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set24BitToRegStack(uint address, uint value)
        {
            uint val = value & 0xFFFFFF;
            RSRAM[address] = (byte)(val >> 16);
            RSRAM[address + 1] = (byte)(val >> 8 & 0b11111111);
            RSRAM[address + 2] = (byte)(val & 0b11111111);
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Get32BitFromRegStack(uint address) => (uint)((RSRAM[address] << 24) + (RSRAM[address + 1] << 16) + (RSRAM[address + 2] << 8) + RSRAM[address + 3]);

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set32BitToRegStack(uint address, uint value)
        {
            RSRAM[address] = (byte)(value >> 24);
            RSRAM[address + 1] = (byte)(value >> 16 & 0b11111111);
            RSRAM[address + 2] = (byte)(value >> 8 & 0b11111111);
            RSRAM[address + 3] = (byte)(value & 0b11111111);
        }

        public byte[] GetMemoryPointedByAllAddressingRegisters()
        {
            int lengthPerMemorySample = 16;
            byte[] response = new byte[26 * lengthPerMemorySample];

            for (byte i = 0; i < 26; i++)
            {
                uint addr = _computer.CPU.REGS.Get24BitRegister(i);
                byte[] data = GetMemoryWrapped(addr, lengthPerMemorySample);
                Array.Copy(data, 0, response, i * lengthPerMemorySample, lengthPerMemorySample);
            }

            return response;
        }

        public byte[] GetMemoryWrapped(uint address, int length)
        {
            byte[] response = new byte[length];
            for (int i = 0; i < length; i++)
            {
                uint cAddr = (uint)(address + i);
                if (cAddr > 0xFFFFFF)
                {
                    cAddr -= 0xFFFFFF;
                }

                response[i] = RAM.Data[cAddr];
            }

            return response;
        }

        public byte[] DumpMemAt(uint address, int length)
        {
            byte[] response = new byte[length];
            Array.Copy(RAM.Data, (int)address, response, 0, length);
            return response;
        }
    }
}
