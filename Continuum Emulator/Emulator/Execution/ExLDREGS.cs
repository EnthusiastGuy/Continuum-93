using Continuum93.Emulator.Mnemonics;
using System;

namespace Continuum93.Emulator.Execution
{
    public static class ExLDREGS
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 7);

            switch (upperLdOp)
            {
                case Mnem.LDSTREGS_r_r_IrrrI: // LFREGS r, r, (rrr)
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte r1 = (byte)(ldOp >> 2);
                        byte r2 = (byte)(((ldOp & 0b00000011) << 3) + (mixedReg >> 5));
                        byte rAdr = (byte)(mixedReg & 0b00011111);
                        uint address = computer.CPU.REGS.Get24BitRegister(rAdr);
                        byte len = (byte)(Math.Abs(r1 - r2) + 1);

                        computer.CPU.REGS.SetRegistersBetween(r1, r2, computer.MEMC.GetMemoryWrapped(address, len));

                        return;
                    }
                case Mnem.LDSTREGS_r_r_InnnI: // LFREGS r, r, (nnn)
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte r1 = (byte)(ldOp & 0b00011111);
                        byte r2 = (byte)(mixedReg & 0b00011111);
                        uint address = computer.MEMC.Fetch24();

                        byte len = (byte)(Math.Abs(r1 - r2) + 1);
                        computer.CPU.REGS.SetRegistersBetween(r1, r2, computer.MEMC.GetMemoryWrapped(address, len));

                        return;
                    }
            }

        }
    }
}
