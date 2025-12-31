# Interrupts Documentation

## Executive Summary

The Continuum 93 interrupt system provides a comprehensive interface for system services, video operations, input handling, file system access, random number generation, and string operations. Interrupts are triggered using the `INT` instruction with an interrupt number and a register that contains the function ID.

## Interrupt Format

```
INT <interrupt_id>, <register>
```

- **Interrupt ID**: 0x00-0x05 (interrupt category)
- **Register**: 8-bit register (A-Z) that contains the function ID (0x00-0xFF)
- **Parameters**: Start from the next register after the specified register (can be 8, 16, 24, or 32-bit depending on the interrupt)

**Example**:
```
LD A, 0x02        ; Load function ID 0x02 (SetVideoPagesCount) into register A
LD B, 3           ; Load parameter (page count) into register B
INT 0x01, A       ; Call interrupt 0x01 (Video), function from A, parameters from B onwards
```

The register specified in the INT instruction contains the function ID, and subsequent registers (starting from the next register) contain the parameters. Parameters can span multiple registers for 16-bit, 24-bit, or 32-bit values.

**Important Notes**:
- All parameter descriptions below use "Register+N" notation, where Register is the register specified in the INT instruction
- 8-bit parameters use a single register
- 16-bit parameters use 2 consecutive registers (e.g., CD, EF, GH)
- 24-bit parameters use 3 consecutive registers (e.g., BCD, EFG, HIJ)
- 32-bit parameters use 4 consecutive registers (e.g., ABCD, BCDE)
- Return values may be written back to the function ID register or to parameter registers

## Interrupt Categories

### INT 0x00: Machine/BIOS Interrupts

System-level operations and machine control.

#### 0x00: Stop
**Function**: Stops the computer execution
**Parameters**: None
**Returns**: None
**Description**: Halts CPU execution, equivalent to `BREAK` instruction.

#### 0x01: Clear
**Function**: Clears all system state
**Parameters**: None
**Returns**: None
**Description**: Clears RAM, HMEM, registers, float registers, and resets video pages.

#### 0x03: ReadClock
**Function**: Reads system clock
**Parameters** (starting from register after function ID register):
- Register+1: Mode (8-bit, 0x00 = milliseconds, 0x01 = ticks)
- Register+2-4: Destination address (24-bit, stored in 3 consecutive registers)
**Returns**: 
- Mode 0x00: 32-bit milliseconds since machine start (written to destination address)
- Mode 0x01: 64-bit ticks since machine start (written to destination address)
**Description**: Returns time since machine initialization.

**Example**:
```
LD A, 0x03        ; Function ID
LD B, 0x00        ; Mode: milliseconds
LD CDE, 0x10000   ; Destination address
INT 0x00, A       ; Call interrupt
```

#### 0x10: ToggleFullscreen
**Function**: Toggles fullscreen mode
**Parameters**: None (register after function ID register is unused)
**Returns**: Function ID register = 1 if fullscreen, 0 if windowed
**Description**: Toggles between fullscreen and windowed display mode.

**Example**:
```
LD A, 0x10        ; Function ID
INT 0x00, A       ; Toggle fullscreen, result in A
```

#### 0x20: ShutDown
**Function**: Shuts down the system
**Parameters**: None
**Returns**: None
**Description**: Requests system shutdown.

#### 0xC0: Build
**Function**: Compiles assembly source file
**Parameters** (starting from register after function ID register):
- Register+1-3: Path address (24-bit) - path to .asm file, stored in 3 consecutive registers
- Register+4: Error count register (8-bit)
**Returns**:
- Register+1-3: Start address (24-bit) - address where code was loaded, or 0xFFFFFF if file not found
- Register+4: Error count (8-bit) - number of compilation errors (max 255)
**Description**: Compiles an assembly source file and loads the resulting code into memory. Creates debug files in `debug/` subdirectory.

**Example**:
```
LD A, 0xC0        ; Function ID
LD BCD, .FilePath ; Path to .asm file (24-bit address)
LD E, 0           ; Error count register
INT 0x00, A       ; Build, result address in BCD, errors in E
```

#### 0xF0: GetCPUDesignationByFrequency
**Function**: Gets CPU designation string based on frequency
**Parameters** (starting from register after function ID register):
- Register+1-2: Frequency (16-bit) - CPU frequency in MHz, stored in 2 consecutive registers
- Register+3-5: Destination address (24-bit) - where to write designation string, stored in 3 consecutive registers
**Returns**: Null-terminated string at destination address
**Description**: Returns CPU designation (e.g., "Nova I", "Comet", "Phoenix Keanu", "Titan", etc.) based on frequency range.

**Example**:
```
LD A, 0xF0        ; Function ID
LD BC, 100        ; Frequency: 100 MHz
LD DEF, 0x20000   ; Destination address
INT 0x00, A       ; Get designation, written to address in DEF
```

### INT 0x01: Video Interrupts

Video and graphics operations.

**Note**: All video interrupts use the format `INT 0x01, <register>` where the register contains the function ID (8-bit). Parameters start from the next register after the function ID register. See individual function descriptions for parameter details.

#### Page Management

