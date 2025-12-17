using Continuum93.CodeAnalysis;
using Continuum93.Emulator;
using Continuum93.ServiceModule.Parsers;

namespace Continuum93.ServiceModule
{
    public static class Operations
    {
        public static string DisassembledCode;
        public static string DisassembledText;

        public static void Disassemble()
        {
            if (Machine.COMPUTER == null)
                return;

            ContinuumDebugger.RunAt(0, Machine.COMPUTER);
            DisassembledCode = ContinuumDebugger.GetDissassembledFull();
            Disassembled.SetResponse(DisassembledCode);
        }

        public static void UpdateAll()
        {
            CPUState.Update();
            Memory.Update();
            Stacks.Update();
            Video.Update();
        }
    }
}
