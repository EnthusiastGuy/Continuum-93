using Continuum93.Emulator.Mnemonics;
using Continuum93.Tools;
using System;

namespace Continuum93.Emulator.Execution
{
    public static class ExPOW
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 2);

            switch (upperLdOp)
            {
                case Mnem.POW_fr_fr:   // POW fr, fr
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        byte fReg1 = (byte)(mixedReg >> 4);
                        byte fReg2 = (byte)(mixedReg & 0b00001111);

                        float fReg1Value = computer.CPU.FREGS.GetRegister(fReg1);
                        float fReg2Value = computer.CPU.FREGS.GetRegister(fReg2);

                        computer.CPU.FREGS.SetRegister(fReg1, MathF.Pow(fReg1Value, fReg2Value));
                        return;
                    }
                case Mnem.POW_fr_r:   // POW fr, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        byte r1Index = (byte)(((ldOp << 5) & 0b00011111) + (mixedReg >> 4));
                        byte fRegIndex = (byte)(mixedReg & 0b00001111);

                        byte reg1Val = computer.CPU.REGS.Get8BitRegister(r1Index);
                        float fRegVal = computer.CPU.FREGS.GetRegister(fRegIndex);

                        computer.CPU.FREGS.SetRegister(fRegIndex, MathF.Pow(fRegVal, reg1Val));
                        return;
                    }
                case Mnem.POW_fr_rr:   // POW fr, rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        byte r1Index = (byte)(((ldOp << 5) & 0b00011111) + (mixedReg >> 4));
                        byte fRegIndex = (byte)(mixedReg & 0b00001111);

                        ushort reg1Val = computer.CPU.REGS.Get16BitRegister(r1Index);
                        float fRegVal = computer.CPU.FREGS.GetRegister(fRegIndex);

                        computer.CPU.FREGS.SetRegister(fRegIndex, MathF.Pow(fRegVal, reg1Val));
                        return;
                    }
                case Mnem.POW_fr_rrr:   // POW fr, rrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        byte r1Index = (byte)(((ldOp << 5) & 0b00011111) + (mixedReg >> 4));
                        byte fRegIndex = (byte)(mixedReg & 0b00001111);

                        uint reg1Val = computer.CPU.REGS.Get24BitRegister(r1Index);
                        float fRegVal = computer.CPU.FREGS.GetRegister(fRegIndex);

                        computer.CPU.FREGS.SetRegister(fRegIndex, MathF.Pow(fRegVal, reg1Val));
                        return;
                    }
                case Mnem.POW_fr_rrrr:   // POW fr, rrrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        byte r1Index = (byte)(((ldOp << 5) & 0b00011111) + (mixedReg >> 4));
                        byte fRegIndex = (byte)(mixedReg & 0b00001111);

                        uint reg1Val = computer.CPU.REGS.Get32BitRegister(r1Index);
                        float fRegVal = computer.CPU.FREGS.GetRegister(fRegIndex);

                        computer.CPU.FREGS.SetRegister(fRegIndex, MathF.Pow(fRegVal, reg1Val));
                        return;
                    }
                case Mnem.POW_r_fr:   // POW r, fr
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        byte r1Index = (byte)(((ldOp << 5) & 0b00011111) + (mixedReg >> 4));
                        byte fRegIndex = (byte)(mixedReg & 0b00001111);

                        byte reg1Val = computer.CPU.REGS.Get8BitRegister(r1Index);
                        float fRegVal = computer.CPU.FREGS.GetRegister(fRegIndex);

                        computer.CPU.REGS.Set8BitRegister(r1Index, (byte)MathF.Pow(reg1Val, fRegVal));
                        return;
                    }
                case Mnem.POW_rr_fr:   // POW rr, fr
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        byte r1Index = (byte)(((ldOp << 5) & 0b00011111) + (mixedReg >> 4));
                        byte fRegIndex = (byte)(mixedReg & 0b00001111);

                        ushort reg1Val = computer.CPU.REGS.Get16BitRegister(r1Index);
                        float fRegVal = computer.CPU.FREGS.GetRegister(fRegIndex);

                        computer.CPU.REGS.Set16BitRegister(r1Index, (ushort)MathF.Pow(reg1Val, fRegVal));
                        return;
                    }
                case Mnem.POW_rrr_fr:   // POW rrr, fr
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        byte r1Index = (byte)(((ldOp << 5) & 0b00011111) + (mixedReg >> 4));
                        byte fRegIndex = (byte)(mixedReg & 0b00001111);

                        uint reg1Val = computer.CPU.REGS.Get24BitRegister(r1Index);
                        float fRegVal = computer.CPU.FREGS.GetRegister(fRegIndex);

                        computer.CPU.REGS.Set24BitRegister(r1Index, (uint)MathF.Pow(reg1Val, fRegVal));
                        return;
                    }
                case Mnem.POW_rrrr_fr:   // POW rrrr, fr
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        byte r1Index = (byte)(((ldOp << 5) & 0b00011111) + (mixedReg >> 4));
                        byte fRegIndex = (byte)(mixedReg & 0b00001111);

                        uint reg1Val = computer.CPU.REGS.Get32BitRegister(r1Index);
                        float fRegVal = computer.CPU.FREGS.GetRegister(fRegIndex);

                        computer.CPU.REGS.Set32BitRegister(r1Index, (uint)MathF.Pow(reg1Val, fRegVal));
                        return;
                    }
                case Mnem.POW_fr_IrrrI:   // POW fr, (rrr)
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        byte r1Index = (byte)(((ldOp << 5) & 0b00011111) + (mixedReg >> 4));
                        byte fRegIndex = (byte)(mixedReg & 0b00001111);

                        uint reg1Val = computer.CPU.REGS.Get24BitRegister(r1Index);
                        float reg1AddressVal = computer.MEMC.GetFloatFromRAM(reg1Val);
                        float fRegVal = computer.CPU.FREGS.GetRegister(fRegIndex);

                        computer.CPU.FREGS.SetRegister(fRegIndex, MathF.Pow(fRegVal, reg1AddressVal));
                        return;
                    }
                case Mnem.POW_IrrrI_fr:   // POW (rrr), fr
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        byte r1Index = (byte)(((ldOp << 5) & 0b00011111) + (mixedReg >> 4));
                        byte fRegIndex = (byte)(mixedReg & 0b00001111);

                        uint reg1Val = computer.CPU.REGS.Get24BitRegister(r1Index);
                        float reg1AddressVal = computer.MEMC.GetFloatFromRAM(reg1Val);
                        float fRegVal = computer.CPU.FREGS.GetRegister(fRegIndex);

                        computer.MEMC.SetFloatToRam(reg1Val, MathF.Pow(reg1AddressVal, fRegVal));
                        return;
                    }
                case Mnem.POW_fr_nnn:   // POW fr, n
                    {
                        byte reg = computer.MEMC.Fetch();

                        byte fRegIndex = (byte)(reg >> 4);
                        float fRegVal = computer.CPU.FREGS.GetRegister(fRegIndex);
                        uint floatBitValue = computer.MEMC.Fetch32();
                        float floatValue = FloatPointUtils.UintToFloat(floatBitValue);

                        computer.CPU.FREGS.SetRegister(fRegIndex, MathF.Pow(fRegVal, floatValue));
                        return;
                    }
                case Mnem.POW_fr_InnnI:   // POW fr, (nnn)
                    {
                        byte reg = computer.MEMC.Fetch();

                        byte fRegIndex = (byte)(reg >> 4);
                        float fRegVal = computer.CPU.FREGS.GetRegister(fRegIndex);
                        uint addressValue = computer.MEMC.Fetch24();
                        float floatValue = computer.MEMC.GetFloatFromRAM(addressValue);

                        computer.CPU.FREGS.SetRegister(fRegIndex, MathF.Pow(fRegVal, floatValue));
                        return;
                    }
                case Mnem.POW_InnnI_fr:   // POW (nnn), fr
                    {
                        byte reg = computer.MEMC.Fetch();

                        byte fRegIndex = (byte)(reg >> 4);
                        float fRegVal = computer.CPU.FREGS.GetRegister(fRegIndex);
                        uint addressValue = computer.MEMC.Fetch24();
                        float floatValue = computer.MEMC.GetFloatFromRAM(addressValue);

                        computer.MEMC.SetFloatToRam(addressValue, MathF.Pow(floatValue, fRegVal));
                        return;
                    }
            }
        }
    }
}