##### 0x00: ReadVideoResolution
**Function**: Reads video resolution
**Parameters** (starting from register after function ID register):
- Register+1-2: X register (16-bit destination) - width, stored in 2 consecutive registers
- Register+3-4: Y register (16-bit destination) - height, stored in 2 consecutive registers
**Returns**: 
- Register+1-2: Width (480, 16-bit)
- Register+3-4: Height (270, 16-bit)
**Description**: Returns current video resolution.

**Example**:
```
LD A, 0x00        ; Function ID
INT 0x01, A       ; Read resolution, width in BC, height in DE
```

##### 0x01: ReadVideoPagesCount
**Function**: Reads number of video pages
**Parameters**: None
**Returns**: Function ID register = number of video pages (1-8, 8-bit)
**Description**: Returns current number of video pages configured.

**Example**:
```
LD A, 0x01        ; Function ID
INT 0x01, A       ; Read page count, result in A
```

##### 0x02: SetVideoPagesCount
**Function**: Sets number of video pages
**Parameters** (starting from register after function ID register):
- Register+1: Page count (8-bit) - number of pages (1-8)
**Returns**: None
**Description**: Configures number of video pages. Recalculates VRAM layout.

**Example**:
```
LD A, 0x02        ; Function ID
LD B, 3           ; Page count: 3
INT 0x01, A       ; Set video pages count
```

##### 0x03: ReadVideoAddress
**Function**: Reads video page address
**Parameters** (starting from register after function ID register):
- Register+1: Page number (8-bit)
- Register+2-4: Address register (24-bit destination) - stored in 3 consecutive registers
**Returns**: Register+2-4 = video page address (24-bit)
**Description**: Returns memory address of specified video page.

**Example**:
```
LD A, 0x03        ; Function ID
LD B, 0           ; Page number: 0
LD CDE, 0         ; Address register (destination)
INT 0x01, A       ; Read address, result in CDE
```

##### 0x04: ReadVideoPaletteAddress
**Function**: Reads video palette address
**Parameters** (starting from register after function ID register):
- Register+1: Page number (8-bit)
- Register+2-4: Address register (24-bit destination) - stored in 3 consecutive registers
**Returns**: Register+2-4 = palette address (24-bit)
**Description**: Returns memory address of palette for specified video page.

**Example**:
```
LD A, 0x04        ; Function ID
LD B, 0           ; Page number: 0
LD CDE, 0         ; Address register (destination)
INT 0x01, A       ; Read palette address, result in CDE
```

##### 0x05: ClearVideoPage
**Function**: Clears video page with color
**Parameters** (starting from register after function ID register):
- Register+1: Page number (8-bit)
- Register+2: Color (8-bit)
**Returns**: None
**Description**: Fills entire video page with specified color.

**Example**:
```
LD A, 0x05        ; Function ID
LD B, 0           ; Page number: 0
LD C, 0x85        ; Color: 0x85
INT 0x01, A       ; Clear video page
```

#### Drawing Primitives

##### 0x06: DrawFilledRectangle
**Function**: Draws filled rectangle
**Parameters** (starting from register after function ID register):
- Register+1: Video page (8-bit)
- Register+2-3: X position (16-bit signed) - stored in 2 consecutive registers
- Register+4-5: Y position (16-bit signed) - stored in 2 consecutive registers
- Register+6-7: Width (16-bit) - stored in 2 consecutive registers
- Register+8-9: Height (16-bit) - stored in 2 consecutive registers
- Register+10: Color (8-bit)
**Returns**: None
**Description**: Draws filled rectangle at specified position and size.

**Example**:
```
LD A, 0x06        ; Function ID
LD B, 0           ; Video page: 0
LD CD, 10         ; X position: 10
LD EF, 20         ; Y position: 20
LD GH, 100        ; Width: 100
LD IJ, 50         ; Height: 50
LD K, 200         ; Color: 200
INT 0x01, A       ; Draw filled rectangle
```

##### 0x07: DrawRectangle
**Function**: Draws rectangle outline
**Parameters**: Same as 0x06
**Returns**: None
**Description**: Draws rectangle outline (border only).

##### 0x08: DrawFilledRoundedRectangle
**Function**: Draws filled rounded rectangle
**Parameters**: Same as 0x06, plus corner radius
**Returns**: None
**Description**: Draws filled rectangle with rounded corners.

##### 0x09: DrawRoundedRectangle
**Function**: Draws rounded rectangle outline
**Parameters**: Same as 0x08
**Returns**: None
**Description**: Draws rounded rectangle outline.

##### 0x0E: DrawTileMapSprite
**Function**: Draws tile map sprite
**Parameters**: Tile map data structure
**Returns**: None
**Description**: Draws sprite from tile map data.

##### 0x10: DrawSpriteToVideoPage
**Function**: Draws sprite to video page
**Parameters** (starting from register after function ID register):
- Register+1-3: Source address (24-bit) - sprite data, stored in 3 consecutive registers
- Register+4-5: X position (16-bit signed) - stored in 2 consecutive registers
- Register+6-7: Y position (16-bit signed) - stored in 2 consecutive registers
- Register+8-9: Width (16-bit) - stored in 2 consecutive registers
- Register+10-11: Height (16-bit) - stored in 2 consecutive registers
- Register+12: Video page (8-bit)
**Returns**: None
**Description**: Draws sprite from memory to video page with clipping.

**Example**:
```
LD A, 0x10        ; Function ID
LD BCD, .SpriteData ; Source address
LD EF, 10         ; X position
LD GH, 20         ; Y position
LD IJ, 32         ; Width
LD KL, 32         ; Height
LD M, 0           ; Video page
INT 0x01, A       ; Draw sprite
```

