/*
 * Halcyon OS - palettes, themes, text and drawing primitives.
 */

/* Accent / secondary colours of each theme */
static const unsigned char theme_acc[6][3] = {
    { 60, 230, 210 },   /* Lagoon  */
    { 255, 110, 170 },  /* Sunset  */
    { 255, 196, 70 },   /* Amber   */
    { 140, 240, 90 },   /* Lime    */
    { 110, 170, 255 },  /* Glacier */
    { 190, 120, 255 }   /* Orchid  */
};
static const unsigned char theme_sec[6][3] = {
    { 160, 90, 255 }, { 255, 170, 60 }, { 255, 90, 90 }, { 40, 200, 220 }, { 210, 120, 255 }, { 80, 220, 255 }
};
static const char *theme_names[6] = { "Lagoon", "Sunset", "Amber", "Lime", "Glacier", "Orchid" };

static const unsigned char paint_rgb[16][3] = {
    { 20, 20, 30 }, { 245, 245, 250 }, { 255, 80, 90 }, { 255, 150, 60 }, { 255, 225, 80 }, { 120, 230, 90 },
    { 40, 200, 140 }, { 60, 210, 240 }, { 70, 120, 255 }, { 150, 90, 255 }, { 240, 100, 220 }, { 150, 100, 60 },
    { 120, 125, 140 }, { 190, 195, 210 }, { 255, 190, 170 }, { 40, 60, 110 }
};

int brightness = 256;
unsigned char ui_pal[256 * 3];              /* the UI palette before fading */
unsigned char *font_ptr[3];

static int mix(int a, int b, int t) { return a + (b - a) * t / 256; }

static void ui_set(int i, int r, int g, int b)
{
    ui_pal[i * 3] = (unsigned char)(r < 0 ? 0 : r > 255 ? 255 : r);
    ui_pal[i * 3 + 1] = (unsigned char)(g < 0 ? 0 : g > 255 ? 255 : g);
    ui_pal[i * 3 + 2] = (unsigned char)(b < 0 ? 0 : b > 255 ? 255 : b);
}

/* Builds the UI palette for the current theme */
void build_ui_palette(void)
{
    const unsigned char *a = theme_acc[prefs.theme], *s = theme_sec[prefs.theme];
    memset(ui_pal, 0, sizeof(ui_pal));

    for (int i = 0; i < 16; i++)                    /* cool neutrals: midnight blue to white */
        ui_set(N(i), 7 + i * 15, 10 + i * 15, 24 + i * 15);

    for (int i = 0; i < 16; i++)
    {
        int t = i * 17;                             /* dark -> full -> pastel */
        if (i < 10)
        {
            ui_set(ACC(i), a[0] * (i + 2) / 11, a[1] * (i + 2) / 11, a[2] * (i + 2) / 11);
            ui_set(SEC(i), s[0] * (i + 2) / 11, s[1] * (i + 2) / 11, s[2] * (i + 2) / 11);
        }
        else
        {
            t = (i - 9) * 36;
            ui_set(ACC(i), mix(a[0], 255, t), mix(a[1], 255, t), mix(a[2], 255, t));
            ui_set(SEC(i), mix(s[0], 255, t), mix(s[1], 255, t), mix(s[2], 255, t));
        }
    }

    ui_set(K_SHADOW, 2, 3, 8);
    ui_set(K_HOT, a[0] / 3 + 30, a[1] / 3 + 30, a[2] / 3 + 40);
    ui_set(K_FOLDER, 250, 196, 80);   ui_set(K_FOLDER_HI, 255, 230, 150); ui_set(K_FOLDER_LO, 180, 120, 40);
    ui_set(K_ASM, 120, 230, 120);     ui_set(K_CSRC, 90, 170, 255);       ui_set(K_BAS, 255, 150, 70);
    ui_set(K_TEXTDOC, 200, 205, 225); ui_set(K_IMG, 210, 120, 255);       ui_set(K_FONT, 60, 220, 200);
    ui_set(K_OTHER, 140, 145, 170);
    ui_set(K_RED, 255, 85, 100);      ui_set(K_AMBER, 255, 190, 60);      ui_set(K_GREEN, 90, 220, 120);
    ui_set(K_WHITE, 255, 255, 255);   ui_set(K_BLACK, 0, 0, 0);

    for (int i = 0; i < 16; i++)                    /* chart gradient: accent -> secondary */
        ui_set(K_CHART + i, mix(a[0], s[0], i * 17), mix(a[1], s[1], i * 17), mix(a[2], s[2], i * 17));
    for (int i = 0; i < 16; i++)
        ui_set(K_PAINT + i, paint_rgb[i][0], paint_rgb[i][1], paint_rgb[i][2]);
    for (int i = 0; i < 16; i++)                    /* sparks: deep accent -> white hot */
        ui_set(K_SPARK + i, mix(a[0] / 3, 255, i * 17), mix(a[1] / 3, 255, i * 17), mix(a[2] / 3, 255, i * 17));
}

