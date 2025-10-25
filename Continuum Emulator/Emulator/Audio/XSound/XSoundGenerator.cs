using Continuum93.Emulator.Audio.XSound.Generators;
using Continuum93.Emulator.Audio.XSound.Utilities;
using System.Collections.Generic;

namespace Continuum93.Emulator.Audio.XSound
{
    public class XSoundGenerator
    {
        private const int OVERSAMPLING = 8;
        private readonly XSoundParams parameters;
        private readonly XFrequencyController frequencyController;
        private readonly XEnvelopeController envelopeController;
        private readonly XFilterController filterController;
        private readonly XFlangerController flangerController;
        private readonly XRepeatController repeatController;
        private readonly XPhaserController phaserController;

        private readonly float sampleRate;

        public XSoundGenerator(XSoundParams parameters)
        {
            this.parameters = parameters;
            sampleRate = parameters.SampleRate;

            // Initialize controllers
            frequencyController = new XFrequencyController(parameters);
            envelopeController = new XEnvelopeController(parameters);
            filterController = new XFilterController(parameters);
            flangerController = new XFlangerController(parameters);
            repeatController = new XRepeatController(parameters);
            phaserController = new XPhaserController(parameters);
        }

        public byte[] GenerateSound()
        {
            List<float> soundData = new();

            int i = 0;

            // Continue generating samples as long as the envelope has not fully decayed.
            while (true)
            {
                float sample = 0.0f;

                // Check for repeat
                if (repeatController.CheckRepeat())
                {
                    // Reset necessary controllers
                    frequencyController.Reset();
                    phaserController.Reset();
                }

                // Get current envelope value (per sample)
                float envelopeValue = envelopeController.GetEnvelope();

                // Stop generation if envelope is fully decayed
                if (envelopeValue <= 0.0f)
                {
                    break;
                }

                // Get current frequency based on ramping (per sample)
                float currentFrequency = frequencyController.GetFrequency();

                // Get current duty cycle
                float dutyCycle = frequencyController.GetDutyCycle();

                // Generate OVERSAMPLING sub-samples
                for (int si = 0; si < OVERSAMPLING; si++)
                {
                    // Calculate time for each sub-sample
                    float time = (float)(i * OVERSAMPLING + si) / (sampleRate * OVERSAMPLING);

                    // Generate the wave (before envelope and filters)
                    float sub_sample = XWaveTypeGenerator.Generate(time, parameters, currentFrequency, dutyCycle, parameters.BitDepth);

                    // Apply filters to sub_sample
                    sub_sample = filterController.ProcessSample(sub_sample);

                    // Apply flanger effect
                    sub_sample = flangerController.ProcessSample(sub_sample);

                    // Apply phaser effect
                    sub_sample = phaserController.ProcessSample(sub_sample);

                    // Apply envelope to sub_sample
                    sub_sample *= envelopeValue;

                    // Accumulate the sub-samples
                    sample += sub_sample;
                }

                // Average the oversampled values
                sample /= OVERSAMPLING;

                // Add the averaged sample to the sound data list
                soundData.Add(sample);

                i++;
            }

            // Convert float list to byte array (16-bit PCM format)
            byte[] soundBytes = PCMConvertor.ConvertToPcm16(soundData.ToArray(), parameters.SoundVolume);

            return soundBytes;
        }
    }

}
