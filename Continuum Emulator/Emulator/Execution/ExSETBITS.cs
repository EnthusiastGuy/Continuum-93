using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public static class ExSETBITS
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 2);

            switch (upperLdOp)
            {
                case Mnem.SETBITS_rrrr_r_n:
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte adrRegIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte sourceRegIndex = (byte)(mixedReg & 0b00011111);

                        uint adrReg = computer.CPU.REGS.Get32BitRegister(adrRegIndex);
                        byte value = computer.CPU.REGS.Get8BitRegister(sourceRegIndex);
                        byte bits = computer.MEMC.Fetch();

                        computer.MEMC.RAM.Set8BitValueToBitMemoryAt(value, adrReg, bits);

                        return;
                    }
                case Mnem.SETBITS_rrrr_rr_n:
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte adrRegIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte sourceRegIndex = (byte)(mixedReg & 0b00011111);

                        uint adrReg = computer.CPU.REGS.Get32BitRegister(adrRegIndex);
                        ushort value = computer.CPU.REGS.Get16BitRegister(sourceRegIndex);
                        byte bits = computer.MEMC.Fetch();

                        computer.MEMC.RAM.Set16BitValueToBitMemoryAt(value, adrReg, bits);

                        return;
                    }
                case Mnem.SETBITS_rrrr_rrr_n:
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte adrRegIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte sourceRegIndex = (byte)(mixedReg & 0b00011111);

                        uint adrReg = computer.CPU.REGS.Get32BitRegister(adrRegIndex);
                        uint value = computer.CPU.REGS.Get24BitRegister(sourceRegIndex);
                        byte bits = computer.MEMC.Fetch();

                        computer.MEMC.RAM.Set24BitValueToBitMemoryAt(value, adrReg, bits);

                        return;
                    }
                case Mnem.SETBITS_rrrr_rrrr_n:
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte adrRegIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte sourceRegIndex = (byte)(mixedReg & 0b00011111);

                        uint adrReg = computer.CPU.REGS.Get32BitRegister(adrRegIndex);
                        uint value = computer.CPU.REGS.Get32BitRegister(sourceRegIndex);
                        byte bits = computer.MEMC.Fetch();

                        computer.MEMC.RAM.Set32BitValueToBitMemoryAt(value, adrReg, bits);

                        return;
                    }
                case Mnem.SETBITS_rrrr_r_r:
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte adrRegIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte sourceRegIndex = (byte)(mixedReg & 0b00011111);
                        byte bitsReg = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        
                        uint adrReg = computer.CPU.REGS.Get32BitRegister(adrRegIndex);
                        byte value = computer.CPU.REGS.Get8BitRegister(sourceRegIndex);
                        byte bits = computer.CPU.REGS.Get8BitRegister(bitsReg);

                        computer.MEMC.RAM.Set8BitValueToBitMemoryAt(value, adrReg, bits);

                        return;
                    }
                case Mnem.SETBITS_rrrr_rr_r:
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte adrRegIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte sourceRegIndex = (byte)(mixedReg & 0b00011111);
                        byte bitsReg = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        uint adrReg = computer.CPU.REGS.Get32BitRegister(adrRegIndex);
                        ushort value = computer.CPU.REGS.Get16BitRegister(sourceRegIndex);
                        byte bits = computer.CPU.REGS.Get8BitRegister(bitsReg);

                        computer.MEMC.RAM.Set16BitValueToBitMemoryAt(value, adrReg, bits);

                        return;
                    }
                case Mnem.SETBITS_rrrr_rrr_r:
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte adrRegIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte sourceRegIndex = (byte)(mixedReg & 0b00011111);
                        byte bitsReg = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        uint adrReg = computer.CPU.REGS.Get32BitRegister(adrRegIndex);
                        uint value = computer.CPU.REGS.Get24BitRegister(sourceRegIndex);
                        byte bits = computer.CPU.REGS.Get8BitRegister(bitsReg);

                        computer.MEMC.RAM.Set24BitValueToBitMemoryAt(value, adrReg, bits);

                        return;
                    }
                case Mnem.SETBITS_rrrr_rrrr_r:
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte adrRegIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte sourceRegIndex = (byte)(mixedReg & 0b00011111);
                        byte bitsReg = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        uint adrReg = computer.CPU.REGS.Get32BitRegister(adrRegIndex);
                        uint value = computer.CPU.REGS.Get32BitRegister(sourceRegIndex);
                        byte bits = computer.CPU.REGS.Get8BitRegister(bitsReg);

                        computer.MEMC.RAM.Set32BitValueToBitMemoryAt(value, adrReg, bits);

                        return;
                    }
            }
        }
    }
}
