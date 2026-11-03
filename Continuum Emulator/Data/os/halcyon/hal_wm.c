/*
 * Halcyon OS - window manager.
 *
 * Every window has a control rail on its side (left by default, right-click the rail to move it to the
 * other side) holding the close / fold / zoom buttons and the vertical title. Windows snap magnetically
 * to the screen and to each other while dragged, and fold across themselves like an accordion into a
 * slim spine that can still be moved around.
 */

#define SNAP_DIST 8

/* App hooks (implemented in hal_apps.c) */
void app_init(Win *w);
void app_draw(Win *w, int x, int y, int cw, int ch);
int app_click(Win *w, int lx, int ly, int dbl, int right);
void app_drag(Win *w, int lx, int ly);
void app_release(Win *w);
void app_key(Win *w);
void app_tick(Win *w);
void app_close(Win *w);

Win wins[MAX_WIN];
int zorder[MAX_WIN], zcount;

enum { DRAG_NONE, DRAG_MOVE, DRAG_RESIZE, DRAG_APP };
int drag_mode;
Win *drag_win;
int drag_dx, drag_dy;
int last_snap_x = -1, last_snap_y = -1;
int cascade;

/* ---------------------------------------------------------------- geometry */

int win_cur_w(Win *w)
{
    if (w->folded) return RAIL_W;
    return RAIL_W + (w->w - RAIL_W) * w->open_t / 256;
}

/* Left edge of what is visible: with the rail on the right, folding collapses towards the right */
int win_vis_x(Win *w) { return w->rail_right ? w->x + w->w - win_cur_w(w) : w->x; }
int win_rail_x(Win *w) { return w->rail_right ? win_vis_x(w) + win_cur_w(w) - RAIL_W : win_vis_x(w); }
int win_body_x(Win *w) { return w->rail_right ? win_vis_x(w) : win_vis_x(w) + RAIL_W; }

/* Inner content rectangle (only meaningful when the window is fully open) */
void win_content(Win *w, int *x, int *y, int *cw, int *ch)
{
    *x = win_body_x(w) + 4;
    *y = w->y + 4;
    *cw = w->w - RAIL_W - 8;
    *ch = w->h - 8;
}

int win_index(Win *w) { return (int)(w - wins); }

Win *active_window(void)
{
    return zcount > 0 ? &wins[zorder[zcount - 1]] : 0;
}

void focus_window(Win *w)
{
    int idx = win_index(w), pos = -1;
    for (int i = 0; i < zcount; i++)
        if (zorder[i] == idx) pos = i;
    if (pos < 0) return;
    for (int i = pos; i < zcount - 1; i++)
        zorder[i] = zorder[i + 1];
    zorder[zcount - 1] = idx;
    win_dirty = 1;
}

Win *win_at(int x, int y)
{
    for (int i = zcount - 1; i >= 0; i--)
    {
        Win *w = &wins[zorder[i]];
        if (inside(x, y, win_vis_x(w), w->y, win_cur_w(w), w->h))
            return w;
    }
    return 0;
}

/* ---------------------------------------------------------------- opening and closing */

Win *new_window(int app, const char *title, int w, int h, int fromx, int fromy)
{
    for (int i = 0; i < MAX_WIN; i++)
    {
        Win *win = &wins[i];
        if (win->used) continue;
        memset(win, 0, sizeof(Win));
        win->used = 1;
        win->app = app;
        win->w = w;
        win->h = h;
        win->x = 70 + (cascade % 6) * 22;
        win->y = DESK_TOP + 6 + (cascade % 6) * 14;
        cascade++;
        if (win->x + w > SCREEN_W - 4) win->x = SCREEN_W - 4 - w;
        if (win->y + h > DESK_BOTTOM) win->y = DESK_BOTTOM - h;
        if (win->y < DESK_TOP) win->y = DESK_TOP;
        win->open_t = 256;
        win->rail_right = prefs.rail_right;
        strcpy(win->title, title);
        zorder[zcount++] = i;
        zoom_box(fromx - 8, fromy - 8, 16, 16, win->x, win->y, w, h);
        win_dirty = 1;
        return win;
    }
    toast("Too many windows open", K_RED);
    return 0;
}

