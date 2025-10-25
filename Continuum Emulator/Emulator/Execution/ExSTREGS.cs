using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public static class ExSTREGS
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 7);

            switch (upperLdOp)
            {
                case Mnem.LDSTREGS_r_r_IrrrI: // STREGS (rrr), r, r 
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        byte rAdr = (byte)(ldOp >> 2);
                        byte r1 = (byte)(((ldOp & 0b00000011) << 3) + (mixedReg >> 5));
                        byte r2 = (byte)(mixedReg & 0b00011111);
                        uint address = computer.CPU.REGS.Get24BitRegister(rAdr);

                        byte[] regs = computer.CPU.REGS.GetRegistersBetween(r1, r2);
                        computer.LoadMemAt(address, regs);

                        return;
                    }
                case Mnem.LDSTREGS_r_r_InnnI: // STREGS (nnn), r, r 
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        byte r1 = (byte)(ldOp & 0b00011111);
                        byte r2 = (byte)(mixedReg & 0b00011111);
                        uint address = computer.MEMC.Fetch24();

                        byte[] regs = computer.CPU.REGS.GetRegistersBetween(r1, r2);
                        computer.LoadMemAt(address, regs);

                        return;
                    }
            }
        }
    }
}