##### 0x20: Plot
**Function**: Draws single pixel
**Parameters** (starting from register after function ID register):
- Register+1: Video page (8-bit)
- Register+2-3: X position (16-bit signed) - stored in 2 consecutive registers
- Register+4-5: Y position (16-bit signed) - stored in 2 consecutive registers
- Register+6: Color (8-bit)
**Returns**: None
**Description**: Sets pixel at specified coordinates.

**Example**:
```
LD A, 0x20        ; Function ID
LD B, 0           ; Video page
LD CD, 100        ; X position
LD EF, 50         ; Y position
LD G, 200         ; Color
INT 0x01, A       ; Plot pixel
```

##### 0x21: Line
**Function**: Draws line
**Parameters** (starting from register after function ID register):
- Register+1: Video page (8-bit)
- Register+2-3: X1 (16-bit signed) - stored in 2 consecutive registers
- Register+4-5: Y1 (16-bit signed) - stored in 2 consecutive registers
- Register+6-7: X2 (16-bit signed) - stored in 2 consecutive registers
- Register+8-9: Y2 (16-bit signed) - stored in 2 consecutive registers
- Register+10: Color (8-bit)
**Returns**: None
**Description**: Draws line between two points using Bresenham algorithm.

**Example**:
```
LD A, 0x21        ; Function ID
LD B, 0           ; Video page
LD CD, 10         ; X1
LD EF, 20         ; Y1
LD GH, 100        ; X2
LD IJ, 80         ; Y2
LD K, 200         ; Color
INT 0x01, A       ; Draw line
```

##### 0x22: Ellipse
**Function**: Draws ellipse outline
**Parameters**:
- Register+1: Video page (8-bit)
- Register+2: Center X (16-bit signed)
- Register+4: Center Y (16-bit signed)
- Register+6: Radius X (16-bit)
- Register+8: Radius Y (16-bit)
- Register+10: Color (8-bit)
**Returns**: None
**Description**: Draws ellipse outline.

##### 0x23: DrawFilledEllipse
**Function**: Draws filled ellipse
**Parameters**: Same as 0x22
**Returns**: None
**Description**: Draws filled ellipse.

##### 0x24: LinePath
**Function**: Draws connected line segments
**Parameters**:
- Register+1: Path address (24-bit) - array of (X, Y) points
- Register+4: Video page (8-bit)
- Register+5: Color (8-bit)
- Register+6: Point count (8-bit)
**Returns**: None
**Description**: Draws connected line segments from point array.

##### 0x25: DrawBezierPath
**Function**: Draws Bezier curve path
**Parameters**:
- Register+1: Bezier path address (24-bit) - Bezier segment data
- Register+4: Video page (8-bit)
- Register+5: Line brush width (8-bit)
- Register+6: Fill segment size (8-bit)
- Register+7: Empty segment size (8-bit)
- Register+8: Color (8-bit)
- Register+9: Outline color (8-bit)
- Register+10: Start percent (8-bit) - 0-100
- Register+11: End percent (8-bit) - 0-100
**Returns**: None
**Description**: Draws Bezier curve path with optional dashed pattern and partial rendering.

**Bezier Path Format**:
- Initial point: X (16-bit), Y (16-bit)
- Segments: Control X (16-bit), Control Y (16-bit), End X (16-bit), End Y (16-bit)
- Terminator: 0xFFFF

##### 0x26: DrawPerlinPath
**Function**: Draws Perlin noise-based path
**Parameters**:
- Register+1: Video page (8-bit)
- Register+2: Start X (16-bit signed)
- Register+4: Start Y (16-bit signed)
- Register+6: End X (16-bit signed)
- Register+8: End Y (16-bit signed)
- Register+10: Color (8-bit)
- Register+12: Pattern 1 seed (16-bit)
- Register+14: Pattern 1 min Y (16-bit signed)
- Register+16: Pattern 1 max Y (16-bit signed)
- Register+18: Pattern 1 zoom (8-bit)
- Register+19: Pattern 1 shift (16-bit signed)
- Register+21: Pattern 2 seed (16-bit) - optional (0 disables)
- Register+23: Pattern 2 min Y (16-bit signed)
- Register+25: Pattern 2 max Y (16-bit signed)
- Register+27: Pattern 2 zoom (8-bit) - 0 disables pattern 2
- Register+28: Pattern 2 shift (16-bit signed)
**Returns**: None
**Description**: Draws path with Perlin noise-based variation (e.g., mountain ranges).

##### 0x28: FillAreaOrPolygon
**Function**: Fills area or polygon
**Parameters**:
- Register+1: Video page (8-bit)
- Register+2: Start X (16-bit)
- Register+4: Start Y (16-bit)
- Register+6: Fill color (8-bit)
- Register+7: Border color (8-bit)
- Register+8: Flags (8-bit)
  - Bit 1: Border fill mode (1 = fill inside border, 0 = flood fill)
**Returns**: None
**Description**: Fills connected area (flood fill) or area bounded by border color (boundary fill).

#### Text Rendering

