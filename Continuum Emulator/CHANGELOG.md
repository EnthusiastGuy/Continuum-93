# Continuum 93

> **Version history** (please include your name or handle when updating this)

---

## 2.1.220 — ?
**Contributed by:** *EnthusiastGuy*

- Small refactor on the AND instructions. Nothing essential changed, just cleaned up the codebase a bit.
- In preparation to deprecating Continuum Tools: Implemented an integrated Service Mode overlay (accessible always with F1) with a smooth animated transition
- The emulator now renders using an aspect-correct destination rectangle with automatic letterboxing/pillarboxing to preserve the 16:9 ratio on any resolution, and it automatically switches between pixel-perfect PointClamp scaling for integer multiples and LinearClamp for non-integer scales to avoid visual artifacts on awkward resolutions.

## 2.1.212 — 25.10.2025

**Contributed by:** *EnthusiastGuy*

- Continuum is now **open source**! The source code is available on GitHub. As such, quite a bit of internal refactoring has been done to prepare the codebase for public release.
- Upgraded the MonoGame project from **.NET 6.0** to **.NET 8.0**, leveraging **native AOT** compilation to achieve performance improvements of up to **20%** and streamline platform-specific build processes.
- Further increased the speed of Continuum (an additional **25%**) by refactoring the architecture and implementing optimizations.
- Reduced memory consumption by **~15%**.
- Added floating-point support to the `RAND` (random) instruction as well as the ability to **seed** the random number generator.
- Added a **hardware (system) variables page** the user can access.
  - This features a raw address space in a separate memory module consisting of **64k variables of 32-bit** each that the user can read or write with the new instructions `SETVAR` and `GETVAR`.
  - The upper side of that page is dedicated to variables the system uses. Right now, only the last variable is used (`ERROR_HANDLER_ADDRESS`). This accepts an address the user can specify where Continuum jumps when encountering a stack overflow or underflow error. Default is zero.
- Added `SETVAR` and `GETVAR` instructions (as per the paragraph above).
- Added `GETBITS` and `SETBITS` instructions that can manipulate memory **bitwise** instead of bytewise.
- Added `FREGS` instruction that allows switching between **256 pages of floating-point registers** (equivalent to `REGS` for regular registers).
- Added `DJNZ` instruction that decrements a register or memory location and **jumps if not zero**.
- Added more variations for the `ADD` and `SUB` instructions to work directly on **memory locations**.
- Refactored `LD` instructions to implement **memory-to-memory** operations. Also added **indexed addressing** using immediate values or registers.
- Modified the `PUSH`/`POP` instructions to accept **memory locations** as well. This means you can now push or pop a register **or** memory location to/from the stack. **Opcodes changed**; assembly output is **not backward compatible**.
- Changed the opcode format for `ABS`, `CEIL`, `FLOOR` and `ROUND` for clearer bit separation. **Assembly output is not backward compatible.**
- Improved the **Q operating system** to report stack overflow errors.
- Added a mechanism to **control stack overflow** handling by implementing a customizable fallback memory address where the CPU will jump when the stack is full. It also **clears all stacks** before jumping. By default, that address is `0x000000` (the OS start address) but it can be changed with `SETVAR`.
- Added **numeric → string** conversion interrupts with custom formats:
  - `INT 0x05`, function `0x01` — **float → string**
  - `INT 0x05`, function `0x02` — **uint → string**
  - `INT 0x05`, function `0x03` — **int → string**
- Added `INT 0x00`, function `0xF0`: **GetCPUDesignationByFrequency**. Takes an input frequency and returns a designation of a Continuum CPU featuring that frequency. Used to standardize frequency ranges across platforms.
- Added `INT 0x01`, function `0x08`: **DrawFilledRoundedRectangle**.
- Added `INT 0x01`, function `0x09`: **DrawRoundedRectangle**.
- Improved `INT 0x01`, function `0x0E` (**DrawTileMapSprite**) to:
  - **Repeat** sprites horizontally, vertically, or both.
  - Add **rotation** in increments of **22.5°** (16 directions: 0, 22.5, 45, 67.5, 90, etc.).
  - Support **horizontal/vertical tiling**.
- Fixed a bug that messed up arguments in the `#DB` directive when strings contained commas.

