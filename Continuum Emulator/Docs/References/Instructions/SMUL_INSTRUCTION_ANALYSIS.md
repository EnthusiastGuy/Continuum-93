# SMUL Instruction Execution Analysis

## Executive Summary

The SMUL (Signed Multiply) instruction implementation in `ExSMUL.cs` provides **approximately 12 distinct instruction variants** for signed multiplication operations across multiple data sizes (8, 16, 24, 32 bits). SMUL performs signed integer multiplication, properly handling negative numbers.

## File Statistics

- **File**: `Emulator/Execution/ExSMUL.cs`
- **Total Lines**: 316 lines
- **Instruction Variants**: ~12 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 6 bits of opcode

## Addressing Modes

### Register Multiplication Operations

#### 8-bit Operations
- `SMUL r, n` - Signed multiply 8-bit register by signed immediate value
- `SMUL r, r` - Signed multiply 8-bit register by signed 8-bit register

#### 16-bit Operations
- `SMUL rr, n` - Signed multiply 16-bit register by signed immediate value
- `SMUL rr, r` - Signed multiply 16-bit register by signed 8-bit register
- `SMUL rr, rr` - Signed multiply 16-bit register by signed 16-bit register

#### 24-bit Operations
- `SMUL rrr, n` - Signed multiply 24-bit register by signed immediate value
- `SMUL rrr, r` - Signed multiply 24-bit register by signed 8-bit register
- `SMUL rrr, rr` - Signed multiply 24-bit register by signed 16-bit register
- `SMUL rrr, rrr` - Signed multiply 24-bit register by signed 24-bit register

#### 32-bit Operations
- `SMUL rrrr, n` - Signed multiply 32-bit register by signed immediate value
- `SMUL rrrr, r` - Signed multiply 32-bit register by signed 8-bit register
- `SMUL rrrr, rr` - Signed multiply 32-bit register by signed 16-bit register
- `SMUL rrrr, rrr` - Signed multiply 32-bit register by signed 24-bit register
- `SMUL rrrr, rrrr` - Signed multiply 32-bit register by signed 32-bit register

## Signed Value Handling

SMUL uses signed value fetching and storage:
- `Get8BitRegisterSigned(regIndex)` - Gets signed 8-bit value
- `Set8BitRegisterSigned(regIndex, value)` - Sets signed 8-bit value
- Similar for 16, 24, 32-bit operations

### Sign Extension

For immediate values, sign extension is performed:
```csharp
ushort combinedValue = (ushort)(((mixedReg & 0b00011111) << 8) + computer.MEMC.Fetch());
if ((combinedValue & 0x1000) != 0)
{
    combinedValue |= 0xF000;  // Sign extend
}
short quotientValue = (short)combinedValue;
```

## Error Handling

SMUL includes error checking (though the check appears to be for division by zero, which is likely a copy-paste artifact from DIV):
```csharp
if (quotientValue.Equals(0))
{
    ErrorHandler.ReportRuntimeError("Division by zero");
}
```

Note: This check doesn't make sense for multiplication and may be a bug in the implementation.

## Flag Updates

SMUL may update CPU flags through register operations, but does not explicitly set flags for multiplication results.

## Operation Categories Summary

1. **8-bit Operations**: 2 variants
2. **16-bit Operations**: 3 variants
3. **24-bit Operations**: 4 variants
4. **32-bit Operations**: 5 variants

## Usage Examples

```
SMUL A, 5        ; A = A * 5 (signed)
SMUL A, -5       ; A = A * -5 (signed)
SMUL BC, DE      ; BC = BC * DE (signed)
```

## Comparison with MUL Instruction

- **SMUL**: Signed multiplication (handles negative numbers correctly)
- **MUL**: Unsigned multiplication (treats all values as positive)
- **SMUL**: Proper sign handling for negative multiplicands and multipliers
- **MUL**: Faster for unsigned operations

## Conclusion

The SMUL instruction provides comprehensive signed multiplication capabilities with support for all data sizes. The proper sign handling makes it essential for signed arithmetic operations in Continuum 93 programs.

