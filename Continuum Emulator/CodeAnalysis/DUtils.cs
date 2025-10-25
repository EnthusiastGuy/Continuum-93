using Continuum93.Emulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Continuum93.CodeAnalysis
{
    public static class DUtils
    {
        public static string ByteToBinary(byte value)
        {
            return Convert.ToString(value, 2).PadLeft(8, '0');
        }

        public static byte GetArgumentsCount(string challenge)
        {
            byte index = 0;
            for (int i = 0; i < challenge.Length; i++)
            {
                if (challenge[i] == 'o' || challenge[i] == 'u' || challenge[i] == '0' || challenge[i] == '1')
                    continue;

                byte foundIndex = (byte)((byte)challenge[i] - (byte)'A' + 1);
                if (foundIndex > index)
                    index = foundIndex;
            }

            return index;
        }

        public static bool IsRegisterForm(string form)
        {
            return form.Contains("r");
        }

        public static bool IsFloatRegisterForm(string form)
        {
            return form.Equals("fr");
        }

        public static bool IsNumberForm(string form)
        {
            return form.Contains("n");
        }

        public static bool IsFlagForm(string form)
        {
            return form.Contains("f");
        }

        public static string GetRegisterRepresentation(string form, byte index)
        {
            string rStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZABC";

            if (index > 25) // TODO fix this once registers are refactored
            {
                return form == "(rrr)" ? $"({index}?)" : new string('?', form.Length);
            }

            if (form == "(rrr)")
                return "(" + rStr.Substring(index, 3) + ")";

            if (form == "(rrr")
                return "(" + rStr.Substring(index, 3);

            if (form == "rrr)")
                return rStr.Substring(index, 3) + ")";

            if (form == "rr)")
                return rStr.Substring(index, 2) + ")";

            if (form == "r)")
                return rStr.Substring(index, 1) + ")";

            return rStr.Substring(index, form.Length);
        }

        public static string GetFloatRegisterRepresentation(byte index)
        {
            return $"F{index}";
        }

        public static string GetNumberRepresentation(string form, DArg arg, bool isRelativeAddress, uint address, bool signedInstruction = false, bool floatInstruction = false)
        {
            if (form == "(nnn)")
                return "(" + arg.GetHexValue24Bit() + ")";

            if (form == "nnn)")
                return arg.GetSignedHexValue24Bit() + ")";

            if (form == "(nnn")
                return "(" + arg.GetHexValue24Bit();

            if (arg.GetByteCount() == 4)
            {
                if (floatInstruction)
                    return arg.GetFloatValue();

                if (signedInstruction)
                    return arg.GetSignedHexValue32Bit();

                return arg.GetHexValue32Bit();
            }
            else if (arg.GetByteCount() == 3)
            {
                if (isRelativeAddress)
                    //return arg.GetHexValue24BitRelativeOffset() + " => " + arg.GetHexValue24BitRelative(address);
                    return arg.GetComposite24BitRelativeValueAndOffset(address);

                if (signedInstruction)
                    return arg.GetSignedHexValue24Bit();


                return arg.GetHexValue24Bit();
            }
            else if (arg.GetByteCount() == 2)
            {
                if (signedInstruction && arg.GetBitCount() == 16)
                    return arg.GetSignedHexValue16Bit();

                if (signedInstruction && arg.GetBitCount() == 13)
                    return arg.GetSignedHexValue13Bit();

                return arg.GetHexValue16Bit();
            }
            else if (arg.GetByteCount() == 1)
            {
                if (signedInstruction)
                    return arg.GetSignedHexValue8Bit();

                return arg.GetHexValue8Bit();
            }

            return "?n?";
        }

        public static string GetFlagRepresentation(byte index)
        {
            return Flags.GetFlagNameByIndex(index);
        }

        public static byte GetByteLengthOfBits(int value)
        {
            return (byte)Math.Ceiling(value / 8.0f);
        }

        public static string GetUntilOrWhole(this string text, string stopAt = ":")
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    return text[..charLocation].Trim();
                }
            }

            return text.Trim();
        }

        public static string[] GetTextParameters(string text)
        {
            if (!text.Contains(':'))
                return new string[] { };

            int charLocation = text.IndexOf(':', StringComparison.Ordinal) + 1;
            string parameters = text[charLocation..];

            return parameters.Split(',')
                               .Select(p => p.Trim())
                               .ToArray();
        }

        public static int[] GetIntParameters(string text)
        {
            string[] parameters = GetTextParameters(text);
            return parameters.Select(int.Parse).ToArray();
        }

    }
}