void close_window(Win *w)
{
    int idx = win_index(w), pos = -1;
    int vx = win_vis_x(w);
    zoom_box(vx, w->y, win_cur_w(w), w->h, vx + win_cur_w(w) / 2 - 4, w->y + w->h / 2 - 4, 8, 8);
    spawn_sparks(vx + win_cur_w(w) / 2, w->y + w->h / 2, 18, 0);
    app_close(w);
    for (int i = 0; i < zcount; i++)
        if (zorder[i] == idx) pos = i;
    if (pos >= 0)
    {
        for (int i = pos; i < zcount - 1; i++)
            zorder[i] = zorder[i + 1];
        zcount--;
    }
    if (drag_win == w)
        drag_mode = DRAG_NONE;
    w->used = 0;
    win_dirty = 1;
}

void toggle_fold(Win *w)
{
    if (w->folded || w->fold_dir < 0)
    {
        w->folded = 0;
        w->fold_dir = 1;
    }
    else
    {
        w->fold_dir = -1;
    }
    win_dirty = 1;
}

void toggle_zoom(Win *w)
{
    int ox = w->x, oy = w->y, ow = w->w, oh = w->h;
    if (w->folded) toggle_fold(w);
    if (!w->maxed)
    {
        w->rx = w->x; w->ry = w->y; w->rw = w->w; w->rh = w->h;
        w->x = 4;
        w->y = DESK_TOP + 2;
        w->w = SCREEN_W - 8;
        w->h = DESK_BOTTOM - DESK_TOP - 4;
        w->maxed = 1;
    }
    else
    {
        w->x = w->rx; w->y = w->ry; w->w = w->rw; w->h = w->rh;
        w->maxed = 0;
    }
    zoom_box(ox, oy, ow, oh, w->x, w->y, w->w, w->h);
    win_dirty = 1;
}

void fold_all(int fold)
{
    for (int i = 0; i < MAX_WIN; i++)
    {
        Win *w = &wins[i];
        if (!w->used) continue;
        if (fold && !w->folded && w->fold_dir >= 0) toggle_fold(w);
        if (!fold && (w->folded || w->fold_dir < 0)) toggle_fold(w);
    }
}

/* Arranges the open windows in a grid that fills the desktop */
void tile_windows(void)
{
    int n = zcount;
    if (n == 0) return;
    int cols = n <= 2 ? n : n <= 4 ? 2 : 3;
    int rows = (n + cols - 1) / cols;
    int cw = (SCREEN_W - 8) / cols, ch = (DESK_BOTTOM - DESK_TOP - 4) / rows;
    for (int i = 0; i < n; i++)
    {
        Win *w = &wins[zorder[i]];
        int ox = w->x, oy = w->y, ow = win_cur_w(w), oh = w->h;
        w->folded = 0;
        w->open_t = 256;
        w->fold_dir = 0;
        w->maxed = 0;
        w->x = 4 + (i % cols) * cw;
        w->y = DESK_TOP + 2 + (i / cols) * ch;
        w->w = cw - 2;
        w->h = ch - 2;
        zoom_box(ox, oy, ow, oh, w->x, w->y, w->w, w->h);
    }
    win_dirty = 1;
}

/* ---------------------------------------------------------------- magnetic snapping */

static void consider(int line, int edge, int *best, int *guide)
{
    int d = line - edge;
    if ((d < 0 ? -d : d) < (*best < 0 ? -*best : *best))
    {
        *best = d;
        *guide = line;
    }
}

