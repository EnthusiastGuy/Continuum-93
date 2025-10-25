using System.Collections.Generic;
using System.Globalization;

namespace Continuum93.Emulator.Interpreter
{
    public static class InterpretArgument
    {
        private static List<string> _validRegs = PopulateValidRegisters();

        private static List<string> PopulateValidRegisters()
        {
            string rStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZABC";
            List<string> regs = new List<string>();

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 26; j++)
                    regs.Add(rStr.Substring(j, i + 1));

            return regs;
        }

        public static bool HasLabel(string argument)
        {
            return argument.IndexOfAny(Constants.LABEL_MARKERS) != -1;
        }

        public static bool IsNotFloatNumber(string argument)
        {
            return !float.TryParse(argument, NumberStyles.Float, CultureInfo.InvariantCulture, out _);
        }

        public static bool HasRelativeLabel(string argument)
        {
            return argument.Contains(Constants.LBL_RELATIVE);
        }

        public static bool IsValidFlag(string argument)
        {
            return Flags.GetFlagIndexByName(argument) != 255;
        }

        public static bool IsValidRegister(string argument)
        {
            return _validRegs.Contains(argument);
        }

        public static bool IsFlagUsingOperator(string oper)
        {
            return oper.Equals("CALL") || oper.Equals("CALLR") ||
                oper.Equals("JP") || oper.Equals("JR") ||
                oper.Equals("SETF") || oper.Equals("RESF") || oper.Equals("INVF") ||
                oper.Equals("RETIF");
        }

        // Check whether the argument contains both round parenthesis indicating
        // the argument is an addressing argument
        public static bool IsAdddressingArgument(string argument)
        {
            return argument.StartsWith('(') && argument.EndsWith(')');
        }

        public static bool IsBeginningOfAddressingArgument(string argument)
        {
            return argument.StartsWith('(') && !argument.EndsWith(')');
        }

        public static bool IsEndOfAddressingArgument(string argument)
        {
            return (argument.StartsWith('+') || argument.StartsWith('-')) && argument.EndsWith(')');
        }

        public static string ExtractAddressingArgument(string argument)
        {
            return string.Join("", string.Join("", argument.Split(")")).Split("(")).Trim();
        }
    }
}
