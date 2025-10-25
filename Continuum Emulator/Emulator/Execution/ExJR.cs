
using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public class ExJR
    {
        public static void Process(Computer computer)
        {
            int JR_InstructionSize = 5;
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 5);

            switch (upperLdOp)
            {
                case Mnem.CLRJ_nnn: // JR nnn
                    {
                        int addressOffset = computer.MEMC.Fetch24Signed();
                        computer.CPU.REGS.IPO = (uint)(computer.CPU.REGS.IPO + addressOffset - JR_InstructionSize);

                        return;
                    }
                case Mnem.CLRJ_ff_nnn: // JR ff, nnn
                    {
                        int addressOffset = computer.MEMC.Fetch24Signed();
                        byte flagIndex = (byte)(ldOp & 0b00011111);
                        if (computer.CPU.FLAGS.GetValueByIndex(flagIndex))
                            computer.CPU.REGS.IPO = (uint)(computer.CPU.REGS.IPO + addressOffset - JR_InstructionSize);

                        return;
                    }
            }
        }
    }
}
