# CP Instruction Execution Analysis

## Executive Summary

The CP (Compare) instruction implementation in `ExCP.cs` contains **approximately 30 distinct instruction variants**, supporting comparison operations across multiple data sizes (8, 16, 24, 32 bits) and floating-point operations. CP performs subtraction for comparison purposes and sets flags accordingly, but does not modify the source operands.

## File Statistics

- **File**: `Emulator/Execution/ExCP.cs`
- **Total Lines**: 493 lines
- **Instruction Variants**: ~30 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 6 bits of opcode

## Addressing Modes

### 1. Integer Register Comparisons

#### 8-bit Operations
- `CP r, n` - Compare 8-bit register with immediate value
- `CP r, r` - Compare two 8-bit registers
- `CP r, (nnn)` - Compare 8-bit register with value from absolute address
- `CP r, (rrr)` - Compare 8-bit register with value from register-indirect address
- `CP (rrr), n` - Compare value at register-indirect address with immediate
- `CP (rrr), (rrr)` - Compare two values at register-indirect addresses

#### 16-bit Operations
- `CP rr, nn` - Compare 16-bit register with immediate value
- `CP rr, rr` - Compare two 16-bit registers
- `CP rr, (rrr)` - Compare 16-bit register with value from register-indirect address

#### 24-bit Operations
- `CP rrr, nnn` - Compare 24-bit register with immediate value
- `CP rrr, rrr` - Compare two 24-bit registers
- `CP rrr, (rrr)` - Compare 24-bit register with value from register-indirect address

#### 32-bit Operations
- `CP rrrr, nnnn` - Compare 32-bit register with immediate value
- `CP rrrr, rrrr` - Compare two 32-bit registers
- `CP rrrr, (rrr)` - Compare 32-bit register with value from register-indirect address

### 2. Floating-Point Comparisons

- `CP fr, fr` - Compare two float registers
- `CP fr, nnn` - Compare float register with immediate float value
- `CP fr, r` - Compare float register with 8-bit integer register (converted to float)
- `CP fr, rr` - Compare float register with 16-bit integer register
- `CP fr, rrr` - Compare float register with 24-bit integer register
- `CP fr, rrrr` - Compare float register with 32-bit integer register
- `CP fr, (nnn)` - Compare float register with float value from memory
- `CP fr, (rrr)` - Compare float register with float value from register-indirect memory
- `CP r, fr` - Compare 8-bit register with float register
- `CP rr, fr` - Compare 16-bit register with float register
- `CP rrr, fr` - Compare 24-bit register with float register
- `CP rrrr, fr` - Compare 32-bit register with float register
- `CP (nnn), fr` - Compare float value in memory with float register
- `CP (rrr), fr` - Compare float value in register-indirect memory with float register

## Flag Updates

CP sets multiple flags based on the comparison result:

### Integer Comparisons (8/16/24/32-bit)

- **Flag 0 (Z)**: Set if values are equal (`val1 == val2`)
- **Flag 1 (C)**: Set if first value is less than second (`val1 < val2`)
- **Flag 2 (SN)**: Set if subtraction result is negative (sign bit set)
- **Flag 3 (OV)**: Always cleared (set to false)
- **Flag 4 (PO)**: Parity flag - set based on overflow detection logic
- **Flag 5 (EQ)**: Set if values are equal (`val1 == val2`)
- **Flag 6 (GT)**: Set if first value is greater than second (`val1 > val2`)
- **Flag 7 (LT)**: Set if first value is less than second (`val1 < val2`)

### Floating-Point Comparisons

- **Flag 0 (Z)**: Set if values are equal (`comparison == 0`)
- **Flag 1 (C)**: Set if first value is less than second (`comparison < 0`)
- **Flag 2 (SN)**: Set if first value is negative (`val1 < 0`)
- **Flag 3 (OV)**: Always cleared (not applicable to floats)
- **Flag 4 (PO)**: Always cleared (not applicable to floats)
- **Flag 5 (EQ)**: Set if values are equal (`comparison == 0`)
- **Flag 6 (GT)**: Set if first value is greater than second (`comparison > 0`)
- **Flag 7 (LT)**: Set if first value is less than second (`comparison < 0`)

## Implementation Details

