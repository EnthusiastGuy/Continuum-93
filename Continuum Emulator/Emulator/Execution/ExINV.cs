using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public class ExINV
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 5);

            switch (upperLdOp)
            {
                case Mnem.INV_r:
                    {
                        byte reg = (byte)(ldOp & 0b00011111);
                        computer.CPU.REGS.Inv8BitRegister(reg);

                        return;
                    }
                case Mnem.INV_rr:
                    {
                        byte reg = (byte)(ldOp & 0b00011111);
                        computer.CPU.REGS.Inv16BitRegister(reg);

                        return;
                    }
                case Mnem.INV_rrr:
                    {
                        byte reg = (byte)(ldOp & 0b00011111);
                        computer.CPU.REGS.Inv24BitRegister(reg);

                        return;
                    }
                case Mnem.INV_rrrr:
                    {
                        byte reg = (byte)(ldOp & 0b00011111);
                        computer.CPU.REGS.Inv32BitRegister(reg);

                        return;
                    }
            }
        }
    }
}
