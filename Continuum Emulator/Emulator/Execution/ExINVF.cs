namespace Continuum93.Emulator.Execution
{
    public class ExINVF
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte flagIndex = (byte)(ldOp & 0b00011111);

            computer.CPU.FLAGS.InvertValueByIndex(flagIndex);
        }
    }
}
