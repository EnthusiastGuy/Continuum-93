using Continuum93.Emulator.Audio.Samples;
using Continuum93.Emulator.Audio.Wav;
using Continuum93.Emulator.Audio.XSound;
using Continuum93.Emulator.Audio.XSound.Debug;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Continuum93.Emulator.Audio.Ogg; // <-- for OggStreamingPlayer

namespace Continuum93.Emulator.Audio
{
    public class APUThreadWrapper : IDisposable
    {
        private readonly APU _apu;
        private readonly ConcurrentQueue<Action> _commandQueue;
        private readonly Task _apuTask;
        private bool _isRunning;
        private XSoundParams lastPlayedSound;

        // We keep Wav players in a dictionary keyed by channel
        private readonly Dictionary<byte, WavStreamingPlayer> _wavPlayers;

        // We keep Ogg players in a dictionary keyed by channel
        private readonly Dictionary<byte, OggStreamingPlayer> _oggPlayers;

        public APUThreadWrapper()
        {
            _apu = new APU();
            _commandQueue = new ConcurrentQueue<Action>();
            _isRunning = true;
            _wavPlayers = new Dictionary<byte, WavStreamingPlayer>();
            _oggPlayers = new Dictionary<byte, OggStreamingPlayer>();

            // Start the APU thread as a Task
            _apuTask = Task.Run(APUThreadLoop);
        }

        // The main loop for the APU running in a separate thread
        private void APUThreadLoop()
        {
            while (_isRunning)
            {
                // Process any commands in the queue
                while (_commandQueue.TryDequeue(out Action command))
                {
                    command?.Invoke();  // Execute the command
                }

                // Sleep briefly to prevent busy-waiting
                Thread.Sleep(10);
            }

            _apu.Dispose();
        }

        // -------------------------------------------------------------------
        //  XSound Methods
        // -------------------------------------------------------------------
        // The original XSound-based approach
        public void PlaySound(int soundIndex)
        {
            _commandQueue.Enqueue(() => _apu.PlaySound(soundIndex));
        }

        public void StopSound(int soundIndex)
        {
            _commandQueue.Enqueue(() => _apu.StopSound(soundIndex));
        }

        public int StoreSound(XSoundParams parameters)
        {
            int soundIndex = 0;
            var completed = new ManualResetEventSlim();

            _commandQueue.Enqueue(() =>
            {
                soundIndex = _apu.StoreSound(parameters);
                completed.Set();
            });

            completed.Wait();  // Wait for the command to be processed on the APU thread
            return soundIndex;
        }

        public void PlayInitializationSound()
        {
            PlaySound(new XSoundParams
            {
                WaveType = XSoundParams.WaveTypes.SQUARE,
                Frequency = 440.0f,
                EnvelopeAttack = 0.0f,
                EnvelopeSustain = 0.3f,
                EnvelopeDecay = 0.4f,
                SoundVolume = 0.5f,
                RepeatSpeed = 0.5f,
                PhaserOffset = 0.5f,
                PhaserSweep = 0.2f,
            });
        }

        public void PlayInitializationSounds()
        {
            var sound1 = new XSoundParams
            {
                WaveType = XSoundParams.WaveTypes.SQUARE,
                Frequency = 700.0f,
                EnvelopeAttack = 0.0f,
                EnvelopeSustain = 0.05f,
                EnvelopeDecay = 0.05f,
                SoundVolume = 0.1f,
                RepeatSpeed = 0.5f,
                PhaserOffset = 0.5f,
                PhaserSweep = 0.9f,
            };

            var sound2 = new XSoundParams
            {
                WaveType = XSoundParams.WaveTypes.SAWTOOTH,
                Frequency = 523.3f,
                EnvelopeAttack = 0.1f,
                EnvelopeSustain = 0.2f,
                EnvelopeDecay = 0.5f,
                SoundVolume = 0.6f,
                RepeatSpeed = 0.4f,
                PhaserOffset = 0.4f,
                PhaserSweep = 0.1f,
            };

            // Create a list of sounds with associated delays
            var scheduledSounds = new List<XSoundScheduled>
            {
                new(sound1, 0),     // Play immediately
                // new(sound2, 200)    // Play after a delay
            };

            PlaySounds(scheduledSounds);
            //XSoundDebugger.SavePlotData(sound2);
        }

        public void PlaySound(XSoundParams parameters)
        {
            lastPlayedSound = parameters;
            _commandQueue.Enqueue(() => _apu.PlaySound(lastPlayedSound));
        }

        public XSoundParams GetLastPlayedSound()
        {
            return lastPlayedSound;
        }

