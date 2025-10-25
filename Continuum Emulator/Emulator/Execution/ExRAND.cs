using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;
using System;

namespace Continuum93.Emulator.Execution
{
    public class ExRAND
    {
        private static Random RNG = new();

        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            //byte upperLdOp = (byte)(ldOp >> 5);

            switch (ldOp)
            {
                case Mnem.RAND_r:   // RAND r
                    {
                        byte regIndex = computer.MEMC.Fetch();
                        byte regValue = computer.CPU.REGS.Get8BitRegister(regIndex);
                        computer.CPU.REGS.Set8BitRegister(regIndex, (byte)RNG.Next(0, regValue == 0 ? 0xFF : regValue));

                        return;
                    }
                case Mnem.RAND_rr:   // RAND rr
                    {
                        byte regIndex = computer.MEMC.Fetch();
                        ushort regValue = computer.CPU.REGS.Get16BitRegister(regIndex);
                        computer.CPU.REGS.Set16BitRegister(regIndex, (ushort)RNG.Next(0, regValue == 0 ? 0xFFFF : regValue));

                        return;
                    }
                case Mnem.RAND_rrr:   // RAND rrr
                    {
                        byte regIndex = computer.MEMC.Fetch();
                        uint regValue = computer.CPU.REGS.Get24BitRegister(regIndex);
                        computer.CPU.REGS.Set24BitRegister(regIndex, (uint)RNG.Next(0, (int)(regValue == 0 ? 0xFFFFFF : regValue)));

                        return;
                    }
                case Mnem.RAND_rrrr:   // RAND rrrr
                    {
                        byte regIndex = computer.MEMC.Fetch();
                        uint regValue = computer.CPU.REGS.Get32BitRegister(regIndex);

                        int regValueHalf = regValue == 0 ? int.MaxValue / 2 : (int)(regValue / 2);

                        uint randVal = (uint)(RNG.Next(-regValueHalf, regValueHalf) + regValueHalf);

                        computer.CPU.REGS.Set32BitRegister(regIndex, randVal);
                        return;
                    }
                case Mnem.RAND_r_n:   // RAND r, n
                    {
                        byte regIndex = computer.MEMC.Fetch();
                        byte value = computer.MEMC.Fetch();

                        computer.CPU.REGS.Set8BitRegister(regIndex, (byte)RNG.Next(0, value));

                        return;
                    }
                case Mnem.RAND_rr_nn:   // RAND rr, nn
                    {
                        byte regIndex = computer.MEMC.Fetch();
                        ushort value = computer.MEMC.Fetch16();

                        computer.CPU.REGS.Set16BitRegister(regIndex, (ushort)RNG.Next(0, value));

                        return;
                    }
                case Mnem.RAND_rrr_nnn:   // RAND rrr, nnn
                    {
                        byte regIndex = computer.MEMC.Fetch();
                        uint value = computer.MEMC.Fetch24();

                        computer.CPU.REGS.Set24BitRegister(regIndex, (uint)RNG.Next(0, (int)value));

                        return;
                    }
                case Mnem.RAND_rrrr_nnnn:   // RAND rrrr, nnnn
                    {
                        byte regIndex = computer.MEMC.Fetch();
                        uint value = computer.MEMC.Fetch32();

                        int regValueHalf = (int)(value / 2);

                        uint randVal = (uint)(RNG.Next(-regValueHalf, regValueHalf) + regValueHalf);

                        computer.CPU.REGS.Set32BitRegister(regIndex, randVal);
                        return;
                    }
                case Mnem.RAND_fr:
                    {
                        byte freg = computer.MEMC.Fetch();
                        byte fRegIndex = (byte)(freg & 0b00001111);

                        computer.CPU.FREGS.SetRegister(fRegIndex, (float)RNG.NextDouble());
                        return;
                    }
                case Mnem.RAND_fr_nnnn:
                    {
                        byte freg = computer.MEMC.Fetch();
                        byte fRegIndex = (byte)(freg & 0b00001111);
                        int seed = computer.MEMC.Fetch32Signed();

                        RNG = new Random(seed);

                        computer.CPU.FREGS.SetRegister(fRegIndex, (float)RNG.NextDouble());
                        return;
                    }
                case Mnem.RAND_fr_rrrr:
                    {
                        byte freg = computer.MEMC.Fetch();
                        byte fRegIndex = (byte)(freg & 0b00001111);
                        byte sReg = computer.MEMC.Fetch();
                        int seed = computer.CPU.REGS.Get32BitRegisterSigned(sReg);

                        RNG = new Random(seed);

                        computer.CPU.FREGS.SetRegister(fRegIndex, (float)RNG.NextDouble());
                        return;
                    }
            }
        }
    }
}