##### 0x11: GetTextMetrics
**Function**: Gets text dimensions
**Parameters**:
- Register+1: Font address (24-bit)
- Register+4: String address (24-bit)
- Register+7: Max width (16-bit)
- Register+13: Flags (8-bit)
**Returns**:
- Register: Width (16-bit)
- Register+2: Height (16-bit)
**Description**: Calculates text dimensions without rendering.

##### 0x12: DrawString
**Function**: Draws string
**Parameters**:
- Register+1: Font address (24-bit)
- Register+4: String address (24-bit)
- Register+7: X position (16-bit signed)
- Register+9: Y position (16-bit signed)
- Register+11: Color (8-bit)
- Register+12: Video page (8-bit)
**Returns**: None
**Description**: Draws string using font data.

##### 0x14: DrawText
**Function**: Draws text with advanced options
**Parameters** (starting from register after function ID register):
- Register+1-3: Font address (24-bit) - stored in 3 consecutive registers
- Register+4-6: String address (24-bit) - stored in 3 consecutive registers
- Register+7-8: X position (16-bit signed) - stored in 2 consecutive registers
- Register+9-10: Y position (16-bit signed) - stored in 2 consecutive registers
- Register+11: Color (8-bit)
- Register+12: Video page (8-bit)
- Register+13-14: Max width (16-bit) - stored in 2 consecutive registers
- Register+15: Flags (8-bit)
  - Bit 0: Monospace
  - Bit 1: Monospace centering
  - Bit 2: Disable kerning
  - Bit 3: Center
  - Bit 4: Wrap
  - Bit 5: Draw outline
- Register+16: Outline color (8-bit)
- Register+17: Outline pattern (8-bit)
**Returns**:
- Function ID register-2 consecutive registers: Width (16-bit)
- Function ID register-1 consecutive register: Height (16-bit)
**Description**: Draws text with monospace, kerning, centering, wrapping, and outline support.

**Example**:
```
LD A, 0x14        ; Function ID
LD BCD, .FontData ; Font address
LD EFG, .Text     ; String address
LD HI, 80         ; X position
LD JK, 40         ; Y position
LD L, 0           ; Color
LD M, 0           ; Video page
LD NO, 320        ; Max width
LD P, 0b00111000  ; Flags
LD Q, 0xFF        ; Outline color
LD R, 0b00100100  ; Outline pattern
INT 0x01, A       ; Draw text (width in BC, height in DE)
```

##### 0x15: GetTextRectangle
**Function**: Gets text bounding rectangle
**Parameters**: Same as 0x14
**Returns**:
- Register: Width (16-bit)
- Register+2: Height (16-bit)
**Description**: Calculates text bounding rectangle with all formatting options.

#### Layer Control

##### 0x30: ReadLayerVisibility
**Function**: Reads layer visibility
**Parameters**: None
**Returns**: Function ID register = visibility bitmask (8 bits, one per layer)
**Description**: Returns which layers are visible.

**Example**:
```
LD A, 0x30        ; Function ID
INT 0x01, A       ; Read visibility (result in A)
```

##### 0x31: SetLayersVisibility
**Function**: Sets layer visibility
**Parameters** (starting from register after function ID register):
- Register+1: Visibility bitmask (8-bit) - one bit per layer (0 = hidden, 1 = visible)
**Returns**: None
**Description**: Shows/hides video layers.

**Example**:
```
LD A, 0x31        ; Function ID
LD B, 0b11111111  ; All layers visible
INT 0x01, A       ; Set layer visibility
```

##### 0x32: ReadBufferControlMode
**Function**: Reads buffer control mode
**Parameters**: Register (8-bit destination)
**Returns**: Register = buffer mode bitmask (8 bits)
**Description**: Returns which layers use auto/manual buffer updates.

##### 0x33: SetBufferControlMode
**Function**: Sets buffer control mode
**Parameters**:
- Register: Buffer mode bitmask (8-bit) - one bit per layer (0 = manual, 1 = auto)
**Returns**: None
**Description**: Sets auto/manual buffer update mode for layers.

#### Video Operations

##### 0x40: Scroll
**Function**: Scrolls or rolls video page region
**Parameters**:
- Register+1: Video page (8-bit)
- Register+2: X (16-bit)
- Register+4: Y (16-bit)
- Register+6: Width (16-bit)
- Register+8: Height (16-bit)
- Register+10: Scroll X (16-bit signed)
- Register+12: Scroll Y (16-bit signed)
- Register+14: Flags (8-bit)
  - Bit 0: Horizontal rolling (1 = wrap, 0 = scroll with fill)
  - Bit 1: Vertical rolling (1 = wrap, 0 = scroll with fill)
**Returns**: None
**Description**: Scrolls or rolls rectangular region. Rolling wraps content, scrolling fills vacated areas with 0.

##### 0x41: CopyRectangle
**Function**: Copies rectangle between video pages
**Parameters**:
- Register+1: Source page (8-bit)
- Register+2: Source X (16-bit)
- Register+4: Source Y (16-bit)
- Register+6: Source width (16-bit)
- Register+8: Source height (16-bit)
- Register+10: Dest page (8-bit)
- Register+11: Dest X (16-bit)
- Register+13: Dest Y (16-bit)
- Register+15: Dest width (16-bit)
- Register+17: Dest height (16-bit)
**Returns**: None
**Description**: Copies rectangular region with nearest-neighbor scaling.

### INT 0x02: Input Interrupts

Input device handling.

