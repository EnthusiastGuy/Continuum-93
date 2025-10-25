using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public static class ExSDIV
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 2);

            switch (upperLdOp)
            {
                // No remainder divisions
                case Mnem.DIVMUL_r_n:  // SDIV r, n
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
                            computer.CPU.REGS.Set8BitRegisterSigned(dividendReg, (sbyte)(dividend / quotientValue));
                        }

                        return;
                    }
                case Mnem.DIVMUL_r_r:  // SDIV r, r
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
                            computer.CPU.REGS.Set8BitRegisterSigned(dividendReg, (sbyte)(dividendValue / quotientValue));
                        }

                        return;
                    }
                case Mnem.DIVMUL_rr_n:  // SDIV rr, n
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
                            computer.CPU.REGS.Set16BitRegisterSigned(dividendReg, (short)(dividend / quotientValue));
                        }
                        return;
                    }
                case Mnem.DIVMUL_rr_r:  // SDIV rr, r
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
                            computer.CPU.REGS.Set16BitRegisterSigned(dividendReg, (short)(dividendValue / quotientValue));
                        }

                        return;
                    }
                case Mnem.DIVMUL_rr_rr:  // SDIV rr, rr
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
                            computer.CPU.REGS.Set16BitRegisterSigned(dividendReg, (short)(dividendValue / quotientValue));
                        }

                        return;
                    }
                case Mnem.DIVMUL_rrr_n:  // SDIV rrr, n
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
                            computer.CPU.REGS.Set24BitRegisterSigned(dividendReg, dividend / quotientValue);
                        }
                        return;
                    }
                case Mnem.DIVMUL_rrr_r:  // SDIV rrr, r
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
                            computer.CPU.REGS.Set24BitRegisterSigned(dividendReg, dividendValue / quotientValue);
                        }

                        return;
                    }
                case Mnem.DIVMUL_rrr_rr:  // SDIV rrr, rr
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
                            computer.CPU.REGS.Set24BitRegisterSigned(dividendReg, dividendValue / quotientValue);
                        }

                        return;
                    }
                case Mnem.DIVMUL_rrr_rrr:  // SDIV rrr, rrr
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
                            computer.CPU.REGS.Set24BitRegisterSigned(dividendReg, dividendValue / quotientValue);
                        }

                        return;
                    }
                case Mnem.DIVMUL_rrrr_n:  // SDIV rrrr, n
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
                            computer.CPU.REGS.Set32BitRegisterSigned(dividendReg, dividend / quotientValue);
                        }
                        return;
                    }
                case Mnem.DIVMUL_rrrr_r:  // SDIV rrrr, r
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
                            computer.CPU.REGS.Set32BitRegisterSigned(dividendReg, dividendValue / quotientValue);
                        }

                        return;
                    }
                case Mnem.DIVMUL_rrrr_rr:  // SDIV rrrr, rr
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
                            computer.CPU.REGS.Set32BitRegisterSigned(dividendReg, dividendValue / quotientValue);
                        }

                        return;
                    }
                case Mnem.DIVMUL_rrrr_rrr:  // SDIV rrrr, rrr
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
                            computer.CPU.REGS.Set32BitRegisterSigned(dividendReg, dividendValue / quotientValue);
                        }

                        return;
                    }
                case Mnem.DIVMUL_rrrr_rrrr:  // SDIV rrrr, rrrr
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
                            computer.CPU.REGS.Set32BitRegisterSigned(dividendReg, dividendValue / quotientValue);
                        }

                        return;
                    }

                // Remainder divisions
                case Mnem.DIV_r_n_r:  // SDIV r, n, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        sbyte dividendValue = computer.CPU.REGS.Get8BitRegisterSigned(dividendReg);
                        byte remaindertReg = (byte)(mixedReg & 0b00011111);
                        sbyte quotientValue = computer.MEMC.FetchSigned();

                        if (quotientValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            computer.CPU.REGS.Set8BitRegisterSigned(dividendReg, (sbyte)(dividendValue / quotientValue));
                            computer.CPU.REGS.Set8BitRegisterSigned(remaindertReg, (sbyte)(dividendValue % quotientValue));
                        }

                        return;
                    }
                case Mnem.DIV_r_r_r:  // SDIV r, r, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        sbyte dividendValue = computer.CPU.REGS.Get8BitRegisterSigned(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        sbyte quotientValue = computer.CPU.REGS.Get8BitRegisterSigned(quotientReg);

                        byte remaindertReg = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        if (quotientValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            computer.CPU.REGS.Set8BitRegisterSigned(dividendReg, (sbyte)(dividendValue / quotientValue));
                            computer.CPU.REGS.Set8BitRegisterSigned(remaindertReg, (sbyte)(dividendValue % quotientValue));
                        }

                        return;
                    }

                case Mnem.DIV_rr_n_rr:  // SDIV rr, n, rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        short dividendValue = computer.CPU.REGS.Get16BitRegisterSigned(dividendReg);
                        byte remaindertReg = (byte)(mixedReg & 0b00011111);

                        short quotientValue = (short)((computer.MEMC.Fetch() << 8) + computer.MEMC.Fetch());

                        if (quotientValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            computer.CPU.REGS.Set16BitRegisterSigned(dividendReg, (short)(dividendValue / quotientValue));
                            computer.CPU.REGS.Set16BitRegisterSigned(remaindertReg, (short)(dividendValue % quotientValue));
                        }

                        return;
                    }
                case Mnem.DIV_rr_r_r:  // SDIV rr, r, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        short dividendValue = computer.CPU.REGS.Get16BitRegisterSigned(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        sbyte quotientValue = computer.CPU.REGS.Get8BitRegisterSigned(quotientReg);

                        byte remaindertReg = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        if (quotientValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            computer.CPU.REGS.Set16BitRegisterSigned(dividendReg, (short)(dividendValue / quotientValue));
                            computer.CPU.REGS.Set8BitRegisterSigned(remaindertReg, (sbyte)(dividendValue % quotientValue));
                        }

                        return;
                    }
                case Mnem.DIV_rr_rr_rr:  // SDIV rr, rr, rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        short dividendValue = computer.CPU.REGS.Get16BitRegisterSigned(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        short quotientValue = computer.CPU.REGS.Get16BitRegisterSigned(quotientReg);

                        byte remaindertReg = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        if (quotientValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            computer.CPU.REGS.Set16BitRegisterSigned(dividendReg, (short)(dividendValue / quotientValue));
                            computer.CPU.REGS.Set16BitRegisterSigned(remaindertReg, (short)(dividendValue % quotientValue));
                        }

                        return;
                    }
                case Mnem.DIV_rrr_n_rr:  // SDIV rrr, n, rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        int dividendValue = computer.CPU.REGS.Get24BitRegisterSigned(dividendReg);
                        byte remaindertReg = (byte)(mixedReg & 0b00011111);

                        short quotientValue = (short)((computer.MEMC.Fetch() << 8) + computer.MEMC.Fetch());

                        if (quotientValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            computer.CPU.REGS.Set24BitRegisterSigned(dividendReg, dividendValue / quotientValue);
                            computer.CPU.REGS.Set16BitRegisterSigned(remaindertReg, (short)(dividendValue % quotientValue));
                        }

                        return;
                    }
                case Mnem.DIV_rrr_r_r:  // DIV rrr, r, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        int dividendValue = computer.CPU.REGS.Get24BitRegisterSigned(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        sbyte quotientValue = computer.CPU.REGS.Get8BitRegisterSigned(quotientReg);

                        byte remaindertReg = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        if (quotientValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            computer.CPU.REGS.Set24BitRegisterSigned(dividendReg, dividendValue / quotientValue);
                            computer.CPU.REGS.Set8BitRegisterSigned(remaindertReg, (sbyte)(dividendValue % quotientValue));
                        }

                        return;
                    }
                case Mnem.DIV_rrr_rr_rr:  // DIV rrr, rr, rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        int dividendValue = computer.CPU.REGS.Get24BitRegisterSigned(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        short quotientValue = computer.CPU.REGS.Get16BitRegisterSigned(quotientReg);

                        byte remaindertReg = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        if (quotientValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            computer.CPU.REGS.Set24BitRegisterSigned(dividendReg, dividendValue / quotientValue);
                            computer.CPU.REGS.Set16BitRegisterSigned(remaindertReg, (short)(dividendValue % quotientValue));
                        }

                        return;
                    }
                case Mnem.DIV_rrr_rrr_rrr:  // DIV rrr, rrr, rrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        int dividendValue = computer.CPU.REGS.Get24BitRegisterSigned(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        int quotientValue = computer.CPU.REGS.Get24BitRegisterSigned(quotientReg);

                        byte remaindertReg = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        if (quotientValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            computer.CPU.REGS.Set24BitRegisterSigned(dividendReg, dividendValue / quotientValue);
                            computer.CPU.REGS.Set24BitRegisterSigned(remaindertReg, dividendValue % quotientValue);
                        }

                        return;
                    }
                case Mnem.DIV_rrrr_n_rr:  // DIV rrrr, n, rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        int dividendValue = computer.CPU.REGS.Get32BitRegisterSigned(dividendReg);
                        byte remaindertReg = (byte)(mixedReg & 0b00011111);

                        short quotientValue = (short)((computer.MEMC.Fetch() << 8) + computer.MEMC.Fetch());

                        if (quotientValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            computer.CPU.REGS.Set32BitRegisterSigned(dividendReg, dividendValue / quotientValue);
                            computer.CPU.REGS.Set16BitRegisterSigned(remaindertReg, (short)(dividendValue % quotientValue));
                        }

                        return;
                    }
                case Mnem.DIV_rrrr_r_r:  // DIV rrrr, r, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        int dividendValue = computer.CPU.REGS.Get32BitRegisterSigned(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        sbyte quotientValue = computer.CPU.REGS.Get8BitRegisterSigned(quotientReg);

                        byte remaindertReg = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        if (quotientValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            computer.CPU.REGS.Set32BitRegisterSigned(dividendReg, dividendValue / quotientValue);
                            computer.CPU.REGS.Set8BitRegisterSigned(remaindertReg, (sbyte)(dividendValue % quotientValue));
                        }

                        return;
                    }
                case Mnem.DIV_rrrr_rr_rr:  // DIV rrrr, rr, rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        int dividendValue = computer.CPU.REGS.Get32BitRegisterSigned(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        short quotientValue = computer.CPU.REGS.Get16BitRegisterSigned(quotientReg);

                        byte remaindertReg = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        if (quotientValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            computer.CPU.REGS.Set32BitRegisterSigned(dividendReg, dividendValue / quotientValue);
                            computer.CPU.REGS.Set16BitRegisterSigned(remaindertReg, (short)(dividendValue % quotientValue));
                        }

                        return;
                    }
                case Mnem.DIV_rrrr_rrr_rrr:  // DIV rrrr, rrr, rrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        int dividendValue = computer.CPU.REGS.Get32BitRegisterSigned(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        int quotientValue = computer.CPU.REGS.Get24BitRegisterSigned(quotientReg);

                        byte remaindertReg = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        if (quotientValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            computer.CPU.REGS.Set32BitRegisterSigned(dividendReg, dividendValue / quotientValue);
                            computer.CPU.REGS.Set24BitRegisterSigned(remaindertReg, dividendValue % quotientValue);

                        }

                        return;
                    }
                case Mnem.DIV_rrrr_rrrr_rrrr:  // DIV rrrr, rrrr, rrrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        int dividendValue = computer.CPU.REGS.Get32BitRegisterSigned(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        int quotientValue = computer.CPU.REGS.Get32BitRegisterSigned(quotientReg);

                        byte remaindertReg = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        if (quotientValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            computer.CPU.REGS.Set32BitRegisterSigned(dividendReg, dividendValue / quotientValue);
                            computer.CPU.REGS.Set32BitRegisterSigned(remaindertReg, dividendValue % quotientValue);

                        }

                        return;
                    }
            }
        }
    }
}
