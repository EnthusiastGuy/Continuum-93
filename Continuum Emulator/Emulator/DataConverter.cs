using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Continuum93.Emulator
{
    public static class DataConverter
    {
        public const string VALID_LABEL_CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_?";
        private const string VALID_OCTAL_CHARACTERS = "01234567";
        static readonly Regex csvSplit = new(@"(?:^|,)\s*(""(?:[^""]|"""")*""|[^,]*)", RegexOptions.Compiled);

        public static string GetBinaryOfUint(uint value, byte bits = 24)
        {
            return GetBitFormatted(Convert.ToString(value, 2), bits);
        }

        public static string GetBitFormatted(string value, byte bits = 24)
        {
            return value.PadLeft(bits, '0')[^bits..];
        }

        public static int CountCharInString(char c, string str)
        {
            return str.Split(c).Length - 1;
        }

        public static string RemoveAllSpacesIn(string source)
        {
            return string.Join("", source.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
        }

        public static byte[] GetBytesFromBinaryString(string source)
        {
            if ((source.Length % 8) != 0)
                return null;

            int groupCount = source.Length / 8;
            byte[] bytes = new byte[groupCount];

            for (int i = 0; i < groupCount; i++)
                bytes[i] = Convert.ToByte(source.Substring(i * 8, 8), 2);

            return bytes;
        }

        public static string FormatFloatExplicit(float value)
        {
            string stringValue = value.ToString();  // Convert to a string with full precision

            if (value % 1 == 0)         // Check if it's an integer
                stringValue += ".0";    // Append ".0" to represent an integer

            return stringValue;
        }

        // Math
        public static uint? TryToGetValueOf(string challenge)
        {
            uint? response = null;
            uint? hex = GetHex(challenge);
            uint? bin = GetBin(challenge);
            uint? dec = GetDec(challenge);
            uint? oct = GetOct(challenge);

            if (hex.HasValue)
                response = hex;
            else if (bin.HasValue)
                response = bin;
            else if (oct.HasValue)
                response = oct;
            else if (dec.HasValue)
                response = dec;

            return response;
        }

        public static byte[] TryToGetBytesValueOf(string challenge)
        {
            byte[] response = new byte[] { };
            uint? hex = GetHex(challenge);
            uint? bin = GetBin(challenge);
            uint? oct = GetOct(challenge);
            uint? dec = GetDec(challenge);

            if (hex.HasValue)
                response = GetBytesFromHex(hex.Value, challenge.Trim());
            else if (bin.HasValue)
                response = GetBytesFromBin(bin.Value, challenge.Trim());
            else if (oct.HasValue)
                response = GetBytesFromOct(oct.Value, challenge.Trim());
            else if (dec.HasValue)
                response = GetBytesFromDec(dec.Value, challenge.Trim());

            return response;
        }

        public static float? TryToGetFloatValueOf(string challenge)
        {
            if (float.TryParse(challenge, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public static byte[] GetBytesFromHex(uint hexValue, string challenge)
        {
            int challengeLength = challenge.Contains("-") ? challenge.Length - 1 : challenge.Length;

            // Taking into account the "0x" prefix
            if (challengeLength == 3 || challengeLength == 4)  // Byte
                return new byte[] { (byte)hexValue };
            else if (challengeLength == 5 || challengeLength == 6)   // ushort
                return GetBytesFrom16bit((ushort)hexValue);
            else if (challengeLength == 7 || challengeLength == 8)   // uint (24 bit)
                return GetBytesFrom24bit(hexValue);
            else if (challengeLength == 9 || challengeLength == 10)   // uint (32 bit)
                return GetBytesFrom32bit(hexValue);

            return new byte[] { };
        }

        public static byte[] GetBytesFromBin(uint binValue, string challenge)
        {
            int challengeLength = challenge.Contains("-") ? challenge.Length - 1 : challenge.Length;

            // Taking into account the "0b" prefix
            if (challengeLength <= 2)
                return new byte[] { };
            else if (challengeLength <= 10)  // Byte
                return new byte[] { (byte)binValue };
            else if (challengeLength <= 18)   // ushort
                return GetBytesFrom16bit((ushort)binValue);
            else if (challengeLength <= 26)   // uint (24 bit)
                return GetBytesFrom24bit(binValue);
            else if (challengeLength <= 34)   // uint (32 bit)
                return GetBytesFrom32bit(binValue);

            return new byte[] { };
        }

        public static byte[] GetBytesFromOct(uint octValue, string challenge)
        {
            if (challenge.Length <= 2)
                return new byte[] { };

            string input = challenge.Replace("-", "")[2..];     // Also account for the possible sign

            var zeroPads = 0;
            for (var i = 0; i < input.Length && input[i] == '0'; i++)
                zeroPads++;

            if (zeroPads == 0)
            {
                if (octValue <= 0xFF)
                    return new byte[] { (byte)octValue };
                else if (octValue <= 0xFFFF)
                    return GetBytesFrom16bit((ushort)octValue);
                else if (octValue <= 0xFFFFFF)
                    return GetBytesFrom24bit(octValue);
                else if (octValue <= 0xFFFFFFFF)
                    return GetBytesFrom32bit(octValue);
            }
            else if (zeroPads == 1)
                return new byte[] { (byte)octValue };
            else if (zeroPads == 2)
                return GetBytesFrom16bit((ushort)octValue);
            else if (zeroPads == 3)
                return GetBytesFrom24bit(octValue);
            else if (zeroPads == 4)
                return GetBytesFrom32bit(octValue);

            return new byte[] { };
        }

        public static byte[] GetBytesFromDec(uint decValue, string challenge)
        {
            if (challenge.Length == 0)
                return new byte[] { };

            bool isNegative = challenge.Contains("-");
            string input = challenge.Replace("-", "");

            uint posNumber = uint.Parse(input);
            string hexPosNumber = posNumber.ToString("X");

            var zeroPads = 0;
            for (var i = 0; i < input.Length && input[i] == '0'; i++)
                zeroPads++;

            if (zeroPads == 0 && !isNegative)
            {
                if (decValue <= 0xFF)
                    return new byte[] { (byte)decValue };
                else if (decValue <= 0xFFFF)
                    return GetBytesFrom16bit((ushort)decValue);
                else if (decValue <= 0xFFFFFF)
                    return GetBytesFrom24bit(decValue);
                else if (decValue <= 0xFFFFFFFF)
                    return GetBytesFrom32bit(decValue);
            }
            else if (zeroPads == 0)
            {
                if (hexPosNumber.Length <= 2)
                    return new byte[] { (byte)decValue };
                else if (hexPosNumber.Length <= 4)
                    return GetBytesFrom16bit((ushort)decValue);
                else if (hexPosNumber.Length <= 6)
                    return GetBytesFrom24bit(decValue);
                else if (hexPosNumber.Length <= 8)
                    return GetBytesFrom32bit(decValue);
            }
            else if (zeroPads == 1)
                return new byte[] { (byte)decValue };
            else if (zeroPads == 2)
                return GetBytesFrom16bit((ushort)decValue);
            else if (zeroPads == 3)
                return GetBytesFrom24bit(decValue);
            else if (zeroPads == 4)
                return GetBytesFrom32bit(decValue);

            return new byte[] { };
        }

        // Try to get a number form a binary representation
        public static uint? GetBin(string challenge)
        {
            Regex binary = new("^[01]{1,32}$", RegexOptions.Compiled);
            uint? result = null;
            string cleanChallenge = challenge.Replace(" ", "");
            if (cleanChallenge.Length > 2 && cleanChallenge[..2].ToLowerInvariant() == "0b" && binary.IsMatch(cleanChallenge[2..]))
            {
                result = Convert.ToUInt32(cleanChallenge[2..], 2);
            }
            else if (cleanChallenge.Length > 3 && cleanChallenge[..3].ToLowerInvariant() == "-0b" && binary.IsMatch(cleanChallenge[3..]))
            {
                result = (uint)-(long)Convert.ToUInt64(cleanChallenge[3..], 2);
            }

            return result;
        }

        public static uint? GetDec(string challenge)
        {
            if (long.TryParse(challenge.Replace(" ", ""), out long result))
            {
                return (uint)result;
            }
            return null;
        }

        public static uint? GetHex(string challenge)
        {
            string cleanChallenge = challenge.Replace(" ", "");
            if (cleanChallenge.Length > 3 && cleanChallenge.StartsWith("-0x"))
            {
                string hexSubstring = cleanChallenge.Replace(" ", "")[3..];
                if (uint.TryParse(hexSubstring, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint result))
                {
                    // Calculate the two's complement for negative numbers
                    result = ~result + 1;
                    return result;
                }
            }
            else if (cleanChallenge.Length > 2 && cleanChallenge[..2].ToLowerInvariant() == "0x")
            {
                if (uint.TryParse(cleanChallenge[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint result))
                {
                    return result;
                }
            }

            return null;
        }

        public static uint? GetOct(string challenge)
        {
            string cleanChallenge = challenge.Replace(" ", "");
            if (cleanChallenge.Length > 2 && cleanChallenge[..2].ToLowerInvariant() == "0o")
            {
                string octalNumber = cleanChallenge[2..];
                if (IsValidOctal(octalNumber))
                {
                    return Convert.ToUInt32(octalNumber, 8);
                }
            }
            else if (cleanChallenge.Length > 3 && cleanChallenge[..3].ToLowerInvariant() == "-0o")
            {
                string octalNumber = cleanChallenge[3..];
                if (IsValidOctal(octalNumber))
                {
                    return (uint)-Convert.ToUInt32(octalNumber, 8);
                }
            }

            return null;
        }

        public static bool IsValidOctal(string challenge)
        {
            foreach (char c in challenge)
                if (!VALID_OCTAL_CHARACTERS.Contains(c.ToString()))
                    return false;

            return true;
        }

        public static string[] GetCSArguments(string challenge)
        {
            List<string> items = new List<string>();

            MatchCollection matches = csvSplit.Matches(challenge);
            foreach (Match match in matches)
            {
                string current = match.Value;

                if (0 == current.Length)
                {
                    items.Add("");
                }

                items.Add(current.TrimStart(',').Trim());
            }

            return items.ToArray();
        }

        public static bool HasValidRepetitionArgument(string challenge)
        {
            if (challenge.Length < 4 || !challenge[0].Equals('[') || !challenge.Contains(']'))
                return false;

            string repetition = challenge[1..challenge.IndexOf(']')];
            string subArgument = challenge[(challenge.IndexOf(']') + 1)..].Trim();

            if (!TryToGetValueOf(repetition.Replace("_", "")).HasValue)
                return false;

            uint repValue = TryToGetValueOf(repetition.Replace("_", "")).GetValueOrDefault();
            if (repValue < 1 || repValue > 0xFFFFFF)
                return false;

            if (HasStringContent(subArgument))
                return true;

            if (!TryToGetValueOf(subArgument.Replace("_", "")).HasValue)
                return false;

            return true;
        }

        public static byte[] GetRepetitionData(string challenge)
        {
            string repetition = challenge[1..challenge.IndexOf(']')];
            string subArgument = challenge[(challenge.IndexOf(']') + 1)..].Trim();

            uint repValue = TryToGetValueOf(repetition.Replace("_", "")).GetValueOrDefault(0);
            byte[] values = new byte[0];

            if (HasStringContent(subArgument))
            {
                string stringArg = GetStringContents(subArgument);
                if (stringArg != null)
                {
                    values = new byte[stringArg.Length];
                    byte[] bStr = Encoding.ASCII.GetBytes(stringArg);
                    for (int i = 0; i < stringArg.Length; i++)
                    {
                        values[i] = bStr[i];
                    }
                }
            }
            else
            {
                values = TryToGetBytesValueOf(subArgument.Replace("_", ""));
            }

            byte[] response = new byte[repValue * values.Length];

            int index = 0;

            for (int i = 0; i < repValue; i++)
            {
                for (int j = 0; j < values.Length; j++)
                {
                    response[index] = values[j];
                    index++;
                }
            }

            return response;
        }

        public static bool HasStringContent(string challenge)
        {
            return (challenge.Length > 2 && challenge[0].Equals('"') && challenge[^1].Equals(challenge[0]));
        }

        public static string GetStringContents(string challenge)
        {
            if (challenge.Length > 2 && challenge[0].Equals('"') && challenge[^1].Equals(challenge[0]))
                return challenge[1..^1];

            return null;
        }

        public static bool IsLabelValid(string possibleLabel)
        {
            if (possibleLabel.Length >= 2 && (possibleLabel[0] == '.' || possibleLabel[0] == '~'))
            {
                string tLabel = possibleLabel.ToUpper()[1..];
                if (possibleLabel.Length == 0 || char.IsDigit(possibleLabel[0]))
                    return false;

                foreach (char c in tLabel)
                    if (!VALID_LABEL_CHARACTERS.Contains(c))
                        return false;

                return true;
            }

            return false;
        }

        public static byte[] MergeByteArrays(byte[] first, byte[] second)
        {
            if (first == null && second == null)
                return Array.Empty<byte>();

            if (first == null)
                return second;

            if (second == null)
                return first;

            byte[] bytes = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, bytes, 0, first.Length);
            Buffer.BlockCopy(second, 0, bytes, first.Length, second.Length);
            return bytes;
        }

        public static byte[] MergeByteArrays(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }

        public static byte[] GetBytesFrom16bit(ushort input)
        {
            byte[] response = new byte[2];

            response[0] = (byte)(input / 256);
            response[1] = (byte)(input % 256);
            return response;
        }

        public static byte[] GetBytesFrom24bit(uint input)
        {
            byte[] response = BitConverter.GetBytes(input);

            return new byte[] { response[2], response[1], response[0] };
        }

        public static byte[] GetBytesFrom32bit(uint input)
        {
            byte[] response = BitConverter.GetBytes(input);
            Array.Reverse(response);

            return response;
        }

        public static byte GetNextRegister(byte regIndex, byte distance)
        {
            return (byte)((regIndex + distance) % 26);
        }

        public static string GetCrossPlatformPath(string input)
        {
            // return Path.Combine(input.Split('\\'));
            string normalizedPath = input.Replace('\\', Path.DirectorySeparatorChar);
            return normalizedPath;
        }
    }
}
