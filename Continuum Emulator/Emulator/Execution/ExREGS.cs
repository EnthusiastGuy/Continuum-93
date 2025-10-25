using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public class ExREGS
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 5);

            switch (upperLdOp)
            {
                case Mnem.REGS_n: // REGS n
                    {
                        byte bankIndex = computer.MEMC.Fetch();

                        computer.CPU.REGS.SetRegisterBank(bankIndex);

                        return;
                    }
                case Mnem.REGS_InnnI: // REGS (nnn)
                    {
                        uint address = computer.MEMC.Fetch24();
                        byte bankIndex = computer.MEMC.Get8bitFromRAM(address);

                        computer.CPU.REGS.SetRegisterBank(bankIndex);

                        return;
                    }
                case Mnem.REGS_r: // REGS r
                    {
                        byte bankIndex = computer.CPU.REGS.Get8BitRegister((byte)(ldOp & 0b00011111));

                        computer.CPU.REGS.SetRegisterBank(bankIndex);

                        return;
                    }
                case Mnem.REGS_IrrrI: // REGS (rrr)
                    {
                        uint address = computer.CPU.REGS.Get24BitRegister((byte)(ldOp & 0b00011111));
                        byte bankIndex = computer.MEMC.Get8bitFromRAM(address);

                        computer.CPU.REGS.SetRegisterBank(bankIndex);

                        return;
                    }

            }
        }
    }
}
