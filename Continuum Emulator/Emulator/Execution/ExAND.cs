using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public class ExAND
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();

            switch (ldOp)
            {
                case Mnem.AOX_r_n: // AND r, n
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte regValue = computer.MEMC.Fetch();
                        computer.CPU.REGS.And8Bit(register, regValue);

                        return;
                    }
                case Mnem.AOX_r_r:  // AND r, r
                    {
                        computer.CPU.REGS.And8Bit(
                            (byte)(computer.MEMC.Fetch() & 0b00011111),         // r1   - destination
                            computer.CPU.REGS.Get8BitRegister(
                                (byte)(computer.MEMC.Fetch() & 0b00011111)      // r2   - source
                                ));

                        return;
                    }
                case Mnem.AOX_rr_nn:    // AND rr, nn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        ushort regValue = computer.MEMC.Fetch16();
                        computer.CPU.REGS.And16Bit(register, regValue);

                        return;
                    }
                case Mnem.AOX_rr_rr:  // AND rr, rr
                    {
                        computer.CPU.REGS.And16Bit(
                            (byte)(computer.MEMC.Fetch() & 0b00011111),   // r1   - destination
                            computer.CPU.REGS.Get16BitRegister(
                                (byte)(computer.MEMC.Fetch() & 0b00011111)                       // r2   - source
                                ));

                        return;
                    }
                case Mnem.AOX_rrr_nnn:    // AND rrr, nnn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regValue = computer.MEMC.Fetch24();
                        computer.CPU.REGS.And24Bit(register, regValue);

                        return;
                    }
                case Mnem.AOX_rrr_rrr:  // AND rrr, rrr
                    {
                        computer.CPU.REGS.And24Bit(
                            (byte)(computer.MEMC.Fetch() & 0b00011111),   // r1   - destination
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(computer.MEMC.Fetch() & 0b00011111)                       // r2   - source
                                ));

                        return;
                    }
                case Mnem.AOX_rrrr_nnnn:    // AND rrrr, nnnn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regValue = computer.MEMC.Fetch32();
                        computer.CPU.REGS.And32Bit(register, regValue);

                        return;
                    }
                case Mnem.AOX_rrrr_rrrr:  // AND rrrr, rrrr
                    {
                        computer.CPU.REGS.And32Bit(
                            (byte)(computer.MEMC.Fetch() & 0b00011111),   // r1   - destination
                            computer.CPU.REGS.Get32BitRegister(
                                (byte)(computer.MEMC.Fetch() & 0b00011111)                       // r2   - source
                                ));

                        return;
                    }
                case Mnem.AOX_IrrrI_n: // AND (rrr), n
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regAddress = computer.CPU.REGS.Get24BitRegister(register);
                        byte regValue = computer.MEMC.Fetch();
                        computer.CPU.REGS.And8BitRegToMem(regAddress, regValue);

                        return;
                    }
                case Mnem.AOX16_IrrrI_nn:   // AND16 (rrr), nn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regAddress = computer.CPU.REGS.Get24BitRegister(register);
                        ushort regValue = computer.MEMC.Fetch16();
                        computer.CPU.REGS.And16BitRegToMem(regAddress, regValue);

                        return;
                    }
                case Mnem.AOX24_IrrrI_nnn:  // AND24 (rrr), nnn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regAddress = computer.CPU.REGS.Get24BitRegister(register);
                        uint regValue = computer.MEMC.Fetch24();
                        computer.CPU.REGS.And24BitRegToMem(regAddress, regValue);

                        return;
                    }
                case Mnem.AOX32_IrrrI_nnnn: // AND32 (rrr), nnnn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint regAddress = computer.CPU.REGS.Get24BitRegister(register);
                        uint regValue = computer.MEMC.Fetch32();
                        computer.CPU.REGS.And32BitRegToMem(regAddress, regValue);

                        return;
                    }
                case Mnem.AOX_IrrrI_r: // AND (rrr), r
                    {
                        computer.CPU.REGS.And8BitRegToMem(
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(computer.MEMC.Fetch() & 0b00011111)
                                ),
                            computer.CPU.REGS.Get8BitRegister(
                                (byte)(computer.MEMC.Fetch() & 0b00011111)
                                )
                        );

                        return;
                    }
                case Mnem.AOX_IrrrI_rr: // AND (rrr), rr
                    {
                        computer.CPU.REGS.And16BitRegToMem(
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(computer.MEMC.Fetch() & 0b00011111)
                                ),
                            computer.CPU.REGS.Get16BitRegister(
                                (byte)(computer.MEMC.Fetch() & 0b00011111)
                                )
                        );

                        return;
                    }
                case Mnem.AOX_IrrrI_rrr: // AND (rrr), rrr
                    {
                        computer.CPU.REGS.And24BitRegToMem(
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(computer.MEMC.Fetch() & 0b00011111)
                                ),
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(computer.MEMC.Fetch() & 0b00011111)
                                )
                        );

                        return;
                    }
                case Mnem.AOX_IrrrI_rrrr: // AND (rrr), rrrr
                    {
                        computer.CPU.REGS.And32BitRegToMem(
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(computer.MEMC.Fetch() & 0b00011111)
                                ),
                            computer.CPU.REGS.Get32BitRegister(
                                (byte)(computer.MEMC.Fetch() & 0b00011111)
                                )
                        );

                        return;
                    }

                // new
                case Mnem.AOX_r_IrrrI: // AND r, (rrr)
                    {
                        computer.CPU.REGS.AndMemTo8BitReg(
                            (byte)(computer.MEMC.Fetch() & 0b00011111),
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(computer.MEMC.Fetch() & 0b00011111)
                        ));

                        return;
                    }
                case Mnem.AOX_rr_IrrrI: // AND rr, (rrr)
                    {
                        computer.CPU.REGS.AndMemTo16BitReg(
                            (byte)(computer.MEMC.Fetch() & 0b00011111),
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(computer.MEMC.Fetch() & 0b00011111)
                        ));

                        return;
                    }
                case Mnem.AOX_rrr_IrrrI: // AND rrr, (rrr)
                    {
                        computer.CPU.REGS.AndMemTo24BitReg(
                            (byte)(computer.MEMC.Fetch() & 0b00011111),
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(computer.MEMC.Fetch() & 0b00011111)
                        ));

                        return;
                    }
                case Mnem.AOX_rrrr_IrrrI: // AND rrrr, (rrr)
                    {
                        computer.CPU.REGS.AndMemTo32BitReg(
                            (byte)(computer.MEMC.Fetch() & 0b00011111),
                            computer.CPU.REGS.Get24BitRegister(
                                (byte)(computer.MEMC.Fetch() & 0b00011111)
                        ));

                        return;
                    }
                case Mnem.AOX_InnnI_n: // AND (nnn), n
                    {
                        uint address = computer.MEMC.Fetch24();
                        byte value = computer.MEMC.Fetch();

                        computer.CPU.REGS.And8BitRegToMem(address, value);

                        return;
                    }
                case Mnem.AOX16_InnnI_nn: // AND16 (nnn), nn
                    {
                        uint address = computer.MEMC.Fetch24();
                        ushort value = computer.MEMC.Fetch16();

                        computer.CPU.REGS.And16BitRegToMem(address, value);

                        return;
                    }
                case Mnem.AOX24_InnnI_nnn: // AND24 (nnn), nnn
                    {
                        uint address = computer.MEMC.Fetch24();
                        uint value = computer.MEMC.Fetch24();

                        computer.CPU.REGS.And24BitRegToMem(address, value);

                        return;
                    }
                case Mnem.AOX32_InnnI_nnnn: // AND32 (nnn), nnnn
                    {
                        uint address = computer.MEMC.Fetch24();
                        uint value = computer.MEMC.Fetch32();

                        computer.CPU.REGS.And32BitRegToMem(address, value);

                        return;
                    }
                case Mnem.AOX_InnnI_r: // AND (nnn), r
                    {
                        uint address = computer.MEMC.Fetch24();
                        byte value =
                            computer.CPU.REGS.Get8BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.CPU.REGS.And8BitRegToMem(address, value);

                        return;
                    }
                case Mnem.AOX_InnnI_rr: // AND (nnn), rr
                    {
                        uint address = computer.MEMC.Fetch24();
                        ushort value =
                            computer.CPU.REGS.Get16BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.CPU.REGS.And16BitRegToMem(address, value);

                        return;
                    }
                case Mnem.AOX_InnnI_rrr: // AND (nnn), rrr
                    {
                        uint address = computer.MEMC.Fetch24();
                        uint value =
                            computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.CPU.REGS.And24BitRegToMem(address, value);

                        return;
                    }
                case Mnem.AOX_InnnI_rrrr: // AND (nnn), rrrr
                    {
                        uint address = computer.MEMC.Fetch24();
                        uint value =
                            computer.CPU.REGS.Get32BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.CPU.REGS.And32BitRegToMem(address, value);

                        return;
                    }
                case Mnem.AOX_r_InnnI: // AND r, (nnn)
                    {
                        byte reg = computer.MEMC.Fetch();
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.AndMemTo8BitReg(reg, address);

                        return;
                    }
                case Mnem.AOX_rr_InnnI: // AND rr, (nnn)
                    {
                        byte reg = computer.MEMC.Fetch();
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.AndMemTo16BitReg(reg, address);

                        return;
                    }
                case Mnem.AOX_rrr_InnnI: // AND rrr, (nnn)
                    {
                        byte reg = computer.MEMC.Fetch();
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.AndMemTo24BitReg(reg, address);

                        return;
                    }
                case Mnem.AOX_rrrr_InnnI: // AND rrrr, (nnn)
                    {
                        byte reg = computer.MEMC.Fetch();
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.AndMemTo32BitReg(reg, address);

                        return;
                    }
            }
        }
    }
}
