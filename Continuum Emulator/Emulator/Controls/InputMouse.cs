using Continuum93.Emulator.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Continuum93.Emulator.Controls
{
    public static class InputMouse
    {
        public static Point MouseXY { get; private set; }

        public static void Update()
        {
            MouseState mouseState = Mouse.GetState();
            MouseXY = new Point(mouseState.X, mouseState.Y);
        }

        public static Point GetMousePos()
        {
            return MouseXY;
        }

        public static Point GetRelativeMousePos()
        {
            if (SettingsManager.GetBoleanSettingsValue("disableMouse"))
                return new Point(0, 0);

            MouseState mouseState = Mouse.GetState();
            return new Point(mouseState.X, mouseState.Y);
        }

        public static byte GetMouseButtonsState()
        {
            if (SettingsManager.GetBoleanSettingsValue("disableMouse"))
                return 0;

            MouseState mouseState = Mouse.GetState();
            byte buttonsState = 0;

            if (mouseState.LeftButton == ButtonState.Pressed)
                buttonsState |= 0x01; // Left button is pressed (0001)

            if (mouseState.RightButton == ButtonState.Pressed)
                buttonsState |= 0x02; // Right button is pressed (0010)

            if (mouseState.MiddleButton == ButtonState.Pressed)
                buttonsState |= 0x04; // Middle button is pressed (0100)

            return buttonsState;
        }
    }
}
