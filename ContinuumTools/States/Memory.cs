using System;
using System.Collections.Generic;

namespace ContinuumTools.States
{
    public static class Memory
    {
        public static int Address = 0x000000;
        public static int FetchLines = 18;

        public static byte[] Data;

        public static volatile List<MemLine> Lines = new();

        public static void SetResponse(byte[] data)
        {
            Data = data;
            ParseResponse();
        }

        private static void ParseResponse()
        {
            Lines.Clear();

            for (int i = 0; i < Data.Length; i+=16)
            {
                byte[] lineData = new byte[16];
                Array.Copy(Data, i, lineData, 0, 16);
                MemLine line = new(Address + i, lineData);
                Lines.Add(line);
            }
        }
    }

    public class MemLine
    {
        public volatile int Address;
        public volatile string TextAddress;
        public volatile string HexBytes;
        public volatile string ASCIIBytes;
        public volatile string COVERBytes;

        public MemLine(int address, byte[] bytes)
        {
            Address = address;
            TextAddress = address.ToString("X6");
            for (int i = 0; i < bytes.Length; i++)
            {
                HexBytes += $"{bytes[i]:X2} ";
                bool isAscii = bytes[i] >= 32 && bytes[i] <= 127;
                ASCIIBytes += isAscii ? (char)bytes[i] : " ";
                COVERBytes += isAscii ? " " : "X";
            }
        }
    }
}
