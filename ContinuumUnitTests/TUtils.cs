using System.Diagnostics;
using System.Numerics;

namespace UnitTests
{
    public static class TUtils
    {
        public static uint _testsRan = 0;
        private static readonly Random rnd = new();

        public static void IncrementCountedTests(string testClass = "")
        {
            _testsRan++;
            if (_testsRan % 100 == 0)
                Debug.WriteLine("Tests ran: " + _testsRan);
        }

        public static string GetFormattedAsm(params string[] asmLines)
        {
            string response = "";
            foreach (string asmLine in asmLines)
                response += asmLine + Environment.NewLine;

            return response;
        }

        public static char Get8bitRegisterChar(byte position)
        {
            char c = 'A';
            c += (char)position;
            return c;
        }

        public static string Get16bitRegisterString(byte position)
        {
            if (position == 25)
                return "ZA";

            char c1 = Get8bitRegisterChar(position);
            char c2 = Get8bitRegisterChar((byte)(position + 1));

            return string.Format("{0}{1}", c1, c2);
        }

        public static string Get24bitRegisterString(byte position)
        {
            if (position == 24)
                return "YZA";
            if (position == 25)
                return "ZAB";

            char c1 = Get8bitRegisterChar(position);
            char c2 = Get8bitRegisterChar((byte)(position + 1));
            char c3 = Get8bitRegisterChar((byte)(position + 2));

            return string.Format("{0}{1}{2}", c1, c2, c3);
        }

        public static string Get32bitRegisterString(byte position)
        {
            if (position == 23)
                return "XYZA";
            if (position == 24)
                return "YZAB";
            if (position == 25)
                return "ZABC";

            char c1 = Get8bitRegisterChar(position);
            char c2 = Get8bitRegisterChar((byte)(position + 1));
            char c3 = Get8bitRegisterChar((byte)(position + 2));
            char c4 = Get8bitRegisterChar((byte)(position + 3));

            return string.Format("{0}{1}{2}{3}", c1, c2, c3, c4);
        }

        public static string GetFloatRegisterString(byte index)
        {
            return $"F{index}";
        }

        public static byte[] GetRandomByteArray(int length)
        {
            byte[] response = new byte[length];
            rnd.NextBytes(response);
            return response;
        }

        public static byte GetUnsignedByte(sbyte input)
        {
            return (byte)input;
        }

        public static ushort GetUnsignedShort(short input)
        {
            return (ushort)input;
        }

        public static uint GetUnsigned24BitInt(int input)
        {
            return (uint)(input & 0xFFFFFF);
        }

        public static uint GetUnsigned32BitInt(int input)
        {
            return (uint)input;
        }

        public static string GetFlagByIndex(byte index)
        {
            return index switch
            {
                0 => "NZ",
                1 => "NC",
                2 => "SP",
                3 => "NO",
                4 => "PE",
                5 => "NE",
                6 => "LTE",
                7 => "GTE",
                8 => "Z",
                9 => "C",
                10 => "SN",
                11 => "OV",
                12 => "PO",
                13 => "EQ",
                14 => "GT",
                15 => "LT",
                _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
            };
        }

        /// <summary>
        /// Returns a sample value in the range [0 … 2^bits–1], evenly divided into maxSequence segments,
        /// selecting the segment given by sequence.
        /// </summary>
        /// <param name="bits">Number of bits (e.g. 8, 12, 16, 32, …).</param>
        /// <param name="sequence">
        /// Zero-based index of which segment to pick; valid range: 0 … maxSequence.
        /// </param>
        /// <param name="maxSequence">
        /// The total number of segments to divide the [0…2^bits−1] range into; must be > 0.
        /// </param>
        /// <returns>
        /// A BigInteger in [0 … (2^bits−1)] corresponding to sequence/maxSequence of that range.
        /// </returns>
        public static uint GetSequentialSampleNumberOfBitCapacity(
            byte bits,
            byte sequence,
            byte maxSequence)
        {
            if (bits == 0)
                throw new ArgumentException("bits must be at least 1.", nameof(bits));
            if (maxSequence == 0)
                throw new ArgumentException("maxSequence must be at least 1.", nameof(maxSequence));
            if (sequence > maxSequence)
                throw new ArgumentOutOfRangeException(
                    nameof(sequence),
                    "sequence must be <= maxSequence.");

            // Compute 2^bits - 1
            BigInteger maxValue = (BigInteger.One << bits) - 1;

            // Scale: (maxValue * sequence) / maxSequence
            // This does integer division (i.e. floors the result).
            BigInteger result = (maxValue * sequence) / maxSequence;

            return (uint)result;
        }
    }
}
