/*
 * Halcyon OS - desktop: menu bar, menus, dock, desktop icons and the mouse pointer.
 */

void fold_all(int fold);
void tile_windows(void);
void menu_action_app(int tag, int item);

/* ---------------------------------------------------------------- menus */

#define MENU_COUNT 3
static const char *menu_titles[MENU_COUNT] = { "Halcyon", "Windows", "Go" };
static const char *menu_items0[9] = { "About Halcyon", "Preferences", "-", "Pulse monitor", "Calc", "Sketch", "-", "Restart...", "Shut down..." };
static const char *menu_items1[5] = { "Fold all", "Unfold all", "Tile windows", "-", "Close all" };
static const char *menu_items2[6] = { "System", "Programs", "Fonts", "Tests", "-", "Read me" };
static const int menu_sizes[MENU_COUNT] = { 9, 5, 6 };

int menu_open = -1;                 /* drop-down menu index, or -1 */
int menu_x[MENU_COUNT];

const char **popup_items;           /* right-click menu */
int popup_count, popup_tag, popup_x, popup_y, popup_open;

const char **menu_list(int m)
{
    return m == 0 ? menu_items0 : m == 1 ? menu_items1 : menu_items2;
}

void popup_menu(int x, int y, const char **items, int count, int tag)
{
    popup_items = items;
    popup_count = count;
    popup_tag = tag;
    popup_x = x + 140 > SCREEN_W ? SCREEN_W - 140 : x;
    popup_y = y + count * 11 + 6 > DOCK_Y ? DOCK_Y - count * 11 - 6 : y;
    popup_open = 1;
    menu_open = -1;
}

static int menu_width(const char **items, int count)
{
    int w = 60;
    for (int i = 0; i < count; i++)
    {
        int tw = txt_w(F_S, items[i]) + 20;
        if (tw > w) w = tw;
    }
    return w;
}

/* Draws a menu panel; returns the hovered item or -1 */
static int draw_menu_panel(int x, int y, const char **items, int count)
{
    int w = menu_width(items, count), h = count * 11 + 6, hovered = -1;
    c93_fill_rect(P_TOP, x + 3, y + 3, w, h, K_SHADOW);
    bevel(P_TOP, x, y, w, h, N(3), N(8), N(1));
    c93_fill_rect(P_TOP, x, y, 2, h, ACC(8));
    for (int i = 0; i < count; i++)
    {
        int iy = y + 3 + i * 11;
        if (items[i][0] == '-')
        {
            c93_fill_rect(P_TOP, x + 6, iy + 5, w - 12, 1, N(6));
            continue;
        }
        if (inside(mouse_x, mouse_y, x, iy, w, 11))
        {
            hovered = i;
            c93_fill_rect(P_TOP, x + 2, iy, w - 3, 11, K_SEL);
        }
        txt(P_TOP, F_S, x + 10, iy + 2, hovered == i ? K_WHITE : N(14), items[i]);
    }
    return hovered;
}

void menu_action(int m, int i)
{
    switch (m * 16 + i)
    {
        case 0: open_app(APP_ABOUT, mouse_x, mouse_y); break;
        case 1: open_app(APP_PREFS, mouse_x, mouse_y); break;
        case 3: open_app(APP_PULSE, mouse_x, mouse_y); break;
        case 4: open_app(APP_CALC, mouse_x, mouse_y); break;
        case 5: open_app(APP_SKETCH, mouse_x, mouse_y); break;
        case 7:
        case 8: open_app(APP_DIALOG, mouse_x, mouse_y); break;
        case 16: fold_all(1); break;
        case 17: fold_all(0); break;
        case 18: tile_windows(); break;
        case 20:
            for (int k = MAX_WIN - 1; k >= 0; k--)
                if (wins[k].used) close_window(&wins[k]);
            break;
        case 32: open_drawer("", mouse_x, mouse_y); break;
        case 33: open_drawer("programs", mouse_x, mouse_y); break;
        case 34: open_drawer("fonts", mouse_x, mouse_y); break;
        case 35: open_drawer("tests", mouse_x, mouse_y); break;
        case 37: open_reader("readme.txt", mouse_x, mouse_y); break;
    }
}