### Comparison Methods

The implementation uses separate comparison methods for each data size:

#### 8-bit Comparison
```csharp
private static void Compare8bit(byte val1, byte val2)
{
    var subtracted = val1 - val2;
    _computer.CPU.FLAGS.SetValueByIndexFast(0, val1 == val2);    // Z flag
    _computer.CPU.FLAGS.SetValueByIndexFast(1, val1 < val2);    // C flag
    _computer.CPU.FLAGS.SetValueByIndexFast(2, (subtracted & 0x80) > 0);    // SN flag
    _computer.CPU.FLAGS.SetValueByIndexFast(3, false);    // OV flag
    _computer.CPU.FLAGS.SetValueByIndexFast(4, !(val1 > 0x80 && val2 > 0x80 && (sbyte)subtracted > 0) || (val1 < 0x80 && val2 < 0x80 && (sbyte)subtracted < 0));    // PO flag
    _computer.CPU.FLAGS.SetValueByIndexFast(5, val1 == val2);    // EQ flag
    _computer.CPU.FLAGS.SetValueByIndexFast(6, val1 > val2);    // GT flag
    _computer.CPU.FLAGS.SetValueByIndexFast(7, val1 < val2);    // LT flag
}
```

#### Floating-Point Comparison
```csharp
private static void CompareFloat(float val1, float val2)
{
    int comparison = val1.CompareTo(val2);
    _computer.CPU.FLAGS.SetValueByIndexFast(0, comparison == 0);  // Z flag (Equal)
    _computer.CPU.FLAGS.SetValueByIndexFast(1, comparison < 0);   // C flag (Carry, if val1 < val2)
    _computer.CPU.FLAGS.SetValueByIndexFast(2, val1 < 0);        // SN flag (Sign of val1)
    _computer.CPU.FLAGS.SetValueByIndexFast(3, false);           // OV flag (Overflow flag, not applicable to floats)
    _computer.CPU.FLAGS.SetValueByIndexFast(4, false);           // PO flag (Parity flag, not applicable to floats)
    _computer.CPU.FLAGS.SetValueByIndexFast(5, comparison == 0);  // EQ flag (Equal)
    _computer.CPU.FLAGS.SetValueByIndexFast(6, comparison > 0);   // GT flag (Greater Than)
    _computer.CPU.FLAGS.SetValueByIndexFast(7, comparison < 0);   // LT flag (Less Than)
}
```

### Register Index Extraction

For register-to-register comparisons, register indices are extracted from opcode:
```csharp
byte mixedReg = _computer.MEMC.Fetch();
byte reg1Index = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
byte reg2Index = (byte)(mixedReg & 0b00011111);
```

## Operation Categories Summary

1. **Integer Register Comparisons**: ~16 variants
   - 8-bit: 6 variants
   - 16-bit: 3 variants
   - 24-bit: 3 variants
   - 32-bit: 3 variants
   - Memory comparisons: 1 variant

2. **Floating-Point Comparisons**: ~14 variants
   - Float register operations: 8 variants
   - Integer-to-float operations: 4 variants
   - Float-to-integer operations: 4 variants
   - Float memory operations: 2 variants

## Comparison with SUB Instruction

- **CP**: Does not modify operands (only sets flags)
- **SUB**: Modifies the destination operand
- **CP**: Used for conditional branching
- **SUB**: Used for arithmetic operations

## Usage Examples

### Integer Comparisons
```
CP A, 5          ; Compare register A with 5, set flags
CP BC, DE        ; Compare register BC with DE, set flags
CP rrr, (rrr)    ; Compare 24-bit register with memory value
```

### Floating-Point Comparisons
```
CP F0, F1        ; Compare float register F0 with F1
CP F0, 3.14      ; Compare float register F0 with 3.14
CP A, F0         ; Compare 8-bit register A with float F0
```

### Conditional Branching
```
CP A, 10
JP GT, greater   ; Jump if A > 10
JP LT, less      ; Jump if A < 10
JP EQ, equal     ; Jump if A == 10
```

## Conclusion

The CP instruction provides comprehensive comparison capabilities across all data sizes and addressing modes. The extensive flag updates enable rich conditional branching logic, making it essential for control flow in Continuum 93 programs.

