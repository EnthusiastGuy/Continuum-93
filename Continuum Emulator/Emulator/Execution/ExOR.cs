using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public class ExOR
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 2);

            switch (upperLdOp)
            {
                case Mnem.AOX_r_n: // OR r, n
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte regValue = computer.MEMC.Fetch();
                        computer.CPU.REGS.Or8Bit(register, regValue);

                        return;
                    }
                case Mnem.AOX_r_r:  // OR r, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.Or8Bit(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),   // r1   - destination
                            computer.CPU.REGS.Get8BitRegister(
                                (byte)(mixedReg & 0b00011111)                       // r2   - source
                                ));

                        return;
                    }
                case Mnem.AOX_rr_nn:    // OR rr, nn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        ushort regValue = computer.MEMC.Fetch16();
                        computer.CPU.REGS.Or16Bit(register, regValue);

                        return;
                    }
                case Mnem.AOX_rr_rr:  // OR rr, rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.Or16Bit(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),   // r1   - destination
                            computer.CPU.REGS.Get16BitRegister(
                                (byte)(mixedReg & 0b00011111)                       // r2   - source
                                ));

                        return;
                    }
                case Mnem.AOX_rrr_nnn:    // OR rrr, nnn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regValue = computer.MEMC.Fetch24();
                        computer.CPU.REGS.Or24Bit(register, regValue);

                        return;
                    }
                case Mnem.AOX_rrr_rrr:  // OR rrr, rrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.Or24Bit(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),   // r1   - destination
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(mixedReg & 0b00011111)                       // r2   - source
                                ));

                        return;
                    }
                case Mnem.AOX_rrrr_nnnn:    // OR rrrr, nnnn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regValue = computer.MEMC.Fetch32();
                        computer.CPU.REGS.Or32Bit(register, regValue);

                        return;
                    }
                case Mnem.AOX_rrrr_rrrr:  // OR rrrr, rrrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.Or32Bit(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),   // r1   - destination
                            computer.CPU.REGS.Get32BitRegister(
                                (byte)(mixedReg & 0b00011111)                       // r2   - source
                                ));

                        return;
                    }
                case Mnem.AOX_IrrrI_n: // OR (rrr), n
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regAddress = computer.CPU.REGS.Get24BitRegister(register);
                        byte regValue = computer.MEMC.Fetch();
                        computer.CPU.REGS.Or8BitMem(regAddress, regValue);

                        return;
                    }
                case Mnem.AOX16_IrrrI_nn:   // OR16 (rrr), nn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regAddress = computer.CPU.REGS.Get24BitRegister(register);
                        ushort regValue = computer.MEMC.Fetch16();
                        computer.CPU.REGS.Or16BitMem(regAddress, regValue);

                        return;
                    }
                case Mnem.AOX24_IrrrI_nnn:  // OR24 (rrr), nnn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regAddress = computer.CPU.REGS.Get24BitRegister(register);
                        uint regValue = computer.MEMC.Fetch24();
                        computer.CPU.REGS.Or24BitMem(regAddress, regValue);

                        return;
                    }
                case Mnem.AOX32_IrrrI_nnnn: // OR32 (rrr), nnnn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regAddress = computer.CPU.REGS.Get24BitRegister(register);
                        uint regValue = computer.MEMC.Fetch32();
                        computer.CPU.REGS.Or32BitMem(regAddress, regValue);

                        return;
                    }
                case Mnem.AOX_IrrrI_r: // OR (rrr), r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.Or8BitMem(
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5))
                                ),
                            computer.CPU.REGS.Get8BitRegister(
                                (byte)(mixedReg & 0b00011111)
                                )
                        );

                        return;
                    }
                case Mnem.AOX_IrrrI_rr: // OR (rrr), rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.Or16BitMem(
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5))
                                ),
                            computer.CPU.REGS.Get16BitRegister(
                                (byte)(mixedReg & 0b00011111)
                                )
                        );

                        return;
                    }
                case Mnem.AOX_IrrrI_rrr: // OR (rrr), rrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.Or24BitMem(
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5))
                                ),
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(mixedReg & 0b00011111)
                                )
                        );

                        return;
                    }
                case Mnem.AOX_IrrrI_rrrr: // OR (rrr), rrrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        computer.CPU.REGS.Or32BitMem(
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
                case Mnem.AOX_r_IrrrI: // OR r, (rrr)
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        computer.CPU.REGS.OrMemTo8BitReg(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(mixedReg & 0b00011111)
                        ));

                        return;
                    }
                case Mnem.AOX_rr_IrrrI: // OR rr, (rrr)
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        computer.CPU.REGS.OrMemTo16BitReg(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(mixedReg & 0b00011111)
                        ));

                        return;
                    }
                case Mnem.AOX_rrr_IrrrI: // OR rrr, (rrr)
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        computer.CPU.REGS.OrMemTo24BitReg(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(mixedReg & 0b00011111)
                        ));

                        return;
                    }
                case Mnem.AOX_rrrr_IrrrI: // OR rrrr, (rrr)
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        computer.CPU.REGS.OrMemTo32BitReg(
                            (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(mixedReg & 0b00011111)
                        ));

                        return;
                    }
                case Mnem.AOX_InnnI_n: // OR (nnn), n
                    {
                        uint address = computer.MEMC.Fetch24();
                        byte value = computer.MEMC.Fetch();

                        computer.CPU.REGS.Or8BitMem(address, value);

                        return;
                    }
                case Mnem.AOX16_InnnI_nn: // OR16 (nnn), nn
                    {
                        uint address = computer.MEMC.Fetch24();
                        ushort value = computer.MEMC.Fetch16();

                        computer.CPU.REGS.Or16BitMem(address, value);

                        return;
                    }
                case Mnem.AOX24_InnnI_nnn: // OR24 (nnn), nnn
                    {
                        uint address = computer.MEMC.Fetch24();
                        uint value = computer.MEMC.Fetch24();

                        computer.CPU.REGS.Or24BitMem(address, value);

                        return;
                    }
                case Mnem.AOX32_InnnI_nnnn: // OR32 (nnn), nnnn
                    {
                        uint address = computer.MEMC.Fetch24();
                        uint value = computer.MEMC.Fetch32();

                        computer.CPU.REGS.Or32BitMem(address, value);

                        return;
                    }
                case Mnem.AOX_InnnI_r: // OR (nnn), r
                    {
                        uint address = computer.MEMC.Fetch24();
                        byte value =
                            computer.CPU.REGS.Get8BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.CPU.REGS.Or8BitMem(address, value);

                        return;
                    }
                case Mnem.AOX_InnnI_rr: // OR (nnn), rr
                    {
                        uint address = computer.MEMC.Fetch24();
                        ushort value =
                            computer.CPU.REGS.Get16BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.CPU.REGS.Or16BitMem(address, value);

                        return;
                    }
                case Mnem.AOX_InnnI_rrr: // OR (nnn), rrr
                    {
                        uint address = computer.MEMC.Fetch24();
                        uint value =
                            computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.CPU.REGS.Or24BitMem(address, value);

                        return;
                    }
                case Mnem.AOX_InnnI_rrrr: // OR (nnn), rrrr
                    {
                        uint address = computer.MEMC.Fetch24();
                        uint value =
                            computer.CPU.REGS.Get32BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.CPU.REGS.Or32BitMem(address, value);

                        return;
                    }
                case Mnem.AOX_r_InnnI: // OR r, (nnn)
                    {
                        byte reg = computer.MEMC.Fetch();
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.OrMemTo8BitReg(reg, address);

                        return;
                    }
                case Mnem.AOX_rr_InnnI: // OR rr, (nnn)
                    {
                        byte reg = computer.MEMC.Fetch();
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.OrMemTo16BitReg(reg, address);

                        return;
                    }
                case Mnem.AOX_rrr_InnnI: // OR rrr, (nnn)
                    {
                        byte reg = computer.MEMC.Fetch();
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.OrMemTo24BitReg(reg, address);

                        return;
                    }
                case Mnem.AOX_rrrr_InnnI: // OR rrrr, (nnn)
                    {
                        byte reg = computer.MEMC.Fetch();
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.OrMemTo32BitReg(reg, address);

                        return;
                    }
            }
        }
    }
}
