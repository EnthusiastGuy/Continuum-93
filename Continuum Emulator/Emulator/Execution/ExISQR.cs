using Continuum93.Emulator.Mnemonics;
using Continuum93.Tools;

namespace Continuum93.Emulator.Execution
{
    public static class ExISQR
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 2);

            switch (upperLdOp)
            {
                case Mnem.SQRCR_fr:   // ISQR fr
                    {
                        byte reg = computer.MEMC.Fetch();
                        byte fRegPointer = (byte)(reg & 0b_0000_1111);
                        float fRegValue = computer.CPU.FREGS.GetRegister(fRegPointer);

                        computer.CPU.FREGS.SetRegister(fRegPointer, CMath.InverseSqrt(fRegValue));
                        return;
                    }
                case Mnem.SQRCR_r:   // ISQR r
                    {
                        byte reg = computer.MEMC.Fetch();
                        byte regPointer = (byte)(reg & 0b_000_11111);
                        float fRegValue = computer.CPU.REGS.Get8BitRegister(regPointer);

                        computer.CPU.REGS.Set8BitRegister(regPointer, (byte)CMath.InverseSqrt(fRegValue));
                        return;
                    }
                case Mnem.SQRCR_rr:   // ISQR rr
                    {
                        byte reg = computer.MEMC.Fetch();
                        byte regPointer = (byte)(reg & 0b_000_11111);
                        float fRegValue = computer.CPU.REGS.Get16BitRegister(regPointer);

                        computer.CPU.REGS.Set16BitRegister(regPointer, (ushort)CMath.InverseSqrt(fRegValue));
                        return;
                    }
                case Mnem.SQRCR_rrr:   // ISQR rrr
                    {
                        byte reg = computer.MEMC.Fetch();
                        byte regPointer = (byte)(reg & 0b_000_11111);
                        float fRegValue = computer.CPU.REGS.Get24BitRegister(regPointer);

                        computer.CPU.REGS.Set24BitRegister(regPointer, (uint)CMath.InverseSqrt(fRegValue));
                        return;
                    }
                case Mnem.SQRCR_rrrr:   // ISQR rrrr
                    {
                        byte reg = computer.MEMC.Fetch();
                        byte regPointer = (byte)(reg & 0b_000_11111);
                        float fRegValue = computer.CPU.REGS.Get32BitRegister(regPointer);

                        computer.CPU.REGS.Set32BitRegister(regPointer, (uint)CMath.InverseSqrt(fRegValue));
                        return;
                    }
                case Mnem.SQRCR_IrrrI:   // ISQR (rrr)
                    {
                        byte reg = computer.MEMC.Fetch();
                        byte regPointer = (byte)(reg & 0b_000_11111);
                        uint regAddress = computer.CPU.REGS.Get24BitRegister(regPointer);
                        float fRegValue = computer.MEMC.GetFloatFromRAM(regAddress);

                        computer.MEMC.SetFloatToRam(regAddress, CMath.InverseSqrt(fRegValue));
                        return;
                    }
                case Mnem.SQRCR_InnnI:   // ISQR (nnn)
                    {
                        uint fValueAddress = computer.MEMC.Fetch24();
                        float fRegValue = computer.MEMC.GetFloatFromRAM(fValueAddress);

                        computer.MEMC.SetFloatToRam(fValueAddress, CMath.InverseSqrt(fRegValue));
                        return;
                    }
                case Mnem.SQRCR_fr_fr:   // ISQR fr, fr
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        byte fReg1 = (byte)(mixedReg >> 4);
                        byte fReg2 = (byte)(mixedReg & 0b00001111);

                        float fReg2Value = computer.CPU.FREGS.GetRegister(fReg2);

                        computer.CPU.FREGS.SetRegister(fReg1, CMath.InverseSqrt(fReg2Value));
                        return;
                    }
                case Mnem.SQRCR_fr_r:   // ISQR fr, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        byte r1Index = (byte)(((ldOp << 5) & 0b00011111) + (mixedReg >> 4));
                        byte fRegIndex = (byte)(mixedReg & 0b00001111);

                        byte reg1Val = computer.CPU.REGS.Get8BitRegister(r1Index);

                        computer.CPU.FREGS.SetRegister(fRegIndex, CMath.InverseSqrt(reg1Val));
                        return;
                    }
                case Mnem.SQRCR_fr_rr:   // ISQR fr, rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        byte r1Index = (byte)(((ldOp << 5) & 0b00011111) + (mixedReg >> 4));
                        byte fRegIndex = (byte)(mixedReg & 0b00001111);

                        ushort reg1Val = computer.CPU.REGS.Get16BitRegister(r1Index);

                        computer.CPU.FREGS.SetRegister(fRegIndex, CMath.InverseSqrt(reg1Val));
                        return;
                    }
                case Mnem.SQRCR_fr_rrr:   // ISQR fr, rrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        byte r1Index = (byte)(((ldOp << 5) & 0b00011111) + (mixedReg >> 4));
                        byte fRegIndex = (byte)(mixedReg & 0b00001111);

                        uint reg1Val = computer.CPU.REGS.Get24BitRegister(r1Index);

                        computer.CPU.FREGS.SetRegister(fRegIndex, CMath.InverseSqrt(reg1Val));
                        return;
                    }
                case Mnem.SQRCR_fr_rrrr:   // ISQR fr, rrrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        byte r1Index = (byte)(((ldOp << 5) & 0b00011111) + (mixedReg >> 4));
                        byte fRegIndex = (byte)(mixedReg & 0b00001111);

                        uint reg1Val = computer.CPU.REGS.Get32BitRegister(r1Index);

                        computer.CPU.FREGS.SetRegister(fRegIndex, CMath.InverseSqrt(reg1Val));
                        return;
                    }
                case Mnem.SQRCR_fr_IrrrI:   // ISQR fr, (rrr)
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        byte r1Index = (byte)(((ldOp << 5) & 0b00011111) + (mixedReg >> 4));
                        byte fRegIndex = (byte)(mixedReg & 0b00001111);

                        uint regAddress = computer.CPU.REGS.Get24BitRegister(r1Index);
                        float fRegValue = computer.MEMC.GetFloatFromRAM(regAddress);

                        computer.CPU.FREGS.SetRegister(fRegIndex, CMath.InverseSqrt(fRegValue));
                        return;
                    }
                case Mnem.SQRCR_fr_InnnI:   // ISQR fr, (nnn)
                    {
                        byte reg = computer.MEMC.Fetch();

                        byte fRegIndex = (byte)(reg & 0b00001111);
                        uint fValueAddress = computer.MEMC.Fetch24();
                        float fRegValue = computer.MEMC.GetFloatFromRAM(fValueAddress);

                        computer.CPU.FREGS.SetRegister(fRegIndex, CMath.InverseSqrt(fRegValue));
                        return;
                    }
                case Mnem.SQRCR_fr_n:   // ISQR fr, n
                    {
                        byte reg = computer.MEMC.Fetch();

                        byte fRegIndex = (byte)(reg & 0b00001111);
                        uint floatBitValue = computer.MEMC.Fetch32();
                        float floatValue = FloatPointUtils.UintToFloat(floatBitValue);

                        computer.CPU.FREGS.SetRegister(fRegIndex, CMath.InverseSqrt(floatValue));
                        return;
                    }
                case Mnem.SQRCR_r_fr:   // ISQR r, fr
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        byte r1Index = (byte)(((ldOp << 5) & 0b00011111) + (mixedReg >> 4));
                        byte fRegIndex = (byte)(mixedReg & 0b00001111);
                        float fRegVal = computer.CPU.FREGS.GetRegister(fRegIndex);

                        float result = CMath.InverseSqrt(fRegVal);
                        if (result < 0x100)
                        {
                            computer.CPU.REGS.Set8BitRegister(r1Index, (byte)result);
                            computer.CPU.FLAGS.SetOverflow(false);
                        }
                        else
                        {
                            computer.CPU.FLAGS.SetOverflow(true);
                        }

                        return;
                    }
                case Mnem.SQRCR_rr_fr:   // ISQR rr, fr
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        byte r1Index = (byte)(((ldOp << 5) & 0b00011111) + (mixedReg >> 4));
                        byte fRegIndex = (byte)(mixedReg & 0b00001111);
                        float fRegVal = computer.CPU.FREGS.GetRegister(fRegIndex);

                        float result = CMath.InverseSqrt(fRegVal);
                        if (result < 0x10000)
                        {
                            computer.CPU.REGS.Set16BitRegister(r1Index, (ushort)result);
                            computer.CPU.FLAGS.SetOverflow(false);
                        }
                        else
                        {
                            computer.CPU.FLAGS.SetOverflow(true);
                        }

                        return;
                    }
                case Mnem.SQRCR_rrr_fr:   // ISQR rrr, fr
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        byte r1Index = (byte)(((ldOp << 5) & 0b00011111) + (mixedReg >> 4));
                        byte fRegIndex = (byte)(mixedReg & 0b00001111);
                        float fRegVal = computer.CPU.FREGS.GetRegister(fRegIndex);

                        float result = CMath.InverseSqrt(fRegVal);
                        if (result < 0x1000000)
                        {
                            computer.CPU.REGS.Set24BitRegister(r1Index, (uint)result);
                            computer.CPU.FLAGS.SetOverflow(false);
                        }
                        else
                        {
                            computer.CPU.FLAGS.SetOverflow(true);
                        }

                        return;
                    }
                case Mnem.SQRCR_rrrr_fr:   // ISQR rrrr, fr
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        byte r1Index = (byte)(((ldOp << 5) & 0b00011111) + (mixedReg >> 4));
                        byte fRegIndex = (byte)(mixedReg & 0b00001111);
                        float fRegVal = computer.CPU.FREGS.GetRegister(fRegIndex);

                        float result = CMath.InverseSqrt(fRegVal);
                        if (result < 0x100000000)
                        {
                            computer.CPU.REGS.Set32BitRegister(r1Index, (uint)result);
                            computer.CPU.FLAGS.SetOverflow(false);
                        }
                        else
                        {
                            computer.CPU.FLAGS.SetOverflow(true);
                        }

                        return;
                    }
                case Mnem.SQRCR_IrrrI_fr:   // ISQR (rrr), fr
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        byte r1Index = (byte)(((ldOp << 5) & 0b00011111) + (mixedReg >> 4));
                        byte fRegIndex = (byte)(mixedReg & 0b00001111);

                        uint regAddress = computer.CPU.REGS.Get24BitRegister(r1Index);
                        float fRegValue = computer.CPU.FREGS.GetRegister(fRegIndex);

                        computer.MEMC.SetFloatToRam(regAddress, CMath.InverseSqrt(fRegValue));
                        return;
                    }
                case Mnem.SQRCR_InnnI_fr:   // ISQR (nnn), fr
                    {
                        byte reg = computer.MEMC.Fetch();

                        byte fRegIndex = (byte)(reg & 0b00001111);
                        uint fValueAddress = computer.MEMC.Fetch24();
                        float fRegValue = computer.CPU.FREGS.GetRegister(fRegIndex);

                        computer.MEMC.SetFloatToRam(fValueAddress, CMath.InverseSqrt(fRegValue));
                        return;
                    }
            }
        }
    }
}
