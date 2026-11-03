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

        private static ushort _lastMachineX, _lastMachineY;

        /// <summary>
        /// The mouse as the emulated machine sees it: position in machine pixels (480 x 270) and buttons.
        /// Normally the machine screen fills the window. In service mode it is shown inside the service UI's
        /// Emulator window: there the pointer is mapped into that viewport, and while it is elsewhere (over the
        /// debugger windows) the machine keeps the last position and sees no buttons, so clicks meant for the
        /// service UI do not leak into the running program.
        /// </summary>
        public static void ReadMachineMouse(out ushort x, out ushort y, out byte buttons)
        {
            Point p = GetRelativeMousePos();
            buttons = GetMouseButtonsState();

            if (ServiceModule.Service.STATE.ServiceMode)
            {
                var window = ServiceModule.ServiceGraphics.EmulatorWindow;
                Rectangle view = window != null && window.Visible
                    ? window.GetViewportRect((int)Constants.V_WIDTH, (int)Constants.V_HEIGHT)
                    : Rectangle.Empty;

                if (view.Width <= 0 || view.Height <= 0 || !view.Contains(p))
                {
                    x = _lastMachineX;
                    y = _lastMachineY;
                    buttons = 0;
                    return;
                }

                x = _lastMachineX = (ushort)((p.X - view.X) * (int)Constants.V_WIDTH / view.Width);
                y = _lastMachineY = (ushort)((p.Y - view.Y) * (int)Constants.V_HEIGHT / view.Height);
                return;
            }

            x = _lastMachineX = (ushort)(p.X / Window.WindowManager.GetClientXRatio());
            y = _lastMachineY = (ushort)(p.Y / Window.WindowManager.GetClientYRatio());
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
