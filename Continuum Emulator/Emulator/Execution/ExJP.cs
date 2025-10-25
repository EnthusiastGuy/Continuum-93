using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public class ExJP
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 5);

            switch (upperLdOp)
            {
                case Mnem.CLRJ_nnn: // JP nnn
                    {
                        uint address = computer.MEMC.Fetch24();
                        computer.CPU.REGS.IPO = address;

                        return;
                    }
                case Mnem.CLRJ_ff_nnn: // JP ff, nnn
                    {
                        uint address = computer.MEMC.Fetch24();
                        byte flagIndex = (byte)(ldOp & 0b00011111);
                        if (computer.CPU.FLAGS.GetValueByIndex(flagIndex))
                        {
                            computer.CPU.REGS.IPO = address;
                        }

                        return;
                    }
                case Mnem.CLRJ_rrr: // JP rrr
                    {
                        byte registerIndex = (byte)(ldOp & 0b00011111);
                        uint address = computer.CPU.REGS.Get24BitRegister(registerIndex);
                        computer.CPU.REGS.IPO = address;

                        return;
                    }
                case Mnem.CLRJ_ff_rrr:  // JP ff, rrr
                    {
                        byte flagIndex = (byte)(ldOp & 0b00011111);
                        byte registerIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint address = computer.CPU.REGS.Get24BitRegister(registerIndex);
                        if (computer.CPU.FLAGS.GetValueByIndex(flagIndex))
                        {
                            computer.CPU.REGS.IPO = address;
                        }

                        return;
                    }

            }
        }
    }
}
