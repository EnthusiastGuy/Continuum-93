/*
 * Halcyon OS - applications: Drawers, Reader, Pulse, Calc, Sketch, Prefs, About and the shutdown dialog.
 */

#define ROW_H 11

void popup_menu(int x, int y, const char **items, int count, int tag);

/* ================================================================ shared helpers */

static int str_ieq_suffix(const char *s, const char *suffix)
{
    int n = strlen(s), m = strlen(suffix);
    if (n < m) return 0;
    for (int i = 0; i < m; i++)
    {
        int a = s[n - m + i];
        if (a >= 'A' && a <= 'Z') a += 32;
        if (a != suffix[i]) return 0;
    }
    return 1;
}

int file_kind(const char *name)
{
    if (str_ieq_suffix(name, ".asm")) return FK_ASM;
    if (str_ieq_suffix(name, ".c")) return FK_C;
    if (str_ieq_suffix(name, ".h")) return FK_H;
    if (str_ieq_suffix(name, ".bas")) return FK_BAS;
    if (str_ieq_suffix(name, ".txt") || str_ieq_suffix(name, ".md") || str_ieq_suffix(name, ".log") ||
        str_ieq_suffix(name, ".cfg") || str_ieq_suffix(name, ".json") || str_ieq_suffix(name, ".prefs"))
        return FK_TEXT;
    if (str_ieq_suffix(name, ".png") || str_ieq_suffix(name, ".bmp")) return FK_IMG;
    if (str_ieq_suffix(name, ".font") || str_ieq_suffix(name, ".ttf")) return FK_FONT;
    return FK_OTHER;
}

static int ci_cmp(const char *a, const char *b)
{
    for (;; a++, b++)
    {
        int x = *a, y = *b;
        if (x >= 'A' && x <= 'Z') x += 32;
        if (y >= 'A' && y <= 'Z') y += 32;
        if (x != y || !x) return x - y;
    }
}

void path_join(char *out, const char *dir, const char *name)
{
    strcpy(out, dir);
    if (out[0]) strcat(out, "/");
    strcat(out, name);
}

const char *path_last(const char *path)
{
    const char *last = path;
    for (const char *p = path; *p; p++)
        if (*p == '/' || *p == '\\') last = p + 1;
    return last;
}

void fmt_size(char *out, unsigned int bytes)
{
    if (bytes < 10000) { fmt_int(out, (int)bytes, 0); strcat(out, " B"); }
    else if (bytes < 10000000) { fmt_int(out, (int)(bytes / 1024), 1); strcat(out, " K"); }
    else { fmt_int(out, (int)(bytes / 1048576), 1); strcat(out, " M"); }
}

/* Vertical scrollbar; returns the thumb geometry through ty/th */
void scrollbar(int x, int y, int h, int total, int visible, int pos, int *ty, int *th)
{
    well(P_WIN, x, y, 6, h, N(2));
    if (total <= visible)
    {
        *ty = y; *th = h;
        return;
    }
    *th = h * visible / total;
    if (*th < 8) *th = 8;
    *ty = y + (h - *th) * pos / (total - visible);
    c93_fill_round_rect(P_WIN, x + 1, *ty, 4, *th, ACC(8), 2);
}

/* ================================================================ Drawers (file browser) */

char *drawer_names(Win *w) { return WS_DIRS + win_index(w) * WS_DIR_SIZE; }
const char *entry_name(Win *w, int i) { return drawer_names(w) + w->name_ofs[i]; }

void load_dir(Win *w)
{
    int folders = 0;
    char *dst = drawer_names(w);
    int used = 0;
    w->count = 0;
    w->folders = 0;
    w->scroll = 0;
    w->sel = -1;

    int total = c93_list_dir(w->path, WS_LISTING, &folders);
    if (total < 0)
    {
        toast("Folder not found", K_RED);
        win_dirty = 1;
        return;
    }

    /* folders, an empty name, then files */
    const char *p = WS_LISTING;
    for (int pass = 0; pass < 2; pass++)
    {
        while (*p)
        {
            int len = strlen(p);
            if (w->count < 160 && used + len + 1 < WS_DIR_SIZE)
            {
                strcpy(dst + used, p);
                w->name_ofs[w->count] = (unsigned short)used;
                w->kinds[w->count] = (unsigned char)(pass == 0 ? FK_DIR : file_kind(p));
                w->sizes[w->count] = 0;
                if (pass == 1)
                {
                    char full[200];
                    path_join(full, w->path, p);
                    w->sizes[w->count] = c93_file_size(full);
                }
                else
                {
                    w->folders++;
                }
                used += len + 1;
                w->count++;
            }
            p += len + 1;
        }
        p++;
    }

    /* insertion sort: folders first, then case-insensitive by name */
    for (int i = 1; i < w->count; i++)
    {
        unsigned short ofs = w->name_ofs[i];
        unsigned char kind = w->kinds[i];
        unsigned int size = w->sizes[i];
        int j = i - 1;
        while (j >= 0)
        {
            int a_dir = w->kinds[j] == FK_DIR, b_dir = kind == FK_DIR;
            int greater = a_dir != b_dir ? b_dir : ci_cmp(dst + w->name_ofs[j], dst + ofs) > 0;
            if (!greater) break;
            w->name_ofs[j + 1] = w->name_ofs[j];
            w->kinds[j + 1] = w->kinds[j];
            w->sizes[j + 1] = w->sizes[j];
            j--;
        }
        w->name_ofs[j + 1] = ofs;
        w->kinds[j + 1] = kind;
        w->sizes[j + 1] = size;
    }

    const char *last = path_last(w->path);
    strcpy(w->title, last[0] ? last : "System");
    win_dirty = 1;
}

