using Continuum93.Emulator.GraphicsAccelerators;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace BenchmarkTests
{
    [Collection("SerialTests")]
    public class GFXImageProcessingTests
    {
        [Fact(Skip = "Skipping this test for now until it will be transformed to a proper performance test.")]
        public void ImageProcessingTest1()
        {
            TestGameProcessImageAndPalette game = new()
            {
                ImagePath = @"Data\images\Cristal-Kingdom-Dizzy.png"
            };
            Thread gameThread = new(() =>
            {
                game.Run();
            });

            gameThread.Start();

            while (!game.TestFinished)
            {
                Thread.Sleep(1);
            }

            Assert.Equal(30, game.ColorPalette.Length);
            Assert.Equal(24320, game.PixelData.Length);
            Assert.True(game.ElapsedMs > 0);
        }

        [Fact(Skip = "Skipping this test for now until it will be transformed to a proper performance test.")]
        public void ImageProcessingTest2()
        {
            TestGameProcessImageAndPalette game = new()
            {
                ImagePath = @"Data\images\Seymour-at-the-movies-map.png"
            };
            Thread gameThread = new(() =>
            {
                game.Run();
            });

            gameThread.Start();

            while (!game.TestFinished)
            {
                Thread.Sleep(1);
            }

            Assert.Equal(18, game.ColorPalette.Length);
            Assert.Equal(5376000, game.PixelData.Length);
            Assert.True(game.ElapsedMs > 0);
        }

        [Fact(Skip = "Skipping this test for now until it will be transformed to a proper performance test.")]
        public void TestProcessPalette()
        {
            Color[] palette = {
                new Color(50, 150, 250), new Color(5, 10, 15), Color.Aqua, Color.Ivory,
                Color.Navy, Color.Aquamarine, Color.MintCream, Color.Orange,
                Color.Orchid, Color.Magenta, Color.PaleGoldenrod, Color.LightYellow,
                Color.OrangeRed, Color.PaleGoldenrod, Color.PaleGreen, Color.Olive};

            Stopwatch sWatch = new();
            sWatch.Start();

            byte[] actual = GFXImageProcessing.ProcessPaletteArray(palette);

            sWatch.Stop();

            long elapsedTicks = sWatch.ElapsedTicks;

            Assert.Equal(50, actual[0]);
            Assert.Equal(150, actual[1]);
            Assert.Equal(250, actual[2]);
            Assert.Equal(5, actual[3]);
            Assert.Equal(10, actual[4]);
            Assert.Equal(15, actual[5]);
            Assert.Equal(3 * 16, actual.Length);
            Assert.True(elapsedTicks > 0);

        }

        /**
         * Tests for consistency of palette structure across different types of calls.
         */
        [Fact(Skip = "Skipping this test for now until it will be transformed to a proper performance test.")]
        public void ImageProcessingConsistentPalettes()
        {
            TestGamePalettes game = new()
            {
                ImagePath = @"Data\images\Cristal-Kingdom-Dizzy.png"
            };
            Thread gameThread = new(() =>
            {
                game.Run();
            });

            gameThread.Start();

            while (!game.TestFinished)
            {
                Thread.Sleep(1);
            }

            Assert.Equal(30, game.ColorPalette1.Length);
            Assert.Equal(30, game.ColorPalette2.Length);
            Assert.True(AreColorArraysEqual(game.ColorPalette1, game.ColorPalette2));
            Assert.True(game.ElapsedMs > 0);
        }


        // Utilities
        private static bool AreColorArraysEqual(Color[] expected, Color[] actual)
        {
            if (expected.Length != actual.Length)
            {
                return false;
            }

            for (int i = 0; i < expected.Length; i++)
            {
                if (expected[i] != actual[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
