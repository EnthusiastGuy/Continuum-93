# CEIL Instruction Execution Analysis

## Executive Summary

The CEIL (Ceiling) instruction implementation in `ExCEIL.cs` provides **2 distinct instruction variants** for rounding floating-point values up to the nearest integer. CEIL rounds toward positive infinity.

## File Statistics

- **File**: `Emulator/Execution/ExCEIL.cs`
- **Total Lines**: 37 lines
- **Instruction Variants**: 2 unique opcodes
- **Implementation Pattern**: Uses switch statement on secondary opcode

## Addressing Modes

### Float Register Operations

- `CEIL fr` - Ceiling float register to nearest higher integer (result in same register)
- `CEIL fr, fr` - Ceiling float register to nearest higher integer (result in different float register)

## Mathematical Operation

CEIL computes the ceiling value using `MathF.Ceiling()`:
- **Ceiling Function**: `result = ceil(value)` = smallest integer ≥ value
- **Rounding Direction**: Toward positive infinity
- **Float Precision**: Uses single-precision floating-point
- **Result Type**: Float (not integer)

## Implementation Details

### In-Place Operation

For `CEIL fr`:
```csharp
float fRegValue = computer.CPU.FREGS.GetRegister(fRegPointer);
computer.CPU.FREGS.SetRegister(fRegPointer, MathF.Ceiling(fRegValue));
```

### Copy Operation

For `CEIL fr, fr`:
```csharp
float fSecondRegValue = computer.CPU.FREGS.GetRegister(fReg2pointer);
computer.CPU.FREGS.SetRegister(fReg1pointer, MathF.Ceiling(fSecondRegValue));
```

## Flag Updates

CEIL does not modify CPU flags.

## Operation Categories Summary

1. **In-Place Operations**: 1 variant
2. **Copy Operations**: 1 variant

## Usage Examples

```
CEIL F0         ; F0 = ceil(F0)
CEIL F0, F1     ; F0 = ceil(F1)
```

## Mathematical Properties

- **Ceiling Function**: Always rounds up
- **Examples**: 
  - 3.2 → 4
  - 3.7 → 4
  - -3.2 → -3
  - -3.7 → -3
- **Result**: Smallest integer greater than or equal to input

## Typical Use Cases

1. **Value Rounding Up**: Round up to integer
2. **Pagination**: Calculate number of pages needed
3. **Resource Allocation**: Allocate sufficient resources
4. **Mathematical Operations**: Ceiling division operations

## Comparison with ROUND/FLOOR Instructions

- **CEIL**: Rounds up (toward positive infinity)
- **ROUND**: Rounds to nearest integer
- **FLOOR**: Rounds down (toward negative infinity)
- **CEIL**: Always increases value (except for integers)

## Conclusion

The CEIL instruction provides efficient ceiling calculation for floating-point values. The upward rounding is essential for resource allocation and pagination operations.

