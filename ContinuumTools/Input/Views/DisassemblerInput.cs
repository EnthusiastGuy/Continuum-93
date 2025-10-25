using ContinuumTools.Display.Views;
using ContinuumTools.Network;
using ContinuumTools.States;

namespace ContinuumTools.Input.Views
{
    public static class DisassemblerInput
    {
        public static void Update()
        {
            UserInput.HoveringDisassembler =
                InputMouse.GetCurrentMouseX() < 500 && InputMouse.GetCurrentMouseX() > 2 && InputMouse.GetCurrentMouseY() > 27 && InputMouse.GetCurrentMouseY() < 275;

            UserInput.HoveringAddress =
                InputMouse.GetCurrentMouseX() < 80 && InputMouse.GetCurrentMouseX() > 2;

            UserInput.HoveringInstruction =
                InputMouse.GetCurrentMouseX() < 500 && InputMouse.GetCurrentMouseX() > Constants.REGS_LEFT_PADDING + 300;

            if (UserInput.HoveringDisassembler)
                UserInput.HoveredDisassemblerLine = (InputMouse.GetCurrentMouseY() - 32) / 12;

            if (InputMouse.LeftMouseDown() && UserInput.HoveringDisassembler)
            {
                if (Disassembled.Lines.Count <= UserInput.HoveredDisassemblerLine)
                    return;

                DissLine line = Disassembled.Lines[UserInput.HoveredDisassemblerLine];

                if (UserInput.HoveringAddress)
                {
                    // Set address as next memory viewer target
                    Memory.Address = line.Address;
                    Protocol.GetMemory();
                } else if (UserInput.HoveringInstruction)
                {
                    int instructionAddress = line.GetInstructionAddress();
                    if (instructionAddress >= 0)
                    {
                        Memory.Address = instructionAddress;
                        Protocol.GetMemory();
                    }
                }
            }
        }
    }
}
