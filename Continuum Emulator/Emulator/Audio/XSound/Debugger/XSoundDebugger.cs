using System;

namespace Continuum93.Emulator.Audio.XSound.Debug
{
    public class XSoundDebugger
    {
        private static XSoundGenerator _soundGenerator;
        private static byte[] _soundData;

        public static void SavePlotData(XSoundParams parameters)
        {
            _soundGenerator = new XSoundGenerator(parameters);
            _soundData = _soundGenerator.GenerateSound();

            int dataLength = _soundData.Length / 2; // 16-bit PCM, so two bytes per sample
            float[] soundData = new float[dataLength];

            // Plot each sound sample into the texture
            for (int i = 0; i < dataLength; i++)
            {
                // Extract the 16-bit PCM value from _soundData (two bytes per sample)
                short sampleValue = BitConverter.ToInt16(_soundData, i * 2);

                // Normalize the sample to fit within the texture height
                float normalizedSample = sampleValue / 65536f;

                // generate data value pair, store it
                soundData[i] = normalizedSample;
            }

            XSoundHtmlGenerator.GenerateHtmlWithSoundData(soundData, "waveform_debug.html");
        }
    }
}
