using System.Collections.Generic;
using System.Linq;

namespace Continuum93.Emulator
{
    public class Flags
    {
        private const byte NO_FLAG = 0xFF;
        private readonly byte[] flagsLookupTable = { 1, 2, 4, 8, 16, 32, 64, 128 };
        private readonly bool[] _flags;
        private static readonly Dictionary<byte, string> _flagNames = [];

        static Flags()
        {
            _flagNames.Add(0, "NZ");
            _flagNames.Add(1, "NC");
            _flagNames.Add(2, "SP");
            _flagNames.Add(3, "NO");
            _flagNames.Add(4, "PE");
            _flagNames.Add(5, "NE");
            _flagNames.Add(6, "LTE");
            _flagNames.Add(7, "GTE");
            _flagNames.Add(8, "Z");
            _flagNames.Add(9, "C");
            _flagNames.Add(10, "SN");
            _flagNames.Add(11, "OV");
            _flagNames.Add(12, "PO");
            _flagNames.Add(13, "EQ");
            _flagNames.Add(14, "GT");
            _flagNames.Add(15, "LT");
        }

        public Flags()
        {
            /*
            // See conditions that set them:
            http://www.gabrielececchetti.it/Teaching/CalcolatoriElettronici/Docs/i8086_instruction_set.pdf

                                    0                       1               Designation
            ------------------------------------------------------------------------------
            0       ZERO            not zero                zero            NZ  Z
            1       CARRY           not carry               carry           NC  C
            2       SIGN            positive                negative        SP  SN
            3       OVERFLOW        no overflow             overflow        NO  OV
            4       PARITY          even                    odd             PE  PO
            5       EQUALS          not equal               equal           NE  EQ
            6       GREATER THAN    less than or equal      greater than    LTE GT
            7       LESS THAN       greater than or equal   less than       GTE LT

             */

            _flags = new bool[8];
        }

        public byte GetFlagsByte()
        {
            byte result = 0;

            for (int i = 0; i < 8; i++)
            {
                if (_flags[i])
                {
                    result |= flagsLookupTable[i];
                }
            }

            return result;
        }

        public void ResetAll()
        {
            for (int i = 0; i < _flags.Length; i++)
                _flags[i] = false;
        }

        public void SetAll()
        {
            for (int i = 0; i < _flags.Length; i++)
                _flags[i] = true;
        }

        public bool IsZero() => _flags[0];

        public bool IsCarry() => _flags[1];

        public bool IsNegative() => _flags[2];

        public bool IsOverflow() => _flags[3];

        public static string GetFlagNameByIndex(byte index) => _flagNames.ContainsKey(index) ? _flagNames[index] : "";

        public static byte GetFlagIndexByName(string name) => _flagNames.ContainsValue(name) ? _flagNames.FirstOrDefault(x => x.Value == name).Key : NO_FLAG;

        public void SetValueByIndexFast(byte index, bool value) => _flags[index] = value;

        public void SetValueByIndex(byte index, bool value) => _flags[(byte)(index % 8)] = index >= 8 ? value : !value;

        public void InvertValueByIndex(byte index) => _flags[(byte)(index % 8)] = !_flags[(byte)(index % 8)];

        public void SetValueByName(string name, bool value) => SetValueByIndex(GetFlagIndexByName(name), value);

        public void SetCarry(bool value) => _flags[1] = value;

        public void ClearCarry() => _flags[1] = false;

        public void SetSignNegative(bool value) => _flags[2] = value;

        public void SetOverflow(bool value) => _flags[3] = value;

        public void ClearOverflow() => _flags[3] = false;

        public void SetZero(bool value) => _flags[0] = value;

        public bool GetValueByIndex(byte index) => index >= 8 ? _flags[(byte)(index % 8)] : !_flags[(byte)(index % 8)];

        public bool GetValueByName(string name) => GetValueByIndex(GetFlagIndexByName(name));

        public void Update8BitAddFlags(byte v1, byte v2)
        {
            var sum = v1 + v2;
            byte bSum = (byte)(v1 + v2);
            SetValueByIndexFast(0, bSum == 0);    // Z flag
            SetValueByIndexFast(1, sum > 0xFF);    // C flag
            SetValueByIndexFast(2, (bSum & 0x80) > 0);    // SN flag
        }

        public void Update16BitAddFlags(ushort v1, ushort v2)
        {
            var sum = v1 + v2;
            ushort uSum = (ushort)(v1 + v2);
            SetValueByIndexFast(0, uSum == 0);    // Z flag
            SetValueByIndexFast(1, sum > 0xFFFF);    // C flag
            SetValueByIndexFast(2, (uSum & 0x8000) > 0);    // SN flag
        }

        public void Update24BitAddFlags(uint v1, uint v2)
        {
            var sum = v1 + v2;
            uint cSum = sum & 0xFFFFFF;
            SetValueByIndexFast(0, cSum == 0);    // Z flag
            SetValueByIndexFast(1, sum > 0xFFFFFF);    // C flag
            SetValueByIndexFast(2, (cSum & 0x800000) > 0);    // SN flag
        }

