using Continuum93.Emulator;
using System;

namespace Continuum93.Emulator.Audio.XSound.ExecutionUtilities
{
    public static class XSoundParamsProcessing
    {
        public static XSoundParams ReadSoundParams(uint address, Computer computer)
        {
            XSoundParams soundParams = new();

            XSoundFlags flags = new(computer.MEMC.GetSafe16bitFromRAM(address));
            address += 2;
            soundParams.Frequency = computer.MEMC.GetFloatFromRAM(address);
            address += 4;
            soundParams.EnvelopeSustain = computer.MEMC.GetFloatFromRAM(address);
            address += 4;
            soundParams.SoundVolume = computer.MEMC.GetFloatFromRAM(address);
            address += 4;

            // WaveType handling
            if (flags.GetFlagValue(XFlags.WaveType))
            {
                soundParams.WaveType = GetWaveTypeFromByte(computer.MEMC.GetSafe8bitFromRAM(address));
                address += 1;

                if (soundParams.WaveType == XSoundParams.WaveTypes.COMPLEX)
                {
                    byte waveTypesCount = computer.MEMC.GetSafe8bitFromRAM(address);
                    address += 1;

                    for (int i = 0; i < waveTypesCount; i++)
                    {
                        byte waveType = computer.MEMC.GetSafe8bitFromRAM(address);
                        address += 1;
                        byte waveWeight = (byte)(computer.MEMC.GetSafe8bitFromRAM(address) % 100);
                        address += 1;

                        soundParams.WaveTypeValues.Add(GetWaveTypeFromByte(waveType), waveWeight);
                    }
                }
            }

            // General envelope settings
            if (flags.GetFlagValue(XFlags.Envelope))
            {
                soundParams.EnvelopeAttack = computer.MEMC.GetFloatFromRAM(address);
                address += 4;

                soundParams.EnvelopeDecay = computer.MEMC.GetFloatFromRAM(address);
                address += 4;
            }

            // Envelope punch
            if (flags.GetFlagValue(XFlags.EnvelopePunch))
            {
                soundParams.EnvelopePunch = computer.MEMC.GetFloatFromRAM(address);
                address += 4;
            }

            // General frequency settings
            if (flags.GetFlagValue(XFlags.Frequency))
            {
                soundParams.FreqLimit = computer.MEMC.GetFloatFromRAM(address);
                address += 4;

                soundParams.FreqRamp = computer.MEMC.GetFloatFromRAM(address);
                address += 4;

                soundParams.FreqDramp = computer.MEMC.GetFloatFromRAM(address);
                address += 4;
            }

            // General vibrato settings
            if (flags.GetFlagValue(XFlags.Vibrato))
            {
                soundParams.VibratoDepth = computer.MEMC.GetFloatFromRAM(address);
                address += 4;

                soundParams.VibratoSpeed = computer.MEMC.GetFloatFromRAM(address);
                address += 4;
            }

            // General arpeggio settings
            if (flags.GetFlagValue(XFlags.Arpeggio))
            {
                soundParams.ArpeggioMod = computer.MEMC.GetFloatFromRAM(address);
                address += 4;

                soundParams.ArpeggioSpeed = computer.MEMC.GetFloatFromRAM(address);
                address += 4;
            }

            // General duty cycle settings
            if (flags.GetFlagValue(XFlags.DutyCycle))
            {
                soundParams.DutyCycle = computer.MEMC.GetFloatFromRAM(address);
                address += 4;

                soundParams.DutyCycleRamp = computer.MEMC.GetFloatFromRAM(address);
                address += 4;
            }

            // Repeat
            if (flags.GetFlagValue(XFlags.Repeat))
            {
                soundParams.RepeatSpeed = computer.MEMC.GetFloatFromRAM(address);
                address += 4;
            }

            // General flanger settings
            if (flags.GetFlagValue(XFlags.Flanger))
            {
                soundParams.FlangerOffset = computer.MEMC.GetFloatFromRAM(address);
                address += 4;

                soundParams.FlangerRamp = computer.MEMC.GetFloatFromRAM(address);
                address += 4;
            }

            // General phaser settings
            if (flags.GetFlagValue(XFlags.Phaser))
            {
                soundParams.PhaserOffset = computer.MEMC.GetFloatFromRAM(address);
                address += 4;

                soundParams.PhaserSweep = computer.MEMC.GetFloatFromRAM(address);
                address += 4;
            }

            // General low pass filter settings
            if (flags.GetFlagValue(XFlags.LowPassFilter))
            {
                soundParams.LpfFreq = computer.MEMC.GetFloatFromRAM(address);
                address += 4;

                soundParams.LpfRamp = computer.MEMC.GetFloatFromRAM(address);
                address += 4;

                soundParams.LpfResonance = computer.MEMC.GetFloatFromRAM(address);
                address += 4;
            }

            // General high pass filter settings
            if (flags.GetFlagValue(XFlags.HighPassFilter))
            {
                soundParams.HpfFreq = computer.MEMC.GetFloatFromRAM(address);
                address += 4;

                soundParams.HpfRamp = computer.MEMC.GetFloatFromRAM(address);
                address += 4;
            }

            // Bit depth
            if (flags.GetFlagValue(XFlags.BitDepth))
            {
                soundParams.BitDepth = computer.MEMC.GetSafe8bitFromRAM(address);
                address += 1;
            }

            // Sample rate
            if (flags.GetFlagValue(XFlags.SampleRate))
            {
                soundParams.SampleRate = computer.MEMC.GetFloatFromRAM(address);
                address += 4;
            }

            return soundParams;
        }

        private static XSoundParams.WaveTypes GetWaveTypeFromByte(byte value)
        {
            if (Enum.IsDefined(typeof(XSoundParams.WaveTypes), (int)value))
            {
                return (XSoundParams.WaveTypes)value;
            }
            else
            {
                throw new ArgumentException($"Invalid wave type value: {value}");
            }
        }
    }
}
