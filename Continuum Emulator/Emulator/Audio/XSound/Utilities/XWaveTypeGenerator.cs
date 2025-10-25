using System;

namespace Continuum93.Emulator.Audio.XSound.Generators
{
    public static class XWaveTypeGenerator
    {
        static readonly Random random = new();

        /// <summary>
        /// Generates a sound sample based on the waveform type and parameters.
        /// </summary>
        /// <param name="time">The current time point for the waveform.</param>
        /// <param name="parameters">Parameters defining the sound characteristics.</param>
        /// <param name="frequency">The frequency of the waveform.</param>
        /// <param name="dutyCycle">The duty cycle for square and pulse waves.</param>
        /// <returns>A float representing the generated waveform sample.</returns>
        public static float Generate(float time, XSoundParams parameters, float frequency, float dutyCycle, int bitDepth)
        {
            float waveSample = 0.0f;

            // Check if we're in complex mode
            if (parameters.WaveType == XSoundParams.WaveTypes.COMPLEX)
            {
                waveSample = GenerateComplexWave(time, parameters, frequency, dutyCycle, bitDepth);
            }
            else
            {
                // Select waveform and generate sample value based on it
                switch (parameters.WaveType)
                {
                    case XSoundParams.WaveTypes.SINE:
                        waveSample = GenerateSineWave(time, frequency);
                        break;

                    case XSoundParams.WaveTypes.SQUARE:
                        waveSample = GenerateSquareWave(time, frequency);
                        break;

                    case XSoundParams.WaveTypes.SAWTOOTH:
                        waveSample = GenerateSawtoothWave(time, frequency);
                        break;

                    case XSoundParams.WaveTypes.TRIANGLE:
                        waveSample = GenerateTriangleWave(time, frequency);
                        break;

                    case XSoundParams.WaveTypes.RAMPWAVE:
                        waveSample = GenerateRampWave(time, frequency, dutyCycle);
                        break;

                    case XSoundParams.WaveTypes.NOISE:
                        waveSample = GenerateNoiseWave();
                        break;

                    case XSoundParams.WaveTypes.PINKNOISE:
                        waveSample = GeneratePinkNoise();
                        break;

                    case XSoundParams.WaveTypes.BREAKER:
                        waveSample = GenerateBreakerWave(time, frequency);
                        break;

                    case XSoundParams.WaveTypes.TAN:
                        waveSample = GenerateTanWave(time, frequency);
                        break;

                    case XSoundParams.WaveTypes.WHISTLE:
                        waveSample = GenerateWhistleWave(time, frequency);
                        break;

                    case XSoundParams.WaveTypes.ORGAN:
                        waveSample = GenerateOrganWave(time, frequency);
                        break;

                    case XSoundParams.WaveTypes.PULSEWAVE:
                        waveSample = GeneratePulseWave(time, frequency, dutyCycle);
                        break;

                    case XSoundParams.WaveTypes.HALFSINE:
                        waveSample = GenerateHalfSineWave(time, frequency);
                        break;

                    case XSoundParams.WaveTypes.REVERSESAWTOOTH:
                        waveSample = GenerateReverseSawtoothWave(time, frequency);
                        break;

                    case XSoundParams.WaveTypes.CLIPPEDSINE:
                        waveSample = GenerateClippedSineWave(time, frequency, dutyCycle);
                        break;

                    case XSoundParams.WaveTypes.EXPONENTIAL:
                        waveSample = GenerateExponentialWave(time, frequency);
                        break;

                    case XSoundParams.WaveTypes.FORMANT:
                        waveSample = GenerateFormantWave(time, frequency);
                        break;

                    case XSoundParams.WaveTypes.GAUSSIANNOISE:
                        waveSample = GenerateGaussianNoise();
                        break;

                    case XSoundParams.WaveTypes.WAVEFOLDING:
                        waveSample = GenerateWavefoldingWave(time, frequency, dutyCycle);
                        break;

                    case XSoundParams.WaveTypes.RINGMODULATION:
                        waveSample = GenerateRingModulation(time, frequency, dutyCycle);
                        break;

                    case XSoundParams.WaveTypes.LFO:
                        waveSample = GenerateLFOmodulatedNoise(time, frequency, dutyCycle);
                        break;

                    case XSoundParams.WaveTypes.WARPEDSINE:
                        waveSample = GenerateWarpedSineWave(time, frequency);
                        break;

                    case XSoundParams.WaveTypes.FRACTALSAWTOOTH:
                        waveSample = GenerateFractalSawtoothWave(time, frequency);
                        break;

                    case XSoundParams.WaveTypes.DETUNEDSINE:
                        waveSample = GenerateDetunedSineWave(time, frequency);
                        break;

                    case XSoundParams.WaveTypes.PHASEDISTORDETSINE:
                        waveSample = GeneratePhaseDistortedSine(time, frequency);
                        break;

                }
            }

            return BitCrush(waveSample, bitDepth);
        }

