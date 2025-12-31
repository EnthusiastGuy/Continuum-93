# HSL2RGB Instruction Execution Analysis

## Executive Summary

The HSL2RGB instruction implementation in `ExC_HSL2RGB.cs` provides **12 distinct instruction variants** for converting HSL color values to RGB (Red, Green, Blue) color space. HSL2RGB converts 32-bit HSL colors to 24-bit RGB colors.

## File Statistics

- **File**: `Emulator/Execution/ExC_HSL2RGB.cs`
- **Total Lines**: 127 lines
- **Instruction Variants**: 12 unique opcodes
- **Implementation Pattern**: Uses switch statement on secondary opcode

## Addressing Modes

### Color Conversion Operations

All combinations of source and destination addressing:
- `HSL2RGB nnnn, rrr` - Convert immediate HSL to RGB, store in 24-bit register
- `HSL2RGB rrrr, rrr` - Convert HSL from 32-bit register to RGB, store in 24-bit register
- `HSL2RGB nnnn, (nnn)` - Convert immediate HSL to RGB, store in memory at absolute address
- `HSL2RGB rrrr, (nnn)` - Convert HSL from register to RGB, store in memory at absolute address
- `HSL2RGB nnnn, (rrr)` - Convert immediate HSL to RGB, store in memory at register-indirect address
- `HSL2RGB rrrr, (rrr)` - Convert HSL from register to RGB, store in memory at register-indirect address
- `HSL2RGB (nnn), rrr` - Convert HSL from memory at absolute address to RGB, store in register
- `HSL2RGB (rrr), rrr` - Convert HSL from memory at register-indirect address to RGB, store in register
- `HSL2RGB (nnn), (nnn)` - Convert HSL from memory to RGB, store in memory (both absolute)
- `HSL2RGB (rrr), (nnn)` - Convert HSL from memory to RGB, store in memory (register-indirect to absolute)
- `HSL2RGB (nnn), (rrr)` - Convert HSL from memory to RGB, store in memory (absolute to register-indirect)
- `HSL2RGB (rrr), (rrr)` - Convert HSL from memory to RGB, store in memory (both register-indirect)

## Operation Description

HSL2RGB performs color space conversion:
- **Input**: 32-bit HSL color value
- **Output**: 24-bit RGB color value
- **Conversion**: Uses `ColorConverter.HSLToRGB(value)`
- **Color Space**: HSL (Hue, Saturation, Lightness) → RGB (Red, Green, Blue)

## Implementation Details

### Color Conversion

HSL2RGB uses the color converter:
```csharp
computer.CPU.REGS.Set24BitRegister(destRegIndex, ColorConverter.HSLToRGB(value));
```

The conversion handles:
- **HSL Format**: 32-bit HSL (packed format)
- **RGB Format**: 24-bit RGB (8 bits per component)
- **Color Accuracy**: Accurate color space conversion

## Opcode Encoding

HSL2RGB uses compact encoding:
1. **Primary Opcode**: Opcode for HSL2RGB
2. **Secondary Opcode**: Determines addressing mode combination
3. **Source**: HSL value from immediate, register, or memory
4. **Destination**: RGB value to register or memory

## Flag Updates

HSL2RGB does not modify CPU flags.

## Operation Categories Summary

1. **Register-to-Register Operations**: 2 variants
2. **Register-to-Memory Operations**: 4 variants
3. **Memory-to-Register Operations**: 2 variants
4. **Memory-to-Memory Operations**: 4 variants

## Usage Examples

```
HSL2RGB 0x00FF80, XYZ    ; Convert HSL to RGB, store in XYZ
HSL2RGB ABCD, XYZ        ; Convert HSL in ABCD to RGB, store in XYZ
HSL2RGB 0x00FF80, (0x1000) ; Convert HSL to RGB, store in memory
HSL2RGB (ABCD), XYZ      ; Convert HSL from memory to RGB, store in XYZ
```

## Color Space Conversion

HSL to RGB conversion:
- **Hue (H)**: Color type (0-360 degrees)
- **Saturation (S)**: Color intensity (0-100%)
- **Lightness (L)**: Brightness (0-100%)
- **RGB Output**: 24-bit RGB (8 bits per component)

## Typical Use Cases

1. **Color Manipulation**: Convert for color adjustments
2. **Graphics Processing**: Color space transformations
3. **Image Processing**: Color rendering and display
4. **UI Development**: Color theme application

## Comparison with RGB2HSL Instruction

- **HSL2RGB**: Converts HSL to RGB
- **RGB2HSL**: Converts RGB to HSL
- **HSL2RGB/RGB2HSL**: Complementary color space conversions

## Conclusion

The HSL2RGB instruction provides efficient HSL to RGB color space conversion with comprehensive addressing mode support. The color conversion is essential for graphics and image processing operations in Continuum 93 programs.