**Note**: All input interrupts use the format `INT 0x02, <register>` where the register contains the function ID (8-bit). Parameters start from the next register after the function ID register. See individual function descriptions for parameter details.

#### 0x00: ReadKeyboardStateAsBits
**Function**: Reads keyboard state as bit array
**Parameters** (starting from register after function ID register):
- Register+1-3: Destination address (24-bit) - stored in 3 consecutive registers
**Returns**:
- Function ID register: Array length (8-bit)
- Memory: Keyboard state bits
**Description**: Returns keyboard state as bit array.

**Example**:
```
LD A, 0x00        ; Function ID
LD BCD, .Buffer   ; Destination address
INT 0x02, A       ; Read keyboard state (length in A)
```

#### 0x01: ReadKeyboardBuffer
**Function**: Reads character from keyboard buffer
**Parameters**: None
**Returns**:
- Function ID register: Character code (8-bit) - 0 if buffer empty
- Register+1: Remaining count (8-bit)
**Description**: Reads next character from keyboard input buffer.

**Example**:
```
LD A, 0x01        ; Function ID
INT 0x02, A       ; Read keyboard buffer (character in A, remaining in B)
```

#### 0x02: HandleKeyboardStateChanged
**Function**: Sets keyboard callback address
**Parameters** (starting from register after function ID register):
- Register+1-3: Callback address (24-bit) - 0xFFFFFF to disable, stored in 3 consecutive registers
**Returns**: None
**Description**: Sets callback address for keyboard state changes. Callback is called when key is pressed.

**Example**:
```
LD A, 0x02        ; Function ID
LD BCD, .Callback ; Callback address
INT 0x02, A       ; Set keyboard callback
```

#### 0x03: ReadMouseState
**Function**: Reads mouse state
**Parameters** (starting from register after function ID register):
- Register+1-2: X register (16-bit destination) - stored in 2 consecutive registers
- Register+3-4: Y register (16-bit destination) - stored in 2 consecutive registers
- Register+5: Button state register (8-bit destination)
**Returns**:
- Register+1-2: Mouse X (16-bit)
- Register+3-4: Mouse Y (16-bit)
- Register+5: Button state (8-bit)
  - Bit 0: Left button
  - Bit 1: Right button
  - Bit 2: Middle button
**Description**: Returns current mouse position and button state.

**Example**:
```
LD A, 0x03        ; Function ID
LD BCD, 0         ; X register (destination)
LD EFG, 0         ; Y register (destination)
LD HIJ, 0         ; Button state register (destination)
INT 0x02, A       ; Read mouse state (X in CD, Y in EF, buttons in I)
```

#### 0x04: HandleMouseStateChanged
**Function**: Sets mouse callback address
**Parameters**:
- Register+1: Callback address (24-bit) - 0xFFFFFF to disable
**Returns**: None
**Description**: Sets callback address for mouse state changes.

#### 0x05: (Reserved)
**Function**: Reserved
**Parameters**: None
**Returns**: None

#### 0x06: HandleControllerStateChanged
**Function**: Sets controller callback address
**Parameters**:
- Register+1: Callback address (24-bit) - 0xFFFFFF to disable
**Returns**: None
**Description**: Sets callback address for gamepad/controller state changes.

#### 0x10: ReadKeyboardStateAsBytes
**Function**: Reads keyboard state as byte codes
**Parameters**:
- Register+1: Destination address (24-bit)
**Returns**:
- Register: Array length (8-bit)
- Memory: Keyboard key codes
**Description**: Returns pressed keys as byte array of key codes.

#### 0x14: ReadGamePadsState
**Function**: Reads gamepad state
**Parameters**:
- Register+1: Destination address (24-bit)
**Returns**: Memory: Gamepad state data
**Description**: Returns state of all connected gamepads.

#### 0x15: ReadGamePadsCapabilities
**Function**: Reads gamepad capabilities
**Parameters**:
- Register+1: Destination address (24-bit)
**Returns**: Memory: Gamepad capabilities data
**Description**: Returns capabilities of all connected gamepads.

#### 0x16: ReadGamePadsNames
**Function**: Reads gamepad names
**Parameters**:
- Register+1: Destination address (24-bit)
**Returns**:
- Register: String length (8-bit)
- Memory: Null-terminated gamepad name strings
**Description**: Returns display names of all connected gamepads.

### INT 0x03: Random Interrupts

Random number generation.

**Note**: All random interrupts use the format `INT 0x03, <register>` where the register contains the function ID (8-bit). Parameters start from the next register after the function ID register. Return values are written to the function ID register (and subsequent registers for 16/24/32-bit values). See individual function descriptions for parameter details.

#### 0x00: Random8Bit
**Function**: Generates random 8-bit value
**Parameters**: None
**Returns**: Function ID register = random value (0-255)
**Description**: Generates random 8-bit unsigned integer.

**Example**:
```
LD A, 0x00        ; Function ID
INT 0x03, A       ; Random 8-bit (result in A)
```

#### 0x01: Random8BitCustom
**Function**: Generates random 8-bit value in range
**Parameters**:
- Register+1: Maximum value (8-bit)
**Returns**: Register+1 = random value (0 to max-1)
**Description**: Generates random 8-bit value in specified range.

#### 0x02: Random16Bit
**Function**: Generates random 16-bit value
**Parameters**: None
**Returns**: Function ID register and next register = random value (0-65535, 16-bit)
**Description**: Generates random 16-bit unsigned integer.

