
using Continuum93.Emulator.Controls;
using Microsoft.Xna.Framework.Input;

namespace Input
{

    public class TestKeyboardStateExt
    {
        [Fact]
        public void TestGetStateAsBytes()
        {
            bool[] testState = new bool[256];

            testState[0] = true;
            testState[1] = true;

            testState[9] = true;
            testState[11] = true;
            testState[255] = true;

            KeyboardStateExt.ClearState();
            KeyboardStateExt.SetStateForTests(testState);

            byte[] byteState = KeyboardStateExt.GetStateAsBytes();

            Assert.Equal(32, byteState.Length);
            Assert.Equal(0b11000000, byteState[0]);
            Assert.Equal(0b01010000, byteState[1]);
            #region Asserting zeros
            Assert.Equal(0, byteState[2]);
            Assert.Equal(0, byteState[3]);
            Assert.Equal(0, byteState[4]);
            Assert.Equal(0, byteState[5]);
            Assert.Equal(0, byteState[6]);
            Assert.Equal(0, byteState[7]);
            Assert.Equal(0, byteState[8]);
            Assert.Equal(0, byteState[9]);
            Assert.Equal(0, byteState[10]);
            Assert.Equal(0, byteState[11]);
            Assert.Equal(0, byteState[12]);
            Assert.Equal(0, byteState[13]);
            Assert.Equal(0, byteState[14]);
            Assert.Equal(0, byteState[15]);
            Assert.Equal(0, byteState[16]);
            Assert.Equal(0, byteState[17]);
            Assert.Equal(0, byteState[18]);
            Assert.Equal(0, byteState[19]);
            Assert.Equal(0, byteState[20]);
            Assert.Equal(0, byteState[21]);
            Assert.Equal(0, byteState[22]);
            Assert.Equal(0, byteState[23]);
            Assert.Equal(0, byteState[24]);
            Assert.Equal(0, byteState[25]);
            Assert.Equal(0, byteState[26]);
            Assert.Equal(0, byteState[27]);
            Assert.Equal(0, byteState[28]);
            Assert.Equal(0, byteState[29]);
            Assert.Equal(0, byteState[30]);
            #endregion
            Assert.Equal(1, byteState[31]);

        }

        [Fact]
        public void TestRegisterKeys()
        {
            KeyboardStateExt.ClearState();
            KeyboardStateExt.RegisterKeyDown(Keys.Space);

            byte[] byteState = KeyboardStateExt.GetStateAsBytes();
            Assert.Equal(0b10000000, byteState[4]);

            KeyboardStateExt.RegisterKeyUp(Keys.Space);

            byteState = KeyboardStateExt.GetStateAsBytes();
            Assert.Equal(0, byteState[4]);
        }

        [Fact]
        public void TestBitShifting()
        {
            bool[] testState = new bool[256];

            testState[0] = true;
            testState[1] = false;
            testState[2] = true;
            testState[3] = false;
            testState[4] = true;
            testState[5] = false;
            testState[6] = true;
            testState[7] = false;

            KeyboardStateExt.ClearState();
            KeyboardStateExt.SetStateForTests(testState);

            byte[] byteState = KeyboardStateExt.GetStateAsBytes();
            Assert.Equal(0b10101010, byteState[0]);

            testState[0] = false;
            testState[1] = true;
            testState[2] = false;
            testState[3] = true;
            testState[4] = false;
            testState[5] = true;
            testState[6] = false;
            testState[7] = true;

            KeyboardStateExt.SetStateForTests(testState);

            byteState = KeyboardStateExt.GetStateAsBytes();
            Assert.Equal(0b01010101, byteState[0]);
        }

        [Fact]
        public void TestCorrectSeparationOfBytes()
        {
            bool[] testState = new bool[256];

            testState[0] = true;
            testState[1] = false;
            testState[2] = true;
            testState[3] = false;
            testState[4] = true;
            testState[5] = false;
            testState[6] = true;
            testState[7] = false;

            testState[8] = false;
            testState[9] = true;
            testState[10] = false;
            testState[11] = true;
            testState[12] = false;
            testState[13] = true;
            testState[14] = false;
            testState[15] = true;

            KeyboardStateExt.ClearState();
            KeyboardStateExt.SetStateForTests(testState);

            byte[] byteState = KeyboardStateExt.GetStateAsBytes();
            Assert.Equal(0b10101010, byteState[0]);
            Assert.Equal(0b01010101, byteState[1]);
        }

        [Fact]
        public void GetStateAsCodes()
        {
            bool[] testState = new bool[256];

            testState[0] = true;
            testState[1] = false;
            testState[2] = true;
            testState[3] = false;
            testState[4] = true;
            testState[5] = false;
            testState[6] = true;
            testState[7] = false;

            KeyboardStateExt.ClearState();
            KeyboardStateExt.SetStateForTests(testState);

            byte[] byteState = KeyboardStateExt.GetStateAsCodes();

            Assert.Equal(4, byteState.Length);
            Assert.Equal(new byte[] { 0, 2, 4, 6 }, byteState);
        }

        [Fact]
        public void TestGetTotalOfPressedKeys()
        {
            KeyboardStateExt.ClearState();
            KeyboardStateExt.RegisterKeyDown(Keys.LeftControl);
            KeyboardStateExt.RegisterKeyDown(Keys.RightControl);
            KeyboardStateExt.RegisterKeyDown(Keys.Escape);
            KeyboardStateExt.RegisterKeyUp(Keys.RightControl);

            Assert.Equal(2, KeyboardStateExt.GetTotalOfPressedKeys());

            KeyboardStateExt.RegisterKeyUp(Keys.Escape);
            KeyboardStateExt.RegisterKeyUp(Keys.Escape);
            KeyboardStateExt.RegisterKeyUp(Keys.Escape);

            Assert.Equal(1, KeyboardStateExt.GetTotalOfPressedKeys());

            KeyboardStateExt.RegisterKeyDown(Keys.W);
            KeyboardStateExt.RegisterKeyDown(Keys.A);
            KeyboardStateExt.RegisterKeyDown(Keys.S);
            KeyboardStateExt.RegisterKeyDown(Keys.D);
            KeyboardStateExt.RegisterKeyDown(Keys.S);
            KeyboardStateExt.RegisterKeyDown(Keys.W);

            Assert.Equal(5, KeyboardStateExt.GetTotalOfPressedKeys());

            KeyboardStateExt.RegisterKeyUp(Keys.W);
            KeyboardStateExt.RegisterKeyUp(Keys.A);
            KeyboardStateExt.RegisterKeyUp(Keys.S);
            KeyboardStateExt.RegisterKeyUp(Keys.D);
            KeyboardStateExt.RegisterKeyUp(Keys.LeftControl);

            Assert.Equal(0, KeyboardStateExt.GetTotalOfPressedKeys());
        }
    }
}