Win *open_drawer(const char *path, int fromx, int fromy)
{
    Win *w = new_window(APP_DRAWERS, "System", 236, 168, fromx, fromy);
    if (!w) return 0;
    strcpy(w->path, path);
    load_dir(w);
    return w;
}

void drawer_up(Win *w)
{
    int n = strlen(w->path);
    while (n > 0 && w->path[n - 1] != '/' && w->path[n - 1] != '\\') n--;
    if (n > 0) n--;
    w->path[n] = 0;
    load_dir(w);
}

void drawer_open_entry(Win *w, int i, int view_source)
{
    char full[200];
    if (i < 0 || i >= w->count) return;
    path_join(full, w->path, entry_name(w, i));
    int kind = w->kinds[i];
    if (kind == FK_DIR)
    {
        strcpy(w->path, full);
        load_dir(w);
        return;
    }
    if (!view_source && (kind == FK_ASM || kind == FK_C || kind == FK_BAS))
    {
        request_run(full);
        return;
    }
    if (kind == FK_TEXT || kind == FK_H || kind == FK_C || kind == FK_ASM || kind == FK_BAS || kind == FK_OTHER)
    {
        open_reader(full, mouse_x, mouse_y);
        return;
    }
    toast(kind == FK_IMG ? "Images open in programs, not here (yet)" : "Fonts are used by programs", K_AMBER);
}

void drawer_context(Win *w, int item)
{
    if (w->sel < 0) return;
    if (item == 0) drawer_open_entry(w, w->sel, 0);
    else if (item == 1) drawer_open_entry(w, w->sel, 1);
    else
    {
        char msg[64], num[16];
        strcpy(msg, entry_name(w, w->sel));
        if (w->kinds[w->sel] != FK_DIR)
        {
            strcat(msg, "  ");
            fmt_size(num, w->sizes[w->sel]);
            strcat(msg, num);
        }
        toast(msg, ACC(9));
    }
}

void drawers_draw(Win *w, int x, int y, int cw, int ch)
{
    char buf[40];
    /* header: up button, path, item count */
    button(P_WIN, x, y, 14, 12, "<", inside(mouse_x, mouse_y, x, y, 14, 12), 0);
    fmt_int(buf, w->count, 0);
    strcat(buf, " items");
    int cnt_w = txt_w(F_S, buf);
    txt(P_WIN, F_S, x + cw - cnt_w, y + 2, N(9), buf);
    txt_clip(P_WIN, F_S, x + 18, y + 2, ACC(11), w->path[0] ? w->path : "/", cw - cnt_w - 26);

    int ly = y + 15, lh = ch - 15;
    well(P_WIN, x, ly, cw - 8, lh, N(1));
    int rows = (lh - 4) / ROW_H;
    if (w->scroll > w->count - rows) w->scroll = w->count - rows;
    if (w->scroll < 0) w->scroll = 0;

    for (int r = 0; r < rows && w->scroll + r < w->count; r++)
    {
        int i = w->scroll + r;
        int ry = ly + 2 + r * ROW_H;
        int hot = inside(mouse_x, mouse_y, x + 2, ry, cw - 12, ROW_H);
        if (i == w->sel) c93_fill_rect(P_WIN, x + 2, ry, cw - 12, ROW_H, K_SEL);
        else if (hot) c93_fill_rect(P_WIN, x + 2, ry, cw - 12, ROW_H, K_HOT);
        draw_mini_icon(P_WIN, w->kinds[i], x + 5, ry + 1);
        int size_w = 0;
        if (w->kinds[i] != FK_DIR)
        {
            fmt_size(buf, w->sizes[i]);
            size_w = txt_w(F_S, buf);
            txt(P_WIN, F_S, x + cw - 14 - size_w, ry + 2, i == w->sel ? N(14) : N(8), buf);
        }
        txt_clip(P_WIN, F_S, x + 19, ry + 2, w->kinds[i] == FK_DIR ? K_FOLDER_HI : N(14), entry_name(w, i), cw - 40 - size_w);
    }

    if (w->count == 0)
        txt(P_WIN, F_S, x + 8, ly + 8, N(8), "(empty)");

    int ty, th;
    scrollbar(x + cw - 6, ly, lh, w->count, rows, w->scroll, &ty, &th);
}

int drawers_rows(Win *w) { return (w->h - 8 - 15 - 4) / ROW_H; }

