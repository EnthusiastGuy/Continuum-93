using Continuum_Emulator.Emulator.Controls;
using ContinuumTools.Network;
using ContinuumTools.States;
using Microsoft.Xna.Framework.Input;

namespace ContinuumTools.Input.Views
{
    public static class MemoryViewInput
    {
        public static void Update()
        {
            UserInput.HoveringMemoryViewer =
                InputMouse.GetCurrentMouseX() < 500 && InputMouse.GetCurrentMouseX() > 2 && InputMouse.GetCurrentMouseY() > 300 && InputMouse.GetCurrentMouseY() < 528;

            if (UserInput.HoveringMemoryViewer)
            {
                int multiplier = (InputKeyboard.KeyDown(Keys.LeftShift) || InputKeyboard.KeyPressed(Keys.LeftShift)) ? 10 : 1;
                int jumpDistance = multiplier * 16;

                if (InputMouse.WheelScrolledDown())
                {
                    if (Memory.Address <= (0xFFFFFF - (jumpDistance + 288)))  // We subtract page size (18 rows of 16 bytes)
                    {
                        Memory.Address += jumpDistance;
                        Protocol.GetMemory();
                    }
                    else if (Memory.Address != (0xFFFFFF - 288))
                    {
                        Memory.Address = 0xFFFFFF - 288;
                        Protocol.GetMemory();
                    }
                }
                else if (InputMouse.WheelScrolledUp())
                {
                    if (Memory.Address >= jumpDistance)
                    {
                        Memory.Address -= jumpDistance;
                        Protocol.GetMemory();
                    }
                    else if (Memory.Address != 0)
                    {
                        Memory.Address = 0;
                        Protocol.GetMemory();
                    }
                }
            }
            
        }
    }
}