**Example**:
```
LD A, 0x02        ; Function ID
INT 0x03, A       ; Random 16-bit (result in AB)
```

#### 0x03: Random16BitCustom
**Function**: Generates random 16-bit value in range
**Parameters**:
- Register+1: Maximum value (16-bit)
**Returns**: Register+1 = random value (0 to max-1)
**Description**: Generates random 16-bit value in specified range.

#### 0x04: Random24Bit
**Function**: Generates random 24-bit value
**Parameters**: None
**Returns**: Function ID register and next 2 registers = random value (0-16777215, 24-bit)
**Description**: Generates random 24-bit unsigned integer.

**Example**:
```
LD A, 0x04        ; Function ID
INT 0x03, A       ; Random 24-bit (result in ABC)
```

#### 0x05: Random24BitCustom
**Function**: Generates random 24-bit value in range
**Parameters**:
- Register+1: Maximum value (24-bit)
**Returns**: Register+1 = random value (0 to max-1)
**Description**: Generates random 24-bit value in specified range.

#### 0x06: Random32Bit
**Function**: Generates random 32-bit value
**Parameters**: None
**Returns**: Function ID register and next 3 registers = random value (full 32-bit range)
**Description**: Generates random 32-bit signed integer.

**Example**:
```
LD A, 0x06        ; Function ID
INT 0x03, A       ; Random 32-bit (result in ABCD)
```

#### 0x07: Random32BitCustom
**Function**: Generates random 32-bit value in range
**Parameters**:
- Register+1: Maximum value (32-bit)
**Returns**: Register+1 = random value (-max/2 to max/2)
**Description**: Generates random 32-bit signed integer in symmetric range.

### INT 0x04: File System Interrupts

File and directory operations.

**Note**: All file system interrupts use the format `INT 0x04, <register>` where the register contains the function ID (8-bit). Parameters start from the next register after the function ID register. See individual function descriptions for parameter details.

#### General Operations

##### 0x02: CheckIfFileExists
**Function**: Checks if file exists
**Parameters** (starting from register after function ID register):
- Register+1-3: Path address (24-bit) - null-terminated string, stored in 3 consecutive registers
**Returns**: Function ID register = 0xFF if exists, 0x00 if not
**Description**: Checks if file exists in filesystem root.

**Example**:
```
LD A, 0x02        ; Function ID
LD BCD, .FilePath ; Path address
INT 0x04, A       ; Check if file exists (result in A)
```

##### 0x03: CheckIfDirectoryExists
**Function**: Checks if directory exists
**Parameters** (starting from register after function ID register):
- Register+1-3: Path address (24-bit) - null-terminated string, stored in 3 consecutive registers
**Returns**: Function ID register = 0xFF if exists, 0x00 if not
**Description**: Checks if directory exists in filesystem root.

**Example**:
```
LD A, 0x03        ; Function ID
LD BCD, .DirPath  ; Path address
INT 0x04, A       ; Check if directory exists (result in A)
```

##### 0x04: ListFilesInDirectory
**Function**: Lists files in directory
**Parameters** (starting from register after function ID register):
- Register+1-3: Path address (24-bit) - directory path, stored in 3 consecutive registers
- Register+4-6: Target address (24-bit) - where to write file list, stored in 3 consecutive registers
**Returns**:
- Register+1-3: File count (24-bit) - stored in 3 consecutive registers
- Memory: Null-terminated file names
**Description**: Lists all files in specified directory.

**Example**:
```
LD A, 0x04        ; Function ID
LD BCD, .DirPath  ; Directory path
LD EFG, .Buffer   ; Target address
INT 0x04, A       ; List files (count in BCD)
```

##### 0x05: ListDirectoriesInDirectory
**Function**: Lists directories in directory
**Parameters**:
- Register+1: Path address (24-bit) - directory path
- Register+4: Target address (24-bit) - where to write directory list
**Returns**:
- Register+1: Directory count (24-bit)
- Memory: Null-terminated directory names
**Description**: Lists all subdirectories in specified directory.

##### 0x06: SaveFile
**Function**: Saves file
**Parameters**:
- Register+1: Path address (24-bit) - file path
- Register+4: Source address (24-bit) - data to save
- Register+7: Source length (24-bit) - bytes to save
**Returns**: None
**Description**: Saves data from memory to file.

##### 0x07: LoadFile
**Function**: Loads file
**Parameters** (starting from register after function ID register):
- Register+1-3: Path address (24-bit) - file path, stored in 3 consecutive registers
- Register+4-6: Destination address (24-bit) - where to load file, stored in 3 consecutive registers
**Returns**: None
**Description**: Loads file into memory.

**Example**:
```
LD A, 0x07        ; Function ID
LD BCD, .FilePath ; Path address
LD EFG, 0x20000   ; Destination address
INT 0x04, A       ; Load file
```

##### 0x15: ListDirectoriesAndFilesInDirectory
**Function**: Lists directories and files
**Parameters**:
- Register+1: Path address (24-bit) - directory path
- Register+4: Target address (24-bit) - where to write list
**Returns**:
- Register+1: Total count (24-bit)
- Register+4: Directory count (24-bit)
- Register+7: File count (24-bit)
- Register+10: File list start offset (24-bit)
- Memory: Directories first, then files (null-terminated)
**Description**: Lists both directories and files in specified directory.

