using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public static class ExVCL
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 6);
            switch (upperLdOp)
            {
                case Mnem.VDCL_n:
                    {
                        byte value = computer.MEMC.Fetch();
                        computer.GRAPHICS.ManualClearBackBuffer(value);

                        return;
                    }
                case Mnem.VDCL_r:
                    {
                        byte regIndex = (byte)(ldOp & 0b00011111);
                        byte regValue = computer.CPU.REGS.Get8BitRegister(regIndex);
                        computer.GRAPHICS.ManualClearBackBuffer(regValue);

                        return;
                    }
                case Mnem.VDCL_InnnI:
                    {
                        uint address = computer.MEMC.Fetch24();
                        computer.GRAPHICS.ManualClearBackBuffer(computer.MEMC.Get8bitFromRAM(address));

                        return;
                    }
                case Mnem.VDCL_IrrrI:
                    {
                        byte regIndex = (byte)(ldOp & 0b00011111);
                        uint address = computer.CPU.REGS.Get24BitRegister(regIndex);
                        computer.GRAPHICS.ManualClearBackBuffer(computer.MEMC.Get8bitFromRAM(address));

                        return;
                    }
            }
        }
    }
}
