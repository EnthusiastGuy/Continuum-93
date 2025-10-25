using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;
using Continuum93.Tools;
using System;

namespace Continuum93.Emulator.Execution
{
    public static class ExMIN
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 2);

            switch (upperLdOp)
            {
                case Mnem.MIAX_r_r:   // MIN r, r
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte reg1Index = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte reg2Index = (byte)(mixedReg & 0b00011111);
                        byte reg1Val = computer.CPU.REGS.Get8BitRegister(reg1Index);
                        byte reg2Val = computer.CPU.REGS.Get8BitRegister(reg2Index);

                        computer.CPU.REGS.Set8BitRegister(reg1Index, Math.Min(reg1Val, reg2Val));

                        return;
                    }
                case Mnem.MIAX_rr_rr:   // MIN rr, rr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte reg1Index = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte reg2Index = (byte)(mixedReg & 0b00011111);
                        ushort reg1Val = computer.CPU.REGS.Get16BitRegister(reg1Index);
                        ushort reg2Val = computer.CPU.REGS.Get16BitRegister(reg2Index);

                        computer.CPU.REGS.Set16BitRegister(reg1Index, Math.Min(reg1Val, reg2Val));

                        return;
                    }
                case Mnem.MIAX_rrr_rrr:   // MIN rrr, rrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte reg1Index = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte reg2Index = (byte)(mixedReg & 0b00011111);
                        uint reg1Val = computer.CPU.REGS.Get24BitRegister(reg1Index);
                        uint reg2Val = computer.CPU.REGS.Get24BitRegister(reg2Index);

                        computer.CPU.REGS.Set24BitRegister(reg1Index, Math.Min(reg1Val, reg2Val));

                        return;
                    }
                case Mnem.MIAX_rrrr_rrrr:   // MIN rrrr, rrrr
                    {
                        byte mixedReg = computer.MEMC.Fetch();
                        byte reg1Index = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
                        byte reg2Index = (byte)(mixedReg & 0b00011111);
                        uint reg1Val = computer.CPU.REGS.Get32BitRegister(reg1Index);
                        uint reg2Val = computer.CPU.REGS.Get32BitRegister(reg2Index);

                        computer.CPU.REGS.Set32BitRegister(reg1Index, Math.Min(reg1Val, reg2Val));

                        return;
                    }
                case Mnem.MIAX_r_n:   // MIN r, n
                    {
                        byte reg = computer.MEMC.Fetch();
                        byte reg1Index = (byte)(reg & 0b00011111);
                        byte reg1Val = computer.CPU.REGS.Get8BitRegister(reg1Index);
                        byte reg2Val = computer.MEMC.Fetch();

                        computer.CPU.REGS.Set8BitRegister(reg1Index, Math.Min(reg1Val, reg2Val));

                        return;
                    }
                case Mnem.MIAX_rr_nn:   // MIN rr, nn
                    {
                        byte reg = computer.MEMC.Fetch();
                        byte reg1Index = (byte)(reg & 0b00011111);
                        ushort reg1Val = computer.CPU.REGS.Get16BitRegister(reg1Index);
                        ushort reg2Val = computer.MEMC.Fetch16();

                        computer.CPU.REGS.Set16BitRegister(reg1Index, Math.Min(reg1Val, reg2Val));

                        return;
                    }
                case Mnem.MIAX_rrr_nnn:   // MIN rrr, nnn
                    {
                        byte reg = computer.MEMC.Fetch();
                        byte reg1Index = (byte)(reg & 0b00011111);
                        uint reg1Val = computer.CPU.REGS.Get24BitRegister(reg1Index);
                        uint reg2Val = computer.MEMC.Fetch24();

                        computer.CPU.REGS.Set24BitRegister(reg1Index, Math.Min(reg1Val, reg2Val));

                        return;
                    }
                case Mnem.MIAX_rrrr_nnnn:   // MIN rrrr, nnnn
                    {
                        byte reg = computer.MEMC.Fetch();
                        byte reg1Index = (byte)(reg & 0b00011111);
                        uint reg1Val = computer.CPU.REGS.Get32BitRegister(reg1Index);
                        uint reg2Val = computer.MEMC.Fetch32();

                        computer.CPU.REGS.Set32BitRegister(reg1Index, Math.Min(reg1Val, reg2Val));

                        return;
                    }
                // Float
                case Mnem.MIAX_fr_n:   // MIN fr, n
                    {
                        byte reg = computer.MEMC.Fetch();
                        byte fRegIndex = (byte)(reg & 0b00001111);
                        float fRegVal = computer.CPU.FREGS.GetRegister(fRegIndex);
                        uint floatBitValue = computer.MEMC.Fetch32();
                        float floatValue = FloatPointUtils.UintToFloat(floatBitValue);

                        computer.CPU.FREGS.SetRegister(fRegIndex, MathF.Min(fRegVal, floatValue));

                        return;
                    }
                case Mnem.MIAX_fr_fr:   // MIN fr, fr
                    {
                        byte reg = computer.MEMC.Fetch();
                        byte fReg1Index = (byte)(reg & 0b11110000);
                        float fReg1Val = computer.CPU.FREGS.GetRegister(fReg1Index);
                        byte fReg2Index = (byte)(reg & 0b00001111);
                        float fReg2Val = computer.CPU.FREGS.GetRegister(fReg2Index);

                        computer.CPU.FREGS.SetRegister(fReg1Index, MathF.Min(fReg1Val, fReg2Val));

                        return;
                    }
                case Mnem.MIAX_r_fr:   // MIN r, fr
                    {
                        byte reg = computer.MEMC.Fetch();

                        byte r1Index = (byte)(((ldOp << 5) & 0b00011111) + (reg >> 4));
                        float fReg1Val = computer.CPU.REGS.Get8BitRegister(r1Index);
                        byte fReg2Index = (byte)(reg & 0b00001111);
                        float fReg2Val = Math.Abs(computer.CPU.FREGS.GetRegister(fReg2Index));

                        computer.CPU.REGS.Set8BitRegister(r1Index, (byte)MathF.Min(fReg1Val, fReg2Val));

                        return;
                    }
                case Mnem.MIAX_rr_fr:   // MIN rr, fr
                    {
                        byte reg = computer.MEMC.Fetch();

                        byte r1Index = (byte)(((ldOp << 5) & 0b00011111) + (reg >> 4));
                        float fReg1Val = computer.CPU.REGS.Get16BitRegister(r1Index);
                        byte fReg2Index = (byte)(reg & 0b00001111);
                        float fReg2Val = Math.Abs(computer.CPU.FREGS.GetRegister(fReg2Index));

                        computer.CPU.REGS.Set16BitRegister(r1Index, (ushort)MathF.Min(fReg1Val, fReg2Val));

                        return;
                    }
                case Mnem.MIAX_rrr_fr:   // MIN rrr, fr
                    {
                        byte reg = computer.MEMC.Fetch();

                        byte r1Index = (byte)(((ldOp << 5) & 0b00011111) + (reg >> 4));
                        float fReg1Val = computer.CPU.REGS.Get24BitRegister(r1Index);
                        byte fReg2Index = (byte)(reg & 0b00001111);
                        float fReg2Val = Math.Abs(computer.CPU.FREGS.GetRegister(fReg2Index));

                        computer.CPU.REGS.Set24BitRegister(r1Index, (uint)MathF.Min(fReg1Val, fReg2Val));

                        return;
                    }
                case Mnem.MIAX_rrrr_fr:   // MIN rrrr, fr
                    {
                        byte reg = computer.MEMC.Fetch();

                        byte r1Index = (byte)(((ldOp << 5) & 0b00011111) + (reg >> 4));
                        float fReg1Val = computer.CPU.REGS.Get32BitRegister(r1Index);
                        byte fReg2Index = (byte)(reg & 0b00001111);
                        float fReg2Val = Math.Abs(computer.CPU.FREGS.GetRegister(fReg2Index));

                        computer.CPU.REGS.Set32BitRegister(r1Index, (uint)MathF.Min(fReg1Val, fReg2Val));

                        return;
                    }

                case Mnem.MIAX_fr_r:   // MIN fr, r
                    {
                        byte reg = computer.MEMC.Fetch();

                        byte r1Index = (byte)(((ldOp << 5) & 0b00011111) + (reg >> 4));
                        float fReg1Val = computer.CPU.REGS.Get8BitRegister(r1Index);
                        byte fReg2Index = (byte)(reg & 0b00001111);
                        float fReg2Val = Math.Abs(computer.CPU.FREGS.GetRegister(fReg2Index));

                        computer.CPU.FREGS.SetRegister(fReg2Index, MathF.Min(fReg1Val, fReg2Val));

                        return;
                    }
                case Mnem.MIAX_fr_rr:   // MIN fr, rr
                    {
                        byte reg = computer.MEMC.Fetch();

                        byte r1Index = (byte)(((ldOp << 5) & 0b00011111) + (reg >> 4));
                        float fReg1Val = computer.CPU.REGS.Get16BitRegister(r1Index);
                        byte fReg2Index = (byte)(reg & 0b00001111);
                        float fReg2Val = Math.Abs(computer.CPU.FREGS.GetRegister(fReg2Index));

                        computer.CPU.FREGS.SetRegister(fReg2Index, MathF.Min(fReg1Val, fReg2Val));

                        return;
                    }
                case Mnem.MIAX_fr_rrr:   // MIN fr, rrr
                    {
                        byte reg = computer.MEMC.Fetch();

                        byte r1Index = (byte)(((ldOp << 5) & 0b00011111) + (reg >> 4));
                        float fReg1Val = computer.CPU.REGS.Get24BitRegister(r1Index);
                        byte fReg2Index = (byte)(reg & 0b00001111);
                        float fReg2Val = Math.Abs(computer.CPU.FREGS.GetRegister(fReg2Index));

                        computer.CPU.FREGS.SetRegister(fReg2Index, MathF.Min(fReg1Val, fReg2Val));

                        return;
                    }
                case Mnem.MIAX_fr_rrrr:   // MIN fr, rrrr
                    {
                        byte reg = computer.MEMC.Fetch();

                        byte r1Index = (byte)(((ldOp << 5) & 0b00011111) + (reg >> 4));
                        float fReg1Val = computer.CPU.REGS.Get32BitRegister(r1Index);
                        byte fReg2Index = (byte)(reg & 0b00001111);
                        float fReg2Val = Math.Abs(computer.CPU.FREGS.GetRegister(fReg2Index));

                        computer.CPU.FREGS.SetRegister(fReg2Index, MathF.Min(fReg1Val, fReg2Val));

                        return;
                    }

            }
        }
    }
}
