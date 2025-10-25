using Continuum93.Emulator;
using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public class ExMEMF
    {
        public static void Process(Computer computer)
        {
            byte op = computer.MEMC.Fetch();
            byte upperOp = (byte)(op >> 5);

            switch (upperOp)
            {
                case Mnem.MEMF_rrr_rrr_r:   // MEMF rrr, rrr, r
                    {
                        byte reg1Index = (byte)(op & 0b00011111);
                        uint address = computer.CPU.REGS.Get24BitRegister(reg1Index);
                        byte reg2Index = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint length = computer.CPU.REGS.Get24BitRegister(reg2Index);
                        byte reg3Index = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte value = computer.CPU.REGS.Get8BitRegister(reg3Index);

                        computer.MEMC.RAM.Fill(value, address, length);
                        return;
                    }
                case Mnem.MEMF_nnn_rrr_r:   // MEMF nnn, rrr, r
                    {
                        byte reg2Index = (byte)(op & 0b00011111);
                        byte reg3Index = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte value = computer.CPU.REGS.Get8BitRegister(reg3Index);
                        uint length = computer.CPU.REGS.Get24BitRegister(reg2Index);
                        uint address = computer.MEMC.Fetch24();

                        computer.MEMC.RAM.Fill(value, address, length);
                        return;
                    }
                case Mnem.MEMF_rrr_nnn_r:   // MEMF rrr, nnn, r
                    {
                        byte reg2Index = (byte)(op & 0b00011111);
                        byte reg3Index = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        byte value = computer.CPU.REGS.Get8BitRegister(reg3Index);
                        uint address = computer.CPU.REGS.Get24BitRegister(reg2Index);
                        uint length = computer.MEMC.Fetch24();

                        computer.MEMC.RAM.Fill(value, address, length);
                        return;
                    }
                case Mnem.MEMF_nnn_nnn_r:   // MEMF nnn, nnn, r
                    {
                        byte reg3Index = (byte)(op & 0b00011111);
                        byte value = computer.CPU.REGS.Get8BitRegister(reg3Index);
                        uint address = computer.MEMC.Fetch24();
                        uint length = computer.MEMC.Fetch24();

                        computer.MEMC.RAM.Fill(value, address, length);
                        return;
                    }
                case Mnem.MEMF_rrr_rrr_n:   // MEMF rrr, rrr, n
                    {
                        byte reg1Index = (byte)(op & 0b00011111);
                        uint address = computer.CPU.REGS.Get24BitRegister(reg1Index);
                        byte reg2Index = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint length = computer.CPU.REGS.Get24BitRegister(reg2Index);
                        byte value = computer.MEMC.Fetch();

                        computer.MEMC.RAM.Fill(value, address, length);
                        return;
                    }
                case Mnem.MEMF_nnn_rrr_n:   // MEMF nnn, rrr, n
                    {
                        byte reg2Index = (byte)(op & 0b00011111);
                        uint length = computer.CPU.REGS.Get24BitRegister(reg2Index);
                        uint address = computer.MEMC.Fetch24();
                        byte value = computer.MEMC.Fetch();

                        computer.MEMC.RAM.Fill(value, address, length);
                        return;
                    }
                case Mnem.MEMF_rrr_nnn_n:   // MEMF rrr, nnn, n
                    {
                        byte regIndex = (byte)(op & 0b00011111);
                        uint address = computer.CPU.REGS.Get24BitRegister(regIndex);
                        uint length = computer.MEMC.Fetch24();
                        byte value = computer.MEMC.Fetch();

                        computer.MEMC.RAM.Fill(value, address, length);
                        return;
                    }
                case Mnem.MEMF_nnn_nnn_n:   // MEMF nnn, nnn, n
                    {
                        uint address = computer.MEMC.Fetch24();
                        uint length = computer.MEMC.Fetch24();
                        byte value = computer.MEMC.Fetch();

                        computer.MEMC.RAM.Fill(value, address, length);
                        return;
                    }
            }
        }
    }
}
