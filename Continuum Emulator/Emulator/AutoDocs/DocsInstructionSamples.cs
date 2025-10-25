using Continuum93.Emulator;
using Continuum93.Emulator.AutoDocs;
using Continuum93.Emulator.Mnemonics;
using System;
using System.Collections.Generic;

namespace Continuum93.Emulator.AutoDocs
{
    public static class DocsInstructionSamples
    {
        private static readonly Random random = new();
        private static readonly string[] flags = new string[] { "NC", "SP", "NO", "PE", "NE", "LTE", "GTE", "Z", "C", "SN", "OV", "PO", "EQ", "GT", "LT" };
        private static readonly string[] labels = new string[] { ".videoData", ".fontData", ".loop1", ".loop2", ".loop3", ".MainList", ".inputBuffer", ".videoBuffer", ".paletteData", ".palette1", ".palette2", ".randomData" };

        public static string Generate()
        {
            string response = $"; This is a sample source file with all instructions, populated{Constants.CR}" +
                $"; with random data{Constants.CR}{Constants.CR}";

            response += $"\t#include someFile.asm{Constants.CR}";
            response += $"\t#include some other file.asm{Constants.CR}{Constants.CR}";

            response += $"\t#ORG 0x3A0000{Constants.CR}{Constants.CR}";
            response += $".MainList     ; do not try to execute this code.{Constants.CR}{Constants.CR}";
            response += $".randomData{Constants.CR}";
            response += $"\t#DB \"A nice string right here \\0\", [100] 255, 0xffffff, 0xffff0d, 0x0dffff, 0x0aff0a, 0xff8f00, 0xff08ff, 0xff032b, 0x7d7da3, 0x3c80db, 0x007062, 0x2929ff, 0x6e0085, 0x800034, 0x2d006e, 0x260a34, 0x000000{Constants.CR}{Constants.CR}";

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
                            response += $"\t{FillDummyData(oper, op.Key)}{Constants.CR}";
                            foundSubops = true;
                        }
                    }

                    if (!foundSubops)
                    {
                        response += $"\t{primeOp.Key}{Constants.CR}";
                    }
                }

            }

            return response;
        }

        public static string GenerateForBenchmark()
        {
            string response = "";

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
                            response += $"\t{FillDummyData(oper, op.Key, true)}{Constants.CR}";
                            foundSubops = true;
                        }
                    }

                    if (!foundSubops)
                    {
                        response += $"\t{primeOp.Key}{Constants.CR}";
                    }
                }

            }

            return response;
        }

        private static string FillDummyData(Oper oper, string key, bool noLabels = false)
        {
            string format = oper.Format;
            byte SubOp = Mnem.GetSecondaryOpCode(key).Value;
            byte subOpBitWidth = (byte)DataConverter.CountCharInString('o', format);

            format = string.Join(
                DataConverter.GetBinaryOfUint(SubOp, subOpBitWidth),
                format.Split(new string('o', subOpBitWidth))
            );

            string correctedFormat = ASMReferenceGenerator.GetCorrectedFormat(key, format);

            string mainOp = correctedFormat.Split(" ")[0];
            string operands = correctedFormat.Split(" ")[1];

            string[] operandList = operands.Split(",");

            for (byte i = 0; i < operandList.Length; i++)
            {
                operandList[i] = SetDummyOperands(operandList[i], i, noLabels);
            }

            return $"{mainOp} {string.Join(", ", operandList)}";
        }

        private static string SetDummyOperands(string operand, byte position, bool noLabels)
        {
            return operand.Trim() switch
            {
                "r" => GetRandomRegister(1),
                "rr" => GetRandomRegister(2),
                "rrr" => GetRandomRegister(3),
                "rrrr" => GetRandomRegister(4),
                "n" => GetRandomNumber(1),
                "nn" => GetRandomNumber(2),
                "nnn" => GetRandomAddressOrLabel(noLabels),
                "nnnn" => GetRandomNumber(4),
                "(nnn)" => $"({GetRandomAddressOrLabel(noLabels)})",
                "(rrr)" => $"({GetRandomRegister(3)})",
                "ff" => GetRandomFlag(),
                _ => operand,
            };
        }

        private static string GetRandomRegister(byte length)
        {
            return Constants.ALPHABET.Substring(random.Next(25), length);
        }

        private static string GetRandomNumber(int bytes)
        {
            byte numberFormatSelector = (byte)random.Next(100);

            string response;
            if (numberFormatSelector <= 10)
            {
                response = GenerateOctalNumber(bytes);
            }
            else if (numberFormatSelector <= 30)
            {
                response = GenerateBinaryNumber(bytes);
            }
            else if (numberFormatSelector <= 70)
            {
                response = GenerateDecimalNumber(bytes);
            }
            else
            {
                response = GenerateHexadecimalNumber(bytes);
            }

            return response;
        }

        static string GenerateDecimalNumber(int bytes)
        {
            int maxValue = (int)Math.Pow(2, bytes * 8) - 1;
            int randomNumber = random.Next(0, maxValue / 2 + 1);

            return randomNumber.ToString();
        }

        static string GenerateOctalNumber(int bytes)
        {
            int maxValue = (int)Math.Pow(2, bytes * 8) - 1;
            int randomNumber = random.Next(0, maxValue / 2 + 1);

            string octalNumber = Convert.ToString(randomNumber, 8);

            // Pad with zeroes to reach the desired length
            int paddingZeros = bytes * 2 - octalNumber.Length; // Each octal digit is equivalent to 3 bits, so 2 octal digits per byte
            if (paddingZeros > 0)
            {
                octalNumber = new string('0', paddingZeros) + octalNumber;
            }

            return $"0o{octalNumber}";
        }

        static string GenerateHexadecimalNumber(int bytes)
        {
            int maxValue = (int)Math.Pow(2, bytes * 8) - 1;
            int randomNumber = random.Next(0, maxValue / 2 + 1);

            string hexNumber = randomNumber.ToString("X"); // Convert to hexadecimal

            // Pad with zeroes to reach the desired length
            int paddingZeros = bytes * 2 - hexNumber.Length;
            if (paddingZeros > 0)
            {
                hexNumber = new string('0', paddingZeros) + hexNumber;
            }

            return $"0x{hexNumber}";
        }

        static string GenerateBinaryNumber(int bytes)
        {
            int maxValue = (int)Math.Pow(2, bytes * 8) - 1;
            int randomNumber = random.Next(0, maxValue / 2 + 1);

            string binaryNumber = Convert.ToString(randomNumber, 2); // Convert to binary

            // Pad with zeroes to reach the desired length
            int paddingZeros = bytes * 8 - binaryNumber.Length;
            if (paddingZeros > 0)
            {
                binaryNumber = new string('0', paddingZeros) + binaryNumber;
            }

            return $"0b{binaryNumber}";
        }

        static string GetRandomAddressOrLabel(bool noLabels = false)
        {
            byte oddsRoll = (byte)random.Next(100);

            string response;
            if (oddsRoll <= 20 && !noLabels)
            {
                response = labels[random.Next(labels.Length)];
            }
            else
            {
                response = GetRandomNumber(3);
            }

            return response;
        }

        static string GetRandomFlag()
        {
            return flags[random.Next(flags.Length)];
        }
    }
}
