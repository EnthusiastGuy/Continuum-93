# XOR Instruction Execution Analysis

## Executive Summary

The XOR instruction implementation in `ExXOR.cs` contains **approximately 30 distinct instruction variants**, supporting bitwise XOR (exclusive OR) operations across multiple data sizes (8, 16, 24, 32 bits). XOR performs bitwise exclusive OR between operands, setting each bit in the result to 1 if the corresponding bits in the operands differ.

## File Statistics

- **File**: `Emulator/Execution/ExXOR.cs`
- **Total Lines**: 348 lines
- **Instruction Variants**: ~30 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 6 bits of opcode

## Addressing Modes

The XOR instruction supports the same addressing modes as AND and OR:

### 1. Register-to-Register Operations

- `XOR r, n` - XOR 8-bit register with immediate value
- `XOR r, r` - XOR 8-bit register with 8-bit register
- `XOR rr, nn` - XOR 16-bit register with immediate value
- `XOR rr, rr` - XOR 16-bit register with 16-bit register
- `XOR rrr, nnn` - XOR 24-bit register with immediate value
- `XOR rrr, rrr` - XOR 24-bit register with 24-bit register
- `XOR rrrr, nnnn` - XOR 32-bit register with immediate value
- `XOR rrrr, rrrr` - XOR 32-bit register with 32-bit register

### 2. Memory-to-Register Operations

- `XOR r, (nnn)` - XOR 8-bit register with value from absolute address
- `XOR r, (rrr)` - XOR 8-bit register with value from register-indirect address
- `XOR rr, (nnn)` - XOR 16-bit register with value from absolute address
- `XOR rr, (rrr)` - XOR 16-bit register with value from register-indirect address
- `XOR rrr, (nnn)` - XOR 24-bit register with value from absolute address
- `XOR rrr, (rrr)` - XOR 24-bit register with value from register-indirect address
- `XOR rrrr, (nnn)` - XOR 32-bit register with value from absolute address
- `XOR rrrr, (rrr)` - XOR 32-bit register with value from register-indirect address

### 3. Register-to-Memory Operations

#### Absolute Address Memory
- `XOR (nnn), n` - XOR memory at absolute address with immediate value
- `XOR16 (nnn), nn` - XOR 16-bit memory with immediate value
- `XOR24 (nnn), nnn` - XOR 24-bit memory with immediate value
- `XOR32 (nnn), nnnn` - XOR 32-bit memory with immediate value
- `XOR (nnn), r` - XOR memory at absolute address with 8-bit register
- `XOR (nnn), rr` - XOR memory at absolute address with 16-bit register
- `XOR (nnn), rrr` - XOR memory at absolute address with 24-bit register
- `XOR (nnn), rrrr` - XOR memory at absolute address with 32-bit register

#### Register-Indirect Memory
- `XOR (rrr), n` - XOR memory at register-indirect address with immediate value
- `XOR16 (rrr), nn` - XOR 16-bit memory with immediate value
- `XOR24 (rrr), nnn` - XOR 24-bit memory with immediate value
- `XOR32 (rrr), nnnn` - XOR 32-bit memory with immediate value
- `XOR (rrr), r` - XOR memory at register-indirect address with 8-bit register
- `XOR (rrr), rr` - XOR memory at register-indirect address with 16-bit register
- `XOR (rrr), rrr` - XOR memory at register-indirect address with 24-bit register
- `XOR (rrr), rrrr` - XOR memory at register-indirect address with 32-bit register

## Data Size Variations

The XOR instruction operates on four data sizes, identical to AND and OR:

1. **8-bit operations** (`XOR`): Operates on single bytes
2. **16-bit operations** (`XOR16`): Operates on 16-bit words
3. **24-bit operations** (`XOR24`): Operates on 24-bit values
4. **32-bit operations** (`XOR32`): Operates on 32-bit double words

## Flag Updates

The XOR instruction updates CPU flags through the register methods:
- **Zero Flag**: Set when result equals zero
- **Sign Flag**: Set when result is negative (for signed operations)
- **Parity Flag**: Updated based on result value
- **Carry Flag**: Typically cleared for logical operations