/* Pulls a window being moved towards nearby screen edges and window edges */
void snap_position(Win *w, int *nx, int *ny)
{
    int cw = win_cur_w(w), h = w->h;
    int bx = SNAP_DIST + 1, by = SNAP_DIST + 1, gx = -1, gy = -1;
    int off = w->rail_right ? w->w - cw : 0;        /* snap the visible part, not the footprint */
    *nx += off;

    consider(0, *nx, &bx, &gx);
    consider(SCREEN_W, *nx + cw, &bx, &gx);
    consider(DESK_TOP, *ny, &by, &gy);
    consider(DESK_BOTTOM, *ny + h, &by, &gy);

    for (int i = 0; i < zcount; i++)
    {
        Win *o = &wins[zorder[i]];
        if (o == w) continue;
        int ox = win_vis_x(o), ow = win_cur_w(o);
        int vert_overlap = *ny < o->y + o->h + SNAP_DIST && *ny + h > o->y - SNAP_DIST;
        int horz_overlap = *nx < ox + ow + SNAP_DIST && *nx + cw > ox - SNAP_DIST;
        if (vert_overlap)
        {
            consider(ox + ow, *nx, &bx, &gx);          /* our left edge to its right edge */
            consider(ox, *nx + cw, &bx, &gx);          /* our right edge to its left edge */
            consider(ox, *nx, &bx, &gx);               /* align left edges */
            consider(ox + ow, *nx + cw, &bx, &gx);     /* align right edges */
        }
        if (horz_overlap)
        {
            consider(o->y + o->h, *ny, &by, &gy);
            consider(o->y, *ny + h, &by, &gy);
            consider(o->y, *ny, &by, &gy);
            consider(o->y + o->h, *ny + h, &by, &gy);
        }
    }

    snap_gx = snap_gy = -1;
    if (prefs.snap && (bx < 0 ? -bx : bx) <= SNAP_DIST)
    {
        *nx += bx;
        snap_gx = gx >= SCREEN_W ? SCREEN_W - 1 : gx;
    }
    if (prefs.snap && (by < 0 ? -by : by) <= SNAP_DIST)
    {
        *ny += by;
        snap_gy = gy;
    }

    /* a little burst of sparks whenever a new magnetic contact is made */
    if (snap_gx >= 0 && snap_gx != last_snap_x)
        spawn_sparks(snap_gx, *ny + h / 2, 10, 0);
    if (snap_gy >= 0 && snap_gy != last_snap_y)
        spawn_sparks(*nx + cw / 2, snap_gy, 10, 0);
    last_snap_x = snap_gx;
    last_snap_y = snap_gy;
    *nx -= off;
}

/* ---------------------------------------------------------------- drawing */

static void draw_rail(Win *w, int active)
{
    int rx = win_rail_x(w), y = w->y, h = w->h;
    int hover_close = 0, hover_fold = 0, hover_zoom = 0;

    if (active) bevel(P_WIN, rx, y, RAIL_W, h, ACC(5), ACC(10), ACC(2));
    else bevel(P_WIN, rx, y, RAIL_W, h, N(5), N(8), N(2));

    /* light streak down the rail */
    c93_fill_rect(P_WIN, rx + 3, y + 3, 1, h - 6, active ? ACC(8) : N(7));

    if (inside(mouse_x, mouse_y, rx, y, RAIL_W, 46))
    {
        hover_close = mouse_y < y + 15;
        hover_fold = mouse_y >= y + 15 && mouse_y < y + 27;
        hover_zoom = mouse_y >= y + 27;
    }
    int cx = rx + RAIL_W / 2;
    c93_fill_ellipse(P_WIN, cx, y + 9, hover_close ? 5 : 4, hover_close ? 5 : 4, K_RED);
    c93_fill_ellipse(P_WIN, cx, y + 21, hover_fold ? 5 : 4, hover_fold ? 5 : 4, K_AMBER);
    c93_fill_ellipse(P_WIN, cx, y + 33, hover_zoom ? 5 : 4, hover_zoom ? 5 : 4, K_GREEN);
    if (hover_close) c93_fill_rect(P_WIN, cx - 2, y + 8, 5, 2, K_WHITE);
    if (hover_fold) c93_fill_rect(P_WIN, cx - 1, y + 19, 2, 5, K_WHITE);
    if (hover_zoom) { c93_fill_rect(P_WIN, cx - 2, y + 32, 5, 2, K_WHITE); c93_fill_rect(P_WIN, cx - 1, y + 31, 2, 5, K_WHITE); }

    /* vertical title: one letter per line */
    char one[2];
    one[1] = 0;
    int ty = y + 46;
    for (int i = 0; w->title[i] && ty + 9 < y + h - 10; i++, ty += 9)
    {
        one[0] = w->title[i];
        if (one[0] == ' ') continue;
        txt(P_WIN, F_S, rx + (RAIL_W - txt_w(F_S, one)) / 2 + 1, ty, active ? K_WHITE : N(12), one);
    }

    /* grip dots */
    for (int i = 0; i < 3; i++)
        c93_fill_rect(P_WIN, rx + 5 + i * 3, y + h - 6, 2, 2, active ? ACC(12) : N(9));
}

