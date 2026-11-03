/*
 * Continuum 93 boot selector
 *
 * Shown at power-on: choose between Halcyon (the default, started automatically after 3 seconds) and the
 * classic Q OS. It also acts as Halcyon's program runner - see bootproto.h for the protocol and memory map.
 * Written in C; compiled by the emulator at boot (Data/init.cfg: bootProgram=Data\os\boot\boot.c).
 */

#pragma c93 origin 0x070000
#pragma c93 stack 0x07F000

#include <continuum.h>
#include <string.h>
#include "bootproto.h"

#define HALCYON_PATH "../os/halcyon/halcyon.c"
#define QOS_PATH     "../os/q.asm"
#define AUTOBOOT_MS  3000

enum { C_BLACK = 1, C_TEXT = 2, C_DIM = 3, C_ACCENT = 4, C_GLOW = 5, C_PANEL = 6, C_PANEL_HI = 7, C_PEACH = 8,
       C_PURPLE = 9, C_SHADOW = 10, C_RED = 11, C_YELLOW = 12, C_GREEN = 13, C_BAR = 14,
       C_SKY = 32,          /* 32 sky shades */
       C_STAR = 80,         /* 8 twinkle levels */
       C_AURORA = 96 };     /* 32 aurora shades for the Halcyon card */

unsigned char font_small[3000];
unsigned char font_big[8000];
unsigned char keys[32], prev_keys[32];
int stars_x[60], stars_y[60];
int frame;

/* ------------------------------------------------------------------------------------------ helpers */

int key_pressed(int k) { return c93_key_down(keys, k) && !c93_key_down(prev_keys, k); }

void text(int x, int y, int color, const char *s) { c93_text(0, font_small, x, y, color, s, 480, 0, 0, 0); }

void text_center(unsigned char *font, int y, int color, const char *s, int outline)
{
    int w = c93_text_width_font(font, s);
    c93_text(0, font, (480 - w) / 2, y, color, s, 480, outline ? C93_TEXT_OUTLINE : 0, C_PURPLE, 0xFF);
}

void present_frame(unsigned int *next)
{
    c93_present(1);
    *next += frame % 3 == 0 ? 16 : 17;
    if ((int)(c93_millis() - *next) > 100)
        *next = c93_millis();
    while ((int)(*next - c93_millis()) > 0)
        ;
}

int sine8(int a)   /* triangle approximation of a sine, 0..255 -> -64..64 */
{
    a &= 255;
    if (a < 64) return a;
    if (a < 192) return 128 - a;
    return a - 256;
}

/* ------------------------------------------------------------------------------------------ palette */

void setup_palette(void)
{
    c93_set_color(0, C_BLACK, 4, 4, 12);
    c93_set_color(0, C_TEXT, 236, 238, 255);
    c93_set_color(0, C_DIM, 120, 128, 170);
    c93_set_color(0, C_ACCENT, 70, 230, 210);
    c93_set_color(0, C_PANEL, 24, 28, 56);
    c93_set_color(0, C_PANEL_HI, 44, 52, 96);
    c93_set_color(0, C_PEACH, 232, 206, 188);
    c93_set_color(0, C_PURPLE, 70, 54, 120);
    c93_set_color(0, C_SHADOW, 2, 2, 8);
    c93_set_color(0, C_RED, 255, 90, 110);
    c93_set_color(0, C_YELLOW, 255, 210, 90);
    c93_set_color(0, C_GREEN, 90, 230, 140);
    c93_set_color(0, C_BAR, 255, 120, 200);
    for (int i = 0; i < 32; i++)
        c93_set_color(0, C_SKY + i, 6 + i / 2, 6 + i / 3, 20 + i * 2);
    for (int i = 0; i < 32; i++)
        c93_set_color(0, C_AURORA + i, 20 + i * 2, 60 + i * 5, 90 + i * 4);
}

