namespace Last_Known_Reality.Reality
{
    using Microsoft.Xna.Framework;
    using System;

    // http://colorizer.org/
    // https://www.rapidtables.com/convert/color/rgb-to-hsl.html
    public class ColorAdvanced
    {
        public Hue Hue;
        public float Saturation;
        public float Luminosity;

        public byte Red;
        public byte Green;
        public byte Blue;
        public byte Alpha;


        public ColorAdvanced(float hue, float sat, float lum)
        {
            Hue = hue;
            Saturation = sat;
            Luminosity = lum;

            Color rgb = ToRGB();
            Red = rgb.R;
            Green = rgb.G;
            Blue = rgb.B;
            Alpha = 255;
        }

        public ColorAdvanced GetLighter()
        {
            return new ColorAdvanced(Hue.Value - 10, Saturation, Luminosity + 10);
        }

        public ColorAdvanced GetDarker()
        {
            return new ColorAdvanced(Hue.Value + 10, Saturation, Luminosity - 10);
        }

        public static ColorAdvanced FromRGB(Color color)
        {
            return FromRGB(color.R, color.G, color.B);
        }

        public static ColorAdvanced FromRGB(Byte R, Byte G, Byte B)
        {
            return FromRGB(R / 255f, G / 255f, B / 255f);
        }

        public static ColorAdvanced FromRGB(float r, float g, float b)
        {
            float min = Math.Min(Math.Min(r, g), b);
            float max = Math.Max(Math.Max(r, g), b);
            float delta = max - min;

            float H = 0;
            float S = 0;
            float L = (max + min) / 2.0f;

            if (Math.Abs(delta) >= 0.00001)
            {
                S = delta / (1 - Math.Abs(2 * L - 1));

                float rDelta = (g - b) / delta;
                float gDelta = (b - r) / delta;
                float bDelta = (r - g) / delta;

                if (r == max) H = rDelta % 6;
                else if (g == max) H = 2 + gDelta;
                else H = 4 + bDelta;

                H *= 60;
            }

            return new ColorAdvanced(H, S * 100, L * 100);
        }

        public Color ToRGB()
        {
            Color color = new Color();

            float l = Luminosity / 100;
            float s = Saturation / 100;
            float c = (1 - Math.Abs(2 * l - 1)) * s;

            float x = c * (1 - Math.Abs((Hue.Value / 60) % 2 - 1));

            float m = l - c / 2;
            float sel = Hue.Value / 60;

            float pR = 0;
            float pG = 0;
            float pB = 0;

            if (sel < 1)
            {
                pR = c;
                pG = x;
                pB = 0;
            }
            else if (sel < 2)
            {
                pR = x;
                pG = c;
                pB = 0;
            }
            else if (sel < 3)
            {
                pR = 0;
                pG = c;
                pB = x;
            }
            else if (sel < 4)
            {
                pR = 0;
                pG = x;
                pB = c;
            }
            else if (sel < 5)
            {
                pR = x;
                pG = 0;
                pB = c;
            }
            else if (sel < 6)
            {
                pR = c;
                pG = 0;
                pB = x;
            }

            color.R = (byte)((pR + m) * 255);
            color.G = (byte)((pG + m) * 255);
            color.B = (byte)((pB + m) * 255);
            color.A = 255;

            return color;
        }
    }
}