int drawers_click(Win *w, int lx, int ly, int dbl, int right)
{
    int cw = w->w - RAIL_W - 8;
    if (!right && inside(lx, ly, 0, 0, 14, 12))
    {
        drawer_up(w);
        return 0;
    }
    if (lx >= cw - 6 && ly >= 15)                       /* scrollbar: jump and drag */
    {
        app_drag(w, lx, ly);
        return 1;
    }
    if (ly >= 17 && lx < cw - 8)
    {
        int i = w->scroll + (ly - 17) / ROW_H;
        if (i < w->count)
        {
            w->sel = i;
            win_dirty = 1;
            if (right)
            {
                static const char *items[3] = { "Open / Run", "View source", "Info" };
                popup_menu(mouse_x, mouse_y, items, 3, 100 + win_index(w));
            }
            else if (dbl)
            {
                drawer_open_entry(w, i, 0);
            }
        }
    }
    return 0;
}

void list_scroll_drag(Win *w, int ly, int top, int count, int rows)
{
    int ch = w->h - 8 - top;
    if (count <= rows) return;
    int pos = (ly - top) * (count - rows) / (ch > 1 ? ch : 1);
    if (pos < 0) pos = 0;
    if (pos > count - rows) pos = count - rows;
    if (pos != w->scroll)
    {
        w->scroll = pos;
        win_dirty = 1;
    }
}

void list_keys(Win *w, int count, int rows, int has_sel)
{
    int old_sel = w->sel, old_scroll = w->scroll;
    if (has_sel)
    {
        if (key_pressed(C93_KEY_DOWN)) w->sel = w->sel < count - 1 ? w->sel + 1 : w->sel;
        if (key_pressed(C93_KEY_UP)) w->sel = w->sel > 0 ? w->sel - 1 : 0;
        if (w->sel >= 0 && w->sel < w->scroll) w->scroll = w->sel;
        if (w->sel >= w->scroll + rows) w->scroll = w->sel - rows + 1;
    }
    else
    {
        if (key_pressed(C93_KEY_DOWN)) w->scroll++;
        if (key_pressed(C93_KEY_UP)) w->scroll--;
    }
    if (key_pressed(34)) w->scroll += rows;      /* page down */
    if (key_pressed(33)) w->scroll -= rows;      /* page up */
    if (key_pressed(36)) w->scroll = 0;          /* home */
    if (key_pressed(35)) w->scroll = count;      /* end */
    if (w->scroll > count - rows) w->scroll = count - rows;
    if (w->scroll < 0) w->scroll = 0;
    if (w->sel != old_sel || w->scroll != old_scroll) win_dirty = 1;
}

void drawers_key(Win *w)
{
    list_keys(w, w->count, drawers_rows(w), 1);
    if (key_pressed(C93_KEY_ENTER)) drawer_open_entry(w, w->sel, 0);
    if (key_pressed(C93_KEY_BACKSPACE)) drawer_up(w);
}

/* ================================================================ Reader */

Win *reader_win;
char reader_path[160];
int reader_len, reader_lines, reader_kind;
unsigned short line_ofs[2600];

void open_reader(const char *path, int fromx, int fromy)
{
    unsigned int size;
    if (!c93_file_exists(path))
    {
        toast("File not found", K_RED);
        return;
    }
    size = c93_file_size(path);
    if (size > WS_TEXT_MAX)
    {
        toast("File is too large for the Reader", K_RED);
        return;
    }
    if (!reader_win || !reader_win->used || reader_win->app != APP_READER)
        reader_win = new_window(APP_READER, "Reader", 300, 190, fromx, fromy);
    if (!reader_win) return;
    focus_window(reader_win);
    if (reader_win->folded) toggle_fold(reader_win);

    strcpy(reader_path, path);
    c93_load_file(path, WS_TEXT);
    WS_TEXT[size] = 0;
    reader_len = (int)size;
    reader_kind = file_kind(path);
    reader_lines = 0;
    line_ofs[reader_lines++] = 0;
    for (int i = 0; i < reader_len && reader_lines < 2600; i++)
        if (WS_TEXT[i] == '\n')
            line_ofs[reader_lines++] = (unsigned short)(i + 1);
    reader_win->scroll = 0;
    strcpy(reader_win->title, path_last(path));
    win_dirty = 1;
}

static int line_color(const char *s)
{
    while (*s == ' ' || *s == '\t') s++;
    if ((s[0] == '/' && (s[1] == '/' || s[1] == '*')) || s[0] == ';' || s[0] == '*') return K_ASM;
    if (s[0] == '#') return SEC(10);
    if (s[0] == '.') return K_AMBER;
    if (s[0] == 0 || s[0] == '\r') return N(14);
    return N(14);
}

int reader_rows(Win *w) { return (w->h - 8 - 15 - 4) / 9; }

