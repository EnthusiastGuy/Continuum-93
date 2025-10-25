using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public static class ExSCP
    {
        private static Computer _computer;

        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 2);
            _computer = computer;

            switch (upperLdOp)
            {
                case Mnem.CP_r_n: // SCP r, n
                    {
                        byte regIndex = (byte)(_computer.MEMC.Fetch() & 0b00011111);
                        sbyte regValue = _computer.CPU.REGS.Get8BitRegisterSigned(regIndex);
                        sbyte value = _computer.MEMC.FetchSigned();

                        Compare8bitSigned(regValue, value);

                        return;
                    }
                case Mnem.CP_r_r: // SCP r, r
                    {
                        byte mixedReg = _computer.MEMC.Fetch();
                        byte reg1Index = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte reg2Index = (byte)(mixedReg & 0b00011111);
                        sbyte reg1Value = _computer.CPU.REGS.Get8BitRegisterSigned(reg1Index);
                        sbyte reg2Value = _computer.CPU.REGS.Get8BitRegisterSigned(reg2Index);

                        Compare8bitSigned(reg1Value, reg2Value);

                        return;
                    }
                case Mnem.CP_rr_nn: // SCP rr, nn
                    {
                        byte regIndex = (byte)(_computer.MEMC.Fetch() & 0b00011111);
                        short regValue = _computer.CPU.REGS.Get16BitRegisterSigned(regIndex);
                        short value = _computer.MEMC.Fetch16Signed();

                        Compare16bitSigned(regValue, value);

                        return;
                    }
                case Mnem.CP_rr_rr: // SCP rr, rr
                    {
                        byte mixedReg = _computer.MEMC.Fetch();
                        byte reg1Index = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte reg2Index = (byte)(mixedReg & 0b00011111);
                        short reg1Value = _computer.CPU.REGS.Get16BitRegisterSigned(reg1Index);
                        short reg2Value = _computer.CPU.REGS.Get16BitRegisterSigned(reg2Index);

                        Compare16bitSigned(reg1Value, reg2Value);

                        return;
                    }
                case Mnem.CP_rrr_nnn: // SCP rrr, nnn
                    {
                        byte regIndex = (byte)(_computer.MEMC.Fetch() & 0b00011111);
                        int regValue = _computer.CPU.REGS.Get24BitRegisterSigned(regIndex);
                        int value = _computer.MEMC.Fetch24Signed();

                        Compare24bitSigned(regValue, value);

                        return;
                    }
                case Mnem.CP_rrr_rrr: // SCP rrr, rrr
                    {
                        byte mixedReg = _computer.MEMC.Fetch();
                        byte reg1Index = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte reg2Index = (byte)(mixedReg & 0b00011111);
                        int reg1Value = _computer.CPU.REGS.Get24BitRegisterSigned(reg1Index);
                        int reg2Value = _computer.CPU.REGS.Get24BitRegisterSigned(reg2Index);

                        Compare24bitSigned(reg1Value, reg2Value);

                        return;
                    }
                case Mnem.CP_rrrr_nnnn: // SCP rrrr, nnnn
                    {
                        byte regIndex = (byte)(_computer.MEMC.Fetch() & 0b00011111);
                        int regValue = _computer.CPU.REGS.Get32BitRegisterSigned(regIndex);
                        int value = _computer.MEMC.Fetch32Signed();

                        Compare32bitSigned(regValue, value);

                        return;
                    }
                case Mnem.CP_rrrr_rrrr: // SCP rrrr, rrrr
                    {
                        byte mixedReg = _computer.MEMC.Fetch();
                        byte reg1Index = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte reg2Index = (byte)(mixedReg & 0b00011111);
                        int reg1Value = _computer.CPU.REGS.Get32BitRegisterSigned(reg1Index);
                        int reg2Value = _computer.CPU.REGS.Get32BitRegisterSigned(reg2Index);

                        Compare32bitSigned(reg1Value, reg2Value);

                        return;
                    }
                case Mnem.CP_r_IrrrI: // SCP r, (rrr)
                    {
                        byte mixedReg = _computer.MEMC.Fetch();
                        byte reg1Index = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte reg2Index = (byte)(mixedReg & 0b00011111);
                        sbyte reg1Value = _computer.CPU.REGS.Get8BitRegisterSigned(reg1Index);
                        uint reg2Value = _computer.CPU.REGS.Get24BitRegister(reg2Index);
                        sbyte value = _computer.MEMC.GetSigned8bitFromRAM(reg2Value);

                        Compare8bitSigned(reg1Value, value);

                        return;
                    }
                case Mnem.CP_rr_IrrrI: // SCP rr, (rrr)
                    {
                        byte mixedReg = _computer.MEMC.Fetch();
                        byte reg1Index = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte reg2Index = (byte)(mixedReg & 0b00011111);
                        short reg1Value = _computer.CPU.REGS.Get16BitRegisterSigned(reg1Index);
                        uint reg2Value = _computer.CPU.REGS.Get24BitRegister(reg2Index);
                        short value = _computer.MEMC.GetSigned16bitFromRAM(reg2Value);

                        Compare16bitSigned(reg1Value, value);

                        return;
                    }
                case Mnem.CP_rrr_IrrrI: // SCP rrr, (rrr)
                    {
                        byte mixedReg = _computer.MEMC.Fetch();
                        byte reg1Index = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte reg2Index = (byte)(mixedReg & 0b00011111);
                        int reg1Value = _computer.CPU.REGS.Get24BitRegisterSigned(reg1Index);
                        uint reg2Value = _computer.CPU.REGS.Get24BitRegister(reg2Index);
                        int value = _computer.MEMC.GetSigned24bitFromRAM(reg2Value);

                        Compare24bitSigned(reg1Value, value);

                        return;
                    }
                case Mnem.CP_rrrr_IrrrI: // SCP rrrr, (rrr)
                    {
                        byte mixedReg = _computer.MEMC.Fetch();
                        byte reg1Index = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte reg2Index = (byte)(mixedReg & 0b00011111);
                        int reg1Value = _computer.CPU.REGS.Get32BitRegisterSigned(reg1Index);
                        uint reg2Value = _computer.CPU.REGS.Get24BitRegister(reg2Index);
                        int value = _computer.MEMC.GetSigned32bitFromRAM(reg2Value);

                        Compare32bitSigned(reg1Value, value);

                        return;
                    }
                case Mnem.CP_IrrrI_IrrrI:   // SCP (rrr), (rrr)  ; byte
                    {
                        byte mixedReg = _computer.MEMC.Fetch();
                        byte reg1Index = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte reg2Index = (byte)(mixedReg & 0b00011111);
                        uint reg1Value = _computer.CPU.REGS.Get24BitRegister(reg1Index);
                        uint reg2Value = _computer.CPU.REGS.Get24BitRegister(reg2Index);
                        sbyte value1 = _computer.MEMC.GetSigned8bitFromRAM(reg1Value);
                        sbyte value2 = _computer.MEMC.GetSigned8bitFromRAM(reg2Value);

                        Compare8bitSigned(value1, value2);

                        return;
                    }
                case Mnem.CP_IrrrI_nnn:   // SCP (rrr), nnn
                    {
                        byte reg1Index = (byte)(_computer.MEMC.Fetch() & 0b00011111);
                        sbyte value2 = _computer.MEMC.FetchSigned();
                        uint reg1Value = _computer.CPU.REGS.Get24BitRegister(reg1Index);
                        sbyte value1 = _computer.MEMC.GetSigned8bitFromRAM(reg1Value);

                        Compare8bitSigned(value1, value2);

                        return;
                    }
            }
        }

        private static void Compare8bitSigned(sbyte val1, sbyte val2)
        {
            var subtracted = val1 - val2;
            _computer.CPU.FLAGS.SetValueByIndexFast(0, val1 == val2);    // Z flag
            //_computer.CPU.FLAGS.SetValueByIndexFast(1, val1 < val2);    // C flag
            _computer.CPU.FLAGS.SetValueByIndexFast(2, subtracted < 0);    // SN flag
            _computer.CPU.FLAGS.SetValueByIndexFast(3, false);    // OV flag
            //_computer.CPU.FLAGS.SetValueByIndexFast(4, !(val1 > 0x80 && val2 > 0x80 && (sbyte)subtracted > 0) || (val1 < 0x80 && val2 < 0x80 && (sbyte)subtracted < 0));    // PO flag
            _computer.CPU.FLAGS.SetValueByIndexFast(5, val1 == val2);    // EQ flag
            _computer.CPU.FLAGS.SetValueByIndexFast(6, val1 > val2);    // GT flag
            _computer.CPU.FLAGS.SetValueByIndexFast(7, val1 < val2);    // LT flag
        }

        private static void Compare16bitSigned(short val1, short val2)
        {
            var subtracted = val1 - val2;
            _computer.CPU.FLAGS.SetValueByIndexFast(0, val1 == val2);    // Z flag
            //_computer.CPU.FLAGS.SetValueByIndexFast(1, val1 < val2);    // C flag
            _computer.CPU.FLAGS.SetValueByIndexFast(2, subtracted < 0);    // SN flag
            _computer.CPU.FLAGS.SetValueByIndexFast(3, false);    // OV flag
            //_computer.CPU.FLAGS.SetValueByIndexFast(4, !(val1 > 0x8000 && val2 > 0x8000 && (sbyte)subtracted > 0) || (val1 < 0x8000 && val2 < 0x8000 && (sbyte)subtracted < 0));    // PO flag
            _computer.CPU.FLAGS.SetValueByIndexFast(5, val1 == val2);    // EQ flag
            _computer.CPU.FLAGS.SetValueByIndexFast(6, val1 > val2);    // GT flag
            _computer.CPU.FLAGS.SetValueByIndexFast(7, val1 < val2);    // LT flag
        }

        private static void Compare24bitSigned(int val1, int val2)
        {
            var subtracted = val1 - val2;
            _computer.CPU.FLAGS.SetValueByIndexFast(0, val1 == val2);    // Z flag
            //_computer.CPU.FLAGS.SetValueByIndexFast(1, val1 < val2);    // C flag
            _computer.CPU.FLAGS.SetValueByIndexFast(2, subtracted < 0);    // SN flag
            _computer.CPU.FLAGS.SetValueByIndexFast(3, false);    // OV flag
            //_computer.CPU.FLAGS.SetValueByIndexFast(4, !(val1 > 0x800000 && val2 > 0x800000 && (sbyte)subtracted > 0) || (val1 < 0x800000 && val2 < 0x800000 && (sbyte)subtracted < 0));    // PO flag
            _computer.CPU.FLAGS.SetValueByIndexFast(5, val1 == val2);    // EQ flag
            _computer.CPU.FLAGS.SetValueByIndexFast(6, val1 > val2);    // GT flag
            _computer.CPU.FLAGS.SetValueByIndexFast(7, val1 < val2);    // LT flag
        }

        private static void Compare32bitSigned(int val1, int val2)
        {
            var subtracted = val1 - val2;
            _computer.CPU.FLAGS.SetValueByIndexFast(0, val1 == val2);    // Z flag
            //_computer.CPU.FLAGS.SetValueByIndexFast(1, val1 < val2);    // C flag
            _computer.CPU.FLAGS.SetValueByIndexFast(2, subtracted < 0);    // SN flag
            _computer.CPU.FLAGS.SetValueByIndexFast(3, false);    // OV flag
            //_computer.CPU.FLAGS.SetValueByIndexFast(4, !(val1 > 0x80000000 && val2 > 0x80000000 && (sbyte)subtracted > 0) || (val1 < 0x80000000 && val2 < 0x80000000 && (sbyte)subtracted < 0));    // PO flag
            _computer.CPU.FLAGS.SetValueByIndexFast(5, val1 == val2);    // EQ flag
            _computer.CPU.FLAGS.SetValueByIndexFast(6, val1 > val2);    // GT flag
            _computer.CPU.FLAGS.SetValueByIndexFast(7, val1 < val2);    // LT flag
        }
    }
}