/* Per-frame palette animation: glowing selection, twinkling stars, flowing aurora */
void animate_palette(void)
{
    int pulse = 128 + sine8(frame * 4) * 2;
    c93_set_color(0, C_GLOW, 60 * pulse / 256 + 40, 230 * pulse / 256 + 20, 220 * pulse / 256 + 30);
    for (int i = 0; i < 8; i++)
    {
        int v = 90 + sine8(frame * 3 + i * 32) * 2;
        c93_set_color(0, C_STAR + i, v, v, v + 20 > 255 ? 255 : v + 20);
    }
    for (int i = 0; i < 32; i++)
    {
        int s = sine8(frame * 2 + i * 8);         /* -64..64 */
        c93_set_color(0, C_AURORA + i, 70 + s, 140 + s * 3 / 2, 150 - s / 2);
    }
}

/* ------------------------------------------------------------------------------------------ drawing */

void draw_backdrop(void)
{
    for (int band = 0; band < 30; band++)
        c93_fill_rect(0, 0, band * 9, 480, 9, C_SKY + band);
    for (int i = 0; i < 60; i++)
        c93_plot(0, stars_x[i], stars_y[i], C_STAR + (i & 7));
}

void draw_halcyon_preview(int x, int y)
{
    /* aurora sky with a tiny desktop: side-rail windows and a dock */
    for (int i = 0; i < 14; i++)
        c93_fill_rect(0, x, y + i * 5, 176, 5, C_AURORA + (i * 2 + frame / 3) % 32);
    c93_fill_rect(0, x, y + 70, 176, 14, C_PANEL);
    c93_fill_round_rect(0, x + 12, y + 12, 70, 44, C_PANEL_HI, 3);
    c93_fill_rect(0, x + 12, y + 12, 8, 44, C_ACCENT);
    c93_fill_rect(0, x + 14, y + 15, 4, 4, C_RED);
    c93_fill_rect(0, x + 14, y + 21, 4, 4, C_YELLOW);
    c93_fill_round_rect(0, x + 96, y + 26, 66, 38, C_PANEL_HI, 3);
    c93_fill_rect(0, x + 96, y + 26, 8, 38, C_BAR);
    for (int i = 0; i < 4; i++)
        c93_fill_rect(0, x + 26, y + 18 + i * 9, 40 + (i * 13) % 17, 3, C_DIM);
    for (int i = 0; i < 6; i++)
        c93_fill_round_rect(0, x + 46 + i * 15, y + 73, 10, 8, C_ACCENT + (i % 2) * 8, 2);
}

void draw_qos_preview(int x, int y)
{
    c93_fill_rect(0, x, y, 176, 84, C_PEACH);
    c93_fill_rect(0, x + 84, y, 92, 12, C_PURPLE);
    c93_fill_rect(0, x + 84, y, 2, 84, C_PURPLE);
    for (int i = 0; i < 6; i++)
        c93_fill_rect(0, x + 6, y + 6 + i * 12, 40 + (i * 23) % 30, 5, i == 2 ? C_PURPLE : C_DIM);
}

void draw_card(int x, int selected, const char *title, const char *line1, const char *line2, int halcyon)
{
    int lift = selected ? 3 : 0;
    int y = 72 - lift;
    c93_fill_round_rect(0, x + 4, y + 6 + lift, 200, 150, C_SHADOW, 8);
    if (selected)
        c93_fill_round_rect(0, x - 3, y - 3, 206, 156, C_GLOW, 10);
    c93_fill_round_rect(0, x, y, 200, 150, C_PANEL, 8);
    if (halcyon) draw_halcyon_preview(x + 12, y + 12);
    else draw_qos_preview(x + 12, y + 12);
    c93_rect(0, x + 12, y + 12, 176, 84, selected ? C_GLOW : C_PANEL_HI);
    c93_text(0, font_big, x + 12, y + 102, selected ? C_TEXT : C_DIM, title, 180, 0, 0, 0);
    text(x + 12, y + 124, C_DIM, line1);
    text(x + 12, y + 135, C_DIM, line2);
}

