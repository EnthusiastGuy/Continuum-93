# HSB2RGB Instruction Execution Analysis

## Executive Summary

The HSB2RGB instruction implementation in `ExC_HSB2RGB.cs` provides **12 distinct instruction variants** for converting HSB color values to RGB (Red, Green, Blue) color space. HSB2RGB converts 32-bit HSB colors to 24-bit RGB colors.

## File Statistics

- **File**: `Emulator/Execution/ExC_HSB2RGB.cs`
- **Total Lines**: 127 lines
- **Instruction Variants**: 12 unique opcodes
- **Implementation Pattern**: Uses switch statement on secondary opcode

## Addressing Modes

### Color Conversion Operations

All combinations of source and destination addressing (same as HSL2RGB):
- `HSB2RGB nnnn, rrr` - Convert immediate HSB to RGB, store in 24-bit register
- `HSB2RGB rrrr, rrr` - Convert HSB from 32-bit register to RGB, store in 24-bit register
- `HSB2RGB nnnn, (nnn)` - Convert immediate HSB to RGB, store in memory at absolute address
- `HSB2RGB rrrr, (nnn)` - Convert HSB from register to RGB, store in memory at absolute address
- `HSB2RGB nnnn, (rrr)` - Convert immediate HSB to RGB, store in memory at register-indirect address
- `HSB2RGB rrrr, (rrr)` - Convert HSB from register to RGB, store in memory at register-indirect address
- `HSB2RGB (nnn), rrr` - Convert HSB from memory at absolute address to RGB, store in register
- `HSB2RGB (rrr), rrr` - Convert HSB from memory at register-indirect address to RGB, store in register
- `HSB2RGB (nnn), (nnn)` - Convert HSB from memory to RGB, store in memory (both absolute)
- `HSB2RGB (rrr), (nnn)` - Convert HSB from memory to RGB, store in memory (register-indirect to absolute)
- `HSB2RGB (nnn), (rrr)` - Convert HSB from memory to RGB, store in memory (absolute to register-indirect)
- `HSB2RGB (rrr), (rrr)` - Convert HSB from memory to RGB, store in memory (both register-indirect)

## Operation Description

HSB2RGB performs color space conversion:
- **Input**: 32-bit HSB color value
- **Output**: 24-bit RGB color value
- **Conversion**: Uses `ColorConverter.HSBToRGB(value)`
- **Color Space**: HSB (Hue, Saturation, Brightness) → RGB (Red, Green, Blue)

## Implementation Details

### Color Conversion

HSB2RGB uses the color converter:
```csharp
computer.CPU.REGS.Set24BitRegister(destRegIndex, ColorConverter.HSBToRGB(value));
```

The conversion handles:
- **HSB Format**: 32-bit HSB (packed format)
- **RGB Format**: 24-bit RGB (8 bits per component)
- **Color Accuracy**: Accurate color space conversion

## Color Space Conversion

HSB to RGB conversion:
- **Hue (H)**: Color type (0-360 degrees)
- **Saturation (S)**: Color intensity (0-100%)
- **Brightness (B)**: Light intensity (0-100%)
- **RGB Output**: 24-bit RGB (8 bits per component)

## Comparison with HSL Color Space

- **HSB**: Uses Brightness (B)
- **HSL**: Uses Lightness (L)
- **HSB**: More intuitive for some applications
- **HSL**: More perceptually uniform

## Conclusion

The HSB2RGB instruction provides efficient HSB to RGB color space conversion with comprehensive addressing mode support. The color conversion is essential for graphics and image processing operations in Continuum 93 programs.

