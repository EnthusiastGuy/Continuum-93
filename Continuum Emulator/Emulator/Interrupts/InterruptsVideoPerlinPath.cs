using Continuum93.Emulator;
using System;

namespace Continuum93.Emulator.Interrupts
{
    public static class InterruptsVideoPerlinPath
    {
        public static void DrawPerlinPath(byte regId, Computer computer)
        {
            // --- Read registers (mirrors your style) ---
            byte pageIdx = computer.CPU.REGS.GetNextRegister(regId, 1);
            byte page = computer.CPU.REGS.Get8BitRegister(pageIdx);

            byte sxIdx = computer.CPU.REGS.GetNextRegister(regId, 2);
            short startX = computer.CPU.REGS.Get16BitRegisterSigned(sxIdx);

            byte syIdx = computer.CPU.REGS.GetNextRegister(regId, 4);
            short startY = computer.CPU.REGS.Get16BitRegisterSigned(syIdx);

            byte exIdx = computer.CPU.REGS.GetNextRegister(regId, 6);
            short endX = computer.CPU.REGS.Get16BitRegisterSigned(exIdx);

            byte eyIdx = computer.CPU.REGS.GetNextRegister(regId, 8);
            short endY = computer.CPU.REGS.Get16BitRegisterSigned(eyIdx);

            byte colorIdx = computer.CPU.REGS.GetNextRegister(regId, 10);
            byte color = computer.CPU.REGS.Get8BitRegister(colorIdx);

            // Pattern 1
            byte p1SeedIdx = computer.CPU.REGS.GetNextRegister(regId, 12);
            ushort p1Seed = computer.CPU.REGS.Get16BitRegister(p1SeedIdx);

            byte p1MinIdx = computer.CPU.REGS.GetNextRegister(regId, 14);
            short p1MinY = computer.CPU.REGS.Get16BitRegisterSigned(p1MinIdx);

            byte p1MaxIdx = computer.CPU.REGS.GetNextRegister(regId, 16);
            short p1MaxY = computer.CPU.REGS.Get16BitRegisterSigned(p1MaxIdx);

            byte p1ZoomIdx = computer.CPU.REGS.GetNextRegister(regId, 18);
            byte p1Zoom = computer.CPU.REGS.Get8BitRegister(p1ZoomIdx);

            byte p1ShiftIdx = computer.CPU.REGS.GetNextRegister(regId, 19);
            short p1Shift = computer.CPU.REGS.Get16BitRegisterSigned(p1ShiftIdx);

            // Pattern 2 (optional)
            byte p2SeedIdx = computer.CPU.REGS.GetNextRegister(regId, 21);
            ushort p2Seed = computer.CPU.REGS.Get16BitRegister(p2SeedIdx);

            byte p2MinIdx = computer.CPU.REGS.GetNextRegister(regId, 23);
            short p2MinY = computer.CPU.REGS.Get16BitRegisterSigned(p2MinIdx);

            byte p2MaxIdx = computer.CPU.REGS.GetNextRegister(regId, 25);
            short p2MaxY = computer.CPU.REGS.Get16BitRegisterSigned(p2MaxIdx);

            byte p2ZoomIdx = computer.CPU.REGS.GetNextRegister(regId, 27);
            byte p2Zoom = computer.CPU.REGS.Get8BitRegister(p2ZoomIdx);

            byte p2ShiftIdx = computer.CPU.REGS.GetNextRegister(regId, 28);
            short p2Shift = computer.CPU.REGS.Get16BitRegisterSigned(p2ShiftIdx);

            // --- Setup ---
            uint videoAddr = computer.GRAPHICS.GetVideoPageAddress(page);
            int width = (int)Constants.V_WIDTH;
            int height = (int)Constants.V_HEIGHT;

            // Create a simple iterator along the dominant axis (DDA)
            int dx = endX - startX;
            int dy = endY - startY;
            int steps = Math.Max(Math.Abs(dx), Math.Abs(dy));
            if (steps <= 0) return;

            // Precompute zoom scales (byte 0 disables when used for p2)
            // Larger zoom value => larger features (i.e., lower frequency).
            // Scale formula: scale = max(1, zoom). You can swap in power-of-two if you prefer.
            float p1Scale = Math.Max(1, (int)p1Zoom);
            float p2Scale = Math.Max(1, (int)p2Zoom);

            // Draw the mountainous line
            for (int i = 0; i <= steps; i++)
            {
                float t = (float)i / steps;

                // Interpolate along the baseline
                float fx = startX + t * dx;
                float fy = startY + t * dy;

                // Use screen-space X to index the noise (stable wrt screen)
                int baseX = (int)MathF.Round(fx);

                // Pattern 1 offset
                float n1 = Noise1D((baseX + p1Shift) / p1Scale, p1Seed);
                float o1 = MapRange(n1, -1f, 1f, p1MinY, p1MaxY);

                // Pattern 2 offset (optional)
                float o2 = 0f;
                if (p2Zoom != 0)
                {
                    float n2 = Noise1D((baseX + p2Shift) / p2Scale, p2Seed);
                    o2 = MapRange(n2, -1f, 1f, p2MinY, p2MaxY);
                }

                int y = (int)MathF.Round(fy + o1 + o2);

                // Clamp to screen and plot
                if (baseX >= 0 && baseX < width && y >= 0 && y < height)
                {
                    uint addr = (uint)(videoAddr + y * width + baseX);
                    computer.MEMC.Set8bitToRAM(addr, color);
                }
            }

            // --- Helpers ---
            static float MapRange(float v, float inMin, float inMax, float outMin, float outMax)
            {
                float u = (v - inMin) / (inMax - inMin);      // 0..1
                return outMin + u * (outMax - outMin);
            }

            // Deterministic 1D Perlin noise with seed (no randomness)
            static float Noise1D(float x, ushort seed)
            {
                int xi0 = (int)MathF.Floor(x);
                int xi1 = xi0 + 1;
                float xf = x - xi0;

                float g0 = Grad1D(Hash(xi0, seed));
                float g1 = Grad1D(Hash(xi1, seed));

                float d0 = g0 * (xf);          // dot at left lattice
                float d1 = g1 * (xf - 1f);     // dot at right lattice

                float u = Fade(xf);
                return Lerp(d0, d1, u);        // in roughly [-1,1]
            }

            static int Hash(int x, ushort seed)
            {
                // 32-bit mix (xorshift-like), seeded; deterministic and fast
                unchecked
                {
                    uint h = (uint)x;
                    h ^= (uint)seed * 0x9E3779B1u; // golden ratio mix
                    h ^= h << 13;
                    h ^= h >> 17;
                    h ^= h << 5;
                    return (int)h;
                }
            }

            static float Grad1D(int h)
            {
                // Map hash to gradient in [-1,1]. Two simple options:
                // (1) sign-only gradients: return (h & 1) == 0 ? 1f : -1f;
                // (2) richer gradient set (used here):
                unchecked
                {
                    // 15-bit fraction -> [-1,1]
                    int v = (h >> 8) & 0x7FFF;
                    return (v / 16383.5f) * 2f - 1f;
                }
            }

            static float Fade(float t)
            {
                // Perlin's 6t^5 - 15t^4 + 10t^3
                return t * t * t * (t * (t * 6f - 15f) + 10f);
            }

            static float Lerp(float a, float b, float t) => a + (b - a) * t;
        }

    }
}
