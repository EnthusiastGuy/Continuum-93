using Continuum93.Emulator;


namespace TestUtils
{

    public class TesUtils
    {
        [Fact]
        public void TestGetBinaryOfUint()
        {
            // Default 24 bit
            string actual = DataConverter.GetBinaryOfUint(65536);
            Assert.Equal("000000010000000000000000", actual);

            actual = DataConverter.GetBinaryOfUint(7, 2);
            Assert.Equal("11", actual);

            actual = DataConverter.GetBinaryOfUint(5, 5);
            Assert.Equal("00101", actual);

            actual = DataConverter.GetBinaryOfUint(255, 5);
            Assert.Equal("11111", actual);
        }

        [Fact]
        public void TestGetBitFormatted()
        {
            string actual = DataConverter.GetBitFormatted("1001011010");
            Assert.Equal("000000000000001001011010", actual);

            actual = DataConverter.GetBitFormatted("1001011010", 2);
            Assert.Equal("10", actual);
        }

        [Fact]
        public void TestCountCharInString()
        {
            string challenge = "oooooouu1111122222222 00000000";
            int actual = DataConverter.CountCharInString('o', challenge);
            Assert.Equal(6, actual);

            actual = DataConverter.CountCharInString('1', challenge);
            Assert.Equal(5, actual);

            actual = DataConverter.CountCharInString('2', challenge);
            Assert.Equal(8, actual);

            actual = DataConverter.CountCharInString('0', challenge);
            Assert.Equal(8, actual);
        }

        [Fact]
        public void TestRemoveAllSpacesIn()
        {
            string challenge = " ooooo ouu1111   122222222 000000 00    ";
            string actual = DataConverter.RemoveAllSpacesIn(challenge);

            Assert.Equal("oooooouu111112222222200000000", actual);
        }

        [Fact]
        public void TestGetBytesFromBinaryString()
        {
            string challenge = "00000100000000110000001000000001";
            byte[] actual = DataConverter.GetBytesFromBinaryString(challenge);

            Assert.Equal(4, actual.Length);
            Assert.Equal(4, actual[0]);
            Assert.Equal(3, actual[1]);
            Assert.Equal(2, actual[2]);
            Assert.Equal(1, actual[3]);
        }

        [Fact]
        public void TestTryToGetPositiveValueOf()
        {
            uint? actual = DataConverter.TryToGetValueOf("0xFFFF");
            Assert.Equal(65535, (int?)actual);

            actual = DataConverter.TryToGetValueOf("32744");
            Assert.Equal(32744, (int?)actual);

            actual = DataConverter.TryToGetValueOf("0b10101010111");
            Assert.Equal(1367, (int?)actual);

            actual = DataConverter.TryToGetValueOf("AAafdds$%^&*(964");
            Assert.False(actual.HasValue);
        }

        [Fact]
        public void TestTryToGetNegativeValueOfHex()
        {
            // Hex
            uint? actual = DataConverter.TryToGetValueOf("-0x0A");
            Assert.Equal(0xFFFFFFF6, actual);

            actual = DataConverter.TryToGetValueOf("-0xFF");
            Assert.Equal(0xFFFFFF01, actual);

            actual = DataConverter.TryToGetValueOf("-0xFFFF");
            Assert.Equal(0xFFFF0001, actual);

            actual = DataConverter.TryToGetValueOf("-0xFFFFFF");
            Assert.Equal(0xFF000001, actual);

            actual = DataConverter.TryToGetValueOf("-0xFFFFFFFF");
            Assert.Equal(0x01, (long?)actual);

            actual = DataConverter.TryToGetValueOf("- 0x0A");
            Assert.Equal(0xFFFFFFF6, actual);
        }

        [Fact]
        public void TestTryToGetNegativeValueOfDecimal()
        {
            // Decimal
            uint? actual = DataConverter.TryToGetValueOf("-1");
            Assert.Equal(0xFFFFFFFF, actual);

            actual = DataConverter.TryToGetValueOf("- 1");
            Assert.Equal(0xFFFFFFFF, actual);
        }

