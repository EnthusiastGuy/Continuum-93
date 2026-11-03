/*
 * NEON RICOCHET
 * An original brick-breaker for the Continuum 93 fantasy computer, written in C and compiled on the
 * machine itself by the built-in C compiler (open this file from the OS file browser to play).
 *
 * Controls: LEFT / RIGHT (or A / D) move the paddle, SPACE launches the ball and starts the game,
 *           P pauses, ESC returns to the operating system.
 *
 * Power-ups fall from some bricks:  W = wide paddle   M = multi-ball   S = slow motion   + = extra life
 *
 * Building with -D C93_TEST_FRAMES=n (or "#define C93_TEST_FRAMES n" before including this file) runs the
 * game headless on autopilot for n frames, which is how the unit tests exercise it.
 *
 * Part of Continuum 93, MIT licensed.
 */

#include <continuum.h>
#include <stdlib.h>
#include <string.h>

/* ------------------------------------------------------------------------------------------ constants */

#define PAGE        0
#define W           C93_SCREEN_WIDTH
#define H           C93_SCREEN_HEIGHT
#define FP          8                       /* fixed point: 8 fractional bits */
#define HUD_H       22

#define COLS        12
#define ROWS        7
#define BRICK_W     36
#define BRICK_H     12
#define BRICK_GAP   2
#define BRICKS_X    13
#define BRICKS_Y    36

#define PADDLE_Y    248
#define PADDLE_H    8
#define PADDLE_W    56
#define PADDLE_WIDE 88
#define PADDLE_SPEED 6

#define BALL_R      3
#define MAX_BALLS   4
#define TRAIL       6
#define MAX_PARTS   160
#define MAX_PICKUPS 6
#define MAX_POPUPS  8
#define MAX_STARS   70

/* Palette layout */
enum
{
    COL_BLACK = 1,
    COL_BG = 2,         /* 32 background shades                     2..33   */
    COL_BRICK = 40,     /* 8 hues x 4 shades (base, light, dark, deep) 40..71 */
    COL_SPARK = 80,     /* 16-step spark fade                       80..95  */
    COL_CYAN = 100,     /* 8-step glow ramp                         100..107 */
    COL_WHITE = 120,
    COL_GREY,
    COL_YELLOW,
    COL_PINK,
    COL_GREEN,
    COL_GRID,
    COL_GRID_HI,
    COL_STAR = 130      /* 16 star brightness levels                130..145 */
};

enum { STATE_TITLE, STATE_SERVE, STATE_PLAY, STATE_LEVEL_CLEAR, STATE_GAME_OVER };
enum { PU_WIDE, PU_MULTI, PU_SLOW, PU_LIFE };

/* ------------------------------------------------------------------------------------------ game state */

typedef struct
{
    int x, y, vx, vy;           /* fixed point */
    int active;
    int tx[TRAIL], ty[TRAIL];   /* trail positions (pixels) */
} Ball;

typedef struct { int x, y, vx, vy, life, hue; } Particle;
typedef struct { int x, y, type, active; } Pickup;
typedef struct { int x, y, life, value; } Popup;
typedef struct { int x, y, phase; } Star;

unsigned char brick_hp[ROWS][COLS];
unsigned char brick_hue[ROWS][COLS];
unsigned char brick_flash[ROWS][COLS];

Ball balls[MAX_BALLS];
Particle parts[MAX_PARTS];
Pickup pickups[MAX_PICKUPS];
Popup popups[MAX_POPUPS];
Star stars[MAX_STARS];

int state;
int score, best_score, lives, level, bricks_left;
int paddle_x, paddle_w, paddle_flash;
int wide_timer, slow_timer;
int shake, shake_x, shake_y;
int frame, state_timer, paused;
int next_part;
unsigned char keys[32], prev_keys[32];

/* Unit vectors (x 256) for the bounce angle off the paddle, from far left to far right */
static const int bounce_vx[10] = { -222, -181, -128, -79, -36, 36, 79, 128, 181, 222 };
static const int bounce_vy[10] = { -128, -181, -222, -243, -254, -254, -243, -222, -181, -128 };

static const unsigned char hues[8][3] =
{
    { 255, 70, 110 }, { 255, 150, 50 }, { 250, 225, 70 }, { 90, 235, 120 },
    { 50, 215, 245 }, { 85, 130, 255 }, { 175, 95, 255 }, { 255, 95, 210 }
};

/* ------------------------------------------------------------------------------------------ helpers */

int key_down(int k) { return c93_key_down(keys, k); }
int key_pressed(int k) { return c93_key_down(keys, k) && !c93_key_down(prev_keys, k); }