        /// <summary>
        /// Generates a complex wave by combining multiple waveforms with their respective weights.
        /// </summary>
        /// <param name="time">The current time point for the waveform.</param>
        /// <param name="parameters">Parameters that define wave types and their contributions.</param>
        /// <param name="dutyCycle">The duty cycle for square or pulse wave components.</param>
        /// <returns>A float representing the generated complex waveform sample.</returns>
        private static float GenerateComplexWave(float time, XSoundParams parameters, float frequency, float dutyCycle, int bitDepth)
        {
            float complexWaveValue = 0.0f;

            // Sum all waveforms' contributions based on the percentages in WaveTypeValues
            foreach (var waveTypeValue in parameters.WaveTypeValues)
            {
                float waveSample = 0.0f;
                float weight = waveTypeValue.Value / 100.0f;  // Convert percentage to a fraction

                // Generate the appropriate waveform sample based on the wave type
                switch (waveTypeValue.Key)
                {
                    case XSoundParams.WaveTypes.SINE:
                        waveSample = GenerateSineWave(time, frequency);
                        break;

                    case XSoundParams.WaveTypes.SQUARE:
                        waveSample = GenerateSquareWave(time, frequency);
                        break;

                    case XSoundParams.WaveTypes.SAWTOOTH:
                        waveSample = GenerateSawtoothWave(time, frequency);
                        break;

                    case XSoundParams.WaveTypes.TRIANGLE:
                        waveSample = GenerateTriangleWave(time, frequency);
                        break;

                    case XSoundParams.WaveTypes.RAMPWAVE:
                        waveSample = GenerateRampWave(time, frequency, dutyCycle);
                        break;

                    case XSoundParams.WaveTypes.NOISE:
                        waveSample = GenerateNoiseWave();
                        break;

                    case XSoundParams.WaveTypes.PINKNOISE:
                        waveSample = GeneratePinkNoise();
                        break;

                    case XSoundParams.WaveTypes.BREAKER:
                        waveSample = GenerateBreakerWave(time, frequency);
                        break;

                    case XSoundParams.WaveTypes.TAN:
                        waveSample = GenerateTanWave(time, frequency);
                        break;

                    case XSoundParams.WaveTypes.WHISTLE:
                        waveSample = GenerateWhistleWave(time, frequency);
                        break;

                    case XSoundParams.WaveTypes.ORGAN:
                        waveSample = GenerateOrganWave(time, frequency);
                        break;

                    case XSoundParams.WaveTypes.PULSEWAVE:
                        waveSample = GeneratePulseWave(time, frequency, dutyCycle);
                        break;

                    case XSoundParams.WaveTypes.HALFSINE:
                        waveSample = GenerateHalfSineWave(time, frequency);
                        break;

                    case XSoundParams.WaveTypes.REVERSESAWTOOTH:
                        waveSample = GenerateReverseSawtoothWave(time, frequency);
                        break;

                    case XSoundParams.WaveTypes.CLIPPEDSINE:
                        waveSample = GenerateClippedSineWave(time, frequency, dutyCycle);
                        break;

                    case XSoundParams.WaveTypes.EXPONENTIAL:
                        waveSample = GenerateExponentialWave(time, frequency);
                        break;

                    case XSoundParams.WaveTypes.FORMANT:
                        waveSample = GenerateFormantWave(time, frequency);
                        break;

                    case XSoundParams.WaveTypes.GAUSSIANNOISE:
                        waveSample = GenerateGaussianNoise();
                        break;

                    case XSoundParams.WaveTypes.WAVEFOLDING:
                        waveSample = GenerateWavefoldingWave(time, frequency, dutyCycle);
                        break;

                    case XSoundParams.WaveTypes.RINGMODULATION:
                        waveSample = GenerateRingModulation(time, frequency, dutyCycle);
                        break;

                    case XSoundParams.WaveTypes.LFO:
                        waveSample = GenerateLFOmodulatedNoise(time, frequency, dutyCycle);
                        break;

                    case XSoundParams.WaveTypes.WARPEDSINE:
                        waveSample = GenerateWarpedSineWave(time, frequency);
                        break;

                    case XSoundParams.WaveTypes.FRACTALSAWTOOTH:
                        waveSample = GenerateFractalSawtoothWave(time, frequency);
                        break;

                    case XSoundParams.WaveTypes.DETUNEDSINE:
                        waveSample = GenerateDetunedSineWave(time, frequency);
                        break;

                    case XSoundParams.WaveTypes.PHASEDISTORDETSINE:
                        waveSample = GeneratePhaseDistortedSine(time, frequency);
                        break;

                }

                // Add the weighted contribution to the complex wave
                complexWaveValue += waveSample * weight;
            }

            // Ensure the final value remains within the range [-1.0, +1.0]
            return BitCrush(complexWaveValue, bitDepth);
        }

