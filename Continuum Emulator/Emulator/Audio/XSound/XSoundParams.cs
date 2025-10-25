using System.Collections.Generic;

namespace Continuum93.Emulator.Audio.XSound
{
    public class XSoundParams
    {
        public enum WaveTypes
        {
            SINE = 0,
            CLIPPEDSINE = 1,
            HALFSINE = 2,

            SQUARE = 3,
            PULSEWAVE = 4,

            TRIANGLE = 5,
            RAMPWAVE = 6,

            SAWTOOTH = 7,
            REVERSESAWTOOTH = 8,

            BREAKER = 9,
            TAN = 10,
            EXPONENTIAL = 11,
            WAVEFOLDING = 12,

            WHISTLE = 13,
            ORGAN = 14,
            FORMANT = 15,

            RINGMODULATION = 16,
            LFO = 17,
            WARPEDSINE = 18,
            FRACTALSAWTOOTH = 19,
            DETUNEDSINE = 20,
            PHASEDISTORDETSINE = 21,

            NOISE = 50,
            PINKNOISE = 51,
            GAUSSIANNOISE = 52,

            COMPLEX = 100
        }

        // Wave type
        public WaveTypes WaveType = WaveTypes.SINE;

        // Mapping WaveTypes to values, used when WaveTypes.COMPLEX is active
        public Dictionary<WaveTypes, int> WaveTypeValues = new();


        // Envelope parameters
        public float EnvelopeAttack = 0.0f;   // Attack time (in seconds)
        public float EnvelopeSustain = 0.0f;  // Sustain time (in seconds)
        public float EnvelopePunch = 0.0f;    // Sustain punch (boosted sustain level)
        public float EnvelopeDecay = 0.0f;    // Decay time (in seconds)

        public float FreqLimit = 0.0f;
        public float FreqRamp = 0.0f;
        public float FreqDramp = 0.0f;

        // Other params
        public float SampleRate = 44100;
        public int BitDepth = 16;
        public float Frequency = 440.0f;
        public float SoundVolume = 0.5f; // Master volume control

        // Duty Cycle Parameters
        public float DutyCycle = 0.0f;       // Initial duty cycle (0 to 0.5)
        public float DutyCycleRamp = 0.0f;   // Duty cycle sweep (SIGNED)

        // Filter Parameters
        public float LpfFreq = 1.0f;        // Low-pass filter cutoff frequency (0 to 1)
        public float LpfRamp = 0.0f;        // Low-pass filter cutoff sweep (SIGNED)
        public float LpfResonance = 0.0f;   // Low-pass filter resonance (0 to 1)
        public float HpfFreq = 0.0f;        // High-pass filter cutoff frequency (0 to 1)
        public float HpfRamp = 0.0f;        // High-pass filter cutoff sweep (SIGNED)

        // Vibrato parameters
        public float VibratoDepth = 0.0f;    // Vibrato depth
        public float VibratoSpeed = 0.0f;    // Vibrato speed

        // Arpeggio Parameters
        public float ArpeggioMod = 0.0f;    // Frequency multiplier (SIGNED)
        public float ArpeggioSpeed = 0.0f;  // Change speed

        // Repeat Parameters
        public float RepeatSpeed = 0.0f;    // Repeat speed (0 to 1)

        // Flanger Parameters
        public float FlangerOffset = 0.0f;    // Flanger offset (SIGNED)
        public float FlangerRamp = 0.0f;      // Flanger sweep (SIGNED)

        // Phaser Parameters
        public float PhaserOffset = 0.0f; // Phaser offset (from -1.0 to +1.0)
        public float PhaserSweep = 0.0f;  // Phaser sweep (from -1.0 to +1.0)
    }
}
