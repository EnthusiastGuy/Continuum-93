# RL Instruction Execution Analysis

## Executive Summary

The RL (Rotate Left) instruction implementation in `ExRL.cs` provides **8 distinct instruction variants** for rotating register values left by a specified number of bits. RL performs circular rotation, where bits shifted out on the left wrap around to the right.

## File Statistics

- **File**: `Emulator/Execution/ExRL.cs`
- **Total Lines**: 102 lines
- **Instruction Variants**: 8 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 6 bits of opcode

## Addressing Modes

### Register Rotate Operations

- `RL r, n` - Rotate 8-bit register left by n bits (0-31)
- `RL r, r` - Rotate 8-bit register left by value in register
- `RL rr, n` - Rotate 16-bit register left by n bits
- `RL rr, r` - Rotate 16-bit register left by value in register
- `RL rrr, n` - Rotate 24-bit register left by n bits
- `RL rrr, r` - Rotate 24-bit register left by value in register
- `RL rrrr, n` - Rotate 32-bit register left by n bits
- `RL rrrr, r` - Rotate 32-bit register left by value in register

## Rotate Amount

The rotate amount is encoded in the lower 5 bits of the mixed register byte:
- **Range**: 0-31 (5 bits)
- **Immediate**: Direct value from opcode
- **Register**: Value from 8-bit register

## Opcode Encoding

RL uses the same compact encoding as SL/SR:
1. **Primary Opcode**: Opcode 8 (RL)
2. **Secondary Opcode**: Upper 6 bits of next byte (`ldOp >> 2`)
3. **Register Index**: `((ldOp << 3) & 0b00011111) + (mixedReg >> 5)`
4. **Rotate Amount**: `(mixedReg & 0b00011111)` or from register

## Implementation Details

### Register Rotate Methods

The implementation uses register rotate methods:
- `Roll8BitRegisterLeft(register, rotateAmount)` - Rotates 8-bit register left
- `Roll16BitRegisterLeft(register, rotateAmount)` - Rotates 16-bit register left
- `Roll24BitRegisterLeft(register, rotateAmount)` - Rotates 24-bit register left
- `Roll32BitRegisterLeft(register, rotateAmount)` - Rotates 32-bit register left

### Rotate Operation

Circular left rotation:
- Bits shifted left by specified amount
- Bits shifted out on the left wrap around to the right
- No bits are lost (unlike shift operations)
- All bits remain in the register

## Flag Updates

RL updates CPU flags through the register methods:
- **Carry Flag**: Set when bits wrap around
- **Zero Flag**: Set when result equals zero
- **Sign Flag**: Updated based on result value

## Operation Categories Summary

1. **8-bit Operations**: 2 variants (immediate rotate, register rotate)
2. **16-bit Operations**: 2 variants
3. **24-bit Operations**: 2 variants
4. **32-bit Operations**: 2 variants

## Usage Examples

```
RL A, 1         ; Rotate A left by 1 bit
RL A, 3         ; Rotate A left by 3 bits
RL BC, B        ; Rotate BC left by (value in B) bits
RL24 XYZ, 4     ; Rotate XYZ left by 4 bits
```

## Mathematical Properties

- **Circular**: All bits preserved, just repositioned
- **Reversible**: Rotating left by n then right by n returns original value
- **Bit Permutation**: Rearranges bits without loss

## Comparison with RR Instruction

- **RL**: Rotates left (bits wrap from left to right)
- **RR**: Rotates right (bits wrap from right to left)
- **RL/RR**: Complementary operations

## Comparison with SL Instruction

- **RL**: Circular rotation (bits wrap around)
- **SL**: Logical shift (bits lost, zeros filled)
- **RL**: Preserves all bits
- **SL**: Loses bits on the left

## Conclusion

The RL instruction provides efficient left rotation operations for all register sizes. The circular nature preserves all bits, making it useful for bit permutation and cryptographic operations.

