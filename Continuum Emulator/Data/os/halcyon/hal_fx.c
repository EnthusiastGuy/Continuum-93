/*
 * Halcyon OS - visual effects drawn on the top layer: sparks, ripples, zoom boxes, toasts and
 * magnetic snap guides.
 */

#define MAX_SPARKS 96
#define MAX_RIPPLES 6
#define MAX_ZOOMS 4

typedef struct { int x, y, vx, vy, life; } Spark;
typedef struct { int x, y, t; } Ripple;
typedef struct { int x0, y0, w0, h0, x1, y1, w1, h1, t; } Zoom;

Spark sparks[MAX_SPARKS];
Ripple ripples[MAX_RIPPLES];
Zoom zooms[MAX_ZOOMS];
int next_spark;

char toast_text[64];
int toast_color, toast_t;            /* toast_t counts down from 240 frames */

/* Snap guides found during the current drag (-1 = none) */
int snap_gx = -1, snap_gy = -1;

/* Small deterministic random generator (independent of the C library's rand) */
unsigned int fx_seed = 0x9E3779B9u;
int fx_rand(int n)
{
    fx_seed = fx_seed * 1664525u + 1013904223u;
    return (int)((fx_seed >> 9) % (unsigned int)n);
}

void spawn_sparks(int x, int y, int count, int hue)
{
    (void)hue;
    for (int i = 0; i < count; i++)
    {
        Spark *s = &sparks[next_spark];
        next_spark = (next_spark + 1) % MAX_SPARKS;
        s->x = x << 8;
        s->y = y << 8;
        s->vx = fx_rand(900) - 450;
        s->vy = fx_rand(900) - 650;
        s->life = 14 + fx_rand(14);
    }
}

void ripple(int x, int y)
{
    for (int i = 0; i < MAX_RIPPLES; i++)
        if (ripples[i].t <= 0)
        {
            ripples[i].x = x;
            ripples[i].y = y;
            ripples[i].t = 16;
            return;
        }
}

/* Amiga-style zoom: a trail of outlines travelling from one rectangle to another */
void zoom_box(int x0, int y0, int w0, int h0, int x1, int y1, int w1, int h1)
{
    for (int i = 0; i < MAX_ZOOMS; i++)
        if (zooms[i].t <= 0)
        {
            Zoom *z = &zooms[i];
            z->x0 = x0; z->y0 = y0; z->w0 = w0; z->h0 = h0;
            z->x1 = x1; z->y1 = y1; z->w1 = w1; z->h1 = h1;
            z->t = 12;
            return;
        }
}

void toast(const char *s, int color)
{
    int n = 0;
    while (s[n] && n < 62) { toast_text[n] = s[n]; n++; }
    toast_text[n] = 0;
    toast_color = color;
    toast_t = 240;
}

void update_fx(void)
{
    for (int i = 0; i < MAX_SPARKS; i++)
    {
        Spark *s = &sparks[i];
        if (s->life <= 0) continue;
        s->x += s->vx;
        s->y += s->vy;
        s->vy += 50;
        s->life--;
    }
    for (int i = 0; i < MAX_RIPPLES; i++)
        if (ripples[i].t > 0) ripples[i].t--;
    for (int i = 0; i < MAX_ZOOMS; i++)
        if (zooms[i].t > 0) zooms[i].t--;
    if (toast_t > 0) toast_t--;
}

void draw_fx(void)
{
    for (int i = 0; i < MAX_ZOOMS; i++)
    {
        Zoom *z = &zooms[i];
        if (z->t <= 0) continue;
        for (int k = 0; k < 3; k++)                  /* three outlines trailing each other */
        {
            int p = (12 - z->t) * 256 / 12 - k * 24;
            if (p < 0) continue;
            int x = mix(z->x0, z->x1, p), y = mix(z->y0, z->y1, p);
            int w = mix(z->w0, z->w1, p), h = mix(z->h0, z->h1, p);
            c93_rect(P_TOP, x, y, w, h, k == 0 ? K_SNAP : ACC(8 - k * 2));
        }
    }

    for (int i = 0; i < MAX_RIPPLES; i++)
    {
        Ripple *r = &ripples[i];
        if (r->t <= 0) continue;
        int rad = (16 - r->t) * 2 + 2;
        c93_ellipse(P_TOP, r->x, r->y, rad, rad, r->t > 8 ? K_SNAP : ACC(6));
    }

    for (int i = 0; i < MAX_SPARKS; i++)
    {
        Spark *s = &sparks[i];
        if (s->life <= 0) continue;
        int c = K_SPARK + (s->life > 15 ? 15 : s->life);
        c93_fill_rect(P_TOP, s->x >> 8, s->y >> 8, s->life > 10 ? 2 : 1, s->life > 10 ? 2 : 1, c);
    }

    if (snap_gx >= 0)
        c93_fill_rect(P_TOP, snap_gx, DESK_TOP, 1, DESK_BOTTOM - DESK_TOP, K_SNAP);
    if (snap_gy >= 0)
        c93_fill_rect(P_TOP, 0, snap_gy, SCREEN_W, 1, K_SNAP);

    if (toast_t > 0)
    {
        /* slides in from the right, waits, slides out */
        int w = txt_w(F_S, toast_text) + 28;
        int slide = toast_t > 220 ? (toast_t - 220) * w / 20 : toast_t < 20 ? (20 - toast_t) * w / 20 : 0;
        int x = SCREEN_W - w - 6 + slide, y = BAR_H + 6;
        c93_fill_round_rect(P_TOP, x + 2, y + 2, w, 18, K_SHADOW, 6);
        c93_fill_round_rect(P_TOP, x, y, w, 18, N(3), 6);
        c93_round_rect(P_TOP, x, y, w, 18, toast_color, 6);
        c93_fill_ellipse(P_TOP, x + 10, y + 9, 3, 3, toast_color);
        txt(P_TOP, F_S, x + 20, y + 5, N(15), toast_text);
    }
}

/* Old-TV transitions drawn over everything on the top layer */
void crt_frame(int t, int closing)
{
    /* t: 0..16; closing squeezes the picture into a line then a dot, opening does the reverse */
    int k = closing ? t : 16 - t;
    int h = SCREEN_H / 2 * (16 - k) / 16;
    c93_fill_rect(P_TOP, 0, 0, SCREEN_W, SCREEN_H / 2 - h, K_BLACK);
    c93_fill_rect(P_TOP, 0, SCREEN_H / 2 + h, SCREEN_W, SCREEN_H / 2 - h + 1, K_BLACK);
    if (k >= 12)
    {
        int w = SCREEN_W * (16 - k) / 4;
        c93_fill_rect(P_TOP, 0, SCREEN_H / 2 - 2, SCREEN_W, 4, K_BLACK);
        c93_fill_rect(P_TOP, SCREEN_W / 2 - w / 2, SCREEN_H / 2 - 1, w + 1, 2, K_WHITE);
    }
}
