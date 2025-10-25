using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContinuumTools.Utils
{
    public static class StringUtils
    {
        public static string ByteToHex(byte value, byte places = 2)
        {
            return value.ToString($"X{places}");
        }

        public static string UintToHex(uint value, byte places = 4)
        {
            return value.ToString($"X{places}");
        }

        public static string IntToHex(int value, byte places = 6)
        {
            return value.ToString($"X{places}");
        }
    }
}
