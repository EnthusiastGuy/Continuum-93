/*
 * Halcyon OS - core definitions shared by every module.
 */

#ifndef HAL_CORE_H
#define HAL_CORE_H

/* ---------------------------------------------------------------- screen and layers */

#define SCREEN_W    480
#define SCREEN_H    270
#define BAR_H       14              /* menu bar height */
#define DOCK_H      34
#define DOCK_Y      (SCREEN_H - DOCK_H)
#define DESK_TOP    (BAR_H + 2)
#define DESK_BOTTOM (DOCK_Y - 2)

/* Four video layers, back to front, each with its own 256-colour palette */
#define P_BACK  0                   /* animated backdrop (palette cycling)       */
#define P_DESK  1                   /* desktop icons                             */
#define P_WIN   2                   /* windows                                   */
#define P_TOP   3                   /* menu bar, dock, menus, cursor and effects */
#define ALL_PAGES 15

/* ---------------------------------------------------------------- workspace (outside the OS image) */

#define WS_FONT_S     ((unsigned char *)0x050000)
#define WS_FONT_M     ((unsigned char *)0x052000)
#define WS_FONT_L     ((unsigned char *)0x054000)
#define WS_DIRS       ((char *)0x058000)          /* 8 directory listings of 3 KB */
#define WS_DIR_SIZE   3072
#define WS_TEXT       ((char *)0x05E000)          /* Reader document (40 KB) */
#define WS_TEXT_MAX   40900
#define WS_LISTING    ((char *)0x068000)          /* raw directory listing scratch (8 KB) */
#define WS_LISTING_MAX 8000
#define WS_CANVAS     ((unsigned char *)0x06A000) /* Sketch canvas */
#define CANVAS_W      200
#define CANVAS_H      112

/* ---------------------------------------------------------------- palette of the UI layers (1-3) */

#define N(i)   (1 + (i))            /* 16 neutrals, 0 = darkest */
#define ACC(i) (20 + (i))           /* 16 accent shades (theme) */
#define SEC(i) (36 + (i))           /* 16 secondary shades (theme) */

enum
{
    K_PULSE = 52,       /* animated: active window glow  */
    K_SEL,              /* animated: selection highlight */
    K_SNAP,             /* animated: magnetic snap guide */
    K_SHADOW,
    K_GLOW,             /* animated: soft glow */
    K_HOT,              /* hover highlight */
    K_RAINBOW = 58,     /* 8 cycling hues 58..65 */
    K_FOLDER = 66, K_FOLDER_HI, K_FOLDER_LO, K_ASM, K_CSRC, K_BAS, K_TEXTDOC, K_IMG, K_FONT, K_OTHER,
    K_RED, K_AMBER, K_GREEN, K_WHITE, K_BLACK,
    K_CHART = 84,       /* 16 chart colours */
    K_PAINT = 100,      /* 16 paint colours */
    K_SPARK = 116,      /* 16 spark fade steps */
    K_SWATCH = 132      /* 6 theme swatches */
};

/* Backdrop palette (layer 0) */
#define B_BAND(b, i) (1 + (b) * 32 + (i))   /* 4 brightness bands x 32 cycling colours */
#define B_STAR       140                    /* 16 twinkle levels */
#define B_HILL       160                    /* 32 silhouette shades */

/* ---------------------------------------------------------------- fonts */

#define F_S 0   /* ModernDOS 8x8   */
#define F_M 1   /* ModernDOS 9x13  */
#define F_L 2   /* ModernDOS 12x18 */

/* ---------------------------------------------------------------- windows */

#define MAX_WIN   8
#define RAIL_W    16
#define MIN_W     120
#define MIN_H     70

enum { APP_NONE, APP_DRAWERS, APP_READER, APP_PULSE, APP_CALC, APP_SKETCH, APP_PREFS, APP_ABOUT, APP_DIALOG, APP_COUNT };

typedef struct
{
    int used;
    int app;
    int x, y, w, h;             /* outer rectangle when unfolded */
    int rail_right;             /* controls rail on the right side instead of the left */
    int folded;                 /* folded into its rail */
    int open_t;                 /* accordion openness 0..256 */
    int fold_dir;               /* -1 folding, +1 unfolding */
    int maxed, rx, ry, rw, rh;  /* zoom restore rectangle */
    int scroll, sel;
    int live;                   /* redraw every frame (animated content) */
    char title[32];

    /* Drawers */
    char path[160];
    int count, folders;
    unsigned short name_ofs[160];
    unsigned char kinds[160];
    unsigned int sizes[160];
} Win;

/* File kinds */
enum { FK_DIR, FK_ASM, FK_C, FK_H, FK_BAS, FK_TEXT, FK_IMG, FK_FONT, FK_OTHER };

typedef struct
{
    unsigned int magic;
    int theme;
    int backdrop;
    int trail;
    int snap;
    int rail_right;
} Prefs;

#define PREFS_MAGIC 0x48414C31u     /* "HAL1" */
#define PREFS_FILE  "halcyon.prefs"

/* ---------------------------------------------------------------- shared state */

extern int frame;
extern unsigned int now_ms;
extern int mouse_x, mouse_y, mouse_b, mouse_pb;
extern int clicked, released, dbl_clicked, rclicked;
extern unsigned char keys[32], prev_keys[32];
extern int desk_dirty, win_dirty;
extern Prefs prefs;
extern Win wins[MAX_WIN];
extern int zorder[MAX_WIN], zcount;
extern int brightness;          /* 0..256 palette fade */

/* ---------------------------------------------------------------- cross-module functions */

int key_pressed(int k);
int key_down(int k);
int txt(int page, int font, int x, int y, int color, const char *s);
int txt_clip(int page, int font, int x, int y, int color, const char *s, int maxw);
int txt_w(int font, const char *s);
int font_h(int font);
void toast(const char *s, int color);
void spawn_sparks(int x, int y, int count, int hue);
void zoom_box(int x0, int y0, int w0, int h0, int x1, int y1, int w1, int h1);
void apply_palettes(void);
void build_backdrop_rows(int y0, int y1);
void animate_backdrop(void);
Win *open_app(int app, int fromx, int fromy);
Win *open_drawer(const char *path, int fromx, int fromy);
void open_reader(const char *path, int fromx, int fromy);
void close_window(Win *w);
void focus_window(Win *w);
Win *active_window(void);
void request_run(const char *path);
void request_boot_menu(void);
void request_shutdown(void);
void save_prefs(void);
void draw_app_icon(int page, int app, int x, int y, int s);

#endif
