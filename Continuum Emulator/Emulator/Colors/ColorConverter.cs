using System;

namespace Continuum93.Emulator.Colors
{
    public static class ColorConverter
    {
        /// <summary>
        /// Converts a 24-bit RGB value into a 32-bit HSL value.
        /// The HSL value is packed as:
        /// - First 16 bits: Hue (0-360 degrees)
        /// - Next 8 bits: Saturation (0-100%, scaled as 0-100)
        /// - Last 8 bits: Lightness (0-100%, scaled as 0-100)
        /// 
        /// The function extracts the Red, Green, and Blue channels from the 24-bit RGB input, 
        /// normalizes them to the [0, 1] range, and calculates the corresponding HSL values.
        /// These values are packed into a 32-bit unsigned integer and returned.
        ///
        /// Input: RGB value packed as 0xRRGGBB.
        /// Output: HSL value packed as 0xHHHHSSLL.
        /// </summary>
        /// <param name="rgbValue">The 24-bit RGB color value (0xRRGGBB).</param>
        /// <returns>The 32-bit HSL color value (0xHHHHSSLL).</returns>
        public static uint RGBToHSL(uint rgbValue)
        {
            // Extract R, G, B from the 32-bit uint (rgbValue is in the form 0xRRGGBB)
            byte r = (byte)((rgbValue >> 16) & 0xFF);
            byte g = (byte)((rgbValue >> 8) & 0xFF);
            byte b = (byte)(rgbValue & 0xFF);

            // Normalize R, G, B to the range [0, 1]
            float rf = r / 255f;
            float gf = g / 255f;
            float bf = b / 255f;

            // Calculate max and min RGB values
            float max = Math.Max(rf, Math.Max(gf, bf));
            float min = Math.Min(rf, Math.Min(gf, bf));

            // Lightness is the average of the max and min values
            float lightness = (max + min) / 2f;

            float hue = 0f;
            float saturation = 0f;

            // If max == min, it's a grayscale color (no saturation, hue is undefined)
            if (max != min)
            {
                // Calculate Saturation
                saturation = lightness > 0.5f
                    ? (max - min) / (2f - max - min)
                    : (max - min) / (max + min);

                // Calculate Hue
                if (max == rf)
                {
                    hue = (gf - bf) / (max - min);
                }
                else if (max == gf)
                {
                    hue = 2f + (bf - rf) / (max - min);
                }
                else
                {
                    hue = 4f + (rf - gf) / (max - min);
                }

                hue *= 60f; // Convert hue to degrees
                if (hue < 0)
                {
                    hue += 360f;
                }
            }

            // Convert values to integer form
            ushort h = (ushort)hue;   // No scaling of hue, only 0-360
            byte s = (byte)(saturation * 100);         // Scale saturation to 0-100%
            byte l = (byte)(lightness * 100);          // Scale lightness to 0-100%

            // Pack the HSL values into a 32-bit uint: 0xHHHHSSLL
            uint hslValue = (uint)(h << 16) | (uint)(s << 8) | l;

            return hslValue;
        }

        /// <summary>
        /// Converts a 32-bit HSL value into a 24-bit RGB value.
        /// The HSL value is unpacked as:
        /// - First 16 bits: Hue (0-360 degrees)
        /// - Next 8 bits: Saturation (0-100%, scaled as 0-100)
        /// - Last 8 bits: Lightness (0-100%, scaled as 0-100)
        /// 
        /// The function uses the extracted Hue, Saturation, and Lightness values to calculate 
        /// the corresponding Red, Green, and Blue channels. The RGB values are scaled to the 
        /// [0, 255] range and packed into a 24-bit unsigned integer.
        ///
        /// Input: HSL value packed as 0xHHHHSSLL.
        /// Output: RGB value packed as 0xRRGGBB.
        /// </summary>
        /// <param name="hslValue">The 32-bit HSL color value (0xHHHHSSLL).</param>
        /// <returns>The 24-bit RGB color value (0xRRGGBB).</returns>

