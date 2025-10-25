using Continuum93.Emulator.Audio.XSound;

namespace Continuum93.Emulator.Audio.Samples
{
    public static class TestSounds
    {
        public static XSoundParams GetXSound(float freq = 8400)
        {

            var soundParams = new XSoundParams
            {
                WaveType = XSoundParams.WaveTypes.SQUARE,
                Frequency = 440.0f,
                EnvelopeAttack = 0.0f,
                EnvelopeSustain = 0.3f,
                EnvelopeDecay = 0.4f,
                SoundVolume = 0.5f,

                // Repeat parameters
                RepeatSpeed = 0.5f, // Moderate repeat speed

                PhaserOffset = 0.5f,
                PhaserSweep = 0.2f,
            };

            return soundParams;

            /*return new XSoundParams()
            {
                SampleRate = 44100,
                WaveType = XSoundParams.WaveTypes.WHISTLE,
                WaveTypeValues = {
                    { XSoundParams.WaveTypes.SINE, 30 },
                    { XSoundParams.WaveTypes.TRIANGLE, 50 },
                    { XSoundParams.WaveTypes.WHISTLE, 20 }
                },

                Frequency = freq,

                EnvelopeAttack = 0,
                EnvelopeDecay = 0,
                EnvelopePunch = 0,
                EnvelopeSustain = 2.8f,

                SoundVolume = 0.5f,
                LpfFreq = 1f,
                FlangerOffset = 0f,
                FlangerRamp = 0f,

                ArpeggioMod = 0.0f,
                ArpeggioSpeed = 0.0f,
            };*/
        }

    }
}
