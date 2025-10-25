using Microsoft.Xna.Framework.Audio;
using System;

namespace Continuum93.Emulator.Audio.XSound
{
    public class XSoundPlayer : IDisposable
    {
        private readonly SoundEffect _soundEffect;
        private readonly SoundEffectInstance _soundEffectInstance;
        private readonly XSoundGenerator _soundGenerator;
        private readonly byte[] _soundData;

        public XSoundPlayer(XSoundParams parameters)
        {
            _soundGenerator = new XSoundGenerator(parameters);
            _soundData = _soundGenerator.GenerateSound();

            // Create the SoundEffect from the generated data
            _soundEffect = new SoundEffect(_soundData, (int)parameters.SampleRate, AudioChannels.Mono);
            _soundEffectInstance = _soundEffect.CreateInstance();
        }

        public bool IsPlaying => _soundEffectInstance.State == SoundState.Playing;

        public void Play()
        {
            if (_soundEffectInstance.State != SoundState.Playing)
            {
                _soundEffectInstance.Play();
            }
        }

        public void Stop()
        {
            if (_soundEffectInstance.State == SoundState.Playing)
            {
                _soundEffectInstance.Stop();
            }
        }

        public void Dispose()
        {
            _soundEffectInstance.Dispose();
            _soundEffect.Dispose();
        }
    }

}