        public static uint HSLToRGB(uint hslValue)
        {
            // Extract Hue, Saturation, and Lightness from the 32-bit uint (0xHHHHSSLL)
            ushort h = (ushort)((hslValue >> 16) & 0xFFFF);  // Extract Hue (16-bit, 0-360)
            byte s = (byte)((hslValue >> 8) & 0xFF);         // Extract Saturation (percentage 0-100%)
            byte l = (byte)(hslValue & 0xFF);                // Extract Lightness (percentage 0-100%)

            // Normalize the values to [0,1] range
            float hue = h;               // Hue remains in degrees [0, 360]
            float saturation = s / 100f; // Scale saturation from [0, 100] to [0, 1]
            float lightness = l / 100f;  // Scale lightness from [0, 100] to [0, 1]

            float r, g, b;

            // If saturation is 0, it's a grayscale color (no hue)
            if (saturation == 0)
            {
                r = g = b = lightness; // Grayscale
            }
            else
            {
                float q = lightness < 0.5f
                    ? lightness * (1f + saturation)
                    : lightness + saturation - (lightness * saturation);
                float p = 2f * lightness - q;

                // Convert hue into RGB, where hue is in [0, 360]
                float hk = hue / 360f;  // Scale hue to [0, 1] for processing
                float[] t = new float[3];
                t[0] = hk + 1f / 3f;  // Offset for red
                t[1] = hk;            // Offset for green
                t[2] = hk - 1f / 3f;  // Offset for blue

                // Helper function to convert hue segment into an RGB component
                float HueToRGB(float t)
                {
                    if (t < 0) t += 1;
                    if (t > 1) t -= 1;
                    if (t < 1f / 6f) return p + (q - p) * 6f * t;
                    if (t < 1f / 2f) return q;
                    if (t < 2f / 3f) return p + (q - p) * (2f / 3f - t) * 6f;
                    return p;
                }

                r = HueToRGB(t[0]);
                g = HueToRGB(t[1]);
                b = HueToRGB(t[2]);
            }

            // Convert to byte values (0-255)
            byte red = (byte)(r * 255);
            byte green = (byte)(g * 255);
            byte blue = (byte)(b * 255);

            // Pack the RGB values into a 24-bit uint: 0xRRGGBB
            uint rgbValue = (uint)(red << 16) | (uint)(green << 8) | blue;

            return rgbValue;
        }

        /// <summary>
        /// Converts a 24-bit RGB value into a 32-bit HSB value.
        /// The HSB value is packed as:
        /// - First 16 bits: Hue (0-360 degrees)
        /// - Next 8 bits: Saturation (0-100%, scaled as 0-100)
        /// - Last 8 bits: Brightness (0-100%, scaled as 0-100)
        /// 
        /// The function extracts the Red, Green, and Blue channels from the 24-bit RGB input,
        /// normalizes them to the [0, 1] range, and calculates the corresponding HSB values.
        /// These values are packed into a 32-bit unsigned integer and returned.
        ///
        /// Input: RGB value packed as 0xRRGGBB.
        /// Output: HSB value packed as 0xHHHHSSBB.
        /// </summary>
        /// <param name="rgbValue">The 24-bit RGB color value (0xRRGGBB).</param>
        /// <returns>The 32-bit HSB color value (0xHHHHSSBB).</returns>
        public static uint RGBToHSB(uint rgbValue)
        {
            // Extract R, G, B from the 32-bit uint (rgbValue is in the form 0xRRGGBB)
            byte r = (byte)((rgbValue >> 16) & 0xFF);
            byte g = (byte)((rgbValue >> 8) & 0xFF);
            byte b = (byte)(rgbValue & 0xFF);

            // Normalize R, G, B to the range [0, 1]
            float rf = r / 255f;
            float gf = g / 255f;
            float bf = b / 255f;

            // Calculate max and min RGB values
            float max = Math.Max(rf, Math.Max(gf, bf));
            float min = Math.Min(rf, Math.Min(gf, bf));

            float brightness = max;  // Brightness is the max of the RGB values
            float saturation = (max == 0f) ? 0f : (max - min) / max;  // Saturation calculation

            float hue = 0f;

            // Calculate Hue
            if (max != min)
            {
                if (max == rf)
                {
                    hue = (gf - bf) / (max - min);
                }
                else if (max == gf)
                {
                    hue = 2f + (bf - rf) / (max - min);
                }
                else
                {
                    hue = 4f + (rf - gf) / (max - min);
                }

                hue *= 60f; // Convert hue to degrees
                if (hue < 0)
                {
                    hue += 360f;
                }
            }

            // Convert values to integer form
            ushort h = (ushort)hue;               // Hue is 0-360
            byte s = (byte)(saturation * 100);     // Saturation is 0-100%
            byte bVal = (byte)(brightness * 100);  // Brightness is 0-100%

            // Pack the HSB values into a 32-bit uint: 0xHHHHSSBB
            uint hsbValue = (uint)(h << 16) | (uint)(s << 8) | bVal;

            return hsbValue;
        }