---

## 1.0.135 — 01.11.2024

- Added **audio** capabilities. Implemented a draft **approximation** of JSFXR which produces complex sounds based on a multitude of parameters.
- Added specialized assembly instructions: `RGB2HSL`, `HSL2RGB`, `RGB2HSB`, `HSB2RGB` for color space conversions.
- Changed the way **division by zero** works:
  - If division by zero is performed, the result register obtains the **maximum value** for that capacity; if a remainder register is available, it is set to **zero**; the **carry flag** is set.
  - Divisions that don't divide by zero **clear** the carry flag.

### Interrupts

- Added `INT 0x04`, function `0x33` (**Load 8-bit PNG**) to load palette-based PNG files directly to memory and `INT 0x04`, function `0x34` (**Load 8-bit PNG with custom transparency**) to specify a transparency key when loading a PNG.
- Added `INT 0x04`, function `0x35` (**Merge 8-bit PNG**) and `INT 0x04`, function `0x36` (**Merge 8-bit PNG with Custom Transparency**) for fast manipulation of PNG files.
- Added `INT 0x04`, function `0x40` (**Load PNG font**) which loads fonts directly from PNG files and optionally kerning pairs from a text file with the same name.
- Added an extra parameter for `INT 0x01`, function `0x0E` (**DrawTileMapSprite**) to allow optionally drawing tiles flipped on **X**, **Y**, or both.
- Added `INT 0x01`, function `0x24` (**Line Path**) to draw a path made up of consecutive lines. Also added a demo program.
- Added `INT 0x01`, function `0x25` (**Draw Bezier Path**) which takes points and control points and draws a line with configurable color, thickness and “dotness”. Useful for game UI maps.
- Added `INT 0x01`, function `0x14` (**Draw text**) which works with PNG fonts loaded by `INT 0x04`, function `0x40`. This aims to eventually obsolete `DrawString`, with improvements such as **monospacing**, **centering monospace characters**, **kerning**, **centering text**, **wrap**, **outline**, and **shadows**. Returns **metrics** indicating width and height of the drawn text.
- Added `INT 0x01`, function `0x28` (**Fill area or polygon**).
- Added `INT 0x01`, function `0x40` (**Scroll/Roll**) to scroll or roll the contents of a rectangle on either video page in X/Y or both directions.
- Added `INT 0x01`, function `0x41` (**Copy rectangle**) to copy content from a source rectangle on any video layer to any other video layer (including the source), with optional **scaling**.

### Improvements & Fixes

- Enabled `MUL` to set **carry** if multiplication overflows the target register and reset **carry** otherwise.
- Improved `#DB` directive to support **floating-point numbers**.
- Improved compiler error reporting to show the failing **instruction** in the log along with the wrong instruction format.
- Improved compiler `INT 0x00`, function `0xC0` to report the **number of errors** found (capped to 255; if over 250, display “Errors: 250+”).
- Updated Continuum OS to prevent running files compiled with errors.
- Added `disableMouse` setting to allow disabling the mouse API for some Linux OSs that cannot handle mouse correctly.
- Fixed assembler ignoring labels for instructions: `MEMC`, `MEMF`, `CP`, `DIV`, `MUL`, `FIND`, `ISQR`, `POW`, `SQR`, `REGS`, `VCL`, `VDL`.
- Fixed wrong color counts on PNG merging interrupts.
- Fixed `DrawTileMapSprite` and `DrawSpriteToVideoPage` incorrectly overwriting pixels with transparency—transparency is now respected.
- Fixed multiplication by zero edge case.
- Updated the gamepad demo for the improved **Draw Tile Map Sprite** interrupt.
- Started adding **detailed explanations** on assembly instructions to the user manual (covered so far: `LD`, `ADD`, `DIV`, `MUL`, `SL`, `SR`, `RL`, `RR`).

---

## 0.7.116 — 14.02.2024

- Fixed the 3D view of Continuum Tools to not show residual texture layers when changing video layer counts.
- Fixed a GPU bug that was reading palette colors in the wrong order.

---

## 0.7.114 — 12.02.2024

