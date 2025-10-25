using System;

namespace Continuum93.Emulator.Audio.XSound.Utilities
{
    public class XPhaserController
    {
        private readonly XSoundParams parameters;

        private float phaserOffset;
        private float phaserOffsetSlide;
        private readonly float[] phaserBuffer;
        private int phaserBufferPos;

        private const int PhaserBufferSize = 1024;
        private readonly bool enablePhaser;

        public XPhaserController(XSoundParams parameters)
        {
            this.parameters = parameters;
            enablePhaser = parameters.PhaserOffset != 0.0f || parameters.PhaserSweep != 0.0f;

            if (enablePhaser)
            {
                phaserBuffer = new float[PhaserBufferSize];
                InitializePhaser(); // Extracted method for initialization
            }
        }

        /// <summary>
        /// Processes a single sample through the phaser effect.
        /// </summary>
        /// <param name="sample">The input audio sample.</param>
        /// <returns>The processed sample with phaser effect applied.</returns>
        public float ProcessSample(float sample)
        {
            if (!enablePhaser)
            {
                return sample;
            }

            // Update phaser offset
            phaserOffset += phaserOffsetSlide;
            int offset = Math.Min((int)Math.Abs(phaserOffset), PhaserBufferSize - 1);

            // Store the current sample in the buffer
            phaserBuffer[phaserBufferPos] = sample;

            // Calculate the delayed buffer index
            int bufferIndex = (phaserBufferPos - offset + PhaserBufferSize) % PhaserBufferSize;

            // Get the delayed sample from the buffer
            float phaserSample = phaserBuffer[bufferIndex];

            // Increment buffer position
            phaserBufferPos = (phaserBufferPos + 1) % PhaserBufferSize;

            // Apply phaser effect
            return sample + phaserSample;
        }

        /// <summary>
        /// Resets the phaser state, reinitializing variables and the buffer.
        /// </summary>
        public void Reset()
        {
            if (enablePhaser)
            {
                InitializePhaser();
            }
        }

        /// <summary>
        /// Initializes or resets phaser-related variables and clears the buffer.
        /// </summary>
        private void InitializePhaser()
        {
            phaserOffset = MathF.Pow(parameters.PhaserOffset, 2.0f) * PhaserBufferSize * (parameters.PhaserOffset < 0.0f ? -1 : 1);
            phaserOffsetSlide = MathF.Pow(parameters.PhaserSweep, 2.0f) * (parameters.PhaserSweep < 0.0f ? -1 : 1);

            phaserBufferPos = 0;
            Array.Clear(phaserBuffer, 0, phaserBuffer.Length);
        }
    }
}
