namespace Continuum93.Emulator.RAM
{
    public static class MemoryDebugController
    {
        public static byte[] GetRegisterStack(Computer computer, uint limit = 10)
        {
            uint regStackPointer = computer.CPU.REGS.SPR;
            if (limit >= regStackPointer)
                limit = regStackPointer;

            return computer.MEMC.RSRAM.GetMemoryAt(regStackPointer - limit, (int)limit);
        }

        public static uint[] GetCallStack(Computer computer, uint limit = 10)
        {
            uint callStackPointer = computer.CPU.REGS.SPC;

            if (limit >= callStackPointer)
                limit = callStackPointer;

            return computer.MEMC.CSRAM.GetMemoryAt(callStackPointer - limit, (int)limit);
        }
    }
}
