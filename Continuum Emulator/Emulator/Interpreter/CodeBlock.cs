namespace Continuum93.Emulator.Interpreter
{
    public class CodeBlock
    {
        public byte[] Data;
        public uint Start;
        public uint End;

        public CodeBlock(uint address)
        {
            Start = address;
        }
    }
}