/* Returns 1 if the menus consumed this frame's mouse input */
int menus_input(void)
{
    if (popup_open)
    {
        if (clicked || rclicked)
        {
            int w = menu_width(popup_items, popup_count);
            int i = (mouse_y - popup_y - 3) / 11;
            popup_open = 0;
            if (inside(mouse_x, mouse_y, popup_x, popup_y, w, popup_count * 11 + 6) && i >= 0 && i < popup_count && popup_items[i][0] != '-')
                menu_action_app(popup_tag, i);
            return 1;
        }
        return 1;
    }

    if (mouse_y < BAR_H && clicked)
    {
        for (int m = 0; m < MENU_COUNT; m++)
            if (mouse_x >= menu_x[m] - 4 && mouse_x < menu_x[m] + txt_w(F_S, menu_titles[m]) + 4)
            {
                menu_open = menu_open == m ? -1 : m;
                return 1;
            }
        menu_open = -1;
        return 1;
    }

    if (menu_open >= 0)
    {
        /* hovering another title switches menus while one is open */
        if (mouse_y < BAR_H)
            for (int m = 0; m < MENU_COUNT; m++)
                if (mouse_x >= menu_x[m] - 4 && mouse_x < menu_x[m] + txt_w(F_S, menu_titles[m]) + 4)
                    menu_open = m;
        if (clicked)
        {
            const char **items = menu_list(menu_open);
            int count = menu_sizes[menu_open];
            int w = menu_width(items, count);
            int i = (mouse_y - BAR_H - 3) / 11;
            int m = menu_open;
            menu_open = -1;
            if (inside(mouse_x, mouse_y, menu_x[m] - 6, BAR_H, w, count * 11 + 6) && i >= 0 && i < count && items[i][0] != '-')
                menu_action(m, i);
            return 1;
        }
        return 1;
    }
    return mouse_y < BAR_H;
}

void draw_menubar(void)
{
    char buf[40], num[8];
    c93_fill_rect(P_TOP, 0, 0, SCREEN_W, BAR_H, N(2));
    c93_fill_rect(P_TOP, 0, BAR_H - 1, SCREEN_W, 1, ACC(7));
    c93_fill_rect(P_TOP, 0, BAR_H, SCREEN_W, 1, K_SHADOW);

    /* logo: a small prism */
    c93_fill_rect(P_TOP, 6, 3, 2, 8, K_RAINBOW + 0);
    c93_fill_rect(P_TOP, 8, 4, 2, 6, K_RAINBOW + 2);
    c93_fill_rect(P_TOP, 10, 5, 2, 4, K_RAINBOW + 4);
    c93_fill_rect(P_TOP, 12, 6, 2, 2, K_RAINBOW + 6);

    int x = 20;
    for (int m = 0; m < MENU_COUNT; m++)
    {
        int tw = txt_w(F_S, menu_titles[m]);
        menu_x[m] = x;
        if (menu_open == m)
            c93_fill_rect(P_TOP, x - 4, 0, tw + 8, BAR_H - 1, ACC(5));
        else if (mouse_y < BAR_H && mouse_x >= x - 4 && mouse_x < x + tw + 4)
            c93_fill_rect(P_TOP, x - 4, 0, tw + 8, BAR_H - 1, N(4));
        if (m == 0) txt_rainbow(P_TOP, F_S, x, 3, menu_titles[m], 0);
        else txt(P_TOP, F_S, x, 3, N(14), menu_titles[m]);
        x += tw + 14;
    }

    /* active window title in the middle */
    Win *a = active_window();
    if (a)
        txt_center(P_TOP, F_S, SCREEN_W / 2, 3, N(10), a->title);

    /* uptime clock */
    int secs = (int)(now_ms / 1000);
    fmt_2d(buf, secs / 3600 % 100);
    strcat(buf, ":");
    fmt_2d(num, secs / 60 % 60);
    strcat(buf, num);
    strcat(buf, (secs & 1) ? " " : ":");
    fmt_2d(num, secs % 60);
    strcat(buf, num);
    int cw = txt_w(F_S, "00:00:00");
    txt(P_TOP, F_S, SCREEN_W - cw - 8, 3, N(15), buf);
    txt(P_TOP, F_S, SCREEN_W - cw - 8 - txt_w(F_S, "16M RAM") - 12, 3, N(8), "16M RAM");
}

void draw_menus(void)
{
    if (menu_open >= 0)
        draw_menu_panel(menu_x[menu_open] - 6, BAR_H, menu_list(menu_open), menu_sizes[menu_open]);
    if (popup_open)
        draw_menu_panel(popup_x, popup_y, popup_items, popup_count);
}

/* ---------------------------------------------------------------- dock */

#define DOCK_ITEMS 7
static const int dock_apps[DOCK_ITEMS] = { APP_DRAWERS, APP_PULSE, APP_CALC, APP_SKETCH, APP_PREFS, APP_ABOUT, APP_DIALOG };
static const char *dock_names[DOCK_ITEMS] = { "Drawers", "Pulse", "Calc", "Sketch", "Prefs", "About", "Power" };
int dock_size[DOCK_ITEMS];          /* current icon sizes x 16 (animated) */
int dock_ix[DOCK_ITEMS], dock_iy[DOCK_ITEMS];
int dock_bounce[DOCK_ITEMS];