void draw_menu(int selected, int countdown_ms, int counting)
{
    draw_backdrop();
    text_center(font_big, 14, C_TEXT, "CONTINUUM 93", 1);
    text_center(font_small, 40, C_DIM, "Choose an operating system", 0);

    draw_card(30, selected == 0, "HALCYON", "Windows, apps and effects", "The new visual desktop", 1);
    draw_card(250, selected == 1, "Q OS", "The classic file browser", "Fast and minimal", 0);

    if (counting)
    {
        char msg[48];
        strcpy(msg, "Starting Halcyon in ");
        msg[20] = (char)('0' + (countdown_ms + 999) / 1000);
        msg[21] = 0;
        text_center(font_small, 236, C_TEXT, msg, 0);
        int w = 300 * countdown_ms / AUTOBOOT_MS;
        c93_fill_round_rect(0, 90, 250, 300, 6, C_PANEL_HI, 3);
        c93_fill_round_rect(0, 90, 250, w < 6 ? 6 : w, 6, C_GLOW, 3);
    }
    else
    {
        text_center(font_small, 236, C_TEXT, "Press ENTER to start the selected system", 0);
    }
    text_center(font_small, 260, C_DIM, "LEFT/RIGHT or mouse select   ENTER start   1/2 quick pick", 0);
}

/* Old TV switch-off: the picture collapses into a line, then a dot */
void crt_off(void)
{
    unsigned int next = c93_millis();
    for (int i = 0; i <= 12; i++)
    {
        int h = 135 * (12 - i) / 12;
        c93_fill_rect(0, 0, 0, 480, 135 - h, C_SHADOW);
        c93_fill_rect(0, 0, 135 + h, 480, 135 - h + 1, C_SHADOW);
        if (h < 3)
        {
            int w = 240 * (12 - i) / 6;
            c93_fill_rect(0, 0, 133, 480, 4, C_SHADOW);
            c93_fill_rect(0, 240 - w / 2, 134, w + 1, 2, C_TEXT);
        }
        frame++;
        present_frame(&next);
    }
}

void message_screen(const char *line1, const char *line2)
{
    draw_backdrop();
    text_center(font_big, 112, C_TEXT, line1, 1);
    text_center(font_small, 140, C_DIM, line2, 0);
    c93_present(1);
}

/* ------------------------------------------------------------------------------------------ running things */

int has_suffix(const char *s, const char *suffix)
{
    int n = strlen(s), m = strlen(suffix);
    if (n < m) return 0;
    for (int i = 0; i < m; i++)
    {
        int a = s[n - m + i], b = suffix[i];
        if (a >= 'A' && a <= 'Z') a += 32;
        if (a != b) return 0;
    }
    return 1;
}

/* Gives programs the same environment Q OS does: 3 freshly reset pages, presented automatically */
void prepare_program_video(void)
{
    c93_set_video_pages(3);
    c93_clear(0, 0);
    c93_clear(1, 0);
    c93_clear(2, 0);
    c93_set_layers_visible(0xFF);
    c93_set_buffer_mode(0xFF);
    c93_set_host_cursor(1);
}

int run_program(const char *path)
{
    int kind = has_suffix(path, ".c") ? C93_BUILD_C : has_suffix(path, ".bas") ? C93_BUILD_BASIC : C93_BUILD_ASM;
    int errors = 0;
    if (!c93_file_exists(path))
        return -1;
    prepare_program_video();
    int addr = c93_build(path, kind, &errors);
    if (addr < 0 || errors)
        return errors ? errors : -1;
    c93_call((unsigned int)addr);
    return 0;
}

unsigned int image_checksum(unsigned int end)
{
    unsigned int sum = 0x1234567u;
    for (unsigned int a = 0; a + 4 <= end; a += 32)
        sum = (sum << 5) + (sum >> 27) + *(unsigned int *)a;
    return sum;
}

void boot_qos(void)
{
    message_screen("Q OS", "Loading the classic operating system...");
    prepare_program_video();
    int errors = 0;
    int addr = c93_build(QOS_PATH, C93_BUILD_ASM, &errors);
    if (addr >= 0 && !errors)
        c93_call((unsigned int)addr);       /* Q OS normally never returns */
}

