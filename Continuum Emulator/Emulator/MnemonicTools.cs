using Continuum93.Emulator.Mnemonics;
using System;

namespace Continuum93.Emulator
{
    public class MnemonicTools
    {
        public static string ExportSubOps()
        {
            string response = string.Empty;

            foreach (var oper in Mnem.OPS)
            {
                string key = oper.Key;
                Oper value = oper.Value;
                response += $"{key}{Environment.NewLine}";
            }

            return response;
        }
    }
}
