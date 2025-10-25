using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public static class ExSETVAR
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();

            switch (ldOp)
            {
                case Mnem.SGVAR_n_n:
                    {
                        uint varIndex = computer.MEMC.Fetch32();
                        uint value = computer.MEMC.Fetch32();

                        computer.MEMC.HMEM[varIndex] = value;

                        break;
                    }
                case Mnem.SGVAR_n_rrrr:
                    {
                        uint varIndex = computer.MEMC.Fetch32();
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint value = computer.CPU.REGS.Get32BitRegister(regIndex);

                        computer.MEMC.HMEM[varIndex] = value;

                        break;
                    }
                case Mnem.SGVAR_rrrr_n:
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint varIndex = computer.CPU.REGS.Get32BitRegister(regIndex);
                        uint value = computer.MEMC.Fetch32();
                        
                        computer.MEMC.HMEM[varIndex] = value;

                        break;
                    }
                case Mnem.SGVAR_rrrr_rrrr:
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint varIndex = computer.CPU.REGS.Get32BitRegister(regIndex);

                        byte regValueIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint value = computer.CPU.REGS.Get32BitRegister(regValueIndex);

                        computer.MEMC.HMEM[varIndex] = value;

                        break;
                    }
            }
        }
    }
}
