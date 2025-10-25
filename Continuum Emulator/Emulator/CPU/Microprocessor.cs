using Continuum93.Emulator;

namespace Continuum93.Emulator.CPU
{
    public class Microprocessor
    {
        private readonly Registers _regs;
        private readonly FloatRegisters _fregs;
        private readonly Flags _flags;
        private readonly Computer _computer;

        public Microprocessor(Computer computer)
        {
            _computer = computer;
            _regs = new Registers(_computer);
            _fregs = new FloatRegisters(_computer);
            _flags = new Flags();
        }

        public Registers REGS => _regs;

        public FloatRegisters FREGS => _fregs;

        public Flags FLAGS => _flags;
    }
}
