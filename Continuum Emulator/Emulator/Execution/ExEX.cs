using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public class ExEX
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 2);

            switch (upperLdOp)
            {
                case Mnem.EX_r_r:   // EX r, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.Ex8BitRegisters(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),
                            (byte)(mixedReg & 0b00011111)
                        );

                        return;
                    }
                case Mnem.EX_rr_rr:   // EX rr, rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.Ex16BitRegisters(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),
                            (byte)(mixedReg & 0b00011111)
                        );

                        return;
                    }
                case Mnem.EX_rrr_rrr:   // EX rrr, rrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.Ex24BitRegisters(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),
                            (byte)(mixedReg & 0b00011111)
                        );

                        return;
                    }
                case Mnem.EX_rrrr_rrrr:   // EX rrrr, rrrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.Ex32BitRegisters(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),
                            (byte)(mixedReg & 0b00011111)
                        );

                        return;
                    }
                case Mnem.EX_fr_fr:   // EX fr, fr
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        computer.CPU.FREGS.ExchangeRegisters(
                            (byte)(mixedReg >> 4),
                            (byte)(mixedReg & 0b_00001111)
                        );

                        return;
                    }
            }
        }
    }
}
