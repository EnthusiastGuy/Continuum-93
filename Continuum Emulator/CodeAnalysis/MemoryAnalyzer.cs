using Continuum93.Emulator;
using System;
using System.IO;
using System.IO.Compression;

namespace Continuum93.CodeAnalysis
{
    public static class MemoryAnalyzer
    {
        public static byte[] GetMemoryAt(uint address, int length)
        {
            return Machine.COMPUTER.MEMC.RAM.GetMemoryAt(address, length);
        }

        public static byte[] GetPalettes()
        {
            byte[] response = new byte[768 * Machine.COMPUTER.GRAPHICS.VRAM_PAGES];
            int bytesPerPalette = 768; // Number of bytes per palette

            for (byte i = 0; i < Machine.COMPUTER.GRAPHICS.VRAM_PAGES; i++)
            {
                int startIndex = i * bytesPerPalette;

                // Use GetMemoryAt to get the palette data and copy it to the response array
                byte[] paletteData = GetMemoryAt(Machine.COMPUTER.GRAPHICS.PALETTE_ADDRESSES[i], bytesPerPalette);
                Array.Copy(paletteData, 0, response, startIndex, bytesPerPalette);
            }

            return response;
        }

        public static byte[] GetVideoLayersData()
        {
            int bytesPerPage = 480 * 270;
            byte[] response = new byte[bytesPerPage * Machine.COMPUTER.GRAPHICS.VRAM_PAGES];

            for (byte i = 0; i < Machine.COMPUTER.GRAPHICS.VRAM_PAGES; i++)
            {
                int startIndex = i * bytesPerPage;

                uint pageAddress = Machine.COMPUTER.GRAPHICS.GetVideoPageAddress(i);

                // Use GetMemoryAt to get the palette data and copy it to the response array
                byte[] pageData = GetMemoryAt(pageAddress, bytesPerPage);
                Array.Copy(pageData, 0, response, startIndex, bytesPerPage);
            }

            return response;
        }

        public static byte[] GetVideoData()
        {
            byte[] palettes = GetPalettes();
            byte[] videoLayers = GetVideoLayersData();

            byte[] response = new byte[palettes.Length + videoLayers.Length];
            Array.Copy(palettes, 0, response, 0, palettes.Length);
            Array.Copy(videoLayers, 0, response, palettes.Length, videoLayers.Length);

            byte[] compressedResponse = Compress(response);

            return compressedResponse;
        }

        public static byte[] Compress(byte[] data)
        {
            using MemoryStream compressedStream = new();
            using (GZipStream compressionStream = new(compressedStream, CompressionMode.Compress))
            {
                compressionStream.Write(data, 0, data.Length);
            }
            return compressedStream.ToArray();
        }
    }
}
