using Continuum93.Emulator;

namespace Continuum93.Emulator.Execution
{
    public static class ExMisc
    {
        public static void ProcessRET(Computer computer)
        {
            if (computer.CPU.REGS.SPC > 0)
            {
                computer.CPU.REGS.IPO = computer.MEMC.GetFromCallStack(--computer.CPU.REGS.SPC);
            }
            else
            {
                // Stack underflow error handling (interrupt)
                computer.Stop();
            }
        }

        public static void ProcessBreak(Computer computer)
        {
            computer.Stop();
        }
    }
}
