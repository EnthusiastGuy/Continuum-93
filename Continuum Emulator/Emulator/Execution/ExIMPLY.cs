using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public static class ExIMPLY
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 2);

            switch (upperLdOp)
            {
                case Mnem.AOX_r_n: // IMPLY r, n
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte regValue = computer.MEMC.Fetch();
                        computer.CPU.REGS.Imply8Bit(register, regValue);

                        return;
                    }
                case Mnem.AOX_r_r:  // IMPLY r, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.Imply8Bit(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),   // r1   - destination
                            computer.CPU.REGS.Get8BitRegister(
                                (byte)(mixedReg & 0b00011111)                       // r2   - source
                                ));

                        return;
                    }
                case Mnem.AOX_rr_nn:    // IMPLY rr, nn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        ushort regValue = computer.MEMC.Fetch16();
                        computer.CPU.REGS.Imply16Bit(register, regValue);

                        return;
                    }
                case Mnem.AOX_rr_rr:  // IMPLY rr, rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.Imply16Bit(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),   // r1   - destination
                            computer.CPU.REGS.Get16BitRegister(
                                (byte)(mixedReg & 0b00011111)                       // r2   - source
                                ));

                        return;
                    }
                case Mnem.AOX_rrr_nnn:    // IMPLY rrr, nnn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regValue = computer.MEMC.Fetch24();
                        computer.CPU.REGS.Imply24Bit(register, regValue);

                        return;
                    }
                case Mnem.AOX_rrr_rrr:  // IMPLY rrr, rrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.Imply24Bit(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),   // r1   - destination
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(mixedReg & 0b00011111)                       // r2   - source
                                ));

                        return;
                    }
                case Mnem.AOX_rrrr_nnnn:    // IMPLY rrrr, nnnn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regValue = computer.MEMC.Fetch32();
                        computer.CPU.REGS.Imply32Bit(register, regValue);

                        return;
                    }
                case Mnem.AOX_rrrr_rrrr:  // IMPLY rrrr, rrrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.Imply32Bit(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),   // r1   - destination
                            computer.CPU.REGS.Get32BitRegister(
                                (byte)(mixedReg & 0b00011111)                       // r2   - source
                                ));

                        return;
                    }

                case Mnem.AOX_IrrrI_n: // IMPLY (rrr), n
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regAddress = computer.CPU.REGS.Get24BitRegister(register);
                        byte regValue = computer.MEMC.Fetch();
                        computer.CPU.REGS.Imply8BitMem(regAddress, regValue);

                        return;
                    }
                case Mnem.AOX16_IrrrI_nn:   // IMPLY (rrr), nn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regAddress = computer.CPU.REGS.Get24BitRegister(register);
                        ushort regValue = computer.MEMC.Fetch16();
                        computer.CPU.REGS.Imply16BitMem(regAddress, regValue);

                        return;
                    }
                case Mnem.AOX24_IrrrI_nnn:  // IMPLY (rrr), nnn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regAddress = computer.CPU.REGS.Get24BitRegister(register);
                        uint regValue = computer.MEMC.Fetch24();
                        computer.CPU.REGS.Imply24BitMem(regAddress, regValue);

                        return;
                    }
                case Mnem.AOX32_IrrrI_nnnn: // IMPLY (rrr), nnnn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regAddress = computer.CPU.REGS.Get24BitRegister(register);
                        uint regValue = computer.MEMC.Fetch32();
                        computer.CPU.REGS.Imply32BitMem(regAddress, regValue);

                        return;
                    }

                case Mnem.AOX_IrrrI_r: // IMPLY (rrr), r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.Imply8BitMem(
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5))
                                ),
                            computer.CPU.REGS.Get8BitRegister(
                                (byte)(mixedReg & 0b00011111)
                                )
                        );

                        return;
                    }
                case Mnem.AOX_IrrrI_rr: // IMPLY (rrr), rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.Imply16BitMem(
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5))
                                ),
                            computer.CPU.REGS.Get16BitRegister(
                                (byte)(mixedReg & 0b00011111)
                                )
                        );

                        return;
                    }
                case Mnem.AOX_IrrrI_rrr: // IMPLY (rrr), rrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.Imply24BitMem(
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5))
                                ),
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(mixedReg & 0b00011111)
                                )
                        );

                        return;
                    }
                case Mnem.AOX_IrrrI_rrrr: // IMPLY (rrr), rrrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.Imply32BitMem(
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5))
                                ),
                            computer.CPU.REGS.Get32BitRegister(
                                (byte)(mixedReg & 0b00011111)
                                )
                        );

                        return;
                    }
                // new
                case Mnem.AOX_r_IrrrI: // IMPLY r, (rrr)
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        computer.CPU.REGS.ImplyMemTo8BitReg(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(mixedReg & 0b00011111)
                        ));

                        return;
                    }
                case Mnem.AOX_rr_IrrrI: // IMPLY rr, (rrr)
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        computer.CPU.REGS.ImplyMemTo16BitReg(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(mixedReg & 0b00011111)
                        ));

                        return;
                    }
                case Mnem.AOX_rrr_IrrrI: // IMPLY rrr, (rrr)
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        computer.CPU.REGS.ImplyMemTo24BitReg(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(mixedReg & 0b00011111)
                        ));

                        return;
                    }
                case Mnem.AOX_rrrr_IrrrI: // IMPLY rrrr, (rrr)
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        computer.CPU.REGS.ImplyMemTo32BitReg(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(mixedReg & 0b00011111)
                        ));

                        return;
                    }
                case Mnem.AOX_InnnI_n: // IMPLY (nnn), n
                    {
                        uint address = computer.MEMC.Fetch24();
                        byte value = computer.MEMC.Fetch();

                        computer.CPU.REGS.Imply8BitMem(address, value);

                        return;
                    }
                case Mnem.AOX16_InnnI_nn: // IMPLY16 (nnn), nn
                    {
                        uint address = computer.MEMC.Fetch24();
                        ushort value = computer.MEMC.Fetch16();

                        computer.CPU.REGS.Imply16BitMem(address, value);

                        return;
                    }
                case Mnem.AOX24_InnnI_nnn: // IMPLY24 (nnn), nnn
                    {
                        uint address = computer.MEMC.Fetch24();
                        uint value = computer.MEMC.Fetch24();

                        computer.CPU.REGS.Imply24BitMem(address, value);

                        return;
                    }
                case Mnem.AOX32_InnnI_nnnn: // IMPLY32 (nnn), nnnn
                    {
                        uint address = computer.MEMC.Fetch24();
                        uint value = computer.MEMC.Fetch32();

                        computer.CPU.REGS.Imply32BitMem(address, value);

                        return;
                    }
                case Mnem.AOX_InnnI_r: // IMPLY (nnn), r
                    {
                        uint address = computer.MEMC.Fetch24();
                        byte value =
                            computer.CPU.REGS.Get8BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.CPU.REGS.Imply8BitMem(address, value);

                        return;
                    }
                case Mnem.AOX_InnnI_rr: // IMPLY (nnn), rr
                    {
                        uint address = computer.MEMC.Fetch24();
                        ushort value =
                            computer.CPU.REGS.Get16BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.CPU.REGS.Imply16BitMem(address, value);

                        return;
                    }
                case Mnem.AOX_InnnI_rrr: // IMPLY (nnn), rrr
                    {
                        uint address = computer.MEMC.Fetch24();
                        uint value =
                            computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.CPU.REGS.Imply24BitMem(address, value);

                        return;
                    }
                case Mnem.AOX_InnnI_rrrr: // IMPLY (nnn), rrrr
                    {
                        uint address = computer.MEMC.Fetch24();
                        uint value =
                            computer.CPU.REGS.Get32BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.CPU.REGS.Imply32BitMem(address, value);

                        return;
                    }
                case Mnem.AOX_r_InnnI: // IMPLY r, (nnn)
                    {
                        byte reg = computer.MEMC.Fetch();
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.ImplyMemTo8BitReg(reg, address);

                        return;
                    }
                case Mnem.AOX_rr_InnnI: // IMPLY rr, (nnn)
                    {
                        byte reg = computer.MEMC.Fetch();
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.ImplyMemTo16BitReg(reg, address);

                        return;
                    }
                case Mnem.AOX_rrr_InnnI: // IMPLY rrr, (nnn)
                    {
                        byte reg = computer.MEMC.Fetch();
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.ImplyMemTo24BitReg(reg, address);

                        return;
                    }
                case Mnem.AOX_rrrr_InnnI: // IMPLY rrrr, (nnn)
                    {
                        byte reg = computer.MEMC.Fetch();
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.ImplyMemTo32BitReg(reg, address);

                        return;
                    }
            }
        }
    }
}
