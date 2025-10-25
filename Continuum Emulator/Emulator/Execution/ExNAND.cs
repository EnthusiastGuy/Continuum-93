using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public static class ExNAND
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 2);

            switch (upperLdOp)
            {
                case Mnem.AOX_r_n: // NAND r, n
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte regValue = computer.MEMC.Fetch();
                        computer.CPU.REGS.NAnd8Bit(register, regValue);

                        return;
                    }
                case Mnem.AOX_r_r:  // NAND r, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.NAnd8Bit(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),   // r1   - destination
                            computer.CPU.REGS.Get8BitRegister(
                                (byte)(mixedReg & 0b00011111)                       // r2   - source
                                ));

                        return;
                    }
                case Mnem.AOX_rr_nn:    // NAND rr, nn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        ushort regValue = computer.MEMC.Fetch16();
                        computer.CPU.REGS.NAnd16Bit(register, regValue);

                        return;
                    }
                case Mnem.AOX_rr_rr:  // NAND rr, rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.NAnd16Bit(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),   // r1   - destination
                            computer.CPU.REGS.Get16BitRegister(
                                (byte)(mixedReg & 0b00011111)                       // r2   - source
                                ));

                        return;
                    }
                case Mnem.AOX_rrr_nnn:    // NAND rrr, nnn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regValue = computer.MEMC.Fetch24();
                        computer.CPU.REGS.NAnd24Bit(register, regValue);

                        return;
                    }
                case Mnem.AOX_rrr_rrr:  // NAND rrr, rrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.NAnd24Bit(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),   // r1   - destination
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(mixedReg & 0b00011111)                       // r2   - source
                                ));

                        return;
                    }
                case Mnem.AOX_rrrr_nnnn:    // NAND rrrr, nnnn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regValue = computer.MEMC.Fetch32();
                        computer.CPU.REGS.NAnd32Bit(register, regValue);

                        return;
                    }
                case Mnem.AOX_rrrr_rrrr:  // NAND rrrr, rrrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.NAnd32Bit(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),   // r1   - destination
                            computer.CPU.REGS.Get32BitRegister(
                                (byte)(mixedReg & 0b00011111)                       // r2   - source
                                ));

                        return;
                    }
                case Mnem.AOX_IrrrI_n: // NAND (rrr), n
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regAddress = computer.CPU.REGS.Get24BitRegister(register);
                        byte regValue = computer.MEMC.Fetch();
                        computer.CPU.REGS.NAnd8BitMem(regAddress, regValue);

                        return;
                    }
                case Mnem.AOX16_IrrrI_nn:   // NAND16 (rrr), nn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regAddress = computer.CPU.REGS.Get24BitRegister(register);
                        ushort regValue = computer.MEMC.Fetch16();
                        computer.CPU.REGS.NAnd16BitMem(regAddress, regValue);

                        return;
                    }
                case Mnem.AOX24_IrrrI_nnn:  // NAND24 (rrr), nnn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regAddress = computer.CPU.REGS.Get24BitRegister(register);
                        uint regValue = computer.MEMC.Fetch24();
                        computer.CPU.REGS.NAnd24BitMem(regAddress, regValue);

                        return;
                    }
                case Mnem.AOX32_IrrrI_nnnn: // NAND32 (rrr), nnnn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regAddress = computer.CPU.REGS.Get24BitRegister(register);
                        uint regValue = computer.MEMC.Fetch32();
                        computer.CPU.REGS.NAnd32BitMem(regAddress, regValue);

                        return;
                    }
                case Mnem.AOX_IrrrI_r: // NAND (rrr), r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.NAnd8BitMem(
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5))
                                ),
                            computer.CPU.REGS.Get8BitRegister(
                                (byte)(mixedReg & 0b00011111)
                                )
                        );

                        return;
                    }
                case Mnem.AOX_IrrrI_rr: // NAND (rrr), rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.NAnd16BitMem(
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5))
                                ),
                            computer.CPU.REGS.Get16BitRegister(
                                (byte)(mixedReg & 0b00011111)
                                )
                        );

                        return;
                    }
                case Mnem.AOX_IrrrI_rrr: // NAND (rrr), rrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.NAnd24BitMem(
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5))
                                ),
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(mixedReg & 0b00011111)
                                )
                        );

                        return;
                    }
                case Mnem.AOX_IrrrI_rrrr: // NAND (rrr), rrrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.NAnd32BitMem(
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
                case Mnem.AOX_r_IrrrI: // NAND r, (rrr)
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        computer.CPU.REGS.NandMemTo8BitReg(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(mixedReg & 0b00011111)
                        ));

                        return;
                    }
                case Mnem.AOX_rr_IrrrI: // NAND rr, (rrr)
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        computer.CPU.REGS.NandMemTo16BitReg(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(mixedReg & 0b00011111)
                        ));

                        return;
                    }
                case Mnem.AOX_rrr_IrrrI: // NAND rrr, (rrr)
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        computer.CPU.REGS.NandMemTo24BitReg(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(mixedReg & 0b00011111)
                        ));

                        return;
                    }
                case Mnem.AOX_rrrr_IrrrI: // NAND rrrr, (rrr)
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        computer.CPU.REGS.NandMemTo32BitReg(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(mixedReg & 0b00011111)
                        ));

                        return;
                    }
                case Mnem.AOX_InnnI_n: // NAND (nnn), n
                    {
                        uint address = computer.MEMC.Fetch24();
                        byte value = computer.MEMC.Fetch();

                        computer.CPU.REGS.NAnd8BitMem(address, value);

                        return;
                    }
                case Mnem.AOX16_InnnI_nn: // NAND16 (nnn), nn
                    {
                        uint address = computer.MEMC.Fetch24();
                        ushort value = computer.MEMC.Fetch16();

                        computer.CPU.REGS.NAnd16BitMem(address, value);

                        return;
                    }
                case Mnem.AOX24_InnnI_nnn: // NAND24 (nnn), nnn
                    {
                        uint address = computer.MEMC.Fetch24();
                        uint value = computer.MEMC.Fetch24();

                        computer.CPU.REGS.NAnd24BitMem(address, value);

                        return;
                    }
                case Mnem.AOX32_InnnI_nnnn: // NAND32 (nnn), nnnn
                    {
                        uint address = computer.MEMC.Fetch24();
                        uint value = computer.MEMC.Fetch32();

                        computer.CPU.REGS.NAnd32BitMem(address, value);

                        return;
                    }
                case Mnem.AOX_InnnI_r: // NAND (nnn), r
                    {
                        uint address = computer.MEMC.Fetch24();
                        byte value =
                            computer.CPU.REGS.Get8BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.CPU.REGS.NAnd8BitMem(address, value);

                        return;
                    }
                case Mnem.AOX_InnnI_rr: // NAND (nnn), rr
                    {
                        uint address = computer.MEMC.Fetch24();
                        ushort value =
                            computer.CPU.REGS.Get16BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.CPU.REGS.NAnd16BitMem(address, value);

                        return;
                    }
                case Mnem.AOX_InnnI_rrr: // NAND (nnn), rrr
                    {
                        uint address = computer.MEMC.Fetch24();
                        uint value =
                            computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.CPU.REGS.NAnd24BitMem(address, value);

                        return;
                    }
                case Mnem.AOX_InnnI_rrrr: // NAND (nnn), rrrr
                    {
                        uint address = computer.MEMC.Fetch24();
                        uint value =
                            computer.CPU.REGS.Get32BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.CPU.REGS.NAnd32BitMem(address, value);

                        return;
                    }
                case Mnem.AOX_r_InnnI: // NAND r, (nnn)
                    {
                        byte reg = computer.MEMC.Fetch();
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.NandMemTo8BitReg(reg, address);

                        return;
                    }
                case Mnem.AOX_rr_InnnI: // NAND rr, (nnn)
                    {
                        byte reg = computer.MEMC.Fetch();
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.NandMemTo16BitReg(reg, address);

                        return;
                    }
                case Mnem.AOX_rrr_InnnI: // NAND rrr, (nnn)
                    {
                        byte reg = computer.MEMC.Fetch();
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.NandMemTo24BitReg(reg, address);

                        return;
                    }
                case Mnem.AOX_rrrr_InnnI: // NAND rrrr, (nnn)
                    {
                        byte reg = computer.MEMC.Fetch();
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.NandMemTo32BitReg(reg, address);

                        return;
                    }
            }
        }
    }
}
