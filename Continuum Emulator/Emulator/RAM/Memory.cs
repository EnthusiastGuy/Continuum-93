using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Continuum93.Emulator;

namespace Continuum93.Emulator.RAM
{
    public class Memory
    {
        private volatile byte[] _data;

        public Memory(uint size)
        {
            _data = new byte[size];
        }

        public byte this[uint addr]
        {
            get => _data[addr];
            set => _data[addr] = value;
        }

        public byte this[long addr]
        {
            get => _data[addr];
            set => _data[addr] = value;
        }

        public byte[] Data => _data;

        public uint Size => (uint)_data.Length;

        public void Clear()
        {
            Array.Clear(_data, 0, _data.Length);
        }

        public void Fill(byte value, uint start, uint count)
        {
            Array.Fill(_data, value, (int)start, (int)count);
        }

        public void Copy(uint sourceAddress, uint destinationAddress, uint length)
        {
            Array.Copy(_data, sourceAddress, _data, destinationAddress, length);
        }

        public void FillRect(byte value, uint start, uint width, uint height)
        {
            for (int y = 0; y < height; y++)
            {
                Array.Fill(_data, value, (int)(start + y * Constants.V_WIDTH), (int)width);
            }
        }

        public uint Find(int address, byte value)
        {
            return (uint)Array.IndexOf(_data, value, address);
        }

        public uint FindPattern(int address, byte[] pattern)
        {
            return GetSequenceIndex(_data, pattern, address);
        }

        public uint GetSequenceIndex(byte[] buffer, byte[] pattern, int startIndex)
        {
            int i = Array.IndexOf(buffer, pattern[0], startIndex);
            while (i >= 0 && i <= buffer.Length - pattern.Length)
            {
                byte[] segment = new byte[pattern.Length];
                Buffer.BlockCopy(buffer, i, segment, 0, pattern.Length);
                if (segment.SequenceEqual(pattern))
                    return (uint)i;

                i = Array.IndexOf(buffer, pattern[0], i + 1);
            }
            return 0xFFFFFF;
        }

        public byte[] GetMemoryAt(uint address, int length)
        {
            byte[] response = new byte[length];
            Array.Copy(_data, address, response, 0, length);

            return response;
        }

        #region Bitwise GET (Big-Endian)

        /// <summary>
        /// Reads 'bits' bits from bitAddress in big-endian (bit #0 = LSB in each byte),
        /// returns as a byte (0..8 bits).
        /// </summary>
        public byte Get8BitValueFromBitMemoryAt(uint bitAddress, byte bits)
        {
            if (bits == 0 || bits > 8)
                bits = 8;

            // Which byte do we start at?
            uint byteAddress = bitAddress >> 3;

            // Offset from the leftmost bit (MSB) in that byte.
            // bit #0 is the *rightmost* (LSB) in that byte for big-endian.
            int bitInByte = (int)(bitAddress & 7);
            int offset = bitInByte;

            // Total bits we need to extract (offset + bits). 
            int totalBits = offset + bits;
            int numBytes = (totalBits + 7) >> 3; // ceiling division by 8

            if (byteAddress + numBytes > _data.Length)
                throw new IndexOutOfRangeException("Attempt to read beyond memory size.");

            // Read these bytes in "big-endian style" into raw.
            ulong raw = 0;
            for (int i = 0; i < numBytes; i++)
            {
                raw = (raw << 8) | _data[byteAddress + i];
            }

            // Now we have numBytes * 8 bits in raw. 
            // We only care about the lowest 'totalBits' bits, so shift off the top.
            int shift = (numBytes * 8) - totalBits;
            raw >>= shift;

            // At this point, the bottom 'bits' bits of raw are the value we want.
            ulong mask = ((ulong)1 << bits) - 1;
            ulong value = raw & mask;

            return (byte)value;
        }