static int tri(int t) { t &= 255; return t < 128 ? t * 2 : (255 - t) * 2; }   /* 0..254 triangle wave */

/* Per-frame animated UI colours */
void animate_ui_palette(void)
{
    const unsigned char *a = theme_acc[prefs.theme], *s = theme_sec[prefs.theme];
    int p = tri(frame * 3);
    ui_set(K_PULSE, mix(a[0] / 2, a[0], p), mix(a[1] / 2, a[1], p), mix(a[2] / 2, a[2], p));
    ui_set(K_SEL, mix(a[0] / 3, a[0] * 2 / 3, p), mix(a[1] / 3, a[1] * 2 / 3, p), mix(a[2] / 3, a[2] * 2 / 3, p));
    ui_set(K_GLOW, mix(s[0] / 2, s[0], 255 - p), mix(s[1] / 2, s[1], 255 - p), mix(s[2] / 2, s[2], 255 - p));
    int f = tri(frame * 12);
    ui_set(K_SNAP, mix(a[0], 255, f), mix(a[1], 255, f), mix(a[2], 255, f));
    for (int i = 0; i < 8; i++)                     /* rainbow that flows across letters */
    {
        int h = (frame * 4 + i * 32) & 255;
        int r = tri(h), g = tri(h + 85), b = tri(h + 170);
        ui_set(K_RAINBOW + i, 60 + r * 3 / 4, 60 + g * 3 / 4, 70 + b * 3 / 4);
    }
}

/* Copies the UI palette (with the global brightness fade) to the UI layers */
void apply_palettes(void)
{
    for (int page = P_DESK; page <= P_TOP; page++)
    {
        unsigned char *dst = c93_palette(page);
        if (brightness >= 256)
            memcpy(dst, ui_pal, 768);
        else
            for (int i = 0; i < 768; i++)
                dst[i] = (unsigned char)(ui_pal[i] * brightness / 256);
    }
}

/* ---------------------------------------------------------------- text */

int font_h(int font) { return c93_font_height(font_ptr[font]); }

int txt(int page, int font, int x, int y, int color, const char *s)
{
    return c93_text(page, font_ptr[font], x, y, color, s, 480, 0, 0, 0);
}

int txt_w(int font, const char *s) { return c93_text_width_font(font_ptr[font], s); }

/* Draws text cut to at most maxw pixels (adds an ellipsis when it had to cut) */
int txt_clip(int page, int font, int x, int y, int color, const char *s, int maxw)
{
    char buf[96];
    int n = 0;
    if (maxw <= 0)
        return 0;
    if (txt_w(font, s) <= maxw)
        return txt(page, font, x, y, color, s);
    int ell = txt_w(font, "..");
    while (s[n] && n < 90)
    {
        buf[n] = s[n];
        buf[n + 1] = 0;
        if (txt_w(font, buf) + ell > maxw)
            break;
        n++;
    }
    buf[n] = '.';
    buf[n + 1] = '.';
    buf[n + 2] = 0;
    return txt(page, font, x, y, color, buf);
}

void txt_shadow(int page, int font, int x, int y, int color, const char *s)
{
    txt(page, font, x + 1, y + 1, K_SHADOW, s);
    txt(page, font, x, y, color, s);
}

void txt_center(int page, int font, int cx, int y, int color, const char *s)
{
    txt(page, font, cx - txt_w(font, s) / 2, y, color, s);
}

/* Big rainbow title: each letter takes the next flowing rainbow colour */
void txt_rainbow(int page, int font, int x, int y, const char *s, int outline)
{
    char one[2];
    one[1] = 0;
    for (int i = 0; s[i]; i++)
    {
        one[0] = s[i];
        int w = c93_text(page, font_ptr[font], x, y, K_RAINBOW + (i & 7), one, 480,
                         outline ? C93_TEXT_OUTLINE : 0, K_SHADOW, 0xFF);
        x += w + 1;
    }
}

