using Continuum93.Emulator.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Input
{

    public class TestGamePadStateExt
    {
        // Test A, B, X, Y buttons state
        public static IEnumerable<object[]> ButtonTestData =>
        new List<object[]>
        {
            new object[] { Buttons.A, 0b_00010000 },
            new object[] { Buttons.B, 0b_00100000 },
            new object[] { Buttons.X, 0b_01000000 },
            new object[] { Buttons.Y, 0b_10000000 },
        };

        [Theory]
        [MemberData(nameof(ButtonTestData))]
        public void TestGetGamepadsStateButtons(Buttons button, byte expectedResult)
        {
            GamePadState mockState = ConstructGamePadStateWithButtons(button);
            for (byte i = 0; i < 4; i++)
            {
                GamepadStateExts.SetNewState(i, mockState);
                byte[] byteState = GamepadStateExts.GetGamepadsState();
                Assert.Equal(expectedResult, byteState[1 + i * 10]);
                Assert.Equal(0b_00000000, byteState[3 + i * 10] & 0b_100000000);    // Test none is false
                Assert.Equal((1 << (4 + i)), byteState[0] & (1 << (4 + i)));    // Test any is true
            }
        }

        // Test DPad up/down/left/right buttons state
        public static IEnumerable<object[]> DPadTestData =>
        new List<object[]>
        {
            new object[] { ButtonState.Pressed, ButtonState.Released, ButtonState.Released, ButtonState.Released, 0b_00000001 },
            new object[] { ButtonState.Released, ButtonState.Pressed, ButtonState.Released, ButtonState.Released, 0b_00000010 },
            new object[] { ButtonState.Released, ButtonState.Released, ButtonState.Pressed, ButtonState.Released, 0b_00000100 },
            new object[] { ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Pressed, 0b_00001000 },

            new object[] { ButtonState.Pressed, ButtonState.Pressed, ButtonState.Released, ButtonState.Released, 0b_00000011 },
            new object[] { ButtonState.Released, ButtonState.Pressed, ButtonState.Pressed, ButtonState.Released, 0b_00000110 },
            new object[] { ButtonState.Released, ButtonState.Released, ButtonState.Pressed, ButtonState.Pressed, 0b_00001100 },
            new object[] { ButtonState.Pressed, ButtonState.Released, ButtonState.Released, ButtonState.Pressed, 0b_00001001 },
        };

        [Theory]
        [MemberData(nameof(DPadTestData))]
        public void TestGetGamepadsStateDPad(ButtonState upValue, ButtonState downValue, ButtonState leftValue, ButtonState rightValue, byte expectedResult)
        {
            GamePadState mockState = ConstructGamePadStateWithDPad(upValue, downValue, leftValue, rightValue);
            for (byte i = 0; i < 4; i++)
            {
                GamepadStateExts.SetNewState(i, mockState);
                byte[] byteState = GamepadStateExts.GetGamepadsState();
                Assert.Equal(expectedResult, byteState[1 + i * 10]);
                Assert.Equal(0b_00000000, byteState[3 + i * 10] & 0b_100000000);    // Test none is false
                Assert.Equal((1 << (4 + i)), byteState[0] & (1 << (4 + i)));    // Test any is true
            }
        }

        // Test Left/Right thumbs up/down/left/right buttons state
        public static IEnumerable<object[]> LRThumbsTestData =>
        new List<object[]>
        {
            new object[] { new Vector2(0, -0.5f), new Vector2(0, 0), 0b_00000001 },
            new object[] { new Vector2(0, 0.5f), new Vector2(0, 0), 0b_00000010 },
            new object[] { new Vector2(-0.5f, 0), new Vector2(0, 0), 0b_00000100 },
            new object[] { new Vector2(0.5f, 0), new Vector2(0, 0), 0b_00001000 },

            new object[] { new Vector2(0, 0), new Vector2(0, -0.5f), 0b_00010000 },
            new object[] { new Vector2(0, 0), new Vector2(0, 0.5f), 0b_00100000 },
            new object[] { new Vector2(0, 0), new Vector2(-0.5f, 0), 0b_01000000 },
            new object[] { new Vector2(0, 0), new Vector2(0.5f, 0), 0b_10000000 },

            new object[] { new Vector2(-0.5f, -0.5f), new Vector2(0, -0.5f), 0b_00010101 },
            new object[] { new Vector2(-0.5f, 0.5f), new Vector2(0, 0.5f), 0b_00100110 },
            new object[] { new Vector2(-0.5f, 0), new Vector2(-0.5f, 0), 0b_01000100 },
            new object[] { new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0), 0b_10001010 },
        };

        [Theory]
        [MemberData(nameof(LRThumbsTestData))]
        public void TestGetGamepadsStateLRThumbs(Vector2 leftPosition, Vector2 rightPosition, byte expectedResult)
        {
            GamePadState mockState = ConstructGamePadStateWithLeftRightThumbs(leftPosition, rightPosition);
            for (byte i = 0; i < 4; i++)
            {
                GamepadStateExts.SetNewState(i, mockState);
                byte[] byteState = GamepadStateExts.GetGamepadsState();
                Assert.Equal(expectedResult, byteState[2 + i * 10]);
                Assert.Equal(0b_00000000, byteState[3 + i * 10] & 0b_100000000);    // Test none is false
                Assert.Equal((1 << (4 + i)), byteState[0] & (1 << (4 + i)));    // Test any is true
            }
        }

        // Test Left/Right shoulder buttons state
        public static IEnumerable<object[]> ShoulderButtonTestData =>
        new List<object[]>
        {
            new object[] { Buttons.LeftShoulder, 0b_00000001 },
            new object[] { Buttons.RightShoulder, 0b_00000010 },
            new object[] { Buttons.LeftShoulder | Buttons.RightShoulder, 0b_00000011 },
        };

        [Theory]
        [MemberData(nameof(ShoulderButtonTestData))]
        public void TestGetGamepadsStateShoulderButtons(Buttons button, byte expectedResult)
        {
            GamePadState mockState = ConstructGamePadStateWithButtons(button);
            for (byte i = 0; i < 4; i++)
            {
                GamepadStateExts.SetNewState(i, mockState);
                byte[] byteState = GamepadStateExts.GetGamepadsState();
                Assert.Equal(expectedResult, byteState[3 + i * 10]);
                Assert.Equal(0b_00000000, byteState[3 + i * 10] & 0b_100000000);    // Test none is false
                Assert.Equal((1 << (4 + i)), byteState[0] & (1 << (4 + i)));    // Test any is true
            }
        }

        // Test Left/Right Trigger buttons state
        public static IEnumerable<object[]> TriggersButtonTestData =>
        new List<object[]>
        {
            new object[] { 0.5f, 0, 0b_00000100 },
            new object[] { 0, 0.5f, 0b_00001000 },
            new object[] { 0.5f, 0.5f, 0b_00001100 },
        };

        [Theory]
        [MemberData(nameof(TriggersButtonTestData))]
        public void TestGetGamepadsStateTriggerButtons(float lT, float rT, byte expectedResult)
        {
            GamePadState mockState = ConstructGamePadStateWithLeftRightTriggers(lT, rT);
            for (byte i = 0; i < 4; i++)
            {
                GamepadStateExts.SetNewState(i, mockState);
                byte[] byteState = GamepadStateExts.GetGamepadsState();
                Assert.Equal(expectedResult, byteState[3 + i * 10]);
                Assert.Equal(0b_00000000, byteState[3 + i * 10] & 0b_100000000);    // Test none is false
                Assert.Equal((1 << (4 + i)), byteState[0] & (1 << (4 + i)));    // Test any is true
            }
        }

        // Test Start, Back, BigButton buttons state
        public static IEnumerable<object[]> MiscButtonTestData =>
        new List<object[]>
        {
            new object[] { Buttons.Start, 0b_00010000 },
            new object[] { Buttons.Back, 0b_00100000 },
            new object[] { Buttons.BigButton, 0b_01000000 },
        };

        [Theory]
        [MemberData(nameof(MiscButtonTestData))]
        public void TestGetGamepadsStateMiscButtons(Buttons button, byte expectedResult)
        {
            GamePadState mockState = ConstructGamePadStateWithButtons(button);
            for (byte i = 0; i < 4; i++)
            {
                GamepadStateExts.SetNewState(i, mockState);
                byte[] byteState = GamepadStateExts.GetGamepadsState();
                Assert.Equal(expectedResult, byteState[3 + i * 10]);
                Assert.Equal(0b_00000000, byteState[3 + i * 10] & 0b_100000000);    // Test none is false
                Assert.Equal((1 << (4 + i)), byteState[0] & (1 << (4 + i)));    // Test any is true
            }
        }

        // Test Left/Right Thumbstick buttons state
        public static IEnumerable<object[]> LRStickButtonTestData =>
        new List<object[]>
        {
            new object[] { Buttons.LeftStick, 0b_00000001 },
            new object[] { Buttons.RightStick, 0b_00000010 },
            new object[] { Buttons.LeftStick | Buttons.RightStick, 0b_00000011 },
        };

        [Theory]
        [MemberData(nameof(LRStickButtonTestData))]
        public void TestGetGamepadsStateLRStickButtons(Buttons button, byte expectedResult)
        {
            GamePadState mockState = ConstructGamePadStateWithButtons(button);
            for (byte i = 0; i < 4; i++)
            {
                GamepadStateExts.SetNewState(i, mockState);
                byte[] byteState = GamepadStateExts.GetGamepadsState();
                Assert.Equal(expectedResult, byteState[4 + i * 10]);
                Assert.Equal(0b_00000000, byteState[3 + i * 10] & 0b_100000000);    // Test none is false
                Assert.Equal((1 << (4 + i)), byteState[0] & (1 << (4 + i)));    // Test any is true
            }
        }

        // Test any of A, B, X, Y buttons state
        public static IEnumerable<object[]> AnyOfABXYButtonTestData =>
        new List<object[]>
        {
            new object[] { Buttons.A, 0b_00000100 },
            new object[] { Buttons.B, 0b_00000100 },
            new object[] { Buttons.X, 0b_00000100 },
            new object[] { Buttons.Y, 0b_00000100 },
        };

        [Theory]
        [MemberData(nameof(AnyOfABXYButtonTestData))]
        public void TestGetGamepadsStateAnyOfABXYButtons(Buttons button, byte expectedResult)
        {
            GamePadState mockState = ConstructGamePadStateWithButtons(button);
            for (byte i = 0; i < 4; i++)
            {
                GamepadStateExts.SetNewState(i, mockState);
                byte[] byteState = GamepadStateExts.GetGamepadsState();
                Assert.Equal(expectedResult, byteState[4 + i * 10]);
            }
        }

        // Test any of DPad buttons state
        public static IEnumerable<object[]> AnyOfDPadButtonTestData =>
        new List<object[]>
        {
            new object[] { ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released, 0b_00000000 },

            new object[] { ButtonState.Pressed, ButtonState.Released, ButtonState.Released, ButtonState.Released, 0b_00001000 },
            new object[] { ButtonState.Released, ButtonState.Pressed, ButtonState.Released, ButtonState.Released, 0b_00001000 },
            new object[] { ButtonState.Released, ButtonState.Released, ButtonState.Pressed, ButtonState.Released, 0b_00001000 },
            new object[] { ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Pressed, 0b_00001000 },

            new object[] { ButtonState.Pressed, ButtonState.Pressed, ButtonState.Released, ButtonState.Released, 0b_00001000 },
            new object[] { ButtonState.Released, ButtonState.Pressed, ButtonState.Pressed, ButtonState.Released, 0b_00001000 },
            new object[] { ButtonState.Released, ButtonState.Released, ButtonState.Pressed, ButtonState.Pressed, 0b_00001000 },
            new object[] { ButtonState.Pressed, ButtonState.Released, ButtonState.Released, ButtonState.Pressed, 0b_00001000 },
        };

        [Theory]
        [MemberData(nameof(AnyOfDPadButtonTestData))]
        public void TestGetGamepadsStateAnyOfDPadButtons(ButtonState upValue, ButtonState downValue, ButtonState leftValue, ButtonState rightValue, byte expectedResult)
        {
            GamePadState mockState = ConstructGamePadStateWithDPad(upValue, downValue, leftValue, rightValue);
            for (byte i = 0; i < 4; i++)
            {
                GamepadStateExts.SetNewState(i, mockState);
                byte[] byteState = GamepadStateExts.GetGamepadsState();
                Assert.Equal(expectedResult, byteState[4 + i * 10]);
            }
        }

        // Test any of Left/Right shoulder buttons state
        public static IEnumerable<object[]> AnyOfShoulderButtonTestData =>
        new List<object[]>
        {
            new object[] { Buttons.None, 0b_00000000 },
            new object[] { Buttons.LeftShoulder, 0b_00010000 },
            new object[] { Buttons.RightShoulder, 0b_00010000 },
            new object[] { Buttons.LeftShoulder | Buttons.RightShoulder, 0b_00010000 },
        };

        [Theory]
        [MemberData(nameof(AnyOfShoulderButtonTestData))]
        public void TestGetGamepadsStateAnyShoulderButtons(Buttons button, byte expectedResult)
        {
            GamePadState mockState = ConstructGamePadStateWithButtons(button);
            for (byte i = 0; i < 4; i++)
            {
                GamepadStateExts.SetNewState(i, mockState);
                byte[] byteState = GamepadStateExts.GetGamepadsState();
                Assert.Equal(expectedResult, byteState[4 + i * 10]);
            }
        }

        // Test any Left/Right Trigger buttons state
        public static IEnumerable<object[]> AnyTriggersButtonTestData =>
        new List<object[]>
        {
            new object[] { 0.5f, 0, 0b_00100000 },
            new object[] { 0, 0.5f, 0b_00100000 },
            new object[] { 0.5f, 0.5f, 0b_00100000 },
        };

        [Theory]
        [MemberData(nameof(AnyTriggersButtonTestData))]
        public void TestGetGamepadsStateAnyTriggerButtons(float lT, float rT, byte expectedResult)
        {
            GamePadState mockState = ConstructGamePadStateWithLeftRightTriggers(lT, rT);
            for (byte i = 0; i < 4; i++)
            {
                GamepadStateExts.SetNewState(i, mockState);
                byte[] byteState = GamepadStateExts.GetGamepadsState();
                Assert.Equal(expectedResult, byteState[4 + i * 10]);
            }
        }

        // Test any Left/Right thumbs up/down/left/right buttons state
        public static IEnumerable<object[]> AnyLRThumbsTestData =>
        new List<object[]>
        {
            new object[] { new Vector2(0, -0.5f), new Vector2(0, 0), 0b_01000000 },
            new object[] { new Vector2(0, 0.5f), new Vector2(0, 0), 0b_01000000 },
            new object[] { new Vector2(-0.5f, 0), new Vector2(0, 0), 0b_01000000 },
            new object[] { new Vector2(0.5f, 0), new Vector2(0, 0), 0b_01000000 },
            new object[] { new Vector2(0, 0), new Vector2(0, -0.5f), 0b_10000000 },
            new object[] { new Vector2(0, 0), new Vector2(0, 0.5f), 0b_10000000 },
            new object[] { new Vector2(0, 0), new Vector2(-0.5f, 0), 0b_10000000 },
            new object[] { new Vector2(0, 0), new Vector2(0.5f, 0), 0b_10000000 },
            new object[] { new Vector2(-0.5f, -0.5f), new Vector2(0, -0.5f), 0b_11000000 },
            new object[] { new Vector2(-0.5f, 0.5f), new Vector2(0, 0.5f), 0b_11000000 },
            new object[] { new Vector2(-0.5f, 0), new Vector2(-0.5f, 0), 0b_11000000 },
            new object[] { new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0), 0b_11000000 },
        };

        [Theory]
        [MemberData(nameof(AnyLRThumbsTestData))]
        public void TestGetGamepadsStateAnyLRThumbs(Vector2 leftPosition, Vector2 rightPosition, byte expectedResult)
        {
            GamePadState mockState = ConstructGamePadStateWithLeftRightThumbs(leftPosition, rightPosition);
            for (byte i = 0; i < 4; i++)
            {
                GamepadStateExts.SetNewState(i, mockState);
                byte[] byteState = GamepadStateExts.GetGamepadsState();
                Assert.Equal(expectedResult, byteState[4 + i * 10]);
            }
        }



        // Test "no buttons pressed" flag
        [Fact]
        public void TestGetGamepadsStateNoButtonsPressed()
        {
            GamePadState mockState = ConstructGamePadEmptyState();
            for (byte i = 0; i < 4; i++)
            {
                GamepadStateExts.SetNewState(i, mockState);
                byte[] byteState = GamepadStateExts.GetGamepadsState();
                Assert.Equal(0b_00000000, byteState[3 + i * 10] & 0b_100000000);
            }
        }


        // Test any Left/Right Trigger buttons values
        public static IEnumerable<object[]> TriggersButtonTestValues =>
        new List<object[]>
        {
            new object[] { 0.00f, 0.25f, 0, 63 },
            new object[] { 0.50f, 0.70f, 127, 178 },
            new object[] { 0.80f, 1.00f, 204, 255 },
        };

        [Theory]
        [MemberData(nameof(TriggersButtonTestValues))]
        public void TestGetGamepadsTriggerButtonsValues(float lT, float rT, byte expectedLeftValue, byte expectedRightValue)
        {
            GamePadState mockState = ConstructGamePadStateWithLeftRightTriggers(lT, rT);
            for (byte i = 0; i < 4; i++)
            {
                GamepadStateExts.SetNewState(i, mockState);
                byte[] byteState = GamepadStateExts.GetGamepadsState();
                Assert.Equal(expectedLeftValue, byteState[5 + i * 10]);
                Assert.Equal(expectedRightValue, byteState[6 + i * 10]);
            }
        }

        // Test any Left/Right Trigger buttons values
        public static IEnumerable<object[]> LeftThumbButtonTestValues =>
        new List<object[]>
        {
            new object[] { new Vector2(-1.0f, -0.75f), new Vector2(-0.5f, -0.25f), 0, 31, 63, 95 },
            new object[] { new Vector2( 0.0f,  0.25f), new Vector2( 0.5f,  0.75f), 127, 159, 191, 223 },
            new object[] { new Vector2( 1.0f,  0.1f), new Vector2( 0.333f,  0.666f), 255, 140, 169, 212 },
        };

        [Theory]
        [MemberData(nameof(LeftThumbButtonTestValues))]
        public void TestGetGamepadsLeftThumbButtonsValues(Vector2 leftPosition, Vector2 rightPosition, byte expectedLeftXValue, byte expectedLeftYValue, byte expectedRightXValue, byte expectedRightYValue)
        {
            GamePadState mockState = ConstructGamePadStateWithLeftRightThumbs(leftPosition, rightPosition);
            for (byte i = 0; i < 4; i++)
            {
                GamepadStateExts.SetNewState(i, mockState);
                byte[] byteState = GamepadStateExts.GetGamepadsState();
                Assert.Equal(expectedLeftXValue, byteState[7 + i * 10]);
                Assert.Equal(expectedLeftYValue, byteState[8 + i * 10]);
                Assert.Equal(expectedRightXValue, byteState[9 + i * 10]);
                Assert.Equal(expectedRightYValue, byteState[10 + i * 10]);
            }
        }




        // Builders
        private static GamePadState ConstructGamePadStateWithButtons(params Buttons[] buttons)
        {
            Buttons sButtons = 0;
            foreach (Buttons button in buttons)
                sButtons |= button;

            GamePadButtons stateButtons = new(sButtons);
            return new(new GamePadThumbSticks(), new GamePadTriggers(), stateButtons, new GamePadDPad());
        }

        private static GamePadState ConstructGamePadStateWithDPad(ButtonState upValue, ButtonState downValue, ButtonState leftValue, ButtonState rightValue)
        {
            GamePadDPad dPadButtons = new(upValue, downValue, leftValue, rightValue);
            return new(new GamePadThumbSticks(), new GamePadTriggers(), new GamePadButtons(), dPadButtons);
        }

        private static GamePadState ConstructGamePadStateWithLeftRightThumbs(Vector2 leftPosition, Vector2 rightPosition)
        {
            return new(new GamePadThumbSticks(leftPosition, rightPosition), new GamePadTriggers(), new GamePadButtons(), new GamePadDPad());
        }

        private static GamePadState ConstructGamePadStateWithLeftRightTriggers(float leftTrigger, float rightTrigger)
        {
            return new(new GamePadThumbSticks(), new GamePadTriggers(leftTrigger, rightTrigger), new GamePadButtons(), new GamePadDPad());
        }

        private static GamePadState ConstructGamePadEmptyState()
        {
            return new(new GamePadThumbSticks(), new GamePadTriggers(), new GamePadButtons(), new GamePadDPad());
        }
    }
}
