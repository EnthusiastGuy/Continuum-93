# SDIV Instruction Execution Analysis

## Executive Summary

The SDIV (Signed Divide) instruction implementation in `ExSDIV.cs` provides **approximately 20 distinct instruction variants** for signed division operations across multiple data sizes (8, 16, 24, 32 bits). SDIV performs signed integer division, handling both division without remainder and division with remainder.

## File Statistics

- **File**: `Emulator/Execution/ExSDIV.cs`
- **Total Lines**: 624 lines
- **Instruction Variants**: ~20 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 6 bits of opcode

## Addressing Modes

### 1. Division Without Remainder

#### 8-bit Operations
- `SDIV r, n` - Signed divide 8-bit register by signed immediate value
- `SDIV r, r` - Signed divide 8-bit register by signed 8-bit register

#### 16-bit Operations
- `SDIV rr, n` - Signed divide 16-bit register by signed immediate value
- `SDIV rr, r` - Signed divide 16-bit register by signed 8-bit register
- `SDIV rr, rr` - Signed divide 16-bit register by signed 16-bit register

#### 24-bit Operations
- `SDIV rrr, n` - Signed divide 24-bit register by signed immediate value
- `SDIV rrr, r` - Signed divide 24-bit register by signed 8-bit register
- `SDIV rrr, rr` - Signed divide 24-bit register by signed 16-bit register
- `SDIV rrr, rrr` - Signed divide 24-bit register by signed 24-bit register

#### 32-bit Operations
- `SDIV rrrr, n` - Signed divide 32-bit register by signed immediate value
- `SDIV rrrr, r` - Signed divide 32-bit register by signed 8-bit register
- `SDIV rrrr, rr` - Signed divide 32-bit register by signed 16-bit register
- `SDIV rrrr, rrr` - Signed divide 32-bit register by signed 24-bit register
- `SDIV rrrr, rrrr` - Signed divide 32-bit register by signed 32-bit register

### 2. Division With Remainder

- `SDIV r, n, r` - Signed divide 8-bit register by signed immediate, store remainder in register
- `SDIV r, r, r` - Signed divide 8-bit register by signed register, store remainder in register
- `SDIV rr, n, rr` - Signed divide 16-bit register by signed immediate, store remainder in register
- `SDIV rr, r, r` - Signed divide 16-bit register by signed 8-bit register, store remainder in 8-bit register
- `SDIV rr, rr, rr` - Signed divide 16-bit register by signed 16-bit register, store remainder in register
- Similar patterns for 24 and 32-bit operations

## Signed Value Handling

SDIV uses signed value fetching and storage:
- `Get8BitRegisterSigned(regIndex)` - Gets signed 8-bit value
- `Set8BitRegisterSigned(regIndex, value)` - Sets signed 8-bit value
- `FetchSigned()` - Fetches signed 8-bit immediate
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

## Division by Zero Handling

SDIV reports runtime errors for division by zero:
```csharp
if (quotientValue.Equals(0))
{
    ErrorHandler.ReportRuntimeError("Division by zero");
}
```

Unlike unsigned DIV, SDIV does not set registers to maximum values on division by zero.

## Flag Updates

SDIV may update CPU flags through register operations, but does not explicitly set flags for division results.

## Operation Categories Summary

1. **Division Without Remainder**: ~12 variants
   - 8-bit: 2 variants
   - 16-bit: 3 variants
   - 24-bit: 4 variants
   - 32-bit: 5 variants

2. **Division With Remainder**: ~8 variants
   - 8-bit: 2 variants
   - 16-bit: 3 variants
   - 24-bit: 3 variants
   - 32-bit: 4 variants

## Usage Examples

```
SDIV A, 5        ; A = A / 5 (signed)
SDIV A, -5       ; A = A / -5 (signed)
SDIV BC, DE      ; BC = BC / DE (signed)
SDIV A, 5, B     ; A = A / 5, B = A % 5 (signed)
```

## Comparison with DIV Instruction

- **SDIV**: Signed division (handles negative numbers correctly)
- **DIV**: Unsigned division (treats all values as positive)
- **SDIV**: Proper sign handling for negative dividends and divisors
- **DIV**: Faster for unsigned operations

## Conclusion

The SDIV instruction provides comprehensive signed division capabilities with support for all data sizes and remainder operations. The proper sign handling makes it essential for signed arithmetic operations in Continuum 93 programs.