void reader_draw(Win *w, int x, int y, int cw, int ch)
{
    char buf[100], num[12];
    char info[48];
    fmt_int(info, reader_lines, 1);
    strcat(info, " lines");
    int iw = txt_w(F_S, info);
    txt(P_WIN, F_S, x + cw - iw, y + 2, N(9), info);
    txt_clip(P_WIN, F_S, x, y + 2, ACC(11), reader_path, cw - iw - 8);

    int ly = y + 15, lh = ch - 15;
    well(P_WIN, x, ly, cw - 8, lh, N(1));
    int rows = (lh - 4) / 9;
    int maxc = (cw - 44) / 6;
    if (maxc > 96) maxc = 96;
    if (w->scroll > reader_lines - rows) w->scroll = reader_lines - rows;
    if (w->scroll < 0) w->scroll = 0;
    int colored = reader_kind == FK_C || reader_kind == FK_H || reader_kind == FK_ASM || reader_kind == FK_BAS;

    for (int r = 0; r < rows && w->scroll + r < reader_lines; r++)
    {
        int li = w->scroll + r, ry = ly + 3 + r * 9;
        const char *s = WS_TEXT + line_ofs[li];
        int n = 0;
        for (int k = 0; s[k] && s[k] != '\n' && n < maxc; k++)
        {
            if (s[k] == '\r') continue;
            if (s[k] == '\t') { int t = 4 - n % 4; while (t-- && n < maxc) buf[n++] = ' '; }
            else buf[n++] = s[k];
        }
        buf[n] = 0;
        fmt_int(num, li + 1, 0);
        txt(P_WIN, F_S, x + 30 - txt_w(F_S, num), ry, N(6), num);
        txt(P_WIN, F_S, x + 36, ry, colored ? line_color(buf) : N(14), buf);
    }

    int ty, th;
    scrollbar(x + cw - 6, ly, lh, reader_lines, rows, w->scroll, &ty, &th);
}

int reader_click(Win *w, int lx, int ly, int dbl, int right)
{
    (void)dbl; (void)right;
    int cw = w->w - RAIL_W - 8;
    if (lx >= cw - 6 && ly >= 15)
    {
        app_drag(w, lx, ly);
        return 1;
    }
    return 0;
}

/* ================================================================ Pulse (system monitor) */

int perf_busy[64];          /* ms of work per frame, written by the main loop */
int perf_pos;
extern char __heap_start[];

void pulse_draw(Win *w, int x, int y, int cw, int ch)
{
    char buf[32], num[16];
    (void)w;

    txt_rainbow(P_WIN, F_M, x + 2, y + 1, "PULSE", 0);
    txt(P_WIN, F_S, x + 58, y + 4, N(9), "live system monitor");

    /* frame-time bars */
    int gx = x, gy = y + 18, gw = cw * 3 / 5, gh = 56;
    well(P_WIN, gx, gy, gw, gh, N(1));
    for (int k = 1; k < 4; k++)
        c93_fill_rect(P_WIN, gx + 1, gy + gh * k / 4, gw - 2, 1, N(2));
    int bars = (gw - 6) / 4;
    int sum = 0;
    for (int i = 0; i < bars && i < 64; i++)
    {
        int v = perf_busy[(perf_pos - bars + i + 64) & 63];
        sum += v;
        int bh = v * (gh - 6) / 17;
        if (bh > gh - 6) bh = gh - 6;
        if (bh < 1) bh = 1;
        c93_fill_rect(P_WIN, gx + 3 + i * 4, gy + gh - 3 - bh, 3, bh, K_CHART + (bh * 15) / (gh - 6));
    }
    int load = bars ? sum * 100 / (bars * 17) : 0;
    if (load > 100) load = 100;
    strcpy(buf, "CPU ");
    fmt_int(num, load, 0);
    strcat(buf, num);
    strcat(buf, "%");
    txt(P_WIN, F_S, gx + 4, gy + 3, ACC(12), buf);

    /* uptime and dial */
    int secs = (int)(now_ms / 1000);
    int dx = gx + gw + 8, dw = cw - gw - 8;
    fmt_2d(buf, secs / 3600 % 100);
    strcat(buf, ":");
    fmt_2d(num, secs / 60 % 60);
    strcat(buf, num);
    strcat(buf, ":");
    fmt_2d(num, secs % 60);
    strcat(buf, num);
    txt(P_WIN, F_M, dx + (dw - txt_w(F_M, buf)) / 2, gy, N(15), buf);
    txt(P_WIN, F_S, dx + (dw - txt_w(F_S, "UPTIME")) / 2, gy + 14, N(8), "UPTIME");
    int cx = dx + dw / 2, cy = gy + 40, rad = 14;
    c93_fill_ellipse(P_WIN, cx, cy, rad, rad, N(2));
    c93_ellipse(P_WIN, cx, cy, rad, rad, ACC(7));
    for (int t = 0; t < 12; t++)
    {
        int a = t * 256 / 12;
        c93_plot(P_WIN, cx + isin(a) * (rad - 2) / 127, cy - isin(a + 64) * (rad - 2) / 127, N(12));
    }
    int sa = (int)(now_ms % 60000) * 256 / 60000;
    c93_line(P_WIN, cx, cy, cx + isin(sa) * (rad - 3) / 127, cy - isin(sa + 64) * (rad - 3) / 127, SEC(10));
    int ma = (secs / 60 % 60) * 256 / 60;
    c93_line(P_WIN, cx, cy, cx + isin(ma) * (rad - 6) / 127, cy - isin(ma + 64) * (rad - 6) / 127, N(15));

    /* memory map of the 16 MB address space */
    int my = gy + gh + 8;
    txt(P_WIN, F_S, x, my, N(10), "MEMORY MAP  16 MB");
    int mw = cw - 2, mbx = x, mby = my + 10;
    int os_w = mw * 8 / 256 + 2;                       /* 512 KB */
    int vram_w = mw * 16 / 256;                        /* top 1 MB */
    well(P_WIN, mbx, mby, mw, 12, N(1));
    c93_fill_rect(P_WIN, mbx + 1, mby + 1, os_w, 10, ACC(8));
    c93_fill_rect(P_WIN, mbx + 1 + os_w, mby + 1, mw - os_w - vram_w - 2, 10, N(4));
    c93_fill_rect(P_WIN, mbx + mw - vram_w - 1, mby + 1, vram_w, 10, SEC(8));
    for (int k = 1; k < 16; k++)
        c93_fill_rect(P_WIN, mbx + mw * k / 16, mby + 1, 1, 10, N(1));

    /* legend, laid out one after another */
    int ly = mby + 16, lx = x;
    strcpy(buf, "System ");
    fmt_int(num, (int)((unsigned int)__heap_start / 1024), 1);
    strcat(buf, num);
    strcat(buf, "K");
    c93_fill_rect(P_WIN, lx, ly + 1, 6, 6, ACC(8));
    lx += 9 + txt(P_WIN, F_S, lx + 9, ly, N(13), buf) + 10;
    c93_fill_rect(P_WIN, lx, ly + 1, 6, 6, N(4));
    lx += 9 + txt(P_WIN, F_S, lx + 9, ly, N(13), "Programs") + 10;
    c93_fill_rect(P_WIN, lx, ly + 1, 6, 6, SEC(8));
    txt(P_WIN, F_S, lx + 9, ly, N(13), "Video");

    if (ch > 140)
    {
        strcpy(buf, "Windows ");
        fmt_int(num, zcount, 0);
        strcat(buf, num);
        strcat(buf, "   Layers 4   Colours 1024");
        txt(P_WIN, F_S, x, ly + 14, N(9), buf);
    }
}

