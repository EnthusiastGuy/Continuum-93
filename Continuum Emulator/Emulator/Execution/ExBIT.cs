using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public class ExBIT
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 2);

            switch (upperLdOp)
            {
                case Mnem.SHRLSTRE_r_n: // BIT r, n
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte value = ((byte)(mixedReg & 0b00011111));

                        computer.CPU.REGS.Test8BitBit(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_r_r:  // BIT r, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte valueIndex = ((byte)(mixedReg & 0b00011111));
                        byte value = computer.CPU.REGS.Get8BitRegister(valueIndex);

                        computer.CPU.REGS.Test8BitBit(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_rr_n:  // BIT rr, n
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte value = ((byte)(mixedReg & 0b00011111));

                        computer.CPU.REGS.Test16BitBit(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_rr_r:  // BIT rr, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte valueIndex = ((byte)(mixedReg & 0b00011111));
                        byte value = computer.CPU.REGS.Get8BitRegister(valueIndex);

                        computer.CPU.REGS.Test16BitBit(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_rrr_n:  // BIT rrr, n
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte value = ((byte)(mixedReg & 0b00011111));

                        computer.CPU.REGS.Test24BitBit(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_rrr_r:  // BIT rrr, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte valueIndex = ((byte)(mixedReg & 0b00011111));
                        byte value = computer.CPU.REGS.Get8BitRegister(valueIndex);

                        computer.CPU.REGS.Test24BitBit(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_rrrr_n:  // BIT rrrr, n
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte value = ((byte)(mixedReg & 0b00011111));

                        computer.CPU.REGS.Test32BitBit(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_rrrr_r:  // BIT rrrr, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte valueIndex = ((byte)(mixedReg & 0b00011111));
                        byte value = computer.CPU.REGS.Get8BitRegister(valueIndex);

                        computer.CPU.REGS.Test32BitBit(regIndex, value);

                        return;
                    }
            }
        }
    }
}