        public void PlaySounds(List<XSoundScheduled> scheduledSounds)
        {
            foreach (var soundWithDelay in scheduledSounds)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(soundWithDelay.Delay);
                    _commandQueue.Enqueue(() => _apu.PlaySound(soundWithDelay.SoundParams));
                });
            }
        }

        // -------------------------------------------------------------------
        //  WAV Streaming Methods
        // -------------------------------------------------------------------
        public void RegisterWav(string path, byte channel)
        {
            _commandQueue.Enqueue(() =>
            {
                if (_wavPlayers.TryGetValue(channel, out var existing))
                {
                    existing.Dispose();
                    _wavPlayers.Remove(channel);
                }

                var player = new WavStreamingPlayer(path);
                _wavPlayers[channel] = player;
            });
        }

        public void PlayWav(byte channel)
        {
            _commandQueue.Enqueue(() =>
            {
                if (_wavPlayers.TryGetValue(channel, out var player))
                {
                    player.Play();
                }
            });
        }

        public void PauseWav(byte channel)
        {
            _commandQueue.Enqueue(() =>
            {
                if (_wavPlayers.TryGetValue(channel, out var player))
                {
                    player.Pause();
                }
            });
        }

        public void StopWav(byte channel)
        {
            _commandQueue.Enqueue(() =>
            {
                if (_wavPlayers.TryGetValue(channel, out var player))
                {
                    player.Stop();
                }
            });
        }

        public void SetWavVolume(byte channel, float value)
        {
            _commandQueue.Enqueue(() =>
            {
                if (_wavPlayers.TryGetValue(channel, out var player))
                {
                    player.SetVolume(value);
                }
            });
        }

        public void SetWavLoop(byte channel, bool looped)
        {
            _commandQueue.Enqueue(() =>
            {
                if (_wavPlayers.TryGetValue(channel, out var player))
                {
                    player.SetLoop(looped);
                }
            });
        }

        // -------------------------------------------------------------------
        //  OGG Streaming Methods
        // -------------------------------------------------------------------
        /// <summary>
        /// Register an .ogg file on a given channel.
        /// If that channel is already registered, it is replaced.
        /// </summary>
        public void RegisterOgg(string path, byte channel)
        {
            _commandQueue.Enqueue(() =>
            {
                // Dispose existing if present
                if (_oggPlayers.TryGetValue(channel, out var existing))
                {
                    existing.Dispose();
                    _oggPlayers.Remove(channel);
                }

                var player = new OggStreamingPlayer(path);
                if (!player.IsValid)
                {
                    // If invalid, we might log or handle as needed
                    // For now, just store it anyway (it won't play)
                }

                _oggPlayers[channel] = player;
            });
        }

        /// <summary>
        /// Play (or resume) the .ogg registered at given channel.
        /// </summary>
        public void PlayOgg(byte channel)
        {
            _commandQueue.Enqueue(() =>
            {
                if (_oggPlayers.TryGetValue(channel, out var player))
                {
                    player.Play();
                }
            });
        }

        /// <summary>
        /// Pause the .ogg registered at given channel (does not reset position).
        /// </summary>
        public void PauseOgg(byte channel)
        {
            _commandQueue.Enqueue(() =>
            {
                if (_oggPlayers.TryGetValue(channel, out var player))
                {
                    player.Pause();
                }
            });
        }

        /// <summary>
        /// Stop the .ogg registered at given channel (resets position to start).
        /// </summary>
        public void StopOgg(byte channel)
        {
            _commandQueue.Enqueue(() =>
            {
                if (_oggPlayers.TryGetValue(channel, out var player))
                {
                    player.Stop();
                }
            });
        }

        /// <summary>
        /// Set the volume for the .ogg registered at given channel (0.0f - 1.0f).
        /// </summary>
        public void SetOggVolume(byte channel, float value)
        {
            _commandQueue.Enqueue(() =>
            {
                if (_oggPlayers.TryGetValue(channel, out var player))
                {
                    player.SetVolume(value);
                }
            });
        }

        /// <summary>
        /// Set looping for the .ogg registered at given channel.
        /// </summary>
        public void SetOggLoop(byte channel, bool looped)
        {
            _commandQueue.Enqueue(() =>
            {
                if (_oggPlayers.TryGetValue(channel, out var player))
                {
                    player.SetLoop(looped);
                }
            });
        }

        // -------------------------------------------------------------------
        //  Shutdown / Disposal
        // -------------------------------------------------------------------
        public void StopAPUThread()
        {
            _isRunning = false;
            _apuTask.Wait();
        }

        public void Dispose()
        {
            StopAPUThread();
            _apu.Dispose();

            // Dispose all Wav players
            foreach (var kvp in _wavPlayers)
            {
                kvp.Value.Dispose();
            }
            _wavPlayers.Clear();

            // Dispose all Ogg players
            foreach (var kvp in _oggPlayers)
            {
                kvp.Value.Dispose();
            }
            _oggPlayers.Clear();
        }
    }
}