int clamp(int v, int lo, int hi) { return v < lo ? lo : v > hi ? hi : v; }

int random_range(int lo, int hi) { return lo + rand() % (hi - lo + 1); }

/* Writes v as decimal, zero padded to at least 'digits' characters */
void format_int(char *out, int v, int digits)
{
    char tmp[12];
    int n = 0;
    if (v < 0) v = 0;
    do { tmp[n++] = (char)('0' + v % 10); v /= 10; } while (v > 0);
    while (n < digits) tmp[n++] = '0';
    for (int i = 0; i < n; i++) out[i] = tmp[n - 1 - i];
    out[n] = 0;
}

int ball_speed(void)
{
    int s = 768 + (level - 1) * 48;          /* 3 px/frame, faster every level */
    if (s > 1280) s = 1280;
    if (slow_timer > 0) s = s * 2 / 3;
    return s;
}

/* Rescales a ball's velocity to the current speed, keeping its direction */
void normalize_speed(Ball *b)
{
    int ax = abs(b->vx), ay = abs(b->vy);
    int len = ax > ay ? ax + ay * 3 / 8 : ay + ax * 3 / 8;   /* cheap length approximation */
    if (len == 0) return;
    int s = ball_speed();
    b->vx = b->vx * s / len;
    b->vy = b->vy * s / len;
    if (abs(b->vy) < 160) b->vy = b->vy < 0 ? -160 : 160;    /* never get stuck horizontally */
}

/* ------------------------------------------------------------------------------------------ palette */

void setup_palette(void)
{
    c93_set_color(PAGE, 0, 0, 0, 0);
    c93_set_color(PAGE, COL_BLACK, 6, 3, 16);

    for (int i = 0; i < 32; i++)                       /* indigo sky fading into a teal horizon */
        c93_set_color(PAGE, COL_BG + i, 14 + i / 4, 5 + i * 3 / 4, 34 + i);

    for (int h = 0; h < 8; h++)
    {
        int r = hues[h][0], g = hues[h][1], b = hues[h][2];
        c93_set_color(PAGE, COL_BRICK + h * 4 + 0, r, g, b);
        c93_set_color(PAGE, COL_BRICK + h * 4 + 1, (r + 255) / 2, (g + 255) / 2, (b + 255) / 2);
        c93_set_color(PAGE, COL_BRICK + h * 4 + 2, r * 3 / 5, g * 3 / 5, b * 3 / 5);
        c93_set_color(PAGE, COL_BRICK + h * 4 + 3, r / 4, g / 4, b / 4);
    }

    for (int k = 0; k < 16; k++)                       /* sparks: deep red -> orange -> white hot */
        c93_set_color(PAGE, COL_SPARK + k, clamp(70 + k * 13, 0, 255), clamp(k * k, 0, 255), k > 11 ? (k - 11) * 55 : 0);

    for (int k = 0; k < 8; k++)                        /* cyan glow */
        c93_set_color(PAGE, COL_CYAN + k, k * 20, 90 + k * 23, 130 + k * 17);

    c93_set_color(PAGE, COL_WHITE, 255, 255, 255);
    c93_set_color(PAGE, COL_GREY, 150, 140, 185);
    c93_set_color(PAGE, COL_YELLOW, 255, 225, 80);
    c93_set_color(PAGE, COL_PINK, 255, 70, 190);
    c93_set_color(PAGE, COL_GREEN, 90, 255, 140);
    c93_set_color(PAGE, COL_GRID, 70, 20, 95);
    c93_set_color(PAGE, COL_GRID_HI, 150, 40, 170);

    for (int k = 0; k < 16; k++)
        c93_set_color(PAGE, COL_STAR + k, 40 + k * 13, 40 + k * 13, 60 + k * 12);
}

/* ------------------------------------------------------------------------------------------ effects */

void spawn_particles(int x, int y, int hue, int count)
{
    for (int i = 0; i < count; i++)
    {
        Particle *p = &parts[next_part];
        next_part = (next_part + 1) % MAX_PARTS;
        p->x = x << FP;
        p->y = y << FP;
        p->vx = random_range(-700, 700);
        p->vy = random_range(-900, 200);
        p->life = random_range(18, 34);
        p->hue = hue;
    }
}

void spawn_popup(int x, int y, int value)
{
    for (int i = 0; i < MAX_POPUPS; i++)
    {
        if (popups[i].life == 0)
        {
            popups[i].x = x;
            popups[i].y = y;
            popups[i].life = 40;
            popups[i].value = value;
            return;
        }
    }
}

