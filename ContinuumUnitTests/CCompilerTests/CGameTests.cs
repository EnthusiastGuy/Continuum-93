using Continuum93.Emulator;
using static ContinuumUnitTests.CCompilerTests.CTestHarness;

namespace ContinuumUnitTests.CCompilerTests
{
    /// <summary>
    /// Runs the Neon Ricochet game (Data/filesystem/programs/neon-ricochet) headless on autopilot, and tests the
    /// continuum.h hardware wrappers it is built on.
    /// </summary>
    [Collection("CCompiler")]
    public class CGameTests
    {
        private static readonly string GamePath = Path.Combine(Constants.FS_ROOT, "programs", "neon-ricochet", "neon-ricochet.c");

        private static CRunResult RunGame(int frames, long maxSteps) =>
            Run($"#define C93_TEST_FRAMES {frames}\n#include \"neon-ricochet.c\"\n",
                new() { ["neon-ricochet.c"] = File.ReadAllText(GamePath) }, maxSteps, captureVideo: true);

        [Fact]
        public void GameCompilesFromTheFilesystem()
        {
            var compiler = new Continuum93.Emulator.Compilers.C93C.CCompiler();
            string asm = compiler.Compile(File.ReadAllText(GamePath), "neon-ricochet.c");
            Assert.True(compiler.Errors == 0, compiler.Log);
            Assert.Contains(".c_main", asm);
        }

        [Fact]
        public void AutopilotPlaysAndScores()
        {
            const int frames = 1200;   // 20 seconds of game time
            var r = RunGame(frames, 400_000_000);

            Assert.Equal(frames, r.GlobalInt("frames_run"));
            Assert.True(r.ReturnValue > 0, "The autopilot should have scored points");
            Assert.Equal(r.ReturnValue, r.GlobalInt("score"));
            int bricksLeft = r.GlobalInt("bricks_left");
            Assert.True(bricksLeft < 72, $"Some of the 72 bricks of level 1 should be destroyed (left: {bricksLeft})");

            // Budget check: the game must comfortably fit a 60 FPS frame on a slow (~20 MHz) Continuum
            long perFrame = r.Steps / frames;
            Assert.True(perFrame < 330_000, $"Frame cost too high: {perFrame} instructions per frame");
        }

        [Fact]
        public void PresentedFrameIsColorful()
        {
            var r = RunGame(90, 100_000_000);

            // Page 0 of the presented frame: count distinct palette indexes and verify the palette was set
            byte[] frame = r.VideoBuffer.Take(480 * 270).ToArray();
            int distinct = frame.Distinct().Count();
            Assert.True(distinct >= 40, $"Expected a rich frame, found {distinct} distinct colors");

            // HUD bar is drawn in COL_BLACK (1) and the palette entry of color 1 matches setup_palette
            Assert.Equal(1, frame[5 * 480 + 300]);
            Assert.Equal(new byte[] { 6, 3, 16 }, r.PaletteBuffer.Skip(3).Take(3).ToArray());
        }

        [Fact]
        public void ContinuumHeaderDrawsIntoVideoMemory()
        {
            var r = Run(@"
#include <continuum.h>
int main(void) {
    c93_set_video_pages(1);
    c93_set_buffer_mode(0);
    c93_clear(0, 3);
    c93_fill_rect(0, 10, 20, 5, 4, 7);
    c93_plot(0, 100, 100, 9);
    c93_line(0, 0, 200, 9, 200, 11);
    c93_set_color(0, 7, 1, 2, 3);
    c93_draw_text(0, 300, 50, 2, 12, ""1"");
    unsigned char *vram = c93_video_page(0);
    c93_present(1);
    return vram[20 * 480 + 10] * 1000 + vram[100 * 480 + 100];
}", captureVideo: true);

            Assert.Equal(7 * 1000 + 9, r.ReturnValue);
            byte[] f = r.VideoBuffer;
            Assert.Equal(3, f[0]);                        // cleared
            Assert.Equal(7, f[23 * 480 + 14]);            // inside the 5x4 rectangle
            Assert.Equal(3, f[24 * 480 + 14]);            // just below it
            Assert.Equal(11, f[200 * 480 + 5]);           // on the line
            Assert.Equal(new byte[] { 1, 2, 3 }, r.PaletteBuffer.Skip(7 * 3).Take(3).ToArray());
            // '1' glyph (5x7) at scale 2: row 0 has only the middle column set (x = 304..305, y = 50..51)
            Assert.Equal(12, f[50 * 480 + 304]);
            Assert.Equal(3, f[50 * 480 + 300]);
        }

        [Fact]
        public void FilledRectangleClipsAtEveryEdge()
        {
            // Regression: the DrawFilledRectangle interrupt wrote past the bottom of the last video page
            // (out of RAM -> machine crash) and over-ran rows for negative x.
            var r = Run(@"
#include <continuum.h>
int main(void) {
    c93_set_video_pages(1);
    c93_clear(0, 1);
    c93_fill_rect(0, 470, 260, 50, 50, 2);     /* bottom-right corner, mostly offscreen */
    c93_fill_rect(0, -20, 100, 500, 1, 3);     /* wider than the screen, negative x */
    c93_fill_rect(0, 10, -5, 4, 8, 4);         /* top edge */
    c93_fill_rect(0, 100, 300, 10, 10, 5);     /* entirely below the screen */
    c93_present(1);
    return 0;
}", captureVideo: true);

            byte[] f = r.VideoBuffer;
            Assert.Equal(2, f[269 * 480 + 479]);
            Assert.Equal(1, f[259 * 480 + 479]);
            Assert.Equal(3, f[100 * 480 + 0]);
            Assert.Equal(3, f[100 * 480 + 479]);
            Assert.Equal(1, f[101 * 480 + 0]);         // did not spill into the next row
            Assert.Equal(4, f[2 * 480 + 12]);
            Assert.Equal(1, f[3 * 480 + 12]);
        }

        [Fact]
        public void ContinuumHeaderKeyStateDecoding() =>
            Assert.Equal(1, Eval(@"
#include <continuum.h>
int main(void) {
    unsigned char state[32] = { 0 };
    state[37 / 8] = 0x80 >> (37 % 8);     /* LEFT pressed */
    state[3] = 0x80;                      /* key 24 */
    return c93_key_down(state, C93_KEY_LEFT) && !c93_key_down(state, C93_KEY_RIGHT) && c93_key_down(state, 24)
        && c93_text_width(""ABC"", 2) == 34;
}"));
    }
}