- Changed versioning’s last number to reflect the number of **global commits**.
- Added `RETIF ff` instruction (returns if specified flag is set).
- Changed bitcodes for several instructions: `INV`, `SL`, `SR`, `RL`, `RR`, `SET`, `RES`, `AND`, `OR`, `XOR`, `NAND`, `NOR`, `XNOR`, `IMPLY`, `INV`, `EX`, `CP`.
- Added more variations for several instructions—`AND`, `OR`, `XOR`, `IMPLY`, `NAND`, `NOR`, `XNOR`, `INC`, `DEC`—to handle **memory** operations.
- Added `REGS r` and `REGS (rrr)` variations to allow **dynamic register page switching**.
- Added `LDREGS` (load regs from memory) and `STREGS` (store regs to memory).
- Added an explicit `enableDebugging` setting in `init.cfg` (default **true**).
- Added missing tests on logical instructions and more debugger tests.

**Fixes & Cleanup**

- Fixed issues in the assembly reference documentation generator.
- Fixed misleading info in the interrupt reference documentation generator.
- Fixed a bug that fed wrong instruction info to Tools in some cases.
- Fixed a debugger infinite-loop lock when an exception was thrown.
- Removed all `LD` instructions that handled registers or **calls stack**.

---

## 0.6.8 — 26.01.2024

### Features

- Enabled Continuum to work on **Raspberry Pi** (tested on RPi 4, RPi 400).
- Refactored compiler, interpreter, interrupts and pathing for **cross-platform** operation.
- Replaced a Windows-specific mouse handler with a **cross-platform** implementation.
- Added a machine interrupt to **shut down** the emulator. Updated the Q OS to shut down on **RCtrl+Del**.
- Improved the Q OS to be controlled by a **gamepad** (only on gamepad index 0).
- Added `INT 0x01`, function `0x20` (**Plot**) to plot points on video pages.
- Added video interrupts for more draw control:
  - `INT 0x01`, function `0x30` (**Read layers visibility**)
  - `INT 0x01`, function `0x31` (**Set layers visibility**)
  - `INT 0x01`, function `0x32` (**Read Buffer Control Mode**)
  - `INT 0x01`, function `0x33` (**Set Buffer Control Mode**)
- Implemented instruction `VDL` to force update of video buffers from RAM per layer. Intended for manual **Buffer Control Mode** workflows.
- **Deprecated/removed** `ER` and `DR` (use `VDL` + buffer control instead).
- Added `SETF f`, `RESF f`, `INVF f` to manipulate specific flags.
- Added `LDF r`, `LDF r, n`, `LDF (rrr)`, `LDF (rrr), n`, `LDF (nnn)`, `LDF (nnn), n` to load flags as a byte to an 8-bit register or memory location referred by a 24-bit register or immediate address. Variants with `n` mask the result via **AND**.

### Floating-Point Additions

- Implemented **16 floating-point registers** (`F0` → `F15`, 32-bit IEEE-754-like) and extended instructions to use them: `LD`, `ADD`, `SUB`, `DIV`, `MUL`, `EX`, `CP`. Modified `MIN`, `MAX`. Added: `SIN`, `COS`, `POW`, `SQR`, `CBR` (cube root), `ISQR` (inverse square root), `ISGN` (invert sign), `ABS`, `ROUND`, `FLOOR`, `CEIL`. (Infinity/NaN not used.)

### Gamepad Support

- `INT 0x02`, function `0x14` (**ReadGamePadsState**) — access up to 4 controllers (41 bytes for all 4).
- `INT 0x02`, function `0x15` (**ReadGamePadsCapabilities**) — capabilities (17 bytes for all 4).
- `INT 0x02`, function `0x16` (**ReadGamePadsNames**) — names as null-terminated strings (max 256 bytes for all 4).

### Image Loading

- `INT 0x04`, function `0x30` (**LoadImageAndPalette**) — loads image, converts to indexed color, deposits palette & image near input address.
- `INT 0x04`, function `0x31` (**LoadImage**) — loads image, converts to indexed color, deposits at specified address.
- `INT 0x04`, function `0x32` (**LoadPalette**) — loads an image’s palette at specified address.

### Sprite Drawing

- `INT 0x01`, function `0x0E` (**Draw Tile Map Sprite**) — simple drawing from tilemap loaded with `0x04:0x30/31`.