        /// <summary>
        /// Reads 'bits' bits (0..16) from bitAddress in big-endian, returns as ushort.
        /// </summary>
        public ushort Get16BitValueFromBitMemoryAt(uint bitAddress, byte bits)
        {
            if (bits == 0 || bits > 16)
                bits = 16;

            uint byteAddress = bitAddress >> 3;
            int bitInByte = (int)(bitAddress & 7);
            int offset = bitInByte;

            int totalBits = offset + bits;
            int numBytes = (totalBits + 7) >> 3;

            if (byteAddress + numBytes > _data.Length)
                throw new IndexOutOfRangeException("Attempt to read beyond memory size.");

            ulong raw = 0;
            for (int i = 0; i < numBytes; i++)
            {
                raw = (raw << 8) | _data[byteAddress + i];
            }

            int shift = (numBytes * 8) - totalBits;
            raw >>= shift;

            ulong mask = ((ulong)1 << bits) - 1;
            ulong value = raw & mask;

            return (ushort)value;
        }

        /// <summary>
        /// Reads 'bits' bits (0..24) from bitAddress in big-endian, returns as 24-bit in a uint.
        /// </summary>
        public uint Get24BitValueFromBitMemoryAt(uint bitAddress, byte bits)
        {
            if (bits == 0 || bits > 24)
                bits = 24;

            uint byteAddress = bitAddress >> 3;
            int bitInByte = (int)(bitAddress & 7);
            int offset = bitInByte;

            int totalBits = offset + bits;
            int numBytes = (totalBits + 7) >> 3;

            if (byteAddress + numBytes > _data.Length)
                throw new IndexOutOfRangeException("Attempt to read beyond memory size.");

            ulong raw = 0;
            for (int i = 0; i < numBytes; i++)
            {
                raw = (raw << 8) | _data[byteAddress + i];
            }

            int shift = (numBytes * 8) - totalBits;
            raw >>= shift;

            ulong mask = ((ulong)1 << bits) - 1;
            ulong value = raw & mask;

            return (uint)value;
        }

        /// <summary>
        /// Reads 'bits' bits (0..32) from bitAddress in big-endian, returns as 32-bit in a uint.
        /// </summary>
        public uint Get32BitValueFromBitMemoryAt(uint bitAddress, byte bits)
        {
            if (bits == 0 || bits > 32)
                bits = 32;

            uint byteAddress = bitAddress >> 3;
            int bitInByte = (int)(bitAddress & 7);
            int offset = bitInByte;

            int totalBits = offset + bits;
            int numBytes = (totalBits + 7) >> 3;

            if (byteAddress + numBytes > _data.Length)
                throw new IndexOutOfRangeException("Attempt to read beyond memory size.");

            ulong raw = 0;
            for (int i = 0; i < numBytes; i++)
            {
                raw = (raw << 8) | _data[byteAddress + i];
            }

            int shift = (numBytes * 8) - totalBits;
            raw >>= shift;

            ulong mask = ((ulong)1 << bits) - 1;
            ulong value = raw & mask;

            return (uint)value;
        }

        #endregion

        #region Bitwise SET (Big-Endian)

        /// <summary>
        /// Sets 'bits' bits from a byte value into the bit-addressed memory, big-endian.
        /// </summary>
        public void Set8BitValueToBitMemoryAt(byte value, uint bitAddress, byte bits)
        {
            if (bits == 0 || bits > 8)
                bits = 8;

            uint byteAddress = bitAddress >> 3;
            int bitInByte = (int)(bitAddress & 7);
            int offset = bitInByte;

            int totalBits = offset + bits;
            int numBytes = (totalBits + 7) >> 3;

            if (byteAddress + numBytes > _data.Length)
                throw new IndexOutOfRangeException("Attempt to write beyond memory size.");

            ulong oldRaw = 0;
            for (int i = 0; i < numBytes; i++)
            {
                oldRaw = (oldRaw << 8) | _data[byteAddress + i];
            }

            int shift = (numBytes * 8) - totalBits;
            oldRaw >>= shift;

            int localOffset = totalBits - offset - bits;

            ulong mask = ((ulong)1 << bits) - 1;
            ulong truncatedVal = (ulong)value & mask;

            ulong oldVal = oldRaw & ~(mask << localOffset);
            oldVal |= (truncatedVal << localOffset);

            oldVal <<= shift;

            for (int i = numBytes - 1; i >= 0; i--)
            {
                _data[byteAddress + i] = (byte)(oldVal & 0xFF);
                oldVal >>= 8;
            }
        }

