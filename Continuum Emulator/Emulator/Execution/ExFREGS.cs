using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public static class ExFREGS
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 5);

            switch (upperLdOp)
            {
                case Mnem.FREGS_n: // FREGS n
                    {
                        byte bankIndex = computer.MEMC.Fetch();

                        computer.CPU.FREGS.SetRegisterBank(bankIndex);

                        return;
                    }
                case Mnem.FREGS_InnnI: // FREGS (nnn)
                    {
                        uint address = computer.MEMC.Fetch24();
                        byte bankIndex = computer.MEMC.Get8bitFromRAM(address);

                        computer.CPU.FREGS.SetRegisterBank(bankIndex);

                        return;
                    }
                case Mnem.FREGS_r: // FREGS r
                    {
                        byte bankIndex = computer.CPU.REGS.Get8BitRegister((byte)(ldOp & 0b00011111));

                        computer.CPU.FREGS.SetRegisterBank(bankIndex);

                        return;
                    }
                case Mnem.FREGS_IrrrI: // FREGS (rrr)
                    {
                        uint address = computer.CPU.REGS.Get24BitRegister((byte)(ldOp & 0b00011111));
                        byte bankIndex = computer.MEMC.Get8bitFromRAM(address);

                        computer.CPU.FREGS.SetRegisterBank(bankIndex);

                        return;
                    }

            }
        }
    }
}
