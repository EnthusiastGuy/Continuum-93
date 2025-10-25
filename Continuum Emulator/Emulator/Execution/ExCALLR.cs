using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public class ExCALLR
    {
        public static void Process(Computer computer)
        {
            int CALLR_InstructionSize = 5;
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 5);

            switch (upperLdOp)
            {
                case Mnem.CLRJ_nnn: // CALLR nnn
                    {
                        int addressOffset = computer.MEMC.Fetch24Signed();
                        computer.MEMC.SetToCallStack(computer.CPU.REGS.SPC++, computer.CPU.REGS.IPO);
                        computer.CPU.REGS.IPO = (uint)(computer.CPU.REGS.IPO + addressOffset - CALLR_InstructionSize);

                        return;
                    }
                case Mnem.CLRJ_ff_nnn: // CALLR ff, nnn
                    {
                        int addressOffset = computer.MEMC.Fetch24Signed();
                        byte flagIndex = (byte)(ldOp & 0b00011111);
                        if (computer.CPU.FLAGS.GetValueByIndex(flagIndex))
                        {
                            computer.MEMC.SetToCallStack(computer.CPU.REGS.SPC++, computer.CPU.REGS.IPO);
                            computer.CPU.REGS.IPO = (uint)(computer.CPU.REGS.IPO + addressOffset - CALLR_InstructionSize);
                        }

                        return;
                    }
            }
        }
    }
}
