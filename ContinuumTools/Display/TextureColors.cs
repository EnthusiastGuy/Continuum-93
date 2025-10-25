using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Last_Known_Reality.Reality
{
    /// <summary>
    /// Initializes and allows use of solid colors as Texture2D in order to be drawn arbitrarly.
    /// Every new color used is cached
    /// </summary>
    public static class TextureColors
    {
        private static Dictionary<Color, Texture2D> textureColors = new Dictionary<Color, Texture2D>();

        public static Color Transparent = Color.Transparent;

        public static Color Health = Color.GreenYellow;

        public static Color ConeTestGreen = new Color(128, 255, 128, 40);
        public static Color ConeTestRed = new Color(255, 255, 0, 40);

        public static Color Pink = new Color(255, 0, 255, 255);
        public static Color White = new Color(255, 255, 255, 255);
        public static Color Blue = Color.Blue;

        public static Color DialogBackground = new Color(0, 255, 255, 128);
        public static Color DialogBackgroundDark = new Color(0, 32, 32, 240);

        public static Color MenuBackground = new Color(0, 0, 0, 255);
        public static Color MapBackground = new Color(0, 25, 25, 230);
        public static Color MenuText = new Color(72, 162, 245);

        public static Color LassoColor = new Color(90, 40, 0, 100);
        public static Color LassoColorMedium = new Color(36, 16, 0, 80);
        public static Color LassoColorLight = new Color(16, 8, 0, 40);
        public static Color ZoomWindowCrosshair = new Color(8, 8, 8, 80);

        public static Color SelectedSpriteBackground = new Color(163, 95, 0, 200);
        public static Color SelectedSpriteBorder = new Color(247, 144, 0, 235);

        public static Color WindowBackground = new Color(0, 25, 25, 250);
        public static Color WindowBorder = new Color(0, 50, 50, 250);

        // Messages
        public static Color MessageInformation = new Color(0, 44, 61, 250);
        public static Color MessageConfirmation = new Color(0, 61, 34, 250);
        public static Color MessageWarning = new Color(61, 44, 0, 250);
        public static Color MessageError = new Color(61, 0, 0, 250);

        public static Color ItemsBackground = new Color(10, 10, 10, 240);

        // Editor
        public static Color EditorTileSheetBackground = new Color(19, 5, 36, 255);
        public static Color OuterSliceRectangle = new Color(37, 157, 204, 100);
        public static Color CollisionTileColor = new Color(40, 0, 0, 200);

        public static void InitializeTextureColors()
        {
            AddColorTexture(DialogBackground);
        }

        public static Texture2D GetTextureColor(Color color)
        {
            if (!textureColors.ContainsKey(color))
                AddColorTexture(color);

            return textureColors[color];
        }


        private static void AddColorTexture(Color color)
        {
            textureColors.Add(color, GetColorTexture(color));
        }

        private static Texture2D GetColorTexture(Color color)
        {
            Texture2D pixel = new Texture2D(Renderer.GetGraphics().GraphicsDevice, 1, 1);
            pixel.SetData(new[] { color });

            return pixel;
        }

        public static Color GetPulsatingRedBlue(Color color)
        {
            ColorAdvanced outputColor = ColorAdvanced.FromRGB(color.R, color.G, color.B);
            //outputColor.Luminosity = 10 + 80 * Metronome.GetUnitPulsar();
            Color response = outputColor.ToRGB();
            response.A = 128;

            return response;
        }

    }
}
