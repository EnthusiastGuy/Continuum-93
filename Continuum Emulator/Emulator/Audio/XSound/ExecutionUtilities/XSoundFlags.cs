using System.Collections.Generic;

namespace Continuum93.Emulator.Audio.XSound.ExecutionUtilities
{
    public class XSoundFlags
    {
        private readonly Dictionary<XFlags, bool> _flags = new();
        public XSoundFlags(ushort flags)
        {
            _flags[XFlags.WaveType] = (flags & (1 << 15)) != 0;
            _flags[XFlags.Envelope] = (flags & (1 << 14)) != 0;
            _flags[XFlags.EnvelopePunch] = (flags & (1 << 13)) != 0;
            _flags[XFlags.Frequency] = (flags & (1 << 12)) != 0;
            _flags[XFlags.Vibrato] = (flags & (1 << 11)) != 0;
            _flags[XFlags.Arpeggio] = (flags & (1 << 10)) != 0;
            _flags[XFlags.DutyCycle] = (flags & (1 << 9)) != 0;
            _flags[XFlags.Repeat] = (flags & (1 << 8)) != 0;
            _flags[XFlags.Flanger] = (flags & (1 << 7)) != 0;
            _flags[XFlags.Phaser] = (flags & (1 << 6)) != 0;
            _flags[XFlags.LowPassFilter] = (flags & (1 << 5)) != 0;
            _flags[XFlags.HighPassFilter] = (flags & (1 << 4)) != 0;
            _flags[XFlags.BitDepth] = (flags & (1 << 3)) != 0;
            _flags[XFlags.SampleRate] = (flags & (1 << 2)) != 0;
        }

        // Check to see if the passed flag is enabled or not
        public bool GetFlagValue(XFlags flag)
        {
            return _flags[flag];
        }
    }

    public enum XFlags
    {
        WaveType,
        Envelope,
        EnvelopePunch,
        Frequency,
        Vibrato,
        Arpeggio,
        DutyCycle,
        Repeat,
        Flanger,
        Phaser,
        LowPassFilter,
        HighPassFilter,
        BitDepth,
        SampleRate
    }
}