/* Accordion: the body drawn as creased paper panels while folding */
static void draw_accordion(Win *w, int bx, int bw, int active)
{
    int panels = 6;
    int light = active ? ACC(4) : N(6), dark = active ? ACC(2) : N(3);
    int shade = 256 - w->open_t;                    /* more folded = more contrast */
    for (int i = 0; i < panels; i++)
    {
        int x0 = bx + bw * i / panels, x1 = bx + bw * (i + 1) / panels;
        int pw = x1 - x0;
        if (pw <= 0) continue;
        int half = pw / 2;
        /* each panel is lit on one half and shaded on the other, alternating like a folded sheet */
        c93_fill_rect(P_WIN, x0, w->y, half, w->h, (i & 1) ? dark : light);
        c93_fill_rect(P_WIN, x0 + half, w->y, pw - half, w->h, (i & 1) ? light : dark);
        c93_fill_rect(P_WIN, x0, w->y, 1, w->h, shade > 128 ? N(1) : N(9));
        for (int k = 1; k < 6; k++)                 /* ghost lines of the content */
            c93_fill_rect(P_WIN, x0 + 2, w->y + k * w->h / 6, pw > 6 ? pw - 4 : 1, 1, (i & 1) ? light : dark);
    }
}

void draw_window(Win *w, int active)
{
    int cw = win_cur_w(w), vx = win_vis_x(w);

    c93_fill_rect(P_WIN, vx + 4, w->y + 4, cw, w->h, K_SHADOW);        /* drop shadow */

    if (!w->folded && cw > RAIL_W)
    {
        int bx = win_body_x(w), bw = cw - RAIL_W;
        if (w->open_t < 256)
        {
            draw_accordion(w, bx, bw, active);
        }
        else
        {
            bevel(P_WIN, bx, w->y, bw, w->h, N(3), N(7), N(1));
            int x, y, ww, hh;
            win_content(w, &x, &y, &ww, &hh);
            app_draw(w, x, y, ww, hh);
            /* resize grip */
            int gx = bx + bw - 9, gy = w->y + w->h - 9;
            for (int i = 0; i < 3; i++)
                c93_fill_rect(P_WIN, gx + i * 3, gy + 6 - i * 3, 7 - i * 3, 1, N(9));
        }
    }

    draw_rail(w, active);
    c93_rect(P_WIN, vx - 1, w->y - 1, cw + 2, w->h + 2, active ? K_PULSE : N(1));
}

void draw_windows(void)
{
    c93_clear(P_WIN, 0);
    for (int i = 0; i < zcount; i++)
        draw_window(&wins[zorder[i]], i == zcount - 1);
}

/* ---------------------------------------------------------------- per-frame update and input */

void wm_tick(void)
{
    for (int i = 0; i < MAX_WIN; i++)
    {
        Win *w = &wins[i];
        if (!w->used) continue;
        if (w->fold_dir)
        {
            w->open_t += w->fold_dir * 24;
            if (w->open_t <= 0)
            {
                w->open_t = 0;
                w->folded = 1;
                w->fold_dir = 0;
                spawn_sparks(win_rail_x(w) + RAIL_W / 2, w->y + w->h / 2, 8, 0);
            }
            if (w->open_t >= 256)
            {
                w->open_t = 256;
                w->fold_dir = 0;
            }
            win_dirty = 1;
        }
        if (!w->folded && w->open_t == 256)
        {
            app_tick(w);
            if (w->live && frame % 3 == 0) win_dirty = 1;
        }
    }
}