## Implementation Details

### Register XOR Methods

The implementation uses register methods that handle flag updates:
- `Xor8Bit(register, value)` - XORs value with 8-bit register
- `Xor16Bit(register, value)` - XORs value with 16-bit register
- `Xor24Bit(register, value)` - XORs value with 24-bit register
- `Xor32Bit(register, value)` - XORs value with 32-bit register

### Memory XOR Methods

For memory operations:
- `Xor8BitMem(address, value)` - XORs memory with value (8-bit)
- `Xor16BitMem(address, value)` - XORs memory with value (16-bit)
- `Xor24BitMem(address, value)` - XORs memory with value (24-bit)
- `Xor32BitMem(address, value)` - XORs memory with value (32-bit)
- `XorMemTo8BitReg(register, address)` - XORs register with memory (8-bit)
- `XorMemTo16BitReg(register, address)` - XORs register with memory (16-bit)
- `XorMemTo24BitReg(register, address)` - XORs register with memory (24-bit)
- `XorMemTo32BitReg(register, address)` - XORs register with memory (32-bit)

### Register Index Extraction

XOR uses compact encoding similar to OR:
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

## Bitwise XOR Truth Table

| A | B | A XOR B |
|---|---|---------|
| 0 | 0 | 0       |
| 0 | 1 | 1       |
| 1 | 0 | 1       |
| 1 | 1 | 0       |

## Key Properties of XOR

1. **Self-Inverse**: `A XOR A = 0` and `A XOR 0 = A`
2. **Commutative**: `A XOR B = B XOR A`
3. **Associative**: `(A XOR B) XOR C = A XOR (B XOR C)`
4. **Toggle Operation**: XORing with 1 toggles a bit

## Common Use Cases

1. **Bit Toggling**: Toggle specific bits
   - `XOR A, 0x01` - Toggle bit 0
   - `XOR A, 0xFF` - Toggle all bits

2. **Register Clearing**: Clear register by XORing with itself
   - `XOR A, A` - Sets A to 0

3. **Value Swapping**: Swap two values without temporary variable
   - `XOR A, B` then `XOR B, A` then `XOR A, B`

4. **Encryption/Decryption**: Simple XOR cipher
   - Encrypt: `XOR data, key`
   - Decrypt: `XOR encrypted, key` (same operation)

5. **Parity Checking**: Calculate parity of values

## Usage Examples

### Bit Toggling
```
XOR A, 0x01     ; Toggle bit 0 of A
XOR A, 0x80     ; Toggle bit 7 (sign bit) of A
XOR BC, 0x00FF  ; Toggle lower 8 bits of BC
```

### Register Clearing
```
XOR A, A        ; A = 0 (clear register)
XOR BC, BC      ; BC = 0
```

### Value Swapping
```
XOR A, B        ; A = A XOR B
XOR B, A        ; B = B XOR (A XOR B) = A
XOR A, B        ; A = (A XOR B) XOR A = B
```

### Encryption
```
XOR A, key      ; Encrypt/decrypt A with key
XOR (addr), key ; Encrypt/decrypt memory with key
```

### Register Operations
```
XOR A, B        ; A = A XOR B
XOR BC, 0xFF00  ; BC = BC XOR 0xFF00
```

### Memory Operations
```
XOR A, (0x1000) ; A = A XOR memory[0x1000]
XOR (XYZ), 0xFF ; memory[XYZ] = memory[XYZ] XOR 0xFF
```

## Comparison with AND/OR Instructions

- **XOR**: Result bit is 1 if operands differ
- **AND**: Result bit is 1 only if both operands have 1
- **OR**: Result bit is 1 if either operand has 1
- **XOR**: Useful for toggling bits and encryption
- **AND**: Useful for masking and clearing bits
- **OR**: Useful for setting bits

## Conclusion

The XOR instruction provides comprehensive bitwise XOR capabilities across all data sizes and addressing modes. The self-inverse property makes it particularly useful for encryption, value swapping, and bit toggling operations in Continuum 93 programs.

