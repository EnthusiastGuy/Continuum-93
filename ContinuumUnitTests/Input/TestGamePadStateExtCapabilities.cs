using Continuum93.Emulator.Controls;
using Microsoft.Xna.Framework.Input;
using System.Text;

namespace Input
{

    public class TestGamePadStateExtCapabilities
    {
        [Fact]
        public void TestGetGamepadDisplayNamesNormal()
        {
            string[] names = { "Name1", "Name2", "Name3", "Name4" };
            byte[] expected = Encoding.ASCII.GetBytes("Name1\0Name2\0Name3\0Name4\0");
            byte[] result = GamepadStateExts.GetGamepadDisplayNames(names);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestGetGamepadDisplayNamesLong()
        {
            string[] names = {
                "A very long name for a controller, one that should ideally never exist since it will take far too much space in the RAM memory and would also require the developer to do all kind of stunts to see it.",
                "This is a shorter name, but still rather lengthy and annoying.",
                "This is a shorter name, but still rather lengthy and annoying.",
                "This is a shorter name, but still rather lengthy and annoying."
            };

            string expected =
                "A very long name for a controller, one that should ideally never exist since it will take far too much space in the RAM memory and|" +
                "This is a shorter name, but still rather|" +
                "This is a shorter name, but still rather|" +
                "This is a shorter name, but still rather";

            byte[] result = GamepadStateExts.GetGamepadDisplayNames(names);
            string stringResult = ConvertNullTerminatedByteArrayToString(result);

            Assert.Equal(expected, stringResult);
        }


        private static GamePadCapabilities ConstructGamePadEmptyState()
        {
            return new();
        }

        private static string ConvertNullTerminatedByteArrayToString(byte[] byteArray)
        {
            List<string> strings = new();
            int startIndex = 0;

            for (int i = 0; i < byteArray.Length; i++)
            {
                if (byteArray[i] == 0)
                {
                    string str = Encoding.ASCII.GetString(byteArray, startIndex, i - startIndex);
                    strings.Add(str);

                    startIndex = i + 1;
                }
            }

            return string.Join("|", strings);
        }
    }
}
