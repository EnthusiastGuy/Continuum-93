using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public class ExWAIT
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 6);

            switch (upperLdOp)
            {
                case Mnem.WAIT_n: // WAIT n
                    {
                        ushort time = computer.MEMC.Fetch16();
                        System.Threading.Thread.Sleep(time);

                        return;
                    }
            }
        }
    }
}
