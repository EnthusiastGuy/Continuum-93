namespace ContinuumTools.States
{
    public static class UserInput
    {
        // Menu
        public static int HoveringButtonIndex;


        // Disassembler
        public static bool HoveringDisassembler = false;
        public static bool HoveringAddress = false;
        public static bool HoveringInstruction = false;
        public static int HoveredDisassemblerLine;

        public static bool HoveringMemoryViewer = false;

        public static bool HoveringRegistryViewer = false;
        public static bool Hovering24bitRegister = false;
        public static bool HoveringRegAddressedData = false;
        public static int Hovered24BitRegisterLine;

        public static bool ShowAsciiRegReferencedData = false;

        public static bool HoveringStacksView = false;

        public static bool LockStepByStep = false;
    }
}