        /// <summary>
        /// Sets 'bits' bits from a 16-bit value into the bit-addressed memory, big-endian.
        /// </summary>
        public void Set16BitValueToBitMemoryAt(ushort value, uint bitAddress, byte bits)
        {
            if (bits == 0 || bits > 16)
                bits = 16;

            uint byteAddress = bitAddress >> 3;
            int bitInByte = (int)(bitAddress & 7);
            int offset = bitInByte;

            int totalBits = offset + bits;
            int numBytes = (totalBits + 7) >> 3;

            if (byteAddress + numBytes > _data.Length)
                throw new IndexOutOfRangeException("Attempt to write beyond memory size.");

            ulong oldRaw = 0;
            for (int i = 0; i < numBytes; i++)
            {
                oldRaw = (oldRaw << 8) | _data[byteAddress + i];
            }

            int shift = (numBytes * 8) - totalBits;
            oldRaw >>= shift;

            int localOffset = totalBits - offset - bits;

            ulong mask = ((ulong)1 << bits) - 1;
            ulong truncatedVal = (ulong)value & mask;

            ulong oldVal = oldRaw & ~(mask << localOffset);
            oldVal |= (truncatedVal << localOffset);

            oldVal <<= shift;

            for (int i = numBytes - 1; i >= 0; i--)
            {
                _data[byteAddress + i] = (byte)(oldVal & 0xFF);
                oldVal >>= 8;
            }
        }

        /// <summary>
        /// Sets 'bits' bits (up to 24) from a uint into the bit-addressed memory, big-endian.
        /// </summary>
        public void Set24BitValueToBitMemoryAt(uint value, uint bitAddress, byte bits)
        {
            if (bits == 0 || bits > 24)
                bits = 24;

            uint byteAddress = bitAddress >> 3;
            int bitInByte = (int)(bitAddress & 7);
            int offset = bitInByte;

            int totalBits = offset + bits;
            int numBytes = (totalBits + 7) >> 3;

            if (byteAddress + numBytes > _data.Length)
                throw new IndexOutOfRangeException("Attempt to write beyond memory size.");

            ulong oldRaw = 0;
            for (int i = 0; i < numBytes; i++)
            {
                oldRaw = (oldRaw << 8) | _data[byteAddress + i];
            }

            int shift = (numBytes * 8) - totalBits;
            oldRaw >>= shift;

            int localOffset = totalBits - offset - bits;

            ulong mask = ((ulong)1 << bits) - 1;
            ulong truncatedVal = (ulong)value & mask;

            ulong oldVal = oldRaw & ~(mask << localOffset);
            oldVal |= (truncatedVal << localOffset);

            oldVal <<= shift;

            for (int i = numBytes - 1; i >= 0; i--)
            {
                _data[byteAddress + i] = (byte)(oldVal & 0xFF);
                oldVal >>= 8;
            }
        }

        /// <summary>
        /// Sets 'bits' bits (up to 32) from a uint into the bit-addressed memory, big-endian.
        /// </summary>
        public void Set32BitValueToBitMemoryAt(uint value, uint bitAddress, byte bits)
        {
            if (bits == 0 || bits > 32)
                bits = 32;

            uint byteAddress = bitAddress >> 3;
            int bitInByte = (int)(bitAddress & 7);
            int offset = bitInByte;

            int totalBits = offset + bits;
            int numBytes = (totalBits + 7) >> 3;

            if (byteAddress + numBytes > _data.Length)
                throw new IndexOutOfRangeException("Attempt to write beyond memory size.");

            ulong oldRaw = 0;
            for (int i = 0; i < numBytes; i++)
            {
                oldRaw = (oldRaw << 8) | _data[byteAddress + i];
            }

            int shift = (numBytes * 8) - totalBits;
            oldRaw >>= shift;

            int localOffset = totalBits - offset - bits;

            ulong mask = ((ulong)1 << bits) - 1;
            ulong truncatedVal = (ulong)value & mask;

            ulong oldVal = oldRaw & ~(mask << localOffset);
            oldVal |= (truncatedVal << localOffset);

            oldVal <<= shift;

            for (int i = numBytes - 1; i >= 0; i--)
            {
                _data[byteAddress + i] = (byte)(oldVal & 0xFF);
                oldVal >>= 8;
            }
        }