/* ================================================================ Calc */

int calc_value, calc_acc, calc_op, calc_new, calc_error, calc_pressed = -1, calc_pressed_t;
static const char *calc_labels[20] = { "C", "+/-", "%", "/", "7", "8", "9", "*", "4", "5", "6", "-", "1", "2", "3", "+", "0", "00", "<", "=" };

void calc_apply(void)
{
    int a = calc_acc, b = calc_value;
    switch (calc_op)
    {
        case '+': calc_value = a + b; break;
        case '-': calc_value = a - b; break;
        case '*': calc_value = a * b; break;
        case '/': if (b == 0) calc_error = 1; else calc_value = a / b; break;
        case '%': if (b == 0) calc_error = 1; else calc_value = a % b; break;
    }
    calc_op = 0;
}

void calc_press(int i)
{
    const char *l = calc_labels[i];
    calc_pressed = i;
    calc_pressed_t = 6;
    win_dirty = 1;
    if (l[0] == 'C') { calc_value = calc_acc = calc_op = calc_error = 0; calc_new = 1; return; }
    if (calc_error) return;
    if (l[0] >= '0' && l[0] <= '9')
    {
        if (calc_new) { calc_value = 0; calc_new = 0; }
        for (int k = 0; l[k]; k++)
            if (calc_value < 100000000 && calc_value > -100000000)
                calc_value = calc_value * 10 + (calc_value < 0 ? -(l[k] - '0') : l[k] - '0');
        return;
    }
    if (l[0] == '<') { calc_value /= 10; return; }
    if (l[0] == '+' && l[1] == '/') { calc_value = -calc_value; return; }
    if (calc_op && !calc_new) calc_apply();
    if (l[0] == '=') { if (calc_op) calc_apply(); calc_new = 1; return; }
    calc_acc = calc_value;
    calc_op = l[0];
    calc_new = 1;
}

void calc_draw(Win *w, int x, int y, int cw, int ch)
{
    char buf[24];
    (void)w;
    well(P_WIN, x, y, cw, 26, N(1));
    if (calc_error) strcpy(buf, "Error");
    else fmt_int(buf, calc_value, 1);
    txt(P_WIN, F_L, x + cw - 6 - txt_w(F_L, buf), y + 4, calc_error ? K_RED : ACC(12), buf);
    if (calc_op)
    {
        buf[0] = (char)calc_op;
        buf[1] = 0;
        txt(P_WIN, F_S, x + 4, y + 3, SEC(9), buf);
    }
    int bw = (cw - 6) / 4, bh = (ch - 32) / 5;
    for (int i = 0; i < 20; i++)
    {
        int bx = x + (i % 4) * (bw + 2), by = y + 30 + (i / 4) * (bh + 1);
        int op = i % 4 == 3 || i == 19;
        int hot = inside(mouse_x, mouse_y, bx, by, bw, bh - 1);
        int pressed = i == calc_pressed && calc_pressed_t > 0;
        if (op && !pressed)
        {
            bevel(P_WIN, bx, by, bw, bh - 1, hot ? SEC(8) : SEC(6), SEC(11), SEC(3));
            txt(P_WIN, F_S, bx + (bw - txt_w(F_S, calc_labels[i])) / 2, by + (bh - 9) / 2, K_WHITE, calc_labels[i]);
        }
        else
        {
            button(P_WIN, bx, by, bw, bh - 1, calc_labels[i], hot, pressed);
        }
    }
}

