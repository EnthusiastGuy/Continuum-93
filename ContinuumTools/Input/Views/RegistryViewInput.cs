using ContinuumTools.Network;
using ContinuumTools.States;

namespace ContinuumTools.Input.Views
{
    public static class RegistryViewInput
    {
        public static void Update()
        {
            UserInput.HoveringRegistryViewer =
                InputMouse.GetCurrentMouseX() < 950 && InputMouse.GetCurrentMouseX() > 550 && InputMouse.GetCurrentMouseY() > 27 && InputMouse.GetCurrentMouseY() < 345;

            UserInput.Hovering24bitRegister =
                InputMouse.GetCurrentMouseX() < 825 && InputMouse.GetCurrentMouseX() > 745;

            UserInput.HoveringRegAddressedData =
                InputMouse.GetCurrentMouseX() > 825 && InputMouse.GetCurrentMouseX() <= 945;

            if (UserInput.HoveringRegistryViewer)
                UserInput.Hovered24BitRegisterLine = (InputMouse.GetCurrentMouseY() - 32) / 12;

            if (InputMouse.LeftMouseDown() && UserInput.HoveringRegistryViewer)
            {
                if (UserInput.Hovering24bitRegister)
                {
                    // Set address as next memory viewer target
                    Memory.Address = (int)CPUState.Get24BitRegisterValue((byte)UserInput.Hovered24BitRegisterLine);
                    Protocol.GetMemory();
                }

                if (UserInput.HoveringRegAddressedData)
                {
                    UserInput.ShowAsciiRegReferencedData = !UserInput.ShowAsciiRegReferencedData;
                }
            }

            
        }
    }
}
