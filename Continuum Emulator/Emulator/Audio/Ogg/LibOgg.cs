using System.Runtime.InteropServices;
using System;

namespace Continuum93.Emulator.Audio.Ogg
{
    internal static class LibOgg
    {
        // Structs, enums, and function prototypes for libogg

        /// <summary>
        /// Represents Ogg bitstream synchronization and buffering.
        /// (Equivalent to 'ogg_sync_state' in libogg.)
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct OggSyncState
        {
            public IntPtr data;       // pointer to internal buffer
            public int storage;       // current buffer capacity
            public int fill;          // bytes in buffer
            public int returned;      // bytes of data returned so far

            public int unsynced;      // set when we’ve encountered sync errors
            public int headerbytes;   // bytes of header processed
            public int bodybytes;     // bytes of data processed
        }

        /// <summary>
        /// Represents a single Ogg bitstream as decoded by libogg.
        /// (Equivalent to 'ogg_stream_state' in libogg.)
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct OggStreamState
        {
            public IntPtr body_data;
            public int body_storage;
            public int body_fill;
            public int body_returned;
            public IntPtr lacing_vals;
            public IntPtr granule_vals;
            public int lacing_storage;
            public int lacing_fill;
            public int lacing_packet;
            public int lacing_returned;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 282)]
            public byte[] header;
            public int header_fill;
            public int e_o_s;
            public int b_o_s;
            public long serialno;
            public long pageno;
            public long packetno;
            public long granulepos;
        }

        /// <summary>
        /// Represents a single Ogg page in a bitstream.
        /// (Equivalent to 'ogg_page' in libogg.)
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct OggPage
        {
            public IntPtr header;     // pointer to the page header bytes
            public int header_len;    // length of the header
            public IntPtr body;       // pointer to the page body bytes
            public int body_len;      // length of the body
        }

        /// <summary>
        /// Represents a single Ogg packet extracted from an Ogg bitstream.
        /// (Equivalent to 'ogg_packet' in libogg.)
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct OggPacket
        {
            public IntPtr packet;     // pointer to packet data
            public int bytes;         // length of packet data
            public int b_o_s;         // set if first packet in a logical bitstream
            public int e_o_s;         // set if last packet in a logical bitstream
            public long granulepos;   // granule position
            public long packetno;     // packet sequence number
        }


        // Import functions from libogg.dll
        /// <summary>
        /// Initializes an OggSyncState. Must be called before usage.
        /// </summary>
        [DllImport("Data/lib/libogg", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ogg_sync_init(ref OggSyncState oy);

        /// <summary>
        /// Clears/frees an OggSyncState.
        /// </summary>
        [DllImport("Data/lib/libogg", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ogg_sync_clear(ref OggSyncState oy);

        /// <summary>
        /// Provides a buffer into which new data can be read. You then call <see cref="ogg_sync_wrote"/> afterward.
        /// </summary>
        /// <returns>Pointer to a buffer of at least 'size' bytes</returns>
        [DllImport("Data/lib/libogg", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ogg_sync_buffer(ref OggSyncState oy, long size);

        /// <summary>
        /// Reports how many bytes were actually read into the buffer obtained from <see cref="ogg_sync_buffer"/>.
        /// </summary>
        /// <param name="oy">The sync state</param>
        /// <param name="bytes">Number of bytes written</param>
        [DllImport("Data/lib/libogg", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ogg_sync_wrote(ref OggSyncState oy, long bytes);

        /// <summary>
        /// Checks if a complete page is available. If so, it returns the page in og,
        /// and removes it from the internal buffer. If not, returns 0 or negative on error.
        /// </summary>
        [DllImport("Data/lib/libogg", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ogg_sync_pageout(ref OggSyncState oy, ref OggPage og);

        /// <summary>
        /// Scans for a page boundary and returns the number of bytes skipped. If it finds a page,
        /// it can return bytes consumed. Typically used for recovering from partial pages.
        /// </summary>
        [DllImport("Data/lib/libogg", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ogg_sync_pageseek(ref OggSyncState oy, ref OggPage og);

        //------------------------------------------------------------------------------------
        // 3. ogg_stream_* functions (logical bitstream management)
        //------------------------------------------------------------------------------------

        /// <summary>
        /// Initialize a stream state with a given serial number.
        /// </summary>
        [DllImport("Data/lib/libogg", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ogg_stream_init(out OggStreamState os, int serialno);

        /// <summary>
        /// Clear/frees a stream state.
        /// </summary>
        [DllImport("Data/lib/libogg", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ogg_stream_clear(ref OggStreamState os);

        /// <summary>
        /// Submits a page to a stream for packet extraction.
        /// </summary>
        [DllImport("Data/lib/libogg", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ogg_stream_pagein(ref OggStreamState os, ref OggPage og);

        /// <summary>
        /// Extracts a packet from the next available packet segment in the Ogg bitstream.
        /// </summary>
        [DllImport("Data/lib/libogg", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ogg_stream_packetout(ref OggStreamState os, ref OggPacket op);

        /// <summary>
        /// Similar to <see cref="ogg_stream_packetout"/>, but does not advance the internal pointer.
        /// </summary>
        [DllImport("Data/lib/libogg", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ogg_stream_packetpeek(ref OggStreamState os, ref OggPacket op);

        /// <summary>
        /// Adds a packet to the bitstream (used during encoding).
        /// </summary>
        [DllImport("Data/lib/libogg", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ogg_stream_packetin(ref OggStreamState os, ref OggPacket op);

        /// <summary>
        /// Produces an Ogg page from buffered packets if available (used during encoding).
        /// </summary>
        [DllImport("Data/lib/libogg", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ogg_stream_pageout(ref OggStreamState os, ref OggPage og);

        /// <summary>
        /// Forces remaining packets into a page (used during encoding).
        /// </summary>
        [DllImport("Data/lib/libogg", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ogg_stream_flush(ref OggStreamState os, ref OggPage og);

        //------------------------------------------------------------------------------------
        // 4. OggPage Macros as C# Helper Methods (since they are not exported functions)
        //------------------------------------------------------------------------------------
        // In C, these are macros in <ogg/ogg.h>. We replicate them by reading fields from OggPage.

        /// <summary>
        /// Returns the stream structure version of this page (usually 0).
        /// </summary>
        public static byte ogg_page_version(ref OggPage og)
        {
            // Page version is stored at header[4]
            return Marshal.ReadByte(og.header, 4);
        }

        /// <summary>
        /// True if the page is continued from the previous packet (header[5] & 0x01).
        /// </summary>
        public static bool ogg_page_continued(ref OggPage og)
        {
            byte flags = Marshal.ReadByte(og.header, 5);
            return (flags & 0x01) != 0;
        }

        /// <summary>
        /// True if the page is the beginning of a stream (header[5] & 0x02).
        /// </summary>
        public static bool ogg_page_bos(ref OggPage og)
        {
            byte flags = Marshal.ReadByte(og.header, 5);
            return (flags & 0x02) != 0;
        }

        /// <summary>
        /// True if the page is the end of a stream (header[5] & 0x04).
        /// </summary>
        public static bool ogg_page_eos(ref OggPage og)
        {
            byte flags = Marshal.ReadByte(og.header, 5);
            return (flags & 0x04) != 0;
        }

        /// <summary>
        /// Returns the granule position (sample index or frame count) of this page.
        /// (8 bytes at header offset 6, little-endian).
        /// </summary>
        public static long ogg_page_granulepos(ref OggPage og)
        {
            ulong lo = 0;
            for (int i = 0; i < 8; i++)
            {
                lo |= (ulong)Marshal.ReadByte(og.header, 6 + i) << (8 * i);
            }
            return (long)lo; // cast to signed
        }

        /// <summary>
        /// Returns the bitstream serial number for this page.
        /// (4 bytes at header offset 14, little-endian).
        /// </summary>
        public static int ogg_page_serialno(ref OggPage og)
        {
            uint serial = 0;
            for (int i = 0; i < 4; i++)
            {
                serial |= (uint)Marshal.ReadByte(og.header, 14 + i) << (8 * i);
            }
            return (int)serial;
        }

        /// <summary>
        /// Returns the page sequence number.
        /// (4 bytes at header offset 18, little-endian).
        /// </summary>
        public static int ogg_page_pageno(ref OggPage og)
        {
            uint pageno = 0;
            for (int i = 0; i < 4; i++)
            {
                pageno |= (uint)Marshal.ReadByte(og.header, 18 + i) << (8 * i);
            }
            return (int)pageno;
        }
    }

    public class AudioInteropWrapper
    {
        public IntPtr CreateOpusDecoder(int sampleRate, int channels)
        {
            IntPtr decoder = LibOpus.opus_decoder_create(sampleRate, channels, out int error);

            if (decoder == IntPtr.Zero || error != LibOpus.OPUS_OK)
            {
                throw new Exception($"Failed to create Opus decoder: {LibOpus.opus_strerror(error)}");
            }

            return decoder;
        }

        public void DestroyOpusDecoder(IntPtr decoder)
        {
            LibOpus.opus_decoder_destroy(decoder);
        }

        public short[] DecodeOpus(IntPtr decoder, byte[] inputData, int inputLength, int frameSize)
        {
            short[] outputBuffer = new short[frameSize * 2]; // Assuming stereo output
            int result = LibOpus.opus_decode(decoder, inputData, inputLength, outputBuffer, frameSize, 0);

            if (result < 0)
            {
                throw new Exception($"Opus decoding failed: {LibOpus.opus_strerror(result)}");
            }

            return outputBuffer;
        }

        public IntPtr CreateOpusEncoder(int sampleRate, int channels, int application)
        {
            IntPtr encoder = LibOpus.opus_encoder_create(sampleRate, channels, application, out int error);

            if (encoder == IntPtr.Zero || error != LibOpus.OPUS_OK)
            {
                throw new Exception($"Failed to create Opus encoder: {LibOpus.opus_strerror(error)}");
            }

            return encoder;
        }

        public void DestroyOpusEncoder(IntPtr encoder)
        {
            LibOpus.opus_encoder_destroy(encoder);
        }

        public byte[] EncodeOpus(IntPtr encoder, short[] inputData, int frameSize, int maxDataBytes)
        {
            byte[] outputBuffer = new byte[maxDataBytes];
            int result = LibOpus.opus_encode(encoder, inputData, frameSize, outputBuffer, maxDataBytes);

            if (result < 0)
            {
                throw new Exception($"Opus encoding failed: {LibOpus.opus_strerror(result)}");
            }

            Array.Resize(ref outputBuffer, result); // Resize to actual encoded length
            return outputBuffer;
        }
    }
}
