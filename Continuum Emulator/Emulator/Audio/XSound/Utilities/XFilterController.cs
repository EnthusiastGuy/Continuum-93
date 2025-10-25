using System;

namespace Continuum93.Emulator.Audio.XSound.Generators
{
    public class XFilterController
    {
        private readonly XSoundParams parameters;

        // Filter state variables
        private float fltp;
        private float fltdp;
        private float fltphp;

        // Filter parameters
        private float fltw;
        private float fltw_d;
        private float fltdmp;
        private float flthp;
        private float flthp_d;
        private readonly bool enableLowPassFilter;

        public XFilterController(XSoundParams parameters)
        {
            this.parameters = parameters;

            // Initialize filter state variables
            fltp = 0.0f;
            fltdp = 0.0f;
            fltphp = 0.0f;

            // Initialize filter parameters
            fltw = (float)Math.Pow(parameters.LpfFreq, 3) * 0.1f;
            enableLowPassFilter = parameters.LpfFreq != 1.0f;
            fltw_d = 1.0f + parameters.LpfRamp * 0.0001f;
            fltdmp = 5.0f / (1.0f + (float)Math.Pow(parameters.LpfResonance, 2) * 20.0f) * (0.01f + fltw);
            if (fltdmp > 0.8f) fltdmp = 0.8f;

            flthp = (float)Math.Pow(parameters.HpfFreq, 2) * 0.1f;
            flthp_d = 1.0f + parameters.HpfRamp * 0.0003f;
        }

        public float ProcessSample(float subSample)
        {
            // Update low-pass filter frequency
            fltw *= fltw_d;
            if (fltw < 0.0f) fltw = 0.0f;
            if (fltw > 0.1f) fltw = 0.1f;

            // Low-pass filter processing
            float previousFltp = fltp;
            if (enableLowPassFilter)
            {
                fltdp += (subSample - fltp) * fltw;
                fltdp -= fltdp * fltdmp;
            }
            else
            {
                fltp = subSample;
                fltdp = 0.0f;
            }
            fltp += fltdp;

            // Update high-pass filter frequency
            flthp *= flthp_d;
            if (flthp < 0.0f) flthp = 0.0f;
            if (flthp > 0.1f) flthp = 0.1f;

            // High-pass filter processing
            fltphp += fltp - previousFltp;
            fltphp -= fltphp * flthp;

            // Return the filtered sample
            return fltphp;
        }

        public void Reset()
        {
            // Reset filter state variables
            fltp = 0.0f;
            fltdp = 0.0f;
            fltphp = 0.0f;

            // Reinitialize filter parameters if necessary
            fltw = (float)Math.Pow(parameters.LpfFreq, 3) * 0.1f;
            fltw_d = 1.0f + parameters.LpfRamp * 0.0001f;
            fltdmp = 5.0f / (1.0f + (float)Math.Pow(parameters.LpfResonance, 2) * 20.0f) * (0.01f + fltw);
            if (fltdmp > 0.8f) fltdmp = 0.8f;

            flthp = (float)Math.Pow(parameters.HpfFreq, 2) * 0.1f;
            flthp_d = 1.0f + parameters.HpfRamp * 0.0003f;
        }

    }

}
