using System;

namespace ContinuumTools.Utils
{
    public static class FloatPointUtils
    {
        public static float BytesToFloat(byte[] input)
        {
            // Reverse the byte order to match the original representation
            Array.Reverse(input);

            // Convert the byte array back to a float
            float result = BitConverter.ToSingle(input, 0);

            return result;
        }
    }
}
