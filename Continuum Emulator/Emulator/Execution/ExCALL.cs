using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public class ExCALL
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 5);

            switch (upperLdOp)
            {
                case Mnem.CLRJ_nnn: // CALL nnn
                    {
                        uint address = computer.MEMC.Fetch24();
                        computer.MEMC.SetToCallStack(computer.CPU.REGS.SPC++, computer.CPU.REGS.IPO);
                        computer.CPU.REGS.IPO = address;

                        return;
                    }
                case Mnem.CLRJ_ff_nnn: // CALL ff, nnn
                    {
                        uint address = computer.MEMC.Fetch24();
                        byte flagIndex = (byte)(ldOp & 0b00011111);
                        if (computer.CPU.FLAGS.GetValueByIndex(flagIndex))
                        {
                            computer.MEMC.SetToCallStack(computer.CPU.REGS.SPC++, computer.CPU.REGS.IPO);
                            computer.CPU.REGS.IPO = address;
                        }

                        return;
                    }
                case Mnem.CLRJ_rrr: // CALL rrr
                    {
                        byte registerIndex = (byte)(ldOp & 0b00011111);
                        uint address = computer.CPU.REGS.Get24BitRegister(registerIndex);
                        computer.MEMC.SetToCallStack(computer.CPU.REGS.SPC++, computer.CPU.REGS.IPO);
                        computer.CPU.REGS.IPO = address;

                        return;
                    }
                case Mnem.CLRJ_ff_rrr:  // CALL ff, rrr
                    {
                        byte flagIndex = (byte)(ldOp & 0b00011111);
                        byte registerIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint address = computer.CPU.REGS.Get24BitRegister(registerIndex);
                        if (computer.CPU.FLAGS.GetValueByIndex(flagIndex))
                        {
                            computer.MEMC.SetToCallStack(computer.CPU.REGS.SPC++, computer.CPU.REGS.IPO);
                            computer.CPU.REGS.IPO = address;
                        }

                        return;
                    }

            }
        }
    }
}
