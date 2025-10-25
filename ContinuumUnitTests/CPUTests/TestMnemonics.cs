using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;

namespace CPUTests
{

    public class TestMnemonics
    {
        [Fact]
        public void TestTryGetRegisterIndex()
        {
            for (int i = 0; i < 26; i++)
            {
                for (int l = 1; l <= 4; l++)
                {
                    string regCandidate = Constants.ALPHABET.Substring(i, l);

                    byte? response = Mnem.TryGetRegisterIndex(regCandidate);

                    Assert.True(response.HasValue);
                    Assert.Equal(i, response.Value);
                }
            }
        }

        [Fact]
        public void TestTryGetRegisterIndexFail()
        {
            string[] failCases = new string[] { "AA", "AC", "CB", "ABD", "XBA", "XYX", "ABCDE" };

            foreach (string failCase in failCases)
            {
                byte? response = Mnem.TryGetRegisterIndex(failCase);
                Assert.False(response.HasValue);
            }
        }

        [Fact]
        public void TestTryGetFloatRegisterIndex()
        {
            for (int i = 0; i < 16; i++)
            {
                for (int l = 1; l <= 3; l++)
                {
                    string regCandidate = $"F{i.ToString().PadLeft(l, '0')}";

                    byte? response = Mnem.TryGetFloatRegisterIndex(regCandidate);

                    Assert.True(response.HasValue);
                    Assert.Equal(i, response.Value);
                }
            }
        }

        [Fact]
        public void TestTryGetFloatRegisterIndexFail()
        {
            string[] failCases = new string[] {
                "F", "F16", "16", "04", "FF", "F00256", "F0_2" };

            foreach (string failCase in failCases)
            {
                byte? response = Mnem.TryGetFloatRegisterIndex(failCase);
                Assert.False(response.HasValue);
            }
        }
    }
}