##### 0x20: GetFileSize
**Function**: Gets file size
**Parameters**:
- Register+1: Path address (24-bit) - file path
**Returns**: Register+1 = file size (32-bit)
**Description**: Returns file size in bytes.

#### Image Operations

##### 0x30: LoadImageAndPalette
**Function**: Loads image and palette
**Parameters**:
- Register+1: Path address (24-bit) - image file path
- Register+4: Destination address (24-bit) - where to load
**Returns**: None (asynchronous, pauses computer)
**Description**: Loads image and palette asynchronously. Computer pauses until load completes.

##### 0x31: LoadImage
**Function**: Loads image only
**Parameters**:
- Register+1: Path address (24-bit) - image file path
- Register+4: Destination address (24-bit) - where to load
**Returns**: None (asynchronous, pauses computer)
**Description**: Loads image data only (no palette).

##### 0x32: LoadPalette
**Function**: Loads palette only
**Parameters**:
- Register+1: Path address (24-bit) - palette file path
- Register+4: Destination address (24-bit) - where to load
**Returns**: None (asynchronous, pauses computer)
**Description**: Loads palette data only.

#### PNG Operations

##### 0x33: Load8BitPNG
**Function**: Loads 8-bit PNG image
**Parameters** (starting from register after function ID register):
- Register+1-3: Path address (24-bit) - PNG file path, stored in 3 consecutive registers
- Register+4-6: Palette destination (24-bit) - stored in 3 consecutive registers
- Register+7-9: Image destination (24-bit) - stored in 3 consecutive registers
**Returns**:
- Function ID register: Error code (8-bit) - 0 = success
- Register+1-2: Width (16-bit) - stored in 2 consecutive registers
- Register+3-4: Height (16-bit) - stored in 2 consecutive registers
- Register+5: Color count (8-bit)
- Memory: Palette data (768 bytes), Image data
**Description**: Loads 8-bit indexed PNG with palette extraction.

**Example**:
```
LD A, 0x33        ; Function ID
LD BCD, .PNGPath  ; PNG file path
LD EFG, .Palette  ; Palette destination
LD HIJ, .Image    ; Image destination
INT 0x04, A       ; Load PNG (error in A, width in BC, height in DE, colors in F)
```

##### 0x34: Load8BitPNGCustomTransparency
**Function**: Loads 8-bit PNG with custom transparency
**Parameters**:
- Register+1: Path address (24-bit) - PNG file path
- Register+4: Palette destination (24-bit)
- Register+7: Image destination (24-bit)
- Register+10: Transparency R (8-bit)
- Register+11: Transparency G (8-bit)
- Register+12: Transparency B (8-bit)
- Register+13: Transparency A (8-bit)
**Returns**: Same as 0x33
**Description**: Loads PNG with specified color treated as transparent (moved to palette index 0).

##### 0x35: Merge8BitPNG
**Function**: Merges 8-bit PNG into existing image
**Parameters**: Same as 0x33, plus merge position
**Returns**: Same as 0x33
**Description**: Merges PNG into existing image at specified position.

##### 0x36: Merge8BitPNGCustomTransparency
**Function**: Merges 8-bit PNG with custom transparency
**Parameters**: Same as 0x34, plus merge position
**Returns**: Same as 0x34
**Description**: Merges PNG with custom transparency into existing image.

#### Font Operations

##### 0x40: LoadPNGFont
**Function**: Loads PNG font
**Parameters**:
- Register+1: Path address (24-bit) - PNG font file path
- Register+4: Destination address (24-bit) - where to load font
**Returns**:
- Register: Error code (8-bit) - 0 = success
- Register: Font data size (24-bit)
- Memory: Font data structure
**Description**: Loads PNG font with kerning data. Font must be 16×6 grid divisible. Kerning file (.txt) loaded if present.

**Font Format**:
- Byte 0: Font type
- Byte 1: Glyph cell width
- Byte 2: Glyph cell height
- Byte 3: Max glyph width
- Byte 4: Max glyph height
- Byte 5: Kerning pair count
- Bytes 6-101: Glyph widths (96 bytes, one per ASCII 32-127)
- Bytes 102+: Kerning pairs (3 bytes each: first, second, offset)
- Bytes after: Glyph bitmap data

### INT 0x05: String Interrupts

String conversion operations.

**Note**: All string interrupts use the format `INT 0x05, <register>` where the register contains the function ID (8-bit). Parameters start from the next register after the function ID register. See individual function descriptions for parameter details.

#### 0x01: FloatToString
**Function**: Converts float to string
**Parameters** (starting from register after function ID register):
- Register+1: Float register index (8-bit) - F0-F15
- Register+2-4: Format string address (24-bit) - C# format string, stored in 3 consecutive registers
- Register+5-7: Target address (24-bit) - where to write string, stored in 3 consecutive registers
**Returns**: Memory: Null-terminated string
**Description**: Converts float register value to string using format specifier.

**Example**:
```
LD A, 0x01        ; Function ID
LD B, 0           ; Float register index (F0)
LD CDE, .Format   ; Format string address
LD FGH, .Output   ; Target address
INT 0x05, A       ; Float to string
```

