using Continuum93.Emulator;
using Microsoft.Xna.Framework.Input;
using System;
using System.Text;

namespace Continuum93.Emulator.Controls
{
    public static class GamepadStateExts
    {
        private const byte GAMEPAD_COUNT = 4;
        private static readonly GamePadState[] oldGamepadState = new GamePadState[GAMEPAD_COUNT];
        private static readonly GamePadState[] gamepadState = new GamePadState[GAMEPAD_COUNT];
        private static readonly GamePadCapabilities[] gamePadCapabilities = new GamePadCapabilities[GAMEPAD_COUNT];

        public static void Update()
        {
            UpdateStates();
            UpdateNewGamePadCapabilities();
        }

        public static byte[] GetGamepadsState()
        {
            byte[] response = new byte[41];

            byte mainStatus = 0;

            if (gamepadState[0].IsConnected) mainStatus |= (1 << 0);
            if (gamepadState[1].IsConnected) mainStatus |= (1 << 1);
            if (gamepadState[2].IsConnected) mainStatus |= (1 << 2);
            if (gamepadState[3].IsConnected) mainStatus |= (1 << 3);

            for (byte i = 0; i < GAMEPAD_COUNT; i++)
            {
                if (!gamepadState[i].IsConnected)
                    continue;

                byte buttons1 = 0;
                byte buttons2 = 0;
                byte buttons3 = 0;
                byte buttons4 = 0;

                if (gamepadState[i].DPad.Up == ButtonState.Pressed) buttons1 |= (1 << 0);
                if (gamepadState[i].DPad.Down == ButtonState.Pressed) buttons1 |= (1 << 1);
                if (gamepadState[i].DPad.Left == ButtonState.Pressed) buttons1 |= (1 << 2);
                if (gamepadState[i].DPad.Right == ButtonState.Pressed) buttons1 |= (1 << 3);
                if (gamepadState[i].Buttons.A == ButtonState.Pressed) buttons1 |= (1 << 4);
                if (gamepadState[i].Buttons.B == ButtonState.Pressed) buttons1 |= (1 << 5);
                if (gamepadState[i].Buttons.X == ButtonState.Pressed) buttons1 |= (1 << 6);
                if (gamepadState[i].Buttons.Y == ButtonState.Pressed) buttons1 |= (1 << 7);

                if (gamepadState[i].ThumbSticks.Left.Y < 0) buttons2 |= (1 << 0);
                if (gamepadState[i].ThumbSticks.Left.Y > 0) buttons2 |= (1 << 1);
                if (gamepadState[i].ThumbSticks.Left.X < 0) buttons2 |= (1 << 2);
                if (gamepadState[i].ThumbSticks.Left.X > 0) buttons2 |= (1 << 3);
                if (gamepadState[i].ThumbSticks.Right.Y < 0) buttons2 |= (1 << 4);
                if (gamepadState[i].ThumbSticks.Right.Y > 0) buttons2 |= (1 << 5);
                if (gamepadState[i].ThumbSticks.Right.X < 0) buttons2 |= (1 << 6);
                if (gamepadState[i].ThumbSticks.Right.X > 0) buttons2 |= (1 << 7);

                if (gamepadState[i].Buttons.LeftShoulder == ButtonState.Pressed) buttons3 |= (1 << 0);  // LB
                if (gamepadState[i].Buttons.RightShoulder == ButtonState.Pressed) buttons3 |= (1 << 1); // RB
                if (gamepadState[i].Triggers.Left > 0) buttons3 |= (1 << 2);
                if (gamepadState[i].Triggers.Right > 0) buttons3 |= (1 << 3);
                if (gamepadState[i].Buttons.Start == ButtonState.Pressed) buttons3 |= (1 << 4);
                if (gamepadState[i].Buttons.Back == ButtonState.Pressed) buttons3 |= (1 << 5);
                if (gamepadState[i].Buttons.BigButton == ButtonState.Pressed) buttons3 |= (1 << 6);

                if (gamepadState[i].Buttons.LeftStick == ButtonState.Pressed) buttons4 |= (1 << 0);
                if (gamepadState[i].Buttons.RightStick == ButtonState.Pressed) buttons4 |= (1 << 1);

                if (buttons1 + buttons2 + buttons3 + buttons4 != 0) mainStatus |= (byte)(1 << (4 + i)); // Any button pressed?
                if (buttons1 + buttons2 + buttons3 + buttons4 == 0) buttons3 |= (1 << 7);  // No buttons pressed?

                if ((buttons1 & 0b_11110000) > 0) buttons4 |= (1 << 2);     // Any of A, B, X, Y
                if ((buttons1 & 0b_00001111) > 0) buttons4 |= (1 << 3);     // Any of DPad buttons
                if ((buttons3 & 0b_00000011) > 0) buttons4 |= (1 << 4);     // Any of LB or RB
                if ((buttons3 & 0b_00001100) > 0) buttons4 |= (1 << 5);     // Any of LT or RT
                if ((buttons2 & 0b_00001111) > 0) buttons4 |= (1 << 6);     // Any LeftThumb
                if ((buttons2 & 0b_11110000) > 0) buttons4 |= (1 << 7);     // Any RightThumb

                byte LeftTriggerZ = ConvertControllerFloatToBytePositive(gamepadState[i].Triggers.Left);
                byte RightTriggerZ = ConvertControllerFloatToBytePositive(gamepadState[i].Triggers.Right);
                byte leftThumbX = ConvertControllerFloatToByte(gamepadState[i].ThumbSticks.Left.X);
                byte leftThumbY = ConvertControllerFloatToByte(gamepadState[i].ThumbSticks.Left.Y);
                byte rightThumbX = ConvertControllerFloatToByte(gamepadState[i].ThumbSticks.Right.X);
                byte rightThumbY = ConvertControllerFloatToByte(gamepadState[i].ThumbSticks.Right.Y);

                response[1 + i * 10] = buttons1;
                response[2 + i * 10] = buttons2;
                response[3 + i * 10] = buttons3;
                response[4 + i * 10] = buttons4;
                response[5 + i * 10] = LeftTriggerZ;
                response[6 + i * 10] = RightTriggerZ;
                response[7 + i * 10] = leftThumbX;
                response[8 + i * 10] = leftThumbY;
                response[9 + i * 10] = rightThumbX;
                response[10 + i * 10] = rightThumbY;
            }

            response[0] = mainStatus;

            return response;
        }

