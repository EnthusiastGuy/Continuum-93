using Continuum93.Emulator.Mnemonics;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Continuum93.Emulator.CPU
{
    public class Registers
    {
        static readonly byte[] Next1 = new byte[26];
        static readonly byte[] Next2 = new byte[26];
        static readonly byte[] Next3 = new byte[26];

        private byte registerBank = 0;
        private int bankOffset = 0;
        private byte[] _gpDataFlat;

        private uint _spc;     // Call stack pointer
        private uint _spr;     // Register stack pointer
        private uint _ipo;     // Instruction pointer

        private readonly Computer _computer;

        static Registers()
        {
            for (byte i = 0; i < 26; i++)
            {
                // Next1: i+1 mod 26  →  if i == 25, wrap to 0
                byte v1 = (byte)(i + 1);
                Next1[i] = (byte)(v1 == 26 ? 0 : v1);

                // Next2: i+2 mod 26 → if ≥26 subtract 26
                byte v2 = (byte)(i + 2);
                Next2[i] = (byte)(v2 >= 26 ? v2 - 26 : v2);

                // Next3: i+3 mod 26 → if ≥26 subtract 26
                byte v3 = (byte)(i + 3);
                Next3[i] = (byte)(v3 >= 26 ? v3 - 26 : v3);
            }
        }

        public Registers(Computer computer)
        {
            _computer = computer;
            _gpDataFlat = new byte[256 * 26];
        }

        public void SetRegistersBetween(byte index1, byte index2, byte[] data)
        {
            // if index1 <= index2: copy data[0..] to flat[bankOffset+index1 .. bankOffset+index2]
            if (index1 <= index2)
            {
                int length = index2 - index1 + 1;
                Array.Copy(
                    /* sourceArray */   data,
                    /* sourceIndex */   0,
                    /* destArray */     _gpDataFlat,
                    /* destIndex */     bankOffset + index1,
                    /* length */        length
                );
            }
            else
            {
                // descending: data[0] → flat[index1], data[1] → flat[index1-1], … until index2
                int length = index1 - index2 + 1;
                for (int i = 0; i < length; i++)
                {
                    _gpDataFlat[bankOffset + index1 - i] = data[i];
                }
            }
        }

        public byte[] GetRegistersBetween(byte index1, byte index2)
        {
            if (index1 <= index2)
            {
                int length = index2 - index1 + 1;
                var result = new byte[length];
                Array.Copy(
                    /* sourceArray */   _gpDataFlat,
                    /* sourceIndex */   bankOffset + index1,
                    /* destArray */     result,
                    /* destIndex */     0,
                    /* length */        length
                );
                return result;
            }
            else
            {
                int length = index1 - index2 + 1;
                var result = new byte[length];
                for (int i = 0; i < length; i++)
                {
                    result[i] = _gpDataFlat[bankOffset + index1 - i];
                }
                return result;
            }
        }

        public byte[] GetCurrentRegisterPageData()
        {
            // grab a full 26-byte slice
            var page = new byte[26];
            Array.Copy(
                /* sourceArray */   _gpDataFlat,
                /* sourceIndex */   bankOffset,
                /* destArray */     page,
                /* destIndex */     0,
                /* length */        26
            );
            return page;
        }


        public void ClearAll()
        {
            Array.Clear(_gpDataFlat, 0, _gpDataFlat.Length);
            _spc = _spr = _ipo = 0;
        }

        public uint SPC
        {
            get { return _spc; }
            set => _spc = value;
        }

        public uint SPR
        {
            get { return _spr; }
            set => _spr = value;
        }

        public uint IPO
        {
            get { return _ipo; }
            set => _ipo = value;
        }

        public void SetRegisterBank(byte value)
        {
            registerBank = value;
            bankOffset = value * 26; // Each register bank has 26 registers, so we calculate the offset.
        }

        public byte GetRegisterBank()
        {
            return registerBank;
        }

        // ADD
        public void AddTo8BitRegister(byte index, byte value)
        {
            _computer.CPU.FLAGS.Update8BitAddFlags(_gpDataFlat[bankOffset + index], value);
            _gpDataFlat[bankOffset + index] += value;
        }

        public void AddTo16BitRegister(byte index, ushort value)
        {
            ushort rVal = Get16BitRegister(index);
            _computer.CPU.FLAGS.Update16BitAddFlags(rVal, value);
            Set16BitRegister(index, (ushort)(rVal + value));
        }

        public void AddTo24BitRegister(byte index, uint value)
        {
            uint rVal = Get24BitRegister(index);
            _computer.CPU.FLAGS.Update24BitAddFlags(rVal, value);
            Set24BitRegister(index, rVal + value);
        }

        public void AddTo32BitRegister(byte index, uint value)
        {
            uint rVal = Get32BitRegister(index);
            _computer.CPU.FLAGS.Update32BitAddFlags(rVal, value);
            Set32BitRegister(index, rVal + value);
        }

        public byte Add8BitValues(byte v1, byte v2)
        {
            _computer.CPU.FLAGS.Update8BitAddFlags(v1, v2);
            return (byte)(v1 + v2);
        }

        public ushort Add16BitValues(ushort v1, ushort v2)
        {
            _computer.CPU.FLAGS.Update16BitAddFlags(v1, v2);
            return (ushort)(v1 + v2);
        }

        public uint Add24BitValues(uint v1, uint v2)
        {
            _computer.CPU.FLAGS.Update24BitAddFlags(v1, v2);
            return v1 + v2 & 0xFFFFFF;
        }

        public uint Add32BitValues(uint v1, uint v2)
        {
            _computer.CPU.FLAGS.Update32BitAddFlags(v1, v2);
            return v1 + v2;
        }

        // SUB
        public void SubtractFrom8BitRegister(byte index, byte value)
        {
            _computer.CPU.FLAGS.Update8BitSubFlags(_gpDataFlat[bankOffset + index], value);
            _gpDataFlat[bankOffset + index] -= value;
        }

        public void SubtractFrom16BitRegister(byte index, ushort value)
        {
            ushort rVal = Get16BitRegister(index);
            _computer.CPU.FLAGS.Update16BitSubFlags(rVal, value);
            Set16BitRegister(index, (ushort)(rVal - value));
        }

        public void SubtractFrom24BitRegister(byte index, uint value)
        {
            uint rVal = Get24BitRegister(index);
            _computer.CPU.FLAGS.Update24BitSubFlags(rVal, value);
            Set24BitRegister(index, rVal - value);
        }

        public void SubtractFrom32BitRegister(byte index, uint value)
        {
            uint rVal = Get32BitRegister(index);
            _computer.CPU.FLAGS.Update32BitSubFlags(rVal, value);
            Set32BitRegister(index, rVal - value);
        }

        public byte Sub8BitValues(byte v1, byte v2)
        {
            _computer.CPU.FLAGS.Update8BitSubFlags(v1, v2);
            return (byte)(v1 - v2);
        }

        public ushort Sub16BitValues(ushort v1, ushort v2)
        {
            _computer.CPU.FLAGS.Update16BitSubFlags(v1, v2);
            return (ushort)(v1 - v2);
        }

        public uint Sub24BitValues(uint v1, uint v2)
        {
            _computer.CPU.FLAGS.Update24BitSubFlags(v1, v2);
            return v1 - v2 & 0xFFFFFF;
        }

        public uint Sub32BitValues(uint v1, uint v2)
        {
            _computer.CPU.FLAGS.Update32BitSubFlags(v1, v2);
            return v1 - v2;
        }

        //SL
        public void Shift8BitRegisterLeft(byte index, byte value)
        {
            if (value == 0) return;
            byte rVal = Get8BitRegister(index);
            Set8BitRegister(index, (byte)(rVal << value));
            // If value >= 8, any non-zero value sets carry. Otherwise, check overflow.
            bool carry = (value >= 8) ? (rVal != 0) : (rVal >> (8 - value)) > 0;
            _computer.CPU.FLAGS.SetCarry(carry);
        }

        public void Shift16BitRegisterLeft(byte index, byte value)
        {
            if (value == 0) return;
            ushort rVal = Get16BitRegister(index);
            Set16BitRegister(index, (ushort)(rVal << value));
            bool carry = (value >= 16) ? (rVal != 0) : (rVal >> (16 - value)) > 0;
            _computer.CPU.FLAGS.SetCarry(carry);
        }

        public void Shift24BitRegisterLeft(byte index, byte value)
        {
            if (value == 0) return;
            uint rVal = Get24BitRegister(index) & 0xFFFFFF; // Ensure 24-bit clean
            Set24BitRegister(index, (rVal << value) & 0xFFFFFF);
            bool carry = (value >= 24) ? (rVal != 0) : (rVal >> (24 - value)) > 0;
            _computer.CPU.FLAGS.SetCarry(carry);
        }

        public void Shift32BitRegisterLeft(byte index, byte value)
        {
            if (value == 0) return;
            uint rVal = Get32BitRegister(index);
            Set32BitRegister(index, rVal << value);
            bool carry = (value >= 32) ? (rVal != 0) : (rVal >> (32 - value)) > 0;
            _computer.CPU.FLAGS.SetCarry(carry);
        }

        //SR
        public void Shift8BitRegisterRight(byte index, byte value)
        {
            if (value == 0) return;
            byte rVal = Get8BitRegister(index);
            Set8BitRegister(index, (byte)(rVal >> value));
            // Mask to see if any bits in the "shifted out" range were 1
            byte mask = (byte)((1 << Math.Min((int)value, 8)) - 1);
            _computer.CPU.FLAGS.SetCarry((rVal & mask) != 0);
        }

        public void Shift16BitRegisterRight(byte index, byte value)
        {
            if (value == 0) return;
            ushort rVal = Get16BitRegister(index);
            Set16BitRegister(index, (ushort)(rVal >> value));
            int mask = (1 << Math.Min((int)value, 16)) - 1;
            _computer.CPU.FLAGS.SetCarry((rVal & mask) != 0);
        }

        public void Shift24BitRegisterRight(byte index, byte value)
        {
            if (value == 0) return;
            uint rVal = Get24BitRegister(index) & 0xFFFFFF;
            Set24BitRegister(index, (rVal >> value) & 0xFFFFFF);
            // Mask the lowest 'value' bits
            uint mask = (1u << (int)Math.Min((int)value, 24)) - 1;
            _computer.CPU.FLAGS.SetCarry((rVal & mask) != 0);
        }

        public void Shift32BitRegisterRight(byte index, byte value)
        {
            if (value == 0) return;
            uint rVal = Get32BitRegister(index);
            Set32BitRegister(index, rVal >> value);
            // Use a long for the mask to avoid overflow if value is 32
            uint mask = (value >= 32) ? 0xFFFFFFFF : (1u << (int)value) - 1;
            _computer.CPU.FLAGS.SetCarry((rVal & mask) != 0);
        }

        //RL
        public void Roll8BitRegisterLeft(byte index, byte value)
        {
            value = (byte)(value % 8);
            if (value == 0) return;
            byte rVal = Get8BitRegister(index);
            Set8BitRegister(index, (byte)((rVal << value) | (rVal >> (8 - value))));
            // Carry is the last bit rotated out of the MSB
            _computer.CPU.FLAGS.SetCarry((rVal >> (8 - value)) > 0);
        }

        public void Roll16BitRegisterLeft(byte index, byte value)
        {
            value = (byte)(value % 16);
            if (value == 0) return;
            ushort rVal = Get16BitRegister(index);
            Set16BitRegister(index, (ushort)((rVal << value) | (rVal >> (16 - value))));
            _computer.CPU.FLAGS.SetCarry((rVal >> (16 - value)) > 0);
        }

        public void Roll24BitRegisterLeft(byte index, byte value)
        {
            value = (byte)(value % 24);
            if (value == 0) return;
            uint rVal = Get24BitRegister(index) & 0xFFFFFF;
            uint result = ((rVal << value) | (rVal >> (24 - value))) & 0xFFFFFF;
            Set24BitRegister(index, result);
            _computer.CPU.FLAGS.SetCarry((rVal >> (24 - value)) > 0);
        }

        public void Roll32BitRegisterLeft(byte index, byte value)
        {
            value = (byte)(value % 32);
            if (value == 0) return;
            uint rVal = Get32BitRegister(index);
            Set32BitRegister(index, (rVal << value) | (rVal >> (32 - value)));
            _computer.CPU.FLAGS.SetCarry((rVal >> (32 - value)) > 0);
        }

        //RR
        public void Roll8BitRegisterRight(byte index, byte value)
        {
            value = (byte)(value % 8);
            if (value == 0) return;
            byte rVal = Get8BitRegister(index);
            Set8BitRegister(index, (byte)((rVal >> value) | (rVal << (8 - value))));
            // Mask the bits that are being moved from the LSB to the MSB
            int mask = (1 << value) - 1;
            _computer.CPU.FLAGS.SetCarry((rVal & mask) != 0);
        }

        public void Roll16BitRegisterRight(byte index, byte value)
        {
            value = (byte)(value % 16);
            if (value == 0) return;
            ushort rVal = Get16BitRegister(index);
            Set16BitRegister(index, (ushort)((rVal >> value) | (rVal << (16 - value))));
            int mask = (1 << value) - 1;
            _computer.CPU.FLAGS.SetCarry((rVal & mask) != 0);
        }

        public void Roll24BitRegisterRight(byte index, byte value)
        {
            value = (byte)(value % 24);
            if (value == 0) return;
            uint rVal = Get24BitRegister(index) & 0xFFFFFF;
            uint result = ((rVal >> value) | (rVal << (24 - value))) & 0xFFFFFF;
            Set24BitRegister(index, result);
            uint mask = (1u << value) - 1;
            _computer.CPU.FLAGS.SetCarry((rVal & mask) != 0);
        }

        public void Roll32BitRegisterRight(byte index, byte value)
        {
            value = (byte)(value % 32);
            if (value == 0) return;
            uint rVal = Get32BitRegister(index);
            Set32BitRegister(index, (rVal >> value) | (rVal << (32 - value)));
            // Use uint cast for mask to avoid 31-bit limit of signed int
            uint mask = (1u << value) - 1;
            _computer.CPU.FLAGS.SetCarry((rVal & mask) != 0);
        }

        // SET
        public void Set8BitBit(byte index, byte value)
        {
            Set8BitRegister(index, (byte)(Get8BitRegister(index) | (1 << value)));
        }

        public void Set16BitBit(byte index, byte value)
        {
            Set16BitRegister(index, (ushort)(Get16BitRegister(index) | (1 << value)));
        }

        public void Set24BitBit(byte index, byte value)
        {
            Set24BitRegister(index, (uint)(Get24BitRegister(index) | (1u << value)));
        }

        public void Set32BitBit(byte index, byte value)
        {
            Set32BitRegister(index, (uint)(Get32BitRegister(index) | (1u << value)));
        }

        // RES
        public void Reset8BitBit(byte index, byte bitPosition)
        {
            // Get a direct ref to the target register byte
            ref byte reg = ref _gpDataFlat[bankOffset + index];

            // Mask out the specified bit in-place
            reg &= (byte)~(1 << bitPosition);
        }

        public void Reset16BitBit(byte index, byte value)
        {
            ushort rVal = Get16BitRegister(index);
            Set16BitRegister(index, (ushort)(rVal & ~(1 << value)));
        }

        public void Reset24BitBit(byte index, byte value)
        {
            uint rVal = Get24BitRegister(index);
            Set24BitRegister(index, (uint)(rVal & ~(1 << value)));
        }

        public void Reset32BitBit(byte index, byte value)
        {
            uint rVal = Get32BitRegister(index);
            Set32BitRegister(index, (uint)(rVal & ~(1 << value)));
        }

        // BIT - to be obsolete soon
        public void Test8BitBit(byte index, byte value)
        {
            _computer.CPU.FLAGS.SetValueByIndexFast(0, (_gpDataFlat[bankOffset + index] & 1 << value) != 0);    // Z flag
        }

        public void Test16BitBit(byte index, byte value)
        {
            ushort rVal = Get16BitRegister(index);
            _computer.CPU.FLAGS.SetValueByIndexFast(0, (rVal & 1 << value) != 0);    // Z flag
        }

        public void Test24BitBit(byte index, byte value)
        {
            uint rVal = Get24BitRegister(index);
            _computer.CPU.FLAGS.SetValueByIndexFast(0, (rVal & 1 << value) != 0);    // Z flag
        }

        public void Test32BitBit(byte index, byte value)
        {
            uint rVal = Get32BitRegister(index);
            _computer.CPU.FLAGS.SetValueByIndexFast(0, (rVal & 1 << value) != 0);    // Z flag
        }

        // BIT (value-based helpers for ExBIT: test a bit in a supplied value, flags only)
        public void Test8BitValue(byte value, byte bitPosition)
        {
            // 8-bit positions are 0..7
            bitPosition &= 0x07;
            _computer.CPU.FLAGS.SetValueByIndexFast(0, (value & (1 << bitPosition)) != 0);
        }

        public void Test16BitValue(ushort value, byte bitPosition)
        {
            // 16-bit positions are 0..15
            bitPosition &= 0x0F;
            _computer.CPU.FLAGS.SetValueByIndexFast(0, (value & (1u << bitPosition)) != 0);
        }

        public void Test24BitValue(uint value, byte bitPosition)
        {
            // 24-bit positions are 0..23; ensure 24-bit clean
            bitPosition &= 0x17; // 0b10111 => 0..23
            value &= 0xFFFFFFu;
            _computer.CPU.FLAGS.SetValueByIndexFast(0, (value & (1u << bitPosition)) != 0);
        }

        public void Test32BitValue(uint value, byte bitPosition)
        {
            // 32-bit positions are 0..31
            bitPosition &= 0x1F;
            _computer.CPU.FLAGS.SetValueByIndexFast(0, (value & (1u << bitPosition)) != 0);
        }

        // AND
        public void And8Bit(byte index, byte value)
        {
            byte result = (byte)(_gpDataFlat[bankOffset + index] & value);
            _gpDataFlat[bankOffset + index] = result;
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void And16Bit(byte index, ushort value)
        {
            ushort result = (ushort)(Get16BitRegister(index) & value);
            Set16BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void And24Bit(byte index, uint value)
        {
            uint result = Get24BitRegister(index) & value;
            Set24BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void And32Bit(byte index, uint value)
        {
            uint result = Get32BitRegister(index) & value;
            Set32BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void And8BitRegToMem(uint address, byte value)
        {
            byte ramVal = _computer.MEMC.Get8bitFromRAM(address);
            byte result = (byte)(ramVal & value);
            _computer.MEMC.Set8bitToRAM(address, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void And16BitRegToMem(uint address, ushort value)
        {
            ushort ramVal = _computer.MEMC.Get16bitFromRAM(address);
            ushort result = (ushort)(ramVal & value);
            _computer.MEMC.Set16bitToRAM(address, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void And24BitRegToMem(uint address, uint value)
        {
            uint ramVal = _computer.MEMC.Get24bitFromRAM(address);
            uint result = ramVal & value;
            _computer.MEMC.Set24bitToRAM(address, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void And32BitRegToMem(uint address, uint value)
        {
            uint ramVal = _computer.MEMC.Get32bitFromRAM(address);
            uint result = ramVal & value;
            _computer.MEMC.Set32bitToRAM(address, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        // new
        public void AndMemTo8BitReg(byte index, uint address)
        {
            byte result = (byte)(
                _computer.MEMC.Get8bitFromRAM(address) & _gpDataFlat[bankOffset + index]);

            _gpDataFlat[bankOffset + index] = result;
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void AndMemTo16BitReg(byte index, uint address)
        {
            ushort result = (ushort)(
                _computer.MEMC.Get16bitFromRAM(address) & Get16BitRegister(index));

            Set16BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void AndMemTo24BitReg(byte index, uint address)
        {
            uint result = _computer.MEMC.Get24bitFromRAM(address) & Get24BitRegister(index);

            Set24BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void AndMemTo32BitReg(byte index, uint address)
        {
            uint result = _computer.MEMC.Get32bitFromRAM(address) & Get32BitRegister(index);

            Set32BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }


        // NAND
        public void NAnd8Bit(byte index, byte value)
        {
            byte result = (byte)~(_gpDataFlat[bankOffset + index] & value);
            _gpDataFlat[bankOffset + index] = result;
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void NAnd16Bit(byte index, ushort value)
        {
            ushort result = (ushort)~(Get16BitRegister(index) & value);
            Set16BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void NAnd24Bit(byte index, uint value)
        {
            uint result = ~(Get24BitRegister(index) & value);
            Set24BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void NAnd32Bit(byte index, uint value)
        {
            uint result = ~(Get32BitRegister(index) & value);
            Set32BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void NAnd8BitMem(uint address, byte value)
        {
            byte ramVal = _computer.MEMC.Get8bitFromRAM(address);
            byte result = (byte)~(ramVal & value);
            _computer.MEMC.Set8bitToRAM(address, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void NAnd16BitMem(uint address, ushort value)
        {
            ushort ramVal = _computer.MEMC.Get16bitFromRAM(address);
            ushort result = (ushort)~(ramVal & value);
            _computer.MEMC.Set16bitToRAM(address, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void NAnd24BitMem(uint address, uint value)
        {
            uint ramVal = _computer.MEMC.Get24bitFromRAM(address);
            uint result = ~(ramVal & value);
            _computer.MEMC.Set24bitToRAM(address, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void NAnd32BitMem(uint address, uint value)
        {
            uint ramVal = _computer.MEMC.Get32bitFromRAM(address);
            uint result = ~(ramVal & value);
            _computer.MEMC.Set32bitToRAM(address, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        // new
        public void NandMemTo8BitReg(byte index, uint address)
        {
            byte result = (byte)(
                ~(_computer.MEMC.Get8bitFromRAM(address) & _gpDataFlat[bankOffset + index]));

            _gpDataFlat[bankOffset + index] = result;
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void NandMemTo16BitReg(byte index, uint address)
        {
            ushort result = (ushort)(
                ~(_computer.MEMC.Get16bitFromRAM(address) & Get16BitRegister(index)));

            Set16BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void NandMemTo24BitReg(byte index, uint address)
        {
            uint result = ~(_computer.MEMC.Get24bitFromRAM(address) & Get24BitRegister(index));

            Set24BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void NandMemTo32BitReg(byte index, uint address)
        {
            uint result = ~(_computer.MEMC.Get32bitFromRAM(address) & Get32BitRegister(index));

            Set32BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        // OR
        public void Or8Bit(byte index, byte value)
        {
            byte result = (byte)(_gpDataFlat[bankOffset + index] | value);
            _gpDataFlat[bankOffset + index] = result;
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void Or16Bit(byte index, ushort value)
        {
            ushort result = (ushort)(Get16BitRegister(index) | value);
            Set16BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void Or24Bit(byte index, uint value)
        {
            uint result = Get24BitRegister(index) | value;
            Set24BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void Or32Bit(byte index, uint value)
        {
            uint result = Get32BitRegister(index) | value;
            Set32BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void Or8BitMem(uint address, byte value)
        {
            byte ramVal = _computer.MEMC.Get8bitFromRAM(address);
            byte result = (byte)(ramVal | value);
            _computer.MEMC.Set8bitToRAM(address, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void Or16BitMem(uint address, ushort value)
        {
            ushort ramVal = _computer.MEMC.Get16bitFromRAM(address);
            ushort result = (ushort)(ramVal | value);
            _computer.MEMC.Set16bitToRAM(address, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void Or24BitMem(uint address, uint value)
        {
            uint ramVal = _computer.MEMC.Get24bitFromRAM(address);
            uint result = ramVal | value;
            _computer.MEMC.Set24bitToRAM(address, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void Or32BitMem(uint address, uint value)
        {
            uint ramVal = _computer.MEMC.Get32bitFromRAM(address);
            uint result = ramVal | value;
            _computer.MEMC.Set32bitToRAM(address, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        // new
        public void OrMemTo8BitReg(byte index, uint address)
        {
            byte result = (byte)(
                _computer.MEMC.Get8bitFromRAM(address) | _gpDataFlat[bankOffset + index]);

            _gpDataFlat[bankOffset + index] = result;
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void OrMemTo16BitReg(byte index, uint address)
        {
            ushort result = (ushort)(
                _computer.MEMC.Get16bitFromRAM(address) | Get16BitRegister(index));

            Set16BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void OrMemTo24BitReg(byte index, uint address)
        {
            uint result = _computer.MEMC.Get24bitFromRAM(address) | Get24BitRegister(index);

            Set24BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void OrMemTo32BitReg(byte index, uint address)
        {
            uint result = _computer.MEMC.Get32bitFromRAM(address) | Get32BitRegister(index);

            Set32BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        // NOR
        public void NOr8Bit(byte index, byte value)
        {
            byte result = (byte)~(_gpDataFlat[bankOffset + index] | value);
            _gpDataFlat[bankOffset + index] = result;
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void NOr16Bit(byte index, ushort value)
        {
            ushort result = (ushort)~(Get16BitRegister(index) | value);
            Set16BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void NOr24Bit(byte index, uint value)
        {
            uint result = ~(Get24BitRegister(index) | value);
            Set24BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void NOr32Bit(byte index, uint value)
        {
            uint result = ~(Get32BitRegister(index) | value);
            Set32BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void NOr8BitMem(uint address, byte value)
        {
            byte ramVal = _computer.MEMC.Get8bitFromRAM(address);
            byte result = (byte)~(ramVal | value);
            _computer.MEMC.Set8bitToRAM(address, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void NOr16BitMem(uint address, ushort value)
        {
            ushort ramVal = _computer.MEMC.Get16bitFromRAM(address);
            ushort result = (ushort)~(ramVal | value);
            _computer.MEMC.Set16bitToRAM(address, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void NOr24BitMem(uint address, uint value)
        {
            uint ramVal = _computer.MEMC.Get24bitFromRAM(address);
            uint result = ~(ramVal | value);
            _computer.MEMC.Set24bitToRAM(address, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void NOr32BitMem(uint address, uint value)
        {
            uint ramVal = _computer.MEMC.Get32bitFromRAM(address);
            uint result = ~(ramVal | value);
            _computer.MEMC.Set32bitToRAM(address, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        // new
        public void NorMemTo8BitReg(byte index, uint address)
        {
            byte result = (byte)(
                ~(_computer.MEMC.Get8bitFromRAM(address) | _gpDataFlat[bankOffset + index]));

            _gpDataFlat[bankOffset + index] = result;
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void NorMemTo16BitReg(byte index, uint address)
        {
            ushort result = (ushort)(
                ~(_computer.MEMC.Get16bitFromRAM(address) | Get16BitRegister(index)));

            Set16BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void NorMemTo24BitReg(byte index, uint address)
        {
            uint result = ~(_computer.MEMC.Get24bitFromRAM(address) | Get24BitRegister(index));

            Set24BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void NorMemTo32BitReg(byte index, uint address)
        {
            uint result = ~(_computer.MEMC.Get32bitFromRAM(address) | Get32BitRegister(index));

            Set32BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        // XOR
        public void Xor8Bit(byte index, byte value)
        {
            byte result = (byte)(_gpDataFlat[bankOffset + index] ^ value);
            _gpDataFlat[bankOffset + index] = result;
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void Xor16Bit(byte index, ushort value)
        {
            ushort result = (ushort)(Get16BitRegister(index) ^ value);
            Set16BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void Xor24Bit(byte index, uint value)
        {
            uint result = Get24BitRegister(index) ^ value;
            Set24BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void Xor32Bit(byte index, uint value)
        {
            uint result = Get32BitRegister(index) ^ value;
            Set32BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void Xor8BitMem(uint address, byte value)
        {
            byte ramVal = _computer.MEMC.Get8bitFromRAM(address);
            byte result = (byte)(ramVal ^ value);
            _computer.MEMC.Set8bitToRAM(address, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void Xor16BitMem(uint address, ushort value)
        {
            ushort ramVal = _computer.MEMC.Get16bitFromRAM(address);
            ushort result = (ushort)(ramVal ^ value);
            _computer.MEMC.Set16bitToRAM(address, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void Xor24BitMem(uint address, uint value)
        {
            uint ramVal = _computer.MEMC.Get24bitFromRAM(address);
            uint result = ramVal ^ value;
            _computer.MEMC.Set24bitToRAM(address, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void Xor32BitMem(uint address, uint value)
        {
            uint ramVal = _computer.MEMC.Get32bitFromRAM(address);
            uint result = ramVal ^ value;
            _computer.MEMC.Set32bitToRAM(address, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        // new
        public void XorMemTo8BitReg(byte index, uint address)
        {
            byte result = (byte)(
                _computer.MEMC.Get8bitFromRAM(address) ^ _gpDataFlat[bankOffset + index]);

            _gpDataFlat[bankOffset + index] = result;
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void XorMemTo16BitReg(byte index, uint address)
        {
            ushort result = (ushort)(
                _computer.MEMC.Get16bitFromRAM(address) ^ Get16BitRegister(index));

            Set16BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void XorMemTo24BitReg(byte index, uint address)
        {
            uint result = _computer.MEMC.Get24bitFromRAM(address) ^ Get24BitRegister(index);

            Set24BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void XorMemTo32BitReg(byte index, uint address)
        {
            uint result = _computer.MEMC.Get32bitFromRAM(address) ^ Get32BitRegister(index);

            Set32BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        // XNOR
        public void XNor8Bit(byte index, byte value)
        {
            byte result = (byte)~(_gpDataFlat[bankOffset + index] ^ value);
            _gpDataFlat[bankOffset + index] = result;
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void XNor16Bit(byte index, ushort value)
        {
            ushort result = (ushort)~(Get16BitRegister(index) ^ value);
            Set16BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void XNor24Bit(byte index, uint value)
        {
            uint result = ~(Get24BitRegister(index) ^ value);
            Set24BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void XNor32Bit(byte index, uint value)
        {
            uint result = ~(Get32BitRegister(index) ^ value);
            Set32BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void XNor8BitMem(uint address, byte value)
        {
            byte ramVal = _computer.MEMC.Get8bitFromRAM(address);
            byte result = (byte)~(ramVal ^ value);
            _computer.MEMC.Set8bitToRAM(address, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void XNor16BitMem(uint address, ushort value)
        {
            ushort ramVal = _computer.MEMC.Get16bitFromRAM(address);
            ushort result = (ushort)~(ramVal ^ value);
            _computer.MEMC.Set16bitToRAM(address, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void XNor24BitMem(uint address, uint value)
        {
            uint ramVal = _computer.MEMC.Get24bitFromRAM(address);
            uint result = ~(ramVal ^ value);
            _computer.MEMC.Set24bitToRAM(address, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void XNor32BitMem(uint address, uint value)
        {
            uint ramVal = _computer.MEMC.Get32bitFromRAM(address);
            uint result = ~(ramVal ^ value);
            _computer.MEMC.Set32bitToRAM(address, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        // new
        public void XNorMemTo8BitReg(byte index, uint address)
        {
            byte result = (byte)(
                ~(_computer.MEMC.Get8bitFromRAM(address) ^ _gpDataFlat[bankOffset + index]));

            _gpDataFlat[bankOffset + index] = result;
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void XNorMemTo16BitReg(byte index, uint address)
        {
            ushort result = (ushort)(
                ~(_computer.MEMC.Get16bitFromRAM(address) ^ Get16BitRegister(index)));

            Set16BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void XNorMemTo24BitReg(byte index, uint address)
        {
            uint result = ~(_computer.MEMC.Get24bitFromRAM(address) ^ Get24BitRegister(index));

            Set24BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void XNorMemTo32BitReg(byte index, uint address)
        {
            uint result = ~(_computer.MEMC.Get32bitFromRAM(address) ^ Get32BitRegister(index));

            Set32BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        // INV
        public void Inv8BitRegister(byte index)
        {
            byte result = (byte)~_gpDataFlat[bankOffset + index];
            _gpDataFlat[bankOffset + index] = result;
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void Inv16BitRegister(byte index)
        {
            ushort result = (ushort)~Get16BitRegister(index);
            Set16BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void Inv24BitRegister(byte index)
        {
            uint result = ~Get24BitRegister(index) & 0xFFFFFF;
            Set24BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void Inv32BitRegister(byte index)
        {
            uint result = ~Get32BitRegister(index);
            Set32BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        // IMPLY
        public void Imply8Bit(byte index, byte value)
        {
            byte result = (byte)((~(_gpDataFlat[bankOffset + index] ^ value)) | value);
            _gpDataFlat[bankOffset + index] = result;
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void Imply16Bit(byte index, ushort value)
        {
            ushort result = (ushort)((~(Get16BitRegister(index) ^ value)) | value);
            Set16BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void Imply24Bit(byte index, uint value)
        {
            uint result = (~(Get24BitRegister(index) ^ value)) | value;
            Set24BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void Imply32Bit(byte index, uint value)
        {
            uint result = (~(Get32BitRegister(index) ^ value)) | value;
            Set32BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void Imply8BitMem(uint address, byte value)
        {
            byte ramVal = _computer.MEMC.Get8bitFromRAM(address);
            byte result = (byte)((~(ramVal ^ value)) | value);
            _computer.MEMC.Set8bitToRAM(address, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void Imply16BitMem(uint address, ushort value)
        {
            ushort ramVal = _computer.MEMC.Get16bitFromRAM(address);
            ushort result = (ushort)((~(ramVal ^ value)) | value);
            _computer.MEMC.Set16bitToRAM(address, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void Imply24BitMem(uint address, uint value)
        {
            uint ramVal = _computer.MEMC.Get24bitFromRAM(address);
            uint result = (~(ramVal ^ value)) | value;
            _computer.MEMC.Set24bitToRAM(address, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void Imply32BitMem(uint address, uint value)
        {
            uint ramVal = _computer.MEMC.Get32bitFromRAM(address);
            uint result = (~(ramVal ^ value)) | value;
            _computer.MEMC.Set32bitToRAM(address, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        // new
        public void ImplyMemTo8BitReg(byte index, uint address)
        {
            byte ramVal = _computer.MEMC.Get8bitFromRAM(address);
            byte value = _gpDataFlat[bankOffset + index];
            byte result = (byte)((~(value ^ ramVal)) | ramVal);

            _gpDataFlat[bankOffset + index] = result;
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void ImplyMemTo16BitReg(byte index, uint address)
        {
            ushort ramVal = _computer.MEMC.Get16bitFromRAM(address);
            ushort value = Get16BitRegister(index);
            ushort result = (ushort)((~(value ^ ramVal)) | ramVal);

            Set16BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void ImplyMemTo24BitReg(byte index, uint address)
        {
            uint ramVal = _computer.MEMC.Get24bitFromRAM(address);
            uint value = Get24BitRegister(index);
            uint result = (~(value ^ ramVal)) | ramVal;

            Set24BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }

        public void ImplyMemTo32BitReg(byte index, uint address)
        {
            uint ramVal = _computer.MEMC.Get32bitFromRAM(address);
            uint value = Get32BitRegister(index);
            uint result = (~(value ^ ramVal)) | ramVal;

            Set32BitRegister(index, result);
            _computer.CPU.FLAGS.ClearCarry();
            _computer.CPU.FLAGS.SetZero(result == 0);
        }


        // EX
        public void Ex8BitRegisters(byte index1, byte index2)
        {
            (_gpDataFlat[bankOffset + index2], _gpDataFlat[bankOffset + index1]) = (_gpDataFlat[bankOffset + index1], _gpDataFlat[bankOffset + index2]);
        }

        public void Ex16BitRegisters(byte index1, byte index2)
        {
            ushort regValue = Get16BitRegister(index1);
            Set16BitRegister(index1, Get16BitRegister(index2));
            Set16BitRegister(index2, regValue);
        }

        public void Ex24BitRegisters(byte index1, byte index2)
        {
            uint regValue = Get24BitRegister(index1);
            Set24BitRegister(index1, Get24BitRegister(index2));
            Set24BitRegister(index2, regValue);
        }

        public void Ex32BitRegisters(byte index1, byte index2)
        {
            uint regValue = Get32BitRegister(index1);
            Set32BitRegister(index1, Get32BitRegister(index2));
            Set32BitRegister(index2, regValue);
        }

        // INC
        public void Increment8Bit(byte index)
        {
            _gpDataFlat[bankOffset + index]++;
        }

        public void Increment16Bit(byte index)
        {
            Set16BitRegister(index, (ushort)(Get16BitRegister(index) + 1));
        }

        public void Increment24Bit(byte index)
        {
            Set24BitRegister(index, Get24BitRegister(index) + 1);
        }

        public void Increment32Bit(byte index)
        {
            Set32BitRegister(index, Get32BitRegister(index) + 1);
        }

        public void Increment8BitMem(uint address)
        {
            byte ramVal = _computer.MEMC.Get8bitFromRAM(address);
            ramVal++;
            _computer.MEMC.Set8bitToRAM(address, ramVal);
        }

        public void Increment16BitMem(uint address)
        {
            ushort ramVal = _computer.MEMC.Get16bitFromRAM(address);
            ramVal++;
            _computer.MEMC.Set16bitToRAM(address, ramVal);
        }

        public void Increment24BitMem(uint address)
        {
            uint ramVal = _computer.MEMC.Get24bitFromRAM(address);
            ramVal++;
            _computer.MEMC.Set24bitToRAM(address, ramVal);
        }

        public void Increment32BitMem(uint address)
        {
            uint ramVal = _computer.MEMC.Get32bitFromRAM(address);
            ramVal++;
            _computer.MEMC.Set32bitToRAM(address, ramVal);
        }

        // DEC
        public void Decrement8Bit(byte index)
        {
            _gpDataFlat[bankOffset + index]--;
        }

        public void Decrement16Bit(byte index)
        {
            Set16BitRegister(index, (ushort)(Get16BitRegister(index) - 1));
        }

        public void Decrement24Bit(byte index)
        {
            Set24BitRegister(index, Get24BitRegister(index) - 1);
        }

        public void Decrement32Bit(byte index)
        {
            Set32BitRegister(index, Get32BitRegister(index) - 1);
        }

        public void Decrement8BitMem(uint address)
        {
            byte ramVal = _computer.MEMC.Get8bitFromRAM(address);
            ramVal--;
            _computer.MEMC.Set8bitToRAM(address, ramVal);
        }

        public void Decrement16BitMem(uint address)
        {
            ushort ramVal = _computer.MEMC.Get16bitFromRAM(address);
            ramVal--;
            _computer.MEMC.Set16bitToRAM(address, ramVal);
        }

        public void Decrement24BitMem(uint address)
        {
            uint ramVal = _computer.MEMC.Get24bitFromRAM(address);
            ramVal--;
            _computer.MEMC.Set24bitToRAM(address, ramVal);
        }

        public void Decrement32BitMem(uint address)
        {
            uint ramVal = _computer.MEMC.Get32bitFromRAM(address);
            ramVal--;
            _computer.MEMC.Set32bitToRAM(address, ramVal);
        }

        public byte Get8BitRegister(byte index)
        {
            return _gpDataFlat[bankOffset + index];
        }

        public void Set8BitRegister(byte index, byte value)
        {
            _gpDataFlat[bankOffset + index] = value;
        }

        public sbyte Get8BitRegisterSigned(byte index)
        {
            return (sbyte)_gpDataFlat[bankOffset + index];
        }

        public void Set8BitRegisterSigned(byte index, sbyte value)
        {
            _gpDataFlat[bankOffset + index] = (byte)value;
        }

        public ushort Get16BitRegister(byte index)
        {
            return Compose16Bit(
                index,
                Next1[index]);
        }

        public void Set16BitRegister(byte index, ushort value)
        {
            Decompose16Bit(value,
                index,
                Next1[index]);
        }

        public short Get16BitRegisterSigned(byte index)
        {
            return (short)Compose16Bit(
                index,
                Next1[index]);
        }

        public void Set16BitRegisterSigned(byte index, short value)
        {
            Decompose16BitSigned(value,
                index,
                Next1[index]);
        }

        public uint Get24BitRegister(byte index)
        {
            return Compose24Bit(
                index,
                Next1[index],
                Next2[index]);
        }

        public int Get24BitRegisterSigned(byte index)
        {
            return Compose24BitSigned(
                index,
                Next1[index],
                Next2[index]);
        }

        public void Set24BitRegister(byte index, uint value)
        {
            Decompose24Bit(value,
                index,
                Next1[index],
                Next2[index]);
        }

        public void Set24BitRegisterSigned(byte index, int value)
        {
            Decompose24BitSigned(value,
                index,
                Next1[index],
                Next2[index]);
        }

        public uint Get32BitRegister(byte index)
        {
            return Compose32Bit(
                index,
                Next1[index],
                Next2[index],
                Next3[index]);
        }

        public int Get32BitRegisterSigned(byte index)
        {
            return (int)Compose32Bit(
                index,
                Next1[index],
                Next2[index],
                Next3[index]);
        }

        public void Set32BitRegister(byte index, uint value)
        {
            Decompose32Bit(value,
                index,
                Next1[index],
                Next2[index],
                Next3[index]);
        }

        public void Set32BitRegisterSigned(byte index, int value)
        {
            Decompose32BitSigned(value,
                index,
                Next1[index],
                Next2[index],
                Next3[index]);
        }

        // TODO move this from here
        public byte GetNextRegister(byte regIndex, byte distance)
        {
            return (byte)((regIndex + distance) % 26);
        }

        #region 8bit registers
        public byte A
        {
            get { return _gpDataFlat[bankOffset + Mnem.A]; }
            set => _gpDataFlat[bankOffset + Mnem.A] = value;
        }

        public byte B
        {
            get { return _gpDataFlat[bankOffset + Mnem.B]; }
            set => _gpDataFlat[bankOffset + Mnem.B] = value;
        }

        public byte C
        {
            get { return _gpDataFlat[bankOffset + Mnem.C]; }
            set => _gpDataFlat[bankOffset + Mnem.C] = value;
        }

        public byte D
        {
            get { return _gpDataFlat[bankOffset + Mnem.D]; }
            set => _gpDataFlat[bankOffset + Mnem.D] = value;
        }

        public byte E
        {
            get { return _gpDataFlat[bankOffset + Mnem.E]; }
            set => _gpDataFlat[bankOffset + Mnem.E] = value;
        }

        public byte F
        {
            get { return _gpDataFlat[bankOffset + Mnem.F]; }
            set => _gpDataFlat[bankOffset + Mnem.F] = value;
        }

        public byte G
        {
            get { return _gpDataFlat[bankOffset + Mnem.G]; }
            set => _gpDataFlat[bankOffset + Mnem.G] = value;
        }

        public byte H
        {
            get { return _gpDataFlat[bankOffset + Mnem.H]; }
            set => _gpDataFlat[bankOffset + Mnem.H] = value;
        }

        public byte I
        {
            get { return _gpDataFlat[bankOffset + Mnem.I]; }
            set => _gpDataFlat[bankOffset + Mnem.I] = value;
        }

        public byte J
        {
            get { return _gpDataFlat[bankOffset + Mnem.J]; }
            set => _gpDataFlat[bankOffset + Mnem.J] = value;
        }

        public byte K
        {
            get { return _gpDataFlat[bankOffset + Mnem.K]; }
            set => _gpDataFlat[bankOffset + Mnem.K] = value;
        }

        public byte L
        {
            get { return _gpDataFlat[bankOffset + Mnem.L]; }
            set => _gpDataFlat[bankOffset + Mnem.L] = value;
        }

        public byte M
        {
            get { return _gpDataFlat[bankOffset + Mnem.M]; }
            set => _gpDataFlat[bankOffset + Mnem.M] = value;
        }

        public byte N
        {
            get { return _gpDataFlat[bankOffset + Mnem.N]; }
            set => _gpDataFlat[bankOffset + Mnem.N] = value;
        }

        public byte O
        {
            get { return _gpDataFlat[bankOffset + Mnem.O]; }
            set => _gpDataFlat[bankOffset + Mnem.O] = value;
        }

        public byte P
        {
            get { return _gpDataFlat[bankOffset + Mnem.P]; }
            set => _gpDataFlat[bankOffset + Mnem.P] = value;
        }

        public byte Q
        {
            get { return _gpDataFlat[bankOffset + Mnem.Q]; }
            set => _gpDataFlat[bankOffset + Mnem.Q] = value;
        }

        public byte R
        {
            get { return _gpDataFlat[bankOffset + Mnem.R]; }
            set => _gpDataFlat[bankOffset + Mnem.R] = value;
        }

        public byte S
        {
            get { return _gpDataFlat[bankOffset + Mnem.S]; }
            set => _gpDataFlat[bankOffset + Mnem.S] = value;
        }

        public byte T
        {
            get { return _gpDataFlat[bankOffset + Mnem.T]; }
            set => _gpDataFlat[bankOffset + Mnem.T] = value;
        }

        public byte U
        {
            get { return _gpDataFlat[bankOffset + Mnem.U]; }
            set => _gpDataFlat[bankOffset + Mnem.U] = value;
        }

        public byte V
        {
            get { return _gpDataFlat[bankOffset + Mnem.V]; }
            set => _gpDataFlat[bankOffset + Mnem.V] = value;
        }

        public byte W
        {
            get { return _gpDataFlat[bankOffset + Mnem.W]; }
            set => _gpDataFlat[bankOffset + Mnem.W] = value;
        }

        public byte X
        {
            get { return _gpDataFlat[bankOffset + Mnem.X]; }
            set => _gpDataFlat[bankOffset + Mnem.X] = value;
        }

        public byte Y
        {
            get { return _gpDataFlat[bankOffset + Mnem.Y]; }
            set => _gpDataFlat[bankOffset + Mnem.Y] = value;
        }

        public byte Z
        {
            get { return _gpDataFlat[bankOffset + Mnem.Z]; }
            set => _gpDataFlat[bankOffset + Mnem.Z] = value;
        }
        #endregion

        #region 16bit registers
        public ushort AB
        {
            get { return Compose16Bit(Mnem.A, Mnem.B); }
            set => Decompose16Bit(value, Mnem.A, Mnem.B);
        }

        public ushort BC
        {
            get { return Compose16Bit(Mnem.B, Mnem.C); }
            set => Decompose16Bit(value, Mnem.B, Mnem.C);
        }

        public ushort CD
        {
            get { return Compose16Bit(Mnem.C, Mnem.D); }
            set => Decompose16Bit(value, Mnem.C, Mnem.D);
        }

        public ushort DE
        {
            get { return Compose16Bit(Mnem.D, Mnem.E); }
            set => Decompose16Bit(value, Mnem.D, Mnem.E);
        }

        public ushort EF
        {
            get { return Compose16Bit(Mnem.E, Mnem.F); }
            set => Decompose16Bit(value, Mnem.E, Mnem.F);
        }

        public ushort FG
        {
            get { return Compose16Bit(Mnem.F, Mnem.G); }
            set => Decompose16Bit(value, Mnem.F, Mnem.G);
        }

        public ushort GH
        {
            get { return Compose16Bit(Mnem.G, Mnem.H); }
            set => Decompose16Bit(value, Mnem.G, Mnem.H);
        }

        public ushort HI
        {
            get { return Compose16Bit(Mnem.H, Mnem.I); }
            set => Decompose16Bit(value, Mnem.H, Mnem.I);
        }

        public ushort IJ
        {
            get { return Compose16Bit(Mnem.I, Mnem.J); }
            set => Decompose16Bit(value, Mnem.I, Mnem.J);
        }

        public ushort JK
        {
            get { return Compose16Bit(Mnem.J, Mnem.K); }
            set => Decompose16Bit(value, Mnem.J, Mnem.K);
        }

        public ushort KL
        {
            get { return Compose16Bit(Mnem.K, Mnem.L); }
            set => Decompose16Bit(value, Mnem.K, Mnem.L);
        }

        public ushort LM
        {
            get { return Compose16Bit(Mnem.L, Mnem.M); }
            set => Decompose16Bit(value, Mnem.L, Mnem.M);
        }

        public ushort MN
        {
            get { return Compose16Bit(Mnem.M, Mnem.N); }
            set => Decompose16Bit(value, Mnem.M, Mnem.N);
        }

        public ushort NO
        {
            get { return Compose16Bit(Mnem.N, Mnem.O); }
            set => Decompose16Bit(value, Mnem.N, Mnem.O);
        }

        public ushort OP
        {
            get { return Compose16Bit(Mnem.O, Mnem.P); }
            set => Decompose16Bit(value, Mnem.O, Mnem.P);
        }

        public ushort PQ
        {
            get { return Compose16Bit(Mnem.P, Mnem.Q); }
            set => Decompose16Bit(value, Mnem.P, Mnem.Q);
        }

        public ushort QR
        {
            get { return Compose16Bit(Mnem.Q, Mnem.R); }
            set => Decompose16Bit(value, Mnem.Q, Mnem.R);
        }

        public ushort RS
        {
            get { return Compose16Bit(Mnem.R, Mnem.S); }
            set => Decompose16Bit(value, Mnem.R, Mnem.S);
        }

        public ushort ST
        {
            get { return Compose16Bit(Mnem.S, Mnem.T); }
            set => Decompose16Bit(value, Mnem.S, Mnem.T);
        }

        public ushort TU
        {
            get { return Compose16Bit(Mnem.T, Mnem.U); }
            set => Decompose16Bit(value, Mnem.T, Mnem.U);
        }

        public ushort UV
        {
            get { return Compose16Bit(Mnem.U, Mnem.V); }
            set => Decompose16Bit(value, Mnem.U, Mnem.V);
        }

        public ushort VW
        {
            get { return Compose16Bit(Mnem.V, Mnem.W); }
            set => Decompose16Bit(value, Mnem.V, Mnem.W);
        }

        public ushort WX
        {
            get { return Compose16Bit(Mnem.W, Mnem.X); }
            set => Decompose16Bit(value, Mnem.W, Mnem.X);
        }

        public ushort XY
        {
            get { return Compose16Bit(Mnem.X, Mnem.Y); }
            set => Decompose16Bit(value, Mnem.X, Mnem.Y);
        }

        public ushort YZ
        {
            get { return Compose16Bit(Mnem.Y, Mnem.Z); }
            set => Decompose16Bit(value, Mnem.Y, Mnem.Z);
        }

        public ushort ZA
        {
            get { return Compose16Bit(Mnem.Z, Mnem.A); }
            set => Decompose16Bit(value, Mnem.Z, Mnem.A);
        }
        #endregion

        #region 24bit registers
        public uint ABC
        {
            get { return Compose24Bit(Mnem.A, Mnem.B, Mnem.C); }
            set => Decompose24Bit(value, Mnem.A, Mnem.B, Mnem.C);
        }

        public uint BCD
        {
            get { return Compose24Bit(Mnem.B, Mnem.C, Mnem.D); }
            set => Decompose24Bit(value, Mnem.B, Mnem.C, Mnem.D);
        }

        public uint CDE
        {
            get { return Compose24Bit(Mnem.C, Mnem.D, Mnem.E); }
            set => Decompose24Bit(value, Mnem.C, Mnem.D, Mnem.E);
        }

        public uint DEF
        {
            get { return Compose24Bit(Mnem.D, Mnem.E, Mnem.F); }
            set => Decompose24Bit(value, Mnem.D, Mnem.E, Mnem.F);
        }

        public uint EFG
        {
            get { return Compose24Bit(Mnem.E, Mnem.F, Mnem.G); }
            set => Decompose24Bit(value, Mnem.E, Mnem.F, Mnem.G);
        }

        public uint FGH
        {
            get { return Compose24Bit(Mnem.F, Mnem.G, Mnem.H); }
            set => Decompose24Bit(value, Mnem.F, Mnem.G, Mnem.H);
        }

        public uint GHI
        {
            get { return Compose24Bit(Mnem.G, Mnem.H, Mnem.I); }
            set => Decompose24Bit(value, Mnem.G, Mnem.H, Mnem.I);
        }

        public uint HIJ
        {
            get { return Compose24Bit(Mnem.H, Mnem.I, Mnem.J); }
            set => Decompose24Bit(value, Mnem.H, Mnem.I, Mnem.J);
        }

        public uint IJK
        {
            get { return Compose24Bit(Mnem.I, Mnem.J, Mnem.K); }
            set => Decompose24Bit(value, Mnem.I, Mnem.J, Mnem.K);
        }

        public uint JKL
        {
            get { return Compose24Bit(Mnem.J, Mnem.K, Mnem.L); }
            set => Decompose24Bit(value, Mnem.J, Mnem.K, Mnem.L);
        }

        public uint KLM
        {
            get { return Compose24Bit(Mnem.K, Mnem.L, Mnem.M); }
            set => Decompose24Bit(value, Mnem.K, Mnem.L, Mnem.M);
        }

        public uint LMN
        {
            get { return Compose24Bit(Mnem.L, Mnem.M, Mnem.N); }
            set => Decompose24Bit(value, Mnem.L, Mnem.M, Mnem.N);
        }

        public uint MNO
        {
            get { return Compose24Bit(Mnem.M, Mnem.N, Mnem.O); }
            set => Decompose24Bit(value, Mnem.M, Mnem.N, Mnem.O);
        }

        public uint NOP
        {
            get { return Compose24Bit(Mnem.N, Mnem.O, Mnem.P); }
            set => Decompose24Bit(value, Mnem.N, Mnem.O, Mnem.P);
        }

        public uint OPQ
        {
            get { return Compose24Bit(Mnem.O, Mnem.P, Mnem.Q); }
            set => Decompose24Bit(value, Mnem.O, Mnem.P, Mnem.Q);
        }

        public uint PQR
        {
            get { return Compose24Bit(Mnem.P, Mnem.Q, Mnem.R); }
            set => Decompose24Bit(value, Mnem.P, Mnem.Q, Mnem.R);
        }

        public uint QRS
        {
            get { return Compose24Bit(Mnem.Q, Mnem.R, Mnem.S); }
            set => Decompose24Bit(value, Mnem.Q, Mnem.R, Mnem.S);
        }

        public uint RST
        {
            get { return Compose24Bit(Mnem.R, Mnem.S, Mnem.T); }
            set => Decompose24Bit(value, Mnem.R, Mnem.S, Mnem.T);
        }

        public uint STU
        {
            get { return Compose24Bit(Mnem.S, Mnem.T, Mnem.U); }
            set => Decompose24Bit(value, Mnem.S, Mnem.T, Mnem.U);
        }

        public uint TUV
        {
            get { return Compose24Bit(Mnem.T, Mnem.U, Mnem.V); }
            set => Decompose24Bit(value, Mnem.T, Mnem.U, Mnem.V);
        }

        public uint UVW
        {
            get { return Compose24Bit(Mnem.U, Mnem.V, Mnem.W); }
            set => Decompose24Bit(value, Mnem.U, Mnem.V, Mnem.W);
        }

        public uint VWX
        {
            get { return Compose24Bit(Mnem.V, Mnem.W, Mnem.X); }
            set => Decompose24Bit(value, Mnem.V, Mnem.W, Mnem.X);
        }

        public uint WXY
        {
            get { return Compose24Bit(Mnem.W, Mnem.X, Mnem.Y); }
            set => Decompose24Bit(value, Mnem.W, Mnem.X, Mnem.Y);
        }

        public uint XYZ
        {
            get { return Compose24Bit(Mnem.X, Mnem.Y, Mnem.Z); }
            set => Decompose24Bit(value, Mnem.X, Mnem.Y, Mnem.Z);
        }

        public uint YZA
        {
            get { return Compose24Bit(Mnem.Y, Mnem.Z, Mnem.A); }
            set => Decompose24Bit(value, Mnem.Y, Mnem.Z, Mnem.A);
        }

        public uint ZAB
        {
            get { return Compose24Bit(Mnem.Z, Mnem.A, Mnem.B); }
            set => Decompose24Bit(value, Mnem.Z, Mnem.A, Mnem.B);
        }
        #endregion

        #region 32bit registers
        public uint ABCD
        {
            get { return Compose32Bit(Mnem.A, Mnem.B, Mnem.C, Mnem.D); }
            set => Decompose32Bit(value, Mnem.A, Mnem.B, Mnem.C, Mnem.D);
        }

        public uint BCDE
        {
            get { return Compose32Bit(Mnem.B, Mnem.C, Mnem.D, Mnem.E); }
            set => Decompose32Bit(value, Mnem.B, Mnem.C, Mnem.D, Mnem.E);
        }

        public uint CDEF
        {
            get { return Compose32Bit(Mnem.C, Mnem.D, Mnem.E, Mnem.F); }
            set => Decompose32Bit(value, Mnem.C, Mnem.D, Mnem.E, Mnem.F);
        }

        public uint DEFG
        {
            get { return Compose32Bit(Mnem.D, Mnem.E, Mnem.F, Mnem.G); }
            set => Decompose32Bit(value, Mnem.D, Mnem.E, Mnem.F, Mnem.G);
        }

        public uint EFGH
        {
            get { return Compose32Bit(Mnem.E, Mnem.F, Mnem.G, Mnem.H); }
            set => Decompose32Bit(value, Mnem.E, Mnem.F, Mnem.G, Mnem.H);
        }

        public uint FGHI
        {
            get { return Compose32Bit(Mnem.F, Mnem.G, Mnem.H, Mnem.I); }
            set => Decompose32Bit(value, Mnem.F, Mnem.G, Mnem.H, Mnem.I);
        }

        public uint GHIJ
        {
            get { return Compose32Bit(Mnem.G, Mnem.H, Mnem.I, Mnem.J); }
            set => Decompose32Bit(value, Mnem.G, Mnem.H, Mnem.I, Mnem.J);
        }

        public uint HIJK
        {
            get { return Compose32Bit(Mnem.H, Mnem.I, Mnem.J, Mnem.K); }
            set => Decompose32Bit(value, Mnem.H, Mnem.I, Mnem.J, Mnem.K);
        }

        public uint IJKL
        {
            get { return Compose32Bit(Mnem.I, Mnem.J, Mnem.K, Mnem.L); }
            set => Decompose32Bit(value, Mnem.I, Mnem.J, Mnem.K, Mnem.L);
        }

        public uint JKLM
        {
            get { return Compose32Bit(Mnem.J, Mnem.K, Mnem.L, Mnem.M); }
            set => Decompose32Bit(value, Mnem.J, Mnem.K, Mnem.L, Mnem.M);
        }

        public uint KLMN
        {
            get { return Compose32Bit(Mnem.K, Mnem.L, Mnem.M, Mnem.N); }
            set => Decompose32Bit(value, Mnem.K, Mnem.L, Mnem.M, Mnem.N);
        }

        public uint LMNO
        {
            get { return Compose32Bit(Mnem.L, Mnem.M, Mnem.N, Mnem.O); }
            set => Decompose32Bit(value, Mnem.L, Mnem.M, Mnem.N, Mnem.O);
        }

        public uint MNOP
        {
            get { return Compose32Bit(Mnem.M, Mnem.N, Mnem.O, Mnem.P); }
            set => Decompose32Bit(value, Mnem.M, Mnem.N, Mnem.O, Mnem.P);
        }

        public uint NOPQ
        {
            get { return Compose32Bit(Mnem.N, Mnem.O, Mnem.P, Mnem.Q); }
            set => Decompose32Bit(value, Mnem.N, Mnem.O, Mnem.P, Mnem.Q);
        }

        public uint OPQR
        {
            get { return Compose32Bit(Mnem.O, Mnem.P, Mnem.Q, Mnem.R); }
            set => Decompose32Bit(value, Mnem.O, Mnem.P, Mnem.Q, Mnem.R);
        }

        public uint PQRS
        {
            get { return Compose32Bit(Mnem.P, Mnem.Q, Mnem.R, Mnem.S); }
            set => Decompose32Bit(value, Mnem.P, Mnem.Q, Mnem.R, Mnem.S);
        }

        public uint QRST
        {
            get { return Compose32Bit(Mnem.Q, Mnem.R, Mnem.S, Mnem.T); }
            set => Decompose32Bit(value, Mnem.Q, Mnem.R, Mnem.S, Mnem.T);
        }

        public uint RSTU
        {
            get { return Compose32Bit(Mnem.R, Mnem.S, Mnem.T, Mnem.U); }
            set => Decompose32Bit(value, Mnem.R, Mnem.S, Mnem.T, Mnem.U);
        }

        public uint STUV
        {
            get { return Compose32Bit(Mnem.S, Mnem.T, Mnem.U, Mnem.V); }
            set => Decompose32Bit(value, Mnem.S, Mnem.T, Mnem.U, Mnem.V);
        }

        public uint TUVW
        {
            get { return Compose32Bit(Mnem.T, Mnem.U, Mnem.V, Mnem.W); }
            set => Decompose32Bit(value, Mnem.T, Mnem.U, Mnem.V, Mnem.W);
        }

        public uint UVWX
        {
            get { return Compose32Bit(Mnem.U, Mnem.V, Mnem.W, Mnem.X); }
            set => Decompose32Bit(value, Mnem.U, Mnem.V, Mnem.W, Mnem.X);
        }

        public uint VWXY
        {
            get { return Compose32Bit(Mnem.V, Mnem.W, Mnem.X, Mnem.Y); }
            set => Decompose32Bit(value, Mnem.V, Mnem.W, Mnem.X, Mnem.Y);
        }

        public uint WXYZ
        {
            get { return Compose32Bit(Mnem.W, Mnem.X, Mnem.Y, Mnem.Z); }
            set => Decompose32Bit(value, Mnem.W, Mnem.X, Mnem.Y, Mnem.Z);
        }

        public uint XYZA
        {
            get { return Compose32Bit(Mnem.X, Mnem.Y, Mnem.Z, Mnem.A); }
            set => Decompose32Bit(value, Mnem.X, Mnem.Y, Mnem.Z, Mnem.A);
        }

        public uint YZAB
        {
            get { return Compose32Bit(Mnem.Y, Mnem.Z, Mnem.A, Mnem.B); }
            set => Decompose32Bit(value, Mnem.Y, Mnem.Z, Mnem.A, Mnem.B);
        }

        public uint ZABC
        {
            get { return Compose32Bit(Mnem.Z, Mnem.A, Mnem.B, Mnem.C); }
            set => Decompose32Bit(value, Mnem.Z, Mnem.A, Mnem.B, Mnem.C);
        }
        #endregion

        private ushort Compose16Bit(byte reg1Pointer, byte reg2Pointer)
        {
            return (ushort)((_gpDataFlat[bankOffset + reg1Pointer] << 8) + _gpDataFlat[bankOffset + reg2Pointer]);
        }

        private uint Compose24Bit(byte reg1Pointer, byte reg2Pointer, byte reg3Pointer)
        {
            return (uint)((_gpDataFlat[bankOffset + reg1Pointer] << 16) + (_gpDataFlat[bankOffset + reg2Pointer] << 8) + _gpDataFlat[bankOffset + reg3Pointer]);
        }

        private int Compose24BitSigned(byte reg1Pointer, byte reg2Pointer, byte reg3Pointer)
        {
            int result = _gpDataFlat[bankOffset + reg1Pointer] << 16 | _gpDataFlat[bankOffset + reg2Pointer] << 8 | _gpDataFlat[bankOffset + reg3Pointer];

            if ((result & 0x00800000) != 0)
            {
                result |= unchecked((int)0xFF000000); // Sign extension
            }

            return result;
        }

        private uint Compose32Bit(byte reg1Pointer, byte reg2Pointer, byte reg3Pointer, byte reg4Pointer)
        {
            return (uint)((_gpDataFlat[bankOffset + reg1Pointer] << 24) + (_gpDataFlat[bankOffset + reg2Pointer] << 16) + (_gpDataFlat[bankOffset + reg3Pointer] << 8) + _gpDataFlat[bankOffset + reg4Pointer]);
        }

        private void Decompose16Bit(ushort value, byte reg1Pointer, byte reg2Pointer)
        {
            _gpDataFlat[bankOffset + reg1Pointer] = (byte)(value >> 8);
            _gpDataFlat[bankOffset + reg2Pointer] = (byte)value;
        }

        private void Decompose16BitSigned(short value, byte reg1Pointer, byte reg2Pointer)
        {
            _gpDataFlat[bankOffset + reg1Pointer] = (byte)(value >> 8);
            _gpDataFlat[bankOffset + reg2Pointer] = (byte)value;
        }

        private void Decompose24Bit(uint value, byte reg1Pointer, byte reg2Pointer, byte reg3Pointer)
        {
            _gpDataFlat[bankOffset + reg1Pointer] = (byte)(value >> 16);
            _gpDataFlat[bankOffset + reg2Pointer] = (byte)(value >> 8);
            _gpDataFlat[bankOffset + reg3Pointer] = (byte)value;
        }

        private void Decompose24BitSigned(int value, byte reg1Pointer, byte reg2Pointer, byte reg3Pointer)
        {
            _gpDataFlat[bankOffset + reg1Pointer] = (byte)(value >> 16);
            _gpDataFlat[bankOffset + reg2Pointer] = (byte)(value >> 8);
            _gpDataFlat[bankOffset + reg3Pointer] = (byte)value;
        }

        private void Decompose32Bit(uint value, byte reg1Pointer, byte reg2Pointer, byte reg3Pointer, byte reg4Pointer)
        {
            _gpDataFlat[bankOffset + reg1Pointer] = (byte)(value >> 24);
            _gpDataFlat[bankOffset + reg2Pointer] = (byte)(value >> 16);
            _gpDataFlat[bankOffset + reg3Pointer] = (byte)(value >> 8);
            _gpDataFlat[bankOffset + reg4Pointer] = (byte)value;
        }

        private void Decompose32BitSigned(int value, byte reg1Pointer, byte reg2Pointer, byte reg3Pointer, byte reg4Pointer)
        {
            _gpDataFlat[bankOffset + reg1Pointer] = (byte)(value >> 24);
            _gpDataFlat[bankOffset + reg2Pointer] = (byte)(value >> 16);
            _gpDataFlat[bankOffset + reg3Pointer] = (byte)(value >> 8);
            _gpDataFlat[bankOffset + reg4Pointer] = (byte)value;
        }

        public string GetDebugInfo()
        {
            string response = "IPO: " + _ipo + Environment.NewLine;
            response += "SPR: " + _spr + Environment.NewLine;
            response += "SPC: " + _spc + Environment.NewLine;

            for (byte i = 0; i < 26; i++)
                response += Convert.ToChar(i + 65) + ": " + Get8BitRegister(i) + Environment.NewLine;

            return response;
        }

        public string GetDebugTemplate()
        {
            string response = "IPO: {0}" + Environment.NewLine;
            response += "SPR: {1}" + Environment.NewLine;
            response += "SPC: {2}" + Environment.NewLine;

            for (byte i = 0; i < 26; i++)
                response += Convert.ToChar(i + 65) + ": {" + (i + 3) + "}" + Environment.NewLine;

            return response;
        }
    }
}
