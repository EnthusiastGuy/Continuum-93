# AND Instruction Execution Analysis

## Executive Summary

The AND instruction implementation in `ExAND.cs` contains **approximately 30 distinct instruction variants**, supporting bitwise AND operations across multiple data sizes (8, 16, 24, 32 bits). AND performs bitwise logical AND between operands, setting each bit in the result to 1 only if both corresponding bits in the operands are 1.

## File Statistics

- **File**: `Emulator/Execution/ExAND.cs`
- **Total Lines**: 332 lines
- **Instruction Variants**: ~30 unique opcodes
- **Implementation Pattern**: Uses switch statement on secondary opcode

## Addressing Modes

### 1. Register-to-Register Operations

- `AND r, n` - AND 8-bit register with immediate value
- `AND r, r` - AND 8-bit register with 8-bit register
- `AND rr, nn` - AND 16-bit register with immediate value
- `AND rr, rr` - AND 16-bit register with 16-bit register
- `AND rrr, nnn` - AND 24-bit register with immediate value
- `AND rrr, rrr` - AND 24-bit register with 24-bit register
- `AND rrrr, nnnn` - AND 32-bit register with immediate value
- `AND rrrr, rrrr` - AND 32-bit register with 32-bit register

### 2. Memory-to-Register Operations

- `AND r, (nnn)` - AND 8-bit register with value from absolute address
- `AND r, (rrr)` - AND 8-bit register with value from register-indirect address
- `AND rr, (nnn)` - AND 16-bit register with value from absolute address
- `AND rr, (rrr)` - AND 16-bit register with value from register-indirect address
- `AND rrr, (nnn)` - AND 24-bit register with value from absolute address
- `AND rrr, (rrr)` - AND 24-bit register with value from register-indirect address
- `AND rrrr, (nnn)` - AND 32-bit register with value from absolute address
- `AND rrrr, (rrr)` - AND 32-bit register with value from register-indirect address

### 3. Register-to-Memory Operations

#### Absolute Address Memory
- `AND (nnn), n` - AND memory at absolute address with immediate value
- `AND16 (nnn), nn` - AND 16-bit memory with immediate value
- `AND24 (nnn), nnn` - AND 24-bit memory with immediate value
- `AND32 (nnn), nnnn` - AND 32-bit memory with immediate value
- `AND (nnn), r` - AND memory at absolute address with 8-bit register
- `AND (nnn), rr` - AND memory at absolute address with 16-bit register
- `AND (nnn), rrr` - AND memory at absolute address with 24-bit register
- `AND (nnn), rrrr` - AND memory at absolute address with 32-bit register

#### Register-Indirect Memory
- `AND (rrr), n` - AND memory at register-indirect address with immediate value
- `AND16 (rrr), nn` - AND 16-bit memory with immediate value
- `AND24 (rrr), nnn` - AND 24-bit memory with immediate value
- `AND32 (rrr), nnnn` - AND 32-bit memory with immediate value
- `AND (rrr), r` - AND memory at register-indirect address with 8-bit register
- `AND (rrr), rr` - AND memory at register-indirect address with 16-bit register
- `AND (rrr), rrr` - AND memory at register-indirect address with 24-bit register
- `AND (rrr), rrrr` - AND memory at register-indirect address with 32-bit register

## Data Size Variations

The AND instruction operates on four data sizes:

1. **8-bit operations** (`AND`): Operates on single bytes
2. **16-bit operations** (`AND16`): Operates on 16-bit words
3. **24-bit operations** (`AND24`): Operates on 24-bit values
4. **32-bit operations** (`AND32`): Operates on 32-bit double words

## Flag Updates

The AND instruction updates CPU flags through the register methods:
- **Zero Flag**: Set when result equals zero
- **Sign Flag**: Set when result is negative (for signed operations)
- **Parity Flag**: Updated based on result value
- **Carry Flag**: Typically cleared for logical operations

## Implementation Details

### Register AND Methods

The implementation uses register methods that handle flag updates:
- `And8Bit(register, value)` - ANDs value with 8-bit register
- `And16Bit(register, value)` - ANDs value with 16-bit register
- `And24Bit(register, value)` - ANDs value with 24-bit register
- `And32Bit(register, value)` - ANDs value with 32-bit register

### Memory AND Methods

For memory operations:
- `And8BitRegToMem(address, value)` - ANDs memory with value (8-bit)
- `And16BitRegToMem(address, value)` - ANDs memory with value (16-bit)
- `And24BitRegToMem(address, value)` - ANDs memory with value (24-bit)
- `And32BitRegToMem(address, value)` - ANDs memory with value (32-bit)
- `AndMemTo8BitReg(register, address)` - ANDs register with memory (8-bit)
- `AndMemTo16BitReg(register, address)` - ANDs register with memory (16-bit)
- `AndMemTo24BitReg(register, address)` - ANDs register with memory (24-bit)
- `AndMemTo32BitReg(register, address)` - ANDs register with memory (32-bit)

## Operation Categories Summary

1. **Register Operations**: ~8 variants
   - Register-to-register: 4 variants (8, 16, 24, 32-bit)
   - Immediate-to-register: 4 variants (8, 16, 24, 32-bit)

2. **Memory Operations**: ~22 variants
   - Memory-to-register: 8 variants
   - Register-to-memory: 8 variants
   - Immediate-to-memory: 8 variants

## Bitwise AND Truth Table

| A | B | A AND B |
|---|---|---------|
| 0 | 0 | 0       |
| 0 | 1 | 0       |
| 1 | 0 | 0       |
| 1 | 1 | 1       |

## Common Use Cases

1. **Bit Masking**: Extract specific bits from a value
   - `AND A, 0x0F` - Extract lower 4 bits
   - `AND A, 0xF0` - Extract upper 4 bits

2. **Bit Clearing**: Clear specific bits
   - `AND A, 0xFE` - Clear bit 0
   - `AND A, 0xFD` - Clear bit 1

3. **Flag Checking**: Check if specific flags are set
   - `AND A, FLAG_MASK` - Check multiple flags at once

4. **Data Filtering**: Filter data based on bit patterns

## Usage Examples

### Bit Masking
```
AND A, 0x0F     ; Extract lower 4 bits of A
AND BC, 0xFF00 ; Extract upper 8 bits of BC
```

### Bit Clearing
```
AND A, 0xFE     ; Clear bit 0 (make even)
AND A, 0x7F     ; Clear bit 7 (clear sign bit)
```

### Register Operations
```
AND A, B        ; A = A AND B
AND BC, 0xFF00  ; BC = BC AND 0xFF00
```

### Memory Operations
```
AND A, (0x1000) ; A = A AND memory[0x1000]
AND (XYZ), 0xFF ; memory[XYZ] = memory[XYZ] AND 0xFF
```

## Comparison with OR/XOR Instructions

- **AND**: Result bit is 1 only if both operands have 1
- **OR**: Result bit is 1 if either operand has 1
- **XOR**: Result bit is 1 if operands differ
- **AND**: Useful for masking and clearing bits
- **OR**: Useful for setting bits
- **XOR**: Useful for toggling bits

## Conclusion

The AND instruction provides comprehensive bitwise AND capabilities across all data sizes and addressing modes. The extensive memory addressing support makes it versatile for bit manipulation, masking, and data filtering operations in Continuum 93 programs.

