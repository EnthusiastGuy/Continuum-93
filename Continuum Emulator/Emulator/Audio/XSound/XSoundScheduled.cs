namespace Continuum93.Emulator.Audio.XSound
{
    public class XSoundScheduled
    {
        public XSoundParams SoundParams { get; set; }
        public int Delay { get; set; } // Delay in milliseconds

        public XSoundScheduled(XSoundParams soundParams, int delay)
        {
            SoundParams = soundParams;
            Delay = delay;
        }
    }
}
