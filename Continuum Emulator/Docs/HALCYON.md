# Halcyon OS and the dual-boot selector

Halcyon is a visual desktop for Continuum 93, written in C and compiled on the machine by the built-in C
compiler every time it boots. Q OS, the classic file browser, is still available from the boot selector.

## Booting

At power-on the **boot selector** (`Data/os/boot/boot.c`) offers two systems:

| Key | Action |
|---|---|
| (nothing for 3 seconds) | Halcyon starts |
| Left / Right, or move the mouse over a card | select a system and stop the countdown |
| Enter, Space or click | start the selected system |
| 1 / 2 | start Halcyon / Q OS immediately |

The boot program is configured in `Data/init.cfg` (`bootProgram=Data\os\boot\boot.c`). Set it to
`Data\os\q.asm` to boot Q OS directly. Configurations that still had the old default are upgraded to the
selector automatically.

## Using Halcyon

- **Menu bar** (top): *Halcyon* (About, Preferences, apps, Restart, Shut down), *Windows* (fold, unfold,
  tile, close all), *Go* (common folders) - plus the active window title and the uptime clock.
- **Dock** (bottom): Drawers, Pulse, Calc, Sketch, Prefs, About and Power. Icons magnify under the mouse;
  a dot shows running apps (amber when the window is folded).
- **Desktop icons**: System (the filesystem root), Programs, Fonts, Tests and Read me. Double-click to open.

### Windows

Each window has a **control rail** on its side:

| Rail part | Action |
|---|---|
| red / amber / green dots | close / fold / zoom (maximize or restore) |
| the rail itself | drag to move; double-click to fold or unfold |
| right-click on the rail | move the rail to the other side of the window |
| bottom-right corner of the body | resize |

- **Magnetic snapping**: while you drag, window edges snap to the screen edges and to the edges of other
  windows (including alignment), with a glowing guide line and sparks on contact. Toggle it in Prefs.
- **Folding**: a folded window collapses like an accordion into its rail, leaving a slim spine with the
  vertical title. Spines can be dragged and snapped like any window.
- Tab cycles through windows; the keyboard goes to the active window.

### Apps

| App | What it does |
|---|---|
| Drawers | Browse the filesystem. Double-click a folder to enter it, a `.asm`, `.c` or `.bas` file to run it, and text files to read them. Right-click a file for Open/Run, View source and Info. Up/Down, Enter, Backspace, Page Up/Down work too. |
| Reader | Text viewer with line numbers and light syntax colouring for C, assembly and BASIC. |
| Pulse | Live system monitor: frame-time bars and CPU load, uptime with an analog dial, and a map of the 16 MB address space. |
| Calc | Integer calculator (mouse or keyboard). |
| Sketch | Paint with 16 colours, three brush sizes and a mirror mode. |
| Prefs | Six colour themes, three backdrops (Aurora, Copper, Nebula), and toggles for snapping, the pointer sparkle trail and default rail side. Saved to `halcyon.prefs` in the filesystem root. |

### Running programs

Programs run full-screen exactly as they would from Q OS (3 freshly reset video pages, presented
automatically). When a program returns, Halcyon resumes where it was - same windows, same folders. Build
errors are reported in a notification; the compiler and assembler logs are in the program's `debug` folder.

## How it works

### Video

Halcyon uses four video layers, each with its own 256-colour palette:

| Layer | Content | Redrawn |
|---|---|---|
| 0 | backdrop: plasma patterns and a mountain silhouette | once; animated by palette cycling |
| 1 | desktop icons | when they change |
| 2 | windows | when they change (Pulse every third frame) |
| 3 | menu bar, dock, menus, notifications, effects, pointer | every frame |

Much of the motion is palette animation (backdrop colour cycling, twinkling stars, the pulsing active
window outline, the flowing rainbow logo), which costs almost nothing per frame.

### Memory map and the boot protocol

| Address | Use |
|---|---|
| `0x000000 - 0x03FFFF` | OS image (Halcyon code and data, or Q OS) |
| `0x040000 - 0x04EFFF` | Halcyon's C stack (`#pragma c93 stack 0x04F000`) |
| `0x050000 - 0x06FFFF` | Halcyon workspace: fonts, directory listings, Reader document, Sketch canvas |
| `0x070000 - 0x07EFFF` | boot selector and its stack |
| `0x07FD00` | boot mailbox (`Data/os/boot/bootproto.h`) |
| `0x080000 +` | user programs |

To run a program Halcyon writes the path into the mailbox and returns to the boot selector, which builds
the program (INT 0x00 with 0xC0, 0xC1 or 0xC2 by extension) and calls it with every register saved. It
then checksums Halcyon's image: if it is intact Halcyon is re-entered and resumes; if the program overwrote
it (some demos load below `0x080000`), Halcyon is rebuilt from source and reopens the program's folder.

### Source layout (`Data/os/halcyon`)

| File | Content |
|---|---|
| `halcyon.c` | entry point, main loop, boot splash, resume, scripted test mode |
| `hal_core.h` | layout constants, palette indexes, shared structures |
| `hal_gfx.c` | themes and palettes, text, bevels, icons |
| `hal_backdrop.c` | backdrop rendering and colour cycling |
| `hal_fx.c` | sparks, ripples, zoom boxes, notifications, CRT transitions |
| `hal_wm.c` | window manager: rails, snapping, folding, zoom, resize |
| `hal_apps.c` | the applications |
| `hal_desk.c` | menu bar, menus, dock, desktop icons, pointer |

Adding an app: add an `APP_*` value in `hal_core.h`, draw/click handlers in `hal_apps.c` (wired up in
`app_draw` / `app_click` / `open_app`), and optionally a dock entry in `hal_desk.c`.

### Tests

`ContinuumUnitTests/CCompilerTests/COsTests.cs` compiles Halcyon with `HAL_SCRIPT` defined, which replaces the
mouse and keyboard with scripted scenes (open apps, drag a window until it snaps, fold one, paint, use Calc,
right-click, request a program run) and checks the results, the memory map limits and the boot selector.
