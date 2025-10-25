using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public class ExSET
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 2);

            switch (upperLdOp)
            {
                case Mnem.SHRLSTRE_r_n: // SET r, n
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte value = ((byte)(mixedReg & 0b00011111));

                        computer.CPU.REGS.Set8BitBit(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_r_r:  // SET r, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte valueIndex = ((byte)(mixedReg & 0b00011111));
                        byte value = computer.CPU.REGS.Get8BitRegister(valueIndex);

                        computer.CPU.REGS.Set8BitBit(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_rr_n:  // SET rr, n
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte value = ((byte)(mixedReg & 0b00011111));

                        computer.CPU.REGS.Set16BitBit(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_rr_r:  // SET rr, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte valueIndex = ((byte)(mixedReg & 0b00011111));
                        byte value = computer.CPU.REGS.Get8BitRegister(valueIndex);

                        computer.CPU.REGS.Set16BitBit(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_rrr_n:  // SET rrr, n
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte value = ((byte)(mixedReg & 0b00011111));

                        computer.CPU.REGS.Set24BitBit(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_rrr_r:  // SET rrr, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte valueIndex = ((byte)(mixedReg & 0b00011111));
                        byte value = computer.CPU.REGS.Get8BitRegister(valueIndex);

                        computer.CPU.REGS.Set24BitBit(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_rrrr_n:  // SET rrrr, n
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte value = ((byte)(mixedReg & 0b00011111));

                        computer.CPU.REGS.Set32BitBit(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_rrrr_r:  // SET rrrr, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte valueIndex = ((byte)(mixedReg & 0b00011111));
                        byte value = computer.CPU.REGS.Get8BitRegister(valueIndex);

                        computer.CPU.REGS.Set32BitBit(regIndex, value);

                        return;
                    }
            }
        }
    }
}
