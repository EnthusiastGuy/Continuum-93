namespace Continuum93.Emulator.Audio
{
    using System;
    using System.Collections.Generic;
    using Continuum93.Emulator.Audio.XSound;

    public class APU: IDisposable
    {
        private readonly List<XSoundPlayer> _activeSounds = new();
        private readonly Dictionary<int, XSoundPlayer> _soundLibrary = new();
        private int _nextSoundIndex = 1;

        // Store a sound and return its index
        public int StoreSound(XSoundParams parameters)
        {
            int index = _nextSoundIndex++;
            var soundPlayer = new XSoundPlayer(parameters);
            _soundLibrary.Add(index, soundPlayer);
            return index;
        }

        // Play a sound on a channel
        public void PlaySound(int soundIndex)
        {
            if (_soundLibrary.ContainsKey(soundIndex))
            {
                _soundLibrary[soundIndex].Play();
            }
        }

        // Play a sound directly from an XSoundParams definition
        public void PlaySound(XSoundParams parameters)
        {
            // Create a temporary XSoundPlayer with the given parameters
            var soundPlayer = new XSoundPlayer(parameters);

            // Play the sound
            soundPlayer.Play();

            _activeSounds.Add(soundPlayer);
        }

        // Stop a sound
        public void StopSound(int soundIndex)
        {
            if (_soundLibrary.ContainsKey(soundIndex))
            {
                _soundLibrary[soundIndex].Stop();
            }
        }

        // Replace a sound at a specific index
        public void ReplaceSoundAtIndex(int soundIndex, XSoundParams newParameters)
        {
            if (_soundLibrary.ContainsKey(soundIndex))
            {
                // Stop the existing sound (if it's playing)
                _soundLibrary[soundIndex].Stop();

                // Create the new sound and replace it in the dictionary
                var newSoundPlayer = new XSoundPlayer(newParameters);
                _soundLibrary[soundIndex] = newSoundPlayer;
            }
            else
            {
                throw new ArgumentException($"Sound with index {soundIndex} does not exist.");
            }
        }

        public void Dispose()
        {
            foreach (var soundPlayer in _activeSounds)
            {
                soundPlayer.Dispose();
            }
            _activeSounds.Clear();

            foreach (var soundPlayer in _soundLibrary.Values)
            {
                soundPlayer.Dispose();
            }
            _soundLibrary.Clear();
        }

        public void CleanupFinishedSounds()
        {
            for (int i = _activeSounds.Count - 1; i >= 0; i--)
            {
                var soundPlayer = _activeSounds[i];
                if (!soundPlayer.IsPlaying)
                {
                    soundPlayer.Dispose();
                    _activeSounds.RemoveAt(i);
                }
            }
        }
    }

}
