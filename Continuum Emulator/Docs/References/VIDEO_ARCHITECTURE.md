# Video/Graphics Architecture Documentation

## Executive Summary

The Continuum 93 video architecture provides a **multi-layer graphics system** with up to 8 video pages (layers), palette-based color rendering, and double buffering. The system supports 480×270 resolution, 256-color palettes per layer, transparency, and automatic/manual buffer management.

## File Structure

- **Graphics.cs**: Main graphics system managing video pages, palettes, and rendering

## Video Resolution

- **Width**: 480 pixels
- **Height**: 270 pixels
- **Total Pixels**: 129,600 pixels per page
- **Aspect Ratio**: 16:9

## Video Pages (Layers)

### Page Organization

- **Maximum Pages**: 8 video pages (layers 0-7)
- **Page Size**: 129,600 bytes (480 × 270)
- **Palette per Page**: 256 colors (768 bytes: 256 × 3 bytes RGB)
- **Total Page Size**: 130,368 bytes (129,600 + 768)

### Page Addressing

Video pages are located at the end of main RAM:
- **VRAM Offset**: `0x1000000 - V_SIZE * VRAM_PAGES`
- **Page Address**: `GetVideoPageAddress(page)` = `0x1000000 - V_SIZE * (page + 1)`
- **Palette Address**: `GetVideoPagePaletteAddress(page)` = `VRAM_OFFSET - PALETTE_SIZE * (page + 1)`

### Page Visibility

- **Visibility Control**: Each page can be individually shown/hidden
- **Visibility Bits**: `LAYER_VISIBLE_BITS` (8 bits, one per layer)
- **Layer Order**: Lower-numbered layers are drawn on top
- **Transparency**: Color 0 is transparent, colors 1-255 are opaque

### Buffer Management

- **Double Buffering**: Each page has a back buffer and display buffer
- **Auto Mode**: Automatic buffer updates each frame
- **Manual Mode**: Manual buffer updates via VDL instruction
- **Buffer Control**: `LAYER_BUFFER_MODE_BITS` (8 bits, one per layer)

## Color System

### Palette Format

- **Colors per Palette**: 256 colors
- **Bytes per Color**: 3 bytes (RGB)
- **Total Palette Size**: 768 bytes (256 × 3)
- **Color Index**: 0-255 (0 = transparent, 1-255 = opaque)

### Color Storage

- **Format**: RGB (Red, Green, Blue)
- **Byte Order**: RGB (3 consecutive bytes per color)
- **Color Data**: Stored in palette buffer adjacent to video page

### Color Rendering

- **Transparency**: Color index 0 is transparent
- **Opaque Colors**: Color indices 1-255 are opaque
- **Layer Blending**: Lower layers show through transparent pixels of upper layers
- **Final Color**: Topmost opaque pixel color is displayed

## Video Memory Layout

### Memory Organization

```
High Memory Addresses (0xFFFFFF)
├── Page 7 Palette (768 bytes)
├── Page 7 Video Data (129,600 bytes)
├── Page 6 Palette (768 bytes)
├── Page 6 Video Data (129,600 bytes)
├── ...
├── Page 0 Palette (768 bytes)
└── Page 0 Video Data (129,600 bytes)
Low Memory Addresses
```

### Address Calculation

- **VRAM Offset**: `0x1000000 - V_SIZE * VRAM_PAGES`
- **Page N Address**: `0x1000000 - V_SIZE * (N + 1)`
- **Page N Palette**: `VRAM_OFFSET - 768 * (N + 1)`

## Graphics Operations

### Drawing Operations

The graphics system supports various drawing operations via interrupts:

1. **Plot**: Draw single pixel
2. **Line**: Draw line between two points
3. **Rectangle**: Draw rectangle outline
4. **Filled Rectangle**: Draw filled rectangle
5. **Rounded Rectangle**: Draw rounded rectangle outline
6. **Filled Rounded Rectangle**: Draw filled rounded rectangle
7. **Ellipse**: Draw ellipse outline
8. **Filled Ellipse**: Draw filled ellipse
9. **Line Path**: Draw connected line segments
10. **Bezier Path**: Draw Bezier curve
11. **Perlin Path**: Draw Perlin noise-based path
12. **Area Fill**: Fill area or polygon
13. **Sprite Drawing**: Draw sprites to video page
14. **Tile Map Sprite**: Draw tile map sprites
15. **Text Drawing**: Draw text with font support

### Text Rendering

- **Font Support**: PNG-based fonts
- **Text Metrics**: Get text dimensions
- **Text Rectangle**: Get text bounding box
- **Text Flags**: Monospace, kerning, center, wrap

### Video Operations

- **Clear Page**: Clear video page with color
- **Copy Rectangle**: Copy rectangular region between pages
- **Scroll/Roll**: Scroll or roll page content

## Buffer Management

### Back Buffer System

- **Back Buffer**: Off-screen drawing buffer
- **Display Buffer**: Visible screen buffer
- **Double Buffering**: Prevents flicker during drawing

### Auto Mode

- **Automatic Updates**: Back buffer automatically copied to display each frame
- **Enabled by Default**: All layers start in auto mode
- **Control**: `LAYER_BUFFER_AUTO_MODE[]` array

### Manual Mode

