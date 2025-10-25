using System;

namespace Continuum93.Tools
{
    public static class CMath
    {
        const float THREE_HALFS = 1.5f;

        public static float InverseSqrt(float number)
        {
            return 1.0f / MathF.Sqrt(number);
        }

        public static unsafe float QInverseSqrt(float number)
        {
            float x2 = 0.5f * number;
            uint i = *(uint*)&number;

            i = 0x5f3759df - (i >> 1);
            float y = *(float*)&i;

            y *= THREE_HALFS - (x2 * y * y);
            //y *= THREE_HALFS - (x2 * y * y);
            return y;
        }

    }
}
