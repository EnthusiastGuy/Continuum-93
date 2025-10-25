using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Continuum93.CodeAnalysis
{
    public class DInstruction
    {
        public bool Illegal = false;
        public string GeneralFormat;
        public string Instruction;
        public string SpecificInstruction;
        public byte InstructionLength;
        public uint Address;
        public bool IsRelativeAddress;
        public uint RelativeAddress;
        public List<byte> Bytes = new();
        public byte ArgumentCount;
        public List<DArg> ArgumentValues = new();
        public bool SimpleRepresentation;

        private string _compactFormat;

        public DInstruction(string generalFormat, string instruction, uint address, bool isRelativeAddress, bool simpleRepresentation)
        {
            GeneralFormat = generalFormat;
            Instruction = instruction;
            Address = address;
            IsRelativeAddress = isRelativeAddress;
            SimpleRepresentation = simpleRepresentation;

            CalculateData();
        }

        private void CalculateData()
        {
            _compactFormat = string.Join("", GeneralFormat.Split(' '));
            InstructionLength = (byte)(_compactFormat.Length / 8 + 1);
        }

        public void CalculateInstructionVariables()
        {
            try
            {
                if (InstructionLength < 2)
                    return;

                string[] tF = GeneralFormat.Split(' ');

                string bytesBin = "";
                for (int i = 1; i < Bytes.Count; i++)
                {
                    bytesBin += DUtils.ByteToBinary(Bytes[i]);
                }

                ArgumentCount = DUtils.GetArgumentsCount(_compactFormat);
                for (byte i = 0; i < ArgumentCount; i++)
                    ArgumentValues.Add(new DArg() { Index = i, SimpleRepresentation = SimpleRepresentation });

                for (int i = 0; i < _compactFormat.Length; i++)
                {
                    if (_compactFormat[i] == 'o' ||
                        _compactFormat[i] == 'u' ||
                        _compactFormat[i] == '0' ||
                        _compactFormat[i] == '1')
                        continue;

                    byte index = (byte)((byte)_compactFormat[i] - (byte)'A');

                    ArgumentValues[index].PushBit(bytesBin[i]);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

        }

        public void ComposeInstruction()
        {
            SpecificInstruction = Instruction;

            if (!Instruction.Contains(' '))
                return;

            string operand = Instruction.Split(' ')[0];
            string argsString = Instruction.Split(' ')[1];

            string[] args = argsString.Split(',');


            string[] rArgs = new string[args.Length];
            for (byte i = 0; i < args.Length; i++)
            {
                rArgs[i] = GetInterpretedInstruction(args[i], ArgumentValues[i]);
            }

            SpecificInstruction = ProcessParentheses(operand + " " + string.Join(", ", rArgs));
        }

        public static string ProcessParentheses(string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            // Pattern: 
            //  \(         literal '('
            //   ([^(),]+) Group 1: any run of chars except parentheses or comma
            //   ,\s*      the comma separator (with optional spaces)
            //   ([^(),]+) Group 2: same as above
            //  \)         literal ')'
            //
            // We assume no nested parentheses inside the two args.
            return Regex.Replace(
                input,
                @"\(([^(),]+),\s*([^(),]+)\)",
                match =>
                {
                    var left = match.Groups[1].Value.Trim();
                    var right = match.Groups[2].Value.Trim();

                    // if the right‐hand arg starts with '-', drop comma; else insert " +"
                    var sep = right.StartsWith("-", StringComparison.Ordinal)
                              ? ""
                              : " + ";

                    return $"({left}{sep}{right})";
                });
        }

        private string GetInterpretedInstruction(string instr, DArg arg)
        {
            if (DUtils.IsFloatRegisterForm(instr))
            {
                return DUtils.GetFloatRegisterRepresentation(arg.GetValue8Bit());
            }
            else if (DUtils.IsRegisterForm(instr))
            {
                return DUtils.GetRegisterRepresentation(instr, arg.GetValue8Bit());
            }
            else if (DUtils.IsNumberForm(instr))
            {
                return DUtils.GetNumberRepresentation(instr, arg, IsRelativeAddress, Address, IsSignedInstruction(), IsFloatInstruction());
            }
            else if (DUtils.IsFlagForm(instr))
            {
                return DUtils.GetFlagRepresentation(arg.GetValue8Bit());
            }

            return instr;
        }

        private bool IsSignedInstruction()
        {
            return (
                Instruction.StartsWith("SDIV") ||
                Instruction.StartsWith("SMUL") ||
                Instruction.StartsWith("SCP"));
        }

        private bool IsFloatInstruction()
        {
            return Instruction.Contains("fr");
        }

    }
}
