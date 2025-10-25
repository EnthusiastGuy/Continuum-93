using System;

namespace Continuum93.Emulator.Audio.XSound.Generators
{
    public class XFrequencyController
    {
        private readonly XSoundParams parameters;
        private readonly float baseFrequency;
        private float frequency;
        private readonly float minFrequency;
        private float frequencyRamp;
        private float deltaFrequencyRamp;

        private float vibratoPhase = 0.0f;
        private readonly float vibratoSpeed;
        private readonly float vibratoAmplitude;

        // Arpeggio variables
        private int arpeggioTime;
        private int arpeggioLimit;
        private readonly float arpeggioMultiplier;

        // Duty cycle variables
        private float dutyCycle;
        private float dutyCycleSlide;

        public XFrequencyController(XSoundParams parameters)
        {
            this.parameters = parameters;
            baseFrequency = parameters.Frequency;
            frequency = baseFrequency;
            minFrequency = parameters.FreqLimit;
            frequencyRamp = parameters.FreqRamp;
            deltaFrequencyRamp = parameters.FreqDramp;

            // Precompute vibrato parameters
            vibratoSpeed = parameters.VibratoSpeed * parameters.VibratoSpeed * 0.01f;
            vibratoAmplitude = parameters.VibratoDepth * 0.5f;

            // Initialize arpeggio parameters
            arpeggioTime = 0;
            arpeggioLimit = CalculateArpeggioLimit(parameters.ArpeggioSpeed);
            arpeggioMultiplier = CalculateArpeggioMultiplier(parameters.ArpeggioMod);

            // Initialize duty cycle parameters
            dutyCycle = parameters.DutyCycle;
            dutyCycleSlide = -parameters.DutyCycleRamp * 0.00005f;
        }

        public float GetFrequency()
        {
            // Frequency slide calculations (matches SFXR behavior)
            frequency += frequencyRamp;
            frequencyRamp += deltaFrequencyRamp;

            // Ensure we don't go below the minimum frequency cutoff
            if (frequency < minFrequency)
            {
                frequency = minFrequency;
            }

            // Apply arpeggio (modifies frequency directly)
            UpdateArpeggio();

            // Apply vibrato
            vibratoPhase += vibratoSpeed;
            float vibratoFactor = 1.0f + (float)Math.Sin(vibratoPhase) * vibratoAmplitude;

            return frequency * vibratoFactor;
        }

        public float GetDutyCycle()
        {
            dutyCycle += dutyCycleSlide;
            if (dutyCycle < 0.0f) dutyCycle = 0.0f;
            if (dutyCycle > 1.0f) dutyCycle = 1.0f;
            return dutyCycle;
        }

        private void UpdateArpeggio()
        {
            if (arpeggioLimit == 0)
            {
                return; // No arpeggio effect
            }

            arpeggioTime++;
            if (arpeggioTime >= arpeggioLimit)
            {
                arpeggioTime = 0;
                frequency *= arpeggioMultiplier;
            }
        }

        public void Reset()
        {
            frequency = baseFrequency;
            frequencyRamp = parameters.FreqRamp;
            deltaFrequencyRamp = parameters.FreqDramp;
            vibratoPhase = 0.0f;

            // Reset arpeggio variables
            arpeggioTime = 0;
            arpeggioLimit = CalculateArpeggioLimit(parameters.ArpeggioSpeed);

            // Reset duty cycle parameters
            dutyCycle = 0.5f - parameters.DutyCycle * 0.5f;
            dutyCycleSlide = -parameters.DutyCycleRamp * 0.00005f;
        }

        private int CalculateArpeggioLimit(float arpeggioSpeed)
        {
            if (arpeggioSpeed >= 1.0f)
            {
                return 0;
            }
            float factor = 1.0f - arpeggioSpeed;
            return (int)(factor * factor * 20000.0f + 32.0f);
        }

        private float CalculateArpeggioMultiplier(float arpeggioMod)
        {
            float modSquared = arpeggioMod * arpeggioMod;
            if (arpeggioMod >= 0.0f)
            {
                return 1.0f - modSquared * 0.9f;
            }
            else
            {
                return 1.0f + modSquared * 10.0f;
            }
        }
    }
}
