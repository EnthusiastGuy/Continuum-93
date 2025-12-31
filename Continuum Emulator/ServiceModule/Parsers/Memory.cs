using Continuum93.Emulator;
using Continuum93.Emulator.RAM;
using System;
using System.Collections.Generic;

namespace Continuum93.ServiceModule.Parsers
{
    public static class Memory
    {
        public static int Address = 0x000000;
        public static int FetchLines = 18;

        public static volatile List<MemLine> Lines = new();

        public static void Update()
        {
            if (Machine.COMPUTER == null)
            {
                Lines.Clear();
                return;
            }

            Lines.Clear();
            var memc = Machine.COMPUTER.MEMC;
            int bytesToFetch = FetchLines * 16;

            for (int i = 0; i < bytesToFetch; i += 16)
            {
                byte[] lineData = new byte[16];
                for (int j = 0; j < 16 && (Address + i + j) < 0x1000000; j++)
                {
                    lineData[j] = memc.Get8bitFromRAM((uint)(Address + i + j));
                }
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



