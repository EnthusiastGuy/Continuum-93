using Continuum93.CodeAnalysis;
using Continuum93.Emulator;

namespace Continuum93.ServiceModule
{
    public static class Operations
    {
        public static string DisassembledCode;
        public static void Disassemble()
        {
            ContinuumDebugger.RunAt(0, Machine.COMPUTER);
            DisassembledCode = ContinuumDebugger.GetDissassembledWithAddresses();
        }
    }
}