void dock_layout(void)
{
    int total = 0;
    for (int i = 0; i < DOCK_ITEMS; i++)
        total += dock_size[i] / 16 + 6;
    int x = (SCREEN_W - total) / 2 + 3;
    for (int i = 0; i < DOCK_ITEMS; i++)
    {
        int s = dock_size[i] / 16;
        int bounce = dock_bounce[i] > 0 ? tri(dock_bounce[i] * 16) / 24 : 0;
        dock_ix[i] = x;
        dock_iy[i] = SCREEN_H - 6 - s - bounce;
        x += s + 6;
    }
}

int dock_hit(void)
{
    if (mouse_y < DOCK_Y - 8) return -1;
    for (int i = 0; i < DOCK_ITEMS; i++)
    {
        int s = dock_size[i] / 16;
        if (inside(mouse_x, mouse_y, dock_ix[i] - 3, DOCK_Y - 8, s + 6, SCREEN_H - DOCK_Y + 8))
            return i;
    }
    return -1;
}

void dock_update(void)
{
    int hover = dock_hit();
    for (int i = 0; i < DOCK_ITEMS; i++)
    {
        int d = hover < 0 ? 99 : abs(i - hover);
        int target = (d == 0 ? 30 : d == 1 ? 25 : 21) * 16;     /* magnify the hovered icon and its neighbours */
        dock_size[i] += (target - dock_size[i]) / 3;
        if (dock_bounce[i] > 0) dock_bounce[i]--;
    }
    dock_layout();
}

void draw_power_icon(int x, int y, int s)
{
    c93_fill_round_rect(P_TOP, x + 1, y + 2, s, s, K_SHADOW, s / 4);
    c93_fill_round_rect(P_TOP, x, y, s, s, N(3), s / 4);
    c93_ellipse(P_TOP, x + s / 2, y + s / 2 + 1, s / 3, s / 3, K_RED);
    c93_fill_rect(P_TOP, x + s / 2 - 2, y + s / 2 - s / 3 - 2, 4, s / 3 + 2, N(3));
    c93_fill_rect(P_TOP, x + s / 2 - 1, y + s / 2 - s / 3 - 1, 2, s / 3 + 1, K_RED);
}

void draw_dock(void)
{
    int left = dock_ix[0] - 8, right = dock_ix[DOCK_ITEMS - 1] + dock_size[DOCK_ITEMS - 1] / 16 + 8;
    int top = DOCK_Y + 4;
    c93_fill_round_rect(P_TOP, left + 2, top + 2, right - left, SCREEN_H - top - 1, K_SHADOW, 8);
    c93_fill_round_rect(P_TOP, left, top, right - left, SCREEN_H - top - 2, N(2), 8);
    c93_fill_rect(P_TOP, left + 6, top + 1, right - left - 12, 1, ACC(9));
    c93_fill_rect(P_TOP, left + 6, top + 2, right - left - 12, 1, ACC(4));

    int hover = dock_hit();
    for (int i = 0; i < DOCK_ITEMS; i++)
    {
        int s = dock_size[i] / 16;
        if (dock_apps[i] == APP_DIALOG) draw_power_icon(dock_ix[i], dock_iy[i], s);
        else draw_app_icon(P_TOP, dock_apps[i], dock_ix[i], dock_iy[i], s);

        /* running indicator */
        Win *w = 0;
        for (int k = 0; k < MAX_WIN; k++)
            if (wins[k].used && wins[k].app == dock_apps[i]) w = &wins[k];
        if (w)
            c93_fill_rect(P_TOP, dock_ix[i] + s / 2 - 1, SCREEN_H - 3, 3, 2, w->folded ? K_AMBER : ACC(10));
    }

    if (hover >= 0)
    {
        const char *name = dock_names[hover];
        int tw = txt_w(F_S, name) + 12, s = dock_size[hover] / 16;
        int tx = dock_ix[hover] + s / 2 - tw / 2, ty = dock_iy[hover] - 16;
        c93_fill_round_rect(P_TOP, tx, ty, tw, 12, N(1), 4);
        c93_round_rect(P_TOP, tx, ty, tw, 12, ACC(8), 4);
        txt(P_TOP, F_S, tx + 6, ty + 2, N(15), name);
    }
}

int dock_input(void)
{
    if (!clicked) return dock_hit() >= 0;
    int i = dock_hit();
    if (i < 0) return 0;
    dock_bounce[i] = 16;
    open_app(dock_apps[i], dock_ix[i] + dock_size[i] / 32, dock_iy[i]);
    return 1;
}

/* ---------------------------------------------------------------- desktop icons */