/* Integer to text with optional thousands separators */
void fmt_int(char *out, int v, int sep)
{
    char tmp[16];
    int n = 0, neg = v < 0, digits = 0;
    unsigned int u = neg ? (unsigned int)-v : (unsigned int)v;
    do
    {
        if (sep && digits && digits % 3 == 0) tmp[n++] = ',';
        tmp[n++] = (char)('0' + u % 10);
        u /= 10;
        digits++;
    } while (u);
    if (neg) tmp[n++] = '-';
    for (int i = 0; i < n; i++) out[i] = tmp[n - 1 - i];
    out[n] = 0;
}

void fmt_2d(char *out, int v)
{
    out[0] = (char)('0' + (v / 10) % 10);
    out[1] = (char)('0' + v % 10);
    out[2] = 0;
}

/* ---------------------------------------------------------------- panels */

/* Amiga-style raised bevel */
void bevel(int page, int x, int y, int w, int h, int face, int light, int dark)
{
    c93_fill_rect(page, x, y, w, h, face);
    c93_fill_rect(page, x, y, w, 1, light);
    c93_fill_rect(page, x, y, 1, h, light);
    c93_fill_rect(page, x, y + h - 1, w, 1, dark);
    c93_fill_rect(page, x + w - 1, y, 1, h, dark);
}

/* Sunken well */
void well(int page, int x, int y, int w, int h, int face)
{
    bevel(page, x, y, w, h, face, N(1), N(9));
}

/* A push button with a centered label */
void button(int page, int x, int y, int w, int h, const char *label, int hot, int pressed)
{
    int face = pressed ? ACC(6) : hot ? N(7) : N(5);
    if (pressed) bevel(page, x, y, w, h, face, N(2), ACC(11));
    else bevel(page, x, y, w, h, face, N(9), N(2));
    txt(page, F_S, x + (w - txt_w(F_S, label)) / 2, y + (h - 8) / 2 + (pressed ? 1 : 0), pressed ? K_WHITE : N(15), label);
}

int inside(int px, int py, int x, int y, int w, int h)
{
    return px >= x && py >= y && px < x + w && py < y + h;
}

/* ---------------------------------------------------------------- icons */

static int kind_color(int kind)
{
    switch (kind)
    {
        case FK_ASM: return K_ASM;
        case FK_C: case FK_H: return K_CSRC;
        case FK_BAS: return K_BAS;
        case FK_TEXT: return K_TEXTDOC;
        case FK_IMG: return K_IMG;
        case FK_FONT: return K_FONT;
        default: return K_OTHER;
    }
}

/* 10x9 list icon */
void draw_mini_icon(int page, int kind, int x, int y)
{
    if (kind == FK_DIR)
    {
        c93_fill_rect(page, x, y + 1, 4, 2, K_FOLDER_HI);
        c93_fill_rect(page, x, y + 2, 10, 7, K_FOLDER);
        c93_fill_rect(page, x, y + 8, 10, 1, K_FOLDER_LO);
        return;
    }
    int c = kind_color(kind);
    c93_fill_rect(page, x + 1, y, 7, 9, N(14));
    c93_fill_rect(page, x + 6, y, 2, 2, N(9));
    c93_fill_rect(page, x + 2, y + 3, 5, 1, c);
    c93_fill_rect(page, x + 2, y + 5, 5, 1, c);
    c93_fill_rect(page, x + 2, y + 7, 3, 1, c);
}

/* Big desktop icon (32 x 26): drawers, disks and documents */
void draw_big_icon(int page, int kind, int x, int y, int hot)
{
    if (hot)
        c93_fill_round_rect(page, x - 4, y - 3, 40, 32, K_SEL, 6);
    if (kind == FK_DIR)
    {
        c93_fill_round_rect(page, x + 1, y + 2, 32, 24, K_SHADOW, 3);
        c93_fill_round_rect(page, x, y + 3, 12, 6, K_FOLDER_LO, 2);
        c93_fill_round_rect(page, x, y + 6, 32, 19, K_FOLDER, 3);
        c93_fill_rect(page, x + 2, y + 8, 28, 2, K_FOLDER_HI);
        c93_fill_rect(page, x + 12, y + 14, 8, 3, K_FOLDER_LO);
        return;
    }
    if (kind == FK_OTHER)               /* a disk drive */
    {
        c93_fill_round_rect(page, x + 1, y + 6, 32, 20, K_SHADOW, 3);
        bevel(page, x, y + 5, 32, 20, N(11), N(15), N(5));
        c93_fill_rect(page, x + 4, y + 10, 24, 3, N(3));
        c93_fill_rect(page, x + 22, y + 18, 6, 2, ACC(9));
        c93_fill_rect(page, x + 4, y + 18, 10, 2, N(7));
        return;
    }
    int c = kind_color(kind);
    c93_fill_rect(page, x + 7, y + 1, 22, 26, K_SHADOW);
    c93_fill_rect(page, x + 6, y, 22, 26, N(14));
    c93_fill_rect(page, x + 22, y, 6, 6, N(10));
    for (int i = 0; i < 5; i++)
        c93_fill_rect(page, x + 9, y + 8 + i * 3, 12 + (i * 5) % 5, 1, c);
}

