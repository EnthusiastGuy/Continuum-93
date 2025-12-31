# SL Instruction Execution Analysis

## Executive Summary

The SL (Shift Left) instruction implementation in `ExSL.cs` provides **8 distinct instruction variants** for shifting register values left by a specified number of bits. SL performs logical left shift, filling with zeros from the right.

## File Statistics

- **File**: `Emulator/Execution/ExSL.cs`
- **Total Lines**: 102 lines
- **Instruction Variants**: 8 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 6 bits of opcode

## Addressing Modes

### Register Shift Operations

- `SL r, n` - Shift 8-bit register left by n bits (0-31)
- `SL r, r` - Shift 8-bit register left by value in register
- `SL rr, n` - Shift 16-bit register left by n bits
- `SL rr, r` - Shift 16-bit register left by value in register
- `SL rrr, n` - Shift 24-bit register left by n bits
- `SL rrr, r` - Shift 24-bit register left by value in register
- `SL rrrr, n` - Shift 32-bit register left by n bits
- `SL rrrr, r` - Shift 32-bit register left by value in register

## Shift Amount

The shift amount is encoded in the lower 5 bits of the mixed register byte:
- **Range**: 0-31 (5 bits)
- **Immediate**: Direct value from opcode
- **Register**: Value from 8-bit register

## Opcode Encoding

SL uses compact encoding:
1. **Primary Opcode**: Opcode 6 (SL)
2. **Secondary Opcode**: Upper 6 bits of next byte (`ldOp >> 2`)
3. **Register Index**: `((ldOp << 3) & 0b00011111) + (mixedReg >> 5)`
4. **Shift Amount**: `(mixedReg & 0b00011111)` or from register

## Implementation Details

### Register Shift Methods

The implementation uses register shift methods:
- `Shift8BitRegisterLeft(register, shiftAmount)` - Shifts 8-bit register left
- `Shift16BitRegisterLeft(register, shiftAmount)` - Shifts 16-bit register left
- `Shift24BitRegisterLeft(register, shiftAmount)` - Shifts 24-bit register left
- `Shift32BitRegisterLeft(register, shiftAmount)` - Shifts 32-bit register left

### Shift Operation

Logical left shift:
- Bits shifted left by specified amount
- Zeros filled in from the right
- Bits shifted out on the left are lost
- Equivalent to multiplication by 2^shiftAmount (for unsigned values)

## Flag Updates

SL updates CPU flags through the register methods:
- **Carry Flag**: Set when bits are shifted out (overflow)
- **Zero Flag**: Set when result equals zero
- **Sign Flag**: Updated based on result value

## Operation Categories Summary

1. **8-bit Operations**: 2 variants (immediate shift, register shift)
2. **16-bit Operations**: 2 variants
3. **24-bit Operations**: 2 variants
4. **32-bit Operations**: 2 variants

## Usage Examples

```
SL A, 1         ; A = A << 1 (multiply by 2)
SL A, 3         ; A = A << 3 (multiply by 8)
SL BC, B        ; BC = BC << (value in B)
SL24 XYZ, 4     ; XYZ = XYZ << 4
```

## Mathematical Properties

- **Multiplication**: Left shift by n bits = multiply by 2^n
- **Power of 2**: Efficient way to multiply by powers of 2
- **Bit Manipulation**: Move bits to higher positions

## Comparison with SR Instruction

- **SL**: Shifts left (multiply by 2^n)
- **SR**: Shifts right (divide by 2^n)
- **SL**: Fills with zeros from right
- **SR**: Fills with zeros from left (logical) or sign bit (arithmetic)

## Conclusion

The SL instruction provides efficient left shift operations for all register sizes. The ability to specify shift amount from a register enables dynamic shift operations, useful for bit manipulation and efficient multiplication by powers of 2.