        /// <summary>
        /// Converts a 32-bit HSB value into a 24-bit RGB value.
        /// The HSB value is unpacked as:
        /// - First 16 bits: Hue (0-360 degrees)
        /// - Next 8 bits: Saturation (0-100%, scaled as 0-100)
        /// - Last 8 bits: Brightness (0-100%, scaled as 0-100)
        /// 
        /// The function uses the extracted Hue, Saturation, and Brightness values to calculate 
        /// the corresponding Red, Green, and Blue channels. The RGB values are scaled to the 
        /// [0, 255] range and packed into a 24-bit unsigned integer.
        ///
        /// Input: HSB value packed as 0xHHHHSSBB.
        /// Output: RGB value packed as 0xRRGGBB.
        /// </summary>
        /// <param name="hsbValue">The 32-bit HSB color value (0xHHHHSSBB).</param>
        /// <returns>The 24-bit RGB color value (0xRRGGBB).</returns>
        public static uint HSBToRGB(uint hsbValue)
        {
            // Extract Hue, Saturation, and Brightness from the 32-bit uint (0xHHHHSSBB)
            ushort h = (ushort)((hsbValue >> 16) & 0xFFFF);  // Extract Hue (16-bit, 0-360)
            byte s = (byte)((hsbValue >> 8) & 0xFF);         // Extract Saturation (percentage 0-100%)
            byte b = (byte)(hsbValue & 0xFF);                // Extract Brightness (percentage 0-100%)

            // Normalize the values to [0,1] range
            float hue = h;               // Hue remains in degrees [0, 360]
            float saturation = s / 100f; // Scale saturation from [0, 100] to [0, 1]
            float brightness = b / 100f; // Scale brightness from [0, 100] to [0, 1]

            float r, g, blue;

            // If saturation is 0, it's a grayscale color (no hue)
            if (saturation == 0)
            {
                r = g = blue = brightness; // Grayscale
            }
            else
            {
                float sector = hue / 60f;           // Sector of the color wheel (0-5)
                int sectorIndex = (int)Math.Floor(sector);
                float fractional = sector - sectorIndex;

                float p = brightness * (1f - saturation);
                float q = brightness * (1f - saturation * fractional);
                float t = brightness * (1f - saturation * (1f - fractional));

                switch (sectorIndex)
                {
                    case 0:
                        r = brightness;
                        g = t;
                        blue = p;
                        break;
                    case 1:
                        r = q;
                        g = brightness;
                        blue = p;
                        break;
                    case 2:
                        r = p;
                        g = brightness;
                        blue = t;
                        break;
                    case 3:
                        r = p;
                        g = q;
                        blue = brightness;
                        break;
                    case 4:
                        r = t;
                        g = p;
                        blue = brightness;
                        break;
                    default:
                        r = brightness;
                        g = p;
                        blue = q;
                        break;
                }
            }

            // Convert to byte values (0-255)
            byte red = (byte)(r * 255);
            byte green = (byte)(g * 255);
            byte bVal = (byte)(blue * 255);

            // Pack the RGB values into a 24-bit uint: 0xRRGGBB
            uint rgbValue = (uint)(red << 16) | (uint)(green << 8) | bVal;

            return rgbValue;
        }


    }
}
