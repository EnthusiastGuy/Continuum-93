using Continuum93.Emulator.Mnemonics;
using System;

namespace Continuum93.Emulator.Execution
{
    public static class ExFLOOR
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();

            switch (ldOp)
            {
                case Mnem.SINCTC_fr:
                    {
                        byte fRegPointer = (byte)(computer.MEMC.Fetch() & 0b_0000_1111);
                        float fRegValue = computer.CPU.FREGS.GetRegister(fRegPointer);

                        computer.CPU.FREGS.SetRegister(fRegPointer, MathF.Floor(fRegValue));

                        return;
                    }
                case Mnem.SINCTC_fr_fr:
                    {
                        byte fReg1pointer = (byte)(computer.MEMC.Fetch() & 0b_0000_1111);
                        byte fReg2pointer = (byte)(computer.MEMC.Fetch() & 0b_0000_1111);
                        float fSecondRegValue = computer.CPU.FREGS.GetRegister(fReg2pointer);

                        computer.CPU.FREGS.SetRegister(fReg1pointer, MathF.Floor(fSecondRegValue));

                        return;
                    }
            }
        }
    }
}
