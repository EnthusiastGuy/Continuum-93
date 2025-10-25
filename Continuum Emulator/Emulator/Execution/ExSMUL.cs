using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public static class ExSMUL
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 2);
            switch (upperLdOp)
            {
                case Mnem.DIVMUL_r_n:  // SMUL r, n
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        sbyte dividend = computer.CPU.REGS.Get8BitRegisterSigned(dividendReg);

                        ushort combinedValue = (ushort)(((mixedReg & 0b00011111) << 8) + computer.MEMC.Fetch());

                        short quotientValue;

                        if ((combinedValue & 0x1000) != 0)
                        {
                            combinedValue |= 0xF000;
                        }

                        quotientValue = (short)combinedValue;

                        if (quotientValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            computer.CPU.REGS.Set8BitRegisterSigned(dividendReg, (sbyte)(dividend * quotientValue));
                        }

                        return;
                    }
                case Mnem.DIVMUL_r_r:  // SMUL r, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        sbyte dividendValue = computer.CPU.REGS.Get8BitRegisterSigned(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        sbyte quotientValue = computer.CPU.REGS.Get8BitRegisterSigned(quotientReg);

                        if (quotientValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            computer.CPU.REGS.Set8BitRegisterSigned(dividendReg, (sbyte)(dividendValue * quotientValue));
                        }

                        return;
                    }
                case Mnem.DIVMUL_rr_n:  // SMUL rr, n
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        short dividend = computer.CPU.REGS.Get16BitRegisterSigned(dividendReg);

                        ushort combinedValue = (ushort)(((mixedReg & 0b00011111) << 8) + computer.MEMC.Fetch());

                        short quotientValue;

                        if ((combinedValue & 0x1000) != 0)
                        {
                            combinedValue |= 0xF000;
                        }

                        quotientValue = (short)combinedValue;

                        if (quotientValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            computer.CPU.REGS.Set16BitRegisterSigned(dividendReg, (short)(dividend * quotientValue));
                        }
                        return;
                    }
                case Mnem.DIVMUL_rr_r:  // SMUL rr, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        short dividendValue = computer.CPU.REGS.Get16BitRegisterSigned(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        sbyte quotientValue = computer.CPU.REGS.Get8BitRegisterSigned(quotientReg);

                        if (quotientValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            computer.CPU.REGS.Set16BitRegisterSigned(dividendReg, (short)(dividendValue * quotientValue));
                        }

                        return;
                    }
                case Mnem.DIVMUL_rr_rr:  // SMUL rr, rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        short dividendValue = computer.CPU.REGS.Get16BitRegisterSigned(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        short quotientValue = computer.CPU.REGS.Get16BitRegisterSigned(quotientReg);

                        if (quotientValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            computer.CPU.REGS.Set16BitRegisterSigned(dividendReg, (short)(dividendValue * quotientValue));
                        }

                        return;
                    }
                case Mnem.DIVMUL_rrr_n:  // SMUL rrr, n
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        int dividend = computer.CPU.REGS.Get24BitRegisterSigned(dividendReg);
                        ushort combinedValue = (ushort)(((mixedReg & 0b00011111) << 8) + computer.MEMC.Fetch());

                        short quotientValue;

                        if ((combinedValue & 0x1000) != 0)
                        {
                            combinedValue |= 0xF000;
                        }

                        quotientValue = (short)combinedValue;

                        if (quotientValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            computer.CPU.REGS.Set24BitRegisterSigned(dividendReg, dividend * quotientValue);
                        }
                        return;
                    }
                case Mnem.DIVMUL_rrr_r:  // SMUL rrr, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        int dividendValue = computer.CPU.REGS.Get24BitRegisterSigned(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        sbyte quotientValue = computer.CPU.REGS.Get8BitRegisterSigned(quotientReg);

                        if (quotientValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            computer.CPU.REGS.Set24BitRegisterSigned(dividendReg, dividendValue * quotientValue);
                        }

                        return;
                    }
                case Mnem.DIVMUL_rrr_rr:  // SMUL rrr, rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        int dividendValue = computer.CPU.REGS.Get24BitRegisterSigned(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        short quotientValue = computer.CPU.REGS.Get16BitRegisterSigned(quotientReg);

                        if (quotientValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            computer.CPU.REGS.Set24BitRegisterSigned(dividendReg, dividendValue * quotientValue);
                        }

                        return;
                    }
                case Mnem.DIVMUL_rrr_rrr:  // SMUL rrr, rrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        int dividendValue = computer.CPU.REGS.Get24BitRegisterSigned(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        int quotientValue = computer.CPU.REGS.Get24BitRegisterSigned(quotientReg);

                        if (quotientValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            computer.CPU.REGS.Set24BitRegisterSigned(dividendReg, dividendValue * quotientValue);
                        }

                        return;
                    }
                case Mnem.DIVMUL_rrrr_n:  // SMUL rrrr, n
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        int dividend = computer.CPU.REGS.Get32BitRegisterSigned(dividendReg);

                        ushort combinedValue = (ushort)(((mixedReg & 0b00011111) << 8) + computer.MEMC.Fetch());

                        short quotientValue;

                        if ((combinedValue & 0x1000) != 0)
                        {
                            combinedValue |= 0xF000;
                        }

                        quotientValue = (short)combinedValue;

                        if (quotientValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            computer.CPU.REGS.Set32BitRegisterSigned(dividendReg, dividend * quotientValue);
                        }
                        return;
                    }
                case Mnem.DIVMUL_rrrr_r:  // SMUL rrrr, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        int dividendValue = computer.CPU.REGS.Get32BitRegisterSigned(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        sbyte quotientValue = computer.CPU.REGS.Get8BitRegisterSigned(quotientReg);

                        if (quotientValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            computer.CPU.REGS.Set32BitRegisterSigned(dividendReg, dividendValue * quotientValue);
                        }

                        return;
                    }
                case Mnem.DIVMUL_rrrr_rr:  // SMUL rrrr, rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        int dividendValue = computer.CPU.REGS.Get32BitRegisterSigned(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        short quotientValue = computer.CPU.REGS.Get16BitRegisterSigned(quotientReg);

                        if (quotientValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            computer.CPU.REGS.Set32BitRegisterSigned(dividendReg, dividendValue * quotientValue);
                        }

                        return;
                    }
                case Mnem.DIVMUL_rrrr_rrr:  // SMUL rrrr, rrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        int dividendValue = computer.CPU.REGS.Get32BitRegisterSigned(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        int quotientValue = computer.CPU.REGS.Get24BitRegisterSigned(quotientReg);

                        if (quotientValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            computer.CPU.REGS.Set32BitRegisterSigned(dividendReg, dividendValue * quotientValue);
                        }

                        return;
                    }
                case Mnem.DIVMUL_rrrr_rrrr:  // MUL rrrr, rrrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        int dividendValue = computer.CPU.REGS.Get32BitRegisterSigned(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        int quotientValue = computer.CPU.REGS.Get32BitRegisterSigned(quotientReg);

                        if (quotientValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            computer.CPU.REGS.Set32BitRegisterSigned(dividendReg, dividendValue * quotientValue);
                        }

                        return;
                    }
            }
        }
    }
}