        public void Update32BitAddFlags(uint v1, uint v2)
        {
            var sum = v1 + v2;
            SetValueByIndexFast(0, sum == 0);    // Z flag
            SetValueByIndexFast(1, sum < v1 || sum < v2);    // C flag
            SetValueByIndexFast(2, (sum & 0x80000000) > 0);    // SN flag
        }

        public void Update8BitSubFlags(byte v1, byte v2)
        {
            var diff = v1 - v2;
            SetValueByIndexFast(0, diff == 0);    // Z flag
            SetValueByIndexFast(1, v2 > v1);    // C flag
            SetValueByIndexFast(2, (diff & 0x80) > 0);    // SN flag
        }

        public void Update16BitSubFlags(ushort v1, ushort v2)
        {
            var diff = v1 - v2;
            SetValueByIndexFast(0, diff == 0);    // Z flag
            SetValueByIndexFast(1, v2 > v1);    // C flag
            SetValueByIndexFast(2, (diff & 0x8000) > 0);    // SN flag
        }

        public void Update24BitSubFlags(uint v1, uint v2)
        {
            var diff = v1 - v2;
            SetValueByIndexFast(0, diff == 0);    // Z flag
            SetValueByIndexFast(1, v2 > v1);    // C flag
            SetValueByIndexFast(2, (diff & 0x800000) > 0);    // SN flag
        }

        public void Update32BitSubFlags(uint v1, uint v2)
        {
            var diff = v1 - v2;
            SetValueByIndexFast(0, diff == 0);    // Z flag
            SetValueByIndexFast(1, v2 > v1);    // C flag
            SetValueByIndexFast(2, (diff & 0x80000000) > 0);    // SN flag
        }

        // Float ADD/SUB
        public void UpdateFloatAddFlags(float v1, float v2)
        {
            float sum = v1 + v2;
            SetValueByIndexFast(0, sum == 0);    // Z flag
            SetValueByIndexFast(2, sum < 0);                  // SN flag
            SetValueByIndexFast(3, sum > float.MaxValue);      // Overflow
        }

        public void UpdateFloatSubFlags(float v1, float v2)
        {
            float sum = v1 - v2;
            SetValueByIndexFast(0, sum == 0);    // Z flag
            SetValueByIndexFast(2, sum < 0);                  // SN flag
            SetValueByIndexFast(3, sum < float.MinValue);      // Overflow
        }
        // END Float ADD/SUB

        public void RightCarryBit(uint value) => SetValueByIndexFast(1, (value & 1) > 0);    // C flag

        public void LeftCarry8Bit(byte value) => SetValueByIndexFast(1, value > 0x7F);    // C flag

        public void LeftCarry16Bit(ushort value) => SetValueByIndexFast(1, value > 0x7FFF);    // C flag

        public void LeftCarry24Bit(uint value) => SetValueByIndexFast(1, value > 0x7FFFFF);    // C flag

        public void LeftCarry32Bit(uint value) => SetValueByIndexFast(1, value > 0x7FFFFFFF);    // C flag
    }
}

/*
Some documentation


(from: http://teaching.idallen.com/dat2343/11w/notes/040_overflow.txt)

Carry Flag
----------

The rules for turning on the carry flag in binary/integer math are two:

1. The carry flag is set if the addition of two numbers causes a carry
   out of the most significant (leftmost) bits added.

   1111 + 0001 = 0000 (carry flag is turned on)

2. The carry (borrow) flag is also set if the subtraction of two numbers
   requires a borrow into the most significant (leftmost) bits subtracted.

   0000 - 0001 = 1111 (carry flag is turned on)

Otherwise, the carry flag is turned off (zero).
 * 0111 + 0001 = 1000 (carry flag is turned off [zero])
 * 1000 - 0001 = 0111 (carry flag is turned off [zero])

In unsigned arithmetic, watch the carry flag to detect errors.
In signed arithmetic, the carry flag tells you nothing interesting.

Overflow Flag
-------------

The rules for turning on the overflow flag in binary/integer math are two:

1. If the sum of two numbers with the sign bits off yields a result number
   with the sign bit on, the "overflow" flag is turned on.

   0100 + 0100 = 1000 (overflow flag is turned on)

2. If the sum of two numbers with the sign bits on yields a result number
   with the sign bit off, the "overflow" flag is turned on.

   1000 + 1000 = 0000 (overflow flag is turned on)

Otherwise, the overflow flag is turned off.
 * 0100 + 0001 = 0101 (overflow flag is turned off)
 * 0110 + 1001 = 1111 (overflow flag is turned off)
 * 1000 + 0001 = 1001 (overflow flag is turned off)
 * 1100 + 1100 = 1000 (overflow flag is turned off)



 */