        [Fact]
        public void TestTryToGetNegativeValuesOfBinary()
        {
            // Binary
            uint? actual = DataConverter.TryToGetValueOf("-0b00000001");
            Assert.Equal(0xFFFFFFFF, actual);

            actual = DataConverter.TryToGetValueOf("- 0b00000001");
            Assert.Equal(0xFFFFFFFF, actual);

            actual = DataConverter.TryToGetValueOf("-0b11111111");
            Assert.Equal(0xFFFFFF01, actual);

            actual = DataConverter.TryToGetValueOf("-0b1111111111111111");
            Assert.Equal(0xFFFF0001, actual);

            actual = DataConverter.TryToGetValueOf("-0b111111111111111111111111");
            Assert.Equal(0xFF000001, actual);

            actual = DataConverter.TryToGetValueOf("-0b11111111 11111111 11111111 11111111");
            Assert.Equal(0x01, (long?)actual);
        }

        [Fact]
        public void TestTryToGetNegativeValuesOfOctal()
        {
            // Binary
            uint? actual = DataConverter.TryToGetValueOf("-0o001");
            Assert.Equal(0xFFFFFFFF, actual);

            actual = DataConverter.TryToGetValueOf("- 0o001");
            Assert.Equal(0xFFFFFFFF, actual);

            actual = DataConverter.TryToGetValueOf("-0o377");
            Assert.Equal(0xFFFFFF01, actual);

            actual = DataConverter.TryToGetValueOf("-0o177777");
            Assert.Equal(0xFFFF0001, actual);

            actual = DataConverter.TryToGetValueOf("-0o77777777");
            Assert.Equal(0xFF000001, actual);

            actual = DataConverter.TryToGetValueOf("-0o37777777777");
            Assert.Equal(0x01, (long?)actual);
        }

        [Fact]
        public void TestTryGetFloatValueOf()
        {
            float? actual = DataConverter.TryToGetFloatValueOf("0.125");
            Assert.Equal(0.125f, actual);

            actual = DataConverter.TryToGetFloatValueOf(".125");
            Assert.Equal(0.125f, actual);
        }

        [Fact]
        public void TestGetBytesFromHex()
        {
            Assert.Equal(
                new byte[] { 255 },
                DataConverter.GetBytesFromHex(255, "0xFF"));
            Assert.Equal(
                new byte[] { 0, 255 },
                DataConverter.GetBytesFromHex(255, "0x0FF"));
            Assert.Equal(
                new byte[] { 10, 255 },
                DataConverter.GetBytesFromHex(2815, "0xAFF"));
            Assert.Equal(
                new byte[] { 10, 255 },
                DataConverter.GetBytesFromHex(2815, "0x0AFF"));
            Assert.Equal(
                new byte[] { 0, 10, 255 },
                DataConverter.GetBytesFromHex(2815, "0x00AFF"));
            Assert.Equal(
                new byte[] { 240, 10, 255 },
                DataConverter.GetBytesFromHex(15731455, "0xF00AFF"));
            Assert.Equal(
                new byte[] { 0, 240, 10, 255 },
                DataConverter.GetBytesFromHex(15731455, "0x0F00AFF"));
            Assert.Equal(
                new byte[] { 32, 240, 10, 255 },
                DataConverter.GetBytesFromHex(552602367, "0x20F00AFF"));
            Assert.Equal(
                new byte[] { },
                DataConverter.GetBytesFromHex(552602367, "0x020F00AFF"));
        }

        [Fact]
        public void TestGetBytesFromBin()
        {
            Assert.Equal(
                new byte[] { },
                DataConverter.GetBytesFromBin(0, "0b"));
            Assert.Equal(
                new byte[] { 1 },
                DataConverter.GetBytesFromBin(1, "0b1"));
            Assert.Equal(
                new byte[] { 19 },
                DataConverter.GetBytesFromBin(19, "0b00010011"));
            Assert.Equal(
                new byte[] { 83 },
                DataConverter.GetBytesFromBin(83, "0b01010011"));
            Assert.Equal(
                new byte[] { 0, 83 },
                DataConverter.GetBytesFromBin(83, "0b001010011"));
            Assert.Equal(
                new byte[] { 50, 83 },
                DataConverter.GetBytesFromBin(12883, "0b11001001010011"));
            Assert.Equal(
                new byte[] { 0, 50, 83 },
                DataConverter.GetBytesFromBin(12883, "0b000011001001010011"));
            Assert.Equal(
                new byte[] { 12, 50, 83 },
                DataConverter.GetBytesFromBin(799315, "0b000011000011001001010011"));
            Assert.Equal(
                new byte[] { 1, 12, 50, 83 },
                DataConverter.GetBytesFromBin(17576531, "0b1000011000011001001010011"));
            Assert.Equal(
                new byte[] { 71, 12, 50, 83 },
                DataConverter.GetBytesFromBin(1191981651, "0b01000111000011000011001001010011"));
            Assert.Equal(
                new byte[] { },
                DataConverter.GetBytesFromBin(1191981651, "0b001000111000011000011001001010011"));
        }