void spawn_pickup(int x, int y)
{
    for (int i = 0; i < MAX_PICKUPS; i++)
    {
        if (!pickups[i].active)
        {
            int roll = rand() % 10;
            pickups[i].type = roll < 4 ? PU_WIDE : roll < 7 ? PU_MULTI : roll < 9 ? PU_SLOW : PU_LIFE;
            pickups[i].x = x;
            pickups[i].y = y << FP;
            pickups[i].active = 1;
            return;
        }
    }
}

void update_effects(void)
{
    for (int i = 0; i < MAX_PARTS; i++)
    {
        Particle *p = &parts[i];
        if (p->life <= 0) continue;
        p->x += p->vx;
        p->y += p->vy;
        p->vy += 40;                                   /* gravity */
        p->vx = p->vx * 31 / 32;                       /* drag */
        p->life--;
    }

    for (int i = 0; i < MAX_POPUPS; i++)
    {
        if (popups[i].life > 0)
        {
            popups[i].life--;
            if (popups[i].life % 2) popups[i].y--;
        }
    }

    if (shake > 0)
    {
        shake--;
        shake_x = random_range(-shake, shake) / 2;
        shake_y = random_range(-shake, shake) / 2;
    }
    else
    {
        shake_x = shake_y = 0;
    }

    if (paddle_flash > 0) paddle_flash--;
    for (int r = 0; r < ROWS; r++)
        for (int c = 0; c < COLS; c++)
            if (brick_flash[r][c]) brick_flash[r][c]--;
}

/* ------------------------------------------------------------------------------------------ levels */

void build_level(int lvl)
{
    int pattern = (lvl - 1) % 4;
    int bonus = (lvl - 1) / 4;                         /* every full cycle bricks get tougher */
    bricks_left = 0;

    for (int r = 0; r < ROWS; r++)
    {
        for (int c = 0; c < COLS; c++)
        {
            int hp = 0, hue = (r + lvl) % 8;
            switch (pattern)
            {
                case 0:                                /* rainbow wall */
                    hp = r < 6 ? 1 : 0;
                    break;
                case 1:                                /* checkerboard, armored crown */
                    hp = (r + c) % 2 == 0 ? (r < 2 ? 2 : 1) : 0;
                    hue = (c + lvl) % 8;
                    break;
                case 2:                                /* diamond with a hard core */
                {
                    int d = abs(c * 2 - (COLS - 1)) + abs(r * 2 - (ROWS - 1)) * 2;
                    hp = d < 13 ? (d < 6 ? 3 : d < 9 ? 2 : 1) : 0;
                    hue = (d / 2 + lvl) % 8;
                    break;
                }
                default:                               /* fortress: armored walls around striped rooms */
                    if (r == 0 || c == 0 || c == COLS - 1) hp = 2;
                    else hp = (c / 2 + r) % 3 != 0 ? 1 : 0;
                    hue = r == 0 || c == 0 || c == COLS - 1 ? 5 : (c / 2 + lvl) % 8;
                    break;
            }
            if (hp) hp = clamp(hp + bonus, 1, 3);
            brick_hp[r][c] = (unsigned char)hp;
            brick_hue[r][c] = (unsigned char)hue;
            brick_flash[r][c] = 0;
            if (hp) bricks_left++;
        }
    }
}

void reset_balls_on_paddle(void)
{
    memset(balls, 0, sizeof(balls));
    balls[0].active = 1;
    balls[0].x = paddle_x << FP;
    balls[0].y = (PADDLE_Y - BALL_R - 1) << FP;
    for (int k = 0; k < TRAIL; k++)
    {
        balls[0].tx[k] = paddle_x;
        balls[0].ty[k] = PADDLE_Y - BALL_R - 1;
    }
}

void start_level(int lvl)
{
    level = lvl;
    build_level(lvl);
    memset(pickups, 0, sizeof(pickups));
    wide_timer = slow_timer = 0;
    paddle_w = PADDLE_W;
    paddle_x = W / 2;
    reset_balls_on_paddle();
    state = STATE_SERVE;
    state_timer = 0;
}

void new_game(void)
{
    score = 0;
    lives = 3;
    start_level(1);
}

/* ------------------------------------------------------------------------------------------ physics */

void launch_ball(void)
{
    balls[0].vx = 120;
    balls[0].vy = -700;
    normalize_speed(&balls[0]);
    state = STATE_PLAY;
}

void hit_brick(int r, int c)
{
    int bx = BRICKS_X + c * (BRICK_W + BRICK_GAP) + BRICK_W / 2;
    int by = BRICKS_Y + r * (BRICK_H + BRICK_GAP) + BRICK_H / 2;

    brick_hp[r][c]--;
    brick_flash[r][c] = 6;
    score += 10;

    if (brick_hp[r][c] == 0)
    {
        int points = 50 * level;
        score += points;
        bricks_left--;
        spawn_particles(bx, by, brick_hue[r][c], 14);
        spawn_popup(bx - 6, by - 4, points);
        shake = 6;
        if (rand() % 7 == 0)
            spawn_pickup(bx, by);
    }
    else
    {
        spawn_particles(bx, by, brick_hue[r][c], 4);
    }
}

