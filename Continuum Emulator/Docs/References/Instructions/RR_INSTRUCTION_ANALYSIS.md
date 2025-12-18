# RR Instruction Execution Analysis

## Executive Summary

The RR (Rotate Right) instruction implementation in `ExRR.cs` provides **8 distinct instruction variants** for rotating register values right by a specified number of bits. RR performs circular rotation, where bits shifted out on the right wrap around to the left.

## File Statistics

- **File**: `Emulator/Execution/ExRR.cs`
- **Total Lines**: 102 lines
- **Instruction Variants**: 8 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 6 bits of opcode

## Addressing Modes

### Register Rotate Operations

- `RR r, n` - Rotate 8-bit register right by n bits (0-31)
- `RR r, r` - Rotate 8-bit register right by value in register
- `RR rr, n` - Rotate 16-bit register right by n bits
- `RR rr, r` - Rotate 16-bit register right by value in register
- `RR rrr, n` - Rotate 24-bit register right by n bits
- `RR rrr, r` - Rotate 24-bit register right by value in register
- `RR rrrr, n` - Rotate 32-bit register right by n bits
- `RR rrrr, r` - Rotate 32-bit register right by value in register

## Rotate Amount

The rotate amount is encoded in the lower 5 bits of the mixed register byte:
- **Range**: 0-31 (5 bits)
- **Immediate**: Direct value from opcode
- **Register**: Value from 8-bit register

## Opcode Encoding

RR uses the same compact encoding as RL:
1. **Primary Opcode**: Opcode 9 (RR)
2. **Secondary Opcode**: Upper 6 bits of next byte (`ldOp >> 2`)
3. **Register Index**: `((ldOp << 3) & 0b00011111) + (mixedReg >> 5)`
4. **Rotate Amount**: `(mixedReg & 0b00011111)` or from register

## Implementation Details

### Register Rotate Methods

The implementation uses register rotate methods:
- `Roll8BitRegisterRight(register, rotateAmount)` - Rotates 8-bit register right
- `Roll16BitRegisterRight(register, rotateAmount)` - Rotates 16-bit register right
- `Roll24BitRegisterRight(register, rotateAmount)` - Rotates 24-bit register right
- `Roll32BitRegisterRight(register, rotateAmount)` - Rotates 32-bit register right

### Rotate Operation

Circular right rotation:
- Bits shifted right by specified amount
- Bits shifted out on the right wrap around to the left
- No bits are lost (unlike shift operations)
- All bits remain in the register

## Flag Updates

RR updates CPU flags through the register methods:
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
RR A, 1         ; Rotate A right by 1 bit
RR A, 3         ; Rotate A right by 3 bits
RR BC, B        ; Rotate BC right by (value in B) bits
RR24 XYZ, 4     ; Rotate XYZ right by 4 bits
```

## Mathematical Properties

- **Circular**: All bits preserved, just repositioned
- **Reversible**: Rotating right by n then left by n returns original value
- **Bit Permutation**: Rearranges bits without loss

## Comparison with RL Instruction

- **RR**: Rotates right (bits wrap from right to left)
- **RL**: Rotates left (bits wrap from left to right)
- **RR/RL**: Complementary operations

## Comparison with SR Instruction

- **RR**: Circular rotation (bits wrap around)
- **SR**: Logical shift (bits lost, zeros filled)
- **RR**: Preserves all bits
- **SR**: Loses bits on the right

## Conclusion

The RR instruction provides efficient right rotation operations for all register sizes. The circular nature preserves all bits, making it useful for bit permutation and cryptographic operations.

