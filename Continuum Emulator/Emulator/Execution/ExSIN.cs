using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;
using System;

namespace Continuum93.Emulator.Execution
{
    public class ExSIN
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 4);
            switch (upperLdOp)
            {
                case Mnem.SINCTC_fr:
                    {
                        byte fRegPointer = (byte)(ldOp & 0b_0000_1111);
                        float fRegValue = computer.CPU.FREGS.GetRegister(fRegPointer);

                        computer.CPU.FREGS.SetRegister(fRegPointer, MathF.Sin(fRegValue));

                        return;
                    }
                case Mnem.SINCTC_fr_fr:
                    {
                        byte mixedReg = computer.MEMC.Fetch();

                        byte fReg1 = (byte)(ldOp & 0b_0000_1111);
                        byte fReg2 = (byte)(mixedReg & 0b_0000_1111);
                        float fSecondRegValue = computer.CPU.FREGS.GetRegister(fReg2);

                        computer.CPU.FREGS.SetRegister(fReg1, MathF.Sin(fSecondRegValue));

                        return;
                    }
            }
        }
    }
}
