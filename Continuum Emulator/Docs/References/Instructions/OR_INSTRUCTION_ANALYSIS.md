# OR Instruction Execution Analysis

## Executive Summary

The OR instruction implementation in `ExOR.cs` contains **approximately 30 distinct instruction variants**, supporting bitwise OR operations across multiple data sizes (8, 16, 24, 32 bits). OR performs bitwise logical OR between operands, setting each bit in the result to 1 if either corresponding bit in the operands is 1.

## File Statistics

- **File**: `Emulator/Execution/ExOR.cs`
- **Total Lines**: 349 lines
- **Instruction Variants**: ~30 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 6 bits of opcode

## Addressing Modes

The OR instruction supports the same addressing modes as AND:

### 1. Register-to-Register Operations

- `OR r, n` - OR 8-bit register with immediate value
- `OR r, r` - OR 8-bit register with 8-bit register
- `OR rr, nn` - OR 16-bit register with immediate value
- `OR rr, rr` - OR 16-bit register with 16-bit register
- `OR rrr, nnn` - OR 24-bit register with immediate value
- `OR rrr, rrr` - OR 24-bit register with 24-bit register
- `OR rrrr, nnnn` - OR 32-bit register with immediate value
- `OR rrrr, rrrr` - OR 32-bit register with 32-bit register

### 2. Memory-to-Register Operations

- `OR r, (nnn)` - OR 8-bit register with value from absolute address
- `OR r, (rrr)` - OR 8-bit register with value from register-indirect address
- `OR rr, (nnn)` - OR 16-bit register with value from absolute address
- `OR rr, (rrr)` - OR 16-bit register with value from register-indirect address
- `OR rrr, (nnn)` - OR 24-bit register with value from absolute address
- `OR rrr, (rrr)` - OR 24-bit register with value from register-indirect address
- `OR rrrr, (nnn)` - OR 32-bit register with value from absolute address
- `OR rrrr, (rrr)` - OR 32-bit register with value from register-indirect address

### 3. Register-to-Memory Operations

#### Absolute Address Memory
- `OR (nnn), n` - OR memory at absolute address with immediate value
- `OR16 (nnn), nn` - OR 16-bit memory with immediate value
- `OR24 (nnn), nnn` - OR 24-bit memory with immediate value
- `OR32 (nnn), nnnn` - OR 32-bit memory with immediate value
- `OR (nnn), r` - OR memory at absolute address with 8-bit register
- `OR (nnn), rr` - OR memory at absolute address with 16-bit register
- `OR (nnn), rrr` - OR memory at absolute address with 24-bit register
- `OR (nnn), rrrr` - OR memory at absolute address with 32-bit register

#### Register-Indirect Memory
- `OR (rrr), n` - OR memory at register-indirect address with immediate value
- `OR16 (rrr), nn` - OR 16-bit memory with immediate value
- `OR24 (rrr), nnn` - OR 24-bit memory with immediate value
- `OR32 (rrr), nnnn` - OR 32-bit memory with immediate value
- `OR (rrr), r` - OR memory at register-indirect address with 8-bit register
- `OR (rrr), rr` - OR memory at register-indirect address with 16-bit register
- `OR (rrr), rrr` - OR memory at register-indirect address with 24-bit register
- `OR (rrr), rrrr` - OR memory at register-indirect address with 32-bit register

## Data Size Variations

The OR instruction operates on four data sizes, identical to AND:

1. **8-bit operations** (`OR`): Operates on single bytes
2. **16-bit operations** (`OR16`): Operates on 16-bit words
3. **24-bit operations** (`OR24`): Operates on 24-bit values
4. **32-bit operations** (`OR32`): Operates on 32-bit double words

## Flag Updates

The OR instruction updates CPU flags through the register methods:
- **Zero Flag**: Set when result equals zero
- **Sign Flag**: Set when result is negative (for signed operations)
- **Parity Flag**: Updated based on result value
- **Carry Flag**: Typically cleared for logical operations

## Implementation Details

### Register OR Methods

The implementation uses register methods that handle flag updates:
- `Or8Bit(register, value)` - ORs value with 8-bit register
- `Or16Bit(register, value)` - ORs value with 16-bit register
- `Or24Bit(register, value)` - ORs value with 24-bit register
- `Or32Bit(register, value)` - ORs value with 32-bit register

### Memory OR Methods

For memory operations:
- `Or8BitMem(address, value)` - ORs memory with value (8-bit)
- `Or16BitMem(address, value)` - ORs memory with value (16-bit)
- `Or24BitMem(address, value)` - ORs memory with value (24-bit)
- `Or32BitMem(address, value)` - ORs memory with value (32-bit)
- `OrMemTo8BitReg(register, address)` - ORs register with memory (8-bit)
- `OrMemTo16BitReg(register, address)` - ORs register with memory (16-bit)
- `OrMemTo24BitReg(register, address)` - ORs register with memory (24-bit)
- `OrMemTo32BitReg(register, address)` - ORs register with memory (32-bit)

### Register Index Extraction

OR uses compact encoding similar to MUL/DIV:
```csharp
byte mixedReg = computer.MEMC.Fetch();
byte reg1Index = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
byte reg2Index = (byte)(mixedReg & 0b00011111);
```

## Operation Categories Summary

1. **Register Operations**: ~8 variants
   - Register-to-register: 4 variants (8, 16, 24, 32-bit)
   - Immediate-to-register: 4 variants (8, 16, 24, 32-bit)

2. **Memory Operations**: ~22 variants
   - Memory-to-register: 8 variants
   - Register-to-memory: 8 variants
   - Immediate-to-memory: 8 variants

## Bitwise OR Truth Table

| A | B | A OR B |
|---|---|--------|
| 0 | 0 | 0      |
| 0 | 1 | 1      |
| 1 | 0 | 1      |
| 1 | 1 | 1      |

## Common Use Cases

1. **Bit Setting**: Set specific bits in a value
   - `OR A, 0x01` - Set bit 0
   - `OR A, 0x80` - Set bit 7

2. **Flag Combining**: Combine multiple flags
   - `OR A, FLAG1` - Add FLAG1 to existing flags
   - `OR A, FLAG2` - Add FLAG2 to existing flags

3. **Value Merging**: Merge values from different sources

4. **Default Values**: Set default bits when value is zero

## Usage Examples

### Bit Setting
```
OR A, 0x01     ; Set bit 0 of A
OR A, 0x80     ; Set bit 7 (sign bit) of A
OR BC, 0x00FF  ; Set lower 8 bits of BC
```

### Flag Combining
```
OR A, FLAG_ZERO    ; Add zero flag to A
OR A, FLAG_CARRY   ; Add carry flag to A
```

### Register Operations
```
OR A, B        ; A = A OR B
OR BC, 0xFF00  ; BC = BC OR 0xFF00
```

### Memory Operations
```
OR A, (0x1000) ; A = A OR memory[0x1000]
OR (XYZ), 0xFF ; memory[XYZ] = memory[XYZ] OR 0xFF
```

## Comparison with AND/XOR Instructions

- **OR**: Result bit is 1 if either operand has 1
- **AND**: Result bit is 1 only if both operands have 1
- **XOR**: Result bit is 1 if operands differ
- **OR**: Useful for setting bits
- **AND**: Useful for masking and clearing bits
- **XOR**: Useful for toggling bits

## Conclusion

The OR instruction provides comprehensive bitwise OR capabilities across all data sizes and addressing modes. The extensive memory addressing support makes it versatile for bit manipulation, flag setting, and value merging operations in Continuum 93 programs.

