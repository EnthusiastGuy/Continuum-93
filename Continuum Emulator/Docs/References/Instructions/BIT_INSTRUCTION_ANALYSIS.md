# BIT Instruction Execution Analysis

## Executive Summary

The BIT instruction implementation in `ExBIT.cs` provides **8 distinct instruction variants** for testing individual bits in registers. BIT checks if a specific bit is set and updates flags accordingly, but does not modify the register value.

## File Statistics

- **File**: `Emulator/Execution/ExBIT.cs`
- **Total Lines**: 103 lines
- **Instruction Variants**: 8 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 6 bits of opcode

## Addressing Modes

### Register Bit Testing Operations

- `BIT r, n` - Test bit n (0-31) in 8-bit register
- `BIT r, r` - Test bit (value from register) in 8-bit register
- `BIT rr, n` - Test bit n in 16-bit register
- `BIT rr, r` - Test bit (value from register) in 16-bit register
- `BIT rrr, n` - Test bit n in 24-bit register
- `BIT rrr, r` - Test bit (value from register) in 24-bit register
- `BIT rrrr, n` - Test bit n in 32-bit register
- `BIT rrrr, r` - Test bit (value from register) in 32-bit register

## Bit Index Range

The bit index is encoded in the lower 5 bits of the mixed register byte:
- **Range**: 0-31 (5 bits)
- **8-bit registers**: Bits 0-7 are valid
- **16-bit registers**: Bits 0-15 are valid
- **24-bit registers**: Bits 0-23 are valid
- **32-bit registers**: Bits 0-31 are valid

## Opcode Encoding

BIT uses the same compact encoding as SET/RES:
1. **Primary Opcode**: Opcode 12 (BIT)
2. **Secondary Opcode**: Upper 6 bits of next byte (`ldOp >> 2`)
3. **Register Index**: `((ldOp << 3) & 0b00011111) + (mixedReg >> 5)`
4. **Bit Index**: `(mixedReg & 0b00011111)`

## Flag Updates

BIT sets flags based on the tested bit:
- **Zero Flag**: Set if the tested bit is 0 (not set)
- **Sign Flag**: May be updated based on bit value
- **Other Flags**: Updated according to register test methods

## Implementation Details

### Register Methods

The implementation uses register test methods:
- `Test8BitBit(register, bitIndex)` - Tests bit in 8-bit register
- `Test16BitBit(register, bitIndex)` - Tests bit in 16-bit register
- `Test24BitBit(register, bitIndex)` - Tests bit in 24-bit register
- `Test32BitBit(register, bitIndex)` - Tests bit in 32-bit register

### Bit Testing Logic

The bit testing operation:
1. Extracts register index from opcode
2. Extracts bit index (0-31) from opcode or register
3. Tests if the specified bit is set
4. Updates flags accordingly
5. Does not modify the register value

## Operation Categories Summary

1. **8-bit Operations**: 2 variants (immediate bit index, register bit index)
2. **16-bit Operations**: 2 variants
3. **24-bit Operations**: 2 variants
4. **32-bit Operations**: 2 variants

## Usage Examples

```
BIT A, 0        ; Test bit 0 of register A, set Z flag if clear
BIT A, 7        ; Test bit 7 (sign bit) of register A
BIT BC, 15      ; Test bit 15 of register BC
BIT XYZ, B      ; Test bit (value in B) of register XYZ
```

## Typical Use Cases

1. **Flag Checking**: Check if specific flags/bits are set
2. **Conditional Branching**: Branch based on bit state
3. **Bit Status**: Determine if a bit is set without modifying it

## Comparison with SET/RES Instructions

- **BIT**: Tests bit (read-only, sets flags)
- **SET**: Sets bit to 1 (modifies register)
- **RES**: Sets bit to 0 (modifies register)
- **BIT**: Non-destructive operation

## Conclusion

The BIT instruction provides efficient bit testing capabilities for all register sizes. The non-destructive nature makes it ideal for conditional branching based on bit states.