        public static byte[] GetGamepadsCapabilities()
        {
            byte[] response = new byte[17];

            byte mainStatus = 0;

            if (gamePadCapabilities[0].IsConnected) mainStatus |= (1 << 0);
            if (gamePadCapabilities[1].IsConnected) mainStatus |= (1 << 1);
            if (gamePadCapabilities[2].IsConnected) mainStatus |= (1 << 2);
            if (gamePadCapabilities[3].IsConnected) mainStatus |= (1 << 3);

            for (byte i = 0; i < GAMEPAD_COUNT; i++)
            {
                byte status1 = 0;
                byte status2 = 0;
                byte status3 = 0;

                if (gamePadCapabilities[i].HasDPadUpButton) status1 |= (1 << 0);
                if (gamePadCapabilities[i].HasDPadDownButton) status1 |= (1 << 1);
                if (gamePadCapabilities[i].HasDPadLeftButton) status1 |= (1 << 2);
                if (gamePadCapabilities[i].HasDPadRightButton) status1 |= (1 << 3);
                if (gamePadCapabilities[i].HasAButton) status1 |= (1 << 4);
                if (gamePadCapabilities[i].HasBButton) status1 |= (1 << 5);
                if (gamePadCapabilities[i].HasXButton) status1 |= (1 << 6);
                if (gamePadCapabilities[i].HasYButton) status1 |= (1 << 7);

                if (gamePadCapabilities[i].HasLeftXThumbStick) status2 |= (1 << 0);
                if (gamePadCapabilities[i].HasLeftYThumbStick) status2 |= (1 << 1);
                if (gamePadCapabilities[i].HasRightXThumbStick) status2 |= (1 << 2);
                if (gamePadCapabilities[i].HasRightYThumbStick) status2 |= (1 << 3);
                if (gamePadCapabilities[i].HasStartButton) status2 |= (1 << 4);
                if (gamePadCapabilities[i].HasBackButton) status2 |= (1 << 5);
                if (gamePadCapabilities[i].HasBigButton) status2 |= (1 << 6);
                if (gamePadCapabilities[i].HasVoiceSupport) status2 |= (1 << 7);

                if (gamePadCapabilities[i].HasLeftShoulderButton) status3 |= (1 << 0);
                if (gamePadCapabilities[i].HasRightShoulderButton) status3 |= (1 << 1);
                if (gamePadCapabilities[i].HasLeftTrigger) status3 |= (1 << 2);
                if (gamePadCapabilities[i].HasRightTrigger) status3 |= (1 << 3);
                if (gamePadCapabilities[i].HasLeftStickButton) status3 |= (1 << 4);
                if (gamePadCapabilities[i].HasRightStickButton) status3 |= (1 << 5);
                if (gamePadCapabilities[i].HasLeftVibrationMotor) status3 |= (1 << 6);
                if (gamePadCapabilities[i].HasRightVibrationMotor) status3 |= (1 << 7);

                int gamePadType = (int)gamePadCapabilities[i].GamePadType;

                byte status4 = gamePadType == (int)GamePadType.BigButtonPad ? (byte)128 : (byte)gamePadType;
                response[1 + i * 4] = status1;
                response[2 + i * 4] = status2;
                response[3 + i * 4] = status3;
                response[4 + i * 4] = status4;
            }

            response[0] = mainStatus;

            return response;
        }

