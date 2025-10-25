using System;

namespace Continuum93.CodeAnalysis
{
    public static class DFormat
    {
        public static string GetFormattedByte(byte value, byte padLength, FormatType type = FormatType.Decimal)
        {
            return GetFormattedValue(value, padLength, type);
        }


        public static string GetFormattedValue(uint value, byte padLength, FormatType type = FormatType.Decimal)
        {
            string response = string.Empty;
            switch (type)
            {
                case FormatType.Decimal:
                    response = value.ToString("D" + padLength);
                    break;
                case FormatType.Hex:
                    response = "0x" + value.ToString("X" + padLength);
                    break;
                case FormatType.SimpleHex:
                    response = value.ToString("X" + padLength);
                    break;
                case FormatType.Binary:
                    response = "0b" + Convert.ToString(value, 2).PadLeft(padLength, '0');
                    break;
                case FormatType.Octal:
                    response = "0o" + Convert.ToString(value, 8).PadLeft(padLength, '0');
                    break;
            }

            return response;
        }
    }

    public enum FormatType
    {
        Decimal,
        Binary,
        Hex,
        SimpleHex,
        Octal
    }
}
