using System;

namespace Continuum93.Emulator.Audio.XSound.Utilities
{
    public class XArpeggioController
    {
        private readonly XSoundParams parameters;
        private int arpeggioTime;
        private readonly int arpeggioLimit;
        private readonly float arpeggioMultiplier;
        private readonly float baseFrequency;

        public XArpeggioController(XSoundParams parameters)
        {
            this.parameters = parameters;
            baseFrequency = parameters.Frequency;

            // Initialize arpeggio parameters
            if (parameters.ArpeggioMod >= 0.0f)
            {
                arpeggioMultiplier = 1.0f - (float)Math.Pow(parameters.ArpeggioMod, 2.0f) * 0.9f;
            }
            else
            {
                arpeggioMultiplier = 1.0f + (float)Math.Pow(parameters.ArpeggioMod, 2.0f) * 10.0f;
            }

            if (parameters.ArpeggioSpeed == 1.0f)
            {
                arpeggioLimit = 0;
            }
            else
            {
                arpeggioLimit = (int)(Math.Pow(1.0f - parameters.ArpeggioSpeed, 2.0f) * 20000.0f + 32.0f);
            }
            arpeggioTime = 0;
        }

        public float GetFrequencyMultiplier()
        {
            if (arpeggioLimit == 0)
            {
                return 1.0f; // No arpeggio effect
            }

            arpeggioTime++;
            if (arpeggioTime >= arpeggioLimit)
            {
                arpeggioTime = 0;
                return arpeggioMultiplier;
            }

            return 1.0f;
        }

        public void Reset()
        {
            arpeggioTime = 0;
        }
    }

}