/* Checks the ball against the bricks around it and bounces off the first one it overlaps */
void collide_bricks(Ball *b)
{
    int bx = b->x >> FP, by = b->y >> FP;
    int c0 = (bx - BALL_R - BRICKS_X) / (BRICK_W + BRICK_GAP);
    int r0 = (by - BALL_R - BRICKS_Y) / (BRICK_H + BRICK_GAP);

    if (by + BALL_R < BRICKS_Y || by - BALL_R > BRICKS_Y + ROWS * (BRICK_H + BRICK_GAP))
        return;

    for (int r = r0 - 1; r <= r0 + 1; r++)
    {
        for (int c = c0 - 1; c <= c0 + 1; c++)
        {
            if (r < 0 || r >= ROWS || c < 0 || c >= COLS || !brick_hp[r][c])
                continue;

            int x0 = BRICKS_X + c * (BRICK_W + BRICK_GAP), x1 = x0 + BRICK_W;
            int y0 = BRICKS_Y + r * (BRICK_H + BRICK_GAP), y1 = y0 + BRICK_H;
            if (bx + BALL_R < x0 || bx - BALL_R > x1 || by + BALL_R < y0 || by - BALL_R > y1)
                continue;

            /* Bounce along the axis with the smallest penetration */
            int px = bx < (x0 + x1) / 2 ? bx + BALL_R - x0 : x1 - (bx - BALL_R);
            int py = by < (y0 + y1) / 2 ? by + BALL_R - y0 : y1 - (by - BALL_R);
            if (px < py)
            {
                b->vx = bx < (x0 + x1) / 2 ? -abs(b->vx) : abs(b->vx);
            }
            else
            {
                b->vy = by < (y0 + y1) / 2 ? -abs(b->vy) : abs(b->vy);
            }
            hit_brick(r, c);
            return;
        }
    }
}

void collide_paddle(Ball *b)
{
    int bx = b->x >> FP, by = b->y >> FP;
    int left = paddle_x - paddle_w / 2;

    if (b->vy <= 0 || by + BALL_R < PADDLE_Y || by - BALL_R > PADDLE_Y + PADDLE_H)
        return;
    if (bx + BALL_R < left || bx - BALL_R > left + paddle_w)
        return;

    int idx = clamp((bx - left) * 10 / (paddle_w + 1), 0, 9);
    int s = ball_speed();
    b->vx = bounce_vx[idx] * s >> 8;
    b->vy = bounce_vy[idx] * s >> 8;
    b->y = (PADDLE_Y - BALL_R - 1) << FP;
    paddle_flash = 8;
    spawn_particles(bx, PADDLE_Y, 4, 5);
}

void update_ball(Ball *b)
{
    for (int step = 0; step < 2; step++)              /* two sub-steps keep fast balls out of bricks */
    {
        b->x += b->vx / 2;
        b->y += b->vy / 2;

        int bx = b->x >> FP, by = b->y >> FP;
        if (bx < BALL_R) { b->x = BALL_R << FP; b->vx = abs(b->vx); }
        if (bx > W - 1 - BALL_R) { b->x = (W - 1 - BALL_R) << FP; b->vx = -abs(b->vx); }
        if (by < HUD_H + BALL_R + 1) { b->y = (HUD_H + BALL_R + 1) << FP; b->vy = abs(b->vy); }

        collide_paddle(b);
        collide_bricks(b);
    }

    for (int k = TRAIL - 1; k > 0; k--)
    {
        b->tx[k] = b->tx[k - 1];
        b->ty[k] = b->ty[k - 1];
    }
    b->tx[0] = b->x >> FP;
    b->ty[0] = b->y >> FP;

    if ((b->y >> FP) > H + 8)
        b->active = 0;
}

void apply_pickup(int type)
{
    switch (type)
    {
        case PU_WIDE: wide_timer = 60 * 12; break;
        case PU_SLOW:
            slow_timer = 60 * 8;
            for (int i = 0; i < MAX_BALLS; i++) if (balls[i].active) normalize_speed(&balls[i]);
            break;
        case PU_LIFE: if (lives < 9) lives++; break;
        case PU_MULTI:
        {
            Ball *src = 0;
            for (int i = 0; i < MAX_BALLS; i++) if (balls[i].active) { src = &balls[i]; break; }
            if (!src) break;
            for (int i = 0, made = 0; i < MAX_BALLS && made < 2; i++)
            {
                if (balls[i].active) continue;
                balls[i] = *src;
                balls[i].vx = made == 0 ? -src->vx - 200 : src->vx + 200;
                normalize_speed(&balls[i]);
                made++;
            }
            break;
        }
    }
    score += 25;
}

