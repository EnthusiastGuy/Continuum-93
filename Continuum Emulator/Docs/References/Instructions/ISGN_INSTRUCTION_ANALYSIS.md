# ISGN Instruction Execution Analysis

## Executive Summary

The ISGN (Invert Sign) instruction implementation in `ExISGN.cs` provides **2 distinct instruction variants** for negating floating-point values. ISGN changes the sign of a float value (multiplies by -1).

## File Statistics

- **File**: `Emulator/Execution/ExISGN.cs`
- **Total Lines**: 38 lines
- **Instruction Variants**: 2 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 4 bits of opcode

## Addressing Modes

### Float Register Operations

- `ISGN fr` - Negate float register (result in same register)
- `ISGN fr, fr` - Negate float register (result in different float register)

## Mathematical Operation

ISGN computes the negation:
- **Negation**: `result = -value`
- **Sign Change**: Positive becomes negative, negative becomes positive
- **Float Precision**: Uses single-precision floating-point

## Implementation Details

### In-Place Operation

For `ISGN fr`:
```csharp
float fRegValue = computer.CPU.FREGS.GetRegister(fRegPointer);
computer.CPU.FREGS.SetRegister(fRegPointer, -fRegValue);
```

### Copy Operation

For `ISGN fr, fr`:
```csharp
float fSecondRegValue = computer.CPU.FREGS.GetRegister(fReg2);
computer.CPU.FREGS.SetRegister(fReg1, -fSecondRegValue);
```

## Flag Updates

ISGN does not modify CPU flags.

## Operation Categories Summary

1. **In-Place Operations**: 1 variant
2. **Copy Operations**: 1 variant

## Usage Examples

```
ISGN F0         ; F0 = -F0
ISGN F0, F1     ; F0 = -F1
```

## Mathematical Properties

- **Self-Inverse**: `-(-x) = x`
- **Negation**: Multiplies by -1
- **Zero**: `-0 = 0`
- **Sign Change**: Flips sign bit

## Typical Use Cases

1. **Sign Inversion**: Change sign of values
2. **Direction Reversal**: Reverse direction in calculations
3. **Subtraction Alternative**: Use addition with negated value
4. **Mathematical Operations**: Negation operations

## Comparison with Other Instructions

- **ISGN**: Negates float value (unary operation)
- **SUB**: Subtracts values (binary operation)
- **ISGN**: `-value`
- **SUB**: `value1 - value2`

## Conclusion

The ISGN instruction provides efficient sign inversion for floating-point values. The simple negation operation is essential for mathematical calculations requiring sign changes.

