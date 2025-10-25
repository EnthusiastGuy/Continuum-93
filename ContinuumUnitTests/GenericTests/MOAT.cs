using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator.AutoDocs;
using Continuum93.Emulator.Mnemonics;
using ContinuumUnitTests._Tools;
using System.Text;

namespace GenericTests
{
    // Mother of all (assembly) tests
    public class MOAT
    {
        private static readonly char[] separator = ['\r', '\n'];
        private static readonly string header = FormatRow("Index", "Status", "Instruction", "Operands (dec)", "Expected (hex)", "Actual (hex)");

        [Fact]
        public void TestAllInstructionsAssembly()
        {
            var instructions = DebugInstructionSamples.GenerateRawInstructions()
                .Split(separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => ParseLine(line))
                .Where(instr => instr != null)
                .GroupBy(instr => instr.Oper.Mnemonic);

            var sb = new StringBuilder();
            var reportFull = new StringBuilder();
            int mainIndex = 1;
            int testCounter = 0;
            int testsFailed = 0;
            bool testsPassed = true;
            sb.AppendLine(header);
            sb.AppendLine(new string('=', 160));
            sb.AppendLine();

            foreach (var group in instructions)
            {
                

                string mainMnemonic = group.Key;
                Oper op = group.First().Oper;
                string format = op.Format;

                if (format.Equals(""))
                {
                    byte[] sExpected = [op.OpCode];
                    Assembler cp = new();
                    string status = "OK";
                    byte[] sActual = [];

                    try
                    {
                        testCounter++;
                        cp.Build(op.Mnemonic);
                        sActual = cp.GetCompiledCode();
                        if (!sExpected.SequenceEqual(sActual))
                        {
                            status = "NOK";
                            testsPassed = false;
                            testsFailed++;
                        }
                    }
                    catch
                    {
                        status = "ERROR";
                        sActual = [];
                        testsPassed = false;
                    }

                    string hexExpected = BitConverter.ToString(sExpected).Replace("-", ", ");
                    string hexActual = BitConverter.ToString(sActual).Replace("-", ", ");
                    sb.AppendLine(FormatRow($"{mainIndex}", status, op.Mnemonic, "N.A.", hexExpected, hexActual));
                }
                else
                {
                    // Header for instruction group
                    sb.AppendLine(header);
                    sb.AppendLine(FormatRow($"{mainIndex}", "", op.Mnemonic, "", "", ""));
                    sb.AppendLine(new string('-', 160));
                }
                
                byte operandCount = (byte)(mainMnemonic.Contains(',') ? 2 : 1);
                //byte bitCount = CountBits(format, 'B');

                int variantIndex = 1;

                IEnumerable<ParsedInstruction> variations = GenerateInstructionVariations(op);

                foreach (var parsed in variations)
                {
                    Assembler cp = new();
                    byte[] expected = BuildExpectedBytes(parsed.Oper, parsed.Args);
                    byte[] actual = [];
                    string status = "OK";

                    try
                    {
                        testCounter++;
                        cp.Build(parsed.Value);
                        actual = cp.GetCompiledCode();
                        if (!expected.SequenceEqual(actual))
                        {
                            status = "NOK";
                            testsPassed = false;
                            testsFailed++;
                        }
                    }
                    catch
                    {
                        status = "ERROR";
                        actual = [];
                        testsPassed = false;
                    }

                    string hexExpected = BitConverter.ToString(expected).Replace("-", ", ");
                    string hexActual = BitConverter.ToString(actual).Replace("-", ", ");
                    string argList = string.Join(", ", parsed.Args);

                    sb.AppendLine(FormatRow($" {mainIndex}.{variantIndex}", status, parsed.Value, argList, hexExpected, hexActual));
                    variantIndex++;
                }

                sb.AppendLine();
                mainIndex++;
            }

            reportFull.AppendLine(GetDescription(testCounter, testsFailed));
            reportFull.Append(sb);

            ReportsManager.SaveReport("InstructionEncodingTestReport", reportFull.ToString());
            Assert.True(testsFailed == 0, $"{testsFailed} tests failed. Check the report for details.");
        }

