# CBR Instruction Execution Analysis

## Executive Summary

The CBR (Cube Root) instruction implementation in `ExCBR.cs` provides **approximately 20 distinct instruction variants** for computing cube roots of values. CBR supports both integer and floating-point operations, with conversion between data types as needed.

## File Statistics

- **File**: `Emulator/Execution/ExCBR.cs`
- **Total Lines**: 286 lines
- **Instruction Variants**: ~20 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 6 bits of opcode

## Addressing Modes

### 1. Float Register Operations

- `CBR fr` - Cube root of float register (result in same register)
- `CBR fr, fr` - Cube root of float register (result in different float register)
- `CBR fr, n` - Cube root of immediate float value
- `CBR fr, r` - Cube root of 8-bit integer register (converted to float)
- `CBR fr, rr` - Cube root of 16-bit integer register
- `CBR fr, rrr` - Cube root of 24-bit integer register
- `CBR fr, rrrr` - Cube root of 32-bit integer register
- `CBR fr, (nnn)` - Cube root of float value from absolute address
- `CBR fr, (rrr)` - Cube root of float value from register-indirect address

### 2. Integer Register Operations

- `CBR r` - Cube root of 8-bit register (result truncated to 8-bit)
- `CBR rr` - Cube root of 16-bit register (result truncated to 16-bit)
- `CBR rrr` - Cube root of 24-bit register (result truncated to 24-bit)
- `CBR rrrr` - Cube root of 32-bit register (result truncated to 32-bit)

### 3. Memory Operations

- `CBR (nnn)` - Cube root of float value at absolute address
- `CBR (rrr)` - Cube root of float value at register-indirect address

### 4. Integer-to-Float Operations

- `CBR r, fr` - Cube root of float register, result to 8-bit register (with overflow check)
- `CBR rr, fr` - Cube root of float register, result to 16-bit register (with overflow check)
- `CBR rrr, fr` - Cube root of float register, result to 24-bit register (with overflow check)
- `CBR rrrr, fr` - Cube root of float register, result to 32-bit register (with overflow check)

### 5. Memory-to-Float Operations

- `CBR (rrr), fr` - Cube root of float register, result to memory at register-indirect address
- `CBR (nnn), fr` - Cube root of float register, result to memory at absolute address

## Mathematical Operation

CBR computes the cube root using `MathF.Cbrt()` or `Math.Cbrt()`:
- **Cube Root**: `result = ∛value`
- **Float Precision**: Uses single-precision floating-point for calculations
- **Integer Conversion**: Results are truncated when stored in integer registers

## Overflow Detection

For integer register results, CBR checks for overflow:
```csharp
float result = MathF.Cbrt(fRegVal);
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

CBR updates CPU flags:
- **Overflow Flag**: Set when result exceeds destination register size
- **Other Flags**: May be updated by register operations

## Implementation Details

### Cube Root Calculation

Uses .NET Math functions:
- `MathF.Cbrt(float)` - For single-precision float operations
- `Math.Cbrt(double)` - For double-precision operations (32-bit integer to float)

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
CBR F0          ; F0 = ∛F0
CBR F0, F1      ; F0 = ∛F1
CBR A           ; A = ∛A (truncated)
CBR F0, 27.0    ; F0 = ∛27.0 = 3.0
CBR A, F0       ; A = ∛F0 (with overflow check)
```

## Mathematical Properties

- **Cube Root**: Inverse operation of cubing
- **Domain**: All real numbers (including negatives)
- **Range**: All real numbers
- **Precision**: Single-precision floating-point

## Conclusion

The CBR instruction provides comprehensive cube root calculation capabilities with support for both integer and floating-point operations. The overflow detection ensures safe conversion from float to integer registers.

