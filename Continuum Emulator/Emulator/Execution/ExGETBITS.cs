using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public static class ExGETBITS
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 2);

            switch (upperLdOp)
            {
                case Mnem.GETBITS_r_rrrr_n:
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte targetRegIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte adrRegIndex = (byte)(mixedReg & 0b00011111);

                        uint adrReg = computer.CPU.REGS.Get32BitRegister(adrRegIndex);
                        byte bits = computer.MEMC.Fetch();

                        computer.CPU.REGS.Set8BitRegister(targetRegIndex, computer.MEMC.RAM.Get8BitValueFromBitMemoryAt(adrReg, bits));

                        return;
                    }
                case Mnem.GETBITS_rr_rrrr_n:
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte targetRegIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte adrRegIndex = (byte)(mixedReg & 0b00011111);

                        uint adrReg = computer.CPU.REGS.Get32BitRegister(adrRegIndex);
                        byte bits = computer.MEMC.Fetch();

                        computer.CPU.REGS.Set16BitRegister(targetRegIndex, computer.MEMC.RAM.Get16BitValueFromBitMemoryAt(adrReg, bits));

                        return;
                    }
                case Mnem.GETBITS_rrr_rrrr_n:
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte targetRegIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte adrRegIndex = (byte)(mixedReg & 0b00011111);

                        uint adrReg = computer.CPU.REGS.Get32BitRegister(adrRegIndex);
                        byte bits = computer.MEMC.Fetch();

                        computer.CPU.REGS.Set24BitRegister(targetRegIndex, computer.MEMC.RAM.Get24BitValueFromBitMemoryAt(adrReg, bits));

                        return;
                    }
                case Mnem.GETBITS_rrrr_rrrr_n:
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte targetRegIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte adrRegIndex = (byte)(mixedReg & 0b00011111);

                        uint adrReg = computer.CPU.REGS.Get32BitRegister(adrRegIndex);
                        byte bits = computer.MEMC.Fetch();

                        computer.CPU.REGS.Set32BitRegister(targetRegIndex, computer.MEMC.RAM.Get32BitValueFromBitMemoryAt(adrReg, bits));

                        return;
                    }
                case Mnem.GETBITS_r_rrrr_r:
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte targetRegIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte adrRegIndex = (byte)(mixedReg & 0b00011111);
                        byte bitsReg = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        uint adrReg = computer.CPU.REGS.Get32BitRegister(adrRegIndex);
                        byte bits = computer.CPU.REGS.Get8BitRegister(bitsReg);

                        computer.CPU.REGS.Set8BitRegister(targetRegIndex, computer.MEMC.RAM.Get8BitValueFromBitMemoryAt(adrReg, bits));

                        return;
                    }
                case Mnem.GETBITS_rr_rrrr_r:
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte targetRegIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte adrRegIndex = (byte)(mixedReg & 0b00011111);
                        byte bitsReg = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        uint adrReg = computer.CPU.REGS.Get32BitRegister(adrRegIndex);
                        byte bits = computer.CPU.REGS.Get8BitRegister(bitsReg);

                        computer.CPU.REGS.Set16BitRegister(targetRegIndex, computer.MEMC.RAM.Get16BitValueFromBitMemoryAt(adrReg, bits));

                        return;
                    }
                case Mnem.GETBITS_rrr_rrrr_r:
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte targetRegIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte adrRegIndex = (byte)(mixedReg & 0b00011111);
                        byte bitsReg = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        uint adrReg = computer.CPU.REGS.Get32BitRegister(adrRegIndex);
                        byte bits = computer.CPU.REGS.Get8BitRegister(bitsReg);

                        computer.CPU.REGS.Set24BitRegister(targetRegIndex, computer.MEMC.RAM.Get24BitValueFromBitMemoryAt(adrReg, bits));

                        return;
                    }
                case Mnem.GETBITS_rrrr_rrrr_r:
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte targetRegIndex = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte adrRegIndex = (byte)(mixedReg & 0b00011111);
                        byte bitsReg = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        uint adrReg = computer.CPU.REGS.Get32BitRegister(adrRegIndex);
                        byte bits = computer.CPU.REGS.Get8BitRegister(bitsReg);

                        computer.CPU.REGS.Set32BitRegister(targetRegIndex, computer.MEMC.RAM.Get32BitValueFromBitMemoryAt(adrReg, bits));

                        return;
                    }

            }
        }
    }
}