int calc_click(Win *w, int lx, int ly, int dbl, int right)
{
    (void)dbl; (void)right;
    int cw = w->w - RAIL_W - 8, ch = w->h - 8;
    int bw = (cw - 6) / 4, bh = (ch - 32) / 5;
    if (ly < 30) return 0;
    int col = lx / (bw + 2), row = (ly - 30) / (bh + 1);
    if (col >= 0 && col < 4 && row >= 0 && row < 5)
        calc_press(row * 4 + col);
    return 0;
}

void calc_key(void)
{
    static const int digit_index[10] = { 16, 12, 13, 14, 8, 9, 10, 4, 5, 6 };
    for (int d = 0; d < 10; d++)
        if (key_pressed('0' + d) || key_pressed(96 + d))
            calc_press(digit_index[d]);
    if (key_pressed(107)) calc_press(15);
    if (key_pressed(109) || key_pressed(189)) calc_press(11);
    if (key_pressed(106)) calc_press(7);
    if (key_pressed(111) || key_pressed(191)) calc_press(3);
    if (key_pressed(C93_KEY_ENTER) || key_pressed(187)) calc_press(19);
    if (key_pressed(C93_KEY_BACKSPACE)) calc_press(18);
    if (key_pressed(C93_KEY_ESCAPE) || key_pressed('C')) calc_press(0);
}

/* ================================================================ Sketch */

int sketch_color = 8, sketch_brush = 2, sketch_mirror, sketch_lx = -1, sketch_ly = -1, sketch_ready;

void sketch_clear(void)
{
    memset(WS_CANVAS, K_PAINT + 1, CANVAS_W * CANVAS_H);
    win_dirty = 1;
}

static void canvas_dot(int x, int y)
{
    int r = sketch_brush;
    for (int dy = -r / 2; dy <= r / 2; dy++)
        for (int dx = -r / 2; dx <= r / 2; dx++)
        {
            int px = x + dx, py = y + dy;
            if (px >= 0 && py >= 0 && px < CANVAS_W && py < CANVAS_H)
            {
                WS_CANVAS[py * CANVAS_W + px] = (unsigned char)(K_PAINT + sketch_color);
                if (sketch_mirror)
                    WS_CANVAS[py * CANVAS_W + (CANVAS_W - 1 - px)] = (unsigned char)(K_PAINT + sketch_color);
            }
        }
}

static void canvas_line(int x0, int y0, int x1, int y1)
{
    int dx = abs(x1 - x0), dy = -abs(y1 - y0);
    int sx = x0 < x1 ? 1 : -1, sy = y0 < y1 ? 1 : -1, err = dx + dy;
    for (;;)
    {
        canvas_dot(x0, y0);
        if (x0 == x1 && y0 == y1) break;
        int e2 = 2 * err;
        if (e2 >= dy) { err += dy; x0 += sx; }
        if (e2 <= dx) { err += dx; y0 += sy; }
    }
}

#define TOOLS_X (CANVAS_W + 6)

void sketch_draw(Win *w, int x, int y, int cw, int ch)
{
    (void)w; (void)cw; (void)ch;
    if (!sketch_ready) { sketch_clear(); sketch_ready = 1; }
    c93_rect(P_WIN, x - 1, y - 1, CANVAS_W + 2, CANVAS_H + 2, N(1));
    c93_draw_sprite(WS_CANVAS, P_WIN, x, y, CANVAS_W, CANVAS_H);

    /* tool column: 2 x 8 colour swatches, brush sizes, mirror and new */
    int tx = x + TOOLS_X;
    for (int i = 0; i < 16; i++)
    {
        int sx = tx + (i & 1) * 12, sy = y + (i >> 1) * 10;
        c93_fill_rect(P_WIN, sx, sy, 10, 9, K_PAINT + i);
        if (i == sketch_color) c93_rect(P_WIN, sx - 1, sy - 1, 12, 11, K_SNAP);
    }
    static const char *sizes[3] = { "S", "M", "L" };
    for (int i = 0; i < 3; i++)
        button(P_WIN, tx + i * 8, y + 83, 8, 10, sizes[i], 0, sketch_brush == 1 + i * 2);
    button(P_WIN, tx, y + 96, 24, 11, "Mir", 0, sketch_mirror);
    button(P_WIN, tx, y + 109, 24, 11, "New", inside(mouse_x, mouse_y, tx, y + 109, 24, 11), 0);
}

int sketch_click(Win *w, int lx, int ly, int dbl, int right)
{
    (void)w; (void)dbl; (void)right;
    if (lx >= TOOLS_X)
    {
        int tx = lx - TOOLS_X;
        if (ly < 80 && tx < 24) { sketch_color = (ly / 10) * 2 + (tx >= 12); win_dirty = 1; }
        else if (ly >= 83 && ly < 93 && tx < 24) { sketch_brush = 1 + (tx / 8) * 2; win_dirty = 1; }
        else if (ly >= 96 && ly < 107) { sketch_mirror = !sketch_mirror; win_dirty = 1; }
        else if (ly >= 109 && ly < 120) { sketch_clear(); spawn_sparks(mouse_x, mouse_y, 12, 0); }
        return 0;
    }
    if (lx >= 0 && lx < CANVAS_W && ly >= 0 && ly < CANVAS_H)
    {
        sketch_lx = lx;
        sketch_ly = ly;
        canvas_dot(sketch_lx, sketch_ly);
        win_dirty = 1;
        return 1;
    }
    return 0;
}

