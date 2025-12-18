# MIN Instruction Execution Analysis

## Executive Summary

The MIN instruction implementation in `ExMIN.cs` provides **approximately 18 distinct instruction variants** for finding the minimum value between two operands. MIN supports both integer and floating-point operations across multiple data sizes.

## File Statistics

- **File**: `Emulator/Execution/ExMIN.cs`
- **Total Lines**: 244 lines
- **Instruction Variants**: ~18 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 6 bits of opcode

## Addressing Modes

### 1. Integer Register Operations

- `MIN r, r` - Minimum of two 8-bit registers
- `MIN r, n` - Minimum of 8-bit register and immediate value
- `MIN rr, rr` - Minimum of two 16-bit registers
- `MIN rr, nn` - Minimum of 16-bit register and immediate value
- `MIN rrr, rrr` - Minimum of two 24-bit registers
- `MIN rrr, nnn` - Minimum of 24-bit register and immediate value
- `MIN rrrr, rrrr` - Minimum of two 32-bit registers
- `MIN rrrr, nnnn` - Minimum of 32-bit register and immediate value

### 2. Float Register Operations

- `MIN fr, fr` - Minimum of two float registers
- `MIN fr, n` - Minimum of float register and immediate float value

### 3. Mixed Integer-Float Operations

- `MIN r, fr` - Minimum of 8-bit register and float register (absolute value of float)
- `MIN rr, fr` - Minimum of 16-bit register and float register (absolute value of float)
- `MIN rrr, fr` - Minimum of 24-bit register and float register (absolute value of float)
- `MIN rrrr, fr` - Minimum of 32-bit register and float register (absolute value of float)
- `MIN fr, r` - Minimum of float register and 8-bit register (absolute value of float)
- `MIN fr, rr` - Minimum of float register and 16-bit register (absolute value of float)
- `MIN fr, rrr` - Minimum of float register and 24-bit register (absolute value of float)
- `MIN fr, rrrr` - Minimum of float register and 32-bit register (absolute value of float)

## Mathematical Operation

MIN computes the minimum using `Math.Min()` or `MathF.Min()`:
- **Minimum**: `result = min(val1, val2)`
- **Integer Operations**: Uses `Math.Min()` for integer types
- **Float Operations**: Uses `MathF.Min()` for float types
- **Mixed Operations**: Converts float to absolute value before comparison

## Implementation Details

### Integer Minimum

For integer operations:
```csharp
byte reg1Val = computer.CPU.REGS.Get8BitRegister(reg1Index);
byte reg2Val = computer.CPU.REGS.Get8BitRegister(reg2Index);
computer.CPU.REGS.Set8BitRegister(reg1Index, Math.Min(reg1Val, reg2Val));
```

### Float Minimum

For float operations:
```csharp
float fReg1Val = computer.CPU.FREGS.GetRegister(fReg1Index);
float fReg2Val = computer.CPU.FREGS.GetRegister(fReg2Index);
computer.CPU.FREGS.SetRegister(fReg1Index, MathF.Min(fReg1Val, fReg2Val));
```

### Mixed Operations

For mixed integer-float operations, the float value is converted to absolute value:
```csharp
float fReg2Val = Math.Abs(computer.CPU.FREGS.GetRegister(fReg2Index));
computer.CPU.REGS.Set8BitRegister(r1Index, (byte)MathF.Min(fReg1Val, fReg2Val));
```

## Flag Updates

MIN does not modify CPU flags.

## Operation Categories Summary

1. **Integer Operations**: 8 variants (4 data sizes × 2 operand types)
2. **Float Operations**: 2 variants
3. **Mixed Operations**: 8 variants (4 data sizes × 2 directions)

## Usage Examples

```
MIN A, B        ; A = min(A, B)
MIN A, 10       ; A = min(A, 10)
MIN F0, F1      ; F0 = min(F0, F1)
MIN A, F0       ; A = min(A, |F0|)
```

## Mathematical Properties

- **Commutative**: `min(a, b) = min(b, a)`
- **Associative**: `min(a, min(b, c)) = min(min(a, b), c)`
- **Idempotent**: `min(a, a) = a`
- **Identity**: `min(a, ∞) = a`

## Typical Use Cases

1. **Range Clamping**: Clamp values to maximum
2. **Comparison**: Find smaller of two values
3. **Bounds Checking**: Ensure values don't exceed limits
4. **Optimization**: Find minimum in algorithms

## Comparison with MAX Instruction

- **MIN**: Returns smaller value
- **MAX**: Returns larger value
- **MIN/MAX**: Complementary operations

## Conclusion

The MIN instruction provides efficient minimum value calculation for both integer and floating-point operations. The support for mixed operations enables flexible value comparisons across data types.

