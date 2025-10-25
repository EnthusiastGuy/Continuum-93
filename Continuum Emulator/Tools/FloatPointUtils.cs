using System;
using System.Text;

namespace Continuum93.Tools
{
    public static class FloatPointUtils
    {
        public static byte[] FloatToBytes(float input)
        {
            byte[] floatBytes = BitConverter.GetBytes(input);   // Convert the float value to its binary representation as a byte array
            Array.Reverse(floatBytes); // Reverse the order of bytes
            return floatBytes;
        }

        public static uint FloatToUint(float input)
        {
            byte[] bytes = FloatToBytes(input);
            Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }

        public static float UintToFloat(uint inputValue)
        {
            byte[] bytes = BitConverter.GetBytes(inputValue);
            return BitConverter.ToSingle(bytes, 0);
        }

        public static byte[] FloatStringToBytes(string input)
        {
            if (float.TryParse(input, out float floatValue))
            {
                return FloatToBytes(floatValue);
            }
            else
            {
                throw new ArgumentException("Invalid input. Please enter a valid floating-point number.");
            }
        }

        public static string FloatToBinary(float input)
        {
            return BytesToBinary(FloatToBytes(input));
        }

        public static string BytesToBinary(byte[] input)
        {
            StringBuilder binaryString = new();
            foreach (byte b in input)
                binaryString.Append(Convert.ToString(b, 2).PadLeft(8, '0')); // Convert each byte to binary and ensure it has 8 bits

            return binaryString.ToString();
        }

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
