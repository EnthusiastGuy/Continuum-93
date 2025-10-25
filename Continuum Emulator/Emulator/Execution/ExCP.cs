using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;
using Continuum93.Tools;

namespace Continuum93.Emulator.Execution
{
    public class ExCP
    {
        private static Computer _computer;
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 2);
            _computer = computer;

            switch (upperLdOp)
            {
                case Mnem.CP_r_n: // CP r, n
                    {
                        byte regIndex = (byte)(_computer.MEMC.Fetch() & 0b00011111);
                        byte regValue = _computer.CPU.REGS.Get8BitRegister(regIndex);
                        byte value = _computer.MEMC.Fetch();

                        Compare8bit(regValue, value);

                        return;
                    }
                case Mnem.CP_r_r: // CP r, r
                    {
                        byte mixedReg = _computer.MEMC.Fetch();
                        byte reg1Index = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte reg2Index = (byte)(mixedReg & 0b00011111);
                        byte reg1Value = _computer.CPU.REGS.Get8BitRegister(reg1Index);
                        byte reg2Value = _computer.CPU.REGS.Get8BitRegister(reg2Index);

                        Compare8bit(reg1Value, reg2Value);

                        return;
                    }
                case Mnem.CP_rr_nn: // CP rr, nn
                    {
                        byte regIndex = (byte)(_computer.MEMC.Fetch() & 0b00011111);
                        ushort regValue = _computer.CPU.REGS.Get16BitRegister(regIndex);
                        ushort value = _computer.MEMC.Fetch16();

                        Compare16bit(regValue, value);

                        return;
                    }
                case Mnem.CP_rr_rr: // CP rr, rr
                    {
                        byte mixedReg = _computer.MEMC.Fetch();
                        byte reg1Index = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte reg2Index = (byte)(mixedReg & 0b00011111);
                        ushort reg1Value = _computer.CPU.REGS.Get16BitRegister(reg1Index);
                        ushort reg2Value = _computer.CPU.REGS.Get16BitRegister(reg2Index);

                        Compare16bit(reg1Value, reg2Value);

                        return;
                    }
                case Mnem.CP_rrr_nnn: // CP rrr, nnn
                    {
                        byte regIndex = (byte)(_computer.MEMC.Fetch() & 0b00011111);
                        uint regValue = _computer.CPU.REGS.Get24BitRegister(regIndex);
                        uint value = _computer.MEMC.Fetch24();

                        Compare24bit(regValue, value);

                        return;
                    }
                case Mnem.CP_rrr_rrr: // CP rrr, rrr
                    {
                        byte mixedReg = _computer.MEMC.Fetch();
                        byte reg1Index = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte reg2Index = (byte)(mixedReg & 0b00011111);
                        uint reg1Value = _computer.CPU.REGS.Get24BitRegister(reg1Index);
                        uint reg2Value = _computer.CPU.REGS.Get24BitRegister(reg2Index);

                        Compare24bit(reg1Value, reg2Value);

                        return;
                    }
                case Mnem.CP_rrrr_nnnn: // CP rrrr, nnnn
                    {
                        byte regIndex = (byte)(_computer.MEMC.Fetch() & 0b00011111);
                        uint regValue = _computer.CPU.REGS.Get32BitRegister(regIndex);
                        uint value = _computer.MEMC.Fetch32();

                        Compare32bit(regValue, value);

                        return;
                    }
                case Mnem.CP_rrrr_rrrr: // CP rrrr, rrrr
                    {
                        byte mixedReg = _computer.MEMC.Fetch();
                        byte reg1Index = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte reg2Index = (byte)(mixedReg & 0b00011111);
                        uint reg1Value = _computer.CPU.REGS.Get32BitRegister(reg1Index);
                        uint reg2Value = _computer.CPU.REGS.Get32BitRegister(reg2Index);

                        Compare32bit(reg1Value, reg2Value);

                        return;
                    }
                case Mnem.CP_r_IrrrI: // CP r, (rrr)
                    {
                        byte mixedReg = _computer.MEMC.Fetch();
                        byte reg1Index = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte reg2Index = (byte)(mixedReg & 0b00011111);
                        byte reg1Value = _computer.CPU.REGS.Get8BitRegister(reg1Index);
                        uint reg2Value = _computer.CPU.REGS.Get24BitRegister(reg2Index);
                        byte value = _computer.MEMC.Get8bitFromRAM(reg2Value);

                        Compare8bit(reg1Value, value);

                        return;
                    }
                case Mnem.CP_rr_IrrrI: // CP rr, (rrr)
                    {
                        byte mixedReg = _computer.MEMC.Fetch();
                        byte reg1Index = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte reg2Index = (byte)(mixedReg & 0b00011111);
                        ushort reg1Value = _computer.CPU.REGS.Get16BitRegister(reg1Index);
                        uint reg2Value = _computer.CPU.REGS.Get24BitRegister(reg2Index);
                        ushort value = _computer.MEMC.Get16bitFromRAM(reg2Value);

                        Compare16bit(reg1Value, value);

                        return;
                    }
                case Mnem.CP_rrr_IrrrI: // CP rrr, (rrr)
                    {
                        byte mixedReg = _computer.MEMC.Fetch();
                        byte reg1Index = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte reg2Index = (byte)(mixedReg & 0b00011111);
                        uint reg1Value = _computer.CPU.REGS.Get24BitRegister(reg1Index);
                        uint reg2Value = _computer.CPU.REGS.Get24BitRegister(reg2Index);
                        uint value = _computer.MEMC.Get24bitFromRAM(reg2Value);

                        Compare24bit(reg1Value, value);

                        return;
                    }
                case Mnem.CP_rrrr_IrrrI: // CP rrrr, (rrr)
                    {
                        byte mixedReg = _computer.MEMC.Fetch();
                        byte reg1Index = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte reg2Index = (byte)(mixedReg & 0b00011111);
                        uint reg1Value = _computer.CPU.REGS.Get32BitRegister(reg1Index);
                        uint reg2Value = _computer.CPU.REGS.Get24BitRegister(reg2Index);
                        uint value = _computer.MEMC.Get32bitFromRAM(reg2Value);

                        Compare32bit(reg1Value, value);

                        return;
                    }
                case Mnem.CP_IrrrI_IrrrI:   // CP (rrr), (rrr)  ; byte
                    {
                        byte mixedReg = _computer.MEMC.Fetch();
                        byte reg1Index = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte reg2Index = (byte)(mixedReg & 0b00011111);
                        uint reg1Value = _computer.CPU.REGS.Get24BitRegister(reg1Index);
                        uint reg2Value = _computer.CPU.REGS.Get24BitRegister(reg2Index);
                        byte value1 = _computer.MEMC.Get8bitFromRAM(reg1Value);
                        byte value2 = _computer.MEMC.Get8bitFromRAM(reg2Value);

                        Compare8bit(value1, value2);

                        return;
                    }
                case Mnem.CP_IrrrI_nnn:   // CP (rrr), nnn
                    {
                        byte reg1Index = (byte)(_computer.MEMC.Fetch() & 0b00011111);
                        byte value2 = _computer.MEMC.Fetch();
                        uint reg1Value = _computer.CPU.REGS.Get24BitRegister(reg1Index);
                        byte value1 = _computer.MEMC.Get8bitFromRAM(reg1Value);

                        Compare8bit(value1, value2);

                        return;
                    }

                // Floating point CP
                case Mnem.CP_fr_fr:
                    {
                        byte mixedReg = _computer.MEMC.Fetch();

                        byte fReg1 = (byte)(mixedReg >> 4);
                        byte fReg2 = (byte)(mixedReg & 0b00001111);

                        float fReg1Value = _computer.CPU.FREGS.GetRegister(fReg1);
                        float fReg2Value = _computer.CPU.FREGS.GetRegister(fReg2);

                        CompareFloat(fReg1Value, fReg2Value);

                        return;
                    }
                case Mnem.CP_fr_nnn:
                    {
                        byte mixedReg = _computer.MEMC.Fetch();
                        byte fReg = (byte)(mixedReg & 0b00001111);
                        uint floatBitValue = _computer.MEMC.Fetch32();
                        float floatValue = FloatPointUtils.UintToFloat(floatBitValue);
                        float fRegValue = _computer.CPU.FREGS.GetRegister(fReg);

                        CompareFloat(fRegValue, floatValue);

                        return;
                    }
                case Mnem.CP_fr_r:
                    {
                        byte fRegIndex = (byte)(_computer.MEMC.Fetch() & 0b_00001111);
                        byte regIndex = (byte)(_computer.MEMC.Fetch() & 0b_00011111);

                        float regValue = _computer.CPU.REGS.Get8BitRegister(regIndex);

                        if (regValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            float fRegValue = _computer.CPU.FREGS.GetRegister(fRegIndex);
                            CompareFloat(fRegValue, regValue);
                        }
                        return;
                    }
                case Mnem.CP_fr_rr:
                    {
                        byte fRegIndex = (byte)(_computer.MEMC.Fetch() & 0b_00001111);
                        byte regIndex = (byte)(_computer.MEMC.Fetch() & 0b_00011111);

                        float regValue = _computer.CPU.REGS.Get16BitRegister(regIndex);

                        if (regValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            float fRegValue = _computer.CPU.FREGS.GetRegister(fRegIndex);
                            CompareFloat(fRegValue, regValue);
                        }
                        return;
                    }
                case Mnem.CP_fr_rrr:
                    {
                        byte fRegIndex = (byte)(_computer.MEMC.Fetch() & 0b_00001111);
                        byte regIndex = (byte)(_computer.MEMC.Fetch() & 0b_00011111);

                        float regValue = _computer.CPU.REGS.Get24BitRegister(regIndex);

                        if (regValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            float fRegValue = _computer.CPU.FREGS.GetRegister(fRegIndex);
                            CompareFloat(fRegValue, regValue);
                        }
                        return;
                    }
                case Mnem.CP_fr_rrrr:
                    {
                        byte fRegIndex = (byte)(_computer.MEMC.Fetch() & 0b_00001111);
                        byte regIndex = (byte)(_computer.MEMC.Fetch() & 0b_00011111);

                        float regValue = _computer.CPU.REGS.Get32BitRegister(regIndex);

                        if (regValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                        }
                        else
                        {
                            float fRegValue = _computer.CPU.FREGS.GetRegister(fRegIndex);
                            CompareFloat(fRegValue, regValue);
                        }

                        return;
                    }
                case Mnem.CP_r_fr:
                    {
                        byte regIndex = (byte)(_computer.MEMC.Fetch() & 0b_00011111);
                        byte fRegIndex = (byte)(_computer.MEMC.Fetch() & 0b_00001111);

                        float fRegValue = _computer.CPU.FREGS.GetRegister(fRegIndex);

                        if (fRegValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                            return;
                        }

                        float regValue = _computer.CPU.REGS.Get8BitRegister(regIndex);

                        CompareFloat(regValue, fRegValue);

                        return;
                    }
                case Mnem.CP_rr_fr:
                    {
                        byte regIndex = (byte)(_computer.MEMC.Fetch() & 0b_00011111);
                        byte fRegIndex = (byte)(_computer.MEMC.Fetch() & 0b_00001111);

                        float fRegValue = _computer.CPU.FREGS.GetRegister(fRegIndex);

                        if (fRegValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                            return;
                        }

                        float regValue = _computer.CPU.REGS.Get16BitRegister(regIndex);

                        CompareFloat(regValue, fRegValue);

                        return;
                    }
                case Mnem.CP_rrr_fr:
                    {
                        byte regIndex = (byte)(_computer.MEMC.Fetch() & 0b_00011111);
                        byte fRegIndex = (byte)(_computer.MEMC.Fetch() & 0b_00001111);

                        float fRegValue = _computer.CPU.FREGS.GetRegister(fRegIndex);

                        if (fRegValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                            return;
                        }

                        float regValue = _computer.CPU.REGS.Get24BitRegister(regIndex);

                        CompareFloat(regValue, fRegValue);

                        return;
                    }
                case Mnem.CP_rrrr_fr:
                    {
                        byte regIndex = (byte)(_computer.MEMC.Fetch() & 0b_00011111);
                        byte fRegIndex = (byte)(_computer.MEMC.Fetch() & 0b_00001111);

                        float fRegValue = _computer.CPU.FREGS.GetRegister(fRegIndex);

                        if (fRegValue.Equals(0))
                        {
                            ErrorHandler.ReportRuntimeError("Division by zero");
                            return;
                        }

                        float regValue = _computer.CPU.REGS.Get32BitRegister(regIndex);

                        CompareFloat(regValue, fRegValue);

                        return;
                    }
                case Mnem.CP_fr_InnnI:
                    {
                        byte mixedReg = _computer.MEMC.Fetch();
                        byte fReg = (byte)(mixedReg & 0b00001111);
                        uint adrFloatPointer = _computer.MEMC.Fetch24();
                        float adrFloatValue = _computer.MEMC.GetFloatFromRAM(adrFloatPointer);
                        float fRegValue = _computer.CPU.FREGS.GetRegister(fReg);

                        CompareFloat(fRegValue, adrFloatValue);

                        return;
                    }
                case Mnem.CP_fr_IrrrI:
                    {
                        byte mixedReg = _computer.MEMC.Fetch();

                        byte fRegPointer = (byte)(((ldOp << 2) & 0b_00001111) + (mixedReg >> 6));
                        byte regPointer = (byte)(mixedReg & 0b_00011111);

                        uint adrFloatPointer = _computer.CPU.REGS.Get24BitRegister(regPointer);

                        float adrFloatValue = _computer.MEMC.GetFloatFromRAM(adrFloatPointer);
                        float fRegValue = _computer.CPU.FREGS.GetRegister(fRegPointer);

                        CompareFloat(fRegValue, adrFloatValue);

                        return;
                    }
                case Mnem.CP_InnnI_fr:
                    {
                        byte mixedReg = _computer.MEMC.Fetch();
                        byte fReg = (byte)(mixedReg & 0b00001111);
                        uint adrFloatPointer = _computer.MEMC.Fetch24();
                        float adrFloatValue = _computer.MEMC.GetFloatFromRAM(adrFloatPointer);
                        float fRegValue = _computer.CPU.FREGS.GetRegister(fReg);

                        CompareFloat(adrFloatValue, fRegValue);

                        return;
                    }
                case Mnem.CP_IrrrI_fr:
                    {
                        byte mixedReg = _computer.MEMC.Fetch();

                        byte fRegPointer = (byte)(((ldOp << 2) & 0b_00001111) + (mixedReg >> 6));
                        byte regPointer = (byte)(mixedReg & 0b_00011111);

                        uint adrFloatPointer = _computer.CPU.REGS.Get24BitRegister(regPointer);

                        float adrFloatValue = _computer.MEMC.GetFloatFromRAM(adrFloatPointer);
                        float fRegValue = _computer.CPU.FREGS.GetRegister(fRegPointer);

                        CompareFloat(adrFloatValue, fRegValue);

                        return;
                    }

            }
        }

        /*
         Parity refers to the number one's present in the given 8-bit data.
        The parity flag becomes set or goes high when the number of one's present in the data is even(2,4,6,8)
        and if the number of one's present in the data is odd (1,3,5,7) then the flag resets or goes low.
         */

        private static void Compare8bit(byte val1, byte val2)
        {
            var subtracted = val1 - val2;
            _computer.CPU.FLAGS.SetValueByIndexFast(0, val1 == val2);    // Z flag
            _computer.CPU.FLAGS.SetValueByIndexFast(1, val1 < val2);    // C flag
            _computer.CPU.FLAGS.SetValueByIndexFast(2, (subtracted & 0x80) > 0);    // SN flag
            _computer.CPU.FLAGS.SetValueByIndexFast(3, false);    // OV flag
            _computer.CPU.FLAGS.SetValueByIndexFast(4, !(val1 > 0x80 && val2 > 0x80 && (sbyte)subtracted > 0) || (val1 < 0x80 && val2 < 0x80 && (sbyte)subtracted < 0));    // PO flag
            _computer.CPU.FLAGS.SetValueByIndexFast(5, val1 == val2);    // EQ flag
            _computer.CPU.FLAGS.SetValueByIndexFast(6, val1 > val2);    // GT flag
            _computer.CPU.FLAGS.SetValueByIndexFast(7, val1 < val2);    // LT flag
        }

        private static void Compare16bit(ushort val1, ushort val2)
        {
            var subtracted = val1 - val2;
            _computer.CPU.FLAGS.SetValueByIndexFast(0, val1 == val2);    // Z flag
            _computer.CPU.FLAGS.SetValueByIndexFast(1, val1 < val2);    // C flag
            _computer.CPU.FLAGS.SetValueByIndexFast(2, (subtracted & 0x8000) > 0);    // SN flag
            _computer.CPU.FLAGS.SetValueByIndexFast(3, false);    // OV flag
            _computer.CPU.FLAGS.SetValueByIndexFast(4, !(val1 > 0x8000 && val2 > 0x8000 && (sbyte)subtracted > 0) || (val1 < 0x8000 && val2 < 0x8000 && (sbyte)subtracted < 0));    // PO flag
            _computer.CPU.FLAGS.SetValueByIndexFast(5, val1 == val2);    // EQ flag
            _computer.CPU.FLAGS.SetValueByIndexFast(6, val1 > val2);    // GT flag
            _computer.CPU.FLAGS.SetValueByIndexFast(7, val1 < val2);    // LT flag
        }

        private static void Compare24bit(uint val1, uint val2)
        {
            var subtracted = val1 - val2;
            _computer.CPU.FLAGS.SetValueByIndexFast(0, val1 == val2);    // Z flag
            _computer.CPU.FLAGS.SetValueByIndexFast(1, val1 < val2);    // C flag
            _computer.CPU.FLAGS.SetValueByIndexFast(2, (subtracted & 0x800000) > 0);    // SN flag
            _computer.CPU.FLAGS.SetValueByIndexFast(3, false);    // OV flag
            _computer.CPU.FLAGS.SetValueByIndexFast(4, !(val1 > 0x800000 && val2 > 0x800000 && (sbyte)subtracted > 0) || (val1 < 0x800000 && val2 < 0x800000 && (sbyte)subtracted < 0));    // PO flag
            _computer.CPU.FLAGS.SetValueByIndexFast(5, val1 == val2);    // EQ flag
            _computer.CPU.FLAGS.SetValueByIndexFast(6, val1 > val2);    // GT flag
            _computer.CPU.FLAGS.SetValueByIndexFast(7, val1 < val2);    // LT flag
        }

        private static void Compare32bit(uint val1, uint val2)
        {
            var subtracted = val1 - val2;
            _computer.CPU.FLAGS.SetValueByIndexFast(0, val1 == val2);    // Z flag
            _computer.CPU.FLAGS.SetValueByIndexFast(1, val1 < val2);    // C flag
            _computer.CPU.FLAGS.SetValueByIndexFast(2, (subtracted & 0x80000000) > 0);    // SN flag
            _computer.CPU.FLAGS.SetValueByIndexFast(3, false);    // OV flag
            _computer.CPU.FLAGS.SetValueByIndexFast(4, !(val1 > 0x80000000 && val2 > 0x80000000 && (sbyte)subtracted > 0) || (val1 < 0x80000000 && val2 < 0x80000000 && (sbyte)subtracted < 0));    // PO flag
            _computer.CPU.FLAGS.SetValueByIndexFast(5, val1 == val2);    // EQ flag
            _computer.CPU.FLAGS.SetValueByIndexFast(6, val1 > val2);    // GT flag
            _computer.CPU.FLAGS.SetValueByIndexFast(7, val1 < val2);    // LT flag
        }

        private static void CompareFloat(float val1, float val2)
        {
            int comparison = val1.CompareTo(val2);

            _computer.CPU.FLAGS.SetValueByIndexFast(0, comparison == 0);  // Z flag (Equal)
            _computer.CPU.FLAGS.SetValueByIndexFast(1, comparison < 0);   // C flag (Carry, if val1 < val2)
            _computer.CPU.FLAGS.SetValueByIndexFast(2, val1 < 0);        // SN flag (Sign of val1)
            _computer.CPU.FLAGS.SetValueByIndexFast(3, false);           // OV flag (Overflow flag, not applicable to floats)
            _computer.CPU.FLAGS.SetValueByIndexFast(4, false);           // PO flag (Parity flag, not applicable to floats)
            _computer.CPU.FLAGS.SetValueByIndexFast(5, comparison == 0);  // EQ flag (Equal)
            _computer.CPU.FLAGS.SetValueByIndexFast(6, comparison > 0);   // GT flag (Greater Than)
            _computer.CPU.FLAGS.SetValueByIndexFast(7, comparison < 0);   // LT flag (Less Than)
        }
    }
}