### Signed Numbers

- Added support for declaring **signed numbers** to simple registers (two’s complement). E.g., `LD A, -1` loads `0xFF` to `A`. Minus sign works with decimal, hex, binary, octal.
- Added `SDIV` and `SMUL` for signed division and multiplication.
- Added `SCP` (signed compare).
- Updated Tools to handle signed numbers where applicable.

### Graphics & Coordinates

Improved interrupts `0x01:0x06` (Draw filled rectangle), `0x01:0x0E` (Draw tile map sprite), `0x01:0x10` (Sprite draw), `0x01:0x12` (Draw text) to support **signed x/y** for partial off-screen drawing and **clipping**.

Implemented new graphics interrupts:
- `0x01:0x07` (**Draw rectangle**)
- `0x01:0x21` (**Line**)
- `0x01:0x22` (**Ellipse**)
- `0x01:0x23` (**Filled ellipse**)

### Logic & Tools

- Added logical instructions: `NAND`, `NOR`, `XNOR`, `IMPLY`.
- Tools/Debugger:
  - Support for floating-point registers and flag visualization.
  - New view to observe color palettes and a **3D model** of stacked video layers.
  - History stack (always visible) and a menu to toggle sections.
- Demos: **Bubble Universe** & **3D Cube** to exercise FP & trig.
- Fonts: **SlickAntsContourSlim** (narrow), **DoctorJack** (bold).
- Heavily improved the **Assembly Reference** documentation.

### Fixes

- Correct label calculation for `ADD`/`SUB` when addressing labeled addresses.
- Implemented missing variants: `ADD rr, r` and `SUB rr, r`.
- Fixed a race condition that could render an invalid frame when switching apps; such a frame is now **dropped**.
- Fixed documentation generator issues; `WAIT` now correctly documented.
- Fixed compiler parsing of **LF** line endings.
- Fixed `Plot` interrupt bug.
- Fixed culture-dependent floating number parsing.
- Fixed string drawing interrupt assuming font start offset by one pixel, causing artifacts.

### Misc

- Implemented a quick ad-hoc logging system.
- Added `bubble-universe.asm` and `gamepad-test-asm` demos.
- Improved OS font.
- Improved Assembly Reference with a simpler instruction list before detailed specs.
- Updated & improved interrupts documentation.

---

## 0.3.7 — 01.09.2023

### Features

- Replaced the original OS with a more **visual and user-friendly** one.
- Refactored the keyboard sample assembly file.
- Improved `DrawString`: pixels exceeding screen X/Y are skipped for cleaner output.
- Added `ListDirectoriesAndFilesInDirectory` interrupt for faster unified directory listing.
- Added **Get File Size** interrupt function to the FileSystem suite.
- Improved Video/Draw String interrupt to accept a **max width** and return a **flag** indicating whether width was exceeded.
- Added **repetition** macro to the `#DB` directive.
- Added `#include <relative path>` to split code across files (labels must still be unique across files).
- Enabled log output, single file source output, and assembled file generation for `.asm` files compiled through the Machine interrupt (created in the **debug** directory next to the main entry file).
- Window resize now snaps the display ratio to **16:9** while keeping the intended size.
- Fixed icons for both the Emulator and Tools.
- Startup scaled resolution is now **2×**.
- Improved the logging system.
- Fixed a bug that prevented debug mode from switching off due to aggressive optimization.
- Fixed CPU running speed behavior.
- Added a config setting to **toggle logging**.
- Implemented a benchmarking class that calculates **relative duration** of all Continuum assembly instructions.
- Implemented a few visual shaders (feature postponed until a unified strategy/API is established).
- Fixed logging between async processes.

### Continuum Tools

- Removed the `0x` prefix on all hex representations to reduce clutter.
- Changed how registers are shown; included a preview for **24-bit registers** to glance at pointed memory.
- Implemented a **hex memory viewer** with clickable input from debugger (labels) and instructions that manipulate addresses; integrates with register clicks and mouse-wheel scrolling.
- Slightly redesigned the **Stacks View** for efficiency.
- Fixed crashes on certain illegal instructions.
- Added ability to enter **step-by-step** mode when the emulator hits a `DEBUG` instruction.
- Added a mechanism to release the emulator from step-by-step mode if Tools is closed mid-session.
- Added **Resume** execution (disable step-by-step) via **F8** in Tools.
- Clear visualization in both Tools and Emulator to distinguish step-by-step mode.
- Hover effects and many UX/UI improvements.
- Prevented registering mouse input when the window is not active.
- Fixed wrong addresses shown for `CALLR` and `JR` (now shows both relative **and** resulting addresses).
- Slight font improvement.