        private static IEnumerable<ParsedInstruction> GenerateInstructionVariations(Oper op)
        {
            string[] parts = op.Mnemonic.Split(' ', 2);
            if (parts.Length < 2) yield break;

            string opName = parts[0];
            string[] operands = parts[1].Split(',');

            byte variationCount = GetVariationCount(operands[0]);

            for (byte variation = 0; variation < variationCount; variation+=1)
            {
                List<string> operandList = [];
                List<uint> args = [];

                byte phasedIndex = variation;
                byte numberPhaseIndex = variation;
                byte fRegPhaseIndex = variation;

                for (int i = 0; i < operands.Length; i++)
                {
                    string token = operands[i].Trim();
                    char formatChar = GetFormatLetterForOperand(i);
                    byte bitCount = CountBits(op.Format, formatChar);

                    if (token == "r")
                    {
                        operandList.Add(TUtils.Get8bitRegisterChar(phasedIndex).ToString());
                        args.Add(phasedIndex);
                        phasedIndex = (byte)((phasedIndex + 1) % 26);
                    }
                    else if (token.Equals("rr"))
                    {
                        operandList.Add(TUtils.Get16bitRegisterString(phasedIndex));
                        args.Add(phasedIndex);
                        phasedIndex = (byte)((phasedIndex + 2) % 26);
                    }
                    else if (token.Equals("rrr"))
                    {
                        operandList.Add(TUtils.Get24bitRegisterString(phasedIndex));
                        args.Add(phasedIndex);
                        phasedIndex = (byte)((phasedIndex + 3) % 26);
                    }
                    else if (token.Equals("rrrr"))
                    {
                        operandList.Add(TUtils.Get32bitRegisterString(phasedIndex));
                        args.Add(phasedIndex);
                        phasedIndex = (byte)((phasedIndex + 4) % 26);
                    }
                    else if (!token.Contains(')') && token.StartsWith('n'))
                    {
                        uint val = TUtils.GetSequentialSampleNumberOfBitCapacity(bitCount, numberPhaseIndex, variationCount);
                        operandList.Add(val.ToString());
                        args.Add(val);
                        numberPhaseIndex = (byte)((numberPhaseIndex + 4) % variationCount);
                    }
                    else if (token.Equals("(nnn)"))
                    {
                        uint val = TUtils.GetSequentialSampleNumberOfBitCapacity(bitCount, numberPhaseIndex, variationCount);
                        operandList.Add($"({val})");
                        args.Add(val);
                        numberPhaseIndex = (byte)((numberPhaseIndex + 4) % variationCount);
                    }
                    else if (token.Equals("(nnn"))
                    {
                        uint val = TUtils.GetSequentialSampleNumberOfBitCapacity(bitCount, numberPhaseIndex, variationCount);
                        operandList.Add($"({val}");
                        args.Add(val);
                        numberPhaseIndex = (byte)((numberPhaseIndex + 4) % variationCount);
                    }
                    else if (token.Equals("nnn)"))
                    {
                        uint val = TUtils.GetSequentialSampleNumberOfBitCapacity(bitCount, numberPhaseIndex, variationCount);
                        operandList.Add($"+ {val})");
                        args.Add(val);
                        numberPhaseIndex = (byte)((numberPhaseIndex + 4) % variationCount);
                    }
                    else if (token.Equals("(rrr)"))
                    {
                        operandList.Add($"({TUtils.Get24bitRegisterString(phasedIndex)})");
                        args.Add(phasedIndex);
                        phasedIndex = (byte)((phasedIndex + 3) % 26);
                    }
                    else if (token.Equals("r)"))
                    {
                        operandList.Add($"+ {TUtils.Get8bitRegisterChar(phasedIndex)})");
                        args.Add(phasedIndex);
                        phasedIndex = (byte)((phasedIndex + 3) % 26);
                    }
                    else if (token.Equals("rr)"))
                    {
                        operandList.Add($"+ {TUtils.Get16bitRegisterString(phasedIndex)})");
                        args.Add(phasedIndex);
                        phasedIndex = (byte)((phasedIndex + 3) % 26);
                    }
                    else if (token.Equals("rrr)"))
                    {
                        operandList.Add($"+ {TUtils.Get24bitRegisterString(phasedIndex)})");
                        args.Add(phasedIndex);
                        phasedIndex = (byte)((phasedIndex + 3) % 26);
                    }
                    else if (token.Equals("(rrr"))
                    {
                        operandList.Add($"({TUtils.Get24bitRegisterString(phasedIndex)}");
                        args.Add(phasedIndex);
                        phasedIndex = (byte)((phasedIndex + 3) % 26);
                    }
                    else if (token == "fr")
                    {
                        byte fIndex = (byte)(fRegPhaseIndex % 16);
                        operandList.Add($"{TUtils.GetFloatRegisterString(fIndex)}");
                        args.Add(fIndex);
                        fRegPhaseIndex = (byte)((fRegPhaseIndex + 1) % 16);
                    }
                    else if (token == "ff")
                    {
                        operandList.Add($"{TUtils.GetFlagByIndex(variation)}");
                        args.Add(variation);
                    }
                    else
                    {
                        operandList.Add("?");
                        args.Add(0);
                    }
                }

                string value = $"{opName} {string.Join(",", operandList)}";

                yield return new ParsedInstruction
                {
                    Oper = op,
                    Value = RemoveCommasInParentheses(value),
                    Args = [.. args]
                };
            }
        }

