# GETBITS Instruction Execution Analysis

## Executive Summary

The GETBITS instruction implementation in `ExGETBITS.cs` provides **8 distinct instruction variants** for reading specific bit ranges from memory. GETBITS reads a value from a bit-addressed memory location, allowing fine-grained bit extraction from memory.

## File Statistics

- **File**: `Emulator/Execution/ExGETBITS.cs`
- **Total Lines**: 127 lines
- **Instruction Variants**: 8 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 6 bits of opcode

## Addressing Modes

### Bit-Addressed Memory Operations

- `GETBITS r, rrrr, n` - Get 8-bit value from bit memory at address in 32-bit register, using immediate bit count
- `GETBITS rr, rrrr, n` - Get 16-bit value from bit memory at address in 32-bit register, using immediate bit count
- `GETBITS rrr, rrrr, n` - Get 24-bit value from bit memory at address in 32-bit register, using immediate bit count
- `GETBITS rrrr, rrrr, n` - Get 32-bit value from bit memory at address in 32-bit register, using immediate bit count
- `GETBITS r, rrrr, r` - Get 8-bit value from bit memory at address in 32-bit register, using bit count from register
- `GETBITS rr, rrrr, r` - Get 16-bit value from bit memory at address in 32-bit register, using bit count from register
- `GETBITS rrr, rrrr, r` - Get 24-bit value from bit memory at address in 32-bit register, using bit count from register
- `GETBITS rrrr, rrrr, r` - Get 32-bit value from bit memory at address in 32-bit register, using bit count from register

## Operation Description

GETBITS performs bit-addressed memory reads:
1. **Target Register**: Register to store the read value (8, 16, 24, or 32-bit)
2. **Address Register**: 32-bit register containing the bit address in memory
3. **Bit Count**: Number of bits to read (immediate value or from register)

## Opcode Encoding

GETBITS uses compact encoding:
1. **Primary Opcode**: Opcode for GETBITS
2. **Secondary Opcode**: Upper 6 bits of next byte (`ldOp >> 2`)
3. **Target Register Index**: `((ldOp << 3) & 0b00011111) + (mixedReg >> 5)`
4. **Address Register Index**: `(mixedReg & 0b00011111)`
5. **Bit Count**: Immediate byte or from register

## Implementation Details

### Bit Memory Methods

The implementation uses RAM bit memory methods:
- `Get8BitValueFromBitMemoryAt(address, bits)` - Gets 8-bit value from bit memory
- `Get16BitValueFromBitMemoryAt(address, bits)` - Gets 16-bit value from bit memory
- `Get24BitValueFromBitMemoryAt(address, bits)` - Gets 24-bit value from bit memory
- `Get32BitValueFromBitMemoryAt(address, bits)` - Gets 32-bit value from bit memory

### Bit Addressing

Bit addressing allows reading values at arbitrary bit positions:
- Address is in bits, not bytes
- Allows unpacking multiple values from memory efficiently
- Useful for extracting packed data structures

## Operation Categories Summary

1. **8-bit Operations**: 2 variants (immediate bit count, register bit count)
2. **16-bit Operations**: 2 variants
3. **24-bit Operations**: 2 variants
4. **32-bit Operations**: 2 variants

## Usage Examples

```
GETBITS A, ABCD, 4    ; Read 4 bits from bit memory at address in ABCD to A
GETBITS BC, ABCD, 8   ; Read 8 bits from bit memory at address in ABCD to BC
GETBITS XYZ, ABCD, B  ; Read (value in B) bits from bit memory to XYZ
```

## Typical Use Cases

1. **Bit Unpacking**: Extract multiple small values from packed memory
2. **Flag Reading**: Read individual boolean flags from packed storage
3. **Data Decompression**: Extract data from compact representations
4. **Custom Data Formats**: Read from custom bit-level data structures

## Comparison with SETBITS Instruction

- **GETBITS**: Reads value from bit memory
- **SETBITS**: Writes value to bit memory
- **GETBITS/SETBITS**: Complementary operations for bit-addressed memory

## Conclusion

The GETBITS instruction provides efficient bit-addressed memory read capabilities, enabling fine-grained bit extraction and efficient memory usage for unpacking compact data structures.