### Bugfixes

- Fixed several erroneous compile-time error reports in the log.
- Fixed compiler bug where all `LD16`, `LD24`, `LD32` ignored labels.
- Fixed includes not attaching properly when compiled from interrupt.
- Fixed `#DB ""` (empty string) being treated as error; now ignored.
- Fixed `SettingsManager` not showing `bootProgram` option when creating a new default settings file.

### Misc

- Updated the user manual adding all video layer combinations for memory mapping.
- Updated the interrupts manual with `ListDirectoriesAndFilesInDirectory`.
- Updated the interrupts manual with the improved Video/Draw String.
- Updated the user manual with the `#DB` **repeat** operation.
- Improved and updated the assembly reference manual with missing definitions.
- Added generated example instruction lists of all supported instructions (included as a text file in the **Support** folder).
- Known limitation: some instructions that theoretically accept **two labels** (e.g., `LD24 (.StringSource), .Title`) aren’t currently supported by the assembler. **Workaround:**  
  `LD ABC, .Title` then `LD24 (.StringSource), ABC`. Will be fixed in another release.
- Refactored hardcoded resolution in `Constants`.
- Refactored most sample programs to **return to the OS** when the user presses **Esc**.

---

## 0.2.5 — 25.05.2023

### Features

- Added **client/server protocols** to enable a separate application to act as a debugger.
- Created a working **debugger application** (“Continuum Tools”) to use those protocols (see manual).
- Set up a **debug trap** in the Machine class to enable step-by-step debugging (if enabled from the debugger).
- Removed remaining key bindings. Fullscreen and loading examples are now handled by the **operating system**.

### Misc

- Updated **README**, version history, manuals; added new manual, covers, icons.

---

## 0.1.5 — 21.03.2023

### Features

- Added `#RUN <address>` compiler directive to expose an address entry point for OS launch. Optional; if missing, the first `#ORG` is the start address.
- Added **Build** interrupt at `Machine/Build` (`INT 0x00, 0xC0`) to compile assembly source files directly from an assembly program.
- Added a **rudimentary OS** able to compile and execute `*.asm` files. Use `run <filepath\name>.asm` to run a program (see manual, “Operating system”).
- Improved `ListFilesInDirectory` and `ListDirectoriesInDirectory`: instead of crashing on bad paths, they now return **`0xFFFFFF`** (error). Since a directory is unlikely to hold 16 million files, this avoids extra registers/flags.

### Misc

- Updated `readme.txt`.
- Removed `Esc` to exit the emulator without warning (delegated to the OS). In the meantime, close via window controls in windowed mode.
- Removed function key quick-load (F1–F8); now handled by the OS and frees up keys.
- **Temporarily** assigned fullscreen from **F12** to **F1** (to be removed later).

### Documentation

- Updated user manual: **Keyboard**, **Video**, added **Compiler** and **Operating system** sections.
- Updated interrupt reference with the newly added interrupt.

### Samples

- Added an example app (with source) to **monitor keypresses**.

---

## 0.1.4 — 18.03.2023

### Features

- Replaced implementation of `INT 0x02`, function `0x00` (**Read Keyboard State**) with a built-from-scratch version.
- Introduced `INT 0x02`, function `0x10` (**Read keyboard state as code bytes**), which is more compact and practical in many situations.

### Documentation

- Updated the manual to explain keyboard state handling.
- Updated the interrupt reference.

### Internals

- Added more unit tests, including some that were previously missing.

---

## 0.0.0 — 26.12.2022

- The very first **stable build** of **Continuum 93**.

---

*Note: Instruction mnemonics, flags, addresses, register names, and interrupt identifiers are formatted as code for clarity. Minor spelling/grammar corrections were applied where it improved readability without changing meaning.*