        private static float BitCrush(float sample, int bitDepth)
        {
            sample = Math.Max(-1.0f, Math.Min(1.0f, sample));  // Clamp to [-1.0, +1.0]
            int levelCount = (int)Math.Pow(2, bitDepth);       // Number of quantization levels
            return MathF.Round(sample * (levelCount / 2)) / (levelCount / 2);
        }

        private static float GenerateSineWave(float time, float frequency)
        {
            return (float)Math.Sin(2.0 * Math.PI * frequency * time);
        }

        private static float GenerateSquareWave(float time, float frequency)
        {
            float period = 1.0f / frequency;
            float t = time % period;

            // Return 1.0f for the first half of the period, -1.0f for the second half
            return t < (period / 2.0f) ? 1.0f : -1.0f;
        }

        private static float GenerateSawtoothWave(float time, float frequency)
        {
            float period = 1.0f / frequency;
            float t = time % period;

            // Standard sawtooth wave (rises from -1 to +1 over the entire period)
            return 2.0f * (t / period) - 1.0f;
        }


        private static float GenerateNoiseWave()
        {
            return (float)(random.NextDouble() * 2.0 - 1.0); // Random values between -1 and 1
        }

        private static float GenerateTriangleWave(float time, float frequency)
        {
            float period = 1.0f / frequency;
            float t = time % period;

            // Generate an inverted triangle wave that starts at -1
            return -(4.0f * Math.Abs(t / period - 0.5f) - 1.0f);
        }

        private static float GenerateRampWave(float time, float frequency, float dutyCycle)
        {
            float period = 1.0f / frequency;
            float t = time % period;

            if (t < dutyCycle * period)
            {
                // Rising slope
                return (2.0f * t) / (dutyCycle * period) - 1.0f;
            }
            else
            {
                // Falling slope
                return 1.0f - (2.0f * (t - dutyCycle * period)) / ((1.0f - dutyCycle) * period);
            }
        }


        private static float GeneratePinkNoise()
        {
            // Basic pink noise generator (there are several ways to do this)
            float white = (float)(random.NextDouble() * 2.0 - 1.0);  // White noise
                                                                     // Apply a filter to get pink noise characteristics (this is just a simple example)
            return white / (float)(1.0 + Math.Pow(white, 2));  // Attenuates higher frequencies
        }

        // Breaker Wave
        private static float GenerateBreakerWave(float time, float frequency)
        {
            return (float)(Math.Abs(1.0 - time * frequency % 2.0) * (1.0 - time * frequency % 1.0));
        }

        // Tan Wave
        private static float GenerateTanWave(float time, float frequency)
        {
            float value = (float)Math.Tan(2.0 * Math.PI * frequency * time);
            return Math.Clamp(value, -1.0f, 1.0f);
        }

        // Whistle Wave
        private static float GenerateWhistleWave(float time, float frequency)
        {
            float modulationFrequency = 5.0f;
            float modulationDepth = 0.05f;
            float modulatedFrequency = frequency * (1.0f + modulationDepth * (float)Math.Sin(2.0 * Math.PI * modulationFrequency * time));

            return (float)Math.Sin(2.0 * Math.PI * modulatedFrequency * time);
        }

        // Organ Wave (Harmonic Wave)
        private static float GenerateOrganWave(float time, float frequency)
        {
            return (float)(Math.Sin(2.0 * Math.PI * frequency * time) +
                           0.5 * Math.Sin(2.0 * Math.PI * 2 * frequency * time) +
                           0.25 * Math.Sin(2.0 * Math.PI * 4 * frequency * time));
        }

        // Pulse Wave (PWM - Pulse Width Modulation)
        private static float GeneratePulseWave(float time, float frequency, float dutyCycle)
        {
            float period = 1.0f / frequency;
            float t = time % period;
            return t < dutyCycle * period ? 1.0f : -1.0f;
        }

        // Half-Sine Wave
        private static float GenerateHalfSineWave(float time, float frequency)
        {
            float sineWave = (float)Math.Sin(2.0 * Math.PI * frequency * time);
            return Math.Max(0.0f, sineWave);  // Cut off the negative part
        }

        // Reverse Sawtooth Wave
        private static float GenerateReverseSawtoothWave(float time, float frequency)
        {
            float period = 1.0f / frequency;
            float t = time % period;

            // Reverse sawtooth wave (falls from -1 to +1 over the entire period)
            return 2.0f * (1.0f - t / period) - 1.0f;
        }

