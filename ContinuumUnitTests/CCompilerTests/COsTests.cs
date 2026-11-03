using Continuum93.Emulator;
using Continuum93.Emulator.Compilers.C93C;
using Continuum93.Emulator.Interpreter;
using static ContinuumUnitTests.CCompilerTests.CTestHarness;

namespace ContinuumUnitTests.CCompilerTests
{
    /// <summary>
    /// Tests for the operating system software written in C: the Halcyon desktop (run headless with scripted input)
    /// and the dual-boot selector.
    /// </summary>
    [Collection("CCompiler")]
    public class COsTests
    {
        private static readonly string OsDir = Path.GetFullPath(Path.Combine(Constants.FS_ROOT, "..", "os"));
        private static readonly string HalcyonDir = Path.Combine(OsDir, "halcyon");
        private static readonly string BootDir = Path.Combine(OsDir, "boot");

        private static Dictionary<string, string> HalcyonIncludes()
        {
            var files = new Dictionary<string, string>();
            foreach (string f in Directory.GetFiles(HalcyonDir))
                files[Path.GetFileName(f)] = File.ReadAllText(f);
            files["../boot/bootproto.h"] = File.ReadAllText(Path.Combine(BootDir, "bootproto.h"));
            return files;
        }

        private static CRunResult RunHalcyon(int scene, int frames) =>
            Run($"#define HAL_SCRIPT\n#define HAL_FRAMES {frames}\n#define HAL_SCENE {scene}\n" + File.ReadAllText(Path.Combine(HalcyonDir, "halcyon.c")),
                HalcyonIncludes(), maxSteps: 2_000_000_000, captureVideo: true);

        private static int Probe(CRunResult r, int i) => r.GlobalInt("test_probe", i);

        [Fact]
        public void HalcyonFitsTheSystemMemoryMap()
        {
            var compiler = new CCompiler { IncludeResolver = (name, _) => HalcyonIncludes().TryGetValue(name, out var s) ? s : null };
            string asm = compiler.Compile(File.ReadAllText(Path.Combine(HalcyonDir, "halcyon.c")), "halcyon.c");
            Assert.True(compiler.Errors == 0, compiler.Log);
            Assert.Equal(0x000000u, compiler.CodeOrigin);       // from #pragma c93 origin
            Assert.Equal(0x04F000u, compiler.StackTop);         // from #pragma c93 stack

            // (CompileLog is shared static state: not reset here, so parallel assembler tests are not disturbed)
            var a = new Assembler();
            a.Build(asm);
            Assert.Equal(0, a.Errors);
            uint imageEnd = a.Stats.Labels[CCodeGen.HeapStartLabel];
            Assert.True(imageEnd < 0x040000, $"Halcyon's image must stay below 0x040000 (stack and workspace follow), ends at 0x{imageEnd:X6}");
        }

        [Fact]
        public void DesktopBootsOpensWindowsSnapsAndFolds()
        {
            var r = RunHalcyon(1, 200);

            Assert.Equal(4, r.ReturnValue);                     // Drawers, Pulse, About and Calc are open
            Assert.True(Probe(r, 0) > 0, "Dragging the About window near Pulse should have snapped it to a guide");
            Assert.Equal(1, Probe(r, 1));                       // Calc was folded into its rail

            // Every layer has content: backdrop, desktop icons, windows and the top layer (menu bar, dock, pointer)
            for (int page = 0; page < 4; page++)
            {
                byte[] layer = r.VideoBuffer.Skip((3 - page) * 480 * 270).Take(480 * 270).ToArray();
                int distinct = layer.Distinct().Count();
                Assert.True(distinct > (page == 0 ? 60 : 8), $"Layer {page} looks empty ({distinct} colours)");
            }

            long perFrame = r.Steps / 237;                      // splash + 200 scripted frames
            Assert.True(perFrame < 400_000, $"Desktop frames are too expensive: {perFrame} instructions");
        }

        [Fact]
        public void AppsCalcAndSketchWork()
        {
            var r = RunHalcyon(2, 60);

            Assert.Equal(3, r.ReturnValue);                     // Sketch, Calc, Prefs
            Assert.Equal(60, r.GlobalInt("calc_value"));        // 2 0 * 3 =
            byte[] canvas = r.Read(0x06A000, 200 * 112);        // Sketch canvas in the workspace
            Assert.True(canvas.Count(b => b == 100 + 2) > 30, "Red strokes expected on the canvas");
            Assert.True(canvas.Count(b => b == 100 + 7) > 30, "Cyan strokes expected on the canvas");
            Assert.True(r.GlobalInt("menu_open") == 0, "The Halcyon menu should be open");
        }

        [Fact]
        public void ReaderLoadsSourceAndDrawersShowContextMenu()
        {
            var r = RunHalcyon(3, 30);
            Assert.Equal(1, Probe(r, 2));                       // right-click opened the popup menu
            Assert.True(Probe(r, 3) > 300, $"Reader should have split hal_wm.c into lines (got {Probe(r, 3)})");
        }

        [Fact]
        public void RunningAProgramGoesThroughTheBootMailbox()
        {
            var r = RunHalcyon(4, 60);

            Assert.Equal(0, r.ReturnValue);                     // Halcyon returned to the boot selector
            const uint mailbox = 0x07FD00;
            byte[] mb = r.Read(mailbox, 64);
            int Int(int o) => (mb[o] << 24) | (mb[o + 1] << 16) | (mb[o + 2] << 8) | mb[o + 3];
            Assert.Equal(0x43393342, Int(0));                   // BOOT_MAGIC
            Assert.Equal(1, Int(4));                            // BOOT_CMD_RUN
            Assert.Equal((int)r.Labels[CCodeGen.HeapStartLabel], Int(12));   // image end for the integrity check
            string path = System.Text.Encoding.ASCII.GetString(r.Read(mailbox + 24, 200)).Split('\0')[0];
            Assert.Equal("programs/neon-ricochet/neon-ricochet.c", path);
        }

        [Fact]
        public void BootSelectorCompilesAndDrawsTheMenu()
        {
            string src = File.ReadAllText(Path.Combine(BootDir, "boot.c"));
            var compiler = new CCompiler
            {
                IncludeResolver = (name, _) => name == "bootproto.h" ? File.ReadAllText(Path.Combine(BootDir, name)) : null
            };
            string asm = compiler.Compile("#define BOOT_TEST\n" + src, "boot.c");   // no host input, no countdown
            Assert.True(compiler.Errors == 0, compiler.Log);
            Assert.Equal(0x070000u, compiler.CodeOrigin);

            // (CompileLog is shared static state: not reset here, so parallel assembler tests are not disturbed)
            var a = new Assembler();
            a.Build(asm);
            Assert.Equal(0, a.Errors);
            Assert.True(a.Stats.Labels[CCodeGen.HeapStartLabel] < 0x07D000, "The selector and its stack must fit below the mailbox");

            // Run it for a while: headless, the clock does not advance, so it stays on the menu
            using var c = new Computer();
            c.Clear();
            foreach (var b in a.BlockManager.GetBlocks()) c.LoadMemAt(b.Start, b.Data);
            c.CPU.REGS.IPO = compiler.CodeOrigin;
            for (int i = 0; i < 6_000_000 && c.IsRunning; i++)
                c.ExecuteNextInstruction();

            Assert.True(c.IsRunning);
            byte[] frame = c.GRAPHICS.GetVideoBuffer().Take(480 * 270).ToArray();
            Assert.True(frame.Distinct().Count() > 20, "The boot menu should be drawn");
        }
    }
}
