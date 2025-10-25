using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public static class ExLDF
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 5);
            switch (upperLdOp)
            {
                case Mnem.LDF_r:            // LDF r
                    {
                        byte regIndex = (byte)(ldOp & 0b00011111);
                        byte flagsValue = computer.CPU.FLAGS.GetFlagsByte();
                        computer.CPU.REGS.Set8BitRegister(regIndex, flagsValue);
                        return;
                    }
                case Mnem.LDF_IrrrI:        // LDF (rrr)
                    {
                        byte regIndex = (byte)(ldOp & 0b00011111);
                        byte flagsValue = computer.CPU.FLAGS.GetFlagsByte();
                        uint regAddressValue = computer.CPU.REGS.Get24BitRegister(regIndex);
                        computer.MEMC.Set8bitToRAM(regAddressValue, flagsValue);
                        return;
                    }
                case Mnem.LDF_InnnI:        // LDF (nnn)
                    {
                        byte flagsValue = computer.CPU.FLAGS.GetFlagsByte();
                        uint addressValue = computer.MEMC.Fetch24();
                        computer.MEMC.Set8bitToRAM(addressValue, flagsValue);
                        return;
                    }
                case Mnem.LDF_r_n:          // LDF r, n
                    {
                        byte regIndex = (byte)(ldOp & 0b00011111);
                        byte flagsValue = computer.CPU.FLAGS.GetFlagsByte();
                        byte mask = computer.MEMC.Fetch();
                        computer.CPU.REGS.Set8BitRegister(regIndex, (byte)(flagsValue & mask));
                        return;
                    }
                case Mnem.LDF_IrrrI_n:      // LDF (rrr), n
                    {
                        byte regIndex = (byte)(ldOp & 0b00011111);
                        byte flagsValue = computer.CPU.FLAGS.GetFlagsByte();
                        uint regAddressValue = computer.CPU.REGS.Get24BitRegister(regIndex);
                        byte mask = computer.MEMC.Fetch();
                        computer.MEMC.Set8bitToRAM(regAddressValue, (byte)(flagsValue & mask));
                        return;
                    }
                case Mnem.LDF_InnnI_n:      // LDF (nnn), n
                    {
                        byte flagsValue = computer.CPU.FLAGS.GetFlagsByte();
                        uint addressValue = computer.MEMC.Fetch24();
                        byte mask = computer.MEMC.Fetch();
                        computer.MEMC.Set8bitToRAM(addressValue, (byte)(flagsValue & mask));
                        return;
                    }
            }
        }
    }
}
