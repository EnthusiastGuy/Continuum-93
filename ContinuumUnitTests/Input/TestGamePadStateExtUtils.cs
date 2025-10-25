using Continuum93.Emulator.Controls;

namespace Input
{

    public class TestGamePadStateExtUtils
    {
        [Fact]
        public void TestConvertControllerFloatToByte()
        {
            Assert.Equal(000, GamepadStateExts.ConvertControllerFloatToByte(-1.00f));
            Assert.Equal(006, GamepadStateExts.ConvertControllerFloatToByte(-0.95f));
            Assert.Equal(012, GamepadStateExts.ConvertControllerFloatToByte(-0.90f));
            Assert.Equal(031, GamepadStateExts.ConvertControllerFloatToByte(-0.75f));
            Assert.Equal(063, GamepadStateExts.ConvertControllerFloatToByte(-0.50f));
            Assert.Equal(095, GamepadStateExts.ConvertControllerFloatToByte(-0.25f));
            Assert.Equal(102, GamepadStateExts.ConvertControllerFloatToByte(-0.20f));
            Assert.Equal(108, GamepadStateExts.ConvertControllerFloatToByte(-0.15f));
            Assert.Equal(114, GamepadStateExts.ConvertControllerFloatToByte(-0.10f));
            Assert.Equal(127, GamepadStateExts.ConvertControllerFloatToByte(+0.00f));
            Assert.Equal(140, GamepadStateExts.ConvertControllerFloatToByte(+0.10f));
            Assert.Equal(146, GamepadStateExts.ConvertControllerFloatToByte(+0.15f));
            Assert.Equal(153, GamepadStateExts.ConvertControllerFloatToByte(+0.20f));
            Assert.Equal(159, GamepadStateExts.ConvertControllerFloatToByte(+0.25f));
            Assert.Equal(191, GamepadStateExts.ConvertControllerFloatToByte(+0.50f));
            Assert.Equal(223, GamepadStateExts.ConvertControllerFloatToByte(+0.75f));
            Assert.Equal(242, GamepadStateExts.ConvertControllerFloatToByte(+0.90f));
            Assert.Equal(248, GamepadStateExts.ConvertControllerFloatToByte(+0.95f));
            Assert.Equal(255, GamepadStateExts.ConvertControllerFloatToByte(+1.00f));
        }

        [Fact]
        public void TestConvertByteToControllerFloat()
        {
            Assert.Equal(-1.00f, GamepadStateExts.ConvertByteToControllerFloat(000), 0.007f);
            Assert.Equal(-0.95f, GamepadStateExts.ConvertByteToControllerFloat(006), 0.007f);
            Assert.Equal(-0.90f, GamepadStateExts.ConvertByteToControllerFloat(012), 0.007f);
            Assert.Equal(-0.75f, GamepadStateExts.ConvertByteToControllerFloat(031), 0.007f);
            Assert.Equal(-0.50f, GamepadStateExts.ConvertByteToControllerFloat(063), 0.007f);
            Assert.Equal(-0.25f, GamepadStateExts.ConvertByteToControllerFloat(095), 0.007f);
            Assert.Equal(-0.20f, GamepadStateExts.ConvertByteToControllerFloat(102), 0.007f);
            Assert.Equal(-0.15f, GamepadStateExts.ConvertByteToControllerFloat(108), 0.007f);
            Assert.Equal(-0.10f, GamepadStateExts.ConvertByteToControllerFloat(114), 0.007f);
            Assert.Equal(+0.00f, GamepadStateExts.ConvertByteToControllerFloat(127), 0.007f);
            Assert.Equal(+0.10f, GamepadStateExts.ConvertByteToControllerFloat(140), 0.007f);
            Assert.Equal(+0.15f, GamepadStateExts.ConvertByteToControllerFloat(146), 0.007f);
            Assert.Equal(+0.20f, GamepadStateExts.ConvertByteToControllerFloat(153), 0.007f);
            Assert.Equal(+0.25f, GamepadStateExts.ConvertByteToControllerFloat(159), 0.007f);
            Assert.Equal(+0.50f, GamepadStateExts.ConvertByteToControllerFloat(191), 0.007f);
            Assert.Equal(+0.75f, GamepadStateExts.ConvertByteToControllerFloat(223), 0.007f);
            Assert.Equal(+0.90f, GamepadStateExts.ConvertByteToControllerFloat(242), 0.007f);
            Assert.Equal(+0.95f, GamepadStateExts.ConvertByteToControllerFloat(248), 0.007f);
            Assert.Equal(+1.00f, GamepadStateExts.ConvertByteToControllerFloat(255), 0.007f);
        }
    }
}
