using Continuum93.Emulator.Colors;
using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public class ExC_HSL2RGB
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();

            switch (ldOp)
            {
                case Mnem.HSLB2RGB_nnnn_rrr:     // HSL2RGB nnnn, rrr
                    {
                        uint value = computer.MEMC.Fetch32();
                        byte destRegIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        computer.CPU.REGS.Set24BitRegister(destRegIndex, ColorConverter.HSLToRGB(value));

                        return;
                    }
                case Mnem.HSLB2RGB_rrrr_rrr:     // HSL2RGB rrrr, rrr
                    {
                        uint value = computer.CPU.REGS.Get32BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));
                        byte destRegIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        computer.CPU.REGS.Set24BitRegister(destRegIndex, ColorConverter.HSLToRGB(value));

                        return;
                    }
                case Mnem.HSLB2RGB_nnnn_InnnI:    // HSL2RGB nnnn, (nnn)
                    {
                        uint value = computer.MEMC.Fetch32();
                        uint address = computer.MEMC.Fetch24();

                        computer.MEMC.Set24bitToRAM(address, ColorConverter.HSLToRGB(value));

                        return;
                    }
                case Mnem.HSLB2RGB_rrrr_InnnI:    // HSL2RGB rrrr, (nnn)
                    {
                        uint value = computer.CPU.REGS.Get32BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));
                        uint address = computer.MEMC.Fetch24();

                        computer.MEMC.Set24bitToRAM(address, ColorConverter.HSLToRGB(value));

                        return;
                    }
                case Mnem.HSLB2RGB_nnnn_IrrrI:    // HSL2RGB nnnn, (rrr)
                    {
                        uint value = computer.MEMC.Fetch32();
                        uint address = computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.MEMC.Set24bitToRAM(address, ColorConverter.HSLToRGB(value));

                        return;
                    }
                case Mnem.HSLB2RGB_rrrr_IrrrI:    // HSL2RGB rrrr, (rrr)
                    {
                        uint value = computer.CPU.REGS.Get32BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));
                        uint address = computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.MEMC.Set24bitToRAM(address, ColorConverter.HSLToRGB(value));

                        return;
                    }

                case Mnem.HSLB2RGB_InnnI_rrr:   // HSL2RGB (nnn), rrr
                    {
                        uint value = computer.MEMC.Get32bitFromRAM(computer.MEMC.Fetch24());
                        byte destRegIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        computer.CPU.REGS.Set24BitRegister(destRegIndex, ColorConverter.HSLToRGB(value));

                        return;
                    }
                case Mnem.HSLB2RGB_IrrrI_rrr:   // HSL2RGB (rrr), rrr
                    {
                        uint value = computer.MEMC.Get32bitFromRAM(computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111)));
                        byte destRegIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        computer.CPU.REGS.Set24BitRegister(destRegIndex, ColorConverter.HSLToRGB(value));

                        return;
                    }
                case Mnem.HSLB2RGB_InnnI_InnnI:    // HSL2RGB (nnn), (nnn)
                    {
                        uint value = computer.MEMC.Get32bitFromRAM(computer.MEMC.Fetch24());
                        uint address = computer.MEMC.Fetch24();

                        computer.MEMC.Set24bitToRAM(address, ColorConverter.HSLToRGB(value));

                        return;
                    }
                case Mnem.HSLB2RGB_IrrrI_InnnI:    // HSL2RGB (rrr), (nnn)
                    {
                        uint value = computer.MEMC.Get32bitFromRAM(computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111)));
                        uint address = computer.MEMC.Fetch24();

                        computer.MEMC.Set24bitToRAM(address, ColorConverter.HSLToRGB(value));

                        return;
                    }
                case Mnem.HSLB2RGB_InnnI_IrrrI:    // HSL2RGB (nnn), (rrr)
                    {
                        uint value = computer.MEMC.Get32bitFromRAM(computer.MEMC.Fetch24());
                        uint address = computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.MEMC.Set24bitToRAM(address, ColorConverter.HSLToRGB(value));

                        return;
                    }
                case Mnem.HSLB2RGB_IrrrI_IrrrI:    // HSL2RGB (rrr), (rrr)
                    {
                        uint value = computer.MEMC.Get32bitFromRAM(computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111)));
                        uint address = computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        computer.MEMC.Set24bitToRAM(address, ColorConverter.HSLToRGB(value));

                        return;
                    }
            }
        }
    }
}
