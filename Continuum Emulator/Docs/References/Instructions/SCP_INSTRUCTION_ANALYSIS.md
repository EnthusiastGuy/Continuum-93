# SCP Instruction Execution Analysis

## Executive Summary

The SCP (Signed Compare) instruction implementation in `ExSCP.cs` contains **approximately 15 distinct instruction variants**, supporting signed comparison operations across multiple data sizes (8, 16, 24, 32 bits). SCP performs signed subtraction for comparison purposes and sets flags accordingly, but does not modify the source operands.

## File Statistics

- **File**: `Emulator/Execution/ExSCP.cs`
- **Total Lines**: 238 lines
- **Instruction Variants**: ~15 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 6 bits of opcode

## Addressing Modes

### 1. Integer Register Comparisons (Signed)

#### 8-bit Operations
- `SCP r, n` - Compare signed 8-bit register with signed immediate value
- `SCP r, r` - Compare two signed 8-bit registers
- `SCP r, (rrr)` - Compare signed 8-bit register with signed value from register-indirect address

#### 16-bit Operations
- `SCP rr, nn` - Compare signed 16-bit register with signed immediate value
- `SCP rr, rr` - Compare two signed 16-bit registers
- `SCP rr, (rrr)` - Compare signed 16-bit register with signed value from register-indirect address

#### 24-bit Operations
- `SCP rrr, nnn` - Compare signed 24-bit register with signed immediate value
- `SCP rrr, rrr` - Compare two signed 24-bit registers
- `SCP rrr, (rrr)` - Compare signed 24-bit register with signed value from register-indirect address

#### 32-bit Operations
- `SCP rrrr, nnnn` - Compare signed 32-bit register with signed immediate value
- `SCP rrrr, rrrr` - Compare two signed 32-bit registers
- `SCP rrrr, (rrr)` - Compare signed 32-bit register with signed value from register-indirect address

### 2. Memory Comparisons (Signed)

- `SCP (rrr), (rrr)` - Compare two signed 8-bit values at register-indirect addresses
- `SCP (rrr), nnn` - Compare signed 8-bit value at register-indirect address with signed immediate

## Flag Updates

SCP sets multiple flags based on the signed comparison result:

### Signed Comparisons (8/16/24/32-bit)

- **Flag 0 (Z)**: Set if values are equal (`val1 == val2`)
- **Flag 1 (C)**: Not set (commented out in implementation)
- **Flag 2 (SN)**: Set if subtraction result is negative (`subtracted < 0`)
- **Flag 3 (OV)**: Always cleared (set to false)
- **Flag 4 (PO)**: Not set (commented out in implementation)
- **Flag 5 (EQ)**: Set if values are equal (`val1 == val2`)
- **Flag 6 (GT)**: Set if first value is greater than second (`val1 > val2`)
- **Flag 7 (LT)**: Set if first value is less than second (`val1 < val2`)

## Implementation Details

### Signed Comparison Methods

The implementation uses separate comparison methods for each data size:

#### 8-bit Signed Comparison
```csharp
private static void Compare8bitSigned(sbyte val1, sbyte val2)
{
    var subtracted = val1 - val2;
    _computer.CPU.FLAGS.SetValueByIndexFast(0, val1 == val2);    // Z flag
    _computer.CPU.FLAGS.SetValueByIndexFast(2, subtracted < 0);    // SN flag
    _computer.CPU.FLAGS.SetValueByIndexFast(3, false);    // OV flag
    _computer.CPU.FLAGS.SetValueByIndexFast(5, val1 == val2);    // EQ flag
    _computer.CPU.FLAGS.SetValueByIndexFast(6, val1 > val2);    // GT flag
    _computer.CPU.FLAGS.SetValueByIndexFast(7, val1 < val2);    // LT flag
}
```

### Signed Value Fetching

SCP uses signed value fetching methods:
- `Get8BitRegisterSigned(regIndex)` - Gets signed 8-bit value
- `FetchSigned()` - Fetches signed 8-bit immediate
- `Get16BitRegisterSigned(regIndex)` - Gets signed 16-bit value
- `Fetch16Signed()` - Fetches signed 16-bit immediate
- Similar for 24 and 32-bit operations

## Operation Categories Summary

1. **Integer Register Comparisons**: ~12 variants
   - 8-bit: 3 variants
   - 16-bit: 3 variants
   - 24-bit: 3 variants
   - 32-bit: 3 variants

2. **Memory Comparisons**: ~3 variants
   - Register-indirect: 2 variants
   - Immediate: 1 variant

## Comparison with CP Instruction

- **SCP**: Signed comparison (treats values as signed integers)
- **CP**: Unsigned comparison (treats values as unsigned integers)
- **SCP**: Properly handles negative numbers
- **CP**: Treats all values as positive

## Usage Examples

### Signed Comparisons
```
SCP A, -5       ; Compare signed register A with -5
SCP BC, DE      ; Compare signed register BC with DE
SCP rrr, (rrr)  ; Compare signed 24-bit register with signed memory value
```

### Conditional Branching
```
SCP A, 10
JP GT, greater  ; Jump if A > 10 (signed)
JP LT, less     ; Jump if A < 10 (signed)
JP EQ, equal    ; Jump if A == 10
```

## Typical Use Cases

1. **Signed Arithmetic Comparisons**: Compare signed integers
2. **Negative Number Handling**: Proper comparison of negative values
3. **Signed Conditional Branching**: Branch based on signed comparisons
4. **Range Checking**: Check if signed values are within ranges

## Conclusion

The SCP instruction provides comprehensive signed comparison capabilities across all data sizes and addressing modes. The signed nature ensures proper handling of negative numbers, making it essential for signed arithmetic operations in Continuum 93 programs.

