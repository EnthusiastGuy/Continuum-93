using System;

namespace Continuum93.Emulator.Audio.XSound.Generators
{
    /// <summary>
    /// Controls the flanger effect applied to audio samples.
    /// </summary>
    public class XFlangerController
    {
        private readonly XSoundParams parameters;
        private float flangerOffset;
        private float flangerOffsetSlide;
        private float[] flangerBuffer;
        private int flangerBufferPos;
        private readonly bool enableFlanger;

        private const int FlangerBufferSize = 1024;

        /// <summary>
        /// Initializes a new instance of the <see cref="XFlangerController"/> class.
        /// </summary>
        /// <param name="parameters">The sound parameters.</param>
        public XFlangerController(XSoundParams parameters)
        {
            this.parameters = parameters;

            // Determine if flanger should be enabled
            enableFlanger = parameters.FlangerOffset != 0.0f || parameters.FlangerRamp != 0.0f;

            if (enableFlanger)
            {
                // Flanger initialization
                flangerOffset = parameters.FlangerOffset * parameters.FlangerOffset * 1020.0f;
                if (parameters.FlangerOffset < 0.0f) flangerOffset = -flangerOffset;

                flangerOffsetSlide = parameters.FlangerRamp * parameters.FlangerRamp;
                if (parameters.FlangerRamp < 0.0f) flangerOffsetSlide = -flangerOffsetSlide;

                flangerBuffer = new float[FlangerBufferSize];
                flangerBufferPos = 0;

                // Initialize flanger buffer with zeros
                Array.Clear(flangerBuffer, 0, FlangerBufferSize);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the flanger effect is enabled.
        /// </summary>
        public bool IsEnabled => enableFlanger;

        /// <summary>
        /// Processes a single audio sample by applying the flanger effect.
        /// </summary>
        /// <param name="sample">The input audio sample.</param>
        /// <returns>The processed audio sample with flanger effect applied.</returns>
        public float ProcessSample(float sample)
        {
            if (!enableFlanger)
            {
                return sample;
            }

            // Update flanger offset
            flangerOffset += flangerOffsetSlide;
            int offset = (int)Math.Abs(flangerOffset);
            if (offset > FlangerBufferSize - 1) offset = FlangerBufferSize - 1;

            // Calculate buffer index with wrapping
            int bufferIndex = (flangerBufferPos - offset + FlangerBufferSize) % FlangerBufferSize;

            // Add delayed sample from buffer
            float flangedSample = flangerBuffer[bufferIndex];
            float outputSample = sample + flangedSample;

            // Store current output sample in flanger buffer
            flangerBuffer[flangerBufferPos] = outputSample;
            flangerBufferPos = (flangerBufferPos + 1) % FlangerBufferSize;

            return outputSample;
        }

        /// <summary>
        /// Resets the flanger controller to its initial state.
        /// </summary>
        public void Reset()
        {
            if (enableFlanger)
            {
                // Reset flanger variables
                flangerOffset = parameters.FlangerOffset * parameters.FlangerOffset * 1020.0f;
                if (parameters.FlangerOffset < 0.0f) flangerOffset = -flangerOffset;

                flangerOffsetSlide = parameters.FlangerRamp * parameters.FlangerRamp;
                if (parameters.FlangerRamp < 0.0f) flangerOffsetSlide = -flangerOffsetSlide;

                flangerBufferPos = 0;

                // Reinitialize flanger buffer with zeros
                Array.Clear(flangerBuffer, 0, FlangerBufferSize);
            }
        }
    }
}