void update_pickups(void)
{
    for (int i = 0; i < MAX_PICKUPS; i++)
    {
        Pickup *p = &pickups[i];
        if (!p->active) continue;
        p->y += 300;
        int py = p->y >> FP;
        if (py > H + 10) { p->active = 0; continue; }
        if (py + 5 >= PADDLE_Y && py - 5 <= PADDLE_Y + PADDLE_H &&
            p->x + 10 >= paddle_x - paddle_w / 2 && p->x - 10 <= paddle_x + paddle_w / 2)
        {
            p->active = 0;
            apply_pickup(p->type);
            spawn_particles(p->x, PADDLE_Y, 7, 10);
        }
    }
}

/* Autopilot used by the headless test build: follows the lowest ball heading down */
int autopilot_target(void)
{
    int best = -1, best_y = -1;
    for (int i = 0; i < MAX_BALLS; i++)
        if (balls[i].active && (balls[i].y >> FP) > best_y)
        {
            best = i;
            best_y = balls[i].y >> FP;
        }
    if (best < 0) return paddle_x;
    return (balls[best].x >> FP) + ((frame / 40) % 5 - 2) * 6;   /* small wobble varies the angles */
}

void update_play(void)
{
    /* Paddle */
    int target_w = wide_timer > 0 ? PADDLE_WIDE : PADDLE_W;
    if (paddle_w < target_w) paddle_w += 2;
    if (paddle_w > target_w) paddle_w -= 2;
    if (wide_timer > 0) wide_timer--;
    if (slow_timer > 0)
    {
        slow_timer--;
        if (slow_timer == 0)
            for (int i = 0; i < MAX_BALLS; i++) if (balls[i].active) normalize_speed(&balls[i]);
    }

#ifdef C93_TEST_FRAMES
    int target = autopilot_target();
    if (paddle_x < target - 2) paddle_x += PADDLE_SPEED;
    else if (paddle_x > target + 2) paddle_x -= PADDLE_SPEED;
#else
    if (key_down(C93_KEY_LEFT) || key_down('A')) paddle_x -= PADDLE_SPEED;
    if (key_down(C93_KEY_RIGHT) || key_down('D')) paddle_x += PADDLE_SPEED;
#endif
    paddle_x = clamp(paddle_x, paddle_w / 2, W - 1 - paddle_w / 2);

    if (state == STATE_SERVE)
    {
        balls[0].x = paddle_x << FP;
        for (int k = 0; k < TRAIL; k++) balls[0].tx[k] = paddle_x;
#ifdef C93_TEST_FRAMES
        if (state_timer > 20) launch_ball();
#else
        if (key_pressed(C93_KEY_SPACE)) launch_ball();
#endif
        return;
    }

    int alive = 0;
    for (int i = 0; i < MAX_BALLS; i++)
    {
        if (!balls[i].active) continue;
        update_ball(&balls[i]);
        alive += balls[i].active;
    }
    update_pickups();

    if (bricks_left == 0)
    {
        state = STATE_LEVEL_CLEAR;
        state_timer = 0;
        score += 500 * level;
        return;
    }

    if (alive == 0)
    {
        lives--;
        shake = 12;
        if (lives <= 0)
        {
            if (score > best_score) best_score = score;
            state = STATE_GAME_OVER;
            state_timer = 0;
        }
        else
        {
            wide_timer = slow_timer = 0;
            reset_balls_on_paddle();
            state = STATE_SERVE;
            state_timer = 0;
        }
    }
}

/* ------------------------------------------------------------------------------------------ drawing */

