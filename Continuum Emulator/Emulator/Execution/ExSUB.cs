using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;
using Continuum93.Tools;
using System;

namespace Continuum93.Emulator.Execution
{
    public static class ExSUB
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();

            switch (ldOp)
            {
                case Mnem.ADDSUB_r_n:   // SUB r, n
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte regValue = computer.MEMC.Fetch();
                        computer.CPU.REGS.SubtractFrom8BitRegister(register, regValue);
                        return;
                    }
                case Mnem.ADDSUB_r_r:   // SUB r, r
                    {
                        byte reg1 = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte reg2 = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        computer.CPU.REGS.SubtractFrom8BitRegister(
                            reg1,
                            computer.CPU.REGS.Get8BitRegister(reg2));
                        return;
                    }
                case Mnem.ADDSUB_r_InnnI:   // SUB r, (nnn)
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint address = computer.MEMC.Fetch24();
                        computer.CPU.REGS.SubtractFrom8BitRegister(register, computer.MEMC.Get8bitFromRAM(address));
                        return;
                    }
                case Mnem.ADDSUB_r_IrrrI:   // SUB r, (rrr)
                    {
                        byte reg1 = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte reg2 = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        computer.CPU.REGS.SubtractFrom8BitRegister(
                            reg1,
                            computer.MEMC.Get8bitFromRAM(computer.CPU.REGS.Get24BitRegister(reg2)));
                        return;
                    }
                case Mnem.ADDSUB_rr_n:   // SUB rr, n
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        ushort regValue = computer.MEMC.Fetch();
                        computer.CPU.REGS.SubtractFrom16BitRegister(register, regValue);
                        return;
                    }
                case Mnem.ADDSUB16_rr_nn:   // SUB16 rr, nn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        ushort regValue = computer.MEMC.Fetch16();
                        computer.CPU.REGS.SubtractFrom16BitRegister(register, regValue);
                        return;
                    }
                case Mnem.ADDSUB_rr_r:   // SUB rr, r
                    {
                        byte reg1 = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte reg2 = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        computer.CPU.REGS.SubtractFrom16BitRegister(reg1, computer.CPU.REGS.Get8BitRegister(reg2));
                        return;
                    }
                case Mnem.ADDSUB_rr_rr:   // SUB rr, rr
                    {
                        byte reg1 = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte reg2 = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        computer.CPU.REGS.SubtractFrom16BitRegister(reg1, computer.CPU.REGS.Get16BitRegister(reg2));
                        return;
                    }
                case Mnem.ADDSUB_rr_InnnI:   // SUB rr, (nnn) or ADD rr, 16(nnn)
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.SubtractFrom16BitRegister(register, computer.MEMC.Get8bitFromRAM(address));

                        return;
                    }
                case Mnem.ADDSUB16_rr_InnnI:
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.SubtractFrom16BitRegister(register, computer.MEMC.Get16bitFromRAM(address));

                        return;
                    }
                case Mnem.ADDSUB_rr_IrrrI:   // SUB rr, (rrr)
                    {
                        byte reg1 = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte reg2 = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        computer.CPU.REGS.SubtractFrom16BitRegister(
                            reg1,
                            computer.MEMC.Get8bitFromRAM(computer.CPU.REGS.Get24BitRegister(reg2)));
                        return;
                    }
                case Mnem.ADDSUB16_rr_IrrrI:    // SUB16 rr, (rrr)
                    {
                        byte reg1 = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte reg2 = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        computer.CPU.REGS.SubtractFrom16BitRegister(
                            reg1,
                            computer.MEMC.Get16bitFromRAM(computer.CPU.REGS.Get24BitRegister(reg2)));
                        return;
                    }
                case Mnem.ADDSUB_rrr_n:    // SUB rrr, n
                    {
                        byte reg = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        computer.CPU.REGS.SubtractFrom24BitRegister(reg, computer.MEMC.Fetch());

                        return;
                    }
                case Mnem.ADDSUB16_rrr_n:   // SUB16 rrr, nn
                    {
                        byte reg = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        computer.CPU.REGS.SubtractFrom24BitRegister(reg, computer.MEMC.Fetch16());

                        return;
                    }
                case Mnem.ADDSUB24_rrr_n:   // SUB24 rrr, nnn
                    {
                        byte reg = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        computer.CPU.REGS.SubtractFrom24BitRegister(reg, computer.MEMC.Fetch24());

                        return;
                    }
                case Mnem.ADDSUB_rrr_r:     // SUB rrr, r
                    {
                        byte reg1 = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte reg2 = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        computer.CPU.REGS.SubtractFrom24BitRegister(reg1, computer.CPU.REGS.Get8BitRegister(reg2));

                        return;
                    }
                case Mnem.ADDSUB_rrr_rr:     // SUB rrr, rr
                    {
                        byte reg1 = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte reg2 = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        computer.CPU.REGS.SubtractFrom24BitRegister(reg1, computer.CPU.REGS.Get16BitRegister(reg2));
                        return;
                    }
                case Mnem.ADDSUB_rrr_rrr:     // SUB rrr, rrr
                    {
                        byte reg1 = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte reg2 = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        computer.CPU.REGS.SubtractFrom24BitRegister(reg1, computer.CPU.REGS.Get24BitRegister(reg2));
                        return;
                    }
                case Mnem.ADDSUB_rrr_InnnI:    // SUB rrr, n or ADD16 rrr, nn or ADD 24 rrr, nnn
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.SubtractFrom24BitRegister(register, computer.MEMC.Get8bitFromRAM(address));
                        return;
                    }
                case Mnem.ADDSUB16_rrr_InnnI:
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.SubtractFrom24BitRegister(register, computer.MEMC.Get16bitFromRAM(address));
                        return;
                    }
                case Mnem.ADDSUB24_rrr_InnnI:
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.SubtractFrom24BitRegister(register, computer.MEMC.Get24bitFromRAM(address));
                        return;
                    }
                case Mnem.ADDSUB_rrr_IrrrI:
                    {
                        byte reg1 = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte reg2 = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        computer.CPU.REGS.SubtractFrom24BitRegister(
                            reg1,
                            computer.MEMC.Get8bitFromRAM(computer.CPU.REGS.Get24BitRegister(reg2)));
                        return;
                    }
                case Mnem.ADDSUB16_rrr_IrrrI:
                    {
                        byte reg1 = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte reg2 = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        computer.CPU.REGS.SubtractFrom24BitRegister(
                            reg1,
                            computer.MEMC.Get16bitFromRAM(computer.CPU.REGS.Get24BitRegister(reg2)));
                        return;
                    }
                case Mnem.ADDSUB24_rrr_IrrrI:
                    {
                        byte reg1 = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte reg2 = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        computer.CPU.REGS.SubtractFrom24BitRegister(
                            reg1,
                            computer.MEMC.Get24bitFromRAM(computer.CPU.REGS.Get24BitRegister(reg2)));
                        return;
                    }
                case Mnem.ADDSUB_rrrr_n:
                    {
                        computer.CPU.REGS.SubtractFrom32BitRegister(
                            (byte)(computer.MEMC.Fetch() & 0b00011111),
                            computer.MEMC.Fetch());
                        return;
                    }
                case Mnem.ADDSUB16_rrrr_n:
                    {
                        computer.CPU.REGS.SubtractFrom32BitRegister(
                            (byte)(computer.MEMC.Fetch() & 0b00011111),
                            computer.MEMC.Fetch16());
                        return;
                    }
                case Mnem.ADDSUB24_rrrr_n:
                    {
                        computer.CPU.REGS.SubtractFrom32BitRegister(
                            (byte)(computer.MEMC.Fetch() & 0b00011111),
                            computer.MEMC.Fetch24());
                        return;
                    }
                case Mnem.ADDSUB32_rrrr_n:
                    {
                        computer.CPU.REGS.SubtractFrom32BitRegister(
                            (byte)(computer.MEMC.Fetch() & 0b00011111),
                            computer.MEMC.Fetch32());
                        return;
                    }
                case Mnem.ADDSUB_rrrr_r:     // SUB rrrr, r
                    {
                        byte reg1 = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte reg2 = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        computer.CPU.REGS.SubtractFrom32BitRegister(reg1, computer.CPU.REGS.Get8BitRegister(reg2));
                        return;
                    }
                case Mnem.ADDSUB_rrrr_rr:     // SUB rrrr, rr
                    {
                        byte reg1 = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte reg2 = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        computer.CPU.REGS.SubtractFrom32BitRegister(reg1, computer.CPU.REGS.Get16BitRegister(reg2));
                        return;
                    }
                case Mnem.ADDSUB_rrrr_rrr:     // SUB rrrr, rrr
                    {
                        byte reg1 = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte reg2 = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        computer.CPU.REGS.SubtractFrom32BitRegister(reg1, computer.CPU.REGS.Get24BitRegister(reg2));
                        return;
                    }
                case Mnem.ADDSUB_rrrr_rrrr:     // SUB rrrr, rrrr
                    {
                        byte reg1 = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte reg2 = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        computer.CPU.REGS.SubtractFrom32BitRegister(reg1, computer.CPU.REGS.Get32BitRegister(reg2));
                        return;
                    }
                case Mnem.ADDSUB_rrrr_InnnI:
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.SubtractFrom32BitRegister(register, computer.MEMC.Get8bitFromRAM(address));
                        return;
                    }
                case Mnem.ADDSUB16_rrrr_InnnI:
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.SubtractFrom32BitRegister(register, computer.MEMC.Get16bitFromRAM(address));
                        return;
                    }
                case Mnem.ADDSUB24_rrrr_InnnI:
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.SubtractFrom32BitRegister(register, computer.MEMC.Get24bitFromRAM(address));
                        return;
                    }
                case Mnem.ADDSUB32_rrrr_InnnI:
                    {
                        byte register = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint address = computer.MEMC.Fetch24();

                        computer.CPU.REGS.SubtractFrom32BitRegister(register, computer.MEMC.Get32bitFromRAM(address));
                        return;
                    }
                case Mnem.ADDSUB_rrrr_IrrrI:
                    {
                        byte reg1 = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte reg2 = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        computer.CPU.REGS.SubtractFrom32BitRegister(
                            reg1,
                            computer.MEMC.Get8bitFromRAM(computer.CPU.REGS.Get24BitRegister(reg2)));
                        return;
                    }
                case Mnem.ADDSUB16_rrrr_IrrrI:
                    {
                        byte reg1 = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte reg2 = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        computer.CPU.REGS.SubtractFrom32BitRegister(
                            reg1,
                            computer.MEMC.Get16bitFromRAM(computer.CPU.REGS.Get24BitRegister(reg2)));
                        return;
                    }
                case Mnem.ADDSUB24_rrrr_IrrrI:
                    {
                        byte reg1 = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte reg2 = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        computer.CPU.REGS.SubtractFrom32BitRegister(
                            reg1,
                            computer.MEMC.Get24bitFromRAM(computer.CPU.REGS.Get24BitRegister(reg2)));
                        return;
                    }
                case Mnem.ADDSUB32_rrrr_IrrrI:
                    {
                        byte reg1 = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte reg2 = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        computer.CPU.REGS.SubtractFrom32BitRegister(
                            reg1,
                            computer.MEMC.Get32bitFromRAM(computer.CPU.REGS.Get24BitRegister(reg2)));
                        return;
                    }
                case Mnem.ADDSUB_InnnI_nnn:
                    {
                        uint target = computer.MEMC.Fetch24();

                        computer.MEMC.Set8bitToRAM(
                                target,
                                computer.CPU.REGS.Sub8BitValues(
                                    computer.MEMC.Get8bitFromRAM(target),
                                    computer.MEMC.Fetch()
                                    )
                                );
                        return;
                    }
                case Mnem.ADDSUB16_InnnI_nnn:
                    {
                        uint target = computer.MEMC.Fetch24();

                        computer.MEMC.Set16bitToRAM(
                                target,
                                computer.CPU.REGS.Sub16BitValues(
                                    computer.MEMC.Get16bitFromRAM(target),
                                    computer.MEMC.Fetch16()
                                    )
                                );
                        return;
                    }
                case Mnem.ADDSUB24_InnnI_nnn:
                    {
                        uint target = computer.MEMC.Fetch24();

                        computer.MEMC.Set24bitToRAM(
                                target,
                                computer.CPU.REGS.Sub24BitValues(
                                    computer.MEMC.Get24bitFromRAM(target),
                                    computer.MEMC.Fetch24()
                                    )
                                );
                        return;
                    }
                case Mnem.ADDSUB32_InnnI_nnn:
                    {
                        uint target = computer.MEMC.Fetch24();

                        computer.MEMC.Set32bitToRAM(
                            target,
                            computer.CPU.REGS.Sub32BitValues(
                                computer.MEMC.Get32bitFromRAM(target),
                                computer.MEMC.Fetch32()
                                )
                            );

                        return;
                    }

                case Mnem.ADDSUB_InnnI_r:
                    {
                        uint target = computer.MEMC.Fetch24();
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        computer.MEMC.Set8bitToRAM(
                            target,
                            computer.CPU.REGS.Sub8BitValues(
                                computer.MEMC.Get8bitFromRAM(target),
                                computer.CPU.REGS.Get8BitRegister((byte)(regIndex & 0b00011111))
                                )
                            );
                        return;
                    }
                case Mnem.ADDSUB16_InnnI_r:
                    {
                        uint target = computer.MEMC.Fetch24();
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        computer.MEMC.Set16bitToRAM(
                            target,
                            computer.CPU.REGS.Sub16BitValues(
                                computer.MEMC.Get16bitFromRAM(target),
                                computer.CPU.REGS.Get8BitRegister((byte)(regIndex & 0b00011111))
                                )
                            );
                        return;
                    }
                case Mnem.ADDSUB24_InnnI_r:
                    {
                        uint target = computer.MEMC.Fetch24();
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        computer.MEMC.Set24bitToRAM(
                            target,
                            computer.CPU.REGS.Sub24BitValues(
                                computer.MEMC.Get24bitFromRAM(target),
                                computer.CPU.REGS.Get8BitRegister((byte)(regIndex & 0b00011111))
                                )
                            );
                        return;
                    }
                case Mnem.ADDSUB32_InnnI_r:
                    {
                        uint target = computer.MEMC.Fetch24();
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        computer.MEMC.Set32bitToRAM(
                            target,
                            computer.CPU.REGS.Sub32BitValues(
                                computer.MEMC.Get32bitFromRAM(target),
                                computer.CPU.REGS.Get8BitRegister((byte)(regIndex & 0b00011111))
                                )
                            );
                        return;
                    }
                case Mnem.ADDSUB16_InnnI_rr:
                    {
                        uint target = computer.MEMC.Fetch24();
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        computer.MEMC.Set16bitToRAM(
                            target,
                            computer.CPU.REGS.Sub16BitValues(
                                computer.MEMC.Get16bitFromRAM(target),
                                computer.CPU.REGS.Get16BitRegister((byte)(regIndex & 0b00011111))
                                )
                            );
                        return;
                    }
                case Mnem.ADDSUB24_InnnI_rr:
                    {
                        uint target = computer.MEMC.Fetch24();
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        computer.MEMC.Set24bitToRAM(
                            target,
                            computer.CPU.REGS.Sub24BitValues(
                                computer.MEMC.Get24bitFromRAM(target),
                                computer.CPU.REGS.Get16BitRegister((byte)(regIndex & 0b00011111))
                                )
                            );
                        return;
                    }
                case Mnem.ADDSUB32_InnnI_rr:
                    {
                        uint target = computer.MEMC.Fetch24();
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        computer.MEMC.Set32bitToRAM(
                            target,
                            computer.CPU.REGS.Sub32BitValues(
                                computer.MEMC.Get32bitFromRAM(target),
                                computer.CPU.REGS.Get16BitRegister((byte)(regIndex & 0b00011111))
                                )
                            );
                        return;
                    }
                case Mnem.ADDSUB24_InnnI_rrr:
                    {
                        uint target = computer.MEMC.Fetch24();
                        byte regIndex = computer.MEMC.Fetch();

                        computer.MEMC.Set24bitToRAM(
                            target,
                            computer.CPU.REGS.Sub24BitValues(
                                computer.MEMC.Get24bitFromRAM(target),
                                computer.CPU.REGS.Get24BitRegister((byte)(regIndex & 0b00011111))
                                )
                            );
                        return;
                    }
                case Mnem.ADDSUB32_InnnI_rrr:
                    {
                        uint target = computer.MEMC.Fetch24();
                        byte regIndex = computer.MEMC.Fetch();

                        computer.MEMC.Set32bitToRAM(
                            target,
                            computer.CPU.REGS.Sub32BitValues(
                                computer.MEMC.Get32bitFromRAM(target),
                                computer.CPU.REGS.Get24BitRegister((byte)(regIndex & 0b00011111))
                                )
                            );

                        return;
                    }
                case Mnem.ADDSUB32_InnnI_rrrr:
                    {
                        uint target = computer.MEMC.Fetch24();
                        byte regIndex = computer.MEMC.Fetch();
                        computer.MEMC.Set32bitToRAM(
                                target,
                                computer.CPU.REGS.Sub32BitValues(
                                    computer.MEMC.Get32bitFromRAM(target),
                                    computer.CPU.REGS.Get32BitRegister((byte)(regIndex & 0b00011111))
                                    )
                                );
                        return;
                    }
                case Mnem.ADDSUB_IrrrI_nnn:
                    {
                        uint target = computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.MEMC.Set8bitToRAM(
                            target,
                            computer.CPU.REGS.Sub8BitValues(
                                computer.MEMC.Get8bitFromRAM(target),
                                computer.MEMC.Fetch()
                                )
                            );
                        return;
                    }
                case Mnem.ADDSUB16_IrrrI_nnn:
                    {
                        uint target = computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.MEMC.Set16bitToRAM(
                            target,
                            computer.CPU.REGS.Sub16BitValues(
                                computer.MEMC.Get16bitFromRAM(target),
                                computer.MEMC.Fetch16()
                                )
                            );
                        return;
                    }
                case Mnem.ADDSUB24_IrrrI_nnn:
                    {
                        uint target = computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.MEMC.Set24bitToRAM(
                            target,
                            computer.CPU.REGS.Sub24BitValues(
                                computer.MEMC.Get24bitFromRAM(target),
                                computer.MEMC.Fetch24()
                                )
                            );
                        return;
                    }
                case Mnem.ADDSUB32_IrrrI_nnn:
                    {
                        uint target = computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.MEMC.Set32bitToRAM(
                            target,
                            computer.CPU.REGS.Sub32BitValues(
                                computer.MEMC.Get32bitFromRAM(target),
                                computer.MEMC.Fetch32()
                                )
                            );

                        return;
                    }
                case Mnem.ADDSUB_IrrrI_r:
                    {
                        uint target = computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));
                        byte regValue = computer.CPU.REGS.Get8BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));
                        byte memValue = computer.MEMC.Get8bitFromRAM(target);

                        computer.MEMC.Set8bitToRAM(
                            target,
                            computer.CPU.REGS.Sub8BitValues(memValue, regValue)
                            );
                        return;
                    }
                case Mnem.ADDSUB16_IrrrI_r:
                    {
                        uint target = computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));
                        byte regValue = computer.CPU.REGS.Get8BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));
                        ushort memValue = computer.MEMC.Get16bitFromRAM(target);

                        computer.MEMC.Set16bitToRAM(
                            target,
                            computer.CPU.REGS.Sub16BitValues(memValue, regValue)
                            );
                        return;
                    }
                case Mnem.ADDSUB24_IrrrI_r:
                    {
                        uint target = computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));
                        byte regValue = computer.CPU.REGS.Get8BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));
                        uint memValue = computer.MEMC.Get24bitFromRAM(target);

                        computer.MEMC.Set24bitToRAM(
                            target,
                            computer.CPU.REGS.Sub24BitValues(memValue, regValue)
                            );
                        return;
                    }
                case Mnem.ADDSUB32_IrrrI_r:
                    {
                        uint target = computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));
                        byte regValue = computer.CPU.REGS.Get8BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));
                        uint memValue = computer.MEMC.Get32bitFromRAM(target);

                        computer.MEMC.Set32bitToRAM(
                            target,
                            computer.CPU.REGS.Sub32BitValues(memValue, regValue)
                            );
                        return;
                    }
                case Mnem.ADDSUB16_IrrrI_rr:
                    {
                        uint target = computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));
                        ushort regValue = computer.CPU.REGS.Get16BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));
                        ushort memValue = computer.MEMC.Get16bitFromRAM(target);

                        computer.MEMC.Set16bitToRAM(
                            target,
                            computer.CPU.REGS.Sub16BitValues(memValue, regValue)
                            );
                        return;
                    }
                case Mnem.ADDSUB24_IrrrI_rr:
                    {
                        uint target = computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));
                        ushort regValue = computer.CPU.REGS.Get16BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));
                        uint memValue = computer.MEMC.Get24bitFromRAM(target);

                        computer.MEMC.Set24bitToRAM(
                            target,
                            computer.CPU.REGS.Sub24BitValues(memValue, regValue)
                            );
                        return;
                    }
                case Mnem.ADDSUB32_IrrrI_rr:
                    {
                        uint target = computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));
                        ushort regValue = computer.CPU.REGS.Get16BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));
                        uint memValue = computer.MEMC.Get32bitFromRAM(target);

                        computer.MEMC.Set32bitToRAM(
                            target,
                            computer.CPU.REGS.Sub32BitValues(memValue, regValue)
                            );
                        return;
                    }
                case Mnem.ADDSUB24_IrrrI_rrr:
                    {
                        uint target = computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));
                        uint regValue = computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));
                        uint memValue = computer.MEMC.Get24bitFromRAM(target);

                        computer.MEMC.Set24bitToRAM(
                            target,
                            computer.CPU.REGS.Sub24BitValues(memValue, regValue)
                            );
                        return;
                    }
                case Mnem.ADDSUB32_IrrrI_rrr:
                    {
                        uint target = computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));
                        uint regValue = computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));
                        uint memValue = computer.MEMC.Get32bitFromRAM(target);

                        computer.MEMC.Set32bitToRAM(
                            target,
                            computer.CPU.REGS.Sub32BitValues(memValue, regValue)
                            );
                        return;
                    }
                case Mnem.ADDSUB_IrrrI_rrrr:
                    {
                        uint target = computer.CPU.REGS.Get24BitRegister(computer.MEMC.Fetch());
                        uint regValue = computer.CPU.REGS.Get32BitRegister(computer.MEMC.Fetch());

                        uint memValue = computer.MEMC.Get32bitFromRAM(target);

                        computer.MEMC.Set32bitToRAM(
                                target,
                                computer.CPU.REGS.Sub32BitValues(memValue, regValue)
                                );
                        return;
                    }

                // Floating point ADD
                case Mnem.ADDSUB_fr_fr:
                    {
                        byte fReg1 = (byte)(computer.MEMC.Fetch() & 0b00001111);
                        byte fReg2 = (byte)(computer.MEMC.Fetch() & 0b00001111);

                        float fReg1Value = computer.CPU.FREGS.GetRegister(fReg1);
                        float fReg2Value = computer.CPU.FREGS.GetRegister(fReg2);

                        computer.CPU.FREGS.SetRegister(fReg1,
                            computer.CPU.FREGS.SubFloatValues(fReg1Value, fReg2Value));

                        return;
                    }
                case Mnem.ADDSUB_fr_nnn:
                    {
                        byte fReg = (byte)(computer.MEMC.Fetch() & 0b00001111);
                        uint floatBitValue = computer.MEMC.Fetch32();
                        float floatValue = FloatPointUtils.UintToFloat(floatBitValue);
                        float fRegValue = computer.CPU.FREGS.GetRegister(fReg);

                        computer.CPU.FREGS.SetRegister(fReg,
                            computer.CPU.FREGS.SubFloatValues(fRegValue, floatValue));

                        return;
                    }
                case Mnem.ADDSUB_fr_r:
                    {
                        byte fRegIndex = (byte)(computer.MEMC.Fetch() & 0b_00001111);
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b_00011111);

                        float regValue = computer.CPU.REGS.Get8BitRegister(regIndex);
                        float fRegValue = computer.CPU.FREGS.GetRegister(fRegIndex);

                        computer.CPU.FREGS.SetRegister(fRegIndex,
                            computer.CPU.FREGS.SubFloatValues(fRegValue, regValue));

                        return;
                    }
                case Mnem.ADDSUB_fr_rr:
                    {
                        byte fRegIndex = (byte)(computer.MEMC.Fetch() & 0b_00001111);
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b_00011111);

                        float regValue = computer.CPU.REGS.Get16BitRegister(regIndex);
                        float fRegValue = computer.CPU.FREGS.GetRegister(fRegIndex);

                        computer.CPU.FREGS.SetRegister(fRegIndex,
                            computer.CPU.FREGS.SubFloatValues(fRegValue, regValue));

                        return;
                    }
                case Mnem.ADDSUB_fr_rrr:
                    {
                        byte fRegIndex = (byte)(computer.MEMC.Fetch() & 0b_00001111);
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b_00011111);

                        float regValue = computer.CPU.REGS.Get24BitRegister(regIndex);
                        float fRegValue = computer.CPU.FREGS.GetRegister(fRegIndex);

                        computer.CPU.FREGS.SetRegister(fRegIndex,
                            computer.CPU.FREGS.SubFloatValues(fRegValue, regValue));

                        return;
                    }
                case Mnem.ADDSUB_fr_rrrr:
                    {
                        byte fRegIndex = (byte)(computer.MEMC.Fetch() & 0b_00001111);
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b_00011111);

                        float regValue = computer.CPU.REGS.Get32BitRegister(regIndex);
                        float fRegValue = computer.CPU.FREGS.GetRegister(fRegIndex);

                        computer.CPU.FREGS.SetRegister(fRegIndex,
                            computer.CPU.FREGS.SubFloatValues(fRegValue, regValue));

                        return;
                    }
                case Mnem.ADDSUB_r_fr:
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b_00011111);
                        byte fRegIndex = (byte)(computer.MEMC.Fetch() & 0b_00001111);

                        float fRegValue = computer.CPU.FREGS.GetRegister(fRegIndex);
                        bool isNegative = fRegValue < 0;
                        fRegValue = Math.Abs(fRegValue);

                        byte fConverted;
                        if (fRegValue > 0xFF)
                        {
                            fConverted = (byte)Math.Round(fRegValue % 0x100);
                            computer.CPU.FLAGS.SetOverflow(true);
                        }
                        else
                        {
                            fConverted = (byte)Math.Round(fRegValue);
                            computer.CPU.FLAGS.SetOverflow(false);
                        }

                        if (isNegative)
                        {
                            computer.CPU.REGS.AddTo8BitRegister(regIndex, fConverted);
                        }
                        else
                        {
                            computer.CPU.REGS.SubtractFrom8BitRegister(regIndex, fConverted);
                        }

                        return;
                    }
                case Mnem.ADDSUB_rr_fr:
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b_00011111);
                        byte fRegIndex = (byte)(computer.MEMC.Fetch() & 0b_00001111);

                        float fRegValue = computer.CPU.FREGS.GetRegister(fRegIndex);
                        bool isNegative = fRegValue < 0;
                        fRegValue = Math.Abs(fRegValue);

                        ushort fConverted;
                        if (fRegValue > 0xFFFF)
                        {
                            fConverted = (ushort)Math.Round(fRegValue % 0x10000);
                            computer.CPU.FLAGS.SetOverflow(true);
                        }
                        else
                        {
                            fConverted = (ushort)Math.Round(fRegValue);
                            computer.CPU.FLAGS.SetOverflow(false);
                        }

                        if (isNegative)
                        {
                            computer.CPU.REGS.AddTo16BitRegister(regIndex, fConverted);
                        }
                        else
                        {
                            computer.CPU.REGS.SubtractFrom16BitRegister(regIndex, fConverted);
                        }

                        return;
                    }
                case Mnem.ADDSUB_rrr_fr:
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b_00011111);
                        byte fRegIndex = (byte)(computer.MEMC.Fetch() & 0b_00001111);

                        float fRegValue = computer.CPU.FREGS.GetRegister(fRegIndex);
                        bool isNegative = fRegValue < 0;
                        fRegValue = Math.Abs(fRegValue);

                        uint fConverted;
                        if (fRegValue > 0xFFFFFF)
                        {
                            fConverted = (uint)Math.Round(fRegValue % 0x1000000);
                            computer.CPU.FLAGS.SetOverflow(true);
                        }
                        else
                        {
                            fConverted = (uint)Math.Round(fRegValue);
                            computer.CPU.FLAGS.SetOverflow(false);
                        }

                        if (isNegative)
                        {
                            computer.CPU.REGS.AddTo24BitRegister(regIndex, fConverted);
                        }
                        else
                        {
                            computer.CPU.REGS.SubtractFrom24BitRegister(regIndex, fConverted);
                        }

                        return;
                    }
                case Mnem.ADDSUB_rrrr_fr:
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b_00011111);
                        byte fRegIndex = (byte)(computer.MEMC.Fetch() & 0b_00001111);

                        float fRegValue = computer.CPU.FREGS.GetRegister(fRegIndex);
                        bool isNegative = fRegValue < 0;
                        fRegValue = Math.Abs(fRegValue);

                        uint fConverted;
                        if (fRegValue > 0xFFFFFFFF)
                        {
                            fConverted = (uint)Math.Round(fRegValue % 0x100000000);
                            computer.CPU.FLAGS.SetOverflow(true);
                        }
                        else
                        {
                            fConverted = (uint)Math.Round(fRegValue);
                            computer.CPU.FLAGS.SetOverflow(false);
                        }

                        if (isNegative)
                        {
                            computer.CPU.REGS.AddTo32BitRegister(regIndex, fConverted);
                        }
                        else
                        {
                            computer.CPU.REGS.SubtractFrom32BitRegister(regIndex, fConverted);
                        }

                        return;
                    }
                case Mnem.ADDSUB_fr_InnnI:
                    {
                        byte fReg = (byte)(computer.MEMC.Fetch() & 0b00001111);
                        uint adrFloatPointer = computer.MEMC.Fetch24();
                        float adrFloatValue = computer.MEMC.GetFloatFromRAM(adrFloatPointer);
                        float fRegValue = computer.CPU.FREGS.GetRegister(fReg);

                        computer.CPU.FREGS.SetRegister(fReg,
                            computer.CPU.FREGS.SubFloatValues(fRegValue, adrFloatValue));

                        return;
                    }
                case Mnem.ADDSUB_fr_IrrrI:
                    {
                        byte fRegPointer = (byte)(computer.MEMC.Fetch() & 0b00001111);
                        byte regPointer = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        uint adrFloatPointer = computer.CPU.REGS.Get24BitRegister(regPointer);

                        float adrFloatValue = computer.MEMC.GetFloatFromRAM(adrFloatPointer);
                        float fRegValue = computer.CPU.FREGS.GetRegister(fRegPointer);

                        computer.CPU.FREGS.SetRegister(fRegPointer,
                            computer.CPU.FREGS.SubFloatValues(fRegValue, adrFloatValue));

                        return;
                    }
                case Mnem.ADDSUB_InnnI_fr:
                    {
                        byte fReg = (byte)(computer.MEMC.Fetch() & 0b00001111);
                        uint adrFloatPointer = computer.MEMC.Fetch24();
                        float adrFloatValue = computer.MEMC.GetFloatFromRAM(adrFloatPointer);
                        float fRegValue = computer.CPU.FREGS.GetRegister(fReg);

                        computer.MEMC.SetFloatToRam(adrFloatPointer,
                            computer.CPU.FREGS.SubFloatValues(adrFloatValue, fRegValue));

                        return;
                    }
                case Mnem.ADDSUB_IrrrI_fr:
                    {
                        byte fRegPointer = (byte)(computer.MEMC.Fetch() & 0b00001111);
                        byte regPointer = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        uint adrFloatPointer = computer.CPU.REGS.Get24BitRegister(regPointer);

                        float adrFloatValue = computer.MEMC.GetFloatFromRAM(adrFloatPointer);
                        float fRegValue = computer.CPU.FREGS.GetRegister(fRegPointer);

                        computer.MEMC.SetFloatToRam(adrFloatPointer,
                            computer.CPU.FREGS.SubFloatValues(adrFloatValue, fRegValue));

                        return;
                    }

                case Mnem.ADDSUB_InnnI_InnnI:
                    {
                        uint target = computer.MEMC.Fetch24();
                        uint secondValue = computer.MEMC.Fetch24();

                        computer.MEMC.Set8bitToRAM(
                            target,
                            computer.CPU.REGS.Sub8BitValues(
                                computer.MEMC.Get8bitFromRAM(target),
                                computer.MEMC.Get8bitFromRAM(secondValue)
                                )
                            );
                        return;
                    }
                case Mnem.ADDSUB16_InnnI_InnnI:
                    {
                        uint target = computer.MEMC.Fetch24();
                        uint secondValue = computer.MEMC.Fetch24();

                        computer.MEMC.Set16bitToRAM(
                            target,
                            computer.CPU.REGS.Sub16BitValues(
                                computer.MEMC.Get16bitFromRAM(target),
                                computer.MEMC.Get16bitFromRAM(secondValue)
                                )
                            );
                        return;
                    }
                case Mnem.ADDSUB24_InnnI_InnnI:
                    {
                        uint target = computer.MEMC.Fetch24();
                        uint secondValue = computer.MEMC.Fetch24();

                        computer.MEMC.Set24bitToRAM(
                            target,
                            computer.CPU.REGS.Sub24BitValues(
                                computer.MEMC.Get24bitFromRAM(target),
                                computer.MEMC.Get24bitFromRAM(secondValue)
                                )
                            );
                        return;
                    }
                case Mnem.ADDSUB32_InnnI_InnnI:
                    {
                        uint target = computer.MEMC.Fetch24();
                        uint secondValue = computer.MEMC.Fetch24();

                        computer.MEMC.Set32bitToRAM(
                            target,
                            computer.CPU.REGS.Sub32BitValues(
                                computer.MEMC.Get32bitFromRAM(target),
                                computer.MEMC.Get32bitFromRAM(secondValue)
                                )
                            );
                        return;
                    }
            }
        }
    }
}
