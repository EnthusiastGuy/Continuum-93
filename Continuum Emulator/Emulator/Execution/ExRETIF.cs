namespace Continuum93.Emulator.Execution
{
    public static class ExRETIF
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte flagIndex = (byte)(ldOp & 0b00001111);
            if (computer.CPU.FLAGS.GetValueByIndex(flagIndex))
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

        }
    }
}