void sketch_drag(Win *w, int lx, int ly)
{
    (void)w;
    if (sketch_lx < 0) return;
    if (lx < -10 || ly < -10 || lx > CANVAS_W + 10 || ly > CANVAS_H + 10) return;
    if (lx != sketch_lx || ly != sketch_ly)
    {
        canvas_line(sketch_lx, sketch_ly, lx, ly);
        sketch_lx = lx;
        sketch_ly = ly;
        win_dirty = 1;
    }
}

/* ================================================================ Prefs */

void apply_theme(void);
void rebuild_backdrop(void);

void prefs_draw(Win *w, int x, int y, int cw, int ch)
{
    (void)w; (void)ch;
    txt(P_WIN, F_M, x, y, N(15), "Preferences");

    txt(P_WIN, F_S, x, y + 18, N(10), "Theme");
    txt(P_WIN, F_S, x + 44, y + 18, ACC(11), theme_names[prefs.theme]);
    for (int i = 0; i < 6; i++)
    {
        int sx = x + i * 34, sy = y + 28;
        if (i == prefs.theme) c93_fill_round_rect(P_WIN, sx - 2, sy - 2, 32, 22, K_SNAP, 5);
        c93_fill_round_rect(P_WIN, sx, sy, 28, 18, K_SWATCH + i, 4);
    }

    txt(P_WIN, F_S, x, y + 56, N(10), "Backdrop");
    for (int i = 0; i < 3; i++)
        button(P_WIN, x + i * 64, y + 68, 60, 13, backdrop_names[i], 0, prefs.backdrop == i);

    static const char *toggles[3] = { "Magnetic snapping", "Cursor sparkle trail", "Rails on the right" };
    int values[3];
    values[0] = prefs.snap;
    values[1] = prefs.trail;
    values[2] = prefs.rail_right;
    for (int i = 0; i < 3; i++)
    {
        int ty = y + 92 + i * 15;
        well(P_WIN, x, ty, 24, 11, values[i] ? ACC(6) : N(2));
        c93_fill_round_rect(P_WIN, values[i] ? x + 13 : x + 1, ty + 1, 10, 9, values[i] ? K_WHITE : N(8), 3);
        txt(P_WIN, F_S, x + 30, ty + 2, N(13), toggles[i]);
    }
}

int prefs_click(Win *w, int lx, int ly, int dbl, int right)
{
    (void)w; (void)dbl; (void)right;
    if (ly >= 28 && ly < 48 && lx < 6 * 34)
    {
        int t = lx / 34;
        if (t != prefs.theme)
        {
            prefs.theme = t;
            apply_theme();
            toast(theme_names[t], ACC(9));
        }
    }
    else if (ly >= 68 && ly < 81 && lx < 192)
    {
        int b = lx / 64;
        if (b != prefs.backdrop)
        {
            prefs.backdrop = b;
            rebuild_backdrop();
        }
    }
    else if (ly >= 92 && ly < 137 && lx < 160)
    {
        int i = (ly - 92) / 15;
        if (i == 0) prefs.snap = !prefs.snap;
        if (i == 1) prefs.trail = !prefs.trail;
        if (i == 2) prefs.rail_right = !prefs.rail_right;
    }
    else
    {
        return 0;
    }
    save_prefs();
    win_dirty = 1;
    return 0;
}

/* ================================================================ About */

void about_draw(Win *w, int x, int y, int cw, int ch)
{
    (void)w; (void)ch;
    int cx = x + cw / 2;
    c93_fill_round_rect(P_WIN, x, y, cw, 44, N(1), 6);
    txt_rainbow(P_WIN, F_L, cx - 52, y + 6, "HALCYON", 1);
    txt_center(P_WIN, F_S, cx, y + 30, N(9), "version 1.0  -  a retro desktop");
    static const char *lines[7] = {
        "Written in C, compiled on the machine",
        "by Continuum 93's own C compiler.",
        "",
        "4 video layers, 1,024 colours at once.",
        "Drag a rail: windows snap together.",
        "Double-click a rail to fold it away,",
        "right-click it to swap sides.",
    };
    for (int i = 0; i < 7; i++)
        txt_center(P_WIN, F_S, cx, y + 52 + i * 11, i < 2 ? N(14) : N(10), lines[i]);
}

/* ================================================================ Shutdown dialog */

