# RGB2HSB Instruction Execution Analysis

## Executive Summary

The RGB2HSB instruction implementation in `ExC_RGB2HSB.cs` provides **12 distinct instruction variants** for converting RGB color values to HSB (Hue, Saturation, Brightness) color space. RGB2HSB converts 24-bit RGB colors to 32-bit HSB colors.

## File Statistics

- **File**: `Emulator/Execution/ExC_RGB2HSB.cs`
- **Total Lines**: 127 lines
- **Instruction Variants**: 12 unique opcodes
- **Implementation Pattern**: Uses switch statement on secondary opcode

## Addressing Modes

### Color Conversion Operations

All combinations of source and destination addressing (same as RGB2HSL):
- `RGB2HSB nnn, rrrr` - Convert immediate RGB to HSB, store in 32-bit register
- `RGB2HSB rrr, rrrr` - Convert RGB from 24-bit register to HSB, store in 32-bit register
- `RGB2HSB nnn, (nnn)` - Convert immediate RGB to HSB, store in memory at absolute address
- `RGB2HSB rrr, (nnn)` - Convert RGB from register to HSB, store in memory at absolute address
- `RGB2HSB nnn, (rrr)` - Convert immediate RGB to HSB, store in memory at register-indirect address
- `RGB2HSB rrr, (rrr)` - Convert RGB from register to HSB, store in memory at register-indirect address
- `RGB2HSB (nnn), rrrr` - Convert RGB from memory at absolute address to HSB, store in register
- `RGB2HSB (rrr), rrrr` - Convert RGB from memory at register-indirect address to HSB, store in register
- `RGB2HSB (nnn), (nnn)` - Convert RGB from memory to HSB, store in memory (both absolute)
- `RGB2HSB (rrr), (nnn)` - Convert RGB from memory to HSB, store in memory (register-indirect to absolute)
- `RGB2HSB (nnn), (rrr)` - Convert RGB from memory to HSB, store in memory (absolute to register-indirect)
- `RGB2HSB (rrr), (rrr)` - Convert RGB from memory to HSB, store in memory (both register-indirect)

## Operation Description

RGB2HSB performs color space conversion:
- **Input**: 24-bit RGB color value
- **Output**: 32-bit HSB color value
- **Conversion**: Uses `ColorConverter.RGBToHSB(value)`
- **Color Space**: RGB (Red, Green, Blue) → HSB (Hue, Saturation, Brightness)

## Implementation Details

### Color Conversion

RGB2HSB uses the color converter:
```csharp
computer.CPU.REGS.Set32BitRegister(destRegIndex, ColorConverter.RGBToHSB(value));
```

The conversion handles:
- **RGB Format**: 24-bit RGB (8 bits per component)
- **HSB Format**: 32-bit HSB (packed format)
- **Color Accuracy**: Accurate color space conversion

## Color Space Conversion

RGB to HSB conversion:
- **Hue (H)**: Color type (0-360 degrees)
- **Saturation (S)**: Color intensity (0-100%)
- **Brightness (B)**: Light intensity (0-100%)
- **Packed Format**: All components in 32-bit value

## Comparison with HSL Color Space

- **HSB**: Uses Brightness (B)
- **HSL**: Uses Lightness (L)
- **HSB**: More intuitive for some applications
- **HSL**: More perceptually uniform

## Conclusion

The RGB2HSB instruction provides efficient RGB to HSB color space conversion with comprehensive addressing mode support. The color conversion is essential for graphics and image processing operations in Continuum 93 programs.

