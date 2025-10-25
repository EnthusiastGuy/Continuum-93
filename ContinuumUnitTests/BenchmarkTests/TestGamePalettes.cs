using Continuum93.Emulator.GraphicsAccelerators;
using Continuum93.Emulator.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace BenchmarkTests
{
    public class TestGamePalettes : Game
    {
        public GraphicsDeviceManager? GDM { get; set; }
        public GraphicsDevice? GDevice { get; set; }

        public volatile bool TestFinished = false;
        public long ElapsedTcks;
        public double ElapsedMs;

        public volatile string? ImagePath;

        public volatile Color[] ColorPalette1 = Array.Empty<Color>();
        public volatile Color[] ColorPalette2 = Array.Empty<Color>();

        public TestGamePalettes()
        {
            GDM = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            GDevice = GDM?.GraphicsDevice;

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            Stopwatch sWatch = new();
            sWatch.Start();
            GFXImageProcessing.ProcessImageAndPalette(ImagePath, GDevice);
            sWatch.Stop();

            ColorPalette1 = ImageLoadState.ColorPalette;

            GFXImageProcessing.ProcessPalette(ImagePath, GDevice);
            ColorPalette2 = ImageLoadState.ColorPalette;

            ElapsedTcks = sWatch.ElapsedTicks;
            ElapsedMs = ElapsedTcks / TimeSpan.TicksPerMillisecond;

            TestFinished = true;

            base.Update(gameTime);
        }
    }
}