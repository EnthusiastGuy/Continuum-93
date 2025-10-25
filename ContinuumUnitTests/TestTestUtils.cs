using Continuum93.Emulator;


namespace TestTestUtils
{

    public class TestTestUtils
    {
        [Fact]
        public void TestGet8bitRegisterChar()
        {
            //string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            for (byte i = 0; i < 26; i++)
            {
                char expected = Constants.ALPHABET[i];
                char actual = TUtils.Get8bitRegisterChar(i);

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void TestGet16bitRegisterString()
        {
            string[] alphabet = new string[] { "AB", "BC", "CD", "DE", "EF", "FG", "GH", "HI", "IJ", "JK", "KL", "LM", "MN", "NO", "OP", "PQ", "QR", "RS", "ST", "TU", "UV", "VW", "WX", "XY", "YZ", "ZA" };
            for (byte i = 0; i < 26; i++)
            {
                string expected = alphabet[i];
                string actual = TUtils.Get16bitRegisterString(i);

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void TestGet24bitRegisterString()
        {
            string[] alphabet = new string[] { "ABC", "BCD", "CDE", "DEF", "EFG", "FGH", "GHI", "HIJ", "IJK", "JKL", "KLM", "LMN", "MNO", "NOP", "OPQ", "PQR", "QRS", "RST", "STU", "TUV", "UVW", "VWX", "WXY", "XYZ", "YZA", "ZAB" };
            for (byte i = 0; i < 26; i++)
            {
                string expected = alphabet[i];
                string actual = TUtils.Get24bitRegisterString(i);

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void TestGet32bitRegisterString()
        {
            string[] alphabet = new string[] { "ABCD", "BCDE", "CDEF", "DEFG", "EFGH", "FGHI", "GHIJ", "HIJK", "IJKL", "JKLM", "KLMN", "LMNO", "MNOP", "NOPQ", "OPQR", "PQRS", "QRST", "RSTU", "STUV", "TUVW", "UVWX", "VWXY", "WXYZ", "XYZA", "YZAB", "ZABC" };
            for (byte i = 0; i < 26; i++)
            {
                string expected = alphabet[i];
                string actual = TUtils.Get32bitRegisterString(i);

                Assert.Equal(expected, actual);
            }
        }
    }
}
