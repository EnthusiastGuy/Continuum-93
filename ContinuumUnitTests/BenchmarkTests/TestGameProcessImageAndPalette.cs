using Continuum93.Emulator.GraphicsAccelerators;
using Continuum93.Emulator.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace BenchmarkTests
{
    public class TestGameProcessImageAndPalette : Game
    {
        public GraphicsDeviceManager? GDM { get; set; }
        public GraphicsDevice? GDevice { get; set; }

        public volatile bool TestFinished = false;
        public long ElapsedTcks;
        public double ElapsedMs;

        public volatile string? ImagePath;

        public volatile Color[] ColorPalette = Array.Empty<Color>();
        public volatile byte[] PixelData = Array.Empty<byte>();

        public TestGameProcessImageAndPalette()
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

            ColorPalette = ImageLoadState.ColorPalette;
            PixelData = ImageLoadState.ImageData;

            ElapsedTcks = sWatch.ElapsedTicks;
            ElapsedMs = ElapsedTcks / TimeSpan.TicksPerMillisecond;

            TestFinished = true;

            base.Update(gameTime);
        }
    }
}