#### 0x02: UintToString
**Function**: Converts unsigned integer to string
**Parameters** (starting from register after function ID register):
- Register+1-4: Value register (32-bit) - unsigned integer value, stored in 4 consecutive registers
- Register+5-7: Format string address (24-bit) - C# format string, stored in 3 consecutive registers
- Register+8-10: Target address (24-bit) - where to write string, stored in 3 consecutive registers
**Returns**: Memory: Null-terminated string
**Description**: Converts 32-bit unsigned integer to string using format specifier.

**Example**:
```
LD A, 0x02        ; Function ID
LD BCDE, 12345    ; Unsigned integer value (32-bit)
LD FGH, .Format   ; Format string address
LD IJK, .Output   ; Target address
INT 0x05, A       ; Uint to string
```

#### 0x03: IntToString
**Function**: Converts signed integer to string
**Parameters** (starting from register after function ID register):
- Register+1-4: Value register (32-bit signed) - signed integer value, stored in 4 consecutive registers
- Register+5-7: Format string address (24-bit) - C# format string, stored in 3 consecutive registers
- Register+8-10: Target address (24-bit) - where to write string, stored in 3 consecutive registers
**Returns**: Memory: Null-terminated string
**Description**: Converts 32-bit signed integer to string using format specifier.

**Example**:
```
LD A, 0x03        ; Function ID
LD BCDE, -12345   ; Signed integer value (32-bit)
LD FGH, .Format   ; Format string address
LD IJK, .Output   ; Target address
INT 0x05, A       ; Int to string
```

## Interrupt Callbacks

Interrupts support callback addresses for event-driven programming:

- **Keyboard Callback**: Set via `INT 0x02, A` where A contains function ID 0x02. Called when key is pressed.
- **Mouse Callback**: Set via `INT 0x02, A` where A contains function ID 0x04. Called when mouse state changes.
- **Controller Callback**: Set via `INT 0x02, A` where A contains function ID 0x06. Called when controller state changes.

**Callback Mechanism**:
1. Callback address stored in `InterruptCallbacks`
2. When event occurs, computer pauses
3. Current IPO pushed to register stack
4. IPO set to callback address
5. Computer unpauses, execution continues at callback
6. Callback should use RET to return

## Usage Examples

```
; Machine interrupts
LD A, 0x00
INT 0x00, A       ; Stop computer

LD A, 0x03
LD B, 0x00        ; Mode: milliseconds
LD CDE, 0x10000   ; Destination address
INT 0x00, A       ; Read clock

LD A, 0xC0
LD BCD, .FilePath ; Path to .asm file
LD E, 0           ; Error count register
INT 0x00, A       ; Build assembly file

; Video interrupts
LD A, 0x00
INT 0x01, A       ; Read video resolution (width in BC, height in DE)

LD A, 0x05
LD B, 0           ; Page number
LD C, 0x85        ; Color
INT 0x01, A       ; Clear video page

LD A, 0x20
LD B, 0           ; Video page
LD CD, 100        ; X position
LD EF, 50         ; Y position
LD G, 200         ; Color
INT 0x01, A       ; Plot pixel

LD A, 0x14
LD BCD, .FontData ; Font address
LD EFG, .Text     ; String address
LD HI, 80         ; X position
LD JK, 40         ; Y position
LD L, 0           ; Color
LD M, 0           ; Video page
LD NO, 320        ; Max width
LD P, 0b00111000  ; Flags
LD Q, 0xFF        ; Outline color
LD R, 0b00100100  ; Outline pattern
INT 0x01, A       ; Draw text

; Input interrupts
LD A, 0x00
LD BCD, .Buffer   ; Destination address
INT 0x02, A       ; Read keyboard state (length in A)

LD A, 0x03
LD BCD, 0         ; X register (destination)
LD EFG, 0         ; Y register (destination)
LD HIJ, 0         ; Button state register (destination)
INT 0x02, A       ; Read mouse state

LD A, 0x02
LD BCD, .Callback ; Callback address
INT 0x02, A       ; Set keyboard callback

; Random interrupts
LD A, 0x00
INT 0x03, A       ; Random 8-bit (result in A)

LD A, 0x06
INT 0x03, A       ; Random 32-bit (result in ABCD)

; File system interrupts
LD A, 0x02
LD BCD, .FilePath ; Path address
INT 0x04, A       ; Check if file exists (result in A: 0xFF = exists, 0x00 = not)

LD A, 0x07
LD BCD, .FilePath ; Path address
LD EFG, 0x20000   ; Destination address
INT 0x04, A       ; Load file

LD A, 0x33
LD BCD, .PNGPath  ; PNG file path
LD EFG, .Palette  ; Palette destination
LD HIJ, .Image    ; Image destination
INT 0x04, A       ; Load 8-bit PNG

; String interrupts
LD A, 0x01
LD B, 0           ; Float register index (F0)
LD CDE, .Format   ; Format string address
LD FGH, .Output   ; Target address
INT 0x05, A       ; Float to string

LD A, 0x03
LD BCDE, 12345    ; Integer value (32-bit)
LD FGH, .Format   ; Format string address
LD IJK, .Output   ; Target address
INT 0x05, A       ; Int to string
```

## Conclusion

The Continuum 93 interrupt system provides comprehensive access to system services, video operations, input handling, file system, random number generation, and string operations. Interrupts use a consistent parameter passing convention via registers and support callback mechanisms for event-driven programming.

