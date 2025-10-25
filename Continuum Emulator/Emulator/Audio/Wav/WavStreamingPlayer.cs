namespace Continuum93.Emulator.Audio.Wav
{
    using Microsoft.Xna.Framework.Audio;
    using System;
    using System.IO;

    public class WavStreamingPlayer : IDisposable
    {
        private const int DefaultBufferMillis = 250; // audio buffer (~ms)
        private DynamicSoundEffectInstance _dynamicSound;
        private FileStream _fileStream;
        private long _dataStartPosition;
        private long _dataLength;

        private bool _isValid = true;    // If false, we won’t attempt playback
        private bool _isPlaying;
        private bool _isLooped;
        private bool _isDisposed;
        private bool _stopRequested;     // For handling Stop logic
        private bool _pauseRequested;    // For handling Pause logic

        // We keep some PCM format info from the .wav
        private int _sampleRate;
        private int _channels;
        private int _bitsPerSample;      // Typically 16
        private int _bytesPerFrame;      // channels * (bitsPerSample/8)

        public bool IsValid => _isValid;
        public bool IsPlaying => _isPlaying && !_pauseRequested && !_stopRequested;
        public bool IsPaused => _pauseRequested;
        public bool IsStopped => _stopRequested || !_isPlaying;

        public WavStreamingPlayer(string path)
        {
            try
            {
                _fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);

                ParseWaveFileHeader(_fileStream,
                                    out _sampleRate,
                                    out _channels,
                                    out _bitsPerSample,
                                    out _dataStartPosition,
                                    out _dataLength);

                _bytesPerFrame = _channels * (_bitsPerSample / 8);

                _dynamicSound = new DynamicSoundEffectInstance(_sampleRate,
                    _channels == 2 ? AudioChannels.Stereo : AudioChannels.Mono);

                _dynamicSound.BufferNeeded += DynamicSound_BufferNeeded;
            }
            catch (InvalidDataException ex)
            {
                // Mark invalid, possibly log the error
                _isValid = false;

                // Clean up the file stream if it was opened
                _fileStream?.Dispose();
                _fileStream = null;

                // Optionally log or store `ex.Message`
                // Console.WriteLine($"Invalid WAV file: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Catch other exceptions as well
                _isValid = false;
                _fileStream?.Dispose();
                _fileStream = null;
                // Log or handle as needed
            }
        }

        /// <summary>
        /// Attempt to read the next chunk from the file and submit it to the DynamicSoundEffectInstance.
        /// </summary>
        private void DynamicSound_BufferNeeded(object sender, EventArgs e)
        {
            // If we’re paused, stopped, or not playing, don’t feed any more data
            if (!_isValid || _pauseRequested || _stopRequested || !_isPlaying)
                return;

            // We’ll keep submitting buffers until we have at least 2 queued
            // (You can change this to 3 or more if you prefer.)
            while (_dynamicSound.PendingBufferCount < 2 &&
                   !_pauseRequested && !_stopRequested && _isPlaying)
            {
                // Calculate how many bytes per chunk
                int bytesPerSec = _sampleRate * _bytesPerFrame;
                int bufferSize = (bytesPerSec * DefaultBufferMillis) / 1000; // e.g. 500ms
                byte[] buffer = new byte[bufferSize];

                int totalBytesRead = 0;

                // Try to fill the entire buffer. Loop if we hit end-of-file but _isLooped is true.
                while (totalBytesRead < bufferSize)
                {
                    int bytesNeeded = bufferSize - totalBytesRead;
                    int bytesRead = _fileStream.Read(buffer, totalBytesRead, bytesNeeded);

                    if (bytesRead == 0)
                    {
                        // We've reached the end of the file
                        if (_isLooped)
                        {
                            // Loop back to start of data
                            _fileStream.Seek(_dataStartPosition, SeekOrigin.Begin);
                            continue; // Attempt to read again
                        }
                        else
                        {
                            // If not looping, stop automatically
                            Stop();
                            return;
                        }
                    }

                    totalBytesRead += bytesRead;
                }

                // If still playing and not paused/stopped, submit this buffer
                if (_isPlaying && !_stopRequested && !_pauseRequested)
                {
                    _dynamicSound.SubmitBuffer(buffer, 0, totalBytesRead);
                }
            }
        }

        /// <summary>
        /// Start or resume playback.
        /// </summary>
        public void Play()
        {
            if (!_isValid || _isDisposed) return;

            if (!_isPlaying)
            {
                // If we haven't started at all or we had stopped, reset to data start
                if (_stopRequested)
                {
                    _fileStream.Seek(_dataStartPosition, SeekOrigin.Begin);
                    _stopRequested = false;
                }

                // If paused, we just resume
                _pauseRequested = false;
                _isPlaying = true;

                // Start the dynamic sound
                _dynamicSound.Play();
            }
            else if (_pauseRequested)
            {
                // We were paused, so just resume
                _pauseRequested = false;
                _dynamicSound.Resume();
            }
            // If already playing, do nothing
        }

        /// <summary>
        /// Pause playback but do not reset position.
        /// </summary>
        public void Pause()
        {
            if (!_isValid || _isDisposed) return;

            if (!_pauseRequested && _isPlaying)
            {
                _pauseRequested = true;
                _dynamicSound.Pause();
            }
        }

        /// <summary>
        /// Stop playback and reset to the beginning of the data chunk.
        /// </summary>
        public void Stop()
        {
            if (!_isValid || _isDisposed) return;

            if (!_stopRequested || _isPlaying)
            {
                _stopRequested = true;
                _pauseRequested = false;
                _isPlaying = false;
                _dynamicSound.Stop();
                // Reset to beginning of data to be ready for next Play
                _fileStream.Seek(_dataStartPosition, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// Set volume (0.0f to 1.0f).
        /// </summary>
        public void SetVolume(float value)
        {
            if (!_isValid || _isDisposed) return;
            _dynamicSound.Volume = Math.Clamp(value, 0f, 1f);
        }

        /// <summary>
        /// Set looping behavior.
        /// </summary>
        public void SetLoop(bool loop)
        {
            _isLooped = loop;
        }

        #region WAV Header Parsing
        private void ParseWaveFileHeader(Stream stream,
            out int sampleRate, out int channels, out int bitsPerSample,
            out long dataStartPos, out long dataLength)
        {
            // Minimal WAV header parser (for uncompressed PCM)
            using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);

            // RIFF header
            string chunkId = new(reader.ReadChars(4)); // "RIFF"
            if (chunkId != "RIFF")
                throw new InvalidDataException("Not a valid WAV file (missing RIFF).");

            _ = reader.ReadInt32(); // file size (unused here)

            string format = new(reader.ReadChars(4)); // "WAVE"
            if (format != "WAVE")
                throw new InvalidDataException("Not a valid WAV file (missing WAVE).");

            // "fmt " sub-chunk
            string subChunkId = new string(reader.ReadChars(4)); // "fmt "
            if (subChunkId != "fmt ")
                throw new InvalidDataException("Not a valid WAV file (missing fmt ).");

            int subChunkSize = reader.ReadInt32(); // 16 for PCM
            short audioFormat = reader.ReadInt16(); // 1 for PCM
            if (audioFormat != 1)
                throw new InvalidDataException("Only PCM WAV files are supported.");

            short numChannels = reader.ReadInt16();
            int sRate = reader.ReadInt32();
            _ = reader.ReadInt32(); // byteRate
            _ = reader.ReadInt16(); // blockAlign
            short bps = reader.ReadInt16();

            // Skip any leftover bytes in the 'fmt ' chunk if subChunkSize > 16
            if (subChunkSize > 16)
            {
                reader.ReadBytes(subChunkSize - 16);
            }

            // Now find the "data" sub-chunk
            string dataChunkId = new(reader.ReadChars(4)); // likely "data"
            if (dataChunkId != "data")
            {
                // If it's not "data", skip other chunks until "data" is found
                int nextChunkSize = reader.ReadInt32();
                reader.ReadBytes(nextChunkSize);
                // Attempt to read "data" again
                dataChunkId = new string(reader.ReadChars(4));
                if (dataChunkId != "data")
                {
                    throw new InvalidDataException("Could not find data chunk in WAV file.");
                }
            }
            int dataSize = reader.ReadInt32();
            long dataStart = reader.BaseStream.Position;

            // Return all info
            sampleRate = sRate;
            channels = numChannels;
            bitsPerSample = bps;
            dataStartPos = dataStart;
            dataLength = dataSize;
        }
        #endregion

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                _dynamicSound.Dispose();
                _fileStream.Dispose();
            }
        }

    }
}
