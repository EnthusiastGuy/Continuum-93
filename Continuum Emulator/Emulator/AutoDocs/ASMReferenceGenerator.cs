using Continuum93.Emulator.AutoDocs.MetaInfo;
using Continuum93.Emulator.Mnemonics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Continuum93.Emulator.AutoDocs
{
    public static class ASMReferenceGenerator
    {

        private static string PAGE_BREAK = "<p style=\"page-break-after: always;\">&nbsp;</p>\r\n<p style=\"page-break-before: always;\">&nbsp;</p>";

        public static string GenerateInstructionPages()
        {
            string response = HTML.H1("Instructions specifications");

            foreach (KeyValuePair<string, Oper> primeOp in Mnem.OPS)
            {
                Oper oper = primeOp.Value;

                if (oper.IsPrimary)
                {
                    bool foundSubops = false;
                    string subOps = "";
                    // Search for suboperators that match this operator
                    foreach (KeyValuePair<string, Oper> op in Mnem.OPS)
                    {
                        string opMatch = op.Key.Split(" ")[0].Trim();
                        if (!op.Value.IsPrimary && primeOp.Key.Equals(opMatch))
                        {
                            subOps += GetMultiOpInfo(op.Value, oper, op.Key) + Constants.CR;
                            foundSubops = true;
                        }
                    }

                    if (!foundSubops)
                    {
                        response += GetMainOpInfo(oper, primeOp.Key) + Constants.CR;
                    }
                    else
                    {
                        //response += HTML.H2Title(primeOp.Key) + HTML.PSubtitle(oper.Title + " - " + ASMMetaInfo.GetDefinition(primeOp.Value.Mnemonic)) + HTML.BR + subOps;
                        response += HTML.H2Title(primeOp.Key) + HTML.PSubtitle($"<strong>{oper.Title}</strong>. {ASMMetaInfo.GetDefinition(primeOp.Value.Mnemonic)}" + HTML.BR) + HTML.BR + subOps;
                    }
                    foundSubops = false;
                }

            }

            return HTML.HTMLRoot("Assembly instructions reference", response, "styles.css");
        }

        private static string GetMainOpInfo(Oper oper, string key)
        {
            string response = Constants.CR + Constants.CR;
            response += HTML.H2Title(key) + HTML.PSubtitle($"<strong>{oper.Title}</strong>. {ASMMetaInfo.GetDefinition(oper.Mnemonic)}" + HTML.BR);

            string format = DataConverter.GetBinaryOfUint(oper.OpCode, 8);

            string description = oper.Description.Length > 0 ? oper.Description[0] : "";

            response += HTML.Table(
                HTML.MetaStyledRow("Description:", description) +
                HTML.MetaStyledRow("Instruction syntax:", key) +
                HTML.MetaStyledRow("Instruction size:", DocUtils.GetReadableBytesSize(1)) +
                HTML.MetaStyledRow("Instruction format:", format)
            );

            string bitsLabels = "";
            for (int i = 0; i < format.Length; i++)
            {
                bitsLabels += HTML.TD(format[i] + "");
            }

            string bitsTable = HTML.Table(
                HTML.TR("<th colspan=\"8\">byte 1</th>") +
                HTML.TR("<td colspan=\"8\">" + oper.OpCode.ToString("X") + "</td>") +
                HTML.TR(bitsLabels, "bits") +
                HTML.TR("<td colspan=\"8\">" + key + "</td>", "emphasizeBottom")
            );

            response += "<br />" +
                HTML.P("<strong>Instruction breakdown</strong>") +
                bitsTable;
            response += PAGE_BREAK;

            return response;
        }

        public static string GetMultiOpInfo(Oper oper, Oper parent, string key)
        {
            string response = "";

            byte SubOp = Mnem.GetSecondaryOpCode(key).Value;
            string format = oper.Format;

            byte subOpBitWidth = (byte)DataConverter.CountCharInString('o', format);
            string formatNoSpaces = string.Join("", format.Split(" "));

            format = string.Join(
                DataConverter.GetBinaryOfUint(SubOp, subOpBitWidth),
                format.Split(new string('o', subOpBitWidth))
            );

            string compactFormatBits = string.Join("", format.Split(" "));

            int instructionSize = 1 + (compactFormatBits.Length / 8);

            string correctedFormat = GetCorrectedFormat(key, format);

            string mainOp = correctedFormat.Split(" ")[0];
            string operands = correctedFormat.Split(" ")[1];

            formatNoSpaces = DataConverter.GetBinaryOfUint(Mnem.GetPrimaryOpCode(mainOp), 8) + formatNoSpaces;
            compactFormatBits = DataConverter.GetBinaryOfUint(Mnem.GetPrimaryOpCode(mainOp), 8) + compactFormatBits;

            string[] operandList = operands.Split(",");

            for (byte i = 0; i < operandList.Length; i++)
            {
                operandList[i] = GetOperandDescription(operandList[i], i);
            }

            List<Tuple<char, byte>> groups = GetFormatGroups(formatNoSpaces);

            string cleanFormat = GetCleanFormatCompactBits(formatNoSpaces);

            response += Constants.CR + Constants.CR;

            response += HTML.H3(correctedFormat);

            string description = "";

            for (byte i = 0; i < parent.Description.Length; i++)
            {
                if (DocUtils.CountFormatArguments(parent.Description[i]) == operandList.Length)
                {
                    description = string.Format(parent.Description[i], operandList);
                    break;
                }
            }

            response += HTML.Table(
                HTML.MetaRow("Description:", description) +
                HTML.MetaRow("Instruction syntax:", correctedFormat) +
                HTML.MetaRow("Instruction size:", DocUtils.GetReadableBytesSize(instructionSize)) +
                HTML.MetaRow("Instruction format:", DataConverter.GetBinaryOfUint(Mnem.GetPrimaryOpCode(mainOp), 8) + " " + format)
            );

            string bytesLabels = "";
            string bytesTopGropus = "";
            string[] topGroups = GetBytesInterpretation(compactFormatBits);

            for (int i = 0; i < instructionSize; i++)
            {
                bytesLabels += $"<th colspan=\"8\">byte {i + 1}</th>";
                bytesTopGropus += $"<td colspan=\"8\">{topGroups[i]}</td>";
            }

            string bitsLabels = "";
            for (int i = 0; i < compactFormatBits.Length; i++)
            {
                bitsLabels += HTML.TD(compactFormatBits[i] + "", compactFormatBits[i].Equals('u') ? "grayBit" : "");
            }

            string bitGroups = "";

            for (int i = 0; i < groups.Count; i++)
            {
                string label;

                if (i == 0)
                {
                    label = mainOp;
                }
                else if (groups[i].Item1.Equals('o'))
                {
                    label = operands;
                }
                else if (groups[i].Item1.Equals('u'))
                {
                    label = "<i>unused</i>";
                }
                else
                {
                    label = groups[i].Item1.ToString();
                }

                bitGroups += string.Format("<td colspan=\"{0}\">{1}</td>", groups[i].Item2, label);
            }

            string bitsTable = HTML.Table(
                HTML.TR(bytesLabels) +
                HTML.TR(bytesTopGropus) +
                HTML.TR(bitsLabels, "bits") +
                HTML.TR(bitGroups, "emphasizeBottom")
            );

            response += "<br />" +
                HTML.P("<strong>Instruction breakdown</strong>") +
                bitsTable;

            response += Constants.CR;

            response += PAGE_BREAK;

            return response;
        }

        private static string[] GetBytesInterpretation(string input)
        {
            input = input.Replace('u', '0');                // Replace 'u' with '0'
            List<string> result = new();                    // Prepare the list to hold the results

            for (int i = 0; i < input.Length; i += 8)       // Process each group of 8 bits
            {
                if (i + 8 <= input.Length)
                {
                    string segment = input.Substring(i, 8);

                    if (Regex.IsMatch(segment, "^[01]+$"))
                    {
                        result.Add($"0x{Convert.ToInt32(segment, 2):X2} ({Convert.ToInt32(segment, 2).ToString("D3"):X3})");      // Convert to hexadecimal and add to the list
                    }
                    else
                    {
                        result.Add(DetermineSegmentType(segment));      // Determine the type of segment
                    }
                }
                else
                {
                    result.Add("Incomplete");                           // Handle the case where the last segment is not complete
                }
            }

            return result.ToArray();
        }

        private static string DetermineSegmentType(string segment)
        {
            var distinctLetters = segment.Where(char.IsLetter).Distinct().ToArray();
            var containsDigits = segment.Any(char.IsDigit);

            if (distinctLetters.Length == 1 && containsDigits)
            {
                return "Partial operand";
            }
            if (distinctLetters.Length > 1 && containsDigits)
            {
                return "Partial operands";
            }
            if (distinctLetters.Length == 1 && !containsDigits)
            {
                return $"Operand {distinctLetters[0]}";
            }
            if (distinctLetters.Length > 1 && !containsDigits)
            {
                return "Mixed operands";
            }

            return "Unknown";
        }

        private static string GetCleanFormatCompactBits(string format)
        {
            return format.Replace('u', '0').Replace('A', '0').Replace('B', '0').Replace('C', '0').Replace('D', '0');
        }

        private static List<Tuple<char, byte>> GetFormatGroups(string format)
        {
            List<Tuple<char, byte>> groups = new List<Tuple<char, byte>>
            {
                new Tuple<char, byte>('n', 8)
            };

            string inFormat = format[8..];

            char lastGroup = ' ';
            byte count = 0;

            for (int i = 0; i < inFormat.Length; i++)
            {
                if (!lastGroup.Equals(inFormat[i]))
                {
                    if (lastGroup != ' ')
                        groups.Add(new Tuple<char, byte>(lastGroup, count));

                    lastGroup = inFormat[i];
                    count = 1;
                }
                else
                {
                    count++;
                }
            }

            groups.Add(new Tuple<char, byte>(lastGroup, count));

            return groups;
        }

        public static string GetOperandDescription(string operand, byte position)
        {
            return operand.Trim() switch
            {
                "r" => position == 0 ? "an 8-bit register specified by 'r'" : "the value of an 8-bit register specified by 'r'",
                "rr" => position == 0 ? "a 16-bit register specified by 'rr'" : "the value of a 16-bit register specified by 'rr'",
                "rrr" => position == 0 ? "a 24-bit register specified by 'rrr'" : "the value of a 24-bit register specified by 'rrr'",
                "rrrr" => position == 0 ? "a 32-bit register specified by 'rrrr'" : "the value of a 32-bit register specified by 'rrrr'",
                "fr" => position == 0 ? "a floating-point register" : "the value of a floating point register",
                "n" => "an 8-bit immediate numerical value (0x00 - 0xFF)",
                "nn" => "a 16-bit immediate numerical value (0x00 - 0xFFFF)",
                "nnn" => "a 24-bit immediate numerical value (0x00 - 0xFFFFFF)",
                "nnnn" => "a 32-bit immediate numerical value (0x00 - 0xFFFFFFFF)",
                "(nnn)" => "a value in memory addressed by immediate 24-bit value 'nnn'",
                "(rrr)" => "a value in memory addressed by the 24-bit register specified with 'rrr'",
                "ff" => "either of flags NZ, NC, SP, NO, PE, NE, LTE, GTE, Z, C, SN, OV, PO, EQ, GT, LT",
                _ => operand,
            };
        }

        public static string GenerateOPSTable()
        {
            string response = "";

            foreach (KeyValuePair<string, Oper> op in Mnem.OPS)
            {
                Oper oper = op.Value;

                if (!oper.IsPrimary)
                {
                    byte SubOp = Mnem.GetSecondaryOpCode(op.Key).Value;
                    string format = oper.Format;

                    byte subOpBitWidth = (byte)DataConverter.CountCharInString('o', format);

                    format = string.Join(
                        DataConverter.GetBinaryOfUint(SubOp, subOpBitWidth),
                        format.Split(new string('o', subOpBitWidth))
                    );

                    string mainOp = op.Key.Split(" ")[0];
                    string operands = op.Key.Split(" ")[1];

                    response += GetCorrectedFormat(op.Key, format) + ": " + DataConverter.GetBinaryOfUint(Mnem.GetPrimaryOpCode(mainOp), 8) + " " + format + Constants.CR;
                }
            }

            return response;
        }

        public static string GetCorrectedFormat(string input, string format)
        {
            if (input == "INT nnn,r")
                return "INT n,r";

            if (input == "LDF (nnn),nnn")
                return "LDF (nnn),n";

            if (input == "VCL nnn")
                return "VCL n";

            if (input == "VDL nnn")
                return "VDL n";

            if (input == "REGS nnn")
                return "REGS n";

            if (!input.Contains("nnn") || input.Contains(",(nnn)") || input.Contains("(nnn),"))
                return input;

            byte bBits = (byte)DataConverter.CountCharInString('B', format);

            byte bBytes = (byte)(bBits / 8);
            byte remainder = (byte)(bBits % 8);

            if (remainder != 0)
            {
                if (bBits == 13)    // 0 - 8191
                    return input.Replace(",nnn", ",n+");
                if (bBits == 5)     // 0 - 31
                    return input.Replace(",nnn", ",n-");

                return "!!!!!!!!  " + input;
            }

            if (bBytes == 1)
                return input.Replace(",nnn", ",n");

            if (bBytes == 2)
                return input.Replace(",nnn", ",nn");

            if (bBytes == 4)
                return input.Replace(",nnn", ",nnnn");

            return input;
        }
    }
}
