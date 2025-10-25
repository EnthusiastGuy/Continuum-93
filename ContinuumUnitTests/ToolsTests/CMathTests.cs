using Continuum93.Tools;

namespace ToolsTests
{
    public class CMathTests
    {
        [Theory]
        [InlineData(1.0f, 1.0f)]
        [InlineData(4.0f, 0.5f)]
        [InlineData(9.0f, 0.3333333f)] // Approximate value
        [InlineData(25.0f, 0.2f)]
        public void TestInverseSqrt(float input, float expected)
        {
            float result = CMath.InverseSqrt(input);
            Assert.Equal(expected, result, 0.0000001f);
        }

        [Theory]
        [InlineData(1.0f, 1.0f)]
        [InlineData(4.0f, 0.5f)]
        [InlineData(9.0f, 0.3333333f)] // Approximate value
        [InlineData(25.0f, 0.2f)]
        public void TestFastInverseSqrt(float input, float expected)
        {
            float result = CMath.QInverseSqrt(input);
            Assert.Equal(expected, result, 0.01f);
        }
    }
}
