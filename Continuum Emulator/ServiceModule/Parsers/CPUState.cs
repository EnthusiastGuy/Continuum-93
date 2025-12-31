using Continuum93.Emulator;
using Continuum93.Emulator.RAM;
using System;

namespace Continuum93.ServiceModule.Parsers
{
    public static class CPUState
    {
        private static byte[] _regPageOld = new byte[26];
        private static byte[] _regPage0 = new byte[26];
        private static float[] _fRegsOld = new float[16];
        private static float[] _fRegs = new float[16];
        private static byte _regPageIndex = 0;
        private static byte _oldFlags = 0;
        private static byte _flags = 0;
        private static byte[] _regMemoryData = new byte[26 * 16];
        private static int _ipAddress = 0;
        private static int _previousIpAddress = 0;
        private static bool[] _regModified = new bool[26];

        public static byte[] RegPage0 => _regPage0;
        public static byte[] RegPageOld => _regPageOld;
        public static float[] FRegs => _fRegs;
        public static float[] FRegsOld => _fRegsOld;
        public static byte RegPageIndex => _regPageIndex;
        public static byte Flags => _flags;
        public static byte OldFlags => _oldFlags;
        public static byte[] RegMemoryData => _regMemoryData;
        public static int IPAddress => _ipAddress;

        public static void Update()
        {
            if (Machine.COMPUTER == null)
                return;

            var cpu = Machine.COMPUTER.CPU;
            var memc = Machine.COMPUTER.MEMC;

            // Update IP address first to detect new instruction execution
            int newIpAddress = (int)cpu.REGS.IPO;
            bool newInstructionExecuted = (newIpAddress != _ipAddress);
            _ipAddress = newIpAddress;

            // Update registers
            var newRegs = cpu.REGS.GetCurrentRegisterPageData();
            
            // Only clear register change flags when a new instruction has executed
            // This ensures flags persist for the duration of the frame until next instruction
            if (newInstructionExecuted)
            {
                for (int i = 0; i < 26; i++)
                {
                    _regModified[i] = false;
                }
            }

            // Only update old values when a new instruction has executed
            // This preserves the previous instruction's register values for comparison
            if (newInstructionExecuted)
            {
                Array.Copy(_regPage0, _regPageOld, 26);
            }

            // Check which registers changed by comparing new values with current values
            // Only registers that change in THIS update will be marked as modified
            for (int i = 0; i < 26; i++)
            {
                if (newRegs[i] != _regPage0[i])
                {
                    _regModified[i] = true;
                }
            }
            
            _regPage0 = newRegs;
            _regPageIndex = cpu.REGS.GetRegisterBank();

            // Update float registers
            Array.Copy(_fRegs, _fRegsOld, _fRegs.Length);
            for (byte i = 0; i < 16; i++)
            {
                _fRegs[i] = cpu.FREGS.GetRegister(i);
            }

            // Update flags
            _oldFlags = _flags;
            _flags = cpu.FLAGS.GetFlagsByte();

            // Update memory pointed by registers
            _regMemoryData = memc.GetMemoryPointedByAllAddressingRegisters();
        }

        public static bool IsRegisterChanged(int registerIndex)
        {
            return _regModified[registerIndex];
        }

        public static bool GetBitValue(byte inputByte, int bitPosition)
        {
            if (bitPosition < 0 || bitPosition > 7)
            {
                throw new ArgumentOutOfRangeException(nameof(bitPosition), "Bit position must be between 0 and 7.");
            }

            return (inputByte & (1 << bitPosition)) != 0;
        }

        public static uint Get24BitRegisterValue(byte index)
        {
            if (Machine.COMPUTER == null)
                return 0;
            return Machine.COMPUTER.CPU.REGS.Get24BitRegister(index);
        }
    }
}



