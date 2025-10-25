namespace Continuum93.Emulator.Execution
{
    public static class ExRESF
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte flagIndex = (byte)(ldOp & 0b00011111);

            computer.CPU.FLAGS.SetValueByIndex(flagIndex, false);
        }
    }
}
