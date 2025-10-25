using System;
using System.Collections.Generic;
using System.Globalization;

namespace ContinuumTools.States
{
    public static class Disassembled
    {
        public static int Address = 0xF00000;
        public static int FetchLines = 20;
        public static volatile List<DissLine> Lines = new();

        private static string _response;
        

        public static void SetResponse(string responseData)
        {
            _response = responseData;
            ParseResponse();
        }

        public static DissLine GetCurentInstruction()
        {
            for (int i = 0; i < Lines.Count; i++)
            {
                if (Lines[i].Address == CPUState.IPAddress)
                {
                    return Lines[i];
                }
            }

            return null;
        }

        private static void ParseResponse()
        {
            Lines.Clear();
            string[] lines = _response.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                Lines.Add(new DissLine(line));
            }
        }
    }

    public class DissLine
    {
        public volatile int Address;
        public volatile string TextAddress;
        public volatile string OpCodes;
        public volatile string Instruction;

        public DissLine(string line)
        {
            string[] data = line.Split('|');
            TextAddress = data[0];
            Address = int.Parse(TextAddress, NumberStyles.HexNumber);
            OpCodes = data[1];
            Instruction = data[2];
        }

        public int GetInstructionAddress()
        {
            int address = -1;
            if (Instruction.Contains("=>")) // relative address
            {
                int delimiterIndex = Instruction.IndexOf("=>");
                address = Convert.ToInt32(Instruction.Substring(delimiterIndex + 2).Trim(), 16);
            } else if (Instruction.Contains('(') && Instruction.Contains(')'))  // check if memory addressing
            {
                int start = Instruction.LastIndexOf('(') + 1;
                int end = Instruction.LastIndexOf(')');
                string possibleAddress = Instruction[start..end].Trim();
                if (possibleAddress.Length == 6)
                {
                    address = Convert.ToInt32(possibleAddress, 16);
                }
            } else if (Instruction.Contains("CALL") || Instruction.Contains("JP"))    // Check if it's an absolute jump or call
            {
                string instr = Instruction.Trim();
                string[] result = instr.Split(new string[] { " ", ","}, StringSplitOptions.RemoveEmptyEntries);
                address = Convert.ToInt32(result[result.Length - 1].Trim(), 16);
            } else if (Instruction.Contains("LD"))
            {
                string instr = Instruction.Trim();
                string[] result = instr.Split(',' , StringSplitOptions.RemoveEmptyEntries);
                string possibleAddress = result[result.Length - 1].Trim();
                if (possibleAddress.Length == 8)    // Assumes a 0x prefix
                {
                    address = Convert.ToInt32(possibleAddress[2..], 16);
                }
            }
            return address;
        }
    }
}
