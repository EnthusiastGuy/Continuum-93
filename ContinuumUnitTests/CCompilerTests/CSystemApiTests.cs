using Continuum93.Emulator;
using static ContinuumUnitTests.CCompilerTests.CTestHarness;

namespace ContinuumUnitTests.CCompilerTests
{
    /// <summary>Compiler pragmas for system software and the continuum.h filesystem / program / text APIs.</summary>
    [Collection("CCompiler")]
    public class CSystemApiTests
    {
        [Fact]
        public void OriginAndStackPragmasPlaceTheProgram()
        {
            var r = Run("#pragma c93 origin 0x300000\n#pragma c93 stack 0x380000\nint g = 7;\nint main(void) { int local = 5; return (int)&local; }");
            Assert.True(r.Labels[".c_main"] > 0x300000 && r.Labels[".c_main"] < 0x310000);
            Assert.InRange((uint)r.ReturnValue, 0x370000u, 0x380000u);     // locals live just below the custom stack top
            Assert.Equal(7, r.GlobalInt("g"));
        }

        [Theory]
        [InlineData("#pragma c93 origin banana\nint main(void){return 0;}")]
        [InlineData("#pragma c93 stack 0x1000000\nint main(void){return 0;}")]
        public void InvalidPragmasAreReported(string src) => AssertError(src, "#pragma c93");

        [Fact]
        public void UnknownPragmasAreIgnored() => Assert.Equal(3, Eval("#pragma c93 optimize all\n#pragma pack(1)\nint main(void){return 3;}"));

        [Fact]
        public void FilesystemApi()
        {
            string dir = Path.Combine(Constants.FS_ROOT, "ctests", "fsapi");
            Directory.CreateDirectory(Path.Combine(dir, "sub"));
            File.WriteAllText(Path.Combine(dir, "hello.txt"), "Hello, Continuum!");
            File.Delete(Path.Combine(dir, "out.bin"));

            var r = Run(@"
#include <continuum.h>
#include <string.h>
char listing[512];
char loaded[64];
char saved[16];
int folders, total, exists_file, exists_dir, missing, size, missing_dir;
int main(void) {
    total = c93_list_dir(""ctests/fsapi"", listing, &folders);
    exists_file = c93_file_exists(""ctests/fsapi/hello.txt"");
    exists_dir = c93_dir_exists(""ctests/fsapi/sub"");
    missing = c93_file_exists(""ctests/fsapi/nope.txt"");
    missing_dir = c93_list_dir(""ctests/no-such-dir"", listing + 400, &folders);
    size = (int)c93_file_size(""ctests/fsapi/hello.txt"");
    c93_load_file(""ctests/fsapi/hello.txt"", loaded);
    c93_save_file(""ctests/fsapi/out.bin"", ""SAVED"", 5);
    return 0;
}");
            Assert.Equal(2, r.GlobalInt("total"));             // sub and hello.txt (out.bin is written afterwards)
            Assert.Equal(-1, r.GlobalInt("missing_dir"));
            Assert.Equal(1, r.GlobalInt("exists_file"));
            Assert.Equal(1, r.GlobalInt("exists_dir"));
            Assert.Equal(0, r.GlobalInt("missing"));
            Assert.Equal(17, r.GlobalInt("size"));
            Assert.Equal("Hello, Continuum!", r.GlobalString("loaded").Substring(0, 17));
            Assert.Equal("SAVED", File.ReadAllText(Path.Combine(dir, "out.bin")));
            string listing = System.Text.Encoding.ASCII.GetString(r.Read(r.AddressOf("listing"), 40));
            Assert.StartsWith("sub\0\0", listing);              // folders, empty name, then files
        }

        [Fact]
        public void CopyAndFillUseBlockInstructions() =>
            Assert.Equal(1, Eval(@"
#include <continuum.h>
unsigned char a[300], b[300];
int main(void) {
    c93_fill(a, 0xAB, 300);
    for (int i = 0; i < 300; i++) if (a[i] != 0xAB) return 100 + i;
    a[150] = 7;
    c93_copy(b, a, 300);
    c93_fill(a, 0, 0);                                  /* zero length is a no-op */
    return b[0] == 0xAB && b[150] == 7 && b[299] == 0xAB && a[0] == 0xAB;
}"));

        [Fact]
        public void FontTextWidthMatchesDrawnWidth() =>
            Assert.Equal(1, Eval(@"
#include <continuum.h>
unsigned char font[6000];
int main(void) {
    c93_set_video_pages(1);
    c93_load_font(""fonts/ModernDOS 8x8.png"", font);
    const char *s = ""Halcyon OS 1.0 - windows!"";
    int drawn = c93_text(0, font, 10, 10, 3, s, 480, 0, 0, 0);
    return drawn == c93_text_width_font(font, s) && c93_font_height(font) == 8;
}"));

        [Fact]
        public void BuildAndCallAnotherProgram()
        {
            // The callee lives at its own origin so it does not overwrite the caller
            string dir = Path.Combine(Constants.FS_ROOT, "ctests", "callee");
            Directory.CreateDirectory(dir);
            File.WriteAllText(Path.Combine(dir, "callee.c"),
                "#pragma c93 origin 0x300000\n#pragma c93 stack 0x380000\nint main(void) { *(int *)0x0F8000 = 4242; return 0; }\n");
            File.WriteAllText(Path.Combine(dir, "broken.c"), "int main(void) { return nope; }\n");

            var r = Run(@"
#include <continuum.h>
int addr, errors, bad_addr, bad_errors, result;
int main(void) {
    addr = c93_build(""ctests/callee/callee.c"", C93_BUILD_C, &errors);
    bad_addr = c93_build(""ctests/callee/broken.c"", C93_BUILD_C, &bad_errors);
    if (addr >= 0) c93_call((unsigned int)addr);       /* all registers are preserved around the call */
    result = *(int *)0x0F8000;
    return 0;
}", maxSteps: 50_000_000);
            Assert.Equal(0x300000, r.GlobalInt("addr"));
            Assert.Equal(0, r.GlobalInt("errors"));
            Assert.Equal(-1, r.GlobalInt("bad_addr"));
            Assert.True(r.GlobalInt("bad_errors") > 0);
            Assert.Equal(4242, r.GlobalInt("result"));
        }
    }
}