        [Fact]
        public void TestGetBytesFromOct()
        {
            Assert.Equal(
                new byte[] { },
                DataConverter.GetBytesFromOct(0, "0o"));
            Assert.Equal(
                new byte[] { 1 },
                DataConverter.GetBytesFromOct(1, "0o1"));
            Assert.Equal(
                new byte[] { 1 },
                DataConverter.GetBytesFromOct(1, "0o01"));
            Assert.Equal(
                new byte[] { 0, 1 },
                DataConverter.GetBytesFromOct(1, "0o001"));
            Assert.Equal(
                new byte[] { 0, 0, 1 },
                DataConverter.GetBytesFromOct(1, "0o0001"));
            Assert.Equal(
                new byte[] { 0, 0, 0, 1 },
                DataConverter.GetBytesFromOct(1, "0o00001"));
            Assert.Equal(
                new byte[] { 1, 0 },
                DataConverter.GetBytesFromOct(0x100, "0o400"));
            Assert.Equal(
                new byte[] { 0 },
                DataConverter.GetBytesFromOct(0x100, "0o0400"));
            Assert.Equal(
                new byte[] { 1, 0 },
                DataConverter.GetBytesFromOct(0x100, "0o00400"));
            Assert.Equal(
                new byte[] { 0, 1, 0 },
                DataConverter.GetBytesFromOct(0x100, "0o000400"));
            Assert.Equal(
                new byte[] { 0, 0, 1, 0 },
                DataConverter.GetBytesFromOct(0x100, "0o0000400"));
            Assert.Equal(
                new byte[] { 0xFF, 0xFF },
                DataConverter.GetBytesFromOct(0xFFFF, "0o177777"));
            Assert.Equal(
                new byte[] { 0xFF },
                DataConverter.GetBytesFromOct(0xFFFF, "0o0177777"));
            Assert.Equal(
                new byte[] { 0xFF, 0xFF },
                DataConverter.GetBytesFromOct(0xFFFF, "0o00177777"));
            Assert.Equal(
                new byte[] { 0, 0xFF, 0xFF },
                DataConverter.GetBytesFromOct(0xFFFF, "0o000177777"));
            Assert.Equal(
                new byte[] { 0, 0, 0xFF, 0xFF },
                DataConverter.GetBytesFromOct(0xFFFF, "0o0000177777"));
            Assert.Equal(
                new byte[] { },
                DataConverter.GetBytesFromOct(0xFFFF, "0o00000177777"));
            Assert.Equal(
                new byte[] { 1, 0, 0 },
                DataConverter.GetBytesFromOct(0x10000, "0o200000"));
            Assert.Equal(
                new byte[] { 0 },
                DataConverter.GetBytesFromOct(0x10000, "0o0200000"));
            Assert.Equal(
                new byte[] { 0, 0 },
                DataConverter.GetBytesFromOct(0x10000, "0o00200000"));
            Assert.Equal(
                new byte[] { 1, 0, 0 },
                DataConverter.GetBytesFromOct(0x10000, "0o000200000"));
            Assert.Equal(
                new byte[] { 0, 1, 0, 0 },
                DataConverter.GetBytesFromOct(0x10000, "0o0000200000"));
            Assert.Equal(
                new byte[] { },
                DataConverter.GetBytesFromOct(0x10000, "0o00000200000"));
            Assert.Equal(
                new byte[] { 1, 0xFF, 0xFF, 0xFF },
                DataConverter.GetBytesFromOct(0x1FFFFFF, "0o177777777"));
            Assert.Equal(
                new byte[] { 0xFF },
                DataConverter.GetBytesFromOct(0x1FFFFFF, "0o0177777777"));
            Assert.Equal(
                new byte[] { 0xFF, 0xFF },
                DataConverter.GetBytesFromOct(0x1FFFFFF, "0o00177777777"));
            Assert.Equal(
                new byte[] { 0xFF, 0xFF, 0xFF },
                DataConverter.GetBytesFromOct(0x1FFFFFF, "0o000177777777"));
            Assert.Equal(
                new byte[] { 1, 0xFF, 0xFF, 0xFF },
                DataConverter.GetBytesFromOct(0x1FFFFFF, "0o0000177777777"));
            Assert.Equal(
                new byte[] { },
                DataConverter.GetBytesFromOct(0x1FFFFFF, "0o00000177777777"));
        }


