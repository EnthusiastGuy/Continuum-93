using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public static class ExDJNZ
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();

            switch (ldOp)
            {
                case Mnem.DJNZ_r_nnn: // DJNZ r, nnn
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint address = computer.MEMC.Fetch24();
                        computer.CPU.REGS.Decrement8Bit(regIndex);
                        byte regValue = computer.CPU.REGS.Get8BitRegister(regIndex);
                        if (regValue != 0)
                        {
                            computer.CPU.REGS.IPO = address;
                        }
                        return;
                    }
                case Mnem.DJNZ_r_rrr: // DJNZ r, rrr
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte addrRegisterIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint address = computer.CPU.REGS.Get24BitRegister(addrRegisterIndex);
                        computer.CPU.REGS.Decrement8Bit(regIndex);
                        ushort regValue = computer.CPU.REGS.Get8BitRegister(regIndex);
                        if (regValue != 0)
                        {
                            computer.CPU.REGS.IPO = address;
                        }
                        return;
                    }
                case Mnem.DJNZ_rr_nnn:  // DJNZ rr, nnn
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint address = computer.MEMC.Fetch24();
                        computer.CPU.REGS.Decrement16Bit(regIndex);
                        ushort regValue = computer.CPU.REGS.Get16BitRegister(regIndex);
                        if (regValue != 0)
                        {
                            computer.CPU.REGS.IPO = address;
                        }
                        return;
                    }
                case Mnem.DJNZ_rr_rrr:  // DJNZ rr, rrr
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte addrRegisterIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint address = computer.CPU.REGS.Get24BitRegister(addrRegisterIndex);
                        computer.CPU.REGS.Decrement16Bit(regIndex);
                        ushort regValue = computer.CPU.REGS.Get16BitRegister(regIndex);
                        if (regValue != 0)
                        {
                            computer.CPU.REGS.IPO = address;
                        }
                        return;
                    }
                case Mnem.DJNZ_rrr_nnn: // DJNZ rrr, nnn
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint address = computer.MEMC.Fetch24();
                        computer.CPU.REGS.Decrement24Bit(regIndex);
                        uint regValue = computer.CPU.REGS.Get24BitRegister(regIndex);
                        if (regValue != 0)
                        {
                            computer.CPU.REGS.IPO = address;
                        }
                        return;
                    }
                case Mnem.DJNZ_rrr_rrr: // DJNZ rrr, rrr
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte addrRegisterIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint address = computer.CPU.REGS.Get24BitRegister(addrRegisterIndex);
                        computer.CPU.REGS.Decrement24Bit(regIndex);
                        uint regValue = computer.CPU.REGS.Get24BitRegister(regIndex);
                        if (regValue != 0)
                        {
                            computer.CPU.REGS.IPO = address;
                        }
                        return;
                    }
                case Mnem.DJNZ_rrrr_nnn:    // DJNZ rrrr, nnn
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint address = computer.MEMC.Fetch24();
                        computer.CPU.REGS.Decrement32Bit(regIndex);
                        uint regValue = computer.CPU.REGS.Get32BitRegister(regIndex);
                        if (regValue != 0)
                        {
                            computer.CPU.REGS.IPO = address;
                        }
                        return;
                    }
                case Mnem.DJNZ_rrrr_rrr:    // DJNZ rrrr, rrr
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte addrRegisterIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint address = computer.CPU.REGS.Get24BitRegister(addrRegisterIndex);
                        computer.CPU.REGS.Decrement32Bit(regIndex);
                        uint regValue = computer.CPU.REGS.Get32BitRegister(regIndex);
                        if (regValue != 0)
                        {
                            computer.CPU.REGS.IPO = address;
                        }
                        return;
                    }

                case Mnem.DJNZ_InnnI_nnn: // DJNZ (nnn), nnn
                    {
                        uint targetAddress = computer.MEMC.Fetch24();
                        uint jumpAddress = computer.MEMC.Fetch24();
                        computer.CPU.REGS.Decrement8BitMem(targetAddress);
                        byte value = computer.MEMC.Get8bitFromRAM(targetAddress);
                        if (value != 0)
                        {
                            computer.CPU.REGS.IPO = jumpAddress;
                        }
                        return;
                    }
                case Mnem.DJNZ_IrrrI_nnn: // DJNZ (rrr), nnn
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint targetAddress = computer.CPU.REGS.Get24BitRegister(regIndex);
                        uint jumpAddress = computer.MEMC.Fetch24();
                        computer.CPU.REGS.Decrement8BitMem(targetAddress);
                        byte value = computer.MEMC.Get8bitFromRAM(targetAddress);
                        if (value != 0)
                        {
                            computer.CPU.REGS.IPO = jumpAddress;
                        }
                        return;
                    }
                case Mnem.DJNZ16_InnnI_nnn: // DJNZ16 (nnn), nnn
                    {
                        uint targetAddress = computer.MEMC.Fetch24();
                        uint jumpAddress = computer.MEMC.Fetch24();
                        computer.CPU.REGS.Decrement16BitMem(targetAddress);
                        ushort value = computer.MEMC.Get16bitFromRAM(targetAddress);
                        if (value != 0)
                        {
                            computer.CPU.REGS.IPO = jumpAddress;
                        }
                        return;
                    }
                case Mnem.DJNZ16_IrrrI_nnn: // DJNZ16 (rrr), nnn
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint targetAddress = computer.CPU.REGS.Get24BitRegister(regIndex);
                        uint jumpAddress = computer.MEMC.Fetch24();
                        computer.CPU.REGS.Decrement16BitMem(targetAddress);
                        ushort value = computer.MEMC.Get16bitFromRAM(targetAddress);
                        if (value != 0)
                        {
                            computer.CPU.REGS.IPO = jumpAddress;
                        }
                        return;
                    }
                case Mnem.DJNZ24_InnnI_nnn: // DJNZ24 (nnn), nnn
                    {
                        uint targetAddress = computer.MEMC.Fetch24();
                        uint jumpAddress = computer.MEMC.Fetch24();
                        computer.CPU.REGS.Decrement24BitMem(targetAddress);
                        uint value = computer.MEMC.Get24bitFromRAM(targetAddress);
                        if (value != 0)
                        {
                            computer.CPU.REGS.IPO = jumpAddress;
                        }
                        return;
                    }
                case Mnem.DJNZ24_IrrrI_nnn: // DJNZ24 (rrr), nnn
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint targetAddress = computer.CPU.REGS.Get24BitRegister(regIndex);
                        uint jumpAddress = computer.MEMC.Fetch24();
                        computer.CPU.REGS.Decrement24BitMem(targetAddress);
                        uint value = computer.MEMC.Get24bitFromRAM(targetAddress);
                        if (value != 0)
                        {
                            computer.CPU.REGS.IPO = jumpAddress;
                        }
                        return;
                    }
                case Mnem.DJNZ32_InnnI_nnn: // DJNZ32 (nnn), nnn
                    {
                        uint targetAddress = computer.MEMC.Fetch24();
                        uint jumpAddress = computer.MEMC.Fetch24();
                        computer.CPU.REGS.Decrement32BitMem(targetAddress);
                        uint value = computer.MEMC.Get32bitFromRAM(targetAddress);
                        if (value != 0)
                        {
                            computer.CPU.REGS.IPO = jumpAddress;
                        }
                        return;
                    }
                case Mnem.DJNZ32_IrrrI_nnn: // DJNZ32 (rrr), nnn
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint targetAddress = computer.CPU.REGS.Get24BitRegister(regIndex);
                        uint jumpAddress = computer.MEMC.Fetch24();
                        computer.CPU.REGS.Decrement32BitMem(targetAddress);
                        uint value = computer.MEMC.Get32bitFromRAM(targetAddress);
                        if (value != 0)
                        {
                            computer.CPU.REGS.IPO = jumpAddress;
                        }
                        return;
                    }

                case Mnem.DJNZ_InnnI_rrr: // DJNZ (nnn), rrr
                    {
                        uint targetAddress = computer.MEMC.Fetch24();
                        byte addrRegisterIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint jumpAddress = computer.CPU.REGS.Get24BitRegister(addrRegisterIndex);
                        computer.CPU.REGS.Decrement8BitMem(targetAddress);
                        byte value = computer.MEMC.Get8bitFromRAM(targetAddress);
                        if (value != 0)
                        {
                            computer.CPU.REGS.IPO = jumpAddress;
                        }
                        return;
                    }
                case Mnem.DJNZ_IrrrI_rrr: // DJNZ (rrr), rrr
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint targetAddress = computer.CPU.REGS.Get24BitRegister(regIndex);
                        byte addrRegisterIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint jumpAddress = computer.CPU.REGS.Get24BitRegister(addrRegisterIndex);
                        computer.CPU.REGS.Decrement8BitMem(targetAddress);
                        byte value = computer.MEMC.Get8bitFromRAM(targetAddress);
                        if (value != 0)
                        {
                            computer.CPU.REGS.IPO = jumpAddress;
                        }
                        return;
                    }
                case Mnem.DJNZ16_InnnI_rrr: // DJNZ16 (nnn), rrr
                    {
                        uint targetAddress = computer.MEMC.Fetch24();
                        byte addrRegisterIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint jumpAddress = computer.CPU.REGS.Get24BitRegister(addrRegisterIndex);
                        computer.CPU.REGS.Decrement16BitMem(targetAddress);
                        ushort value = computer.MEMC.Get16bitFromRAM(targetAddress);
                        if (value != 0)
                        {
                            computer.CPU.REGS.IPO = jumpAddress;
                        }
                        return;
                    }
                case Mnem.DJNZ16_IrrrI_rrr: // DJNZ16 (rrr), rrr
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint targetAddress = computer.CPU.REGS.Get24BitRegister(regIndex);
                        byte addrRegisterIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint jumpAddress = computer.CPU.REGS.Get24BitRegister(addrRegisterIndex);
                        computer.CPU.REGS.Decrement16BitMem(targetAddress);
                        ushort value = computer.MEMC.Get16bitFromRAM(targetAddress);
                        if (value != 0)
                        {
                            computer.CPU.REGS.IPO = jumpAddress;
                        }
                        return;
                    }
                case Mnem.DJNZ24_InnnI_rrr: // DJNZ24 (nnn), rrr
                    {
                        uint targetAddress = computer.MEMC.Fetch24();
                        byte addrRegisterIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint jumpAddress = computer.CPU.REGS.Get24BitRegister(addrRegisterIndex);
                        computer.CPU.REGS.Decrement24BitMem(targetAddress);
                        uint value = computer.MEMC.Get24bitFromRAM(targetAddress);
                        if (value != 0)
                        {
                            computer.CPU.REGS.IPO = jumpAddress;
                        }
                        return;
                    }
                case Mnem.DJNZ24_IrrrI_rrr: // DJNZ24 (rrr), rrr
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint targetAddress = computer.CPU.REGS.Get24BitRegister(regIndex);
                        byte addrRegisterIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint jumpAddress = computer.CPU.REGS.Get24BitRegister(addrRegisterIndex);
                        computer.CPU.REGS.Decrement24BitMem(targetAddress);
                        uint value = computer.MEMC.Get24bitFromRAM(targetAddress);
                        if (value != 0)
                        {
                            computer.CPU.REGS.IPO = jumpAddress;
                        }
                        return;
                    }
                case Mnem.DJNZ32_InnnI_rrr: // DJNZ32 (nnn), rrr
                    {
                        uint targetAddress = computer.MEMC.Fetch24();
                        byte addrRegisterIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint jumpAddress = computer.CPU.REGS.Get24BitRegister(addrRegisterIndex);
                        computer.CPU.REGS.Decrement32BitMem(targetAddress);
                        uint value = computer.MEMC.Get32bitFromRAM(targetAddress);
                        if (value != 0)
                        {
                            computer.CPU.REGS.IPO = jumpAddress;
                        }
                        return;
                    }
                case Mnem.DJNZ32_IrrrI_rrr: // DJNZ32 (rrr), rrr
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint targetAddress = computer.CPU.REGS.Get24BitRegister(regIndex);
                        byte addrRegisterIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint jumpAddress = computer.CPU.REGS.Get24BitRegister(addrRegisterIndex);
                        computer.CPU.REGS.Decrement32BitMem(targetAddress);
                        uint value = computer.MEMC.Get32bitFromRAM(targetAddress);
                        if (value != 0)
                        {
                            computer.CPU.REGS.IPO = jumpAddress;
                        }
                        return;
                    }
            }
        }
    }
}
