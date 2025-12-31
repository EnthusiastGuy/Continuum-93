# SQR Instruction Execution Analysis

## Executive Summary

The SQR (Square Root) instruction implementation in `ExSQR.cs` provides **approximately 20 distinct instruction variants** for computing square roots of values. SQR supports both integer and floating-point operations, with conversion between data types as needed.

## File Statistics

- **File**: `Emulator/Execution/ExSQR.cs`
- **Total Lines**: 286 lines
- **Instruction Variants**: ~20 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 6 bits of opcode

## Addressing Modes

### 1. Float Register Operations

- `SQR fr` - Square root of float register (result in same register)
- `SQR fr, fr` - Square root of float register (result in different float register)
- `SQR fr, n` - Square root of immediate float value
- `SQR fr, r` - Square root of 8-bit integer register (converted to float)
- `SQR fr, rr` - Square root of 16-bit integer register
- `SQR fr, rrr` - Square root of 24-bit integer register
- `SQR fr, rrrr` - Square root of 32-bit integer register
- `SQR fr, (nnn)` - Square root of float value from absolute address
- `SQR fr, (rrr)` - Square root of float value from register-indirect address

### 2. Integer Register Operations

- `SQR r` - Square root of 8-bit register (result truncated to 8-bit)
- `SQR rr` - Square root of 16-bit register (result truncated to 16-bit)
- `SQR rrr` - Square root of 24-bit register (result truncated to 24-bit)
- `SQR rrrr` - Square root of 32-bit register (result truncated to 32-bit)

### 3. Memory Operations

- `SQR (nnn)` - Square root of float value at absolute address
- `SQR (rrr)` - Square root of float value at register-indirect address

### 4. Integer-to-Float Operations

- `SQR r, fr` - Square root of float register, result to 8-bit register (with overflow check)
- `SQR rr, fr` - Square root of float register, result to 16-bit register (with overflow check)
- `SQR rrr, fr` - Square root of float register, result to 24-bit register (with overflow check)
- `SQR rrrr, fr` - Square root of float register, result to 32-bit register (with overflow check)

### 5. Memory-to-Float Operations

- `SQR (rrr), fr` - Square root of float register, result to memory at register-indirect address
- `SQR (nnn), fr` - Square root of float register, result to memory at absolute address

## Mathematical Operation

SQR computes the square root using `MathF.Sqrt()` or `Math.Sqrt()`:
- **Square Root**: `result = √value`
- **Float Precision**: Uses single-precision floating-point for calculations
- **Integer Conversion**: Results are truncated when stored in integer registers

## Overflow Detection

For integer register results, SQR checks for overflow:
```csharp
float result = MathF.Sqrt(fRegVal);
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

SQR updates CPU flags:
- **Overflow Flag**: Set when result exceeds destination register size
- **Other Flags**: May be updated by register operations

## Implementation Details

### Square Root Calculation

Uses .NET Math functions:
- `MathF.Sqrt(float)` - For single-precision float operations
- `Math.Sqrt(double)` - For double-precision operations (32-bit integer to float)

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
SQR F0          ; F0 = √F0
SQR F0, F1      ; F0 = √F1
SQR A           ; A = √A (truncated)
SQR F0, 16.0    ; F0 = √16.0 = 4.0
SQR A, F0       ; A = √F0 (with overflow check)
```

## Mathematical Properties

- **Square Root**: Inverse operation of squaring
- **Domain**: Non-negative numbers (0 and positive)
- **Range**: Non-negative numbers
- **Precision**: Single-precision floating-point

## Conclusion

The SQR instruction provides comprehensive square root calculation capabilities with support for both integer and floating-point operations. The overflow detection ensures safe conversion from float to integer registers.