void draw_background(void)
{
    for (int band = 0; band < 27; band++)               /* vertical gradient */
        c93_fill_rect(PAGE, 0, band * 10, W, 10, COL_BG + band * 31 / 26);

    for (int i = 0; i < MAX_STARS; i++)                 /* twinkling stars */
    {
        int t = (stars[i].phase + frame) & 31;
        int level16 = t < 16 ? t : 31 - t;
        c93_plot(PAGE, stars[i].x, stars[i].y, COL_STAR + level16);
        if (level16 > 12)
        {
            c93_plot(PAGE, stars[i].x - 1, stars[i].y, COL_STAR + 6);
            c93_plot(PAGE, stars[i].x + 1, stars[i].y, COL_STAR + 6);
        }
    }

    /* Synthwave floor: lines rushing towards the viewer and rays from the vanishing point */
    int horizon = 178;
    c93_fill_rect(PAGE, 0, horizon, W, 1, COL_GRID_HI);
    for (int k = 0; k < 8; k++)
    {
        int t = (k * 16 + frame % 16);                  /* 0..127 */
        int y = horizon + t * t / 180;
        if (y < H) c93_fill_rect(PAGE, 0, y, W, 1, t > 64 ? COL_GRID_HI : COL_GRID);
    }
    for (int k = -9; k <= 9; k++)
        c93_line(PAGE, W / 2 + k * 14, horizon, W / 2 + k * 70, H, COL_GRID);
}

void draw_brick(int r, int c)
{
    int hp = brick_hp[r][c];
    if (!hp) return;

    int x = BRICKS_X + c * (BRICK_W + BRICK_GAP) + shake_x;
    int y = BRICKS_Y + r * (BRICK_H + BRICK_GAP) + shake_y;
    int base = COL_BRICK + brick_hue[r][c] * 4;

    if (brick_flash[r][c])
    {
        c93_fill_round_rect(PAGE, x, y, BRICK_W, BRICK_H, COL_WHITE, 3);
        return;
    }

    c93_fill_round_rect(PAGE, x, y + 1, BRICK_W, BRICK_H, base + 3, 3);        /* drop shadow */
    c93_fill_round_rect(PAGE, x, y, BRICK_W, BRICK_H - 1, base, 3);            /* body */
    c93_fill_rect(PAGE, x + 3, y + 2, BRICK_W - 6, 2, base + 1);               /* glossy highlight */
    c93_fill_rect(PAGE, x + 3, y + BRICK_H - 4, BRICK_W - 6, 1, base + 2);     /* lower bevel */

    for (int k = 1; k < hp; k++)                                               /* armor rivets */
        c93_fill_rect(PAGE, x + BRICK_W / 2 - 7 + k * 5, y + 6, 3, 2, base + 3);
}

void draw_bricks(void)
{
    for (int r = 0; r < ROWS; r++)
        for (int c = 0; c < COLS; c++)
            draw_brick(r, c);
}

void draw_paddle(void)
{
    int x = paddle_x - paddle_w / 2 + shake_x;
    int y = PADDLE_Y + shake_y;
    int glow = paddle_flash > 0 ? 7 : 3 + (frame / 6) % 2;

    c93_fill_round_rect(PAGE, x - 3, y - 2, paddle_w + 6, PADDLE_H + 4, COL_CYAN + glow - 2, 5);
    c93_fill_round_rect(PAGE, x, y, paddle_w, PADDLE_H, COL_CYAN + glow, 4);
    c93_fill_rect(PAGE, x + 5, y + 1, paddle_w - 10, 2, COL_WHITE);
    c93_fill_rect(PAGE, x + 2, y + PADDLE_H - 2, 4, 2, COL_PINK);              /* thruster lights */
    c93_fill_rect(PAGE, x + paddle_w - 6, y + PADDLE_H - 2, 4, 2, COL_PINK);
}

void draw_balls(void)
{
    for (int i = 0; i < MAX_BALLS; i++)
    {
        Ball *b = &balls[i];
        if (!b->active) continue;
        for (int k = TRAIL - 1; k >= 1; k--)
            c93_fill_ellipse(PAGE, b->tx[k] + shake_x, b->ty[k] + shake_y, 3 - k / 2, 3 - k / 2, COL_CYAN + 6 - k);
        int x = (b->x >> FP) + shake_x, y = (b->y >> FP) + shake_y;
        c93_fill_ellipse(PAGE, x, y, BALL_R + 1, BALL_R + 1, COL_CYAN + 4);
        c93_fill_ellipse(PAGE, x, y, BALL_R, BALL_R, COL_WHITE);
    }
}