void dialog_draw(Win *w, int x, int y, int cw, int ch)
{
    (void)w; (void)ch;
    txt(P_WIN, F_M, x + 4, y + 4, N(15), "Leave Halcyon?");
    txt(P_WIN, F_S, x + 4, y + 22, N(10), "Restart shows the boot menu.");
    int bw = (cw - 8) / 3;
    button(P_WIN, x, y + 42, bw, 15, "Cancel", inside(mouse_x, mouse_y, x, y + 42, bw, 15), 0);
    button(P_WIN, x + bw + 4, y + 42, bw, 15, "Restart", inside(mouse_x, mouse_y, x + bw + 4, y + 42, bw, 15), 0);
    button(P_WIN, x + 2 * (bw + 4), y + 42, bw, 15, "Shut down", inside(mouse_x, mouse_y, x + 2 * (bw + 4), y + 42, bw, 15), 0);
}

int dialog_click(Win *w, int lx, int ly, int dbl, int right)
{
    int cw = w->w - RAIL_W - 8, bw = (cw - 8) / 3;
    (void)dbl; (void)right;
    if (ly < 42 || ly > 57) return 0;
    if (lx < bw) close_window(w);
    else if (lx < 2 * bw + 4) request_boot_menu();
    else request_shutdown();
    return 0;
}

/* ================================================================ dispatch */

Win *find_app(int app)
{
    for (int i = 0; i < MAX_WIN; i++)
        if (wins[i].used && wins[i].app == app)
            return &wins[i];
    return 0;
}

Win *open_app(int app, int fromx, int fromy)
{
    Win *w = 0;
    if (app != APP_DRAWERS)
    {
        w = find_app(app);
        if (w)
        {
            focus_window(w);
            if (w->folded || w->fold_dir < 0) toggle_fold(w);
            return w;
        }
    }
    switch (app)
    {
        case APP_DRAWERS: return open_drawer("", fromx, fromy);
        case APP_PULSE: w = new_window(app, "Pulse", 286, 168, fromx, fromy); if (w) w->live = 1; break;
        case APP_CALC: w = new_window(app, "Calc", 150, 168, fromx, fromy); calc_new = 1; break;
        case APP_SKETCH: w = new_window(app, "Sketch", CANVAS_W + RAIL_W + 8 + 6 + 26, CANVAS_H + 16, fromx, fromy); break;
        case APP_PREFS: w = new_window(app, "Prefs", 228, 150, fromx, fromy); break;
        case APP_ABOUT: w = new_window(app, "About", 296, 150, fromx, fromy); break;
        case APP_DIALOG: w = new_window(app, "Power", 262, 74, fromx, fromy); break;
        case APP_READER: w = new_window(app, "Reader", 300, 190, fromx, fromy); break;
    }
    return w;
}

void app_init(Win *w) { (void)w; }

void app_draw(Win *w, int x, int y, int cw, int ch)
{
    switch (w->app)
    {
        case APP_DRAWERS: drawers_draw(w, x, y, cw, ch); break;
        case APP_READER: if (w == reader_win) reader_draw(w, x, y, cw, ch); break;
        case APP_PULSE: pulse_draw(w, x, y, cw, ch); break;
        case APP_CALC: calc_draw(w, x, y, cw, ch); break;
        case APP_SKETCH: sketch_draw(w, x, y, cw, ch); break;
        case APP_PREFS: prefs_draw(w, x, y, cw, ch); break;
        case APP_ABOUT: about_draw(w, x, y, cw, ch); break;
        case APP_DIALOG: dialog_draw(w, x, y, cw, ch); break;
    }
}

int app_click(Win *w, int lx, int ly, int dbl, int right)
{
    switch (w->app)
    {
        case APP_DRAWERS: return drawers_click(w, lx, ly, dbl, right);
        case APP_READER: return reader_click(w, lx, ly, dbl, right);
        case APP_CALC: return right ? 0 : calc_click(w, lx, ly, dbl, right);
        case APP_SKETCH: return right ? 0 : sketch_click(w, lx, ly, dbl, right);
        case APP_PREFS: return right ? 0 : prefs_click(w, lx, ly, dbl, right);
        case APP_DIALOG: return right ? 0 : dialog_click(w, lx, ly, dbl, right);
    }
    return 0;
}

void app_drag(Win *w, int lx, int ly)
{
    if (w->app == APP_SKETCH) { sketch_drag(w, lx, ly); return; }
    (void)lx;
    if (w->app == APP_DRAWERS) list_scroll_drag(w, ly, 15, w->count, drawers_rows(w));
    if (w->app == APP_READER) list_scroll_drag(w, ly, 15, reader_lines, reader_rows(w));
}

void app_release(Win *w)
{
    if (w->app == APP_SKETCH) sketch_lx = -1;
}

void app_key(Win *w)
{
    switch (w->app)
    {
        case APP_DRAWERS: drawers_key(w); break;
        case APP_READER: list_keys(w, reader_lines, reader_rows(w), 0); break;
        case APP_CALC: calc_key(); break;
    }
}

void app_tick(Win *w)
{
    if (w->app == APP_CALC && calc_pressed_t > 0 && --calc_pressed_t == 0)
        win_dirty = 1;
}

void app_close(Win *w)
{
    if (w == reader_win) reader_win = 0;
}

void menu_action_app(int tag, int item)
{
    int idx = tag - 100;
    if (idx >= 0 && idx < MAX_WIN && wins[idx].used && wins[idx].app == APP_DRAWERS)
        drawer_context(&wins[idx], item);
}
