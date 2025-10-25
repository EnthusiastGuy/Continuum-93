using Continuum93.Emulator;
using System;

namespace Continuum93.Emulator.Interrupts
{
    public static class InterruptsRandom
    {
        static readonly Random RNG = new();
        public static void Random8Bit(byte regId, Computer computer)
        {
            computer.CPU.REGS.Set8BitRegister(regId, (byte)RNG.Next(0, 0xFF));
        }

        public static void Random8BitCustom(byte regId, Computer computer)
        {
            byte rIndex = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte rValue = computer.CPU.REGS.Get8BitRegister(rIndex);

            computer.CPU.REGS.Set8BitRegister(rIndex, (byte)RNG.Next(0, rValue));
        }

        public static void Random16Bit(byte regId, Computer computer)
        {
            computer.CPU.REGS.Set16BitRegister(regId, (ushort)RNG.Next(0, 0xFFFF));
        }

        public static void Random16BitCustom(byte regId, Computer computer)
        {
            byte rIndex = computer.CPU.REGS.GetNextRegister(regId, 1);
            ushort rValue = computer.CPU.REGS.Get16BitRegister(rIndex);

            computer.CPU.REGS.Set16BitRegister(rIndex, (ushort)RNG.Next(0, rValue));
        }

        public static void Random24Bit(byte regId, Computer computer)
        {
            computer.CPU.REGS.Set24BitRegister(regId, (uint)RNG.Next(0, 0xFFFFFF));
        }

        public static void Random24BitCustom(byte regId, Computer computer)
        {
            byte rIndex = computer.CPU.REGS.GetNextRegister(regId, 1);
            uint rValue = computer.CPU.REGS.Get24BitRegister(rIndex);

            computer.CPU.REGS.Set24BitRegister(rIndex, (uint)RNG.Next(0, (int)rValue));
        }

        public static void Random32Bit(byte regId, Computer computer)
        {
            computer.CPU.REGS.Set32BitRegister(regId, (uint)RNG.Next(-int.MaxValue, int.MaxValue));
        }

        public static void Random32BitCustom(byte regId, Computer computer)
        {
            byte rIndex = computer.CPU.REGS.GetNextRegister(regId, 1);
            uint rValueHalf = computer.CPU.REGS.Get32BitRegister(rIndex) / 2;

            computer.CPU.REGS.Set32BitRegister(rIndex, (uint)RNG.Next((int)-rValueHalf, (int)rValueHalf));
        }
    }
}
