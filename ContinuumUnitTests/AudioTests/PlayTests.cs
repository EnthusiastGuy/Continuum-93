using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;
using Continuum93.Emulator.Audio.XSound;
using static Continuum93.Emulator.Audio.XSound.XSoundParams;

namespace Audio
{
    public class PlayTests
    {
        [Theory]
        [InlineData("0b10000000, 0b00000000, 440.0, 200.0, 1.0, 0x08", 440, 200, 1.0f, WaveTypes.REVERSESAWTOOTH)]
        [InlineData("0b10000000, 0b00000000, 440.0, 200.0, 1.0, 100, 3, 1, 10, 2, 20, 3, 30", 440, 200, 1.0f, WaveTypes.COMPLEX)]
        [InlineData("0b01000000, 0b00000000, 440.0, 200.0, 1.0, 0.5, 0.8", 440, 200, 1.0f, WaveTypes.SINE, 0.5f, 0.8f)]
        [InlineData("0b11000000, 0b00000000, 440.0, 200.0, 1.0, 52, 0.5, 0.8", 440, 200, 1.0f, WaveTypes.GAUSSIANNOISE, 0.5f, 0.8f)]
        [InlineData("0b00100000, 0b00000000, 440.0, 200.0, 1.0, 0.7", 440, 200, 1.0f, WaveTypes.SINE, null, null, 0.7f)]
        [InlineData("0b00010000, 0b00000000, 440.0, 200.0, 1.0, 0.1, 0.2, 0.3", 440, 200, 1.0f, WaveTypes.SINE, null, null, null, 0.1f, 0.2f, 0.3f)]
        [InlineData("0b00001000, 0b00000000, 440.0, 200.0, 1.0, 0.35, 0.45", 440, 200, 1.0f, WaveTypes.SINE, null, null, null, null, null, null, 0.35f, 0.45f)]
        [InlineData("0b00000100, 0b00000000, 440.0, 200.0, 1.0, 0.11, 0.22", 440, 200, 1.0f, WaveTypes.SINE, null, null, null, null, null, null, null, null, 0.11f, 0.22f)]
        public void TestPlayParameters(
            string soundDefinition,
            float expectedFrequency,
            float expectedEnvelopeSustain,
            float expectedSoundVolume,
            WaveTypes expectedWaveType,
            float? expectedAttack = null,
            float? expectedDecay = null,
            float? expectedPunch = null,
            float? expectedFreqLimit = null,
            float? expectedFreqRamp = null,
            float? expectedFreqDRamp = null,
            float? expectedVibratoDepth = null,
            float? expectedVibratoSpeed = null,
            float? expectedArpegioMod = null,
            float? expectedArpegioSpeed = null
        )
        {
            Assembler cp = new();
            using Computer computer = new(true);

            Assert.Null(computer.APU.GetLastPlayedSound());

            string code = $@"
                PLAY .SoundDefinition
                WAIT 1000
                BREAK

            .SoundDefinition
                #DB {soundDefinition}
            ";

            cp.Build(code);

            computer.LoadMemAt(0, cp.GetCompiledCode());
            computer.Run();

            XSoundParams actualParams = computer.APU.GetLastPlayedSound();

            Assert.NotNull(actualParams);
            Assert.Equal(expectedFrequency, actualParams.Frequency);
            Assert.Equal(expectedEnvelopeSustain, actualParams.EnvelopeSustain);
            Assert.Equal(expectedSoundVolume, actualParams.SoundVolume);
            Assert.Equal(expectedWaveType, actualParams.WaveType);

            // Check optional values
            AssertOptionalValue(expectedAttack, actualParams.EnvelopeAttack, nameof(actualParams.EnvelopeAttack));
            AssertOptionalValue(expectedDecay, actualParams.EnvelopeDecay, nameof(actualParams.EnvelopeDecay));
            AssertOptionalValue(expectedPunch, actualParams.EnvelopePunch, nameof(actualParams.EnvelopePunch));
            AssertOptionalValue(expectedFreqLimit, actualParams.FreqLimit, nameof(actualParams.FreqLimit));
            AssertOptionalValue(expectedFreqRamp, actualParams.FreqRamp, nameof(actualParams.FreqRamp));
            AssertOptionalValue(expectedFreqDRamp, actualParams.FreqDramp, nameof(actualParams.FreqDramp));
            AssertOptionalValue(expectedVibratoDepth, actualParams.VibratoDepth, nameof(actualParams.VibratoDepth));
            AssertOptionalValue(expectedVibratoSpeed, actualParams.VibratoSpeed, nameof(actualParams.VibratoSpeed));
            AssertOptionalValue(expectedArpegioMod, actualParams.ArpeggioMod, nameof(actualParams.ArpeggioMod));
            AssertOptionalValue(expectedArpegioSpeed, actualParams.ArpeggioSpeed, nameof(actualParams.ArpeggioSpeed));
        }

        private static void AssertOptionalValue(float? expected, float actual, string paramName)
        {
            if (expected.HasValue)
            {
                Assert.Equal(expected.Value, actual);
            }
        }
    }
}
