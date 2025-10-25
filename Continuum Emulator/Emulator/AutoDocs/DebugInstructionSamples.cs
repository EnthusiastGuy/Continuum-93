using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;

namespace Continuum93.Emulator.AutoDocs
{
    public static class DebugInstructionSamples
    {
        static List<DebugInstruction> Instructions = [];
        public static List<string> possibleBodies = new();

        public static string Generate()
        {
            GenerateInstructions();

            string list = "";
            foreach (var instruction in Instructions)
            {
                if (instruction.Primary)
                {
                    list += $"{instruction.Mnemonic}{Environment.NewLine}";
                }
                else
                {
                    list += $"{instruction.Operand} {instruction.MnemonicValued}{Environment.NewLine}";
                }
            }

            

            return list;
        }

        public static string GenerateRawInstructions()
        {
            GenerateInstructions();

            string list = "";
            foreach (var instruction in Instructions)
            {
                list += $"{instruction.Mnemonic};{instruction.Format} {Environment.NewLine}";
            }

            return list;
        }

        public static string GenerateUniqueInstructionsList()
        {
            GenerateUniqueInstructions();

            string list = "";
            foreach (var instruction in Instructions)
            {
                list += $"{instruction.OpCode},0x{instruction.OpCode:X2},{instruction.Mnemonic}{Environment.NewLine}";
            }

            return list.ToString();
        }

        public static void GenerateUniqueInstructions()
        {
            Instructions.Clear();

            foreach (KeyValuePair<string, Oper> primeOp in Mnem.OPS)
            {
                Oper oper = primeOp.Value;

                if (oper.IsPrimary)
                {
                    Instructions.Add(new DebugInstruction(oper));
                }
            }
        }

        private static void GenerateInstructions()
        {
            Instructions.Clear();

            foreach (KeyValuePair<string, Oper> primeOp in Mnem.OPS)
            {
                Oper oper = primeOp.Value;

                if (oper.IsPrimary)
                {
                    bool foundSubops = false;

                    foreach (KeyValuePair<string, Oper> op in Mnem.OPS)
                    {
                        string opMatch = op.Key.Split(" ")[0].Trim();
                        if (!op.Value.IsPrimary && primeOp.Key.Equals(opMatch))
                        {
                            Instructions.Add(new DebugInstruction(op.Value));
                            foundSubops = true;
                        }
                    }

                    if (!foundSubops)
                    {
                        Instructions.Add(new DebugInstruction(oper));
                    }
                }
            }
        }
    }

    public class DebugInstruction
    {
        private static readonly Regex UnclosedParen = new Regex(@"^\([^)]*$", RegexOptions.Compiled);

        public bool Primary;
        public bool FloatInstruction = false;
        public string Operand;
        public string Mnemonic;
        public byte OpCode;
        public string Format;
        public List<InstrArg> Arguments = [];
        public string MnemonicValued;

        public DebugInstruction(Oper oper)
        {
            Mnemonic = oper.Mnemonic;
            OpCode = oper.OpCode;
            Format = oper.Format;
            Primary = oper.IsPrimary;
            Process();
        }

        private void Process()
        {
            if (!Primary)
            {
                Operand = Mnemonic.Split(' ')[0].Trim();
                string argsString = Mnemonic.Split(' ')[1];

                FloatInstruction = argsString.Contains("fr");

                string[] args = argsString.Split(',');
                for (byte i = 0; i < args.Length; i++)
                {
                    if (!DebugInstructionSamples.possibleBodies.Contains(args[i]))
                    {
                        DebugInstructionSamples.possibleBodies.Add(args[i]);
                    }
                    Arguments.Add(new InstrArg(args[i], Format, i, FloatInstruction));
                }
            }

            //MnemonicValued = string.Join(", ", Arguments.Select(arg => arg.BodyValue));
            var sb = new StringBuilder();
            for (int i = 0; i < Arguments.Count; i++)
            {
                var val = Arguments[i].BodyValue;
                sb.Append(val);

                // if this is an unclosed-paren piece, skip any separator—
                // the next arg already starts with " +"
                if (UnclosedParen.IsMatch(val))
                    continue;

                // otherwise, if there’s another arg coming, append a comma+space
                if (i < Arguments.Count - 1)
                    sb.Append(", ");
            }
            MnemonicValued = sb.ToString();
        }
    }

    public class InstrArg
    {
        private static readonly string[] flags = ["NC", "SP", "NO", "PE", "NE", "LTE", "GTE", "Z", "C", "SN", "OV", "PO", "EQ", "GT", "LT"];
        static Random random = new();

        public string Body;
        public byte BitCount = 0;
        public int MaxValue;
        public string BodyValue;
        public bool FloatParent;

        public InstrArg(string body, string format, byte index, bool floatParent)
        {
            Body = body;
            FloatParent = floatParent;
            char searchChar = (char)('A' + index);
            foreach (char c in format)
            {
                if (c == searchChar)
                {
                    BitCount++;
                }
            }

            MaxValue = (int)Math.Pow(2, BitCount) - 1;

            BodyValue = GenerateValue();
        }

        private string GenerateValue()
        {
            return Body switch
            {
                "r" => GetRandomRegister(1),
                "rr" => GetRandomRegister(2),
                "rrr" => GetRandomRegister(3),
                "rrrr" => GetRandomRegister(4),
                "fr" => GetRandomFloatRegister(),
                "n" => FloatParent ? GetRandomFloat() : GenerateHexadecimalNumber(BitCount),
                "nn" => FloatParent ? GetRandomFloat() : GenerateHexadecimalNumber(BitCount),
                "nnn" => FloatParent ? GetRandomFloat() : GenerateHexadecimalNumber(BitCount),
                "nnnn" => FloatParent ? GetRandomFloat() : GenerateHexadecimalNumber(BitCount),
                "(nnn)" => $"({GenerateHexadecimalNumber(BitCount)})",
                "(nnn" => $"({GenerateHexadecimalNumber(BitCount)}",
                "nnn)" => $" + {GenerateHexadecimalNumber(24)})",
                "(rrr)" => $"({GetRandomRegister(3)})",
                "(rrr" => $"({GetRandomRegister(3)}",
                "r)" => $" + {GetRandomRegister(1)})",
                "rr)" => $" + {GetRandomRegister(2)})",
                "rrr)" => $" + {GetRandomRegister(3)})",
                "ff" => GetRandomFlag(),
                _ => Body,
            };
        }

        private static string GetRandomRegister(byte length)
        {
            return Constants.ALPHABET.Substring(random.Next(25), length);
        }

        private static string GetRandomFloatRegister()
        {
            return $"F{random.Next(15)}";
        }

        private static string GetRandomFlag()
        {
            return flags[random.Next(flags.Length)];
        }

        static string GetRandomFloat()
        {
            return ((float)random.NextDouble()).ToString();
        }

        private string GenerateHexadecimalNumber(int bits)
        {
            int randomNumber = random.Next(0, (MaxValue / 2 + 1));

            string hexNumber = randomNumber.ToString("X"); // Convert to hexadecimal

            byte bytes = (byte)Math.Ceiling(((double)bits / 8));

            // Pad with zeroes to reach the desired length
            int paddingZeros = bytes * 2 - hexNumber.Length;
            if (paddingZeros > 0)
            {
                hexNumber = new string('0', paddingZeros) + hexNumber;
            }

            return $"0x{hexNumber}";
        }
    }

}
