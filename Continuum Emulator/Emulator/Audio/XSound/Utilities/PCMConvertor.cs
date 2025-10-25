using System;
using System.Linq;

namespace Continuum93.Emulator.Audio.XSound.Utilities
{
    public static class PCMConvertor
    {
        public static byte[] ConvertToPcm16(float[] samples, float soundVolume)
        {
            byte[] pcm = new byte[samples.Length * 2]; // 16-bit (2 bytes per sample)

            float maxAmplitude = samples.Max(sample => Math.Abs(sample));

            for (int i = 0; i < samples.Length; i++)
            {
                // Normalize the sample to prevent clipping
                float normalizedSample = samples[i] / maxAmplitude;

                // Apply master volume
                normalizedSample *= soundVolume;

                // Convert to 16-bit PCM range
                short value = (short)(normalizedSample * short.MaxValue);

                pcm[i * 2] = (byte)(value & 0xFF);
                pcm[i * 2 + 1] = (byte)(value >> 8 & 0xFF);
            }

            return pcm;
        }
    }
}
