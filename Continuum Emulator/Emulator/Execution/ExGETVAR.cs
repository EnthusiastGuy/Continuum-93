using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public static class ExGETVAR
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();

            switch (ldOp)
            {
                case Mnem.SGVAR_n_rrrr:
                    {
                        uint varIndex = computer.MEMC.Fetch32();
                        uint value = computer.MEMC.HMEM[varIndex];
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);

                        computer.CPU.REGS.Set32BitRegister(regIndex, value);

                        break;
                    }
                case Mnem.SGVAR_rrrr_rrrr:
                    {
                        byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        uint varIndex = computer.CPU.REGS.Get32BitRegister(regIndex);

                        uint value = computer.MEMC.HMEM[varIndex];

                        byte regValueIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
                        computer.CPU.REGS.Set32BitRegister(regValueIndex, value);

                        break;
                    }
            }
        }
    }
}
