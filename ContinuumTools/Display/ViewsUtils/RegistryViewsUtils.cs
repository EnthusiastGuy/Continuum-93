using Continuum93.CodeAnalysis;
using ContinuumTools.States;
using ContinuumTools.Utils;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContinuumTools.Display.ViewsUtils
{
    public static class RegistryViewsUtils
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
                builder.Append(StringUtils.ByteToHex(regs[(regindex + i) % 26]));
            }
            return builder.ToString();
        }

        public static string GetDecimalValueForNBitRegister(int regindex, int length, byte[] regs)
        {
            StringBuilder builder = new(length * 3);
            for (int i = 0; i < length; i++)
            {
                builder.Append(StringUtils.ByteToHex(regs[(regindex + i) % 26]));
            }
            string hexValue = builder.ToString();

            int paddingLength = 3 + ((length - 1) * 3); // calculate the padding length
            uint decimalValue = uint.Parse(hexValue, NumberStyles.HexNumber); // parse hexValue as a uint
            string paddedDecimalValue = decimalValue.ToString($"D{paddingLength}"); // pad the decimalValue with leading zeroes

            if (length == 4 && decimalValue < 0) // check if the decimalValue is negative
            {
                throw new OverflowException("The decimal value is too large to fit in an unsigned 32-bit integer.");
            }

            return paddedDecimalValue;
        }

        public static Color GetRegisterStateColor(int regindex)
        {
            return (CPUState.IsRegisterChanged(regindex) ? Color.OrangeRed : Color.White);
        }
    }
}
