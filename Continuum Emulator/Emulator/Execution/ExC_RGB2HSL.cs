using Continuum93.Emulator.Colors;
using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public class ExC_RGB2HSL
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();

            switch (ldOp)
            {
                case Mnem.RGB2HSLB_nnn_rrrr:     // RGB2HSL nnn, rrrr
                    {
                        uint value = computer.MEMC.Fetch24();
                        byte destRegIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        computer.CPU.REGS.Set32BitRegister(destRegIndex, ColorConverter.RGBToHSL(value));

                        return;
                    }
                case Mnem.RGB2HSLB_rrr_rrrr:     // RGB2HSL rrr, rrrr
                    {
                        uint value = computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));
                        byte destRegIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        computer.CPU.REGS.Set32BitRegister(destRegIndex, ColorConverter.RGBToHSL(value));

                        return;
                    }
                case Mnem.RGB2HSLB_nnn_InnnI:    // RGB2HSL nnn, (nnn)
                    {
                        uint value = computer.MEMC.Fetch24();
                        uint address = computer.MEMC.Fetch24();

                        computer.MEMC.Set32bitToRAM(address, ColorConverter.RGBToHSL(value));

                        return;
                    }
                case Mnem.RGB2HSLB_rrr_InnnI:    // RGB2HSL rrr, (nnn)
                    {
                        uint value = computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));
                        uint address = computer.MEMC.Fetch24();

                        computer.MEMC.Set32bitToRAM(address, ColorConverter.RGBToHSL(value));

                        return;
                    }
                case Mnem.RGB2HSLB_nnn_IrrrI:    // RGB2HSL nnn, (rrr)
                    {
                        uint value = computer.MEMC.Fetch24();
                        uint address = computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.MEMC.Set32bitToRAM(address, ColorConverter.RGBToHSL(value));

                        return;
                    }
                case Mnem.RGB2HSLB_rrr_IrrrI:    // RGB2HSL rrr, (rrr)
                    {
                        uint value = computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));
                        uint address = computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.MEMC.Set32bitToRAM(address, ColorConverter.RGBToHSL(value));

                        return;
                    }

                case Mnem.RGB2HSLB_InnnI_rrrr:   // RGB2HSL (nnn), rrrr
                    {
                        uint value = computer.MEMC.Get24bitFromRAM(computer.MEMC.Fetch24());
                        byte destRegIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        computer.CPU.REGS.Set32BitRegister(destRegIndex, ColorConverter.RGBToHSL(value));

                        return;
                    }
                case Mnem.RGB2HSLB_IrrrI_rrrr:   // RGB2HSL (rrr), rrrr
                    {
                        uint value = computer.MEMC.Get24bitFromRAM(computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111)));
                        byte destRegIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        computer.CPU.REGS.Set32BitRegister(destRegIndex, ColorConverter.RGBToHSL(value));

                        return;
                    }
                case Mnem.RGB2HSLB_InnnI_InnnI:    // RGB2HSL (nnn), (nnn)
                    {
                        uint value = computer.MEMC.Get24bitFromRAM(computer.MEMC.Fetch24());
                        uint address = computer.MEMC.Fetch24();

                        computer.MEMC.Set32bitToRAM(address, ColorConverter.RGBToHSL(value));

                        return;
                    }
                case Mnem.RGB2HSLB_IrrrI_InnnI:    // RGB2HSL (rrr), (nnn)
                    {
                        uint value = computer.MEMC.Get24bitFromRAM(computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111)));
                        uint address = computer.MEMC.Fetch24();

                        computer.MEMC.Set32bitToRAM(address, ColorConverter.RGBToHSL(value));

                        return;
                    }
                case Mnem.RGB2HSLB_InnnI_IrrrI:    // RGB2HSL (nnn), (rrr)
                    {
                        uint value = computer.MEMC.Get24bitFromRAM(computer.MEMC.Fetch24());
                        uint address = computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.MEMC.Set32bitToRAM(address, ColorConverter.RGBToHSL(value));

                        return;
                    }
                case Mnem.RGB2HSLB_IrrrI_IrrrI:    // RGB2HSL (rrr), (rrr)
                    {
                        uint value = computer.MEMC.Get24bitFromRAM(computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111)));
                        uint address = computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.MEMC.Set32bitToRAM(address, ColorConverter.RGBToHSL(value));

                        return;
                    }
            }
        }
    }
}
