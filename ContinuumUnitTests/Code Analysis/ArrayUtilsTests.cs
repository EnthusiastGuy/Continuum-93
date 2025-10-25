using Continuum93.CodeAnalysis;

namespace Code_Analysis
{

    public class ArrayUtilsTests
    {
        [Fact]
        public void TestSplitArrayByTerminator_Multiple_Arrays()
        {
            byte[] input = new byte[] { 1, 2, 3, 0, 4, 5, 6, 0, 7, 8, 9 };
            List<byte[]> response = ArrayUtils.SplitArrayByTerminator(input);

            Assert.Equal(3, response.Count);
            Assert.Equal(new byte[] { 1, 2, 3 }, response[0]);
            Assert.Equal(new byte[] { 4, 5, 6 }, response[1]);
            Assert.Equal(new byte[] { 7, 8, 9 }, response[2]);
        }

        [Fact]
        public void TestSplitArrayByTerminator_Single_Array()
        {
            byte[] input = new byte[] { 1, 2, 3 };
            List<byte[]> response = ArrayUtils.SplitArrayByTerminator(input);

            Assert.Single(response);
            Assert.Equal(new byte[] { 1, 2, 3 }, response[0]);
        }

        [Fact]
        public void TestSplitArrayByTerminator_Multiple_Adjacent_Separators()
        {
            byte[] input = new byte[] { 1, 2, 3, 0, 0, 4, 5, 6 };
            List<byte[]> response = ArrayUtils.SplitArrayByTerminator(input);

            Assert.Equal(2, response.Count);
            Assert.Equal(new byte[] { 1, 2, 3 }, response[0]);
            Assert.Equal(new byte[] { 4, 5, 6 }, response[1]);
        }

        [Fact]
        public void TestSplitArrayByTerminator_Multiple_Adjacent_Separators_At_End()
        {
            byte[] input = new byte[] { 1, 2, 3, 4, 5, 6, 0, 0 };
            List<byte[]> response = ArrayUtils.SplitArrayByTerminator(input);

            Assert.Single(response);
            Assert.Equal(new byte[] { 1, 2, 3, 4, 5, 6 }, response[0]);
        }

        [Fact]
        public void TestSplitArrayByTerminator_Multiple_Adjacent_Separators_At_End_Many()
        {
            byte[] input = new byte[] { 1, 2, 3, 4, 5, 6, 0, 0, 0, 0 };
            List<byte[]> response = ArrayUtils.SplitArrayByTerminator(input);

            Assert.Single(response);
            Assert.Equal(new byte[] { 1, 2, 3, 4, 5, 6 }, response[0]);
        }
    }
}
