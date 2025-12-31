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

        public const byte ReadFlag = 0b0000_0001;
        public const byte WriteFlag = 0b0000_0010;

        private readonly byte[] _activity = new byte[PageCount];

        public bool Enabled { get; set; }

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

        public void ConsumeActivity(byte[] destination)
        {
            if (destination == null || destination.Length < _activity.Length)
                throw new ArgumentException("Destination buffer is too small.", nameof(destination));

            Buffer.BlockCopy(_activity, 0, destination, 0, _activity.Length);
            Array.Clear(_activity, 0, _activity.Length);
        }

        public void Clear() => Array.Clear(_activity, 0, _activity.Length);

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
        }
    }
}

