using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;
using System;
using System.Collections.Generic;

namespace Continuum93.CodeAnalysis
{
    public static class ContinuumDebugger
    {
        public static List<DInstruction> Instructions = [];
        public static int DebugInstructionsCount = 200;
        private static Computer _computer;

        public static void RunAt(uint address, Computer computer, bool simpleRepresentation = false)
        {
            _computer = computer;
            Instructions.Clear();

            uint IP = address;
            int length = DebugInstructionsCount;

            while (length > 0)
            {
                try
                {
                    DInstruction instr = GetNextInstruction(IP, simpleRepresentation);
                    IP += instr.Illegal ? (byte)1 : instr.InstructionLength;
                    Instructions.Add(instr);

                    length--;
                }
                catch (Exception e)
                {
                    Log.WriteLine($"Debugger RunAt('{address}') failed: {e.Message}");
                    length = 0;
                }

            }
        }

        public static string GetDissassembled()
        {
            string response = "";

            for (int i = 0; i < Instructions.Count; i++)
                response += Instructions[i].SpecificInstruction + Environment.NewLine;

            return response;
        }

        public static string GetDissassembledWithAddresses()
        {
            string response = "";

            for (int i = 0; i < Instructions.Count; i++)
                response += Instructions[i].Address + "\t\t" + Instructions[i].SpecificInstruction + Environment.NewLine;

            return response;
        }

        public static string GetDissassembledFull()
        {
            string response = "";

            for (int i = 0; i < Instructions.Count; i++)
            {
                string bytes = "";

                foreach (byte b in Instructions[i].Bytes)
                {
                    bytes += DFormat.GetFormattedByte(b, 2, FormatType.SimpleHex) + " ";
                }

                response += DFormat.GetFormattedValue(Instructions[i].Address, 6, FormatType.SimpleHex) + "|"
                    + bytes.Trim() + "|" + Instructions[i].SpecificInstruction + Environment.NewLine;
            }


            return response;
        }

        private static DInstruction GetNextInstruction(uint address, bool simpleRepresentation)
        {
            byte firstByte = FetchByteAt(address);
            byte nextByte = FetchByteAt(address + 1);

            Oper primaryOper = Mnem.GetPrimaryOperator(firstByte);

            Oper secondaryOper = Mnem.GetSecondaryOperator(firstByte, nextByte);

            Oper oper = Mnem.IsSingleByteInstruction(firstByte) ? primaryOper : secondaryOper;

            bool relativeAddressing = primaryOper != null && (primaryOper.Mnemonic == "CALLR" || primaryOper.Mnemonic == "JR");

            DInstruction dInstruction = new(
                oper != null ? oper.Format : "",
                oper != null ? oper.Mnemonic : "ILLEGAL",
                address,
                relativeAddressing,
                simpleRepresentation
            )
            { Illegal = oper == null };

            for (uint i = 0; i < dInstruction.InstructionLength; i++)
                dInstruction.Bytes.Add(FetchByteAt(address + i));

            dInstruction.CalculateInstructionVariables();
            dInstruction.ComposeInstruction();

            return dInstruction;
        }

        private static byte FetchByteAt(uint address)
        {
            return _computer.MEMC.Get8bitFromRAM(address);
        }
    }
}
