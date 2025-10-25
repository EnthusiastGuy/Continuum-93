namespace Continuum93.Emulator.Interrupts
{
    public static class InterruptsStrings
    {
        public static void FloatToString(byte regId, Computer computer)
        {
            byte fIndex = computer.CPU.REGS.Get8BitRegister(computer.CPU.REGS.GetNextRegister(regId, 1));
            float floatValue = computer.CPU.FREGS.GetRegister(fIndex);

            uint formatAddress = computer.CPU.REGS.Get24BitRegister(computer.CPU.REGS.GetNextRegister(regId, 2));
            string formatString = computer.MEMC.GetStringAt(formatAddress);

            uint targetAddress = computer.CPU.REGS.Get24BitRegister(computer.CPU.REGS.GetNextRegister(regId, 5));

            string result = floatValue.ToString(formatString);

            computer.MEMC.SetStringAt(result, targetAddress);
        }

        public static void UintToString(byte regId, Computer computer)
        {
            uint value = computer.CPU.REGS.Get32BitRegister(computer.CPU.REGS.GetNextRegister(regId, 1));
            
            uint formatAddress = computer.CPU.REGS.Get24BitRegister(computer.CPU.REGS.GetNextRegister(regId, 5));
            string formatString = computer.MEMC.GetStringAt(formatAddress);

            uint targetAddress = computer.CPU.REGS.Get24BitRegister(computer.CPU.REGS.GetNextRegister(regId, 8));

            string result = value.ToString(formatString);

            computer.MEMC.SetStringAt(result, targetAddress);
        }

        public static void IntToString(byte regId, Computer computer)
        {
            int value = computer.CPU.REGS.Get32BitRegisterSigned(computer.CPU.REGS.GetNextRegister(regId, 1));

            uint formatAddress = computer.CPU.REGS.Get24BitRegister(computer.CPU.REGS.GetNextRegister(regId, 5));
            string formatString = computer.MEMC.GetStringAt(formatAddress);

            uint targetAddress = computer.CPU.REGS.Get24BitRegister(computer.CPU.REGS.GetNextRegister(regId, 8));

            string result = value.ToString(formatString);

            computer.MEMC.SetStringAt(result, targetAddress);
        }
    }
}
