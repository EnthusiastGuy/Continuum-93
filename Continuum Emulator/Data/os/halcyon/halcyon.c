/*
 * HALCYON - a visual desktop operating system for the Continuum 93 fantasy computer.
 *
 * Written in C and compiled by the machine's own C compiler. Started by the boot selector
 * (Data/os/boot/boot.c), which also runs programs on Halcyon's behalf - see bootproto.h.
 *
 * Highlights
 *   - four video layers: palette-cycled backdrop, desktop icons, windows, and a top layer for the
 *     menu bar, dock, menus, pointer and effects
 *   - side-rail windows that snap magnetically and fold across themselves like an accordion
 *   - Drawers (files), Reader, Pulse (system monitor), Calc, Sketch, Prefs and About
 *   - runs .asm, .c and .bas programs, and resumes exactly where it was afterwards
 *
 * Build variants: define HAL_SCRIPT to run headless with scripted input for HAL_FRAMES frames (used by
 * the unit tests and screenshot renders).
 */

#pragma c93 origin 0x000000
#pragma c93 stack 0x04F000

#include <continuum.h>
#include <string.h>
#include <stdlib.h>
#include "../boot/bootproto.h"
#include "hal_core.h"

/* ---------------------------------------------------------------- shared state */

int frame;
unsigned int now_ms;
int mouse_x = SCREEN_W / 2, mouse_y = SCREEN_H / 2, mouse_b, mouse_pb;
int clicked, released, dbl_clicked, rclicked;
unsigned char keys[32], prev_keys[32];
int desk_dirty = 1, win_dirty = 1;
Prefs prefs;

unsigned int last_click_ms;
int last_click_x, last_click_y;
int exit_requested, exit_t;
int backdrop_row = SCREEN_H;              /* < SCREEN_H while the backdrop is being (re)painted */
unsigned int os_alive;
#define OS_ALIVE 0x484C4359u

int key_pressed(int k) { return c93_key_down(keys, k) && !c93_key_down(prev_keys, k); }
int key_down(int k) { return c93_key_down(keys, k); }

#include "hal_gfx.c"
#include "hal_backdrop.c"
#include "hal_fx.c"
#include "hal_wm.c"
#include "hal_apps.c"
#include "hal_desk.c"

/* ---------------------------------------------------------------- preferences */

void default_prefs(void)
{
    prefs.magic = PREFS_MAGIC;
    prefs.theme = 0;
    prefs.backdrop = 0;
    prefs.trail = 1;
    prefs.snap = 1;
    prefs.rail_right = 0;
}

void load_prefs(void)
{
    default_prefs();
#ifndef HAL_SCRIPT
    if (c93_file_exists(PREFS_FILE) && c93_file_size(PREFS_FILE) == sizeof(Prefs))
    {
        c93_load_file(PREFS_FILE, &prefs);
        if (prefs.magic != PREFS_MAGIC || prefs.theme < 0 || prefs.theme > 5 || prefs.backdrop < 0 || prefs.backdrop > 2)
            default_prefs();
    }
#endif
}

void save_prefs(void)
{
#ifndef HAL_SCRIPT
    c93_save_file(PREFS_FILE, &prefs, sizeof(Prefs));
#endif
}

void apply_theme(void)
{
    build_ui_palette();
    for (int i = 0; i < 6; i++)
        ui_set(K_SWATCH + i, theme_acc[i][0], theme_acc[i][1], theme_acc[i][2]);
    apply_palettes();
    backdrop_static_palette();
    desk_dirty = win_dirty = 1;
}

void rebuild_backdrop(void)
{
    backdrop_row = 0;                       /* repainted progressively by the main loop */
    toast(backdrop_names[prefs.backdrop], ACC(9));
}

/* ---------------------------------------------------------------- requests to the boot selector */

void begin_exit(int command)
{
    BootMailbox *mb = BOOT_MAILBOX;
    mb->magic = BOOT_MAGIC;
    mb->command = command;
    mb->image_end = (unsigned int)__heap_start;
    exit_requested = 1;
    exit_t = 0;
}

void request_run(const char *path)
{
    BootMailbox *mb = BOOT_MAILBOX;
    strcpy(mb->path, path);
    begin_exit(BOOT_CMD_RUN);
}

void request_boot_menu(void) { begin_exit(BOOT_CMD_MENU); }
void request_shutdown(void) { begin_exit(BOOT_CMD_SHUTDOWN); }

/* ---------------------------------------------------------------- setup */

void setup_video(void)
{
    c93_set_video_pages(4);
    c93_set_layers_visible(0xFF);
    c93_set_buffer_mode(0);                 /* Halcyon presents every frame itself */
    c93_clear(P_BACK, 1);
    c93_clear(P_DESK, 0);
    c93_clear(P_WIN, 0);
    c93_clear(P_TOP, 0);
#ifndef HAL_SCRIPT
    c93_set_host_cursor(0);
#endif
}