void draw_effects(void)
{
    for (int i = 0; i < MAX_PARTS; i++)
    {
        Particle *p = &parts[i];
        if (p->life <= 0) continue;
        int color = p->life > 22 ? COL_SPARK + 15 : p->life > 12 ? COL_BRICK + p->hue * 4 + 1 : COL_BRICK + p->hue * 4;
        int size = p->life > 20 ? 3 : 2;
        c93_fill_rect(PAGE, (p->x >> FP) + shake_x, (p->y >> FP) + shake_y, size, size, color);
    }

    for (int i = 0; i < MAX_POPUPS; i++)
    {
        if (popups[i].life <= 0) continue;
        char text[12];
        text[0] = '+';
        format_int(text + 1, popups[i].value, 1);
        c93_draw_text(PAGE, popups[i].x + 1, popups[i].y + 1, 1, COL_BLACK, text);
        c93_draw_text(PAGE, popups[i].x, popups[i].y, 1, popups[i].life > 10 ? COL_YELLOW : COL_GREY, text);
    }

    static const char letters[4] = { 'W', 'M', 'S', '+' };
    static const int colors[4] = { COL_CYAN + 6, COL_PINK, COL_YELLOW, COL_GREEN };
    for (int i = 0; i < MAX_PICKUPS; i++)
    {
        Pickup *p = &pickups[i];
        if (!p->active) continue;
        int x = p->x - 11, y = (p->y >> FP) - 5;
        char label[2];
        label[0] = letters[p->type];
        label[1] = 0;
        c93_fill_round_rect(PAGE, x - 1, y - 1, 24, 12, COL_WHITE, 5);
        c93_fill_round_rect(PAGE, x, y, 22, 10, colors[p->type], 4);
        c93_draw_text(PAGE, x + 8, y + 1, 1, COL_BLACK, label);
    }
}

void draw_hud(void)
{
    char buf[16];
    c93_fill_rect(PAGE, 0, 0, W, HUD_H, COL_BLACK);
    c93_fill_rect(PAGE, 0, HUD_H, W, 1, COL_PINK);
    c93_fill_rect(PAGE, 0, HUD_H + 1, W, 1, COL_GRID);

    c93_draw_text(PAGE, 8, 8, 1, COL_GREY, "SCORE");
    format_int(buf, score, 6);
    c93_draw_text(PAGE, 46, 4, 2, COL_WHITE, buf);

    c93_draw_text(PAGE, 200, 8, 1, COL_GREY, "LEVEL");
    format_int(buf, level, 2);
    c93_draw_text(PAGE, 238, 4, 2, COL_YELLOW, buf);

    for (int i = 0; i < lives && i < 9; i++)
        c93_fill_round_rect(PAGE, W - 22 - i * 22, 8, 18, 6, COL_CYAN + 6, 3);

    if (wide_timer > 0 && (wide_timer > 120 || frame % 8 < 4))
        c93_draw_text(PAGE, 300, 8, 1, COL_CYAN + 7, "WIDE");
    if (slow_timer > 0 && (slow_timer > 120 || frame % 8 < 4))
        c93_draw_text(PAGE, 330, 8, 1, COL_YELLOW, "SLOW");
}

/* Draws text centered horizontally, one hue per letter, with a drop shadow */
void draw_rainbow_text(int y, int scale, const char *text, int wave)
{
    int x = (W - c93_text_width(text, scale)) / 2;
    int shadow = scale / 3 + 1;
    char one[2];
    one[1] = 0;
    for (int i = 0; text[i]; i++, x += 6 * scale)
    {
        int dy = wave ? ((frame * 3 + i * 20) % 64 < 32 ? (frame * 3 + i * 20) % 32 : 31 - (frame * 3 + i * 20) % 32) / 8 : 0;
        one[0] = text[i];
        c93_draw_text(PAGE, x + shadow, y + dy + shadow, scale, COL_BRICK + ((i + frame / 8) % 8) * 4 + 3, one);
        c93_draw_text(PAGE, x, y + dy, scale, COL_BRICK + ((i + frame / 8) % 8) * 4, one);
    }
}

void draw_centered(int y, int scale, int color, const char *text)
{
    c93_draw_text(PAGE, (W - c93_text_width(text, scale)) / 2, y, scale, color, text);
}

void draw_title(void)
{
    draw_background();

    /* A decorative wall of bricks drifting behind the logo */
    for (int c = 0; c < 14; c++)
    {
        int x = c * 36 - (frame % 36);
        int hue = (c + frame / 36) % 8;
        c93_fill_round_rect(PAGE, x, 196, 32, 10, COL_BRICK + hue * 4, 3);
        c93_fill_rect(PAGE, x + 3, 198, 26, 2, COL_BRICK + hue * 4 + 1);
    }

    draw_rainbow_text(30, 9, "NEON", 1);
    draw_rainbow_text(104, 5, "RICOCHET", 1);

    if ((frame / 30) % 2 == 0)
        draw_centered(152, 2, COL_WHITE, "PRESS SPACE TO START");
    draw_centered(174, 1, COL_GREY, "ARROWS OR A/D TO MOVE - SPACE TO LAUNCH - P PAUSE - ESC QUIT");

    if (best_score > 0)
    {
        char buf[24];
        strcpy(buf, "BEST ");
        format_int(buf + 5, best_score, 6);
        draw_centered(220, 2, COL_YELLOW, buf);
    }
    draw_centered(256, 1, COL_GRID_HI, "WRITTEN IN C - COMPILED BY CONTINUUM 93");
}

