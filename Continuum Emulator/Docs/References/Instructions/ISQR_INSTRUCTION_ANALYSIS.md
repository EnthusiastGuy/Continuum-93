# ISQR Instruction Execution Analysis

## Executive Summary

The ISQR (Inverse Square Root) instruction implementation in `ExISQR.cs` provides **approximately 20 distinct instruction variants** for computing the inverse square root (1/√x) of values. ISQR supports both integer and floating-point operations, with conversion between data types as needed.

## File Statistics

- **File**: `Emulator/Execution/ExISQR.cs`
- **Total Lines**: 285 lines
- **Instruction Variants**: ~20 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 6 bits of opcode

## Addressing Modes

### 1. Float Register Operations

- `ISQR fr` - Inverse square root of float register (result in same register)
- `ISQR fr, fr` - Inverse square root of float register (result in different float register)
- `ISQR fr, n` - Inverse square root of immediate float value
- `ISQR fr, r` - Inverse square root of 8-bit integer register (converted to float)
- `ISQR fr, rr` - Inverse square root of 16-bit integer register
- `ISQR fr, rrr` - Inverse square root of 24-bit integer register
- `ISQR fr, rrrr` - Inverse square root of 32-bit integer register
- `ISQR fr, (nnn)` - Inverse square root of float value from absolute address
- `ISQR fr, (rrr)` - Inverse square root of float value from register-indirect address

### 2. Integer Register Operations

- `ISQR r` - Inverse square root of 8-bit register (result truncated to 8-bit)
- `ISQR rr` - Inverse square root of 16-bit register (result truncated to 16-bit)
- `ISQR rrr` - Inverse square root of 24-bit register (result truncated to 24-bit)
- `ISQR rrrr` - Inverse square root of 32-bit register (result truncated to 32-bit)

### 3. Memory Operations

- `ISQR (nnn)` - Inverse square root of float value at absolute address
- `ISQR (rrr)` - Inverse square root of float value at register-indirect address

### 4. Integer-to-Float Operations

- `ISQR r, fr` - Inverse square root of float register, result to 8-bit register (with overflow check)
- `ISQR rr, fr` - Inverse square root of float register, result to 16-bit register (with overflow check)
- `ISQR rrr, fr` - Inverse square root of float register, result to 24-bit register (with overflow check)
- `ISQR rrrr, fr` - Inverse square root of float register, result to 32-bit register (with overflow check)

### 5. Memory-to-Float Operations

- `ISQR (rrr), fr` - Inverse square root of float register, result to memory at register-indirect address
- `ISQR (nnn), fr` - Inverse square root of float register, result to memory at absolute address

## Mathematical Operation

ISQR computes the inverse square root using `CMath.InverseSqrt()`:
- **Inverse Square Root**: `result = 1/√value`
- **Float Precision**: Uses single-precision floating-point for calculations
- **Integer Conversion**: Results are truncated when stored in integer registers

## Overflow Detection

For integer register results, ISQR checks for overflow:
```csharp
float result = CMath.InverseSqrt(fRegVal);
if (result < 0x100)
{
    computer.CPU.REGS.Set8BitRegister(r1Index, (byte)result);
    computer.CPU.FLAGS.SetOverflow(false);
}
else
{
    computer.CPU.FLAGS.SetOverflow(true);
}
```

Overflow thresholds:
- **8-bit**: 0x100 (256)
- **16-bit**: 0x10000 (65536)
- **24-bit**: 0x1000000 (16777216)
- **32-bit**: 0x100000000 (4294967296)

## Flag Updates

ISQR updates CPU flags:
- **Overflow Flag**: Set when result exceeds destination register size
- **Other Flags**: May be updated by register operations

## Implementation Details

### Inverse Square Root Calculation

Uses custom `CMath.InverseSqrt()` function:
- Optimized implementation for inverse square root
- Faster than computing `1.0 / MathF.Sqrt(value)`
- Commonly used in graphics and physics calculations

### Type Conversions

- **Integer to Float**: Automatic conversion for integer source operands
- **Float to Integer**: Truncation when storing in integer registers
- **Float Operations**: Maintains float precision for float-to-float operations

## Operation Categories Summary

1. **Float Register Operations**: ~9 variants
2. **Integer Register Operations**: 4 variants
3. **Memory Operations**: 2 variants
4. **Integer-to-Float Operations**: 4 variants
5. **Memory-to-Float Operations**: 2 variants

## Usage Examples

```
ISQR F0         ; F0 = 1/√F0
ISQR F0, F1     ; F0 = 1/√F1
ISQR A          ; A = 1/√A (truncated)
ISQR F0, 4.0    ; F0 = 1/√4.0 = 0.5
ISQR A, F0      ; A = 1/√F0 (with overflow check)
```

## Mathematical Properties

- **Inverse Square Root**: 1 divided by square root
- **Domain**: Positive numbers (0 and positive)
- **Range**: Positive numbers
- **Precision**: Single-precision floating-point
- **Optimization**: Fast inverse square root algorithm

## Typical Use Cases

1. **Graphics**: Normalization of vectors (faster than division)
2. **Physics**: Distance calculations, normalization
3. **Game Development**: Vector operations, lighting calculations
4. **Performance**: Optimized alternative to 1/√x

## Comparison with SQR Instruction

- **ISQR**: Computes 1/√x (inverse square root)
- **SQR**: Computes √x (square root)
- **ISQR**: Optimized for vector normalization
- **SQR**: Standard square root operation

## Conclusion

The ISQR instruction provides efficient inverse square root calculation capabilities with support for both integer and floating-point operations. The optimized implementation makes it ideal for graphics and physics calculations requiring vector normalization.

