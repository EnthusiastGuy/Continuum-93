using Continuum93.ServiceModule.Parsers;
using System;
using System.Globalization;
using System.Text;

namespace Continuum93.ServiceModule.Parsers
{
    public static class RegistryUtils
    {
        private static string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZABC";

        public static string GetNBitRegisterName(int regIndex, int length)
        {
            return ALPHABET.Substring(regIndex, length);
        }

        public static string GetFloatRegisterRepresentation(byte index)
        {
            return $"F{index}";
        }

        public static string GetHexValueForNBitRegister(int regindex, int length, byte[] regs)
        {
            StringBuilder builder = new(length * 2);
            for (int i = 0; i < length; i++)
            {
                builder.Append(regs[(regindex + i) % 26].ToString("X2"));
            }
            return builder.ToString();
        }

        public static string GetDecimalValueForNBitRegister(int regindex, int length, byte[] regs)
        {
            StringBuilder builder = new(length * 3);
            for (int i = 0; i < length; i++)
            {
                builder.Append(regs[(regindex + i) % 26].ToString("X2"));
            }
            string hexValue = builder.ToString();

            int paddingLength = 3 + ((length - 1) * 3);
            uint decimalValue = uint.Parse(hexValue, NumberStyles.HexNumber);
            string paddedDecimalValue = decimalValue.ToString($"D{paddingLength}");

            if (length == 4 && decimalValue < 0)
            {
                throw new OverflowException("The decimal value is too large to fit in an unsigned 32-bit integer.");
            }

            return paddedDecimalValue;
        }

        public static Microsoft.Xna.Framework.Color GetRegisterStateColor(int regindex)
        {
            return (CPUState.IsRegisterChanged(regindex) ? 
                Microsoft.Xna.Framework.Color.OrangeRed : 
                Microsoft.Xna.Framework.Color.White);
        }
    }
}