void load_fonts(void)
{
    c93_load_font("fonts/ModernDOS 8x8.png", WS_FONT_S);
    c93_load_font("fonts/ModernDOS-9x13.png", WS_FONT_M);
    c93_load_font("../fonts/ModernDOS-12x18.png", WS_FONT_L);
    font_ptr[F_S] = WS_FONT_S;
    font_ptr[F_M] = WS_FONT_M;
    font_ptr[F_L] = WS_FONT_L;
}

void reload_drawers(void)
{
    for (int i = 0; i < MAX_WIN; i++)
    {
        Win *w = &wins[i];
        if (!w->used) continue;
        if (w->app == APP_DRAWERS)
        {
            int sel = w->sel, scroll = w->scroll;
            load_dir(w);
            w->sel = sel < w->count ? sel : -1;
            w->scroll = scroll;
        }
        if (w->app == APP_READER && w == reader_win && reader_path[0])
        {
            int scroll = w->scroll;
            char path[160];
            strcpy(path, reader_path);
            open_reader(path, w->x, w->y);
            w->scroll = scroll;
        }
    }
}

/* ---------------------------------------------------------------- input */

#ifdef HAL_SCRIPT
#ifndef HAL_FRAMES
#define HAL_FRAMES 200
#endif
/* Scripted session used by the tests: opens apps, drags a window until it snaps, folds another. */
int script_base = -1;
int test_probe[8];                          /* values recorded by the scripts for the unit tests */
#ifndef HAL_SCENE
#define HAL_SCENE 1
#endif
void script_input(void)
{
    if (script_base < 0) script_base = frame;
    int f = frame - script_base;
    mouse_b = 0;
#if HAL_SCENE == 2
    /* apps: Calc with a sum, Sketch with strokes, Prefs, and the Halcyon menu open */
    if (f == 2) { Win *w = open_app(APP_SKETCH, 100, 250); if (w) { w->x = 6; w->y = 120; } }
    if (f == 4) { Win *w = open_app(APP_CALC, 200, 250); if (w) { w->x = 300; w->y = 20; } }
    if (f == 6) { Win *w = open_app(APP_PREFS, 200, 250); if (w) { w->x = 30; w->y = 18; } }
    if (f == 8) { calc_press(13); calc_press(16); calc_press(7); calc_press(14); calc_press(19); }
    if (f >= 10 && f < 40) { Win *w = find_app(APP_SKETCH); int x, y, cw, ch; win_content(w, &x, &y, &cw, &ch);
        mouse_x = x + 20 + (f - 10) * 5; mouse_y = y + 40 + isin((f - 10) * 16) / 6; mouse_b = 1; if (f == 10) { focus_window(w); } }
    if (f == 10) { Win *w = find_app(APP_SKETCH); sketch_color = 2; sketch_brush = 3; (void)w; }
    if (f == 20) { sketch_color = 7; }
    if (f == 30) { sketch_color = 4; }
    if (f >= 40 && f < 42) mouse_b = 0;
    if (f == 45) { mouse_x = 26; mouse_y = 5; }
    if (f == 46) mouse_b = 1;
    if (f >= 47) { mouse_x = 40; mouse_y = 44; }
    return;
#elif HAL_SCENE == 3
    /* Reader showing this very source file, plus a drawer with its right-click menu */
    if (f == 2) { open_reader("../os/halcyon/hal_wm.c", 200, 120); if (reader_win) { reader_win->x = 150; reader_win->y = 30; reader_win->w = 320; reader_win->h = 196; reader_win->scroll = 140; } }
    if (f == 4) { Win *w = open_drawer("programs", 30, 60); if (w) { w->x = 8; w->y = 40; w->rail_right = 1; } }
    if (f == 12) { Win *w = find_app(APP_DRAWERS); w->sel = 4; mouse_x = 70; mouse_y = 104; }
    if (f == 13) { mouse_b = 2; }
    if (f >= 14) { mouse_x = 90; mouse_y = 118; }
    test_probe[2] = popup_open;
    test_probe[3] = reader_lines;
    return;
#elif HAL_SCENE == 4
    /* run a program: Halcyon must hand the request to the boot selector through the mailbox */
    if (f == 3) open_drawer("programs/neon-ricochet", 100, 100);
    if (f == 6)
    {
        Win *w = find_app(APP_DRAWERS);
        for (int i = 0; i < w->count; i++)
            if (w->kinds[i] == FK_C) drawer_open_entry(w, i, 0);
    }
    return;
#endif
    if (f == 3) open_drawer("programs", 30, 60);
    if (f == 6) open_app(APP_PULSE, 240, 250);
    if (f == 9) open_app(APP_ABOUT, 260, 250);
    if (f == 12) open_app(APP_CALC, 300, 250);
    if (f == 40 && find_app(APP_CALC)) toggle_fold(find_app(APP_CALC));
    if (f == 50) { Win *w = find_app(APP_PULSE); if (w) { w->x = 180; w->y = 80; win_dirty = 1; } }
    /* drag the About window by its rail towards Pulse: it should snap to its edge */
    Win *about = find_app(APP_ABOUT);
    if (about && f >= 60 && f < 90)
    {
        if (f == 60) { mouse_x = about->x + 8; mouse_y = about->y + 60; }
        mouse_b = 1;
        if (f > 62) { mouse_x += 3; mouse_y += 1; }
    }
    if (f == 89) test_probe[0] = snap_gx + 1;                       /* > 0: About snapped to a guide */
    if (f >= 90 && f < 92) mouse_b = 0;
    if (f == 100) { Win *d = find_app(APP_DRAWERS); if (d) { d->sel = 2; focus_window(d); } }
    if (f >= 110) { mouse_x = 300; mouse_y = 200; }
    if (find_app(APP_CALC)) test_probe[1] = find_app(APP_CALC)->folded;
}
#endif