        public static byte[] GetGamepadDisplayNames()
        {
            string[] names = new string[4];

            for (byte i = 0; i < GAMEPAD_COUNT; i++)
                names[i] = gamePadCapabilities[i].IsConnected ? gamePadCapabilities[i].DisplayName : string.Empty;

            return GetGamepadDisplayNames(names);
        }

        public static byte[] GetGamepadDisplayNames(string[] names)
        {
            StringBuilder nameBuilder = new();
            int maxSize = 252;

            int totalLength = 0;
            foreach (string name in names)
                totalLength += name.Length;

            if (totalLength > 252)
            {
                for (byte i = 0; i < names.Length; i++)
                {
                    string cutName = names[i][..(int)MathF.Floor((names[i].Length * maxSize) / totalLength)];
                    nameBuilder.Append(cutName + '\0');
                }
            }
            else
            {
                for (byte i = 0; i < names.Length; i++)
                {
                    nameBuilder.Append(names[i] + '\0');
                }
            }

            byte[] nameBytes = Encoding.ASCII.GetBytes(nameBuilder.ToString());
            return nameBytes;
        }

        // For unit tests
        public static void SetOldState(byte index, GamePadState state)
        {
            oldGamepadState[index] = state;
        }

        // For unit tests
        public static void SetNewState(byte index, GamePadState state)
        {
            gamepadState[index] = state;
        }

        private static void UpdateStates()
        {
            for (byte i = 0; i < GAMEPAD_COUNT; i++)
            {
                oldGamepadState[i] = gamepadState[i];
                gamepadState[i] = GamePad.GetState(i);
            }
        }

        private static void UpdateNewGamePadCapabilities()
        {
            try
            {
                for (byte i = 0; i < GAMEPAD_COUNT; i++)
                {

                    if (gamepadState[i].IsConnected && !oldGamepadState[i].IsConnected)
                    {
                        // Raspberry Pi OS on Rpi4 fails here with null pointer exception
                        gamePadCapabilities[i] = GamePad.GetCapabilities(i);
                    }

                }
            }
            catch (Exception ex)
            {
                Log.WriteLine($"Update new gamepad capabilities was unable to run: {ex.Message}");
            }
        }

        public static byte ConvertControllerFloatToByte(float value)
        {
            return (byte)((value + 1.0f) * 127.5f);
        }

        public static byte ConvertControllerFloatToBytePositive(float value)
        {
            return (byte)(value * 255f);
        }

        public static float ConvertByteToControllerFloat(byte value)
        {
            return (value / 127.5f) - 1.0f;
        }

        public static float ConvertPositiveByteToControllerFloat(byte value)
        {
            return value / 255f;
        }
    }
}
