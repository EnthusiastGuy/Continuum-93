using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Continuum93.Emulator.Audio.Ogg
{
    internal static class LibOpus
    {
        // Import constants from opus.h
        public const int OPUS_OK = 0;
        public const int OPUS_BAD_ARG = -1;
        public const int OPUS_INVALID_PACKET = -4;

        // Import functions from libopus.dll
        [DllImport("Data/lib/opus", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr opus_decoder_create(int sample_rate, int channels, out int error);

        [DllImport("Data/lib/opus", CallingConvention = CallingConvention.Cdecl)]
        public static extern void opus_decoder_destroy(IntPtr decoder);

        [DllImport("Data/lib/opus", CallingConvention = CallingConvention.Cdecl)]
        public static extern int opus_decode(
            IntPtr decoder,
            byte[] inputBuffer,
            int inputLength,
            short[] outputBuffer,
            int frameSize,
            int decodeFec
        );

        [DllImport("Data/lib/opus", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr opus_encoder_create(int sample_rate, int channels, int application, out int error);

        [DllImport("Data/lib/opus", CallingConvention = CallingConvention.Cdecl)]
        public static extern void opus_encoder_destroy(IntPtr encoder);

        [DllImport("Data/lib/opus", CallingConvention = CallingConvention.Cdecl)]
        public static extern int opus_encode(
            IntPtr encoder,
            short[] inputBuffer,
            int frameSize,
            byte[] outputBuffer,
            int maxDataBytes
        );

        [DllImport("Data/lib/opus", CallingConvention = CallingConvention.Cdecl)]
        public static extern string opus_strerror(int error);
    }
}
