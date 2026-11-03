namespace Continuum93.Emulator.Compilers.C93C
{
    /// <summary>
    /// Built-in headers and the C runtime library. The library is plain C compiled together with the user's
    /// program; only functions reachable from main are emitted, and user definitions replace library ones.
    /// </summary>
    public static class CRuntime
    {
        public static string GetHeader(string name) => name switch
        {
            "stddef.h" => StdDef,
            "stdint.h" => StdInt,
            "stdbool.h" => StdBool,
            "limits.h" => Limits,
            "string.h" => StringH,
            "stdlib.h" => StdLib,
            "ctype.h" => CType,
            "continuum.h" => Continuum,
            _ => null
        };

        /// <summary>
        /// Hardware access for Continuum 93: video (INT 0x01), input (INT 0x02) and clock (INT 0x00).
        /// The wrappers read their arguments straight from the stack frame: argument i lives at (QRS + 4*i),
        /// so its low byte is at (QRS + 4*i + 3) and its low 16 bits at (QRS + 4*i + 2).
        /// Functions returning a value leave it in ABCD from the inline assembly.
        /// </summary>
        private const string Continuum = @"
#ifndef __C93_CONTINUUM_H
#define __C93_CONTINUUM_H

#define C93_SCREEN_WIDTH 480
#define C93_SCREEN_HEIGHT 270

/* Key codes (letters and digits use their upper case ASCII codes, e.g. 'A', '7') */
#define C93_KEY_BACKSPACE 8
#define C93_KEY_TAB 9
#define C93_KEY_ENTER 13
#define C93_KEY_SHIFT 16
#define C93_KEY_CONTROL 17
#define C93_KEY_ESCAPE 27
#define C93_KEY_SPACE 32
#define C93_KEY_LEFT 37
#define C93_KEY_UP 38
#define C93_KEY_RIGHT 39
#define C93_KEY_DOWN 40

/* ---- Video. Pages (layers) are 0..7; colors are palette indexes, 0 is transparent. ---- */

/* Sets how many video pages are rendered (1 - 8). Resets palettes. */
static void c93_set_video_pages(int count)
{
    asm(""LD B, (QRS + 3)\nLD A, 0x02\nINT 0x01, A"");
}

/* Bit i = 1: page is presented automatically; 0: the program presents frames with c93_present(). */
static void c93_set_buffer_mode(int auto_mask)
{
    asm(""LD B, (QRS + 3)\nLD A, 0x33\nINT 0x01, A"");
}

/* Copies the given pages (bit mask) to the display. Use with manual buffer mode for tear-free frames. */
static void c93_present(int page_mask)
{
    asm(""LD A, (QRS + 3)\nVDL A"");
}

static void c93_clear(int page, int color)
{
    asm(""LD B, (QRS + 3)\nLD C, (QRS + 7)\nLD A, 0x05\nINT 0x01, A"");
}

static void c93_fill_rect(int page, int x, int y, int w, int h, int color)
{
    asm(""LD B, (QRS + 3)\nLD CD, (QRS + 6)\nLD EF, (QRS + 10)\nLD GH, (QRS + 14)\nLD IJ, (QRS + 18)\nLD K, (QRS + 23)\nLD A, 0x06\nINT 0x01, A"");
}

static void c93_rect(int page, int x, int y, int w, int h, int color)
{
    asm(""LD B, (QRS + 3)\nLD CD, (QRS + 6)\nLD EF, (QRS + 10)\nLD GH, (QRS + 14)\nLD IJ, (QRS + 18)\nLD K, (QRS + 23)\nLD A, 0x07\nINT 0x01, A"");
}

static void c93_fill_round_rect(int page, int x, int y, int w, int h, int color, int roundness)
{
    asm(""LD B, (QRS + 3)\nLD CD, (QRS + 6)\nLD EF, (QRS + 10)\nLD GH, (QRS + 14)\nLD IJ, (QRS + 18)\nLD K, (QRS + 23)\nLD L, (QRS + 27)\nLD A, 0x08\nINT 0x01, A"");
}

static void c93_round_rect(int page, int x, int y, int w, int h, int color, int roundness)
{
    asm(""LD B, (QRS + 3)\nLD CD, (QRS + 6)\nLD EF, (QRS + 10)\nLD GH, (QRS + 14)\nLD IJ, (QRS + 18)\nLD K, (QRS + 23)\nLD L, (QRS + 27)\nLD A, 0x09\nINT 0x01, A"");
}

static void c93_plot(int page, int x, int y, int color)
{
    asm(""LD B, (QRS + 3)\nLD CD, (QRS + 6)\nLD EF, (QRS + 10)\nLD G, (QRS + 15)\nLD A, 0x20\nINT 0x01, A"");
}

static void c93_line(int page, int x1, int y1, int x2, int y2, int color)
{
    asm(""LD B, (QRS + 3)\nLD CD, (QRS + 6)\nLD EF, (QRS + 10)\nLD GH, (QRS + 14)\nLD IJ, (QRS + 18)\nLD K, (QRS + 23)\nLD A, 0x21\nINT 0x01, A"");
}

static void c93_ellipse(int page, int x, int y, int rx, int ry, int color)
{
    asm(""LD B, (QRS + 3)\nLD CD, (QRS + 6)\nLD EF, (QRS + 10)\nLD GH, (QRS + 14)\nLD IJ, (QRS + 18)\nLD K, (QRS + 23)\nLD A, 0x22\nINT 0x01, A"");
}

static void c93_fill_ellipse(int page, int x, int y, int rx, int ry, int color)
{
    asm(""LD B, (QRS + 3)\nLD CD, (QRS + 6)\nLD EF, (QRS + 10)\nLD GH, (QRS + 14)\nLD IJ, (QRS + 18)\nLD K, (QRS + 23)\nLD A, 0x23\nINT 0x01, A"");
}

/* Address of a page's 256-entry palette (3 bytes per color: R, G, B). */
static unsigned char *c93_palette(int page)
{
    asm(""LD B, (QRS + 3)\nLD A, 0x04\nINT 0x01, A\nLD A, 0"");
}

/* Address of a page's pixels (one byte per pixel, 480 x 270, row by row). */
static unsigned char *c93_video_page(int page)
{
    asm(""LD B, (QRS + 3)\nLD A, 0x03\nINT 0x01, A\nLD A, 0"");
}

static void c93_set_color(int page, int index, int r, int g, int b)
{
    unsigned char *p = c93_palette(page) + index * 3;
    p[0] = (unsigned char)r;
    p[1] = (unsigned char)g;
    p[2] = (unsigned char)b;
}

/* ---- Input ---- */

/* Fills a 32-byte buffer with the state of all 256 keys (see c93_key_down). */
static void c93_read_keys(unsigned char *state)
{
    asm(""LD BCD, (QRS + 1)\nLD A, 0x00\nINT 0x02, A"");
}

static int c93_key_down(const unsigned char *state, int key)
{
    return (state[(key >> 3) & 31] >> (7 - (key & 7))) & 1;
}

/* ---- Time ---- */

static unsigned int __c93_clock_ms;

/* Milliseconds since the machine started. */
static unsigned int c93_millis(void)
{
    asm(""LD A, 0x03\nLD B, 0x00\nLD CDE, .c___c93_clock_ms\nINT 0x00, A"");
    return __c93_clock_ms;
}

static void c93_wait_ms(unsigned int ms)
{
    unsigned int start = c93_millis();
    while (c93_millis() - start < ms)
        ;
}

/* Shows or hides video pages: bit i = page i visible. */
static void c93_set_layers_visible(int mask)
{
    asm(""LD B, (QRS + 3)\nLD A, 0x31\nINT 0x01, A"");
}

/* Fast block copy / fill using the CPU's MEMC / MEMF instructions (n up to 16 MB). */
static void c93_copy(void *dst, const void *src, unsigned int n)
{
    if (n) asm(""LD EFG, (QRS + 1)\nLD BCD, (QRS + 5)\nLD HIJ, (QRS + 9)\nMEMC BCD, EFG, HIJ"");
}

static void c93_fill(void *dst, int value, unsigned int n)
{
    if (n) asm(""LD BCD, (QRS + 1)\nLD EFG, (QRS + 9)\nLD H, (QRS + 7)\nMEMF BCD, EFG, H"");
}

/* Copies a w x h block of pixels (one byte each, row by row) from memory to a page; color 0 is transparent
   except on page 0. */
static void c93_draw_sprite(const void *pixels, int page, int x, int y, int w, int h)
{
    asm(""LD BCD, (QRS + 1)\nLD E, (QRS + 7)\nLD FG, (QRS + 10)\nLD HI, (QRS + 14)\nLD JK, (QRS + 18)\nLD LM, (QRS + 22)\nLD A, 0x10\nINT 0x01, A"");
}

/* Shows (1) or hides (0) the host's mouse cursor, for programs that draw their own pointer. */
static void c93_set_host_cursor(int visible)
{
    asm(""LD B, (QRS + 3)\nLD A, 0x11\nINT 0x00, A"");
}

/* ---- Mouse ---- */

static unsigned short __c93_mx, __c93_my;
static unsigned char __c93_mb;

/* Reads the mouse position in screen pixels; returns the buttons (bit 0 left, bit 1 right, bit 2 middle). */
static int c93_read_mouse(int *x, int *y)
{
    asm(""LD A, 0x03\nINT 0x02, A\nLD (.c___c93_mx), BC\nLD (.c___c93_my), DE\nLD (.c___c93_mb), F"");
    *x = (short)__c93_mx;
    *y = (short)__c93_my;
    return __c93_mb;
}

/* ---- Filesystem (paths are relative to the filesystem root, e.g. ""programs/demo/demo.c"") ---- */

static unsigned char __c93_r24a[3], __c93_r24b[3], __c93_r8;
static unsigned int __c93_r32;

static unsigned int __c93_u24(const unsigned char *b) { return ((unsigned int)b[0] << 16) | ((unsigned int)b[1] << 8) | b[2]; }

static int c93_file_exists(const char *path)
{
    asm(""LD BCD, (QRS + 1)\nLD A, 0x02\nINT 0x04, A\nLD (.c___c93_r8), A"");
    return __c93_r8 != 0;
}

static int c93_dir_exists(const char *path)
{
    asm(""LD BCD, (QRS + 1)\nLD A, 0x03\nINT 0x04, A\nLD (.c___c93_r8), A"");
    return __c93_r8 != 0;
}

/* Size in bytes of an existing file. */
static unsigned int c93_file_size(const char *path)
{
    asm(""LD BCD, (QRS + 1)\nLD A, 0x20\nINT 0x04, A\nLD (.c___c93_r32), BCDE"");
    return __c93_r32;
}

/* Loads a whole file into memory at dest. */
static void c93_load_file(const char *path, void *dest)
{
    asm(""LD BCD, (QRS + 1)\nLD EFG, (QRS + 5)\nLD A, 0x07\nINT 0x04, A"");
}

/* Saves len bytes from src into a file. */
static void c93_save_file(const char *path, const void *src, unsigned int len)
{
    asm(""LD BCD, (QRS + 1)\nLD EFG, (QRS + 5)\nLD HIJ, (QRS + 9)\nLD A, 0x06\nINT 0x04, A"");
}

/* Lists a directory into buffer as NUL-terminated names: all folders, an empty name, all files, an empty name.
   Returns the number of entries or -1 if the directory does not exist; *folders receives the folder count. */
static int c93_list_dir(const char *path, char *buffer, int *folders)
{
    asm(""LD BCD, (QRS + 1)\nLD EFG, (QRS + 5)\nLD A, 0x15\nINT 0x04, A\nLD (.c___c93_r24a), BCD\nLD (.c___c93_r24b), EFG"");
    unsigned int total = __c93_u24(__c93_r24a);
    if (total == 0xFFFFFF)
        return -1;
    *folders = (int)__c93_u24(__c93_r24b);
    return (int)total;
}

/* ---- Fonts: .font files drawn natively by the video hardware ---- */

/* Text flags for c93_text */
#define C93_TEXT_MONOSPACE 1
#define C93_TEXT_CENTER    8
#define C93_TEXT_WRAP      16
#define C93_TEXT_OUTLINE   32

/* Loads a PNG font (a 16 x 6 grid of glyphs, e.g. ""fonts/ModernDOS 8x8.png"") into dest for c93_text.
   Returns the size in bytes of the converted font. */
static unsigned int c93_load_font(const char *path, void *dest)
{
    asm(""LD BCD, (QRS + 1)\nLD EFG, (QRS + 5)\nLD A, 0x40\nINT 0x04, A\nLD (.c___c93_r24a), ABC"");
    return __c93_u24(__c93_r24a);
}

static unsigned short __c93_tw;

/* Draws text with a loaded .font; returns the drawn width. outline_pattern selects the 8 neighbor directions
   (0xFF = full outline) used when C93_TEXT_OUTLINE is set. */
static int c93_text(int page, const void *font, int x, int y, int color, const char *text, int max_width,
                    int flags, int outline_color, int outline_pattern)
{
    /* DrawText needs registers up to R, which overlaps the frame pointer: save it and read every argument first */
    asm(""PUSH QRS\n""
        ""LD BCD, (QRS + 5)\nLD EFG, (QRS + 21)\nLD HI, (QRS + 10)\nLD JK, (QRS + 14)\nLD L, (QRS + 19)\n""
        ""LD M, (QRS + 3)\nLD NO, (QRS + 26)\nLD P, (QRS + 31)\nLD X, (QRS + 35)\nLD Y, (QRS + 39)\n""
        ""LD Q, X\nLD R, Y\nLD A, 0x14\nINT 0x01, A\nPOP QRS\nLD (.c___c93_tw), AB"");
    return __c93_tw;
}

/* Width in pixels of text in a loaded font, measured like c93_text reports it (each glyph plus 1 pixel spacing). */
static int c93_text_width_font(const unsigned char *font, const char *text)
{
    int w = 0;
    for (; *text; text++)
    {
        int c = (unsigned char)*text;
        if (c >= 32 && c <= 127)
            w += font[6 + c - 32] + 1;
    }
    return w;
}

static int c93_font_height(const unsigned char *font)
{
    return font[2];
}

/* ---- Programs ---- */

#define C93_BUILD_ASM   0xC0
#define C93_BUILD_BASIC 0xC1
#define C93_BUILD_C     0xC2

static unsigned char __c93_errors;

/* Builds (assembles/compiles) a program file into memory. Returns its entry address, or -1 on failure;
   *errors receives the number of errors. kind is C93_BUILD_ASM, C93_BUILD_BASIC or C93_BUILD_C. */
static int c93_build(const char *path, int kind, int *errors)
{
    asm(""LD BCD, (QRS + 1)\nLD A, (QRS + 7)\nINT 0x00, A\nLD (.c___c93_r24a), BCD\nLD (.c___c93_errors), E"");
    *errors = __c93_errors;
    unsigned int adr = __c93_u24(__c93_r24a);
    return adr == 0xFFFFFF ? -1 : (int)adr;
}

/* Calls machine code at address with every register saved and restored around the call. */
static void c93_call(unsigned int address)
{
    asm(""LD XYZ, (QRS + 1)\nPUSH A, Z\nCALL XYZ\nPOP A, Z"");
}

/* ---- Text: a built-in 5x7 pixel font (space to 'Z'; lower case is drawn as upper case) ---- */

/* 7 rows per glyph, 5 bits per row (bit 4 = leftmost pixel), for characters 32 ('' '') to 90 ('Z') */
static const unsigned char __c93_font[59 * 7] = {
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,   /*   */
    0x04, 0x04, 0x04, 0x04, 0x04, 0x00, 0x04,   /* ! */
    0x0A, 0x0A, 0x00, 0x00, 0x00, 0x00, 0x00,   /* "" */
    0x0A, 0x1F, 0x0A, 0x0A, 0x1F, 0x0A, 0x00,   /* # */
    0x04, 0x0F, 0x14, 0x0E, 0x05, 0x1E, 0x04,   /* $ */
    0x19, 0x1A, 0x02, 0x04, 0x08, 0x0B, 0x13,   /* % */
    0x0C, 0x12, 0x14, 0x08, 0x15, 0x12, 0x0D,   /* & */
    0x04, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00,   /* ' */
    0x02, 0x04, 0x08, 0x08, 0x08, 0x04, 0x02,   /* ( */
    0x08, 0x04, 0x02, 0x02, 0x02, 0x04, 0x08,   /* ) */
    0x00, 0x04, 0x15, 0x0E, 0x15, 0x04, 0x00,   /* * */
    0x00, 0x04, 0x04, 0x1F, 0x04, 0x04, 0x00,   /* + */
    0x00, 0x00, 0x00, 0x00, 0x0C, 0x04, 0x08,   /* , */
    0x00, 0x00, 0x00, 0x1F, 0x00, 0x00, 0x00,   /* - */
    0x00, 0x00, 0x00, 0x00, 0x00, 0x0C, 0x0C,   /* . */
    0x01, 0x02, 0x02, 0x04, 0x08, 0x08, 0x10,   /* / */
    0x0E, 0x11, 0x11, 0x11, 0x11, 0x11, 0x0E,   /* 0 */
    0x04, 0x0C, 0x04, 0x04, 0x04, 0x04, 0x0E,   /* 1 */
    0x0E, 0x11, 0x01, 0x02, 0x04, 0x08, 0x1F,   /* 2 */
    0x1F, 0x02, 0x04, 0x02, 0x01, 0x11, 0x0E,   /* 3 */
    0x02, 0x06, 0x0A, 0x12, 0x1F, 0x02, 0x02,   /* 4 */
    0x1F, 0x10, 0x1E, 0x01, 0x01, 0x11, 0x0E,   /* 5 */
    0x06, 0x08, 0x10, 0x1E, 0x11, 0x11, 0x0E,   /* 6 */
    0x1F, 0x01, 0x02, 0x04, 0x08, 0x08, 0x08,   /* 7 */
    0x0E, 0x11, 0x11, 0x0E, 0x11, 0x11, 0x0E,   /* 8 */
    0x0E, 0x11, 0x11, 0x0F, 0x01, 0x02, 0x0C,   /* 9 */
    0x00, 0x0C, 0x0C, 0x00, 0x0C, 0x0C, 0x00,   /* : */
    0x00, 0x0C, 0x0C, 0x00, 0x0C, 0x04, 0x08,   /* ; */
    0x02, 0x04, 0x08, 0x10, 0x08, 0x04, 0x02,   /* < */
    0x00, 0x00, 0x1F, 0x00, 0x1F, 0x00, 0x00,   /* = */
    0x08, 0x04, 0x02, 0x01, 0x02, 0x04, 0x08,   /* > */
    0x0E, 0x11, 0x01, 0x02, 0x04, 0x00, 0x04,   /* ? */
    0x0E, 0x11, 0x01, 0x0D, 0x15, 0x15, 0x0E,   /* @ */
    0x0E, 0x11, 0x11, 0x1F, 0x11, 0x11, 0x11,   /* A */
    0x1E, 0x11, 0x11, 0x1E, 0x11, 0x11, 0x1E,   /* B */
    0x0E, 0x11, 0x10, 0x10, 0x10, 0x11, 0x0E,   /* C */
    0x1C, 0x12, 0x11, 0x11, 0x11, 0x12, 0x1C,   /* D */
    0x1F, 0x10, 0x10, 0x1E, 0x10, 0x10, 0x1F,   /* E */
    0x1F, 0x10, 0x10, 0x1E, 0x10, 0x10, 0x10,   /* F */
    0x0E, 0x11, 0x10, 0x17, 0x11, 0x11, 0x0F,   /* G */
    0x11, 0x11, 0x11, 0x1F, 0x11, 0x11, 0x11,   /* H */
    0x0E, 0x04, 0x04, 0x04, 0x04, 0x04, 0x0E,   /* I */
    0x07, 0x02, 0x02, 0x02, 0x02, 0x12, 0x0C,   /* J */
    0x11, 0x12, 0x14, 0x18, 0x14, 0x12, 0x11,   /* K */
    0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x1F,   /* L */
    0x11, 0x1B, 0x15, 0x15, 0x11, 0x11, 0x11,   /* M */
    0x11, 0x11, 0x19, 0x15, 0x13, 0x11, 0x11,   /* N */
    0x0E, 0x11, 0x11, 0x11, 0x11, 0x11, 0x0E,   /* O */
    0x1E, 0x11, 0x11, 0x1E, 0x10, 0x10, 0x10,   /* P */
    0x0E, 0x11, 0x11, 0x11, 0x15, 0x12, 0x0D,   /* Q */
    0x1E, 0x11, 0x11, 0x1E, 0x14, 0x12, 0x11,   /* R */
    0x0F, 0x10, 0x10, 0x0E, 0x01, 0x01, 0x1E,   /* S */
    0x1F, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04,   /* T */
    0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x0E,   /* U */
    0x11, 0x11, 0x11, 0x11, 0x11, 0x0A, 0x04,   /* V */
    0x11, 0x11, 0x11, 0x15, 0x15, 0x15, 0x0A,   /* W */
    0x11, 0x11, 0x0A, 0x04, 0x0A, 0x11, 0x11,   /* X */
    0x11, 0x11, 0x11, 0x0A, 0x04, 0x04, 0x04,   /* Y */
    0x1F, 0x01, 0x02, 0x04, 0x08, 0x10, 0x1F    /* Z */
};

/* Width in pixels of a string drawn with c93_draw_text at the given scale (glyphs advance 6 * scale). */
static int c93_text_width(const char *text, int scale)
{
    int n = 0;
    while (text[n])
        n++;
    return n > 0 ? n * 6 * scale - scale : 0;
}

/* Draws text with the built-in font; each font pixel becomes a scale x scale square.
   Horizontal runs of pixels are merged into a single rectangle to keep drawing cheap. */
static void c93_draw_text(int page, int x, int y, int scale, int color, const char *text)
{
    for (; *text; text++, x += 6 * scale)
    {
        int c = *text;
        if (c >= 'a' && c <= 'z')
            c -= 32;
        if (c < 32 || c > 'Z')
            continue;
        const unsigned char *glyph = __c93_font + (c - 32) * 7;
        for (int row = 0; row < 7; row++)
        {
            int bits = glyph[row];
            int col = 0;
            while (col < 5)
            {
                if (!(bits & (16 >> col)))
                {
                    col++;
                    continue;
                }
                int start = col;
                while (col < 5 && (bits & (16 >> col)))
                    col++;
                c93_fill_rect(page, x + start * scale, y + row * scale, (col - start) * scale, scale, color);
            }
        }
    }
}

#endif
";

        private const string StdDef = @"
#ifndef __C93_STDDEF_H
#define __C93_STDDEF_H
typedef unsigned int size_t;
typedef int ptrdiff_t;
#define NULL ((void *)0)
#define offsetof(type, member) ((size_t)&((type *)0)->member)
#endif
";

        private const string StdInt = @"
#ifndef __C93_STDINT_H
#define __C93_STDINT_H
typedef signed char int8_t;
typedef unsigned char uint8_t;
typedef short int16_t;
typedef unsigned short uint16_t;
typedef int int32_t;
typedef unsigned int uint32_t;
typedef int intptr_t;
typedef unsigned int uintptr_t;
#define INT8_MIN (-128)
#define INT8_MAX 127
#define UINT8_MAX 255
#define INT16_MIN (-32768)
#define INT16_MAX 32767
#define UINT16_MAX 65535
#define INT32_MIN (-2147483647 - 1)
#define INT32_MAX 2147483647
#define UINT32_MAX 4294967295u
#endif
";

        private const string StdBool = @"
#ifndef __C93_STDBOOL_H
#define __C93_STDBOOL_H
#define bool _Bool
#define true 1
#define false 0
#endif
";

        private const string Limits = @"
#ifndef __C93_LIMITS_H
#define __C93_LIMITS_H
#define CHAR_BIT 8
#define SCHAR_MIN (-128)
#define SCHAR_MAX 127
#define UCHAR_MAX 255
#define CHAR_MIN (-128)
#define CHAR_MAX 127
#define SHRT_MIN (-32768)
#define SHRT_MAX 32767
#define USHRT_MAX 65535
#define INT_MIN (-2147483647 - 1)
#define INT_MAX 2147483647
#define UINT_MAX 4294967295u
#define LONG_MIN (-2147483647 - 1)
#define LONG_MAX 2147483647
#define ULONG_MAX 4294967295u
#endif
";

        private const string StringH = @"
#ifndef __C93_STRING_H
#define __C93_STRING_H
#include <stddef.h>
void *memset(void *dst, int c, size_t n);
void *memcpy(void *dst, const void *src, size_t n);
void *memmove(void *dst, const void *src, size_t n);
int memcmp(const void *a, const void *b, size_t n);
size_t strlen(const char *s);
char *strcpy(char *dst, const char *src);
char *strncpy(char *dst, const char *src, size_t n);
char *strcat(char *dst, const char *src);
int strcmp(const char *a, const char *b);
int strncmp(const char *a, const char *b, size_t n);
char *strchr(const char *s, int c);
#endif
";

        private const string StdLib = @"
#ifndef __C93_STDLIB_H
#define __C93_STDLIB_H
#include <stddef.h>
#define RAND_MAX 32767
void *malloc(size_t size);
void *calloc(size_t count, size_t size);
void free(void *p);
int abs(int x);
int atoi(const char *s);
int rand(void);
void srand(unsigned int seed);
#endif
";

        private const string CType = @"
#ifndef __C93_CTYPE_H
#define __C93_CTYPE_H
int isdigit(int c);
int isalpha(int c);
int isalnum(int c);
int isspace(int c);
int isupper(int c);
int islower(int c);
int toupper(int c);
int tolower(int c);
#endif
";

        public const string LibrarySource = @"
#include <stddef.h>
#include <string.h>
#include <stdlib.h>
#include <ctype.h>

/* The bump allocator hands out memory after the program image; free() is a no-op. */
extern char __heap_start[];
static char *__c93_heap_ptr;
static unsigned int __c93_rand_state = 1;

void *malloc(size_t size)
{
    char *p;
    if (__c93_heap_ptr == 0)
        __c93_heap_ptr = __heap_start;
    p = __c93_heap_ptr;
    __c93_heap_ptr += size;
    return p;
}

void *calloc(size_t count, size_t size)
{
    void *p = malloc(count * size);
    memset(p, 0, count * size);
    return p;
}

void free(void *p)
{
    (void)p;
}

void *memset(void *dst, int c, size_t n)
{
    unsigned char *d = dst;
    while (n--)
        *d++ = (unsigned char)c;
    return dst;
}

void *memcpy(void *dst, const void *src, size_t n)
{
    unsigned char *d = dst;
    const unsigned char *s = src;
    while (n--)
        *d++ = *s++;
    return dst;
}

void *memmove(void *dst, const void *src, size_t n)
{
    unsigned char *d = dst;
    const unsigned char *s = src;
    if (d < s)
    {
        while (n--)
            *d++ = *s++;
    }
    else
    {
        d += n;
        s += n;
        while (n--)
            *--d = *--s;
    }
    return dst;
}

int memcmp(const void *a, const void *b, size_t n)
{
    const unsigned char *x = a;
    const unsigned char *y = b;
    for (; n; n--, x++, y++)
        if (*x != *y)
            return *x - *y;
    return 0;
}

size_t strlen(const char *s)
{
    const char *p = s;
    while (*p)
        p++;
    return p - s;
}

char *strcpy(char *dst, const char *src)
{
    char *r = dst;
    while ((*dst++ = *src++) != 0)
        ;
    return r;
}

char *strncpy(char *dst, const char *src, size_t n)
{
    size_t i = 0;
    for (; i < n && src[i]; i++)
        dst[i] = src[i];
    for (; i < n; i++)
        dst[i] = 0;
    return dst;
}

char *strcat(char *dst, const char *src)
{
    strcpy(dst + strlen(dst), src);
    return dst;
}

int strcmp(const char *a, const char *b)
{
    while (*a && *a == *b)
    {
        a++;
        b++;
    }
    return (unsigned char)*a - (unsigned char)*b;
}

int strncmp(const char *a, const char *b, size_t n)
{
    for (; n; n--, a++, b++)
    {
        if (*a != *b)
            return (unsigned char)*a - (unsigned char)*b;
        if (!*a)
            return 0;
    }
    return 0;
}

char *strchr(const char *s, int c)
{
    for (;; s++)
    {
        if (*s == (char)c)
            return (char *)s;
        if (!*s)
            return 0;
    }
}

int abs(int x)
{
    return x < 0 ? -x : x;
}

int atoi(const char *s)
{
    int sign = 1, v = 0;
    while (isspace(*s))
        s++;
    if (*s == '-' || *s == '+')
    {
        if (*s == '-')
            sign = -1;
        s++;
    }
    while (isdigit(*s))
        v = v * 10 + (*s++ - '0');
    return sign * v;
}

int rand(void)
{
    __c93_rand_state = __c93_rand_state * 1103515245u + 12345u;
    return (__c93_rand_state >> 16) & 0x7FFF;
}

void srand(unsigned int seed)
{
    __c93_rand_state = seed;
}

int isdigit(int c) { return c >= '0' && c <= '9'; }
int isalpha(int c) { return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'); }
int isalnum(int c) { return isalpha(c) || isdigit(c); }
int isspace(int c) { return c == ' ' || c == '\t' || c == '\n' || c == '\r' || c == '\f' || c == '\v'; }
int isupper(int c) { return c >= 'A' && c <= 'Z'; }
int islower(int c) { return c >= 'a' && c <= 'z'; }
int toupper(int c) { return islower(c) ? c - 32 : c; }
int tolower(int c) { return isupper(c) ? c + 32 : c; }
";
    }
}
