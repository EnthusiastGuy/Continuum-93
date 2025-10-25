using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public class ExINC
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();

            switch (ldOp)
            {
                case Mnem.INCDEC_r: // INC r
                    {
                        computer.CPU.REGS.Increment8Bit((byte)(computer.MEMC.Fetch() & 0b00011111));

                        return;
                    }
                case Mnem.INCDEC_rr: // INC rr
                    {
                        computer.CPU.REGS.Increment16Bit((byte)(computer.MEMC.Fetch() & 0b00011111));

                        return;
                    }
                case Mnem.INCDEC_rrr: // INC rrr
                    {
                        computer.CPU.REGS.Increment24Bit((byte)(computer.MEMC.Fetch() & 0b00011111));

                        return;
                    }
                case Mnem.INCDEC_rrrr: // INC rrrr
                    {
                        computer.CPU.REGS.Increment32Bit((byte)(computer.MEMC.Fetch() & 0b00011111));

                        return;
                    }
                case Mnem.INCDEC_IrrrI: // INC (rrr)
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regValue = computer.CPU.REGS.Get24BitRegister(regIndex);
                        computer.CPU.REGS.Increment8BitMem(regValue);
                        return;
                    }
                case Mnem.INCDEC16_IrrrI: // INC16 (rrr)
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regValue = computer.CPU.REGS.Get24BitRegister(regIndex);
                        computer.CPU.REGS.Increment16BitMem(regValue);
                        return;
                    }
                case Mnem.INCDEC24_IrrrI: // INC24 (rrr)
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regValue = computer.CPU.REGS.Get24BitRegister(regIndex);
                        computer.CPU.REGS.Increment24BitMem(regValue);
                        return;
                    }
                case Mnem.INCDEC32_IrrrI: // INC32 (rrr)
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regValue = computer.CPU.REGS.Get24BitRegister(regIndex);
                        computer.CPU.REGS.Increment32BitMem(regValue);
                        return;
                    }
                case Mnem.INCDEC_InnnI: // INC (nnn)
                    {
                        uint address = computer.MEMC.Fetch24();
                        computer.CPU.REGS.Increment8BitMem(address);
                        return;
                    }
                case Mnem.INCDEC16_InnnI: // INC16 (nnn)
                    {
                        uint address = computer.MEMC.Fetch24();
                        computer.CPU.REGS.Increment16BitMem(address);
                        return;
                    }
                case Mnem.INCDEC24_InnnI: // INC24 (nnn)
                    {
                        uint address = computer.MEMC.Fetch24();
                        computer.CPU.REGS.Increment24BitMem(address);
                        return;
                    }
                case Mnem.INCDEC32_InnnI: // INC32 (nnn)
                    {
                        uint address = computer.MEMC.Fetch24();
                        computer.CPU.REGS.Increment32BitMem(address);
                        return;
                    }
            }
        }
    }
}
