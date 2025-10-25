using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public class ExFIND
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();
            byte upperLdOp = (byte)(ldOp >> 5);

            switch (upperLdOp)
            {
                case Mnem.FIND_IrrrI_n:   // FIND (rrr), n
                    {
                        byte addressRegIndex = (byte)(ldOp & 0b00011111);
                        uint startAddress = computer.CPU.REGS.Get24BitRegister(addressRegIndex);
                        byte value = computer.MEMC.Fetch();

                        computer.CPU.REGS.Set24BitRegister(addressRegIndex,
                            computer.MEMC.RAM.Find((int)startAddress, value));

                        return;
                    }
                case Mnem.FIND_IrrrI_InnnI:   // FIND (rrr), (nnn)
                    {
                        byte addressRegIndex = (byte)(ldOp & 0b00011111);
                        uint startAddress = computer.CPU.REGS.Get24BitRegister(addressRegIndex);
                        uint stringAddress = computer.MEMC.Fetch24();
                        byte[] stringData = computer.MEMC.GetStringBytesAt(stringAddress);

                        computer.CPU.REGS.Set24BitRegister(addressRegIndex,
                            computer.MEMC.RAM.FindPattern((int)startAddress, stringData));

                        return;
                    }
                case Mnem.FIND_IrrrI_IrrrI:   // FIND (rrr), (rrr)
                    {
                        byte addressRegIndex = (byte)(ldOp & 0b00011111);
                        uint startAddress = computer.CPU.REGS.Get24BitRegister(addressRegIndex);
                        byte stringAddressIndex = computer.MEMC.Fetch();
                        uint stringAddress = computer.CPU.REGS.Get24BitRegister(stringAddressIndex);

                        byte[] stringData = computer.MEMC.GetStringBytesAt(stringAddress);

                        computer.CPU.REGS.Set24BitRegister(addressRegIndex,
                            computer.MEMC.RAM.FindPattern((int)startAddress, stringData));

                        return;
                    }
            }
        }
    }
}
