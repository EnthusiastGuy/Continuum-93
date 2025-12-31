# RGB2HSL Instruction Execution Analysis

## Executive Summary

The RGB2HSL instruction implementation in `ExC_RGB2HSL.cs` provides **12 distinct instruction variants** for converting RGB color values to HSL (Hue, Saturation, Lightness) color space. RGB2HSL converts 24-bit RGB colors to 32-bit HSL colors.

## File Statistics

- **File**: `Emulator/Execution/ExC_RGB2HSL.cs`
- **Total Lines**: 127 lines
- **Instruction Variants**: 12 unique opcodes
- **Implementation Pattern**: Uses switch statement on secondary opcode

## Addressing Modes

### Color Conversion Operations

All combinations of source and destination addressing:
- `RGB2HSL nnn, rrrr` - Convert immediate RGB to HSL, store in 32-bit register
- `RGB2HSL rrr, rrrr` - Convert RGB from 24-bit register to HSL, store in 32-bit register
- `RGB2HSL nnn, (nnn)` - Convert immediate RGB to HSL, store in memory at absolute address
- `RGB2HSL rrr, (nnn)` - Convert RGB from register to HSL, store in memory at absolute address
- `RGB2HSL nnn, (rrr)` - Convert immediate RGB to HSL, store in memory at register-indirect address
- `RGB2HSL rrr, (rrr)` - Convert RGB from register to HSL, store in memory at register-indirect address
- `RGB2HSL (nnn), rrrr` - Convert RGB from memory at absolute address to HSL, store in register
- `RGB2HSL (rrr), rrrr` - Convert RGB from memory at register-indirect address to HSL, store in register
- `RGB2HSL (nnn), (nnn)` - Convert RGB from memory to HSL, store in memory (both absolute)
- `RGB2HSL (rrr), (nnn)` - Convert RGB from memory to HSL, store in memory (register-indirect to absolute)
- `RGB2HSL (nnn), (rrr)` - Convert RGB from memory to HSL, store in memory (absolute to register-indirect)
- `RGB2HSL (rrr), (rrr)` - Convert RGB from memory to HSL, store in memory (both register-indirect)

## Operation Description

RGB2HSL performs color space conversion:
- **Input**: 24-bit RGB color value
- **Output**: 32-bit HSL color value
- **Conversion**: Uses `ColorConverter.RGBToHSL(value)`
- **Color Space**: RGB (Red, Green, Blue) → HSL (Hue, Saturation, Lightness)

## Implementation Details

### Color Conversion

RGB2HSL uses the color converter:
```csharp
computer.CPU.REGS.Set32BitRegister(destRegIndex, ColorConverter.RGBToHSL(value));
```

The conversion handles:
- **RGB Format**: 24-bit RGB (8 bits per component)
- **HSL Format**: 32-bit HSL (packed format)
- **Color Accuracy**: Accurate color space conversion

## Opcode Encoding

RGB2HSL uses compact encoding:
1. **Primary Opcode**: Opcode for RGB2HSL
2. **Secondary Opcode**: Determines addressing mode combination
3. **Source**: RGB value from immediate, register, or memory
4. **Destination**: HSL value to register or memory

## Flag Updates

RGB2HSL does not modify CPU flags.

## Operation Categories Summary

1. **Register-to-Register Operations**: 2 variants
2. **Register-to-Memory Operations**: 4 variants
3. **Memory-to-Register Operations**: 2 variants
4. **Memory-to-Memory Operations**: 4 variants

## Usage Examples

```
RGB2HSL 0xFF0000, ABCD    ; Convert red (RGB) to HSL, store in ABCD
RGB2HSL XYZ, ABCD         ; Convert RGB in XYZ to HSL, store in ABCD
RGB2HSL 0x00FF00, (0x1000) ; Convert green to HSL, store in memory
RGB2HSL (XYZ), ABCD       ; Convert RGB from memory to HSL, store in ABCD
```

## Color Space Conversion

RGB to HSL conversion:
- **Hue (H)**: Color type (0-360 degrees)
- **Saturation (S)**: Color intensity (0-100%)
- **Lightness (L)**: Brightness (0-100%)
- **Packed Format**: All components in 32-bit value

## Typical Use Cases

1. **Color Manipulation**: Convert for color adjustments
2. **Graphics Processing**: Color space transformations
3. **Image Processing**: Color analysis and manipulation
4. **UI Development**: Color theme management

## Comparison with HSL2RGB Instruction

- **RGB2HSL**: Converts RGB to HSL
- **HSL2RGB**: Converts HSL to RGB
- **RGB2HSL/HSL2RGB**: Complementary color space conversions

## Conclusion

The RGB2HSL instruction provides efficient RGB to HSL color space conversion with comprehensive addressing mode support. The color conversion is essential for graphics and image processing operations in Continuum 93 programs.

