using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public class ExDEC
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();

            switch (ldOp)
            {
                case Mnem.INCDEC_r: // DEC r
                    {
                        computer.CPU.REGS.Decrement8Bit((byte)(computer.MEMC.Fetch() & 0b00011111));

                        return;
                    }
                case Mnem.INCDEC_rr: // DEC rr
                    {
                        computer.CPU.REGS.Decrement16Bit((byte)(computer.MEMC.Fetch() & 0b00011111));

                        return;
                    }
                case Mnem.INCDEC_rrr: // DEC rrr
                    {
                        computer.CPU.REGS.Decrement24Bit((byte)(computer.MEMC.Fetch() & 0b00011111));

                        return;
                    }
                case Mnem.INCDEC_rrrr: // DEC rrrr
                    {
                        computer.CPU.REGS.Decrement32Bit((byte)(computer.MEMC.Fetch() & 0b00011111));

                        return;
                    }
                case Mnem.INCDEC_IrrrI: // DEC (rrr)
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regValue = computer.CPU.REGS.Get24BitRegister(regIndex);
                        computer.CPU.REGS.Decrement8BitMem(regValue);
                        return;
                    }
                case Mnem.INCDEC16_IrrrI: // DEC16 (rrr)
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regValue = computer.CPU.REGS.Get24BitRegister(regIndex);
                        computer.CPU.REGS.Decrement16BitMem(regValue);
                        return;
                    }
                case Mnem.INCDEC24_IrrrI: // DEC24 (rrr)
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regValue = computer.CPU.REGS.Get24BitRegister(regIndex);
                        computer.CPU.REGS.Decrement24BitMem(regValue);
                        return;
                    }
                case Mnem.INCDEC32_IrrrI: // DEC32 (rrr)
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regValue = computer.CPU.REGS.Get24BitRegister(regIndex);
                        computer.CPU.REGS.Decrement32BitMem(regValue);
                        return;
                    }
                case Mnem.INCDEC_InnnI: // DEC (nnn)
                    {
                        uint address = computer.MEMC.Fetch24();
                        computer.CPU.REGS.Decrement8BitMem(address);
                        return;
                    }
                case Mnem.INCDEC16_InnnI: // DEC16 (nnn)
                    {
                        uint address = computer.MEMC.Fetch24();
                        computer.CPU.REGS.Decrement16BitMem(address);
                        return;
                    }
                case Mnem.INCDEC24_InnnI: // DEC24 (nnn)
                    {
                        uint address = computer.MEMC.Fetch24();
                        computer.CPU.REGS.Decrement24BitMem(address);
                        return;
                    }
                case Mnem.INCDEC32_InnnI: // DEC32 (nnn)
                    {
                        uint address = computer.MEMC.Fetch24();
                        computer.CPU.REGS.Decrement32BitMem(address);
                        return;
                    }
            }
        }
    }
}