        [Fact]
        public void TestGetBytesFromDec()
        {
            Assert.Equal(
                new byte[] { },
                DataConverter.GetBytesFromDec(0, ""));
            Assert.Equal(
                new byte[] { 1 },
                DataConverter.GetBytesFromDec(1, "1"));
            Assert.Equal(
                new byte[] { 1 },
                DataConverter.GetBytesFromDec(1, "01"));
            Assert.Equal(
                new byte[] { 0, 1 },
                DataConverter.GetBytesFromDec(1, "001"));
            Assert.Equal(
                new byte[] { 0, 0, 1 },
                DataConverter.GetBytesFromDec(1, "0001"));
            Assert.Equal(
                new byte[] { 0, 0, 0, 1 },
                DataConverter.GetBytesFromDec(1, "00001"));
            Assert.Equal(
                new byte[] { 1, 0 },
                DataConverter.GetBytesFromDec(0x100, "256"));
            Assert.Equal(
                new byte[] { 0 },
                DataConverter.GetBytesFromDec(0x100, "0256"));
            Assert.Equal(
                new byte[] { 1, 0 },
                DataConverter.GetBytesFromDec(0x100, "00256"));
            Assert.Equal(
                new byte[] { 0, 1, 0 },
                DataConverter.GetBytesFromDec(0x100, "000256"));
            Assert.Equal(
                new byte[] { 0, 0, 1, 0 },
                DataConverter.GetBytesFromDec(0x100, "0000256"));
            Assert.Equal(
                new byte[] { 0xFF, 0xFF },
                DataConverter.GetBytesFromDec(0xFFFF, "655355"));
            Assert.Equal(
                new byte[] { 0xFF },
                DataConverter.GetBytesFromDec(0xFFFF, "0655355"));
            Assert.Equal(
                new byte[] { 0xFF, 0xFF },
                DataConverter.GetBytesFromDec(0xFFFF, "00655355"));
            Assert.Equal(
                new byte[] { 0, 0xFF, 0xFF },
                DataConverter.GetBytesFromDec(0xFFFF, "000655355"));
            Assert.Equal(
                new byte[] { 0, 0, 0xFF, 0xFF },
                DataConverter.GetBytesFromDec(0xFFFF, "0000655355"));
            Assert.Equal(
                new byte[] { },
                DataConverter.GetBytesFromDec(0xFFFF, "0000065535"));
            Assert.Equal(
                new byte[] { 1, 0, 0 },
                DataConverter.GetBytesFromDec(0x10000, "65536"));
            Assert.Equal(
                new byte[] { 0 },
                DataConverter.GetBytesFromDec(0x10000, "065536"));
            Assert.Equal(
                new byte[] { 0, 0 },
                DataConverter.GetBytesFromDec(0x10000, "0065536"));
            Assert.Equal(
                new byte[] { 1, 0, 0 },
                DataConverter.GetBytesFromDec(0x10000, "00065536"));
            Assert.Equal(
                new byte[] { 0, 1, 0, 0 },
                DataConverter.GetBytesFromDec(0x10000, "000065536"));
            Assert.Equal(
                new byte[] { },
                DataConverter.GetBytesFromDec(0x10000, "0000065536"));
            Assert.Equal(
                new byte[] { 1, 0xFF, 0xFF, 0xFF },
                DataConverter.GetBytesFromDec(0x1FFFFFF, "33554431"));
            Assert.Equal(
                new byte[] { 0xFF },
                DataConverter.GetBytesFromDec(0x1FFFFFF, "033554431"));
            Assert.Equal(
                new byte[] { 0xFF, 0xFF },
                DataConverter.GetBytesFromDec(0x1FFFFFF, "0033554431"));
            Assert.Equal(
                new byte[] { 0xFF, 0xFF, 0xFF },
                DataConverter.GetBytesFromDec(0x1FFFFFF, "00033554431"));
            Assert.Equal(
                new byte[] { 1, 0xFF, 0xFF, 0xFF },
                DataConverter.GetBytesFromDec(0x1FFFFFF, "000033554431"));
            Assert.Equal(
                new byte[] { },
                DataConverter.GetBytesFromDec(0x1FFFFFF, "0000033554431"));
        }