void draw_game(void)
{
    draw_background();
    draw_bricks();
    draw_paddle();
    draw_balls();
    draw_effects();
    draw_hud();

    if (state == STATE_SERVE && (frame / 20) % 2 == 0)
        draw_centered(210, 1, COL_WHITE, "PRESS SPACE TO LAUNCH");

    if (state == STATE_LEVEL_CLEAR)
    {
        draw_rainbow_text(104, 5, "LEVEL CLEAR", 1);
        char buf[24];
        strcpy(buf, "BONUS ");
        format_int(buf + 6, 500 * level, 1);
        draw_centered(150, 2, COL_YELLOW, buf);
    }

    if (state == STATE_GAME_OVER)
    {
        c93_fill_rect(PAGE, 0, 92, W, 90, COL_BLACK);
        c93_fill_rect(PAGE, 0, 92, W, 1, COL_PINK);
        c93_fill_rect(PAGE, 0, 181, W, 1, COL_PINK);
        draw_rainbow_text(100, 6, "GAME OVER", 0);
        char buf[24];
        strcpy(buf, "SCORE ");
        format_int(buf + 6, score, 6);
        draw_centered(148, 2, COL_WHITE, buf);
        if ((frame / 30) % 2 == 0)
            draw_centered(168, 1, COL_GREY, "PRESS SPACE");
    }

    if (paused)
    {
        c93_fill_rect(PAGE, 150, 115, 180, 40, COL_BLACK);
        c93_rect(PAGE, 150, 115, 180, 40, COL_CYAN + 6);
        draw_centered(125, 3, COL_WHITE, "PAUSED");
    }
}

/* ------------------------------------------------------------------------------------------ main loop */

void init(void)
{
    c93_set_video_pages(1);
    setup_palette();
    c93_set_buffer_mode(0);                            /* we present frames ourselves */
#ifdef C93_TEST_FRAMES
#ifndef C93_TEST_SEED
#define C93_TEST_SEED 1
#endif
    srand(C93_TEST_SEED);                              /* deterministic runs for the test build */
#else
    srand(c93_millis() | 1);
#endif

    for (int i = 0; i < MAX_STARS; i++)
    {
        stars[i].x = random_range(0, W - 1);
        stars[i].y = random_range(HUD_H + 2, 170);
        stars[i].phase = random_range(0, 31);
    }
    memset(keys, 0, sizeof(keys));
    level = 1;
    state = STATE_TITLE;
}

/* Returns 0 to keep running, 1 to quit */
int tick(void)
{
#ifndef C93_TEST_FRAMES
    /* The headless test build never reads the host keyboard (the autopilot plays instead) */
    memcpy(prev_keys, keys, sizeof(keys));
    c93_read_keys(keys);
    if (key_pressed(C93_KEY_ESCAPE))
        return 1;
#endif

    frame++;
    state_timer++;

    switch (state)
    {
        case STATE_TITLE:
            if (key_pressed(C93_KEY_SPACE) || key_pressed(C93_KEY_ENTER))
                new_game();
            draw_title();
            return 0;

        case STATE_SERVE:
        case STATE_PLAY:
            if (key_pressed('P'))
                paused = !paused;
            if (!paused)
                update_play();
            break;

        case STATE_LEVEL_CLEAR:
            if (state_timer > 120)
                start_level(level + 1);
            break;

        case STATE_GAME_OVER:
#ifdef C93_TEST_FRAMES
            if (state_timer > 30) new_game();
#else
            if (state_timer > 30 && key_pressed(C93_KEY_SPACE))
                state = STATE_TITLE;
#endif
            break;
    }

    if (!paused)
        update_effects();
    draw_game();
    return 0;
}

#ifdef C93_TEST_FRAMES
int frames_run;

int main(void)
{
    init();
    new_game();
    for (frames_run = 0; frames_run < C93_TEST_FRAMES; frames_run++)
    {
        tick();
        c93_present(1);
    }
    return score;
}
#else
int main(void)
{
    init();
    unsigned int next = c93_millis();

    for (;;)
    {
        if (tick())
            break;
        c93_present(1);

        /* ~60 frames per second: 16, 17, 17 ms */
        next += frame % 3 == 0 ? 16 : 17;
        unsigned int now = c93_millis();
        if ((int)(now - next) > 100)
            next = now;                                /* fell far behind (e.g. debugger pause): resync */
        while ((int)(next - c93_millis()) > 0)
            ;
    }

    c93_set_buffer_mode(0xFF);
    return 0;
}
#endif
