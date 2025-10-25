using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;
using Continuum93.Tools;
using System;

namespace Continuum93.Emulator.Execution
{
    public class ExMUL
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 2);
            switch (upperLdOp)
            {
                case Mnem.DIVMUL_r_n:  // MUL r, n
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte multiplicandReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte multiplicand = computer.CPU.REGS.Get8BitRegister(multiplicandReg);

                        ushort multiplierValue = (ushort)(((byte)(mixedReg & 0b00011111) << 8) + computer.MEMC.Fetch());
                        uint product = (uint)multiplicand * multiplierValue;

                        computer.CPU.REGS.Set8BitRegister(multiplicandReg, (byte)product);
                        computer.CPU.FLAGS.SetCarry(product > byte.MaxValue);

                        return;
                    }
                case Mnem.DIVMUL_r_r:  // MUL r, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte multiplicandReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte multiplicandValue = computer.CPU.REGS.Get8BitRegister(multiplicandReg);
                        byte multiplierReg = (byte)(mixedReg & 0b00011111);
                        byte multiplierValue = computer.CPU.REGS.Get8BitRegister(multiplierReg);

                        uint product = (uint)multiplicandValue * multiplierValue;

                        computer.CPU.REGS.Set8BitRegister(multiplicandReg, (byte)product);
                        computer.CPU.FLAGS.SetCarry(product > byte.MaxValue);

                        return;
                    }
                case Mnem.DIVMUL_rr_n:  // MUL rr, n
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte multiplicandReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        ushort multiplicand = computer.CPU.REGS.Get16BitRegister(multiplicandReg);
                        ushort multiplierValue = (ushort)(((byte)(mixedReg & 0b00011111) << 8) + computer.MEMC.Fetch());

                        uint product = (uint)multiplicand * multiplierValue;

                        computer.CPU.REGS.Set16BitRegister(multiplicandReg, (ushort)product);
                        computer.CPU.FLAGS.SetCarry(product > ushort.MaxValue);

                        return;
                    }
                case Mnem.DIVMUL_rr_r:  // MUL rr, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte multiplicandReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        ushort multiplicandValue = computer.CPU.REGS.Get16BitRegister(multiplicandReg);
                        byte multiplierReg = (byte)(mixedReg & 0b00011111);
                        byte multiplierValue = computer.CPU.REGS.Get8BitRegister(multiplierReg);

                        uint product = (uint)multiplicandValue * multiplierValue;

                        computer.CPU.REGS.Set16BitRegister(multiplicandReg, (ushort)product);
                        computer.CPU.FLAGS.SetCarry(product > ushort.MaxValue);

                        return;
                    }
                case Mnem.DIVMUL_rr_rr:  // MUL rr, rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte multiplicandReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        ushort multiplicandValue = computer.CPU.REGS.Get16BitRegister(multiplicandReg);
                        byte multiplierReg = (byte)(mixedReg & 0b00011111);
                        ushort multiplierValue = computer.CPU.REGS.Get16BitRegister(multiplierReg);

                        uint product = (uint)multiplicandValue * multiplierValue;

                        computer.CPU.REGS.Set16BitRegister(multiplicandReg, (ushort)product);
                        computer.CPU.FLAGS.SetCarry(product > ushort.MaxValue);

                        return;
                    }
                case Mnem.DIVMUL_rrr_n:  // MUL rrr, n
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte multiplicandReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        uint multiplicand = computer.CPU.REGS.Get24BitRegister(multiplicandReg);
                        uint multiplierValue = (ushort)(((byte)(mixedReg & 0b00011111) << 8) + computer.MEMC.Fetch());

                        uint product = multiplicand * multiplierValue;

                        computer.CPU.REGS.Set24BitRegister(multiplicandReg, product);
                        computer.CPU.FLAGS.SetCarry(product > 0xFFFFFF);

                        return;
                    }
                case Mnem.DIVMUL_rrr_r:  // MUL rrr, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte multiplicandReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        uint multiplicandValue = computer.CPU.REGS.Get24BitRegister(multiplicandReg);
                        byte multiplierReg = (byte)(mixedReg & 0b00011111);
                        byte multiplierValue = computer.CPU.REGS.Get8BitRegister(multiplierReg);

                        uint product = multiplicandValue * multiplierValue;

                        computer.CPU.REGS.Set24BitRegister(multiplicandReg, product);
                        computer.CPU.FLAGS.SetCarry(product > 0xFFFFFF);

                        return;
                    }
                case Mnem.DIVMUL_rrr_rr:  // MUL rrr, rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte multiplicandReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        uint multiplicandValue = computer.CPU.REGS.Get24BitRegister(multiplicandReg);
                        byte multiplierReg = (byte)(mixedReg & 0b00011111);
                        ushort multiplierValue = computer.CPU.REGS.Get16BitRegister(multiplierReg);

                        uint product = multiplicandValue * multiplierValue;

                        computer.CPU.REGS.Set24BitRegister(multiplicandReg, product);
                        computer.CPU.FLAGS.SetCarry(product > 0xFFFFFF);

                        return;
                    }
                case Mnem.DIVMUL_rrr_rrr:  // MUL rrr, rrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte multiplicandReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        uint multiplicandValue = computer.CPU.REGS.Get24BitRegister(multiplicandReg);
                        byte multiplierReg = (byte)(mixedReg & 0b00011111);
                        uint multiplierValue = computer.CPU.REGS.Get24BitRegister(multiplierReg);

                        uint product = multiplicandValue * multiplierValue;

                        computer.CPU.REGS.Set24BitRegister(multiplicandReg, product);
                        computer.CPU.FLAGS.SetCarry(product > 0xFFFFFF);

                        return;
                    }
                case Mnem.DIVMUL_rrrr_n:  // MUL rrrr, n
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte multiplicandReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        uint multiplicand = computer.CPU.REGS.Get32BitRegister(multiplicandReg);
                        ushort multiplierValue = (ushort)(((byte)(mixedReg & 0b00011111) << 8) + computer.MEMC.Fetch());

                        ulong product = multiplicand * (ulong)multiplierValue;

                        computer.CPU.REGS.Set32BitRegister(multiplicandReg, (uint)product);
                        computer.CPU.FLAGS.SetCarry(product > 0xFFFFFFFF);

                        return;
                    }
                case Mnem.DIVMUL_rrrr_r:  // MUL rrrr, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte multiplicandReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        uint multiplicandValue = computer.CPU.REGS.Get32BitRegister(multiplicandReg);
                        byte multiplierReg = (byte)(mixedReg & 0b00011111);
                        byte multiplierValue = computer.CPU.REGS.Get8BitRegister(multiplierReg);

                        ulong product = multiplicandValue * (ulong)multiplierValue;

                        computer.CPU.REGS.Set32BitRegister(multiplicandReg, (uint)product);
                        computer.CPU.FLAGS.SetCarry(product > 0xFFFFFFFF);

                        return;
                    }
                case Mnem.DIVMUL_rrrr_rr:  // MUL rrrr, rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte multiplicandReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        uint multiplicandValue = computer.CPU.REGS.Get32BitRegister(multiplicandReg);
                        byte multiplierReg = (byte)(mixedReg & 0b00011111);
                        ushort multiplierValue = computer.CPU.REGS.Get16BitRegister(multiplierReg);

                        ulong product = multiplicandValue * (ulong)multiplierValue;

                        computer.CPU.REGS.Set32BitRegister(multiplicandReg, (uint)product);
                        computer.CPU.FLAGS.SetCarry(product > 0xFFFFFFFF);

                        return;
                    }
                case Mnem.DIVMUL_rrrr_rrr:  // MUL rrrr, rrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte multiplicandReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        uint multiplicandValue = computer.CPU.REGS.Get32BitRegister(multiplicandReg);
                        byte multiplierReg = (byte)(mixedReg & 0b00011111);
                        uint multiplierValue = computer.CPU.REGS.Get24BitRegister(multiplierReg);

                        ulong product = multiplicandValue * (ulong)multiplierValue;

                        computer.CPU.REGS.Set32BitRegister(multiplicandReg, (uint)product);
                        computer.CPU.FLAGS.SetCarry(product > 0xFFFFFFFF);

                        return;
                    }
                case Mnem.DIVMUL_rrrr_rrrr:  // MUL rrrr, rrrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte multiplicandReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        uint multiplicandValue = computer.CPU.REGS.Get32BitRegister(multiplicandReg);
                        byte multiplierReg = (byte)(mixedReg & 0b00011111);
                        uint multiplierValue = computer.CPU.REGS.Get32BitRegister(multiplierReg);

                        ulong product = multiplicandValue * (ulong)multiplierValue;

                        computer.CPU.REGS.Set32BitRegister(multiplicandReg, (uint)product);
                        computer.CPU.FLAGS.SetCarry(product > 0xFFFFFFFF);

                        return;
                    }

                // Floating point MUL
                case Mnem.DIVMUL_fr_fr:
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        byte fReg1 = (byte)(mixedReg >> 4);
                        byte fReg2 = (byte)(mixedReg & 0b00001111);

                        float fReg1Value = computer.CPU.FREGS.GetRegister(fReg1);
                        float fReg2Value = computer.CPU.FREGS.GetRegister(fReg2);

                        float result = fReg1Value * fReg2Value;
                        computer.CPU.FLAGS.SetSignNegative(result < 0);
                        computer.CPU.FREGS.SetRegister(fReg1, result);

                        return;
                    }
                case Mnem.DIVMUL_fr_nnn:
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte fReg = (byte)(mixedReg & 0b00001111);
                        uint floatBitValue = computer.MEMC.Fetch32();
                        float floatValue = FloatPointUtils.UintToFloat(floatBitValue);
                        float fRegValue = computer.CPU.FREGS.GetRegister(fReg);

                        float result = fRegValue * floatValue;
                        computer.CPU.FLAGS.SetSignNegative(result < 0);
                        computer.CPU.FREGS.SetRegister(fReg, result);

                        return;
                    }
                case Mnem.DIVMUL_fr_r:
                    {
                        byte fRegIndex = (byte)(computer.MEMC.Fetch() & 0b_00001111);
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b_00011111);

                        float regValue = computer.CPU.REGS.Get8BitRegister(regIndex);

                        float fRegValue = computer.CPU.FREGS.GetRegister(fRegIndex);
                        float result = fRegValue * regValue;
                        computer.CPU.FLAGS.SetSignNegative(result < 0);
                        computer.CPU.FREGS.SetRegister(fRegIndex, result);
                        return;
                    }
                case Mnem.DIVMUL_fr_rr:
                    {
                        byte fRegIndex = (byte)(computer.MEMC.Fetch() & 0b_00001111);
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b_00011111);

                        float regValue = computer.CPU.REGS.Get16BitRegister(regIndex);

                        float fRegValue = computer.CPU.FREGS.GetRegister(fRegIndex);
                        float result = fRegValue * regValue;
                        computer.CPU.FLAGS.SetSignNegative(result < 0);
                        computer.CPU.FREGS.SetRegister(fRegIndex, result);
                        return;
                    }
                case Mnem.DIVMUL_fr_rrr:
                    {
                        byte fRegIndex = (byte)(computer.MEMC.Fetch() & 0b_00001111);
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b_00011111);

                        float regValue = computer.CPU.REGS.Get24BitRegister(regIndex);

                        float fRegValue = computer.CPU.FREGS.GetRegister(fRegIndex);
                        float result = fRegValue * regValue;
                        computer.CPU.FLAGS.SetSignNegative(result < 0);
                        computer.CPU.FREGS.SetRegister(fRegIndex, result);
                        return;
                    }
                case Mnem.DIVMUL_fr_rrrr:
                    {
                        byte fRegIndex = (byte)(computer.MEMC.Fetch() & 0b_00001111);
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b_00011111);

                        float regValue = computer.CPU.REGS.Get32BitRegister(regIndex);

                        float fRegValue = computer.CPU.FREGS.GetRegister(fRegIndex);
                        float result = fRegValue * regValue;
                        computer.CPU.FLAGS.SetSignNegative(result < 0);
                        computer.CPU.FREGS.SetRegister(fRegIndex, result);

                        return;
                    }
                case Mnem.DIVMUL_r_fr:
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b_00011111);
                        byte fRegIndex = (byte)(computer.MEMC.Fetch() & 0b_00001111);

                        float fRegValue = computer.CPU.FREGS.GetRegister(fRegIndex);
                        float regValue;

                        regValue = computer.CPU.REGS.Get8BitRegister(regIndex);
                        regValue *= fRegValue;
                        computer.CPU.REGS.Set8BitRegister(regIndex, (byte)Math.Round(regValue));

                        return;
                    }
                case Mnem.DIVMUL_rr_fr:
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b_00011111);
                        byte fRegIndex = (byte)(computer.MEMC.Fetch() & 0b_00001111);

                        float fRegValue = computer.CPU.FREGS.GetRegister(fRegIndex);
                        float regValue;

                        regValue = computer.CPU.REGS.Get16BitRegister(regIndex);
                        regValue *= fRegValue;
                        computer.CPU.REGS.Set16BitRegister(regIndex, (ushort)Math.Round(regValue));

                        return;
                    }
                case Mnem.DIVMUL_rrr_fr:
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b_00011111);
                        byte fRegIndex = (byte)(computer.MEMC.Fetch() & 0b_00001111);

                        float fRegValue = computer.CPU.FREGS.GetRegister(fRegIndex);
                        float regValue;

                        regValue = computer.CPU.REGS.Get24BitRegister(regIndex);
                        regValue *= fRegValue;
                        computer.CPU.REGS.Set24BitRegister(regIndex, (uint)Math.Abs(Math.Round(regValue)));

                        return;
                    }
                case Mnem.DIVMUL_rrrr_fr:
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b_00011111);
                        byte fRegIndex = (byte)(computer.MEMC.Fetch() & 0b_00001111);

                        float fRegValue = computer.CPU.FREGS.GetRegister(fRegIndex);
                        float regValue;

                        regValue = computer.CPU.REGS.Get32BitRegister(regIndex);
                        regValue *= fRegValue;
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

                        float result = fRegValue * adrFloatValue;
                        computer.CPU.FLAGS.SetSignNegative(result < 0);
                        computer.CPU.FREGS.SetRegister(fReg, result);

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

                        float result = fRegValue * adrFloatValue;
                        computer.CPU.FLAGS.SetSignNegative(result < 0);
                        computer.CPU.FREGS.SetRegister(fRegPointer, result);

                        return;
                    }
                case Mnem.DIVMUL_InnnI_fr:
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte fReg = (byte)(mixedReg & 0b00001111);
                        uint adrFloatPointer = computer.MEMC.Fetch24();
                        float adrFloatValue = computer.MEMC.GetFloatFromRAM(adrFloatPointer);
                        float fRegValue = computer.CPU.FREGS.GetRegister(fReg);

                        float result = adrFloatValue * fRegValue;
                        computer.CPU.FLAGS.SetSignNegative(result < 0);
                        computer.MEMC.SetFloatToRam(adrFloatPointer, result);

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

                        float result = adrFloatValue * fRegValue;
                        computer.CPU.FLAGS.SetSignNegative(result < 0);
                        computer.MEMC.SetFloatToRam(adrFloatPointer, result);

                        return;
                    }
            }
        }
    }
}