        // Clipped Sine Wave
        private static float GenerateClippedSineWave(float time, float frequency, float dutyCycle)
        {
            float sineWave = (float)Math.Sin(2.0 * Math.PI * frequency * time);

            // Invert the effect so dutyCycle 0 gives full threshold (no clipping) and 1 gives minimal threshold (full clipping)
            float clipThreshold = 1.0f - dutyCycle;

            // Clamp the sine wave based on this threshold
            return Math.Clamp(sineWave, -clipThreshold, clipThreshold);
        }

        // Exponential Wave
        private static float GenerateExponentialWave(float time, float frequency)
        {
            float sineWave = (float)Math.Sin(2.0 * Math.PI * frequency * time);
            return (float)Math.Pow(2.0, sineWave) - 1.0f;  // Exponential growth of the sine wave
        }

        // Formant Wave (Vocal-Like Sound)
        private static float GenerateFormantWave(float time, float baseFrequency)
        {
            // Formant frequencies for a vowel-like sound (e.g., "ah" sound)
            float f1 = 730.0f;  // First formant (Hz)
            float f2 = 1090.0f; // Second formant (Hz)
            float f3 = 2440.0f; // Third formant (Hz)

            // Combining sine waves at different formant frequencies
            return (float)(0.5 * Math.Sin(2.0 * Math.PI * baseFrequency * time) +
                           0.3 * Math.Sin(2.0 * Math.PI * f1 * time) +
                           0.2 * Math.Sin(2.0 * Math.PI * f2 * time) +
                           0.1 * Math.Sin(2.0 * Math.PI * f3 * time));
        }

        // Gaussian Noise
        private static float GenerateGaussianNoise()
        {
            // Box-Muller transform to generate Gaussian-distributed noise
            double u1 = random.NextDouble();
            double u2 = random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return (float)(randStdNormal); // Gaussian-distributed noise value
        }

        // Wavefolding
        private static float GenerateWavefoldingWave(float time, float frequency, float foldThreshold)
        {
            float sineWave = (float)Math.Sin(2.0 * Math.PI * frequency * time);
            if (Math.Abs(sineWave) > foldThreshold)
            {
                sineWave = (float)(2.0 * foldThreshold - Math.Abs(sineWave));
            }
            return sineWave;
        }

        // Ring Modulation (Cosmic Interference)
        private static float GenerateRingModulation(float time, float carrierFrequency, float modulatorFrequency)
        {
            float carrier = (float)Math.Sin(2.0 * Math.PI * carrierFrequency * time);
            float modulator = (float)Math.Sin(2.0 * Math.PI * modulatorFrequency * time);
            return carrier * modulator;  // Multiplying carrier and modulator
        }

        // Low-Frequency Oscillator (LFO) Modulated Noise (Radio Static)
        private static float GenerateLFOmodulatedNoise(float time, float noiseFrequency, float lfoFrequency)
        {
            float noise = (float)(random.NextDouble() * 2.0 - 1.0); // Generate white noise
            float lfo = (float)(0.5 * (1.0 + Math.Sin(2.0 * Math.PI * lfoFrequency * time)));  // LFO for amplitude modulation
            return lfo * noise;
        }

        // Warped Sine Wave (Alien Drone)
        private static float GenerateWarpedSineWave(float time, float frequency)
        {
            // Introduce some irregularity by modulating the frequency with a sine wave
            float modulation = (float)(Math.Sin(2.0 * Math.PI * frequency * time * 0.25) * 0.5);
            return (float)Math.Sin(2.0 * Math.PI * (frequency + modulation) * time);
        }

        // Fractal Sawtooth (Cosmic Ripples)
        private static float GenerateFractalSawtoothWave(float time, float frequency)
        {
            float period = 1.0f / frequency;
            float sawtooth = 2.0f * (time / period - (float)Math.Floor(0.5 + time / period));

            // Fractal self-modulation effect
            float fractalMod = 0.5f * sawtooth * (float)Math.Sin(2.0 * Math.PI * frequency * time * 0.1);

            return sawtooth + fractalMod;
        }

        // Detuned Dual Sine Waves (Phased Alien Tone)
        private static float GenerateDetunedSineWave(float time, float frequency)
        {
            float baseSine = (float)Math.Sin(2.0 * Math.PI * frequency * time);
            float detunedSine = (float)Math.Sin(2.0 * Math.PI * (frequency * 0.995f) * time);  // Slightly detuned

            return 0.5f * (baseSine + detunedSine);  // Mix the two sine waves
        }

        // Phase Distorted Wave (Radio Signal Distortion)
        private static float GeneratePhaseDistortedSine(float time, float frequency)
        {
            // Introduce phase distortion by altering the time parameter non-linearly
            float phase = time + 0.5f * (float)Math.Sin(2.0 * Math.PI * time * 0.1f);  // Slow-moving modulation
            return (float)Math.Sin(2.0 * Math.PI * frequency * phase);
        }

    }
}
