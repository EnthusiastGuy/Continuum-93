using Microsoft.Xna.Framework.Audio;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Continuum93.Emulator.Audio.Ogg
{
    public class OggStreamingPlayer : IDisposable
    {
        private const int DefaultBufferMillis = 250;   // e.g. 250ms buffer
        private const int ReadBufferSize = 4096;       // bytes to read each chunk from file
        private const int OpusMaxFrameSize = 5760;     // 120ms @48kHz is 5760 samples/ch
        // (Opus can produce up to 120ms per frame; 960 samples = 20ms for typical usage.)

        private DynamicSoundEffectInstance _dynamicSound;
        private FileStream _fileStream;

        private bool _isValid = true;
        private bool _isPlaying;
        private bool _isLooped;
        private bool _isDisposed;
        private bool _stopRequested;
        private bool _pauseRequested;

        // Ogg & Opus structures
        private LibOgg.OggSyncState _oy;
        private LibOgg.OggStreamState _os;
        private bool _streamInitialized;

        private AudioInteropWrapper _audioWrapper = new AudioInteropWrapper();
        private IntPtr _opusDecoder = IntPtr.Zero;

        // We'll store these once we parse the OpusHead or deduce them
        private int _sampleRate = 48000;   // Opus commonly at 48k
        private int _channels = 1;        // Usually stereo, but can be mono

        // PCM short buffer reused for decode
        // Maximum theoretical frame size for Opus is 120ms at 48kHz * channels
        private short[] _decodeBuffer = new short[OpusMaxFrameSize * 1]; // up to stereo

        // Constructed similarly to WavStreamingPlayer
        public OggStreamingPlayer(string path)
        {
            try
            {
                // 1) Open file
                _fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);

                // 2) Initialize Ogg sync state
                LibOgg.ogg_sync_init(ref _oy);

                // 3) (Optional) We could parse the first pages to find an OpusHead packet,
                //    read sample rate, channels, etc. For simplicity, we assume 48k, stereo.
                //    If your file has different channels, you'd parse the first packet
                //    to detect actual channel count and sampling rate.

                // 4) Create Opus decoder with the deduce  sampleRate/channels
                _opusDecoder = _audioWrapper.CreateOpusDecoder(_sampleRate, _channels);

                // 5) Initialize DynamicSoundEffectInstance
                _dynamicSound = new DynamicSoundEffectInstance(
                    _sampleRate,
                    _channels == 2 ? AudioChannels.Stereo : AudioChannels.Mono
                );
                _dynamicSound.BufferNeeded += DynamicSound_BufferNeeded;

                // We won't actually decode anything until we call Play().
            }
            catch (Exception ex)
            {
                _isValid = false;
                _fileStream?.Dispose();
                _fileStream = null;
                // Optionally log or handle `ex`.
            }
        }

        // -----------------------------------------------------
        // Public properties, matching WavStreamingPlayer style
        // -----------------------------------------------------
        public bool IsValid => _isValid;
        public bool IsPlaying => _isPlaying && !_pauseRequested && !_stopRequested;
        public bool IsPaused => _pauseRequested;
        public bool IsStopped => _stopRequested || !_isPlaying;

        // -----------------------------------------------------
        // The core streaming logic
        // -----------------------------------------------------

        /// <summary>
        /// This event handler is called by DynamicSoundEffectInstance when it needs more PCM data.
        /// We'll read from the Ogg file, decode Opus, and submit buffers until we have enough queued.
        /// </summary>
        private void DynamicSound_BufferNeeded(object sender, EventArgs e)
        {
            if (!_isValid || _pauseRequested || _stopRequested || !_isPlaying)
                return;

            // We'll keep submitting buffers until we have at least 2 queued.
            while (_dynamicSound.PendingBufferCount < 2 &&
                   !_pauseRequested && !_stopRequested && _isPlaying)
            {
                // We'll decode ~DefaultBufferMillis worth of audio
                int bytesPerSec = _sampleRate * _channels * sizeof(short);
                int desiredBytes = (bytesPerSec * DefaultBufferMillis) / 1000;
                byte[] pcmBuffer = new byte[desiredBytes];

                int totalBytesDecoded = 0;

                // Keep decoding packets until we fill this chunk or reach EOF.
                while (totalBytesDecoded < pcmBuffer.Length)
                {
                    // Decode one Opus packet from the Ogg stream
                    int decodedBytes = DecodeNextOpusPacket(
                        pcmBuffer,
                        totalBytesDecoded,
                        pcmBuffer.Length - totalBytesDecoded
                    );

                    if (decodedBytes <= 0)
                    {
                        // End-of-file or stream.
                        if (_isLooped)
                        {
                            // If looped, we reset file & Ogg state, then continue
                            ResetOgg();
                            continue;
                        }
                        else
                        {
                            // No loop: we stop.
                            Stop();
                            return;
                        }
                    }
                    totalBytesDecoded += decodedBytes;
                }

                // Submit the filled buffer to DynamicSoundEffectInstance
                if (_isPlaying && !_pauseRequested && !_stopRequested)
                {
                    // totalBytesDecoded might be smaller if we hit EOF mid-chunk
                    _dynamicSound.SubmitBuffer(pcmBuffer, 0, totalBytesDecoded);
                }
            }
        }

        /// <summary>
        /// Decodes a single Opus packet from the Ogg bitstream into the given PCM buffer.
        /// Returns how many bytes of PCM were written, or 0 if EOF/end-of-stream.
        /// </summary>
        private int DecodeNextOpusPacket(byte[] dstBuffer, int dstOffset, int dstMaxCount)
        {
            // 1) Ensure there's a valid packet. If we don't have one, we read from file until we get one or EOF.
            LibOgg.OggPacket oggPacket = GetNextPacket();
            if (oggPacket.packet == IntPtr.Zero || oggPacket.bytes == 0)
            {
                // Means EOF or no more packets
                return 0;
            }

            // 2) Copy packet data into a managed byte array
            byte[] packetData = new byte[oggPacket.bytes];
            Marshal.Copy(oggPacket.packet, packetData, 0, oggPacket.bytes);

            // 3) Decode with Opus
            // We'll decode up to OpusMaxFrameSize samples per channel (short).
            // The actual number of samples returned might be < OpusMaxFrameSize.
            int frameSize = OpusMaxFrameSize; // maximum we can decode at once
            short[] decoded = _audioWrapper.DecodeOpus(_opusDecoder, packetData, packetData.Length, frameSize);

            // 'decoded' array has (frameSize * channels) capacity,
            // but Opus returns how many samples were actually decoded.
            // The actual count is the "result" from `opus_decode()`.
            // Because your wrapper always returns the full short[] (including unused samples),
            // you might detect real sample count if you extended your wrapper. 
            // For now, we assume the entire array is valid up to frameSize*samples.

            // 4) Convert short[] -> byte[] and copy to dstBuffer
            int numShorts = decoded.Length;
            int numBytes = numShorts * sizeof(short);
            if (numBytes > dstMaxCount)
            {
                numBytes = dstMaxCount; // clamp if needed
            }
            Buffer.BlockCopy(decoded, 0, dstBuffer, dstOffset, numBytes);

            return numBytes;
        }

        /// <summary>
        /// Retrieves the next Ogg packet from our Ogg bitstream, reading from file if necessary.
        /// Returns an OggPacket with packet==IntPtr.Zero if EOF or error.
        /// </summary>
        private LibOgg.OggPacket GetNextPacket()
        {
            // If we haven't inited the stream, do it with the first valid page we see.
            if (!_streamInitialized)
            {
                if (!InitOggStream())
                {
                    // Could not init => end or invalid file
                    return default;
                }
            }

            // Attempt to get next packet
            LibOgg.OggPacket packet = new LibOgg.OggPacket();
            while (true)
            {
                int result = LibOgg.ogg_stream_packetout(ref _os, ref packet);
                if (result == 1)
                {
                    // We got a packet
                    return packet;
                }
                else if (result == 0)
                {
                    // Need more data / or end of pages in the current stream
                    // Read next page from file
                    if (!ReadNextPage())
                    {
                        // EOF or error
                        return default;
                    }
                    // then loop around to try again
                }
                else
                {
                    // Negative => stream error
                    return default;
                }
            }
        }

        /// <summary>
        /// Reads the next Ogg page from disk (if available) and submits it to ogg_stream.
        /// Returns true if a page was read successfully, false if EOF or error.
        /// </summary>
        private bool ReadNextPage()
        {
            // 1) Try to extract a page from the sync buffer
            LibOgg.OggPage page = new LibOgg.OggPage();
            while (true)
            {
                int pageoutResult = LibOgg.ogg_sync_pageout(ref _oy, ref page);
                if (pageoutResult == 1)
                {
                    // We got a page => feed it to _os
                    LibOgg.ogg_stream_pagein(ref _os, ref page);
                    return true;
                }
                else if (pageoutResult == 0)
                {
                    // Need more data => read from file
                    if (!BufferFileData())
                    {
                        // EOF or error
                        return false;
                    }
                }
                else
                {
                    // Negative => stream error
                    return false;
                }
            }
        }

        /// <summary>
        /// Reads data from the file into the OggSyncState buffer. Returns false if EOF.
        /// </summary>
        private bool BufferFileData()
        {
            IntPtr bufferPtr = LibOgg.ogg_sync_buffer(ref _oy, ReadBufferSize);
            if (bufferPtr == IntPtr.Zero) return false;

            // Read from file
            byte[] localBuf = new byte[ReadBufferSize];
            int bytesRead = _fileStream.Read(localBuf, 0, ReadBufferSize);
            if (bytesRead == 0)
            {
                // EOF
                return false;
            }

            // Copy to the ogg buffer
            Marshal.Copy(localBuf, 0, bufferPtr, bytesRead);

            // Tell libogg how many bytes we wrote
            LibOgg.ogg_sync_wrote(ref _oy, bytesRead);
            return true;
        }

        /// <summary>
        /// Initializes the OggStreamState with the first valid page found in the file.
        /// </summary>
        private bool InitOggStream()
        {
            // We read until we get a valid page. Then we init the stream with its serialno.
            while (true)
            {
                LibOgg.OggPage page = new LibOgg.OggPage();
                int result = LibOgg.ogg_sync_pageout(ref _oy, ref page);
                if (result == 1)
                {
                    // We have a page => init OggStreamState
                    int serialno = LibOgg.ogg_page_serialno(ref page);
                    int ret = LibOgg.ogg_stream_init(out _os, serialno);
                    if (ret != 0) return false;

                    // Feed that page to the stream
                    LibOgg.ogg_stream_pagein(ref _os, ref page);
                    _streamInitialized = true;
                    return true;
                }
                else if (result == 0)
                {
                    // Need more data
                    if (!BufferFileData())
                        return false; // EOF
                }
                else
                {
                    // stream error
                    return false;
                }
            }
        }

        /// <summary>
        /// If looping is set, we reset the file and the Ogg/Opus state to the beginning.
        /// </summary>
        private void ResetOgg()
        {
            // Seek file to start
            _fileStream.Seek(0, SeekOrigin.Begin);
            // Clear existing Ogg structures
            LibOgg.ogg_stream_clear(ref _os);
            LibOgg.ogg_sync_clear(ref _oy);

            // Re-init Ogg sync
            _oy = new LibOgg.OggSyncState();
            LibOgg.ogg_sync_init(ref _oy);

            _streamInitialized = false;

            // Also reset the Opus decoder state by destroying/recreating if needed
            // Alternatively, we can do opus_decoder_ctl(decoder, OPUS_RESET_STATE), 
            // but for simplicity:
            _audioWrapper.DestroyOpusDecoder(_opusDecoder);
            _opusDecoder = _audioWrapper.CreateOpusDecoder(_sampleRate, _channels);
        }

        // -----------------------------------------------------
        // Playback Control (Play, Pause, Stop, Volume, Loop)
        // -----------------------------------------------------
        public void Play()
        {
            if (!_isValid || _isDisposed) return;

            if (!_isPlaying)
            {
                // If we had stopped, reset everything
                if (_stopRequested)
                {
                    _stopRequested = false;
                    ResetOgg();
                }

                _pauseRequested = false;
                _isPlaying = true;
                _dynamicSound.Play();
            }
            else if (_pauseRequested)
            {
                // Resume
                _pauseRequested = false;
                _dynamicSound.Resume();
            }
        }

        public void Pause()
        {
            if (!_isValid || _isDisposed) return;

            if (!_pauseRequested && _isPlaying)
            {
                _pauseRequested = true;
                _dynamicSound.Pause();
            }
        }

        public void Stop()
        {
            if (!_isValid || _isDisposed) return;

            if (!_stopRequested || _isPlaying)
            {
                _stopRequested = true;
                _pauseRequested = false;
                _isPlaying = false;
                _dynamicSound.Stop();

                // Reset so we're ready to play from the beginning next time
                ResetOgg();
            }
        }

        public void SetVolume(float value)
        {
            if (!_isValid || _isDisposed) return;
            _dynamicSound.Volume = Math.Clamp(value, 0f, 1f);
        }

        public void SetLoop(bool loop)
        {
            _isLooped = loop;
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                if (_dynamicSound != null)
                {
                    _dynamicSound.Dispose();
                }
                if (_fileStream != null)
                {
                    _fileStream.Dispose();
                }

                // Clean up Ogg/Opus
                if (_streamInitialized)
                {
                    LibOgg.ogg_stream_clear(ref _os);
                }
                LibOgg.ogg_sync_clear(ref _oy);

                if (_opusDecoder != IntPtr.Zero)
                {
                    _audioWrapper.DestroyOpusDecoder(_opusDecoder);
                    _opusDecoder = IntPtr.Zero;
                }
            }
        }
    }
}
