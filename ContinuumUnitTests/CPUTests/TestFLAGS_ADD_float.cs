using Continuum93.Emulator;

namespace CPUTests
{

    public class TestFLAGS_ADD_float
    {
        [Fact]
        public void TestAddFloatValues_Zero()
        {
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsZero());
            Assert.False(computer.CPU.FLAGS.IsNegative());
            Assert.False(computer.CPU.FLAGS.IsOverflow());

            float v1 = -1.0f;
            float v2 = 1.0f;

            computer.CPU.FREGS.AddFloatValues(v1, v2);

            Assert.True(computer.CPU.FLAGS.IsZero());
            Assert.False(computer.CPU.FLAGS.IsNegative());
            Assert.False(computer.CPU.FLAGS.IsOverflow());
        }

        [Fact]
        public void TestAddFloatValues_Negative()
        {
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsZero());
            Assert.False(computer.CPU.FLAGS.IsNegative());
            Assert.False(computer.CPU.FLAGS.IsOverflow());

            float v1 = -2.0f;
            float v2 = 1.0f;

            computer.CPU.FREGS.AddFloatValues(v1, v2);

            Assert.False(computer.CPU.FLAGS.IsZero());
            Assert.True(computer.CPU.FLAGS.IsNegative());
            Assert.False(computer.CPU.FLAGS.IsOverflow());
        }

        [Fact]
        public void TestAddFloatValues_Overflow()
        {
            using Computer computer = new();
            computer.CPU.FLAGS.ResetAll();

            Assert.False(computer.CPU.FLAGS.IsZero());
            Assert.False(computer.CPU.FLAGS.IsNegative());
            Assert.False(computer.CPU.FLAGS.IsOverflow());

            float v1 = 3.1E+35f;
            float v2 = float.MaxValue;

            computer.CPU.FREGS.AddFloatValues(v1, v2);

            Assert.False(computer.CPU.FLAGS.IsZero());
            Assert.False(computer.CPU.FLAGS.IsNegative());
            Assert.True(computer.CPU.FLAGS.IsOverflow());
        }
    }
}