/* Returns 1 if the windows consumed the mouse event */
int wm_input(void)
{
    if (drag_mode != DRAG_NONE)
    {
        Win *w = drag_win;
        if (!(mouse_b & 1))
        {
            if (drag_mode == DRAG_APP) app_release(w);
            drag_mode = DRAG_NONE;
            snap_gx = snap_gy = -1;
            last_snap_x = last_snap_y = -1;
            win_dirty = 1;
            return 1;
        }
        if (drag_mode == DRAG_MOVE)
        {
            int nx = mouse_x - drag_dx, ny = mouse_y - drag_dy;
            snap_position(w, &nx, &ny);
            if (ny < DESK_TOP) ny = DESK_TOP;
            if (ny > DESK_BOTTOM - 20) ny = DESK_BOTTOM - 20;
            if (nx < 20 - win_cur_w(w)) nx = 20 - win_cur_w(w);
            if (nx > SCREEN_W - 20) nx = SCREEN_W - 20;
            if (nx != w->x || ny != w->y)
            {
                w->x = nx;
                w->y = ny;
                w->maxed = 0;
                win_dirty = 1;
            }
        }
        else if (drag_mode == DRAG_RESIZE)
        {
            int nw = mouse_x - w->x + drag_dx, nh = mouse_y - w->y + drag_dy;
            int min_w = w->app == APP_SKETCH ? CANVAS_W + RAIL_W + 40 : w->app == APP_PULSE ? 220 : MIN_W;
            int min_h = w->app == APP_SKETCH ? CANVAS_H + 16 : w->app == APP_CALC ? 130 : MIN_H;
            if (nw < min_w) nw = min_w;
            if (nh < min_h) nh = min_h;
            if (w->x + nw > SCREEN_W) nw = SCREEN_W - w->x;
            if (w->y + nh > DESK_BOTTOM) nh = DESK_BOTTOM - w->y;
            if (nw != w->w || nh != w->h)
            {
                w->w = nw;
                w->h = nh;
                w->maxed = 0;
                win_dirty = 1;
            }
        }
        else
        {
            int x, y, cw, ch;
            win_content(w, &x, &y, &cw, &ch);
            app_drag(w, mouse_x - x, mouse_y - y);
        }
        return 1;
    }

    if (!clicked && !rclicked)
        return win_at(mouse_x, mouse_y) != 0;

    Win *w = win_at(mouse_x, mouse_y);
    if (!w)
        return 0;

    focus_window(w);
    int rx = win_rail_x(w);

    if (inside(mouse_x, mouse_y, rx, w->y, RAIL_W, w->h))
    {
        if (rclicked)
        {
            /* right-click on a rail moves the controls to the other side */
            w->rail_right = !w->rail_right;
            spawn_sparks(rx + RAIL_W / 2, mouse_y, 12, 0);
            win_dirty = 1;
            return 1;
        }
        int ly = mouse_y - w->y;
        if (ly < 15) { close_window(w); return 1; }
        if (ly < 27) { toggle_fold(w); return 1; }
        if (ly < 39) { toggle_zoom(w); return 1; }
        if (dbl_clicked) { toggle_fold(w); return 1; }
        drag_mode = DRAG_MOVE;
        drag_win = w;
        drag_dx = mouse_x - w->x;
        drag_dy = mouse_y - w->y;
        return 1;
    }

    if (w->folded || w->open_t < 256)
        return 1;

    int bx = win_body_x(w), bw = win_cur_w(w) - RAIL_W;
    if (clicked && inside(mouse_x, mouse_y, bx + bw - 10, w->y + w->h - 10, 10, 10))
    {
        drag_mode = DRAG_RESIZE;
        drag_win = w;
        drag_dx = w->x + w->w - mouse_x;
        drag_dy = w->y + w->h - mouse_y;
        return 1;
    }

    int x, y, cw, ch;
    win_content(w, &x, &y, &cw, &ch);
    if (app_click(w, mouse_x - x, mouse_y - y, dbl_clicked, rclicked))
    {
        drag_mode = DRAG_APP;
        drag_win = w;
    }
    return 1;
}
