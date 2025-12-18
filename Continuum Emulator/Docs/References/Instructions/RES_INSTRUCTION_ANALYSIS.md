# RES Instruction Execution Analysis

## Executive Summary

The RES instruction implementation in `ExRES.cs` provides **8 distinct instruction variants** for resetting (clearing) individual bits in registers. RES sets a specific bit (by index) to 0 in 8, 16, 24, or 32-bit registers.

## File Statistics

- **File**: `Emulator/Execution/ExRES.cs`
- **Total Lines**: 103 lines
- **Instruction Variants**: 8 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 6 bits of opcode

## Addressing Modes

### Register Bit Resetting Operations

- `RES r, n` - Reset bit n (0-31) in 8-bit register
- `RES r, r` - Reset bit (value from register) in 8-bit register
- `RES rr, n` - Reset bit n in 16-bit register
- `RES rr, r` - Reset bit (value from register) in 16-bit register
- `RES rrr, n` - Reset bit n in 24-bit register
- `RES rrr, r` - Reset bit (value from register) in 24-bit register
- `RES rrrr, n` - Reset bit n in 32-bit register
- `RES rrrr, r` - Reset bit (value from register) in 32-bit register

## Bit Index Range

The bit index is encoded in the lower 5 bits of the mixed register byte:
- **Range**: 0-31 (5 bits)
- **8-bit registers**: Bits 0-7 are valid
- **16-bit registers**: Bits 0-15 are valid
- **24-bit registers**: Bits 0-23 are valid
- **32-bit registers**: Bits 0-31 are valid

## Opcode Encoding

RES uses the same compact encoding as SET:
1. **Primary Opcode**: Opcode 11 (RES)
2. **Secondary Opcode**: Upper 6 bits of next byte (`ldOp >> 2`)
3. **Register Index**: `((ldOp << 3) & 0b00011111) + (mixedReg >> 5)`
4. **Bit Index**: `(mixedReg & 0b00011111)`

## Implementation Details

### Register Methods

The implementation uses register methods:
- `Reset8BitBit(register, bitIndex)` - Resets bit in 8-bit register
- `Reset16BitBit(register, bitIndex)` - Resets bit in 16-bit register
- `Reset24BitBit(register, bitIndex)` - Resets bit in 24-bit register
- `Reset32BitBit(register, bitIndex)` - Resets bit in 32-bit register

### Bit Resetting Logic

The bit resetting operation:
1. Extracts register index from opcode
2. Extracts bit index (0-31) from opcode or register
3. Sets the specified bit to 0
4. Other bits remain unchanged

## Operation Categories Summary

1. **8-bit Operations**: 2 variants (immediate bit index, register bit index)
2. **16-bit Operations**: 2 variants
3. **24-bit Operations**: 2 variants
4. **32-bit Operations**: 2 variants

## Usage Examples

```
RES A, 0        ; Clear bit 0 of register A
RES A, 7        ; Clear bit 7 (sign bit) of register A
RES BC, 15      ; Clear bit 15 of register BC
RES XYZ, B      ; Clear bit (value in B) of register XYZ
```

## Comparison with SET Instruction

- **RES**: Sets bit to 0
- **SET**: Sets bit to 1
- **RES/SET**: Complementary operations

## Conclusion

The RES instruction provides efficient bit clearing capabilities for all register sizes. The compact encoding allows specifying both the register and bit index in minimal instruction space.

