# SR Instruction Execution Analysis

## Executive Summary

The SR (Shift Right) instruction implementation in `ExSR.cs` provides **8 distinct instruction variants** for shifting register values right by a specified number of bits. SR performs logical right shift, filling with zeros from the left.

## File Statistics

- **File**: `Emulator/Execution/ExSR.cs`
- **Total Lines**: 102 lines
- **Instruction Variants**: 8 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 6 bits of opcode

## Addressing Modes

### Register Shift Operations

- `SR r, n` - Shift 8-bit register right by n bits (0-31)
- `SR r, r` - Shift 8-bit register right by value in register
- `SR rr, n` - Shift 16-bit register right by n bits
- `SR rr, r` - Shift 16-bit register right by value in register
- `SR rrr, n` - Shift 24-bit register right by n bits
- `SR rrr, r` - Shift 24-bit register right by value in register
- `SR rrrr, n` - Shift 32-bit register right by n bits
- `SR rrrr, r` - Shift 32-bit register right by value in register

## Shift Amount

The shift amount is encoded in the lower 5 bits of the mixed register byte:
- **Range**: 0-31 (5 bits)
- **Immediate**: Direct value from opcode
- **Register**: Value from 8-bit register

## Opcode Encoding

SR uses the same compact encoding as SL:
1. **Primary Opcode**: Opcode 7 (SR)
2. **Secondary Opcode**: Upper 6 bits of next byte (`ldOp >> 2`)
3. **Register Index**: `((ldOp << 3) & 0b00011111) + (mixedReg >> 5)`
4. **Shift Amount**: `(mixedReg & 0b00011111)` or from register

## Implementation Details

### Register Shift Methods

The implementation uses register shift methods:
- `Shift8BitRegisterRight(register, shiftAmount)` - Shifts 8-bit register right
- `Shift16BitRegisterRight(register, shiftAmount)` - Shifts 16-bit register right
- `Shift24BitRegisterRight(register, shiftAmount)` - Shifts 24-bit register right
- `Shift32BitRegisterRight(register, shiftAmount)` - Shifts 32-bit register right

### Shift Operation

Logical right shift:
- Bits shifted right by specified amount
- Zeros filled in from the left
- Bits shifted out on the right are lost
- Equivalent to division by 2^shiftAmount (for unsigned values, truncating)

## Flag Updates

SR updates CPU flags through the register methods:
- **Carry Flag**: Set when bits are shifted out
- **Zero Flag**: Set when result equals zero
- **Sign Flag**: Updated based on result value

## Operation Categories Summary

1. **8-bit Operations**: 2 variants (immediate shift, register shift)
2. **16-bit Operations**: 2 variants
3. **24-bit Operations**: 2 variants
4. **32-bit Operations**: 2 variants

## Usage Examples

```
SR A, 1         ; A = A >> 1 (divide by 2, truncate)
SR A, 3         ; A = A >> 3 (divide by 8, truncate)
SR BC, B        ; BC = BC >> (value in B)
SR24 XYZ, 4     ; XYZ = XYZ >> 4
```

## Mathematical Properties

- **Division**: Right shift by n bits = divide by 2^n (truncated)
- **Power of 2**: Efficient way to divide by powers of 2
- **Bit Extraction**: Extract lower bits by shifting right then masking

## Comparison with SL Instruction

- **SR**: Shifts right (divide by 2^n)
- **SL**: Shifts left (multiply by 2^n)
- **SR**: Fills with zeros from left (logical shift)
- **SL**: Fills with zeros from right

## Conclusion

The SR instruction provides efficient right shift operations for all register sizes. The ability to specify shift amount from a register enables dynamic shift operations, useful for bit manipulation and efficient division by powers of 2.