#define DESK_ICONS 5
static const char *desk_names[DESK_ICONS] = { "System", "Programs", "Fonts", "Tests", "Read me" };
static const char *desk_paths[DESK_ICONS] = { "", "programs", "fonts", "tests", "readme.txt" };
static const int desk_kinds[DESK_ICONS] = { FK_OTHER, FK_DIR, FK_DIR, FK_DIR, FK_TEXT };
int desk_sel = -1;

void desk_icon_pos(int i, int *x, int *y)
{
    *x = 22;
    *y = DESK_TOP + 8 + i * 42;
}

void draw_desktop(void)
{
    c93_clear(P_DESK, 0);
    for (int i = 0; i < DESK_ICONS; i++)
    {
        int x, y;
        desk_icon_pos(i, &x, &y);
        draw_big_icon(P_DESK, desk_kinds[i], x, y, i == desk_sel);
        int tw = txt_w(F_S, desk_names[i]);
        txt_shadow(P_DESK, F_S, x + 16 - tw / 2, y + 29, i == desk_sel ? K_WHITE : N(14), desk_names[i]);
    }
    txt_shadow(P_DESK, F_S, SCREEN_W - txt_w(F_S, "Halcyon 1.0 - Continuum 93") - 8, DOCK_Y - 10, N(8), "Halcyon 1.0 - Continuum 93");
}

void desktop_input(void)
{
    if (!clicked) return;
    int hit = -1;
    for (int i = 0; i < DESK_ICONS; i++)
    {
        int x, y;
        desk_icon_pos(i, &x, &y);
        if (inside(mouse_x, mouse_y, x - 6, y - 3, 44, 40)) hit = i;
    }
    if (hit != desk_sel)
    {
        desk_sel = hit;
        desk_dirty = 1;
    }
    if (hit >= 0 && dbl_clicked)
    {
        int x, y;
        desk_icon_pos(hit, &x, &y);
        if (desk_kinds[hit] == FK_TEXT) open_reader(desk_paths[hit], x + 16, y + 12);
        else open_drawer(desk_paths[hit], x + 16, y + 12);
    }
    if (hit < 0)
        ripple(mouse_x, mouse_y);
}

/* ---------------------------------------------------------------- mouse pointer */

static const char *ptr_arrow[16] = {
    "X.........", "XX........", "XWX.......", "XWWX......", "XWWWX.....", "XWWWWX....", "XWWWWWX...", "XWWWWWWX..",
    "XWWWWWWWX.", "XWWWWWXXXX", "XWWXWWX...", "XWX.XWWX..", "XX..XWWX..", "X....XWWX.", ".....XWWX.", "......XX.."
};
static const char *ptr_grab[16] = {
    "..........", "...XX.XX..", "..XWWXWWX.", "..XWWXWWXX", ".XXWWXWWXW", "XWXWWWWWXW", "XWWWWWWWWX", "XWWWWWWWWX",
    ".XWWWWWWWX", ".XWWWWWWX.", "..XWWWWWX.", "..XWWWWX..", "...XXXX...", "..........", "..........", ".........."
};
unsigned char ptr_sprites[3][160];

void build_pointers(void)
{
    for (int k = 0; k < 2; k++)
    {
        const char **rows = k == 0 ? ptr_arrow : ptr_grab;
        for (int y = 0; y < 16; y++)
            for (int x = 0; x < 10; x++)
            {
                int c = rows[y][x];
                ptr_sprites[k][y * 10 + x] = (unsigned char)(c == 'X' ? K_BLACK : c == 'W' ? K_WHITE : 0);
            }
    }
}

int pointer_busy;

void draw_pointer(void)
{
    extern int drag_mode;
    if (pointer_busy)
    {
        for (int i = 0; i < 8; i++)
        {
            int a = frame * 12 + i * 32;
            c93_fill_rect(P_TOP, mouse_x + isin(a) * 6 / 127, mouse_y + isin(a + 64) * 6 / 127, 2, 2, (i & 1) ? ACC(10) : SEC(10));
        }
        return;
    }
    int grab = drag_mode == 1;                 /* moving a window */
    c93_draw_sprite(ptr_sprites[grab ? 1 : 0], P_TOP, mouse_x - (grab ? 4 : 0), mouse_y - (grab ? 4 : 0), 10, 16);
}

int trail_x, trail_y;

void pointer_trail(void)
{
    if (!prefs.trail) return;
    int dx = mouse_x - trail_x, dy = mouse_y - trail_y;
    if (dx * dx + dy * dy > 20)
    {
        Spark *s = &sparks[next_spark];
        next_spark = (next_spark + 1) % MAX_SPARKS;
        s->x = mouse_x << 8;
        s->y = mouse_y << 8;
        s->vx = -dx * 12 + fx_rand(160) - 80;
        s->vy = -dy * 12 + fx_rand(160) - 80;
        s->life = 10 + fx_rand(6);
    }
    trail_x = mouse_x;
    trail_y = mouse_y;
}