        #endregion

        public unsafe void SetBytesToRAM(uint address, uint value, byte count)
        {
            // Pin the underlying array once
            fixed (byte* basePtr = _data)
            {
                byte* dest = basePtr + address;

                // 1) zero out all `count` bytes
                // This compiles to a very fast memset under the hood.
                Unsafe.InitBlockUnaligned(dest, 0, count);

                // 2) determine how many of the low-order bytes of `value` to copy
                byte toCopy = Math.Min(count, (byte)4);

                // 3) point at the rightmost `toCopy` bytes of `dest`
                byte* writePtr = dest + (count - toCopy);

                // 4) unroll the up-to-4-byte copy, big-endian
                switch (toCopy)
                {
                    case 4:
                        writePtr[0] = (byte)(value >> 24);
                        goto case 3;
                    case 3:
                        writePtr[toCopy - 3] = (byte)(value >> 16);
                        goto case 2;
                    case 2:
                        writePtr[toCopy - 2] = (byte)(value >> 8);
                        goto case 1;
                    case 1:
                        writePtr[toCopy - 1] = (byte)value;
                        break;
                    case 0:
                        // nothing to copy
                        break;
                }
            }
        }

        /// <summary>
        /// Write `repeat` blocks of `count` bytes each at `address`, big-endian,
        /// zero-padded on the left out to `count` bytes from the low 32-bit `value`.
        /// Runs in O(log repeat) copies via doubling.
        /// </summary>
        public unsafe void SetBytesToRAMRepeated(uint address, uint value, byte count, uint repeat)
        {
            // bounds check
            uint totalBytes = (uint)count * repeat;
            if (address > Size || totalBytes > Size - address)
                throw new IndexOutOfRangeException("Write beyond memory size.");

            fixed (byte* pBase = _data)
            {
                byte* dest = pBase + address;

                // 1) zero everything in one vectorized memset
                Unsafe.InitBlockUnaligned(dest, 0, totalBytes);

                // 2) figure out how many of the low-order bytes of 'value' to copy per block
                byte toCopy = Math.Min(count, (byte)4);

                // 3) write the first block at dest (big-endian, unrolled)
                byte* blockStart = dest + (count - toCopy);
                switch (toCopy)
                {
                    case 4:
                        blockStart[0] = (byte)(value >> 24);
                        goto case 3;
                    case 3:
                        blockStart[toCopy - 3] = (byte)(value >> 16);
                        goto case 2;
                    case 2:
                        blockStart[toCopy - 2] = (byte)(value >> 8);
                        goto case 1;
                    case 1:
                        blockStart[toCopy - 1] = (byte)value;
                        break;
                    case 0:
                        // nothing to copy
                        break;
                }

                // 4) now replicate via doubling: copy [0..blocks*count) → next region
                uint blocksDone = 1;
                while (blocksDone < repeat)
                {
                    // copy up to blocksDone more blocks
                    uint copyBlocks = Math.Min(blocksDone, repeat - blocksDone);
                    uint copyBytes = copyBlocks * count;

                    Unsafe.CopyBlockUnaligned(
                        dest + blocksDone * count,  // target
                        dest,                       // source
                        copyBytes                   // how many bytes
                    );

                    blocksDone += copyBlocks;
                }
            }
        }
    }
}