        [Fact]
        public void TestIsLabelValid()
        {
            Assert.True(DataConverter.IsLabelValid(".SomeLabel"));
            Assert.True(DataConverter.IsLabelValid("._LOOP1"));
            Assert.True(DataConverter.IsLabelValid("~DT?123"));
            Assert.False(DataConverter.IsLabelValid(""));
            Assert.False(DataConverter.IsLabelValid("4~SomeLabel"));
            Assert.False(DataConverter.IsLabelValid("Lab.el@"));
            Assert.False(DataConverter.IsLabelValid("Some label"));
            string illegals = " !@#$%^&*()-=+\\|}{][';\":/><.,`~";

            foreach (char c in illegals)
            {
                Assert.False(DataConverter.IsLabelValid("Label" + c));
            }
        }

        [Fact]
        public void TestMergeByteArrays()
        {
            byte[] arr1 = new byte[] { 1, 2, 3 };
            byte[] arr2 = new byte[] { 7, 8, 9 };
            byte[] expected = new byte[] { 1, 2, 3, 7, 8, 9 };
            byte[] actual = DataConverter.MergeByteArrays(arr1, arr2);

            Assert.Equal(expected, actual);

            actual = DataConverter.MergeByteArrays(arr2, arr1);
            expected = new byte[] { 7, 8, 9, 1, 2, 3 };

            Assert.Equal(expected, actual);

            byte[] arr = Array.Empty<byte>();
            actual = DataConverter.MergeByteArrays(arr, arr1);
            expected = new byte[] { 1, 2, 3 };

            Assert.Equal(expected, actual);

            byte[] arrr = Array.Empty<byte>();
            actual = DataConverter.MergeByteArrays(arr, arrr);
            expected = Array.Empty<byte>();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestGetBytesFrom16bit()
        {
            byte[] expected = new byte[] { 255, 254 };
            byte[] actual = DataConverter.GetBytesFrom16bit(65534);

            Assert.Equal(expected, actual);

            expected = new byte[] { 0, 200 };
            actual = DataConverter.GetBytesFrom16bit(200);

            Assert.Equal(expected, actual);

            expected = new byte[] { 2, 88 };
            actual = DataConverter.GetBytesFrom16bit(600);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestGetBytesFrom24bit()
        {
            byte[] expected = new byte[] { 0, 255, 254 };
            byte[] actual = DataConverter.GetBytesFrom24bit(65534);

            Assert.Equal(expected, actual);

            expected = new byte[] { 1, 56, 128 };
            actual = DataConverter.GetBytesFrom24bit(80000);

            Assert.Equal(expected, actual);

            expected = new byte[] { 9, 216, 157 };
            actual = DataConverter.GetBytesFrom24bit(645_277);



            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestGetBytesFrom32bit()
        {
            byte[] expected = new byte[] { 69, 137, 50, 113 };
            byte[] actual = DataConverter.GetBytesFrom32bit(1166619249);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestHasRepetitionArgument()
        {
            Assert.True(DataConverter.HasValidRepetitionArgument("[100] 0"));
            Assert.True(DataConverter.HasValidRepetitionArgument("[1_000]0"));
            Assert.True(DataConverter.HasValidRepetitionArgument("[0x100] 255"));
            Assert.True(DataConverter.HasValidRepetitionArgument("[0o100] 0xFF"));
            Assert.True(DataConverter.HasValidRepetitionArgument("[0b101] 0xff"));
            Assert.True(DataConverter.HasValidRepetitionArgument("[0b101] 0xff_ff"));
            Assert.True(DataConverter.HasValidRepetitionArgument("[10] \"string\""));
            Assert.False(DataConverter.HasValidRepetitionArgument("[0x1000000] 0"));
            Assert.False(DataConverter.HasValidRepetitionArgument("[0b101 ff"));
            Assert.False(DataConverter.HasValidRepetitionArgument("[A] ff"));
            Assert.False(DataConverter.HasValidRepetitionArgument("[0x0G] ff"));
            Assert.False(DataConverter.HasValidRepetitionArgument("[0o9] ff"));
            Assert.False(DataConverter.HasValidRepetitionArgument("[0b12] ff"));
            Assert.False(DataConverter.HasValidRepetitionArgument("[] 0"));
            Assert.False(DataConverter.HasValidRepetitionArgument("[0] 0"));
            Assert.False(DataConverter.HasValidRepetitionArgument("[-1] 0"));
            Assert.False(DataConverter.HasValidRepetitionArgument("[1] [1] 0"));
        }

        [Fact]
        public void TestGetRepetitionData()
        {
            Assert.Equal(new byte[] { 23, 23, 23, 23, 23, 23, 23, 23, 23, 23 },
                DataConverter.GetRepetitionData("[10] 23"));
            Assert.Equal(new byte[] { 0xAB, 0xAB, 0xAB, 0xAB, 0xAB, 0xAB, 0xAB, 0xAB, 0xAB, 0xAB },
                DataConverter.GetRepetitionData("[0x0A] 0xAB"));
            Assert.Equal(new byte[] { 0xAB, 0xCD, 0xAB, 0xCD, 0xAB, 0xCD },
                DataConverter.GetRepetitionData("[3] 0xABCD"));
            Assert.Equal(new byte[] { 84, 101, 115, 116, 0, 84, 101, 115, 116, 0, 84, 101, 115, 116, 0, 84, 101, 115, 116, 0 },
                DataConverter.GetRepetitionData("[04] \"Test\0\""));
        }

        [Fact]
        public void TestHasStringContent()
        {
            Assert.True(DataConverter.HasStringContent("\"Some text \"which\" is a string\""));
            Assert.False(DataConverter.HasStringContent("\"Some text \"which\" is not a string"));
            Assert.False(DataConverter.HasStringContent("Some text \"which\" is not a string\""));
            Assert.False(DataConverter.HasStringContent("Some text \"which\" is not a string"));
        }

        [Fact]
        public void TestGetStringContents()
        {
            string expected = "This is something we are all aware of";
            string actual = DataConverter.GetStringContents('"' + expected + '"');

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestGetCSArguments()
        {
            Assert.Equal(
                new string[] { "\"This, is a regular string\"", "244", "0xFF", "0b00110011" },
                DataConverter.GetCSArguments("\"This, is a regular string\", 244, 0xFF, 0b00110011")
            );

            Assert.Equal(
                new string[] { "\"This, is a regular string\"", "244", "\"0xFF\"", "0b00110011" },
                DataConverter.GetCSArguments("\"This, is a regular string\", 244, \"0xFF\", 0b00110011")
            );
        }

        [Fact]
        public void TestIsValidOctal()
        {
            Assert.True(DataConverter.IsValidOctal("000"));
            Assert.True(DataConverter.IsValidOctal("010"));
            Assert.True(DataConverter.IsValidOctal("015"));
            Assert.False(DataConverter.IsValidOctal("018"));
        }

        [Fact]
        public void TestGetOctal()
        {
            Assert.Equal(45, (long)DataConverter.GetOct("0o55").GetValueOrDefault(0));
            Assert.Equal(0, (long)DataConverter.GetOct("0o59").GetValueOrDefault(0));
            Assert.Equal(319, (long)DataConverter.GetOct("0o477").GetValueOrDefault(0));
            Assert.Equal(0, (long)DataConverter.GetOct("477").GetValueOrDefault(0));
        }
    }
}
