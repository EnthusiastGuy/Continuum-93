using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public class ExRR
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 2);
            switch (upperLdOp)
            {
                case Mnem.SHRLSTRE_r_n:  // RR r, n
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte value = ((byte)(mixedReg & 0b00011111));

                        computer.CPU.REGS.Roll8BitRegisterRight(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_r_r:  // RR r, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte valueIndex = ((byte)(mixedReg & 0b00011111));
                        byte value = computer.CPU.REGS.Get8BitRegister(valueIndex);

                        computer.CPU.REGS.Roll8BitRegisterRight(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_rr_n:  // RR rr, n
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte value = ((byte)(mixedReg & 0b00011111));

                        computer.CPU.REGS.Roll16BitRegisterRight(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_rr_r:  // RR rr, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte valueIndex = ((byte)(mixedReg & 0b00011111));
                        byte value = computer.CPU.REGS.Get8BitRegister(valueIndex);

                        computer.CPU.REGS.Roll16BitRegisterRight(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_rrr_n:  // RR rrr, n
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte value = ((byte)(mixedReg & 0b00011111));

                        computer.CPU.REGS.Roll24BitRegisterRight(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_rrr_r:  // RR rrr, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte valueIndex = ((byte)(mixedReg & 0b00011111));
                        byte value = computer.CPU.REGS.Get8BitRegister(valueIndex);

                        computer.CPU.REGS.Roll24BitRegisterRight(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_rrrr_n:  // RR rrrr, n
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte value = ((byte)(mixedReg & 0b00011111));

                        computer.CPU.REGS.Roll32BitRegisterRight(regIndex, value);

                        return;
                    }
                case Mnem.SHRLSTRE_rrrr_r:  // RR rrrr, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte regIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte valueIndex = ((byte)(mixedReg & 0b00011111));
                        byte value = computer.CPU.REGS.Get8BitRegister(valueIndex);

                        computer.CPU.REGS.Roll32BitRegisterRight(regIndex, value);

                        return;
                    }
            }
        }
    }
}
