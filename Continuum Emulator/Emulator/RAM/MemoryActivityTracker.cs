using System;
using System.Runtime.CompilerServices;

namespace Continuum93.Emulator.RAM
{
    /// <summary>
    /// Lightweight read/write tracker for RAM pages. Writes are recorded as bitflags
    /// and consumed by the UI at a slower cadence (30–60 FPS) to avoid touching the
    /// CPU hot path more than necessary.
    /// </summary>
    public class MemoryActivityTracker
    {
        public const int PageShift = 8;           // 256 bytes per page
        public const int PageSize = 1 << PageShift;
        public const int PageCount = 0x1000000 / PageSize;
        private const int BitsPerWord = 64;
        public const int PageBitMaskWordCount = PageSize / BitsPerWord; // 256 bytes / 64-bit words
        public const int ByteMaskLength = PageCount * PageBitMaskWordCount;

        public const byte ReadFlag = 0b0000_0001;
        public const byte WriteFlag = 0b0000_0010;

        private readonly byte[] _activity = new byte[PageCount];
        private readonly ulong[] _readByteMask = new ulong[ByteMaskLength];
        private readonly ulong[] _writeByteMask = new ulong[ByteMaskLength];
        private uint _stepTag;

        public bool Enabled { get; set; }
        public uint StepTag => _stepTag;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RecordRead(uint address, int byteCount = 1)
        {
            if (!Enabled)
                return;

            MarkRange(address, byteCount, ReadFlag);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RecordWrite(uint address, int byteCount = 1)
        {
            if (!Enabled)
                return;

            MarkRange(address, byteCount, WriteFlag);
        }

        public void ConsumeActivity(byte[] pageDestination, ulong[] readByteDestination, ulong[] writeByteDestination)
        {
            if (pageDestination == null || pageDestination.Length < _activity.Length)
                throw new ArgumentException("Destination buffer is too small.", nameof(pageDestination));
            if (readByteDestination == null || readByteDestination.Length < _readByteMask.Length)
                throw new ArgumentException("Destination buffer is too small.", nameof(readByteDestination));
            if (writeByteDestination == null || writeByteDestination.Length < _writeByteMask.Length)
                throw new ArgumentException("Destination buffer is too small.", nameof(writeByteDestination));

            Buffer.BlockCopy(_activity, 0, pageDestination, 0, _activity.Length);
            Buffer.BlockCopy(_readByteMask, 0, readByteDestination, 0, _readByteMask.Length * sizeof(ulong));
            Buffer.BlockCopy(_writeByteMask, 0, writeByteDestination, 0, _writeByteMask.Length * sizeof(ulong));

            ClearInternal();
        }

        public void Clear() => ClearInternal();

        /// <summary>
        /// Clears activity and increments the step tag so the UI can reset highlights
        /// when advancing in step-by-step mode.
        /// </summary>
        public void ClearForNextStep()
        {
            ClearInternal();
            unchecked { _stepTag++; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void MarkRange(uint address, int byteCount, byte flag)
        {
            int startPage = (int)(address >> PageShift);
            int endPage = (int)((address + (uint)Math.Max(0, byteCount - 1)) >> PageShift);

            _activity[startPage] |= flag;
            if (endPage != startPage)
            {
                _activity[endPage] |= flag;
            }

            MarkBytes(address, byteCount, flag);
        }

        private void MarkBytes(uint address, int byteCount, byte flag)
        {
            int remaining = Math.Max(1, byteCount);
            uint current = address;

            while (remaining > 0)
            {
                int pageIndex = (int)(current >> PageShift);
                int offsetInPage = (int)(current & (PageSize - 1));
                int chunk = Math.Min(remaining, PageSize - offsetInPage);

                if ((flag & ReadFlag) != 0)
                    SetByteRange(_readByteMask, pageIndex, offsetInPage, chunk);

                if ((flag & WriteFlag) != 0)
                    SetByteRange(_writeByteMask, pageIndex, offsetInPage, chunk);

                current += (uint)chunk;
                remaining -= chunk;
            }
        }

        private void SetByteRange(ulong[] mask, int pageIndex, int offset, int length)
        {
            int end = offset + length;
            int wordBase = pageIndex * PageBitMaskWordCount;
            int cursor = offset;

            while (cursor < end)
            {
                int wordIndex = cursor >> 6;
                int bitIndex = cursor & 63;
                int span = Math.Min(64 - bitIndex, end - cursor);

                ulong bits = span == 64
                    ? ulong.MaxValue
                    : (((1UL << span) - 1UL) << bitIndex);

                mask[wordBase + wordIndex] |= bits;
                cursor += span;
            }
        }

        private void ClearInternal()
        {
            Array.Clear(_activity, 0, _activity.Length);
            Array.Clear(_readByteMask, 0, _readByteMask.Length);
            Array.Clear(_writeByteMask, 0, _writeByteMask.Length);
        }
    }
}