/* App icons drawn procedurally at size s (around 22-34 px) */
void draw_app_icon(int page, int app, int x, int y, int s)
{
    int u = s / 8;                      /* unit */
    if (u < 2) u = 2;
    c93_fill_round_rect(page, x + 1, y + 2, s, s, K_SHADOW, s / 4);
    switch (app)
    {
        case APP_DRAWERS:
            c93_fill_round_rect(page, x, y, s, s, K_FOLDER_LO, s / 4);
            c93_fill_round_rect(page, x + u, y + u * 2, s - u * 2, s - u * 3, K_FOLDER, s / 6);
            c93_fill_rect(page, x + u, y + u * 2, s - u * 2, u, K_FOLDER_HI);
            c93_fill_rect(page, x + s / 2 - u, y + s / 2, u * 2, u, K_FOLDER_LO);
            break;
        case APP_PULSE:
            c93_fill_round_rect(page, x, y, s, s, N(2), s / 4);
            for (int i = 0; i < 4; i++)
            {
                int bh = (u * (2 + ((i * 3 + frame / 8) % 5)));
                c93_fill_rect(page, x + u + i * (u * 3 / 2 + 1), y + s - u - bh, u, bh, K_CHART + i * 4);
            }
            break;
        case APP_CALC:
            c93_fill_round_rect(page, x, y, s, s, N(6), s / 4);
            c93_fill_rect(page, x + u, y + u, s - u * 2, u * 2, ACC(3));
            for (int r = 0; r < 2; r++)
                for (int c = 0; c < 3; c++)
                    c93_fill_rect(page, x + u + c * (u * 2), y + u * 4 + r * (u * 2), u + u / 2, u + u / 2, c == 2 ? SEC(8) : N(12));
            break;
        case APP_SKETCH:
            c93_fill_round_rect(page, x, y, s, s, N(14), s / 4);
            c93_fill_ellipse(page, x + s / 3, y + s / 3, u + 1, u + 1, K_PAINT + 2);
            c93_fill_ellipse(page, x + s * 2 / 3, y + s / 3, u + 1, u + 1, K_PAINT + 7);
            c93_fill_ellipse(page, x + s / 3, y + s * 2 / 3, u + 1, u + 1, K_PAINT + 4);
            c93_fill_ellipse(page, x + s * 2 / 3, y + s * 2 / 3, u + 1, u + 1, K_PAINT + 9);
            break;
        case APP_PREFS:
            c93_fill_round_rect(page, x, y, s, s, N(4), s / 4);
            c93_fill_ellipse(page, x + s / 2, y + s / 2, s / 3, s / 3, ACC(9));
            c93_fill_ellipse(page, x + s / 2, y + s / 2, s / 6, s / 6, N(4));
            for (int i = 0; i < 4; i++)
                c93_fill_rect(page, x + s / 2 - u / 2 + (i == 1 ? s / 3 : i == 3 ? -s / 3 : 0), y + s / 2 - u / 2 + (i == 0 ? -s / 3 : i == 2 ? s / 3 : 0), u, u, ACC(9));
            break;
        case APP_READER:
            c93_fill_round_rect(page, x, y, s, s, N(13), s / 4);
            for (int i = 0; i < 4; i++)
                c93_fill_rect(page, x + u, y + u + i * u * 3 / 2, s - u * 2 - (i % 2) * u * 2, u / 2 + 1, i == 0 ? ACC(6) : N(6));
            break;
        case APP_ABOUT:
        default:
            c93_fill_round_rect(page, x, y, s, s, N(2), s / 4);
            for (int i = 0; i < 4; i++)
                c93_fill_rect(page, x + u, y + u + i * (s - u * 2) / 4, s - u * 2, (s - u * 2) / 4, K_RAINBOW + i * 2);
            break;
    }
}
