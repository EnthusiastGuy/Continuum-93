using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public class ExRL
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 2);
            switch (upperLdOp)
            {
                case Mnem.SHRLSTRE_r_n:  // RL r, n
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte value = ((byte)(mixedReg & 0b00011111));

                        computer.CPU.REGS.Roll8BitRegisterLeft(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_r_r:  // RL r, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte valueIndex = ((byte)(mixedReg & 0b00011111));
                        byte value = computer.CPU.REGS.Get8BitRegister(valueIndex);

                        computer.CPU.REGS.Roll8BitRegisterLeft(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_rr_n:  // RL rr, n
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte value = ((byte)(mixedReg & 0b00011111));

                        computer.CPU.REGS.Roll16BitRegisterLeft(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_rr_r:  // RL rr, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte valueIndex = ((byte)(mixedReg & 0b00011111));
                        byte value = computer.CPU.REGS.Get8BitRegister(valueIndex);

                        computer.CPU.REGS.Roll16BitRegisterLeft(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_rrr_n:  // RL rrr, n
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte value = ((byte)(mixedReg & 0b00011111));

                        computer.CPU.REGS.Roll24BitRegisterLeft(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_rrr_r:  // RL rrr, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte valueIndex = ((byte)(mixedReg & 0b00011111));
                        byte value = computer.CPU.REGS.Get8BitRegister(valueIndex);

                        computer.CPU.REGS.Roll24BitRegisterLeft(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_rrrr_n:  // RL rrrr, n
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte value = ((byte)(mixedReg & 0b00011111));

                        computer.CPU.REGS.Roll32BitRegisterLeft(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_rrrr_r:  // RL rrrr, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte valueIndex = ((byte)(mixedReg & 0b00011111));
                        byte value = computer.CPU.REGS.Get8BitRegister(valueIndex);

                        computer.CPU.REGS.Roll32BitRegisterLeft(regIndex, value);

                        return;
                    }
            }
        }
    }
}
