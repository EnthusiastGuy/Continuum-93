# Continuum 93 BASIC Language Specification

## Executive Summary

This document defines the Continuum 93 BASIC programming language, a ZX BASIC-inspired language adapted for the Continuum 93 machine architecture. The language uses labels instead of line numbers, supports the full range of machine capabilities including multi-layer video, advanced audio synthesis, and comprehensive mathematical operations.

## Table of Contents

1. [Language Overview](#language-overview)
2. [Program Structure](#program-structure)
3. [Data Types and Variables](#data-types-and-variables)
4. [Operators](#operators)
5. [Control Flow](#control-flow)
6. [Graphics Commands](#graphics-commands)
7. [Audio Commands](#audio-commands)
8. [Input/Output Commands](#inputoutput-commands)
9. [Mathematical Functions](#mathematical-functions)
10. [String Operations](#string-operations)
11. [Memory Operations](#memory-operations)
12. [System Commands](#system-commands)
13. [Compiler Implementation Notes](#compiler-implementation-notes)

---

## Language Overview

### Key Features

- **Label-based structure**: Uses labels (e.g., `Loop1:`) instead of line numbers
- **Multi-layer graphics**: Support for up to 8 video layers with transparency
- **Advanced audio**: BEEP and PLAY commands for sound synthesis
- **Comprehensive math**: Full set of mathematical and logical operations
- **24-bit addressing**: 16MB address space
- **Register-based**: Direct access to machine registers when needed

### Syntax Conventions

- **Case sensitivity**: Commands and keywords are case-insensitive
- **Line continuation**: Not supported (each statement on one line)
- **Comments**: `REM` or `'` for single-line comments
- **Labels**: Must end with `:` and appear at the start of a line
- **Whitespace**: Spaces and tabs are generally ignored except within string literals

---

## Program Structure

### Basic Program Format

```
Label1:
    REM This is a comment
    PRINT "Hello, World"
    GOTO Label1
```

### Labels

Labels are used for:
- Jump targets (GOTO, GOSUB)
- Loop markers
- Function entry points
- Data section markers

**Syntax**: `LabelName:`

**Rules**:
- Must start with a letter or underscore
- Can contain letters, digits, and underscores
- Must end with `:`
- Must appear at the start of a line
- Case-sensitive (Label1 and label1 are different)

**Examples**:
```
MainLoop:
    PRINT "Running"
    GOTO MainLoop

ProcessData:
    LET x = 10
    RETURN
```

### Comments

Two comment styles are supported:

1. **REM statement**: `REM This is a comment`
2. **Single quote**: `' This is also a comment`

Comments extend to the end of the line.

---

## Data Types and Variables

### Numeric Types

#### Integer
- **Range**: -2,147,483,648 to 2,147,483,647 (32-bit signed)
- **Storage**: 4 bytes, big-endian
- **Examples**: `10`, `-42`, `0xFF`, `0b1010`

#### Float
- **Format**: IEEE 754 single-precision (32-bit)
- **Range**: Approximately ±3.4 × 10³⁸
- **Precision**: 7 decimal digits
- **Examples**: `3.14`, `-0.5`, `1.5e10`

### Variable Names

**Rules**:
- Must start with a letter
- Can contain letters, digits, and underscores
- Maximum length: Implementation-defined (recommended 255 characters)
- Case-sensitive

**Examples**:
```
LET counter = 0
LET player_score = 100
LET xPos# = 50.5
```

### Variable Declaration

Variables are declared implicitly on first use. No explicit declaration required.

**Type Suffixes**:
- **Integer**: No suffix (default for whole numbers)
- **Float**: `#` suffix (e.g., `y#`, `position#`)
- **String**: `$` suffix (e.g., `name$`, `text$`)

**Examples**:
```
LET x = 10        ' Integer variable
LET y# = 3.14     ' Float variable (note the # suffix)
LET name$ = "Test" ' String variable
```

### Type Coercion and Cross-Type Operations

Cross-type operations are legal and follow these rules:

1. **Integer + Float → Float**: When an integer and float are combined, the result is a float.
2. **Float → Integer**: When a float is assigned to an integer variable, it is truncated (not rounded).
3. **Integer / Integer → Float**: Division always produces a float result (use `\` for integer division).

**Examples**:
```
LET A = 10        ' Integer
LET B# = 3.14     ' Float
LET C# = A + B#   ' Result: 13.14 (float)
LET D = A + B#    ' Result: 13 (integer, truncated)
LET E = A / 3     ' Result: 3.333... (float, even though A is integer)
LET F = A \ 3     ' Result: 3 (integer division)
```

### String Variables

String variables end with `$`:
- **Type**: Null-terminated UTF-8 strings
- **Maximum length**: Limited by available memory
- **Examples**: `name$`, `message$`, `buffer$`

```
LET text$ = "Hello"
LET greeting$ = "World"
```

### Arrays

Arrays are declared with `DIM`:

**Syntax**: `DIM arrayName(size)`

**Examples**:
```
DIM numbers(100)      ' Integer array, indices 0-100
DIM scores(10)        ' Integer array, indices 0-10
DIM names$(20)        ' String array, indices 0-20
DIM matrix(10, 10)     ' 2D array, 10x10
```

**Array Access**:
```
LET numbers(0) = 10
LET numbers(1) = 20
LET value = numbers(0)
```

**Multi-dimensional Arrays**:
```
DIM grid(10, 10)      ' 2D array
LET grid(5, 3) = 42
```

---

## Operators

### Arithmetic Operators

| Operator | Description | Example |
|----------|-------------|---------|
| `+` | Addition | `x + y` |
| `-` | Subtraction | `x - y` |
| `*` | Multiplication | `x * y` |
| `/` | Division (float) | `x / y` |
| `\` | Integer division | `x \ y` |
| `MOD` | Modulo | `x MOD y` |
| `^` | Exponentiation | `x ^ y` |

**Examples**:
```
LET result = 10 + 5        ' 15
LET result = 10 - 3        ' 7
LET result = 4 * 5         ' 20
LET result = 10 / 3         ' 3.333...
LET result = 10 \ 3         ' 3
LET result = 10 MOD 3       ' 1
LET result = 2 ^ 8          ' 256
```

### Comparison Operators

| Operator | Description | Example |
|----------|-------------|---------|
| `=` | Equal | `x = y` |
| `<>` | Not equal | `x <> y` |
| `<` | Less than | `x < y` |
| `>` | Greater than | `x > y` |
| `<=` | Less than or equal | `x <= y` |
| `>=` | Greater than or equal | `x >= y` |

**Examples**:
```
IF x = 10 THEN PRINT "Equal"
IF x <> 0 THEN PRINT "Not zero"
IF x < 100 THEN PRINT "Less than 100"
```

### Logical Operators

| Operator | Description | Example |
|----------|-------------|---------|
| `AND` | Logical AND | `x AND y` |
| `OR` | Logical OR | `x OR y` |
| `XOR` | Logical XOR | `x XOR y` |
| `NOT` | Logical NOT | `NOT x` |
| `IMPLY` | Logical implication | `x IMPLY y` |
| `NAND` | Logical NAND | `x NAND y` |
| `NOR` | Logical NOR | `x NOR y` |
| `XNOR` | Logical XNOR | `x XNOR y` |

**Note**: `IMPLY`, `NAND`, `NOR`, and `XNOR` are extensions not in standard ZX BASIC.

**Examples**:
```
IF x > 0 AND x < 100 THEN PRINT "In range"
IF x = 0 OR y = 0 THEN PRINT "Zero found"
IF NOT done THEN PRINT "Not done"
IF condition1 IMPLY condition2 THEN PRINT "Implication"
```

### Bitwise Operators

Bitwise operators operate on integer values:

| Operator | Description | Example |
|----------|-------------|---------|
| `AND` | Bitwise AND | `x AND y` |
| `OR` | Bitwise OR | `x OR y` |
| `XOR` | Bitwise XOR | `x XOR y` |
| `NOT` | Bitwise NOT | `NOT x` |
| `SHL` | Shift left | `x SHL n` |
| `SHR` | Shift right | `x SHR n` |
| `ROL` | Rotate left | `x ROL n` |
| `ROR` | Rotate right | `x ROR n` |

**Examples**:
```
LET result = 5 AND 3        ' 1
LET result = 5 OR 3         ' 7
LET result = 5 XOR 3         ' 6
LET result = NOT 0           ' -1 (all bits set)
LET result = 8 SHL 2        ' 32
LET result = 32 SHR 2        ' 8
```

### Operator Precedence

1. Parentheses `()`
2. Exponentiation `^`
3. Unary operators (`NOT`, `-`)
4. Multiplication, Division, Modulo (`*`, `/`, `\`, `MOD`)
5. Addition, Subtraction (`+`, `-`)
6. Bitwise shifts (`SHL`, `SHR`, `ROL`, `ROR`)
7. Comparison (`=`, `<>`, `<`, `>`, `<=`, `>=`)
8. Logical AND (`AND`)
9. Logical XOR (`XOR`)
10. Logical OR (`OR`)
11. Logical IMPLY (`IMPLY`)
12. Logical NAND (`NAND`)
13. Logical NOR (`NOR`)
14. Logical XNOR (`XNOR`)

---

## Control Flow

### Assignment

**Syntax**: `LET variable = expression`

The `LET` keyword is optional but recommended for clarity.

**Examples**:
```
LET x = 10
LET y = x + 5
LET name$ = "Hello"
x = 20        ' LET is optional
```

### Conditional Statements

#### IF-THEN

**Syntax**: `IF condition THEN statement`

**Examples**:
```
IF x > 10 THEN PRINT "Large"
IF x = 0 THEN LET y = 1
```

#### IF-THEN-ELSE

**Syntax**:
```
IF condition THEN
    statements
ELSE
    statements
END IF
```

**Examples**:
```
IF x > 10 THEN
    PRINT "Large"
ELSE
    PRINT "Small"
END IF
```

#### IF-THEN-ELSEIF

**Syntax**:
```
IF condition1 THEN
    statements
ELSEIF condition2 THEN
    statements
ELSE
    statements
END IF
```

**Examples**:
```
IF x > 100 THEN
    PRINT "Very large"
ELSEIF x > 10 THEN
    PRINT "Large"
ELSE
    PRINT "Small"
END IF
```

### Loops

#### FOR-NEXT Loop

**Syntax**:
```
FOR variable = start TO end [STEP step]
    statements
NEXT [variable]
```

**Examples**:
```
FOR i = 1 TO 10
    PRINT i
NEXT i

FOR i = 10 TO 1 STEP -1
    PRINT i
NEXT i

FOR x = 0 TO 100 STEP 5
    PRINT x
NEXT x
```

#### WHILE-WEND Loop

**Syntax**:
```
WHILE condition
    statements
WEND
```

**Examples**:
```
WHILE x < 100
    LET x = x + 1
    PRINT x
WEND
```

#### REPEAT-UNTIL Loop

**Syntax**:
```
REPEAT
    statements
UNTIL condition
```

**Examples**:
```
REPEAT
    LET x = x + 1
    PRINT x
UNTIL x >= 100
```

### Jump Statements

#### GOTO

**Syntax**: `GOTO label`

**Examples**:
```
MainLoop:
    PRINT "Running"
    GOTO MainLoop
```

#### GOSUB-RETURN

**Syntax**:
```
GOSUB label
...
label:
    statements
    RETURN
```

**Examples**:
```
GOSUB ProcessData
END

ProcessData:
    PRINT "Processing"
    RETURN
```

**Nested Subroutines**: Subroutines can call other subroutines. Each `RETURN` returns to the most recent `GOSUB`.

### Loop Control

#### EXIT FOR

Exits a FOR-NEXT loop early.

**Examples**:
```
FOR i = 1 TO 100
    IF i = 50 THEN EXIT FOR
    PRINT i
NEXT i
```

#### EXIT WHILE

Exits a WHILE-WEND loop early.

**Examples**:
```
WHILE x < 100
    IF x = 50 THEN EXIT WHILE
    LET x = x + 1
WEND
```

#### CONTINUE FOR

Skips to the next iteration of a FOR-NEXT loop.

**Examples**:
```
FOR i = 1 TO 10
    IF i MOD 2 = 0 THEN CONTINUE FOR
    PRINT i
NEXT i
```

#### CONTINUE WHILE

Skips to the next iteration of a WHILE-WEND loop.

**Examples**:
```
WHILE x < 100
    LET x = x + 1
    IF x MOD 2 = 0 THEN CONTINUE WHILE
    PRINT x
WEND
```

---

## Graphics Commands

### Screen Management

#### CLS

**Syntax**: `CLS [n]`

Clears the screen. If `n` is specified (0-7), clears video page `n`. If omitted, clears the current active page.

**Examples**:
```
CLS              ' Clear current page
CLS 0            ' Clear page 0
CLS 3            ' Clear page 3
```

#### SCREEN

**Syntax**: `SCREEN n`

Switches the active video page to page `n` (0-7). All subsequent graphics operations target this page until changed.

**Examples**:
```
SCREEN 0        ' Switch to page 0
SCREEN 1        ' Switch to page 1
```

#### VIDEO

**Syntax**: `VIDEO n`

Sets the number of video layers (pages) to `n` (1-8). This reconfigures the video system and recalculates VRAM layout.

**Examples**:
```
VIDEO 1         ' Single layer
VIDEO 4         ' Four layers
VIDEO 8         ' Maximum layers
```

### Drawing Primitives

#### PLOT

**Syntax**: `PLOT x, y [, color]`

Plots a pixel at coordinates (x, y). Color is optional (defaults to current color).

**Examples**:
```
PLOT 100, 50
PLOT 200, 100, 15
```

#### LINE

**Syntax**: `LINE x1, y1, x2, y2 [, color]`

Draws a line from (x1, y1) to (x2, y2). Color is optional.

**Examples**:
```
LINE 0, 0, 479, 269
LINE 10, 10, 100, 100, 200
```

#### RECTANGLE

**Syntax**: `RECTANGLE x, y, width, height [, color]`

Draws a rectangle outline at position (x, y) with specified width and height.

**Examples**:
```
RECTANGLE 10, 10, 100, 50
RECTANGLE 50, 50, 200, 100, 15
```

#### RECTANGLE FILLED

**Syntax**: `RECTANGLE FILLED x, y, width, height [, color]`

Draws a filled rectangle.

**Examples**:
```
RECTANGLE FILLED 10, 10, 100, 50
RECTANGLE FILLED 50, 50, 200, 100, 15
```

#### CIRCLE

**Syntax**: `CIRCLE x, y, radius [, color]`

Draws a circle outline centered at (x, y) with specified radius.

**Examples**:
```
CIRCLE 240, 135, 50
CIRCLE 100, 100, 25, 200
```

#### CIRCLE FILLED

**Syntax**: `CIRCLE FILLED x, y, radius [, color]`

Draws a filled circle.

**Examples**:
```
CIRCLE FILLED 240, 135, 50
CIRCLE FILLED 100, 100, 25, 200
```

#### ELLIPSE

**Syntax**: `ELLIPSE x, y, radiusX, radiusY [, color]`

Draws an ellipse outline centered at (x, y).

**Examples**:
```
ELLIPSE 240, 135, 100, 50
ELLIPSE 100, 100, 50, 25, 200
```

#### ELLIPSE FILLED

**Syntax**: `ELLIPSE FILLED x, y, radiusX, radiusY [, color]`

Draws a filled ellipse.

**Examples**:
```
ELLIPSE FILLED 240, 135, 100, 50
ELLIPSE FILLED 100, 100, 50, 25, 200
```

### Color Management

#### INK

**Syntax**: `INK color`

Sets the current drawing color (0-255). Color 0 is transparent.

**Examples**:
```
INK 15          ' White
INK 200         ' Color index 200
```

#### PAPER

**Syntax**: `PAPER color`

Sets the background/paper color. Note: In multi-layer system, this affects fill operations.

**Examples**:
```
PAPER 0         ' Black/transparent
PAPER 1         ' Background color
```

### Text Output

#### PRINT

**Syntax**: `PRINT [expression1] [;] [,] [expression2] ...`

Prints text or values to the screen. Multiple items can be printed on the same line using `;` or `,`.

**Examples**:
```
PRINT "Hello, World"
PRINT "Value: "; x
PRINT "X="; x; ", Y="; y
PRINT x, y, z
```

**Print at Pixel Coordinates**:
- `PRINT AT x, y; expression` - Print at specific pixel coordinates (x, y)
- `PRINT AT x, y; expression; USING fontAddr` - Print using specific font
- `PRINT AT x, y; expression; USING fontAddr, color, flags, maxWidth, outlineColor, outlinePattern` - Print with all options

**Parameters for PRINT AT**:
- `x, y`: Pixel coordinates (16-bit signed integers)
- `fontAddr`: Font address in memory (24-bit address, optional - uses current font if omitted)
- `color`: Text color (8-bit, 0-255, optional - uses current font color if omitted)
- `flags`: Font flags (8-bit, optional - uses current font flags if omitted)
  - Bit 0: Monospace
  - Bit 1: Monospace centering
  - Bit 2: Disable kerning
  - Bit 3: Center
  - Bit 4: Wrap
  - Bit 5: Draw outline
- `maxWidth`: Maximum width in pixels (16-bit, optional - 0 = no limit)
- `outlineColor`: Outline color (8-bit, optional - only used if outline flag is set)
- `outlinePattern`: Outline pattern (8-bit, optional - only used if outline flag is set)

**Examples**:
```
PRINT AT 100, 50; "Hello"                    ' Simple print at pixel coordinates
PRINT AT 100, 50; "Hello"; USING myFont      ' Using specific font
PRINT AT 100, 50; "Hello"; USING myFont, 15 ' With color
PRINT AT 100, 50; "Text"; USING myFont, 15, &B00111000, 320, 255, &B00100100
```

**Print with Tab Spacing**:
- `PRINT TAB(n); "text"` - Print with tab spacing

**Examples**:
```
PRINT TAB(10); "Indented"
```

#### Font Management

Fonts are loaded from PNG files and stored in memory. Multiple fonts can be loaded and managed.

##### LOAD FONT

**Syntax**: `LOAD FONT filename$, address`

Loads a PNG font file into memory at the specified address. Returns error code (0 = success).

**Examples**:
```
LOAD FONT "fonts/default.png", fontAddr
LOAD FONT "fonts/large.png", &H20000
```

**Note**: The font file must be a PNG image with dimensions divisible by a 16×6 grid. If a corresponding `.txt` file exists (e.g., `fonts/default.txt`), kerning data will be loaded automatically.

##### FONT

**Syntax**: `FONT address`

Sets the current active font to the font at the specified memory address. All subsequent PRINT statements will use this font until changed.

**Examples**:
```
FONT fontAddr
FONT &H20000
FONT myFontAddress
```

##### FONT COLOR

**Syntax**: `FONT COLOR color`

Sets the current font color for subsequent PRINT statements.

**Examples**:
```
FONT COLOR 15      ' White
FONT COLOR 200    ' Color index 200
```

##### FONT FLAGS

**Syntax**: `FONT FLAGS flags`

Sets the current font flags for subsequent PRINT statements. Flags are specified as a bitmask.

**Flag Bits**:
- Bit 0: Monospace
- Bit 1: Monospace centering
- Bit 2: Disable kerning
- Bit 3: Center
- Bit 4: Wrap
- Bit 5: Draw outline

**Examples**:
```
FONT FLAGS &B00111000  ' Center, wrap, outline enabled
FONT FLAGS 0           ' All flags disabled
```

##### FONT MAXWIDTH

**Syntax**: `FONT MAXWIDTH width`

Sets the maximum width in pixels for text wrapping. Use 0 for no limit.

**Examples**:
```
FONT MAXWIDTH 320     ' Wrap at 320 pixels
FONT MAXWIDTH 0       ' No wrapping
```

##### FONT OUTLINE

**Syntax**: `FONT OUTLINE color, pattern`

Sets the outline color and pattern for text. Only used when the outline flag (bit 5) is set.

**Examples**:
```
FONT OUTLINE 255, &B00100100  ' White outline with pattern
FONT OUTLINE 0, 0             ' Disable outline
```

**Note**: The outline pattern is an 8-bit value where each bit controls which directions around a pixel should have outline. See the DrawText interrupt documentation for pattern details.

#### LOCATE

**Syntax**: `LOCATE x, y`

Sets the text cursor position for subsequent PRINT statements (pixel coordinates).

**Examples**:
```
LOCATE 100, 50
PRINT "At pixel position 100,50"
```

### Layer Control

#### LAYER SHOW

**Syntax**: `LAYER SHOW n`

Makes layer `n` (0-7) visible.

**Examples**:
```
LAYER SHOW 0
LAYER SHOW 1
```

#### LAYER HIDE

**Syntax**: `LAYER HIDE n`

Hides layer `n` (0-7).

**Examples**:
```
LAYER HIDE 0
LAYER HIDE 3
```

#### LAYER VISIBILITY

**Syntax**: `LAYER VISIBILITY mask`

Sets layer visibility using a bitmask. Each bit (0-7) corresponds to a layer.

**Examples**:
```
LAYER VISIBILITY &B11111111  ' All layers visible
LAYER VISIBILITY &B00001111  ' Only layers 0-3 visible
```

### Sprite Operations

#### SPRITE

**Syntax**: `SPRITE x, y, address, width, height [, page]`

Draws a sprite from memory at the specified position.

**Examples**:
```
SPRITE 100, 50, spriteData, 32, 32
SPRITE 200, 100, spriteData, 64, 64, 0
```

---

## Audio Commands

### BEEP

**Syntax**: `BEEP duration, pitch [, volume]`

Plays a simple beep sound.

**Parameters**:
- `duration`: Duration in seconds (float)
- `pitch`: Frequency in Hz (float)
- `volume`: Volume 0.0-1.0 (float, optional, default 0.5)

**Examples**:
```
BEEP 0.5, 440        ' A4 note for 0.5 seconds
BEEP 0.1, 880, 0.8   ' Higher pitch, louder
```

**Implementation Note**: BEEP should generate a simple sine wave sound using the PLAY instruction with minimal parameters.

### PLAY

**Syntax**: `PLAY address`

Plays a sound using parameters stored in memory at the specified address. The address can be a numeric expression or a label pointing to sound parameter data.

**Examples**:
```
PLAY soundParams     ' Play sound at label address
PLAY &H10000          ' Play sound at hex address
PLAY soundData + offset
```

**Sound Parameter Structure**:

The sound parameters are stored in memory as a structured format (see APU_ARCHITECTURE.md for details):

1. **Flags** (2 bytes): Bit flags indicating which optional parameters are present
2. **Frequency** (4 bytes, float): Base frequency in Hz
3. **EnvelopeSustain** (4 bytes, float): Sustain time in seconds
4. **SoundVolume** (4 bytes, float): Master volume 0.0-1.0
5. **Optional parameters** (based on flags): WaveType, Envelope, Vibrato, etc.

**Example Sound Data**:
```
soundParams:
    DATA 0, 0                    ' Flags: no optional parameters
    DATA 440.0                   ' Frequency: 440 Hz (A4)
    DATA 0.5                     ' Sustain: 0.5 seconds
    DATA 0.5                     ' Volume: 50%

Main:
    PLAY soundParams
```

**Advanced Sound Example**:
```
complexSound:
    DATA &B11110000, 0           ' Flags: WaveType, Envelope, Vibrato enabled
    DATA 220.0                    ' Frequency: 220 Hz
    DATA 0.2                      ' Sustain: 0.2 seconds
    DATA 0.6                      ' Volume: 60%
    DATA 3                        ' WaveType: SQUARE
    DATA 0.1                      ' Attack: 0.1 seconds
    DATA 0.4                      ' Decay: 0.4 seconds
    DATA 0.2                      ' VibratoDepth: 0.2
    DATA 5.0                      ' VibratoSpeed: 5.0 Hz

Main:
    PLAY complexSound
```

---

## Input/Output Commands

### Keyboard Input

#### INPUT

**Syntax**: `INPUT [prompt$;] variable`

Reads input from the keyboard and stores it in a variable.

**Examples**:
```
INPUT "Enter name: "; name$
INPUT "Enter number: "; x
INPUT x                          ' No prompt
```

#### INKEY$

**Syntax**: `INKEY$`

Returns a string containing the currently pressed key, or empty string if no key is pressed. Non-blocking.

**Examples**:
```
LET key$ = INKEY$
IF key$ <> "" THEN PRINT "Key pressed: "; key$
```

#### INKEY

**Syntax**: `INKEY`

Returns the ASCII code of the currently pressed key, or 0 if no key is pressed. Non-blocking.

**Examples**:
```
LET key = INKEY
IF key <> 0 THEN PRINT "Key code: "; key
```

### Mouse Input

#### MOUSE X

**Syntax**: `MOUSE X`

Returns the current X coordinate of the mouse cursor.

**Examples**:
```
LET mx = MOUSE X
PRINT "Mouse X: "; mx
```

#### MOUSE Y

**Syntax**: `MOUSE Y`

Returns the current Y coordinate of the mouse cursor.

**Examples**:
```
LET my = MOUSE Y
PRINT "Mouse Y: "; my
```

#### MOUSE BUTTON

**Syntax**: `MOUSE BUTTON n`

Returns 1 if mouse button `n` is pressed, 0 otherwise. Button 0 = left, 1 = right, 2 = middle.

**Examples**:
```
IF MOUSE BUTTON 0 THEN PRINT "Left button pressed"
IF MOUSE BUTTON 1 THEN PRINT "Right button pressed"
```

### File Operations

#### LOAD

**Syntax**: `LOAD filename$ [, address]`

Loads a file from disk into memory. If address is specified, loads at that address; otherwise uses a default location.

**Examples**:
```
LOAD "data.bin"
LOAD "sprite.dat", &H20000
```

#### SAVE

**Syntax**: `SAVE filename$, address, length`

Saves a block of memory to a file.

**Examples**:
```
SAVE "data.bin", &H10000, 1024
SAVE "output.dat", dataStart, dataLength
```

#### OPEN

**Syntax**: `OPEN filename$ FOR mode AS #n`

Opens a file for reading or writing. (Implementation may vary based on file system capabilities)

**Examples**:
```
OPEN "data.txt" FOR INPUT AS #1
OPEN "output.txt" FOR OUTPUT AS #2
```

#### CLOSE

**Syntax**: `CLOSE #n`

Closes a file.

**Examples**:
```
CLOSE #1
```

#### READ

**Syntax**: `READ #n, variable`

Reads data from an open file.

**Examples**:
```
READ #1, x
READ #1, name$
```

#### WRITE

**Syntax**: `WRITE #n, expression`

Writes data to an open file.

**Examples**:
```
WRITE #1, x
WRITE #1, "Hello"
```

---

## Mathematical Functions

### Basic Math Functions

#### ABS

**Syntax**: `ABS(x)`

Returns the absolute value of `x`.

**Examples**:
```
LET result = ABS(-10)        ' 10
LET result = ABS(10)         ' 10
```

#### SGN

**Syntax**: `SGN(x)`

Returns the sign of `x`: -1 if negative, 0 if zero, 1 if positive.

**Examples**:
```
LET result = SGN(-5)         ' -1
LET result = SGN(0)           ' 0
LET result = SGN(5)           ' 1
```

#### INT

**Syntax**: `INT(x)`

Returns the integer part of `x` (truncates toward zero).

**Examples**:
```
LET result = INT(3.7)         ' 3
LET result = INT(-3.7)        ' -3
```

#### FIX

**Syntax**: `FIX(x)`

Same as `INT(x)` - truncates toward zero.

#### FLOOR

**Syntax**: `FLOOR(x)`

Returns the largest integer less than or equal to `x`.

**Examples**:
```
LET result = FLOOR(3.7)       ' 3
LET result = FLOOR(-3.7)      ' -4
```

#### CEIL

**Syntax**: `CEIL(x)`

Returns the smallest integer greater than or equal to `x`.

**Examples**:
```
LET result = CEIL(3.2)        ' 4
LET result = CEIL(-3.2)       ' -3
```

#### ROUND

**Syntax**: `ROUND(x [, decimals])`

Rounds `x` to the nearest integer, or to the specified number of decimal places.

**Examples**:
```
LET result = ROUND(3.5)       ' 4
LET result = ROUND(3.4)       ' 3
LET result = ROUND(3.14159, 2) ' 3.14
```

### Trigonometric Functions

#### SIN

**Syntax**: `SIN(x)`

Returns the sine of `x` (x in radians).

**Examples**:
```
LET result = SIN(0)           ' 0
LET result = SIN(PI / 2)      ' 1
```

#### COS

**Syntax**: `COS(x)`

Returns the cosine of `x` (x in radians).

**Examples**:
```
LET result = COS(0)            ' 1
LET result = COS(PI / 2)      ' 0
```

#### TAN

**Syntax**: `TAN(x)`

Returns the tangent of `x` (x in radians).

**Examples**:
```
LET result = TAN(0)            ' 0
LET result = TAN(PI / 4)      ' 1
```

#### ASIN

**Syntax**: `ASIN(x)`

Returns the arcsine of `x` (result in radians, range -π/2 to π/2).

**Examples**:
```
LET result = ASIN(1)           ' π/2
```

#### ACOS

**Syntax**: `ACOS(x)`

Returns the arccosine of `x` (result in radians, range 0 to π).

**Examples**:
```
LET result = ACOS(1)           ' 0
```

#### ATAN

**Syntax**: `ATAN(x)`

Returns the arctangent of `x` (result in radians, range -π/2 to π/2).

**Examples**:
```
LET result = ATAN(1)           ' π/4
```

#### ATAN2

**Syntax**: `ATAN2(y, x)`

Returns the arctangent of y/x (result in radians, range -π to π).

**Examples**:
```
LET result = ATAN2(1, 1)       ' π/4
LET result = ATAN2(1, -1)      ' 3π/4
```

### Exponential and Logarithmic Functions

#### EXP

**Syntax**: `EXP(x)`

Returns e raised to the power of `x`.

**Examples**:
```
LET result = EXP(1)            ' e (≈2.718)
LET result = EXP(0)             ' 1
```

#### LOG

**Syntax**: `LOG(x)`

Returns the natural logarithm (base e) of `x`.

**Examples**:
```
LET result = LOG(EXP(1))       ' 1
LET result = LOG(1)             ' 0
```

#### LOG10

**Syntax**: `LOG10(x)`

Returns the base-10 logarithm of `x`.

**Examples**:
```
LET result = LOG10(100)         ' 2
LET result = LOG10(10)          ' 1
```

### Power and Root Functions

#### SQR

**Syntax**: `SQR(x)`

Returns the square root of `x`.

**Examples**:
```
LET result = SQR(16)            ' 4
LET result = SQR(2)             ' ≈1.414
```

#### POW

**Syntax**: `POW(x, y)`

Returns `x` raised to the power of `y`.

**Examples**:
```
LET result = POW(2, 8)         ' 256
LET result = POW(10, 2)        ' 100
```

#### ISQR

**Syntax**: `ISQR(x)`

Returns the integer square root of `x` (largest integer whose square is ≤ x).

**Examples**:
```
LET result = ISQR(16)           ' 4
LET result = ISQR(15)           ' 3
```

### Min/Max Functions

#### MIN

**Syntax**: `MIN(x, y)`

Returns the minimum of `x` and `y`.

**Examples**:
```
LET result = MIN(10, 20)        ' 10
LET result = MIN(-5, 5)         ' -5
```

#### MAX

**Syntax**: `MAX(x, y)`

Returns the maximum of `x` and `y`.

**Examples**:
```
LET result = MAX(10, 20)        ' 20
LET result = MAX(-5, 5)         ' 5
```

### Random Number Generation

#### RND

**Syntax**: `RND([x])`

Returns a random number. If `x` is omitted or 0, returns a random float in range [0, 1). If `x` > 0, returns a random integer in range [1, x]. If `x` < 0, seeds the random number generator.

**Examples**:
```
LET r = RND                    ' Random float [0, 1)
LET r = RND(100)               ' Random integer [1, 100]
LET r = RND(0)                  ' Random float [0, 1)
RND(-1)                        ' Seed random number generator
```

#### RANDOMIZE

**Syntax**: `RANDOMIZE [seed]`

Seeds the random number generator. If seed is omitted, uses system time.

**Examples**:
```
RANDOMIZE                      ' Seed with system time
RANDOMIZE 12345                ' Seed with specific value
```

### Mathematical Constants

#### PI

**Syntax**: `PI`

Returns the mathematical constant π (approximately 3.141592653589793).

**Examples**:
```
LET circumference = 2 * PI * radius
LET area = PI * radius ^ 2
```

#### E

**Syntax**: `E`

Returns the mathematical constant e (approximately 2.718281828459045).

**Examples**:
```
LET result = E ^ 2
```

---

## String Operations

### String Functions

#### LEN

**Syntax**: `LEN(string$)`

Returns the length of a string.

**Examples**:
```
LET length = LEN("Hello")      ' 5
LET length = LEN(name$)
```

#### LEFT$

**Syntax**: `LEFT$(string$, n)`

Returns the leftmost `n` characters of `string$`.

**Examples**:
```
LET result$ = LEFT$("Hello", 2) ' "He"
LET result$ = LEFT$(text$, 10)
```

#### RIGHT$

**Syntax**: `RIGHT$(string$, n)`

Returns the rightmost `n` characters of `string$`.

**Examples**:
```
LET result$ = RIGHT$("Hello", 2) ' "lo"
LET result$ = RIGHT$(text$, 5)
```

#### MID$

**Syntax**: `MID$(string$, start [, length])`

Returns a substring of `string$` starting at position `start` (1-based). If `length` is omitted, returns from `start` to the end.

**Examples**:
```
LET result$ = MID$("Hello", 2, 2)    ' "el"
LET result$ = MID$("Hello", 3)       ' "llo"
LET result$ = MID$(text$, 5, 10)
```

#### CHR$

**Syntax**: `CHR$(code)`

Returns a string containing the character with the specified ASCII code.

**Examples**:
```
LET char$ = CHR$(65)           ' "A"
LET char$ = CHR$(10)           ' Newline
```

#### ASC

**Syntax**: `ASC(string$)`

Returns the ASCII code of the first character in `string$`.

**Examples**:
```
LET code = ASC("A")            ' 65
LET code = ASC("Hello")        ' 72 (H)
```

#### VAL

**Syntax**: `VAL(string$)`

Converts a string to a numeric value.

**Examples**:
```
LET num = VAL("123")           ' 123
LET num = VAL("3.14")          ' 3.14
```

#### STR$

**Syntax**: `STR$(number)`

Converts a number to a string.

**Examples**:
```
LET text$ = STR$(123)          ' "123"
LET text$ = STR$(3.14)         ' "3.14"
```

#### STRING$

**Syntax**: `STRING$(n, char$)`

Returns a string containing `n` copies of `char$` (or the first character of `char$`).

**Examples**:
```
LET result$ = STRING$(5, "*")  ' "*****"
LET result$ = STRING$(10, "-")
```

#### INSTR

**Syntax**: `INSTR([start, ] string1$, string2$)`

Returns the position of the first occurrence of `string2$` in `string1$`, starting from position `start` (1-based). Returns 0 if not found.

**Examples**:
```
LET pos = INSTR("Hello", "el") ' 2
LET pos = INSTR(3, "Hello", "l") ' 3
LET pos = INSTR(text$, search$)
```

#### UCASE$

**Syntax**: `UCASE$(string$)`

Returns `string$` converted to uppercase.

**Examples**:
```
LET result$ = UCASE$("Hello")  ' "HELLO"
LET result$ = UCASE$(text$)
```

#### LCASE$

**Syntax**: `LCASE$(string$)`

Returns `string$` converted to lowercase.

**Examples**:
```
LET result$ = LCASE$("Hello")  ' "hello"
LET result$ = LCASE$(text$)
```

#### TRIM$

**Syntax**: `TRIM$(string$)`

Returns `string$` with leading and trailing whitespace removed.

**Examples**:
```
LET result$ = TRIM$("  Hello  ") ' "Hello"
LET result$ = TRIM$(text$)
```

#### LTRIM$

**Syntax**: `LTRIM$(string$)`

Returns `string$` with leading whitespace removed.

**Examples**:
```
LET result$ = LTRIM$("  Hello") ' "Hello"
```

#### RTRIM$

**Syntax**: `RTRIM$(string$)`

Returns `string$` with trailing whitespace removed.

**Examples**:
```
LET result$ = RTRIM$("Hello  ") ' "Hello"
```

### String Concatenation

Strings are concatenated using the `+` operator:

**Examples**:
```
LET result$ = "Hello" + " " + "World"  ' "Hello World"
LET result$ = name$ + " says hello"
```

---

## Memory Operations

### PEEK and POKE

#### PEEK

**Syntax**: `PEEK(address)`

Returns the byte value at memory address `address`.

**Examples**:
```
LET value = PEEK(&H10000)
LET value = PEEK(dataAddress)
```

#### POKE

**Syntax**: `POKE address, value`

Writes a byte value to memory address `address`.

**Examples**:
```
POKE &H10000, 255
POKE dataAddress, value
```

#### PEEK16

**Syntax**: `PEEK16(address)`

Returns the 16-bit value at memory address `address` (big-endian).

**Examples**:
```
LET value = PEEK16(&H10000)
```

#### POKE16

**Syntax**: `POKE16 address, value`

Writes a 16-bit value to memory address `address` (big-endian).

**Examples**:
```
POKE16 &H10000, 0x1234
```

#### PEEK24

**Syntax**: `PEEK24(address)`

Returns the 24-bit value at memory address `address` (big-endian).

**Examples**:
```
LET value = PEEK24(&H10000)
```

#### POKE24

**Syntax**: `POKE24 address, value`

Writes a 24-bit value to memory address `address` (big-endian).

**Examples**:
```
POKE24 &H10000, 0x123456
```

#### PEEK32

**Syntax**: `PEEK32(address)`

Returns the 32-bit value at memory address `address` (big-endian).

**Examples**:
```
LET value = PEEK32(&H10000)
```

#### POKE32

**Syntax**: `POKE32 address, value`

Writes a 32-bit value to memory address `address` (big-endian).

**Examples**:
```
POKE32 &H10000, 0x12345678
```

### Memory Copy and Fill

#### MEMCOPY

**Syntax**: `MEMCOPY source, dest, length`

Copies `length` bytes from `source` address to `dest` address.

**Examples**:
```
MEMCOPY &H10000, &H20000, 1024
MEMCOPY sourceAddr, destAddr, size
```

#### MEMFILL

**Syntax**: `MEMFILL address, length, value`

Fills `length` bytes starting at `address` with `value`.

**Examples**:
```
MEMFILL &H10000, 1024, 0
MEMFILL bufferAddr, size, 255
```

### Variable Address

#### VARPTR

**Syntax**: `VARPTR(variable)`

Returns the memory address of a variable.

**Examples**:
```
LET addr = VARPTR(x)
LET addr = VARPTR(name$)
```

---

## System Commands

### Program Control

#### END

**Syntax**: `END`

Terminates program execution.

**Examples**:
```
IF x > 100 THEN END
END
```

#### STOP

**Syntax**: `STOP`

Stops program execution (similar to END, but may allow debugging).

**Examples**:
```
STOP
```

#### WAIT

**Syntax**: `WAIT frames`

Waits for the specified number of frames.

**Examples**:
```
WAIT 60          ' Wait 60 frames (1 second at 60 FPS)
WAIT frames
```

#### SLEEP

**Syntax**: `SLEEP milliseconds`

Waits for the specified number of milliseconds.

**Examples**:
```
SLEEP 1000       ' Wait 1 second
SLEEP 500        ' Wait 0.5 seconds
```

### System Information

#### TIME

**Syntax**: `TIME`

Returns the number of milliseconds since program start.

**Examples**:
```
LET elapsed = TIME
PRINT "Elapsed: "; elapsed; " ms"
```

#### TICKS

**Syntax**: `TICKS`

Returns the number of ticks since program start (higher precision than TIME).

**Examples**:
```
LET ticks = TICKS
```

### Error Handling

#### ON ERROR

**Syntax**: `ON ERROR GOTO label`

Sets an error handler. When an error occurs, execution jumps to the specified label.

**Examples**:
```
ON ERROR GOTO ErrorHandler
...
ErrorHandler:
    PRINT "Error occurred"
    RESUME NEXT
```

#### RESUME

**Syntax**: `RESUME [NEXT]`

Resumes execution after an error. `RESUME NEXT` continues with the next statement.

**Examples**:
```
ErrorHandler:
    PRINT "Error"
    RESUME NEXT
```

#### ERR

**Syntax**: `ERR`

Returns the error code of the last error.

**Examples**:
```
IF ERR <> 0 THEN PRINT "Error code: "; ERR
```

#### ERL

**Syntax**: `ERL`

Returns the label/address where the error occurred. (Implementation may vary)

**Examples**:
```
PRINT "Error at: "; ERL
```

---

## Compiler Implementation Notes

### Label Handling

1. **Label Resolution**: The compiler must perform two passes:
   - First pass: Collect all labels and assign addresses
   - Second pass: Resolve label references in GOTO, GOSUB, etc.

2. **Label Storage**: Labels should be stored in a symbol table mapping label names to memory addresses.

3. **Forward References**: Labels can be referenced before they are defined (forward references are supported).

### Variable Storage

1. **Variable Allocation**: Variables should be allocated in memory. The compiler should maintain a variable table mapping variable names to memory addresses.

2. **Type Inference**: Variable types are determined by suffix:
   - Variables ending with `$` are strings
   - Variables ending with `#` are floats
   - Variables without suffix are integers
   - Type is determined at declaration/first use and cannot change

3. **Array Storage**: Arrays should be stored as contiguous memory blocks. Multi-dimensional arrays should use row-major order. Array types follow the same suffix rules (e.g., `numbers#(10)` is a float array).

### Expression Compilation

1. **Operator Precedence**: The compiler must respect operator precedence as defined in this specification.

2. **Type Coercion**: 
   - Integer + Float → Float (result is float)
   - Integer / Integer → Float (unless using `\` for integer division)
   - Float → Integer: Truncation (not rounding) when assigning float to integer variable
   - String + Number → String (number converted to string)
   - String + String → String (concatenation)
   
   **Examples**:
   - `LET A = 10` (integer) + `LET B# = 3.14` (float) → `LET C# = A + B#` yields 13.14 (float)
   - `LET D = A + B#` yields 13 (integer, truncated from 13.14)

3. **Register Allocation**: The compiler should use machine registers (A-Z, F0-F15) efficiently for intermediate calculations.

### Graphics Command Mapping

Graphics commands should map to video interrupts (INT 0x01):

- `CLS n` → INT 0x01, function 0x05 (ClearVideoPage)
- `SCREEN n` → Sets active page (may require interrupt or direct memory access)
- `VIDEO n` → INT 0x01, function 0x02 (SetVideoPagesCount)
- `PLOT x, y, color` → INT 0x01, function 0x20 (Plot)
- `LINE x1, y1, x2, y2, color` → INT 0x01, function 0x21 (Line)
- `RECTANGLE` → INT 0x01, function 0x07 (DrawRectangle)
- `RECTANGLE FILLED` → INT 0x01, function 0x06 (DrawFilledRectangle)
- `CIRCLE` → INT 0x01, function 0x22 (Ellipse with equal radii)
- `ELLIPSE` → INT 0x01, function 0x22 (Ellipse)

### Text and Font Command Mapping

- `PRINT AT x, y; text` → INT 0x01, function 0x14 (DrawText)
  - Uses pixel coordinates (x, y) - not character coordinates
  - Parameters: font address, string address, x, y, color, video page, max width, flags, outline color, outline pattern
  - If font/color/flags not specified in PRINT AT, use current font settings (set via FONT commands)
  
- `LOAD FONT filename$, address` → INT 0x04, function 0x40 (LoadPNGFont)
  - Loads PNG font file into memory
  - Returns error code (0 = success) and font data size
  
- `FONT address` → Sets current font (compiler maintains font state)
- `FONT COLOR color` → Sets current font color (compiler maintains state)
- `FONT FLAGS flags` → Sets current font flags (compiler maintains state)
- `FONT MAXWIDTH width` → Sets current max width (compiler maintains state)
- `FONT OUTLINE color, pattern` → Sets current outline settings (compiler maintains state)

**Font State Management**: The compiler should maintain a "current font" state structure containing:
- Font address (24-bit)
- Color (8-bit)
- Flags (8-bit)
- Max width (16-bit)
- Outline color (8-bit)
- Outline pattern (8-bit)

When `PRINT AT` is called without explicit font parameters, use the current font state. When parameters are provided in `PRINT AT`, they override the current state for that print operation only.

### Audio Command Mapping

- `BEEP duration, pitch, volume` → Generate simple sound parameters and call PLAY instruction
- `PLAY address` → PLAY instruction (direct mapping)

### Input/Output Mapping

- `INPUT` → INT 0x02, function 0x01 (ReadKeyboardBuffer)
- `INKEY$` → INT 0x02, function 0x01 (ReadKeyboardBuffer)
- `MOUSE X/Y` → INT 0x02, function 0x03 (ReadMouseState)
- `LOAD` → INT 0x04, function 0x07 (LoadFile)
- `SAVE` → INT 0x04, function 0x06 (SaveFile)

### Mathematical Function Mapping

Mathematical functions map to assembly instructions:

- `SIN`, `COS`, `TAN` → SIN, COS, TAN instructions (float registers)
- `SQR` → SQR instruction (float registers)
- `POW` → POW instruction (float registers)
- `ABS` → ABS instruction
- `FLOOR`, `CEIL`, `ROUND`, `INT` → FLOOR, CEIL, ROUND, INT instructions
- `MIN`, `MAX` → MIN, MAX instructions
- `RND` → RAND instruction

### String Operation Implementation

String operations should use string interrupts (INT 0x05) where available, or implement using memory operations:

- String concatenation: Allocate new memory, copy both strings
- String functions: Use memory operations and string interrupts

### Memory Layout

The compiler should organize memory as follows:

1. **Code Section**: Program code starting at a base address (e.g., 0x080000)
2. **Data Section**: Variables and arrays
3. **String Pool**: String literals
4. **Stack**: For subroutine calls (uses machine stack)
5. **Video RAM**: At high addresses (managed by system)

### Code Generation Strategy

1. **Register Usage**: 
   - Use integer registers (A-Z) for integer operations
   - Use float registers (F0-F15) for float operations
   - Spill to memory when registers are exhausted

2. **Function Calls**: 
   - Use CALL instruction for GOSUB
   - Use RET instruction for RETURN
   - Pass parameters via registers or memory

3. **Loops**: 
   - FOR loops: Use DJNZ or conditional jumps
   - WHILE loops: Use conditional jumps (JR, JP)
   - REPEAT loops: Use conditional jumps

4. **Conditional Statements**: 
   - Use CP/SCP for comparisons
   - Use JR/JP with condition codes (Z, NZ, C, NC, etc.)

### Error Handling

The compiler should:

1. Report syntax errors with line numbers/labels
2. Report undefined labels
3. Report type mismatches
4. Report array bounds errors (if detectable at compile time)
5. Generate code that checks for runtime errors (division by zero, array bounds, etc.)

### Optimization Opportunities

1. **Constant Folding**: Evaluate constant expressions at compile time
2. **Dead Code Elimination**: Remove unreachable code
3. **Register Allocation**: Efficiently allocate registers to minimize memory access
4. **Loop Optimization**: Optimize loop structures
5. **Function Inlining**: Inline small functions/subroutines

---

## Appendix A: Complete Command Reference

### Program Structure
- Labels: `LabelName:`
- Comments: `REM` or `'`
- `END`
- `STOP`

### Variables and Data
- `LET variable = expression`
- `DIM array(size)`
- `DATA value1, value2, ...`

### Control Flow
- `IF condition THEN statement`
- `IF condition THEN ... ELSE ... END IF`
- `FOR variable = start TO end [STEP step] ... NEXT`
- `WHILE condition ... WEND`
- `REPEAT ... UNTIL condition`
- `GOTO label`
- `GOSUB label` / `RETURN`
- `EXIT FOR` / `EXIT WHILE`
- `CONTINUE FOR` / `CONTINUE WHILE`

### Graphics
- `CLS [n]`
- `SCREEN n`
- `VIDEO n`
- `PLOT x, y [, color]`
- `LINE x1, y1, x2, y2 [, color]`
- `RECTANGLE x, y, width, height [, color]`
- `RECTANGLE FILLED x, y, width, height [, color]`
- `CIRCLE x, y, radius [, color]`
- `CIRCLE FILLED x, y, radius [, color]`
- `ELLIPSE x, y, radiusX, radiusY [, color]`
- `ELLIPSE FILLED x, y, radiusX, radiusY [, color]`
- `INK color`
- `PAPER color`
- `PRINT [expression] [;] [,] ...`
- `PRINT AT x, y; expression [; USING fontAddr, color, flags, maxWidth, outlineColor, outlinePattern]`
- `LOCATE x, y`
- `LOAD FONT filename$, address`
- `FONT address`
- `FONT COLOR color`
- `FONT FLAGS flags`
- `FONT MAXWIDTH width`
- `FONT OUTLINE color, pattern`
- `LAYER SHOW n` / `LAYER HIDE n`
- `LAYER VISIBILITY mask`
- `SPRITE x, y, address, width, height [, page]`

### Audio
- `BEEP duration, pitch [, volume]`
- `PLAY address`

### Input/Output
- `INPUT [prompt$;] variable`
- `INKEY$`
- `INKEY`
- `MOUSE X` / `MOUSE Y` / `MOUSE BUTTON n`
- `LOAD filename$ [, address]`
- `SAVE filename$, address, length`
- `OPEN filename$ FOR mode AS #n`
- `CLOSE #n`
- `READ #n, variable`
- `WRITE #n, expression`

### Math Functions
- `ABS(x)`, `SGN(x)`, `INT(x)`, `FIX(x)`
- `FLOOR(x)`, `CEIL(x)`, `ROUND(x [, decimals])`
- `SIN(x)`, `COS(x)`, `TAN(x)`
- `ASIN(x)`, `ACOS(x)`, `ATAN(x)`, `ATAN2(y, x)`
- `EXP(x)`, `LOG(x)`, `LOG10(x)`
- `SQR(x)`, `POW(x, y)`, `ISQR(x)`
- `MIN(x, y)`, `MAX(x, y)`
- `RND([x])`, `RANDOMIZE [seed]`
- `PI`, `E`

### String Functions
- `LEN(string$)`, `LEFT$(string$, n)`, `RIGHT$(string$, n)`, `MID$(string$, start [, length])`
- `CHR$(code)`, `ASC(string$)`
- `VAL(string$)`, `STR$(number)`
- `STRING$(n, char$)`
- `INSTR([start, ] string1$, string2$)`
- `UCASE$(string$)`, `LCASE$(string$)`
- `TRIM$(string$)`, `LTRIM$(string$)`, `RTRIM$(string$)`

### Memory
- `PEEK(address)`, `POKE address, value`
- `PEEK16(address)`, `POKE16 address, value`
- `PEEK24(address)`, `POKE24 address, value`
- `PEEK32(address)`, `POKE32 address, value`
- `MEMCOPY source, dest, length`
- `MEMFILL address, length, value`
- `VARPTR(variable)`

### System
- `WAIT frames`
- `SLEEP milliseconds`
- `TIME`, `TICKS`
- `ON ERROR GOTO label`
- `RESUME [NEXT]`
- `ERR`, `ERL`

---

## Appendix B: Example Programs

### Hello World

```
Main:
    CLS
    PRINT "Hello, World!"
    END
```

### Simple Loop

```
Main:
    FOR i = 1 TO 10
        PRINT "Count: "; i
    NEXT i
    END
```

### Graphics Example

```
Main:
    VIDEO 1
    CLS 0
    SCREEN 0
    
    FOR i = 0 TO 100
        PLOT i, i, 15
    NEXT i
    
    CIRCLE FILLED 240, 135, 50, 200
    END
```

### Audio Example

```
soundData:
    DATA 0, 0
    DATA 440.0
    DATA 0.5
    DATA 0.5

Main:
    PLAY soundData
    SLEEP 1000
    END
```

### Font and Text Example

```
Main:
    VIDEO 1
    CLS 0
    SCREEN 0
    
    REM Load font
    LOAD FONT "fonts/default.png", fontAddr
    
    REM Set current font
    FONT fontAddr
    FONT COLOR 15
    FONT FLAGS &B00111000  ' Center, wrap, outline
    FONT MAXWIDTH 320
    FONT OUTLINE 255, &B00100100
    
    REM Print text at pixel coordinates
    PRINT AT 100, 50; "Hello, World!"
    PRINT AT 100, 100; "Centered text"; USING fontAddr, 15
    
    END
```

### Input Example

```
Main:
    INPUT "Enter your name: "; name$
    PRINT "Hello, "; name$; "!"
    END
```

### Game Loop Example

```
Main:
    VIDEO 1
    CLS 0
    
GameLoop:
    REM Update game state
    LET x = MOUSE X
    LET y = MOUSE Y
    
    REM Clear and redraw
    CLS 0
    CIRCLE FILLED x, y, 10, 15
    
    REM Wait for next frame
    WAIT 1
    GOTO GameLoop
```

---

## Appendix C: Differences from ZX BASIC

1. **No Line Numbers**: Uses labels instead
2. **Extended Logical Operators**: IMPLY, NAND, NOR, XNOR
3. **Multi-layer Graphics**: SCREEN, VIDEO, CLS with page parameter
4. **Advanced Audio**: PLAY with parameter-based synthesis
5. **24-bit Addressing**: Larger address space
6. **Extended Math Functions**: FLOOR, CEIL, ISQR, ISGN
7. **Enhanced String Functions**: More comprehensive string operations
8. **Memory Operations**: PEEK/POKE variants for different data sizes
9. **Modern Control Structures**: EXIT, CONTINUE
10. **Layer Control**: LAYER SHOW/HIDE/VISIBILITY commands
11. **Font Management**: LOAD FONT, FONT commands for text rendering
12. **Type Suffixes**: Float variables use `#` suffix, strings use `$` suffix

---

## Document Version

**Version**: 1.0  
**Date**: 2024  
**Author**: Continuum 93 Documentation Team  
**Purpose**: Complete specification for BASIC compiler implementation

---

## End of Specification

This document provides a complete specification for the Continuum 93 BASIC programming language. Compiler implementers should refer to the machine architecture documentation (CPU_ARCHITECTURE.md, VIDEO_ARCHITECTURE.md, APU_ARCHITECTURE.md, INTERRUPTS.md) for low-level implementation details.

