using Continuum93.Emulator.Interrupts;
using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public class ExINT
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 5);

            switch (upperLdOp)
            {
                case Mnem.INT_n_r: // INC n, r
                    {
                        //Log.WriteLine("Starting handling interrupt");
                        byte regIndex = (byte)(ldOp & 0b00011111);
                        byte number = computer.MEMC.Fetch();
                        InterruptHandler.HandleInterrupt(number, computer.CPU.REGS.Get8BitRegister(regIndex), regIndex, computer);
                        //Log.WriteLine("Finished handling interrupt");
                        return;
                    }

            }
        }
    }
}
