using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public static class ExXNOR
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 2);

            switch (upperLdOp)
            {
                case Mnem.AOX_r_n: // XNOR r, n
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte regValue = computer.MEMC.Fetch();
                        computer.CPU.REGS.XNor8Bit(register, regValue);

                        return;
                    }
                case Mnem.AOX_r_r:  // XNOR r, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.XNor8Bit(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),   // r1   - destination
                            computer.CPU.REGS.Get8BitRegister(
                                (byte)(mixedReg & 0b00011111)                       // r2   - source
                                ));

                        return;
                    }
                case Mnem.AOX_rr_nn:    // XNOR rr, nn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        ushort regValue = computer.MEMC.Fetch16();
                        computer.CPU.REGS.XNor16Bit(register, regValue);

                        return;
                    }
                case Mnem.AOX_rr_rr:  // XNOR rr, rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.XNor16Bit(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),   // r1   - destination
                            computer.CPU.REGS.Get16BitRegister(
                                (byte)(mixedReg & 0b00011111)                       // r2   - source
                                ));

                        return;
                    }
                case Mnem.AOX_rrr_nnn:    // XNOR rrr, nnn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regValue = computer.MEMC.Fetch24();
                        computer.CPU.REGS.XNor24Bit(register, regValue);

                        return;
                    }
                case Mnem.AOX_rrr_rrr:  // XNOR rrr, rrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.XNor24Bit(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),   // r1   - destination
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(mixedReg & 0b00011111)                       // r2   - source
                                ));

                        return;
                    }
                case Mnem.AOX_rrrr_nnnn:    // XNOR rrrr, nnnn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regValue = computer.MEMC.Fetch32();
                        computer.CPU.REGS.XNor32Bit(register, regValue);

                        return;
                    }
                case Mnem.AOX_rrrr_rrrr:  // XNOR rrrr, rrrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.XNor32Bit(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),   // r1   - destination
                            computer.CPU.REGS.Get32BitRegister(
                                (byte)(mixedReg & 0b00011111)                       // r2   - source
                                ));

                        return;
                    }
                case Mnem.AOX_IrrrI_n: // XNOR (rrr), n
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regAddress = computer.CPU.REGS.Get24BitRegister(register);
                        byte regValue = computer.MEMC.Fetch();
                        computer.CPU.REGS.XNor8BitMem(regAddress, regValue);

                        return;
                    }
                case Mnem.AOX16_IrrrI_nn:   // XNOR16 (rrr), nn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regAddress = computer.CPU.REGS.Get24BitRegister(register);
                        ushort regValue = computer.MEMC.Fetch16();
                        computer.CPU.REGS.XNor16BitMem(regAddress, regValue);

                        return;
                    }
                case Mnem.AOX24_IrrrI_nnn:  // XNOR24 (rrr), nnn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regAddress = computer.CPU.REGS.Get24BitRegister(register);
                        uint regValue = computer.MEMC.Fetch24();
                        computer.CPU.REGS.XNor24BitMem(regAddress, regValue);

                        return;
                    }
                case Mnem.AOX32_IrrrI_nnnn: // XNOR32 (rrr), nnnn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regAddress = computer.CPU.REGS.Get24BitRegister(register);
                        uint regValue = computer.MEMC.Fetch32();
                        computer.CPU.REGS.XNor32BitMem(regAddress, regValue);

                        return;
                    }
                case Mnem.AOX_IrrrI_r: // XNOR (rrr), r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.XNor8BitMem(
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5))
                                ),
                            computer.CPU.REGS.Get8BitRegister(
                                (byte)(mixedReg & 0b00011111)
                                )
                        );

                        return;
                    }
                case Mnem.AOX_IrrrI_rr: // XNOR (rrr), rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.XNor16BitMem(
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5))
                                ),
                            computer.CPU.REGS.Get16BitRegister(
                                (byte)(mixedReg & 0b00011111)
                                )
                        );

                        return;
                    }
                case Mnem.AOX_IrrrI_rrr: // XNOR (rrr), rrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.XNor24BitMem(
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5))
                                ),
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(mixedReg & 0b00011111)
                                )
                        );

                        return;
                    }
                case Mnem.AOX_IrrrI_rrrr: // XNOR (rrr), rrrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.XNor32BitMem(
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
                case Mnem.AOX_r_IrrrI: // XNOR r, (rrr)
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        computer.CPU.REGS.XNorMemTo8BitReg(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(mixedReg & 0b00011111)
                        ));

                        return;
                    }
                case Mnem.AOX_rr_IrrrI: // XNOR rr, (rrr)
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        computer.CPU.REGS.XNorMemTo16BitReg(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(mixedReg & 0b00011111)
                        ));

                        return;
                    }
                case Mnem.AOX_rrr_IrrrI: // XNOR rrr, (rrr)
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        computer.CPU.REGS.XNorMemTo24BitReg(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(mixedReg & 0b00011111)
                        ));

                        return;
                    }
                case Mnem.AOX_rrrr_IrrrI: // XNOR rrrr, (rrr)
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        computer.CPU.REGS.XNorMemTo32BitReg(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(mixedReg & 0b00011111)
                        ));

                        return;
                    }
                case Mnem.AOX_InnnI_n: // XNOR (nnn), n
                    {
                        uint address = computer.MEMC.Fetch24();
                        byte value = computer.MEMC.Fetch();

                        computer.CPU.REGS.XNor8BitMem(address, value);

                        return;
                    }
                case Mnem.AOX16_InnnI_nn: // XNOR16 (nnn), nn
                    {
                        uint address = computer.MEMC.Fetch24();
                        ushort value = computer.MEMC.Fetch16();

                        computer.CPU.REGS.XNor16BitMem(address, value);

                        return;
                    }
                case Mnem.AOX24_InnnI_nnn: // XNOR24 (nnn), nnn
                    {
                        uint address = computer.MEMC.Fetch24();
                        uint value = computer.MEMC.Fetch24();

                        computer.CPU.REGS.XNor24BitMem(address, value);

                        return;
                    }
                case Mnem.AOX32_InnnI_nnnn: // XNOR32 (nnn), nnnn
                    {
                        uint address = computer.MEMC.Fetch24();
                        uint value = computer.MEMC.Fetch32();

                        computer.CPU.REGS.XNor32BitMem(address, value);

                        return;
                    }
                case Mnem.AOX_InnnI_r: // XNOR (nnn), r
                    {
                        uint address = computer.MEMC.Fetch24();
                        byte value =
                            computer.CPU.REGS.Get8BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.CPU.REGS.XNor8BitMem(address, value);

                        return;
                    }
                case Mnem.AOX_InnnI_rr: // XNOR (nnn), rr
                    {
                        uint address = computer.MEMC.Fetch24();
                        ushort value =
                            computer.CPU.REGS.Get16BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.CPU.REGS.XNor16BitMem(address, value);

                        return;
                    }
                case Mnem.AOX_InnnI_rrr: // XNOR (nnn), rrr
                    {
                        uint address = computer.MEMC.Fetch24();
                        uint value =
                            computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.CPU.REGS.XNor24BitMem(address, value);

                        return;
                    }
                case Mnem.AOX_InnnI_rrrr: // XNOR (nnn), rrrr
                    {
                        uint address = computer.MEMC.Fetch24();
                        uint value =
                            computer.CPU.REGS.Get32BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.CPU.REGS.XNor32BitMem(address, value);

                        return;
                    }
                case Mnem.AOX_r_InnnI: // XNOR r, (nnn)
                    {
                        byte reg = computer.MEMC.Fetch();
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.XNorMemTo8BitReg(reg, address);

                        return;
                    }
                case Mnem.AOX_rr_InnnI: // XNOR rr, (nnn)
                    {
                        byte reg = computer.MEMC.Fetch();
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.XNorMemTo16BitReg(reg, address);

                        return;
                    }
                case Mnem.AOX_rrr_InnnI: // XNOR rrr, (nnn)
                    {
                        byte reg = computer.MEMC.Fetch();
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.XNorMemTo24BitReg(reg, address);

                        return;
                    }
                case Mnem.AOX_rrrr_InnnI: // XNOR rrrr, (nnn)
                    {
                        byte reg = computer.MEMC.Fetch();
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.XNorMemTo32BitReg(reg, address);

                        return;
                    }
            }
        }
    }
}
