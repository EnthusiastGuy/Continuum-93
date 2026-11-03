/*
 * Halcyon OS - animated backdrop.
 *
 * The backdrop is rendered once into layer 0 as palette indexes and then animated purely by rotating
 * the palette (Amiga-style colour cycling), so it costs almost nothing per frame.
 *   Aurora : flowing ribbons of light       Copper : waving Amiga copper bars       Nebula : drifting clouds
 * All styles share a twinkling star field and a mountain silhouette with a glowing horizon.
 */

static const char *backdrop_names[3] = { "Aurora", "Copper", "Nebula" };
static signed char sine_tab[256];
static int hill_y[SCREEN_W];
static const int bayer4[4][4] = { { 0, 8, 2, 10 }, { 12, 4, 14, 6 }, { 3, 11, 1, 9 }, { 15, 7, 13, 5 } };

void init_sine(void)
{
    for (int a = 0; a < 256; a++)
    {
        int x = a & 127;
        int v = 4 * x * (128 - x) / 128;             /* parabola approximating a half sine, 0..128 */
        sine_tab[a] = (signed char)(a < 128 ? (v > 127 ? 127 : v) : -(v > 127 ? 127 : v));
    }
    for (int x = 0; x < SCREEN_W; x++)
        hill_y[x] = 214 + sine_tab[(x * 2) & 255] / 9 + sine_tab[(x * 5 + 60) & 255] / 14 + sine_tab[(x * 11 + 7) & 255] / 30;
}

#define isin(a) sine_tab[(a) & 255]

/* Renders rows y0..y1-1 of the backdrop; called a few rows per frame during the boot splash */
void build_backdrop_rows(int y0, int y1)
{
    unsigned char *vram = c93_video_page(P_BACK);
    int style = prefs.backdrop;

    for (int y = y0; y < y1 && y < SCREEN_H; y++)
    {
        unsigned char *row = vram + y * SCREEN_W;
        for (int x = 0; x < SCREEN_W; x++)
        {
            int idx;
            /* brightness band with an ordered-dither seam instead of a hard edge */
            int t = y + bayer4[y & 3][x & 3] * 2 - 16;   /* 32 px dithered transition */
            int band = t < 70 ? 0 : t < 130 ? 1 : t < 180 ? 2 : 3;
            if (y >= hill_y[x])
            {
                /* mountains: darker towards the bottom, with a bright rim */
                int d = y - hill_y[x];
                idx = d == 0 ? B_HILL + 31 : d == 1 ? B_HILL + 24 : B_HILL + (d > 40 ? 0 : 16 - d * 16 / 40);
                row[x] = (unsigned char)idx;
                continue;
            }
            if (style == 0)         /* aurora ribbons */
                idx = isin(x * 2 + isin(y * 3 + x / 2) / 2) + isin(y * 4 - x) / 2 + isin(x / 2 + y * 6) / 3;
            else if (style == 1)    /* copper bars waving sideways */
                idx = y * 4 + isin(x * 2 + y) / 6;
            else                    /* nebula */
                idx = isin(x + isin(y * 2) / 2) + isin(y * 3 + isin(x * 2 + 40) / 3) + isin((x + y) * 2) / 2;
            row[x] = (unsigned char)B_BAND(band, (idx >> 3) & 31);
        }
    }

    /* stars sprinkled over the sky (each picks one of 16 twinkle levels) */
    if (y1 >= SCREEN_H)
    {
        unsigned int seed = 12345;
        for (int i = 0; i < 140; i++)
        {
            seed = seed * 1103515245u + 12345u;
            int x = (int)((seed >> 8) % SCREEN_W);
            seed = seed * 1103515245u + 12345u;
            int y = (int)((seed >> 8) % 200);
            if (y < hill_y[x] - 3)
                vram[y * SCREEN_W + x] = (unsigned char)(B_STAR + (i & 15));
        }
    }
}

/* Rotates the band colours and twinkles the stars */
void animate_backdrop(void)
{
    unsigned char *pal = c93_palette(P_BACK);
    const unsigned char *a = theme_acc[prefs.theme], *s = theme_sec[prefs.theme];
    static const int band_level[4] = { 256, 180, 120, 70 };
    int style = prefs.backdrop;
    int shift = frame / (style == 1 ? 1 : 2);

    for (int b = 0; b < 4; b++)
    {
        for (int i = 0; i < 32; i++)
        {
            int t = tri((i + shift) * 8);                  /* 0..254 colour wave */
            int glow = style == 1 ? tri((i + shift) * 16) : t;
            int r = mix(6, mix(a[0], s[0], t), glow * 3 / 4);
            int g = mix(8, mix(a[1], s[1], t), glow * 3 / 4);
            int bl = mix(22, mix(a[2], s[2], t), glow * 3 / 4);
            if (style == 2)                                /* nebula is darker and more purple */
            {
                r = r * 3 / 4 + 10;
                g = g / 2;
                bl = bl * 3 / 4 + 20;
            }
            int lv = band_level[b] * brightness / 256;
            unsigned char *p = pal + B_BAND(b, i) * 3;
            p[0] = (unsigned char)(r * lv / 256);
            p[1] = (unsigned char)(g * lv / 256);
            p[2] = (unsigned char)(bl * lv / 256);
        }
    }

    for (int i = 0; i < 16; i++)
    {
        int v = 70 + tri(frame * 5 + i * 37) * 2 / 3;
        unsigned char *p = pal + (B_STAR + i) * 3;
        p[0] = (unsigned char)(v * brightness / 256);
        p[1] = (unsigned char)(v * brightness / 256);
        p[2] = (unsigned char)((v + 30 > 255 ? 255 : v + 30) * brightness / 256);
    }
}

/* Static silhouette colours (refreshed on theme change and during fades) */
void backdrop_static_palette(void)
{
    unsigned char *pal = c93_palette(P_BACK);
    const unsigned char *a = theme_acc[prefs.theme];
    for (int i = 0; i < 32; i++)
    {
        int r = 4 + i / 3, g = 5 + i / 3, b = 14 + i / 2;
        if (i >= 24)                                    /* glowing rim */
        {
            r = mix(r, a[0], (i - 23) * 28);
            g = mix(g, a[1], (i - 23) * 28);
            b = mix(b, a[2], (i - 23) * 28);
        }
        unsigned char *p = pal + (B_HILL + i) * 3;
        p[0] = (unsigned char)(r * brightness / 256);
        p[1] = (unsigned char)(g * brightness / 256);
        p[2] = (unsigned char)(b * brightness / 256);
    }
}
