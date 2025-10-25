using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public class ExSL
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 2);
            switch (upperLdOp)
            {
                case Mnem.SHRLSTRE_r_n:  // SL r, n
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte value = ((byte)(mixedReg & 0b00011111));

                        computer.CPU.REGS.Shift8BitRegisterLeft(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_r_r:  // SL r, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte valueIndex = ((byte)(mixedReg & 0b00011111));
                        byte value = computer.CPU.REGS.Get8BitRegister(valueIndex);

                        computer.CPU.REGS.Shift8BitRegisterLeft(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_rr_n:  // SL rr, n
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte value = ((byte)(mixedReg & 0b00011111));

                        computer.CPU.REGS.Shift16BitRegisterLeft(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_rr_r:  // SL rr, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte valueIndex = ((byte)(mixedReg & 0b00011111));
                        byte value = computer.CPU.REGS.Get8BitRegister(valueIndex);

                        computer.CPU.REGS.Shift16BitRegisterLeft(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_rrr_n:  // SL rrr, n
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte value = ((byte)(mixedReg & 0b00011111));

                        computer.CPU.REGS.Shift24BitRegisterLeft(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_rrr_r:  // SL rrr, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte valueIndex = ((byte)(mixedReg & 0b00011111));
                        byte value = computer.CPU.REGS.Get8BitRegister(valueIndex);

                        computer.CPU.REGS.Shift24BitRegisterLeft(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_rrrr_n:  // SL rrrr, n
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte value = ((byte)(mixedReg & 0b00011111));

                        computer.CPU.REGS.Shift32BitRegisterLeft(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_rrrr_r:  // SL rrrr, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte valueIndex = ((byte)(mixedReg & 0b00011111));
                        byte value = computer.CPU.REGS.Get8BitRegister(valueIndex);

                        computer.CPU.REGS.Shift32BitRegisterLeft(regIndex, value);

                        return;
                    }
            }
        }
    }
}
