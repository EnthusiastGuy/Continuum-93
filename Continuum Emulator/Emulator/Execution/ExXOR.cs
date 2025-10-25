using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public class ExXOR
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 2);

            switch (upperLdOp)
            {
                case Mnem.AOX_r_n: // XOR r, n
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte regValue = computer.MEMC.Fetch();
                        computer.CPU.REGS.Xor8Bit(register, regValue);

                        return;
                    }
                case Mnem.AOX_r_r:  // XOR r, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.Xor8Bit(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),   // r1   - destination
                            computer.CPU.REGS.Get8BitRegister(
                                (byte)(mixedReg & 0b00011111)                       // r2   - source
                                ));

                        return;
                    }
                case Mnem.AOX_rr_nn:    // XOR rr, nn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        ushort regValue = computer.MEMC.Fetch16();
                        computer.CPU.REGS.Xor16Bit(register, regValue);

                        return;
                    }
                case Mnem.AOX_rr_rr:  // XOR rr, rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.Xor16Bit(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),   // r1   - destination
                            computer.CPU.REGS.Get16BitRegister(
                                (byte)(mixedReg & 0b00011111)                       // r2   - source
                                ));

                        return;
                    }
                case Mnem.AOX_rrr_nnn:    // XOR rrr, nnn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regValue = computer.MEMC.Fetch24();
                        computer.CPU.REGS.Xor24Bit(register, regValue);

                        return;
                    }
                case Mnem.AOX_rrr_rrr:  // XOR rrr, rrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.Xor24Bit(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),   // r1   - destination
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(mixedReg & 0b00011111)                       // r2   - source
                                ));

                        return;
                    }
                case Mnem.AOX_rrrr_nnnn:    // XOR rrrr, nnnn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regValue = computer.MEMC.Fetch32();
                        computer.CPU.REGS.Xor32Bit(register, regValue);

                        return;
                    }
                case Mnem.AOX_rrrr_rrrr:  // XOR rrrr, rrrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.Xor32Bit(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),   // r1   - destination
                            computer.CPU.REGS.Get32BitRegister(
                                (byte)(mixedReg & 0b00011111)                       // r2   - source
                                ));

                        return;
                    }
                case Mnem.AOX_IrrrI_n: // XOR (rrr), n
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regAddress = computer.CPU.REGS.Get24BitRegister(register);
                        byte regValue = computer.MEMC.Fetch();
                        computer.CPU.REGS.Xor8BitMem(regAddress, regValue);

                        return;
                    }
                case Mnem.AOX16_IrrrI_nn:   // XOR16 (rrr), nn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regAddress = computer.CPU.REGS.Get24BitRegister(register);
                        ushort regValue = computer.MEMC.Fetch16();
                        computer.CPU.REGS.Xor16BitMem(regAddress, regValue);

                        return;
                    }
                case Mnem.AOX24_IrrrI_nnn:  // XOR24 (rrr), nnn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regAddress = computer.CPU.REGS.Get24BitRegister(register);
                        uint regValue = computer.MEMC.Fetch24();
                        computer.CPU.REGS.Xor24BitMem(regAddress, regValue);

                        return;
                    }
                case Mnem.AOX32_IrrrI_nnnn: // XOR32 (rrr), nnnn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regAddress = computer.CPU.REGS.Get24BitRegister(register);
                        uint regValue = computer.MEMC.Fetch32();
                        computer.CPU.REGS.Xor32BitMem(regAddress, regValue);

                        return;
                    }
                case Mnem.AOX_IrrrI_r: // XOR (rrr), r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.Xor8BitMem(
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5))
                                ),
                            computer.CPU.REGS.Get8BitRegister(
                                (byte)(mixedReg & 0b00011111)
                                )
                        );

                        return;
                    }
                case Mnem.AOX_IrrrI_rr: // XOR (rrr), rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.Xor16BitMem(
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5))
                                ),
                            computer.CPU.REGS.Get16BitRegister(
                                (byte)(mixedReg & 0b00011111)
                                )
                        );

                        return;
                    }
                case Mnem.AOX_IrrrI_rrr: // XOR (rrr), rrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.Xor24BitMem(
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5))
                                ),
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(mixedReg & 0b00011111)
                                )
                        );

                        return;
                    }
                case Mnem.AOX_IrrrI_rrrr: // XOR (rrr), rrrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.Xor32BitMem(
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
                case Mnem.AOX_r_IrrrI: // XOR r, (rrr)
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        computer.CPU.REGS.XorMemTo8BitReg(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(mixedReg & 0b00011111)
                        ));

                        return;
                    }
                case Mnem.AOX_rr_IrrrI: // XOR rr, (rrr)
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        computer.CPU.REGS.XorMemTo16BitReg(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(mixedReg & 0b00011111)
                        ));

                        return;
                    }
                case Mnem.AOX_rrr_IrrrI: // XOR rrr, (rrr)
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        computer.CPU.REGS.XorMemTo24BitReg(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(mixedReg & 0b00011111)
                        ));

                        return;
                    }
                case Mnem.AOX_rrrr_IrrrI: // XOR rrrr, (rrr)
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        computer.CPU.REGS.XorMemTo32BitReg(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(mixedReg & 0b00011111)
                        ));

                        return;
                    }
                case Mnem.AOX_InnnI_n: // XOR (nnn), n
                    {
                        uint address = computer.MEMC.Fetch24();
                        byte value = computer.MEMC.Fetch();

                        computer.CPU.REGS.Xor8BitMem(address, value);

                        return;
                    }
                case Mnem.AOX16_InnnI_nn: // XOR16 (nnn), nn
                    {
                        uint address = computer.MEMC.Fetch24();
                        ushort value = computer.MEMC.Fetch16();

                        computer.CPU.REGS.Xor16BitMem(address, value);

                        return;
                    }
                case Mnem.AOX24_InnnI_nnn: // XOR24 (nnn), nnn
                    {
                        uint address = computer.MEMC.Fetch24();
                        uint value = computer.MEMC.Fetch24();

                        computer.CPU.REGS.Xor24BitMem(address, value);

                        return;
                    }
                case Mnem.AOX32_InnnI_nnnn: // XOR32 (nnn), nnnn
                    {
                        uint address = computer.MEMC.Fetch24();
                        uint value = computer.MEMC.Fetch32();

                        computer.CPU.REGS.Xor32BitMem(address, value);

                        return;
                    }
                case Mnem.AOX_InnnI_r: // XOR (nnn), r
                    {
                        uint address = computer.MEMC.Fetch24();
                        byte value =
                            computer.CPU.REGS.Get8BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.CPU.REGS.Xor8BitMem(address, value);

                        return;
                    }
                case Mnem.AOX_InnnI_rr: // XOR (nnn), rr
                    {
                        uint address = computer.MEMC.Fetch24();
                        ushort value =
                            computer.CPU.REGS.Get16BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.CPU.REGS.Xor16BitMem(address, value);

                        return;
                    }
                case Mnem.AOX_InnnI_rrr: // XOR (nnn), rrr
                    {
                        uint address = computer.MEMC.Fetch24();
                        uint value =
                            computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.CPU.REGS.Xor24BitMem(address, value);

                        return;
                    }
                case Mnem.AOX_InnnI_rrrr: // XOR (nnn), rrrr
                    {
                        uint address = computer.MEMC.Fetch24();
                        uint value =
                            computer.CPU.REGS.Get32BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.CPU.REGS.Xor32BitMem(address, value);

                        return;
                    }
                case Mnem.AOX_r_InnnI: // XOR r, (nnn)
                    {
                        byte reg = computer.MEMC.Fetch();
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.XorMemTo8BitReg(reg, address);

                        return;
                    }
                case Mnem.AOX_rr_InnnI: // XOR rr, (nnn)
                    {
                        byte reg = computer.MEMC.Fetch();
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.XorMemTo16BitReg(reg, address);

                        return;
                    }
                case Mnem.AOX_rrr_InnnI: // XOR rrr, (nnn)
                    {
                        byte reg = computer.MEMC.Fetch();
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.XorMemTo24BitReg(reg, address);

                        return;
                    }
                case Mnem.AOX_rrrr_InnnI: // XOR rrrr, (nnn)
                    {
                        byte reg = computer.MEMC.Fetch();
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.XorMemTo32BitReg(reg, address);

                        return;
                    }
            }
        }
    }
}
