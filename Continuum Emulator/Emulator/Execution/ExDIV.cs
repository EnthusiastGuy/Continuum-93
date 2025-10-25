using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;
using Continuum93.Tools;
using System;

namespace Continuum93.Emulator.Execution
{
    public static class ExDIV
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 2);
            switch (upperLdOp)
            {
                // No remainder divisions
                case Mnem.DIVMUL_r_n:  // DIV r, n
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte dividend = computer.CPU.REGS.Get8BitRegister(dividendReg);
                        ushort quotientValue = (ushort)(((byte)(mixedReg & 0b00011111) << 8) + computer.MEMC.Fetch());

                        if (quotientValue.Equals(0))
                        {
                            computer.CPU.REGS.Set8BitRegister(dividendReg, byte.MaxValue);
                            computer.CPU.FLAGS.SetCarry(true);
                        }
                        else
                        {
                            computer.CPU.REGS.Set8BitRegister(dividendReg, (byte)(dividend / quotientValue));
                            computer.CPU.FLAGS.SetCarry(false);
                        }

                        return;
                    }
                case Mnem.DIVMUL_r_r:  // DIV r, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte dividendValue = computer.CPU.REGS.Get8BitRegister(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        byte quotientValue = computer.CPU.REGS.Get8BitRegister(quotientReg);

                        if (quotientValue.Equals(0))
                        {
                            computer.CPU.REGS.Set8BitRegister(dividendReg, byte.MaxValue);
                            computer.CPU.FLAGS.SetCarry(true);
                        }
                        else
                        {
                            computer.CPU.REGS.Set8BitRegister(dividendReg, (byte)(dividendValue / quotientValue));
                            computer.CPU.FLAGS.SetCarry(false);
                        }

                        return;
                    }
                case Mnem.DIVMUL_rr_n:  // DIV rr, n
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        ushort dividend = computer.CPU.REGS.Get16BitRegister(dividendReg);
                        ushort quotientValue = (ushort)(((byte)(mixedReg & 0b00011111) << 8) + computer.MEMC.Fetch());

                        if (quotientValue.Equals(0))
                        {
                            computer.CPU.REGS.Set16BitRegister(dividendReg, ushort.MaxValue);
                            computer.CPU.FLAGS.SetCarry(true);
                        }
                        else
                        {
                            computer.CPU.REGS.Set16BitRegister(dividendReg, (ushort)(dividend / quotientValue));
                            computer.CPU.FLAGS.SetCarry(false);
                        }
                        return;
                    }
                case Mnem.DIVMUL_rr_r:  // DIV rr, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        ushort dividendValue = computer.CPU.REGS.Get16BitRegister(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        byte quotientValue = computer.CPU.REGS.Get8BitRegister(quotientReg);

                        if (quotientValue.Equals(0))
                        {
                            computer.CPU.REGS.Set16BitRegister(dividendReg, ushort.MaxValue);
                            computer.CPU.FLAGS.SetCarry(true);
                        }
                        else
                        {
                            computer.CPU.REGS.Set16BitRegister(dividendReg, (ushort)(dividendValue / quotientValue));
                            computer.CPU.FLAGS.SetCarry(false);
                        }

                        return;
                    }
                case Mnem.DIVMUL_rr_rr:  // DIV rr, rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        ushort dividendValue = computer.CPU.REGS.Get16BitRegister(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        ushort quotientValue = computer.CPU.REGS.Get16BitRegister(quotientReg);

                        if (quotientValue.Equals(0))
                        {
                            computer.CPU.REGS.Set16BitRegister(dividendReg, ushort.MaxValue);
                            computer.CPU.FLAGS.SetCarry(true);
                        }
                        else
                        {
                            computer.CPU.REGS.Set16BitRegister(dividendReg, (ushort)(dividendValue / quotientValue));
                            computer.CPU.FLAGS.SetCarry(false);
                        }

                        return;
                    }
                case Mnem.DIVMUL_rrr_n:  // DIV rrr, n
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        uint dividend = computer.CPU.REGS.Get24BitRegister(dividendReg);
                        uint quotientValue = (ushort)(((byte)(mixedReg & 0b00011111) << 8) + computer.MEMC.Fetch());

                        if (quotientValue.Equals(0))
                        {
                            computer.CPU.REGS.Set24BitRegister(dividendReg, 0xFFFFFF);
                            computer.CPU.FLAGS.SetCarry(true);
                        }
                        else
                        {
                            computer.CPU.REGS.Set24BitRegister(dividendReg, dividend / quotientValue);
                            computer.CPU.FLAGS.SetCarry(false);
                        }
                        return;
                    }
                case Mnem.DIVMUL_rrr_r:  // DIV rrr, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        uint dividendValue = computer.CPU.REGS.Get24BitRegister(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        byte quotientValue = computer.CPU.REGS.Get8BitRegister(quotientReg);

                        if (quotientValue.Equals(0))
                        {
                            computer.CPU.REGS.Set24BitRegister(dividendReg, 0xFFFFFF);
                            computer.CPU.FLAGS.SetCarry(true);
                        }
                        else
                        {
                            computer.CPU.REGS.Set24BitRegister(dividendReg, dividendValue / quotientValue);
                            computer.CPU.FLAGS.SetCarry(false);
                        }

                        return;
                    }
                case Mnem.DIVMUL_rrr_rr:  // DIV rrr, rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        uint dividendValue = computer.CPU.REGS.Get24BitRegister(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        ushort quotientValue = computer.CPU.REGS.Get16BitRegister(quotientReg);

                        if (quotientValue.Equals(0))
                        {
                            computer.CPU.REGS.Set24BitRegister(dividendReg, 0xFFFFFF);
                            computer.CPU.FLAGS.SetCarry(true);
                        }
                        else
                        {
                            computer.CPU.REGS.Set24BitRegister(dividendReg, dividendValue / quotientValue);
                            computer.CPU.FLAGS.SetCarry(false);
                        }

                        return;
                    }
                case Mnem.DIVMUL_rrr_rrr:  // DIV rrr, rrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        uint dividendValue = computer.CPU.REGS.Get24BitRegister(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        uint quotientValue = computer.CPU.REGS.Get24BitRegister(quotientReg);

                        if (quotientValue.Equals(0))
                        {
                            computer.CPU.REGS.Set24BitRegister(dividendReg, 0xFFFFFF);
                            computer.CPU.FLAGS.SetCarry(true);
                        }
                        else
                        {
                            computer.CPU.REGS.Set24BitRegister(dividendReg, dividendValue / quotientValue);
                            computer.CPU.FLAGS.SetCarry(false);
                        }

                        return;
                    }
                case Mnem.DIVMUL_rrrr_n:  // DIV rrrr, n
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        uint dividend = computer.CPU.REGS.Get32BitRegister(dividendReg);
                        ushort quotientValue = (ushort)(((byte)(mixedReg & 0b00011111) << 8) + computer.MEMC.Fetch());

                        if (quotientValue.Equals(0))
                        {
                            computer.CPU.REGS.Set32BitRegister(dividendReg, 0xFFFFFFFF);
                            computer.CPU.FLAGS.SetCarry(true);
                        }
                        else
                        {
                            computer.CPU.REGS.Set32BitRegister(dividendReg, dividend / quotientValue);
                            computer.CPU.FLAGS.SetCarry(false);
                        }
                        return;
                    }
                case Mnem.DIVMUL_rrrr_r:  // DIV rrrr, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        uint dividendValue = computer.CPU.REGS.Get32BitRegister(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        byte quotientValue = computer.CPU.REGS.Get8BitRegister(quotientReg);

                        if (quotientValue.Equals(0))
                        {
                            computer.CPU.REGS.Set32BitRegister(dividendReg, 0xFFFFFFFF);
                            computer.CPU.FLAGS.SetCarry(true);
                        }
                        else
                        {
                            computer.CPU.REGS.Set32BitRegister(dividendReg, dividendValue / quotientValue);
                            computer.CPU.FLAGS.SetCarry(false);
                        }

                        return;
                    }
                case Mnem.DIVMUL_rrrr_rr:  // DIV rrrr, rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        uint dividendValue = computer.CPU.REGS.Get32BitRegister(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        ushort quotientValue = computer.CPU.REGS.Get16BitRegister(quotientReg);

                        if (quotientValue.Equals(0))
                        {
                            computer.CPU.REGS.Set32BitRegister(dividendReg, 0xFFFFFFFF);
                            computer.CPU.FLAGS.SetCarry(true);
                        }
                        else
                        {
                            computer.CPU.REGS.Set32BitRegister(dividendReg, dividendValue / quotientValue);
                            computer.CPU.FLAGS.SetCarry(false);
                        }

                        return;
                    }
                case Mnem.DIVMUL_rrrr_rrr:  // DIV rrrr, rrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        uint dividendValue = computer.CPU.REGS.Get32BitRegister(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        uint quotientValue = computer.CPU.REGS.Get24BitRegister(quotientReg);

                        if (quotientValue.Equals(0))
                        {
                            computer.CPU.REGS.Set32BitRegister(dividendReg, 0xFFFFFFFF);
                            computer.CPU.FLAGS.SetCarry(true);
                        }
                        else
                        {
                            computer.CPU.REGS.Set32BitRegister(dividendReg, dividendValue / quotientValue);
                            computer.CPU.FLAGS.SetCarry(false);
                        }

                        return;
                    }
                case Mnem.DIVMUL_rrrr_rrrr:  // DIV rrrr, rrrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        uint dividendValue = computer.CPU.REGS.Get32BitRegister(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        uint quotientValue = computer.CPU.REGS.Get32BitRegister(quotientReg);

                        if (quotientValue.Equals(0))
                        {
                            computer.CPU.REGS.Set32BitRegister(dividendReg, 0xFFFFFFFF);
                            computer.CPU.FLAGS.SetCarry(true);
                        }
                        else
                        {
                            computer.CPU.REGS.Set32BitRegister(dividendReg, dividendValue / quotientValue);
                            computer.CPU.FLAGS.SetCarry(false);
                        }

                        return;
                    }


                // Remainder divisions
                case Mnem.DIV_r_n_r:  // DIV r, n, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte dividendValue = computer.CPU.REGS.Get8BitRegister(dividendReg);
                        byte remaindertReg = (byte)(mixedReg & 0b00011111);
                        byte quotientValue = computer.MEMC.Fetch();

                        if (quotientValue.Equals(0))
                        {
                            computer.CPU.REGS.Set8BitRegister(dividendReg, byte.MaxValue);
                            computer.CPU.REGS.Set8BitRegister(remaindertReg, 0);
                            computer.CPU.FLAGS.SetCarry(true);
                        }
                        else
                        {
                            computer.CPU.REGS.Set8BitRegister(dividendReg, (byte)(dividendValue / quotientValue));
                            computer.CPU.REGS.Set8BitRegister(remaindertReg, (byte)(dividendValue % quotientValue));
                            computer.CPU.FLAGS.SetCarry(false);
                        }

                        return;
                    }
                case Mnem.DIV_r_r_r:  // DIV r, r, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte dividendValue = computer.CPU.REGS.Get8BitRegister(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        byte quotientValue = computer.CPU.REGS.Get8BitRegister(quotientReg);

                        byte remaindertReg = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        if (quotientValue.Equals(0))
                        {
                            computer.CPU.REGS.Set8BitRegister(dividendReg, byte.MaxValue);
                            computer.CPU.REGS.Set8BitRegister(remaindertReg, 0);
                            computer.CPU.FLAGS.SetCarry(true);
                        }
                        else
                        {
                            computer.CPU.REGS.Set8BitRegister(dividendReg, (byte)(dividendValue / quotientValue));
                            computer.CPU.REGS.Set8BitRegister(remaindertReg, (byte)(dividendValue % quotientValue));
                            computer.CPU.FLAGS.SetCarry(false);
                        }

                        return;
                    }
                case Mnem.DIV_rr_n_rr:  // DIV rr, n, rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        ushort dividendValue = computer.CPU.REGS.Get16BitRegister(dividendReg);
                        byte remaindertReg = (byte)(mixedReg & 0b00011111);
                        ushort quotientValue = (ushort)((computer.MEMC.Fetch() << 8) + computer.MEMC.Fetch());

                        if (quotientValue.Equals(0))
                        {
                            computer.CPU.REGS.Set16BitRegister(dividendReg, 0xFFFF);
                            computer.CPU.REGS.Set16BitRegister(remaindertReg, 0);
                            computer.CPU.FLAGS.SetCarry(true);
                        }
                        else
                        {
                            computer.CPU.REGS.Set16BitRegister(dividendReg, (ushort)(dividendValue / quotientValue));
                            computer.CPU.REGS.Set16BitRegister(remaindertReg, (ushort)(dividendValue % quotientValue));
                            computer.CPU.FLAGS.SetCarry(false);
                        }

                        return;
                    }
                case Mnem.DIV_rr_r_r:  // DIV rr, r, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        ushort dividendValue = computer.CPU.REGS.Get16BitRegister(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        byte quotientValue = computer.CPU.REGS.Get8BitRegister(quotientReg);

                        byte remaindertReg = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        if (quotientValue.Equals(0))
                        {
                            computer.CPU.REGS.Set16BitRegister(dividendReg, 0xFFFF);
                            computer.CPU.REGS.Set8BitRegister(remaindertReg, 0);
                            computer.CPU.FLAGS.SetCarry(true);
                        }
                        else
                        {
                            computer.CPU.REGS.Set16BitRegister(dividendReg, (ushort)(dividendValue / quotientValue));
                            computer.CPU.REGS.Set8BitRegister(remaindertReg, (byte)(dividendValue % quotientValue));
                            computer.CPU.FLAGS.SetCarry(false);
                        }

                        return;
                    }
                case Mnem.DIV_rr_rr_rr:  // DIV rr, rr, rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        ushort dividendValue = computer.CPU.REGS.Get16BitRegister(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        ushort quotientValue = computer.CPU.REGS.Get16BitRegister(quotientReg);

                        byte remaindertReg = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        if (quotientValue.Equals(0))
                        {
                            computer.CPU.REGS.Set16BitRegister(dividendReg, 0xFFFF);
                            computer.CPU.REGS.Set16BitRegister(remaindertReg, 0);
                            computer.CPU.FLAGS.SetCarry(true);
                        }
                        else
                        {
                            computer.CPU.REGS.Set16BitRegister(dividendReg, (ushort)(dividendValue / quotientValue));
                            computer.CPU.REGS.Set16BitRegister(remaindertReg, (ushort)(dividendValue % quotientValue));
                            computer.CPU.FLAGS.SetCarry(false);
                        }

                        return;
                    }
                case Mnem.DIV_rrr_n_rr:  // DIV rrr, n, rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        uint dividendValue = computer.CPU.REGS.Get24BitRegister(dividendReg);
                        byte remaindertReg = (byte)(mixedReg & 0b00011111);
                        uint quotientValue = (uint)((computer.MEMC.Fetch() << 8) + computer.MEMC.Fetch());

                        if (quotientValue.Equals(0))
                        {
                            computer.CPU.REGS.Set24BitRegister(dividendReg, 0xFFFFFF);
                            computer.CPU.REGS.Set16BitRegister(remaindertReg, 0);
                            computer.CPU.FLAGS.SetCarry(true);
                        }
                        else
                        {
                            computer.CPU.REGS.Set24BitRegister(dividendReg, dividendValue / quotientValue);
                            computer.CPU.REGS.Set16BitRegister(remaindertReg, (ushort)(dividendValue % quotientValue));
                            computer.CPU.FLAGS.SetCarry(false);
                        }

                        return;
                    }
                case Mnem.DIV_rrr_r_r:  // DIV rrr, r, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        uint dividendValue = computer.CPU.REGS.Get24BitRegister(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        byte quotientValue = computer.CPU.REGS.Get8BitRegister(quotientReg);

                        byte remaindertReg = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        if (quotientValue.Equals(0))
                        {
                            computer.CPU.REGS.Set24BitRegister(dividendReg, 0xFFFFFF);
                            computer.CPU.REGS.Set8BitRegister(remaindertReg, 0);
                            computer.CPU.FLAGS.SetCarry(true);
                        }
                        else
                        {
                            computer.CPU.REGS.Set24BitRegister(dividendReg, dividendValue / quotientValue);
                            computer.CPU.REGS.Set8BitRegister(remaindertReg, (byte)(dividendValue % quotientValue));
                            computer.CPU.FLAGS.SetCarry(false);
                        }

                        return;
                    }
                case Mnem.DIV_rrr_rr_rr:  // DIV rrr, rr, rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        uint dividendValue = computer.CPU.REGS.Get24BitRegister(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        ushort quotientValue = computer.CPU.REGS.Get16BitRegister(quotientReg);

                        byte remaindertReg = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        if (quotientValue.Equals(0))
                        {
                            computer.CPU.REGS.Set24BitRegister(dividendReg, 0xFFFFFF);
                            computer.CPU.REGS.Set16BitRegister(remaindertReg, 0);
                            computer.CPU.FLAGS.SetCarry(true);
                        }
                        else
                        {
                            computer.CPU.REGS.Set24BitRegister(dividendReg, dividendValue / quotientValue);
                            computer.CPU.REGS.Set16BitRegister(remaindertReg, (ushort)(dividendValue % quotientValue));
                            computer.CPU.FLAGS.SetCarry(false);
                        }

                        return;
                    }
                case Mnem.DIV_rrr_rrr_rrr:  // DIV rrr, rrr, rrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        uint dividendValue = computer.CPU.REGS.Get24BitRegister(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        uint quotientValue = computer.CPU.REGS.Get24BitRegister(quotientReg);

                        byte remaindertReg = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        if (quotientValue.Equals(0))
                        {
                            computer.CPU.REGS.Set24BitRegister(dividendReg, 0xFFFFFF);
                            computer.CPU.REGS.Set24BitRegister(remaindertReg, 0);
                            computer.CPU.FLAGS.SetCarry(true);
                        }
                        else
                        {
                            computer.CPU.REGS.Set24BitRegister(dividendReg, dividendValue / quotientValue);
                            computer.CPU.REGS.Set24BitRegister(remaindertReg, dividendValue % quotientValue);
                            computer.CPU.FLAGS.SetCarry(false);
                        }

                        return;
                    }
                case Mnem.DIV_rrrr_n_rr:  // DIV rrrr, n, rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        uint dividendValue = computer.CPU.REGS.Get32BitRegister(dividendReg);
                        byte remaindertReg = (byte)(mixedReg & 0b00011111);
                        ushort quotientValue = (ushort)((computer.MEMC.Fetch() << 8) + computer.MEMC.Fetch());

                        if (quotientValue.Equals(0))
                        {
                            computer.CPU.REGS.Set32BitRegister(dividendReg, 0xFFFFFFFF);
                            computer.CPU.REGS.Set16BitRegister(remaindertReg, 0);
                            computer.CPU.FLAGS.SetCarry(true);
                        }
                        else
                        {
                            computer.CPU.REGS.Set32BitRegister(dividendReg, dividendValue / quotientValue);
                            computer.CPU.REGS.Set16BitRegister(remaindertReg, (ushort)(dividendValue % quotientValue));
                            computer.CPU.FLAGS.SetCarry(false);
                        }

                        return;
                    }
                case Mnem.DIV_rrrr_r_r:  // DIV rrrr, r, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        uint dividendValue = computer.CPU.REGS.Get32BitRegister(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        byte quotientValue = computer.CPU.REGS.Get8BitRegister(quotientReg);

                        byte remaindertReg = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        if (quotientValue.Equals(0))
                        {
                            computer.CPU.REGS.Set32BitRegister(dividendReg, 0xFFFFFFFF);
                            computer.CPU.REGS.Set8BitRegister(remaindertReg, 0);
                            computer.CPU.FLAGS.SetCarry(true);
                        }
                        else
                        {
                            computer.CPU.REGS.Set32BitRegister(dividendReg, dividendValue / quotientValue);
                            computer.CPU.REGS.Set8BitRegister(remaindertReg, (byte)(dividendValue % quotientValue));
                            computer.CPU.FLAGS.SetCarry(false);
                        }

                        return;
                    }
                case Mnem.DIV_rrrr_rr_rr:  // DIV rrrr, rr, rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        uint dividendValue = computer.CPU.REGS.Get32BitRegister(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        ushort quotientValue = computer.CPU.REGS.Get16BitRegister(quotientReg);

                        byte remaindertReg = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        if (quotientValue.Equals(0))
                        {
                            computer.CPU.REGS.Set32BitRegister(dividendReg, 0xFFFFFFFF);
                            computer.CPU.REGS.Set16BitRegister(remaindertReg, 0);
                            computer.CPU.FLAGS.SetCarry(true);
                        }
                        else
                        {
                            computer.CPU.REGS.Set32BitRegister(dividendReg, dividendValue / quotientValue);
                            computer.CPU.REGS.Set16BitRegister(remaindertReg, (ushort)(dividendValue % quotientValue));
                            computer.CPU.FLAGS.SetCarry(false);
                        }

                        return;
                    }
                case Mnem.DIV_rrrr_rrr_rrr:  // DIV rrrr, rrr, rrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        uint dividendValue = computer.CPU.REGS.Get32BitRegister(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        uint quotientValue = computer.CPU.REGS.Get24BitRegister(quotientReg);

                        byte remaindertReg = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        if (quotientValue.Equals(0))
                        {
                            computer.CPU.REGS.Set32BitRegister(dividendReg, 0xFFFFFFFF);
                            computer.CPU.REGS.Set24BitRegister(remaindertReg, 0);
                            computer.CPU.FLAGS.SetCarry(true);
                        }
                        else
                        {
                            computer.CPU.REGS.Set32BitRegister(dividendReg, dividendValue / quotientValue);
                            computer.CPU.REGS.Set24BitRegister(remaindertReg, dividendValue % quotientValue);
                            computer.CPU.FLAGS.SetCarry(false);
                        }

                        return;
                    }
                case Mnem.DIV_rrrr_rrrr_rrrr:  // DIV rrrr, rrrr, rrrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        uint dividendValue = computer.CPU.REGS.Get32BitRegister(dividendReg);
                        byte quotientReg = (byte)(mixedReg & 0b00011111);
                        uint quotientValue = computer.CPU.REGS.Get32BitRegister(quotientReg);

                        byte remaindertReg = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        if (quotientValue.Equals(0))
                        {
                            computer.CPU.REGS.Set32BitRegister(dividendReg, 0xFFFFFFFF);
                            computer.CPU.REGS.Set32BitRegister(remaindertReg, 0);
                            computer.CPU.FLAGS.SetCarry(true);
                        }
                        else
                        {
                            computer.CPU.REGS.Set32BitRegister(dividendReg, dividendValue / quotientValue);
                            computer.CPU.REGS.Set32BitRegister(remaindertReg, dividendValue % quotientValue);
                            computer.CPU.FLAGS.SetCarry(false);
                        }

                        return;
                    }

                // Floating point DIV
                case Mnem.DIVMUL_fr_fr:
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        byte fReg1 = (byte)(mixedReg >> 4);
                        byte fReg2 = (byte)(mixedReg & 0b00001111);

                        float fReg1Value = computer.CPU.FREGS.GetRegister(fReg1);
                        float fReg2Value = computer.CPU.FREGS.GetRegister(fReg2);

                        if (fReg2Value.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            float result = fReg1Value / fReg2Value;
                            computer.CPU.FLAGS.SetSignNegative(result < 0);
                            computer.CPU.FREGS.SetRegister(fReg1, result);
                        }

                        return;
                    }
                case Mnem.DIVMUL_fr_nnn:
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte fReg = (byte)(mixedReg & 0b00001111);
                        uint floatBitValue = computer.MEMC.Fetch32();
                        float floatValue = FloatPointUtils.UintToFloat(floatBitValue);
                        float fRegValue = computer.CPU.FREGS.GetRegister(fReg);

                        if (floatValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            float result = fRegValue / floatValue;
                            computer.CPU.FLAGS.SetSignNegative(result < 0);
                            computer.CPU.FREGS.SetRegister(fReg, result);
                        }

                        return;
                    }
                case Mnem.DIVMUL_fr_r:
                    {
                        byte fRegIndex = (byte)(computer.MEMC.Fetch() & 0b_00001111);
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b_00011111);

                        float regValue = computer.CPU.REGS.Get8BitRegister(regIndex);

                        if (regValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            float fRegValue = computer.CPU.FREGS.GetRegister(fRegIndex);
                            float result = fRegValue / regValue;
                            computer.CPU.FLAGS.SetSignNegative(result < 0);
                            computer.CPU.FREGS.SetRegister(fRegIndex, result);
                        }
                        return;
                    }
                case Mnem.DIVMUL_fr_rr:
                    {
                        byte fRegIndex = (byte)(computer.MEMC.Fetch() & 0b_00001111);
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b_00011111);

                        float regValue = computer.CPU.REGS.Get16BitRegister(regIndex);

                        if (regValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            float fRegValue = computer.CPU.FREGS.GetRegister(fRegIndex);
                            float result = fRegValue / regValue;
                            computer.CPU.FLAGS.SetSignNegative(result < 0);
                            computer.CPU.FREGS.SetRegister(fRegIndex, result);
                        }
                        return;
                    }
                case Mnem.DIVMUL_fr_rrr:
                    {
                        byte fRegIndex = (byte)(computer.MEMC.Fetch() & 0b_00001111);
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b_00011111);

                        float regValue = computer.CPU.REGS.Get24BitRegister(regIndex);

                        if (regValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            float fRegValue = computer.CPU.FREGS.GetRegister(fRegIndex);
                            float result = fRegValue / regValue;
                            computer.CPU.FLAGS.SetSignNegative(result < 0);
                            computer.CPU.FREGS.SetRegister(fRegIndex, result);
                        }
                        return;
                    }
                case Mnem.DIVMUL_fr_rrrr:
                    {
                        byte fRegIndex = (byte)(computer.MEMC.Fetch() & 0b_00001111);
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b_00011111);

                        float regValue = computer.CPU.REGS.Get32BitRegister(regIndex);

                        if (regValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            float fRegValue = computer.CPU.FREGS.GetRegister(fRegIndex);
                            float result = fRegValue / regValue;
                            computer.CPU.FLAGS.SetSignNegative(result < 0);
                            computer.CPU.FREGS.SetRegister(fRegIndex, result);
                        }

                        return;
                    }
                case Mnem.DIVMUL_r_fr:
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b_00011111);
                        byte fRegIndex = (byte)(computer.MEMC.Fetch() & 0b_00001111);

                        float fRegValue = computer.CPU.FREGS.GetRegister(fRegIndex);

                        if (fRegValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                            return;
                        }

                        float regValue = computer.CPU.REGS.Get8BitRegister(regIndex);
                        regValue /= fRegValue;
                        computer.CPU.REGS.Set8BitRegister(regIndex, (byte)Math.Round(regValue));

                        return;
                    }
                case Mnem.DIVMUL_rr_fr:
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b_00011111);
                        byte fRegIndex = (byte)(computer.MEMC.Fetch() & 0b_00001111);

                        float fRegValue = computer.CPU.FREGS.GetRegister(fRegIndex);

                        if (fRegValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                            return;
                        }

                        float regValue = computer.CPU.REGS.Get16BitRegister(regIndex);
                        regValue /= fRegValue;
                        computer.CPU.REGS.Set16BitRegister(regIndex, (ushort)Math.Round(regValue));

                        return;
                    }
                case Mnem.DIVMUL_rrr_fr:
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b_00011111);
                        byte fRegIndex = (byte)(computer.MEMC.Fetch() & 0b_00001111);

                        float fRegValue = computer.CPU.FREGS.GetRegister(fRegIndex);

                        if (fRegValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                            return;
                        }

                        float regValue = computer.CPU.REGS.Get24BitRegister(regIndex);
                        regValue /= fRegValue;
                        computer.CPU.REGS.Set24BitRegister(regIndex, (uint)Math.Abs(Math.Round(regValue)));

                        return;
                    }
                case Mnem.DIVMUL_rrrr_fr:
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b_00011111);
                        byte fRegIndex = (byte)(computer.MEMC.Fetch() & 0b_00001111);

                        float fRegValue = computer.CPU.FREGS.GetRegister(fRegIndex);

                        if (fRegValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                            return;
                        }

                        float regValue = computer.CPU.REGS.Get32BitRegister(regIndex);
                        regValue /= fRegValue;
                        computer.CPU.REGS.Set32BitRegister(regIndex, (uint)Math.Abs(Math.Round(regValue)));

                        return;
                    }
                case Mnem.DIVMUL_fr_InnnI:
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte fReg = (byte)(mixedReg & 0b00001111);
                        uint adrFloatPointer = computer.MEMC.Fetch24();
                        float adrFloatValue = computer.MEMC.GetFloatFromRAM(adrFloatPointer);
                        float fRegValue = computer.CPU.FREGS.GetRegister(fReg);

                        if (adrFloatValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            float result = fRegValue / adrFloatValue;
                            computer.CPU.FLAGS.SetSignNegative(result < 0);
                            computer.CPU.FREGS.SetRegister(fReg, result);
                        }

                        return;
                    }
                case Mnem.DIVMUL_fr_IrrrI:
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        byte fRegPointer = (byte)(((ldOp << 2) & 0b_00001111) + (mixedReg >> 6));
                        byte regPointer = (byte)(mixedReg & 0b_00011111);

                        uint adrFloatPointer = computer.CPU.REGS.Get24BitRegister(regPointer);

                        float adrFloatValue = computer.MEMC.GetFloatFromRAM(adrFloatPointer);
                        float fRegValue = computer.CPU.FREGS.GetRegister(fRegPointer);

                        if (adrFloatValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            float result = fRegValue / adrFloatValue;
                            computer.CPU.FLAGS.SetSignNegative(result < 0);
                            computer.CPU.FREGS.SetRegister(fRegPointer, result);
                        }

                        return;
                    }
                case Mnem.DIVMUL_InnnI_fr:
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte fReg = (byte)(mixedReg & 0b00001111);
                        uint adrFloatPointer = computer.MEMC.Fetch24();
                        float adrFloatValue = computer.MEMC.GetFloatFromRAM(adrFloatPointer);
                        float fRegValue = computer.CPU.FREGS.GetRegister(fReg);

                        if (adrFloatValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            float result = adrFloatValue / fRegValue;
                            computer.CPU.FLAGS.SetSignNegative(result < 0);
                            computer.MEMC.SetFloatToRam(adrFloatPointer, result);
                        }

                        return;
                    }
                case Mnem.DIVMUL_IrrrI_fr:
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        byte fRegPointer = (byte)(((ldOp << 2) & 0b_00001111) + (mixedReg >> 6));
                        byte regPointer = (byte)(mixedReg & 0b_00011111);

                        uint adrFloatPointer = computer.CPU.REGS.Get24BitRegister(regPointer);

                        float adrFloatValue = computer.MEMC.GetFloatFromRAM(adrFloatPointer);
                        float fRegValue = computer.CPU.FREGS.GetRegister(fRegPointer);

                        if (adrFloatValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            float result = adrFloatValue / fRegValue;
                            computer.CPU.FLAGS.SetSignNegative(result < 0);
                            computer.MEMC.SetFloatToRam(adrFloatPointer, result);
                        }

                        return;
                    }
            }
        }
    }
}
