using Continuum93.Tools;
using System;

namespace Continuum93.CodeAnalysis
{
    public class DArg
    {
        public byte Index;
        public bool SimpleRepresentation;
        private string _binary;
        private uint _value;
        private static string MINUS = "-";
        private static string PLUS = "+";

        public DArg()
        {

        }

        // For tests
        public DArg(string binary)
        {
            _binary = binary;
            _value = Convert.ToUInt32(_binary, 2);
        }

        public void PushBit(char bit)
        {
            _binary += bit;
            _value = Convert.ToUInt32(_binary, 2);
        }

        public uint GetValue32Bit()
        {
            return _value;
        }

        public string GetHexValue32Bit()
        {
            return DecimalToHex(_value, 8);
        }

        public uint GetValue24Bit()
        {
            return _value & 0xFFFFFF;
        }

        public string GetHexValue24Bit()
        {
            return DecimalToHex(_value & 0xFFFFFF, 6);
        }

        /*public string GetSignedHexValue24Bit()
        {
            // grab only the low 24 bits
            uint raw = _value & 0xFFFFFF;

            // if the 24th bit is set, treat as negative (2's-complement)
            int signed = (raw & 0x800000) != 0
                ? (int)(raw | 0xFF000000)   // sign-extend: fill high 8 bits with 1s
                : (int)raw;                 // positive as-is

            // compute absolute magnitude in the 24-bit range
            uint magnitude = signed < 0
                ? (uint)(-signed)
                : (uint)signed;

            // format with 6 hex digits
            string hex = DecimalToHex(magnitude, 6);

            // prepend minus sign if negative
            return signed < 0 ? "-" + hex : hex;
        }*/

        public string GetComposite24BitRelativeValueAndOffset(uint address)
        {
            return SimpleRepresentation ?
                GetHexValue24BitRelative(address) :
                GetHexValue24BitRelativeOffset() + " => " + GetHexValue24BitRelative(address);
        }

        public string GetHexValue24BitRelativeOffset()
        {
            uint positiveVal = 0x7FFFFF & _value;
            uint negativeVal = 0x800000 - positiveVal;
            bool negative = _value >= 0x800000;
            if (negative)
                return $"{(SimpleRepresentation ? string.Empty : MINUS)}{negativeVal}";

            return $"{(SimpleRepresentation ? string.Empty : PLUS)}{positiveVal}";
        }

        public string GetHexValue24BitRelative(uint address)
        {
            uint positiveVal = 0x7FFFFF & _value;
            uint negativeVal = 0x800000 - positiveVal;
            bool negative = _value >= 0x800000;
            uint rAddr = (uint)(address - (negative ? negativeVal : -positiveVal));
            return DecimalToHex(rAddr & 0xFFFFFF, 6);
        }

        public ushort GetValue16Bit()
        {
            return (ushort)_value;
        }

        public string GetHexValue16Bit()
        {
            return DecimalToHex(_value & 0xFFFF, 4);
        }

        public byte GetValue8Bit()
        {
            return (byte)_value;
        }

        public string GetHexValue8Bit()
        {
            return DecimalToHex(_value & 0xFF, 2);
        }

        public string GetSignedHexValue8Bit()
        {
            sbyte value = (sbyte)(_value & 0xFF);
            if (value < 0)
            {
                string hexString = Math.Abs((int)value).ToString("X2");
                return $"{(SimpleRepresentation ? string.Empty : MINUS)}0x{hexString}";
            }
            else
            {
                string hexString = value.ToString("X2");
                return $"{(SimpleRepresentation ? string.Empty : PLUS)}0x{hexString}";
            }
        }

        public string GetSignedHexValue13Bit()
        {
            int value = (int)(_value & 0x1FFF);
            if ((value & 0x1000) != 0)
            {
                int absValue = (value ^ 0x1FFF) + 1; // Two's complement
                string hexString = absValue.ToString("X4");
                return $"{(SimpleRepresentation ? string.Empty : MINUS)}0x{hexString}";
            }
            else
            {
                string hexString = value.ToString("X4");
                return $"{(SimpleRepresentation ? string.Empty : PLUS)}0x{hexString}";
            }
        }

        public string GetSignedHexValue16Bit()
        {
            short value = (short)(_value & 0xFFFF);
            if (value < 0)
            {
                string hexString = Math.Abs((int)value).ToString("X4");
                return $"{(SimpleRepresentation ? string.Empty : MINUS)}0x{hexString}";
            }
            else
            {
                string hexString = value.ToString("X4");
                return $"{(SimpleRepresentation ? string.Empty : PLUS)}0x{hexString}";
            }
        }

        public string GetSignedHexValue24Bit()
        {
            int value = (int)(_value & 0xFFFFFF);
            if ((value & 0x800000) != 0)
            {
                int absValue = (value ^ 0xFFFFFF) + 1; // Two's complement
                string hexString = absValue.ToString("X6");
                return $"{(SimpleRepresentation ? string.Empty : MINUS)}0x{hexString}";
            }
            else
            {
                string hexString = value.ToString("X6");
                return $"{(SimpleRepresentation ? string.Empty : PLUS)}0x{hexString}";
            }
        }

        public string GetSignedHexValue32Bit()
        {
            long value = (int)_value;
            if (value < 0)
            {
                string hexString = Math.Abs(value).ToString("X8");
                return $"{(SimpleRepresentation ? string.Empty : MINUS)}0x{hexString}";
            }
            else
            {
                string hexString = value.ToString("X8");
                return $"{(SimpleRepresentation ? string.Empty : PLUS)}0x{hexString}";
            }
        }

        public string GetFloatValue()
        {
            return FloatPointUtils.UintToFloat(_value).ToString();
        }

        public int GetBitCount()
        {
            return _binary.Length;
        }

        public byte GetByteCount()
        {
            return DUtils.GetByteLengthOfBits(_binary.Length);
        }

        private static string DecimalToHex(uint value, int paddingLen)
        {
            string hexString = value.ToString("X").PadLeft(paddingLen, '0');
            return $"0x{hexString}";
        }

        private static string DecimalToSignedHex(uint value, int paddingLen)
        {
            if ((value & 0x8000) != 0)  // Check if the most significant bit is set (indicating a negative number)
            {
                // Convert to signed 16-bit two's complement representation
                int signedValue = (int)(value | 0xFFFF0000);
                string hexString = signedValue.ToString("X").PadLeft(paddingLen, '0');
                return $"-0x{hexString}";
            }
            else
            {
                string hexString = value.ToString("X").PadLeft(paddingLen, '0');
                return $"+0x{hexString}";
            }
        }
    }
}