void read_input(void)
{
    mouse_pb = mouse_b;
    memcpy(prev_keys, keys, 32);
#ifdef HAL_SCRIPT
    script_input();
#else
    c93_read_keys(keys);
    int x, y;
    mouse_b = c93_read_mouse(&x, &y);
    if (x >= 0 && x < SCREEN_W && y >= 0 && y < SCREEN_H)
    {
        mouse_x = x;
        mouse_y = y;
    }
#endif
    clicked = (mouse_b & 1) && !(mouse_pb & 1);
    released = !(mouse_b & 1) && (mouse_pb & 1);
    rclicked = (mouse_b & 2) && !(mouse_pb & 2);
    dbl_clicked = 0;
    if (clicked)
    {
        if (now_ms - last_click_ms < 400 && abs(mouse_x - last_click_x) < 5 && abs(mouse_y - last_click_y) < 5)
        {
            dbl_clicked = 1;
            last_click_ms = 0;
        }
        else
        {
            last_click_ms = now_ms;
        }
        last_click_x = mouse_x;
        last_click_y = mouse_y;
    }
}

void handle_input(void)
{
    if (exit_requested)
        return;

    if (key_pressed(C93_KEY_ESCAPE))
    {
        menu_open = -1;
        popup_open = 0;
    }

    if (!menus_input())
        if (!dock_input())
            if (!wm_input())
                desktop_input();

    if (clicked)
        ripple(mouse_x, mouse_y);

    /* keyboard: Tab cycles windows, everything else goes to the active window */
    if (key_pressed(C93_KEY_TAB) && zcount > 1)
        focus_window(&wins[zorder[0]]);
    else if (menu_open < 0 && !popup_open)
    {
        Win *a = active_window();
        if (a && !a->folded && a->open_t == 256)
            app_key(a);
    }
}

/* ---------------------------------------------------------------- frames */

void draw_top(void)
{
    c93_clear(P_TOP, 0);
    draw_menubar();
    draw_dock();
    draw_fx();
    draw_menus();
    if (exit_requested)
        crt_frame(exit_t, 1);
    draw_pointer();
}

void render_frame(void)
{
    if (backdrop_row < SCREEN_H)
    {
        build_backdrop_rows(backdrop_row, backdrop_row + 30);
        backdrop_row += 30;
    }
    if (desk_dirty) { draw_desktop(); desk_dirty = 0; }
    if (win_dirty) { draw_windows(); win_dirty = 0; }
    draw_top();
    animate_ui_palette();
    apply_palettes();
    animate_backdrop();
    c93_present(ALL_PAGES);
}

unsigned int next_frame_ms;

void wait_frame(void)
{
#ifndef HAL_SCRIPT
    next_frame_ms += frame % 3 == 0 ? 16 : 17;
    if ((int)(c93_millis() - next_frame_ms) > 100)
        next_frame_ms = c93_millis();
    while ((int)(next_frame_ms - c93_millis()) > 0)
        ;
#endif
}

