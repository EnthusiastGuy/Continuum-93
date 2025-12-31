# FLOOR Instruction Execution Analysis

## Executive Summary

The FLOOR instruction implementation in `ExFLOOR.cs` provides **2 distinct instruction variants** for rounding floating-point values down to the nearest integer. FLOOR rounds toward negative infinity.

## File Statistics

- **File**: `Emulator/Execution/ExFLOOR.cs`
- **Total Lines**: 37 lines
- **Instruction Variants**: 2 unique opcodes
- **Implementation Pattern**: Uses switch statement on secondary opcode

## Addressing Modes

### Float Register Operations

- `FLOOR fr` - Floor float register to nearest lower integer (result in same register)
- `FLOOR fr, fr` - Floor float register to nearest lower integer (result in different float register)

## Mathematical Operation

FLOOR computes the floor value using `MathF.Floor()`:
- **Floor Function**: `result = floor(value)` = largest integer ≤ value
- **Rounding Direction**: Toward negative infinity
- **Float Precision**: Uses single-precision floating-point
- **Result Type**: Float (not integer)

## Implementation Details

### In-Place Operation

For `FLOOR fr`:
```csharp
float fRegValue = computer.CPU.FREGS.GetRegister(fRegPointer);
computer.CPU.FREGS.SetRegister(fRegPointer, MathF.Floor(fRegValue));
```

### Copy Operation

For `FLOOR fr, fr`:
```csharp
float fSecondRegValue = computer.CPU.FREGS.GetRegister(fReg2pointer);
computer.CPU.FREGS.SetRegister(fReg1pointer, MathF.Floor(fSecondRegValue));
```

## Flag Updates

FLOOR does not modify CPU flags.

## Operation Categories Summary

1. **In-Place Operations**: 1 variant
2. **Copy Operations**: 1 variant

## Usage Examples

```
FLOOR F0        ; F0 = floor(F0)
FLOOR F0, F1    ; F0 = floor(F1)
```

## Mathematical Properties

- **Floor Function**: Always rounds down
- **Examples**: 
  - 3.7 → 3
  - 3.2 → 3
  - -3.2 → -4
  - -3.7 → -4
- **Result**: Largest integer less than or equal to input

## Typical Use Cases

1. **Value Truncation**: Round down to integer
2. **Integer Division**: Implement integer division
3. **Grid Alignment**: Align to grid boundaries
4. **Mathematical Operations**: Floor division operations

## Comparison with ROUND/CEIL Instructions

- **FLOOR**: Rounds down (toward negative infinity)
- **ROUND**: Rounds to nearest integer
- **CEIL**: Rounds up (toward positive infinity)
- **FLOOR**: Always decreases value (except for integers)

## Conclusion

The FLOOR instruction provides efficient floor calculation for floating-point values. The downward rounding is essential for integer division and grid alignment operations.

