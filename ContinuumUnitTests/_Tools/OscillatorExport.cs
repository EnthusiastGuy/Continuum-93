using System.Diagnostics;

namespace ContinuumUnitTests._Tools
{
    public class OscillatorExport
    {
        //[Fact]
        public void TestOscillation()
        {
            for (byte i = 0; i < 255; i++)
            {
                Debug.WriteLine(Oscillate(i));
            }
        }

        private byte Oscillate(byte A)
        {
            if (A < 128)
            {
                A = (byte)(A * 2);
            }
            else
            {
                A = (byte)(A - 128);
                A = (byte)(A * 2);
                A = (byte)(254 - A);
            }
            return A;
        }
    }
}