        public static string RemoveCommasInParentheses(string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            var sb = new StringBuilder();
            int level = 0;

            foreach (char c in input)
            {
                if (c == '(')
                {
                    level++;
                    sb.Append(c);
                }
                else if (c == ')')
                {
                    if (level > 0) level--;
                    sb.Append(c);
                }
                else if (c == ',' && level > 0)
                {
                    // skip commas inside parentheses
                    sb.Append(' ');
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        private static ParsedInstruction ParseLine(string line)
        {
            string[] parts = line.Split(';');
            if (parts.Length != 2) return null;

            string mnemonic = parts[0].Trim();
            if (!Mnem.OPS.TryGetValue(mnemonic, out Oper op)) return null;

            return new ParsedInstruction { Oper = op, Value = mnemonic, Args = [0] };
        }

        private static byte GetVariationCount(string operand)
        {
            if (operand.StartsWith('r') || operand.StartsWith("(r"))
            {
                return 26;
            } else if (operand.StartsWith('n') || operand.StartsWith("(n"))
            {
                return 5;
            } else if (operand.StartsWith("fr"))
            {
                return 16;
            } else if (operand.StartsWith("ff"))
            {
                return 16;
            }

            return 1;
        }

        private static char GetFormatLetterForOperand(int index)
        {
            return (uint)index < 6
                ? (char)('A' + index)
                : 'u';
        }

        private static byte CountBits(string format, char letter) => (byte)format.Count(c => c == letter);

        private static byte[] BuildExpectedBytes(Oper op, uint[] args)
        {
            string format = op.Format;
            List<bool> bits = new();
            Dictionary<char, int> argBitOffsets = new() { { 'A', 0 }, { 'B', 0 }, { 'C', 0 }, { 'D', 0 }, { 'E', 0 }, { 'F', 0 } };

            int subOpBitIndex = 0;
            for (int i = 0; i < format.Length; i++)
            {
                char c = format[i];
                if (i < 8 && (c == 'o'))
                {
                    int totalSubOpBits = format.Take(8).Count(fc => fc == 'o');
                    int bitPosition = totalSubOpBits - 1 - subOpBitIndex++;
                    bits.Add((op.OpCodes[0] & (1 << bitPosition)) != 0);
                }
                else if (c == '1') bits.Add(true);
                else if (c == '0' || c == 'u') bits.Add(false);
                else if (c == 'A' || c == 'B' || c == 'C' || c == 'D' || c == 'E' || c == 'F')
                {
                    int argIndex = c switch { 'A' => 0, 'B' => 1, 'C' => 2, 'D' => 3, 'E' => 4, 'F' => 5, _ => 0 };
                    int bitOffset = argBitOffsets[c]++;
                    int bitInArg = CountBits(format, c) - 1 - bitOffset;
                    bits.Add(((args[argIndex] >> bitInArg) & 1) == 1);
                }
            }

            List<byte> bytes = new() { op.ParentCode ?? 0 };
            for (int i = 0; i < bits.Count; i += 8)
            {
                byte val = 0;
                for (int j = 0; j < 8 && i + j < bits.Count; j++)
                {
                    if (bits[i + j])
                        val |= (byte)(1 << (7 - j));
                }
                bytes.Add(val);
            }

            return bytes.ToArray();
        }

        private static string FormatRow(string index, string status, string instruction, string operands, string expected, string actual, bool hideSeparator = false)
        {
            return string.Format("{0,9} {1,-8} {2,-30} {3,-22} {4,-40} {5}",
                                 index, status, instruction, operands, expected, actual);
        }

        private static string GetDescription(int totalTests, int testsFailed)
        {
            var now = DateTime.Now;
            string daySuffix = now.Day switch
            {
                1 or 21 or 31 => "st",
                2 or 22 => "nd",
                3 or 23 => "rd",
                _ => "th"
            };

            string formattedDate = $"{now:dd}'{daySuffix} of {now:MMMM} {now:yyyy} at {now:HH:mm:ss}";

            return $"This report validates the encoding logic of all defined assembly instructions for Continuum 93. Each instruction is assembled and\r\nits output byte sequence is compared against the expected encoded format based on the instruction definition.\r\n\r\n" +
                   $"For each instruction variant, the report includes:\r\n" +
                   $"- Index: Instruction number and variant\r\n" +
                   $"- Status: OK if the encoding matched, NOK or ERROR if not\r\n" +
                   $"- Instruction: The human-readable assembly instruction\r\n" +
                   $"- Operands (dec): Operand values in decimal\r\n" +
                   $"- Expected (hex): Expected byte output from the encoder\r\n" +
                   $"- Actual (hex): Actual byte output produced by the assembler\r\n" +
                   $"Discrepancies between expected and actual encodings are flagged, making it easier to detect implementation bugs or definition mismatches.\r\n\r\n" +
                   $"Total tests ran: {totalTests}. Failed tests: {testsFailed}\r\n" +
                   $"Report generated on the {formattedDate}.\r\n\r\n";
        }


        private class ParsedInstruction
        {
            public Oper Oper;
            public string Value;
            public uint[] Args;
        }
    }
}