/* Boot splash: the backdrop is painted progressively behind a glowing logo and expanding rings */
void boot_splash(void)
{
    int steps = 36;
    for (int s = 0; s <= steps; s++)
    {
        now_ms = c93_millis();
        frame++;
        brightness = s * 256 / steps;
        build_backdrop_rows(s * SCREEN_H / (steps + 1), (s + 1) * SCREEN_H / (steps + 1));
        c93_clear(P_TOP, 0);
        int cx = SCREEN_W / 2, cy = SCREEN_H / 2 - 10;
        for (int r = 0; r < 3; r++)
        {
            int rad = (s * 6 + r * 40) % 160;
            c93_ellipse(P_TOP, cx, cy, rad + 20, rad / 2 + 10, r == 0 ? K_SNAP : ACC(6 + r * 2));
        }
        c93_fill_round_rect(P_TOP, cx - 96, cy - 22, 192, 48, N(1), 10);
        c93_round_rect(P_TOP, cx - 96, cy - 22, 192, 48, K_PULSE, 10);
        txt_rainbow(P_TOP, F_L, cx - 52, cy - 14, "HALCYON", 1);
        txt_center(P_TOP, F_S, cx, cy + 10, N(11), "waking up the desktop");
        c93_fill_rect(P_TOP, cx - 60, cy + 30, 120, 3, N(3));
        c93_fill_rect(P_TOP, cx - 60, cy + 30, 120 * s / steps, 3, K_SNAP);
        animate_ui_palette();
        apply_palettes();
        backdrop_static_palette();
        animate_backdrop();
        c93_present(ALL_PAGES);
        wait_frame();
    }
    brightness = 256;
    apply_theme();
}

/* Old-TV switch-on effect after coming back from a program */
void crt_on(void)
{
    for (int t = 0; t <= 16; t++)
    {
        now_ms = c93_millis();
        frame++;
        if (backdrop_row < SCREEN_H)
        {
            build_backdrop_rows(backdrop_row, backdrop_row + 18);
            backdrop_row += 18;
        }
        if (desk_dirty) { draw_desktop(); desk_dirty = 0; }
        if (win_dirty) { draw_windows(); win_dirty = 0; }
        c93_clear(P_TOP, 0);
        draw_menubar();
        draw_dock();
        crt_frame(t, 0);
        animate_ui_palette();
        apply_palettes();
        animate_backdrop();
        c93_present(ALL_PAGES);
        wait_frame();
    }
}

int main(void)
{
    BootMailbox *mb = BOOT_MAILBOX;
    int warm = mb->magic == BOOT_MAGIC && mb->resume && os_alive == OS_ALIVE;

    exit_requested = 0;
    setup_video();
    load_fonts();
    build_pointers();
    init_sine();
    now_ms = c93_millis();
    next_frame_ms = now_ms;

    if (!warm)
    {
        memset(wins, 0, sizeof(wins));
        zcount = 0;
        load_prefs();
        apply_theme();
        for (int i = 0; i < DOCK_ITEMS; i++) dock_size[i] = 21 * 16;
        dock_layout();
        brightness = 0;
        boot_splash();
        backdrop_row = SCREEN_H;
        toast("Welcome to Halcyon", ACC(9));
        if (mb->magic == BOOT_MAGIC && mb->reopen[0])
        {
            /* the last program overwrote the OS, which was rebuilt: show its folder again */
            char dir[200];
            strcpy(dir, mb->reopen);
            int n = strlen(dir);
            while (n > 0 && dir[n - 1] != '/') n--;
            dir[n > 0 ? n - 1 : 0] = 0;
            open_drawer(dir, SCREEN_W / 2, SCREEN_H / 2);
            mb->reopen[0] = 0;
        }
    }
    else
    {
        apply_theme();
        backdrop_row = 0;
        reload_drawers();
        desk_dirty = win_dirty = 1;
        crt_on();
        if (mb->last_result == 0) toast("Welcome back", ACC(9));
        else if (mb->last_result < 0) toast("The program could not be found", K_RED);
        else
        {
            char msg[64], num[8];
            fmt_int(num, mb->last_result, 0);
            strcpy(msg, "Build failed: ");
            strcat(msg, num);
            strcat(msg, " error(s) - see its debug folder");
            toast(msg, K_RED);
        }
    }
    os_alive = OS_ALIVE;
    if (mb->magic == BOOT_MAGIC) mb->command = BOOT_CMD_NONE;

    for (;;)
    {
        unsigned int t0 = c93_millis();
        now_ms = t0;
        frame++;

        read_input();
        handle_input();
        update_fx();
        dock_update();
        pointer_trail();
        wm_tick();
        render_frame();

        perf_busy[perf_pos] = (int)(c93_millis() - t0);
        perf_pos = (perf_pos + 1) & 63;

        if (exit_requested && ++exit_t > 16)
            return 0;                       /* back to the boot selector, which reads the mailbox */
#ifdef HAL_SCRIPT
        if (frame - script_base >= HAL_FRAMES)
            return zcount;
#endif
        wait_frame();
    }
}
