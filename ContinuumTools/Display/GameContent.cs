namespace Last_Known_Reality.Reality
{
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Media;
    using System.IO;

    public static class GameContent
    {
        private static ContentManager _content;

        public static void RegisterContentManager(ContentManager content)
        {
            content.RootDirectory = "Content";
            _content = new ContentManager(content.ServiceProvider, "content");
        }

        // Load all fonts, effects...
        public static void Load()
        {
            Fonts.RealityOneWide = new PngFont("RealityOneWide");
            //Fonts.RealityOneRegular = new PngFont("RealityOneRegular");
            //Fonts.RealityOneNarrow = new PngFont("RealityOneNarrow");
            //Fonts.RealityThinRegular = new PngFont("RealityThinRegular");
            //Fonts.RealityThinRegularShort = new PngFont("RealityThinRegularShort");
            Fonts.SparkleMedium = new PngFont("SparkleMedium");
            //Fonts.ThinArches = new PngFont("ThinArches");
            //Fonts.ThinArchesContour = new PngFont("ThinArchesContour");
            //Fonts.SlickAntsContour = new PngFont("SlickAntsContour");

            // Cooldown font
            //Fonts.RealityTiny = new PngFont("RealityTiny");
        }

        public static Texture2D LoadTexture(string path)
        {
            FileStream fileStream = new FileStream(path, FileMode.Open);
            return Texture2D.FromStream(Renderer.GetGraphics().GraphicsDevice, fileStream);
        }

        public static Song LoadSong(string path)
        {
            return _content.Load<Song>(path);
        }

        private static Effect LoadEffect(string effect)
        {
            return _content.Load<Effect>(effect);
        }

        private static SpriteFont LoadSpriteFont(string font)
        {
            return _content.Load<SpriteFont>("fonts/" + font);
        }

        public static void Unload()
        {
            _content.Unload();
        }

        public static class Fonts
        {
            public static PngFont RealityOneWide;
            //public static PngFont RealityOneRegular;
            //public static PngFont RealityOneNarrow;
            //public static PngFont RealityThinRegular;
            //public static PngFont RealityThinRegularShort;
            public static PngFont SparkleMedium;
            //public static PngFont ThinArches;
            //public static PngFont ThinArchesContour;
            //public static PngFont SlickAntsContour;

            //public static PngFont RealityTiny;
        }

        public static class Effects
        {
            public static Effect FxBlackAndWhite;
            public static Effect TestEffect;
            public static Effect xBR;
        }

    }
}