- **Manual Updates**: Back buffer updated only when VDL instruction called
- **Control**: `LAYER_BUFFER_MODE_BITS` bit flags
- **Update**: `ManualUpdateBackBuffer(bitmask)` updates specified layers
- **Clear**: `ManualClearBackBuffer(bitmask)` clears specified layers

## Rendering Pipeline

### Frame Rendering

1. **Update Back Buffers**: Copy RAM video data to back buffers (if auto mode)
2. **Layer Composition**: Composite visible layers (bottom to top)
3. **Transparency Handling**: Process transparent pixels (color 0)
4. **Palette Lookup**: Convert color indices to RGB colors
5. **Color Data Generation**: Create final color array
6. **Texture Update**: Update video projection texture
7. **Window Rendering**: Render texture to window

### Layer Composition

```csharp
for (uint y = 0; y < video_height; y++)
{
    for (uint x = 0; x < video_width; x++)
    {
        // Find topmost opaque pixel
        for (l = 0; l < visibleLayersCount; l++)
        {
            colVal = videoBuffer[(video_size * visibleLayers[l]) + rowOffset + x];
            if (colVal != 0)  // Not transparent
                break;
        }
        
        // Lookup color from palette
        colorData[y * video_width + x] = palette[colVal];
    }
}
```

## Video Instructions

### VCL (Video Clear)

- **Purpose**: Clear video back buffer
- **Operands**: Color value (immediate, register, or memory)
- **Operation**: Fills entire back buffer with specified color

### VDL (Video Display/Update)

- **Purpose**: Update display with back buffer
- **Operands**: Parameter (immediate, register, or memory)
- **Operation**: Swaps or copies back buffer to display buffer

## Video Interrupts

The video system provides extensive interrupt support via INT 0x01. The interrupt format is:

```
INT 0x01, <register>
```

Where the register contains the function ID (8-bit), and parameters start from the next register. See the [Interrupts Documentation](INTERRUPTS.md) for detailed parameter specifications.

### Page Management
- **0x00**: Read video resolution
- **0x01**: Read video pages count
- **0x02**: Set video pages count
- **0x03**: Read video address
- **0x04**: Read video palette address
- **0x05**: Clear video page

### Drawing Primitives
- **0x06**: Draw filled rectangle
- **0x07**: Draw rectangle
- **0x08**: Draw filled rounded rectangle
- **0x09**: Draw rounded rectangle
- **0x0E**: Draw tile map sprite
- **0x10**: Draw sprite
- **0x20**: Plot (draw pixel)
- **0x21**: Line
- **0x22**: Ellipse
- **0x23**: Filled ellipse
- **0x24**: Line path
- **0x25**: Bezier path
- **0x26**: Perlin path
- **0x28**: Area or polygon fill

### Text Rendering
- **0x11**: Get text metrics
- **0x12**: Draw string
- **0x14**: Draw text
- **0x15**: Get text rectangle

### Layer Control
- **0x30**: Read layers visibility
- **0x31**: Set layers visibility
- **0x32**: Read buffer control mode
- **0x33**: Set buffer control mode

### Video Operations
- **0x40**: Scroll/roll
- **0x41**: Copy rectangle

## Performance Considerations

- **Double Buffering**: Prevents screen tearing and flicker
- **Layer Caching**: Visible layers cached for efficient rendering
- **Palette Caching**: Palettes cached in separate buffer
- **Efficient Composition**: Optimized layer composition algorithm
- **Texture Updates**: Direct texture data updates for fast rendering

## Usage Examples

```
; Set video pages
LD A, 0x02        ; Function ID: SetVideoPagesCount
LD B, 3           ; Page count: 3
INT 0x01, A       ; Set video pages count

; Clear video page
LD A, 0x05        ; Function ID: ClearVideoPage
LD B, 0           ; Page number: 0
LD C, 0x85        ; Color: 0x85
INT 0x01, A       ; Clear video page

; Draw operations
LD A, 0x20        ; Function ID: Plot
LD B, 0           ; Video page: 0
LD CD, 100        ; X position: 100
LD EF, 50         ; Y position: 50
LD G, 200         ; Color: 200
INT 0x01, A       ; Plot pixel

LD A, 0x21        ; Function ID: Line
LD B, 0           ; Video page: 0
LD CD, 10         ; X1: 10
LD EF, 20         ; Y1: 20
LD GH, 100        ; X2: 100
LD IJ, 80         ; Y2: 80
LD K, 200         ; Color: 200
INT 0x01, A       ; Draw line

LD A, 0x06        ; Function ID: DrawFilledRectangle
LD B, 0           ; Video page: 0
LD CD, 10         ; X position: 10
LD EF, 20         ; Y position: 20
LD GH, 100        ; Width: 100
LD IJ, 50         ; Height: 50
LD K, 200         ; Color: 200
INT 0x01, A       ; Draw filled rectangle

; Layer control
LD A, 0x31        ; Function ID: SetLayersVisibility
LD B, 0b11111111  ; All layers visible
INT 0x01, A       ; Set layers visibility

; Manual buffer update
VDL 1             ; Update layer 0 (bit 0 set)
VCL 0             ; Clear layer 0 (bit 0 set)
```

## Conclusion

The Continuum 93 video architecture provides a powerful multi-layer graphics system with palette-based rendering, transparency, double buffering, and comprehensive drawing operations. The architecture supports efficient rendering with up to 8 layers, automatic and manual buffer management, and extensive drawing primitives.

