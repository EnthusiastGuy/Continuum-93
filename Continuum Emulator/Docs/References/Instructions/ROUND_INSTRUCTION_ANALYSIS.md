# ROUND Instruction Execution Analysis

## Executive Summary

The ROUND instruction implementation in `ExROUND.cs` provides **2 distinct instruction variants** for rounding floating-point values to the nearest integer. ROUND uses standard rounding (round half to even, also known as banker's rounding).

## File Statistics

- **File**: `Emulator/Execution/ExROUND.cs`
- **Total Lines**: 37 lines
- **Instruction Variants**: 2 unique opcodes
- **Implementation Pattern**: Uses switch statement on secondary opcode

## Addressing Modes

### Float Register Operations

- `ROUND fr` - Round float register to nearest integer (result in same register)
- `ROUND fr, fr` - Round float register to nearest integer (result in different float register)

## Mathematical Operation

ROUND computes the rounded value using `MathF.Round()`:
- **Rounding**: `result = round(value)`
- **Method**: Round half to even (banker's rounding)
- **Float Precision**: Uses single-precision floating-point
- **Result Type**: Float (not integer)

## Implementation Details

### In-Place Operation

For `ROUND fr`:
```csharp
float fRegValue = computer.CPU.FREGS.GetRegister(fRegPointer);
computer.CPU.FREGS.SetRegister(fRegPointer, MathF.Round(fRegValue));
```

### Copy Operation

For `ROUND fr, fr`:
```csharp
float fSecondRegValue = computer.CPU.FREGS.GetRegister(fReg2pointer);
computer.CPU.FREGS.SetRegister(fReg1pointer, MathF.Round(fSecondRegValue));
```

## Rounding Method

ROUND uses "round half to even" (banker's rounding):
- **0.5 rounds to**: Nearest even number
- **Examples**: 
  - 2.5 → 2
  - 3.5 → 4
  - 4.5 → 4
  - 5.5 → 6

## Flag Updates

ROUND does not modify CPU flags.

## Operation Categories Summary

1. **In-Place Operations**: 1 variant
2. **Copy Operations**: 1 variant

## Usage Examples

```
ROUND F0        ; F0 = round(F0)
ROUND F0, F1    ; F0 = round(F1)
```

## Mathematical Properties

- **Rounding**: Nearest integer value
- **Method**: Round half to even (IEEE 754 standard)
- **Bias**: Unbiased rounding (reduces statistical bias)
- **Result**: Float value representing integer

## Typical Use Cases

1. **Value Rounding**: Round floating-point to nearest integer
2. **Display Formatting**: Prepare values for display
3. **Quantization**: Convert continuous to discrete values
4. **Statistical Operations**: Reduce rounding bias

## Comparison with FLOOR/CEIL Instructions

- **ROUND**: Rounds to nearest integer
- **FLOOR**: Rounds down (toward negative infinity)
- **CEIL**: Rounds up (toward positive infinity)
- **ROUND**: Unbiased rounding method

## Conclusion

The ROUND instruction provides efficient rounding capabilities for floating-point values. The banker's rounding method ensures unbiased results for statistical operations.

