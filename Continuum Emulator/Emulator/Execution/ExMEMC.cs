using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public class ExMEMC
    {
        public static void Process(Computer computer)
        {
            byte op = computer.MEMC.Fetch();
            byte upperOp = (byte)(op >> 5);

            switch (upperOp)
            {
                case Mnem.MEMC_rrr_rrr_rrr:   // MEMC rrr, rrr, rrr
                    {
                        byte reg1Index = (byte)(op & 0b00011111);
                        uint sourceAddress = computer.CPU.REGS.Get24BitRegister(reg1Index);
                        byte reg2Index = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint destAddress = computer.CPU.REGS.Get24BitRegister(reg2Index);
                        byte reg3Index = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint length = computer.CPU.REGS.Get24BitRegister(reg3Index);

                        computer.MEMC.RAM.Copy(sourceAddress, destAddress, length);
                        return;
                    }
                case Mnem.MEMC_nnn_rrr_rrr:   // MEMC nnn, rrr, rrr
                    {
                        byte reg2Index = (byte)(op & 0b00011111);
                        byte reg3Index = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint destAddress = computer.CPU.REGS.Get24BitRegister(reg2Index);
                        uint length = computer.CPU.REGS.Get24BitRegister(reg3Index);
                        uint sourceAddress = computer.MEMC.Fetch24();

                        computer.MEMC.RAM.Copy(sourceAddress, destAddress, length);
                        return;
                    }
                case Mnem.MEMC_rrr_nnn_rrr:   // MEMC rrr, nnn, rrr
                    {
                        byte reg1Index = (byte)(op & 0b00011111);
                        byte reg3Index = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint length = computer.CPU.REGS.Get24BitRegister(reg3Index);
                        uint sourceAddress = computer.CPU.REGS.Get24BitRegister(reg1Index);
                        uint destAddress = computer.MEMC.Fetch24();

                        computer.MEMC.RAM.Copy(sourceAddress, destAddress, length);
                        return;
                    }
                case Mnem.MEMC_nnn_nnn_rrr:   // MEMC nnn, nnn, rrr
                    {
                        byte reg3Index = (byte)(op & 0b00011111);
                        uint length = computer.CPU.REGS.Get24BitRegister(reg3Index);
                        uint sourceAddress = computer.MEMC.Fetch24();
                        uint destAddress = computer.MEMC.Fetch24();

                        computer.MEMC.RAM.Copy(sourceAddress, destAddress, length);
                        return;
                    }
                case Mnem.MEMC_rrr_rrr_nnn:   // MEMC rrr, rrr, nnn
                    {
                        byte reg1Index = (byte)(op & 0b00011111);
                        uint sourceAddress = computer.CPU.REGS.Get24BitRegister(reg1Index);
                        byte reg2Index = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint destAddress = computer.CPU.REGS.Get24BitRegister(reg2Index);
                        uint length = computer.MEMC.Fetch24();

                        computer.MEMC.RAM.Copy(sourceAddress, destAddress, length);
                        return;
                    }
                case Mnem.MEMC_nnn_rrr_nnn:   // MEMC nnn, rrr, nnn
                    {
                        byte reg2Index = (byte)(op & 0b00011111);
                        uint destAddress = computer.CPU.REGS.Get24BitRegister(reg2Index);
                        uint sourceAddress = computer.MEMC.Fetch24();
                        uint length = computer.MEMC.Fetch24();

                        computer.MEMC.RAM.Copy(sourceAddress, destAddress, length);
                        return;
                    }
                case Mnem.MEMC_rrr_nnn_nnn:   // MEMC rrr, nnn, nnn
                    {
                        byte regIndex = (byte)(op & 0b00011111);
                        uint sourceAddress = computer.CPU.REGS.Get24BitRegister(regIndex);
                        uint destAddress = computer.MEMC.Fetch24();
                        uint length = computer.MEMC.Fetch24();

                        computer.MEMC.RAM.Copy(sourceAddress, destAddress, length);
                        return;
                    }
                case Mnem.MEMC_nnn_nnn_nnn:   // MEMC nnn, nnn, nnn
                    {
                        uint sourceAddress = computer.MEMC.Fetch24();
                        uint destAddress = computer.MEMC.Fetch24();
                        uint length = computer.MEMC.Fetch24();

                        computer.MEMC.RAM.Copy(sourceAddress, destAddress, length);
                        return;
                    }
            }
        }
    }
}