void boot_halcyon(void)
{
    BootMailbox *mb = BOOT_MAILBOX;
    int addr = -1, resume = 0;
    memset(mb, 0, sizeof(BootMailbox));
    mb->magic = BOOT_MAGIC;

    for (;;)
    {
        if (addr < 0)
        {
            int errors = 0;
            message_screen("HALCYON", "Building the desktop...");
            addr = c93_build(HALCYON_PATH, C93_BUILD_C, &errors);
            if (addr < 0 || errors)
            {
                message_screen("Halcyon failed to build", "See Data/os/halcyon/debug - returning to the menu");
                c93_wait_ms(2500);
                return;
            }
            resume = 0;
        }

        mb->command = BOOT_CMD_NONE;
        mb->resume = resume;
        c93_set_host_cursor(0);
        c93_call((unsigned int)addr);
        c93_set_host_cursor(1);

        if (mb->command == BOOT_CMD_RUN)
        {
            unsigned int before = image_checksum(mb->image_end);
            mb->last_result = run_program(mb->path);
            if (image_checksum(mb->image_end) != before)
            {
                addr = -1;                    /* the program overwrote the OS: rebuild it */
                strcpy(mb->reopen, mb->path);
            }
            resume = 1;
            continue;
        }
        if (mb->command == BOOT_CMD_SHUTDOWN)
        {
            asm("LD A, 0x20\nINT 0x00, A");
            for (;;)
                ;
        }
        return;                                /* BOOT_CMD_MENU or a plain return */
    }
}

/* ------------------------------------------------------------------------------------------ menu */

int choose_system(void)
{
    int selected = 0, counting = 1;
    unsigned int start = c93_millis(), next = start;
    int mx0 = -1, my0 = -1;

    for (;;)
    {
        int mx, my;
        memcpy(prev_keys, keys, 32);
#ifdef BOOT_TEST
        /* test build: no host input and no countdown, so the menu just keeps animating */
        int buttons = 0;
        mx = my = 0;
        counting = 1;
        start = c93_millis();
#else
        c93_read_keys(keys);
        int buttons = c93_read_mouse(&mx, &my);
#endif

        if (mx0 < 0) { mx0 = mx; my0 = my; }
        if (mx != mx0 || my != my0)
        {
            /* mouse moved: hovering a card selects it and stops the countdown */
            if (my >= 66 && my <= 222)
            {
                if (mx >= 30 && mx < 230) { selected = 0; counting = 0; }
                if (mx >= 250 && mx < 450) { selected = 1; counting = 0; }
            }
            mx0 = mx;
            my0 = my;
        }
        if ((buttons & 1) && my >= 66 && my <= 222 && ((mx >= 30 && mx < 230) || (mx >= 250 && mx < 450)))
            return mx < 240 ? 0 : 1;

        if (key_pressed(C93_KEY_LEFT) || key_pressed(C93_KEY_RIGHT) || key_pressed(C93_KEY_UP) || key_pressed(C93_KEY_DOWN))
        {
            selected = 1 - selected;
            counting = 0;
        }
        if (key_pressed('1')) return 0;
        if (key_pressed('2')) return 1;
        if (key_pressed(C93_KEY_ENTER) || key_pressed(C93_KEY_SPACE)) return selected;

        int remaining = AUTOBOOT_MS - (int)(c93_millis() - start);
        if (counting && remaining <= 0)
            return 0;

        frame++;
        animate_palette();
        draw_menu(selected, remaining < 0 ? 0 : remaining, counting);
        present_frame(&next);
    }
}

int main(void)
{
    c93_load_font("fonts/ModernDOS 8x8.png", font_small);
    c93_load_font("../fonts/ModernDOS-12x18.png", font_big);

    for (int i = 0; i < 60; i++)
    {
        stars_x[i] = (i * 97 + 13) % 480;
        stars_y[i] = (i * 53 + 7) % 270;
    }

    for (;;)
    {
        c93_set_video_pages(1);
        c93_set_layers_visible(0xFF);
        c93_set_buffer_mode(0);
        c93_set_host_cursor(1);
        setup_palette();
        memset(keys, 0, 32);

        int choice = choose_system();
        crt_off();
        if (choice == 0)
            boot_halcyon();
        else
            boot_qos();
    }
}
