# XNOR Instruction Execution Analysis

## Executive Summary

The XNOR instruction implementation in `ExXNOR.cs` contains **approximately 30 distinct instruction variants**, supporting bitwise XNOR (NOT XOR, or equivalence) operations across multiple data sizes (8, 16, 24, 32 bits). XNOR performs bitwise exclusive NOR between operands, setting each bit to 1 if the corresponding bits are equal.

## File Statistics

- **File**: `Emulator/Execution/ExXNOR.cs`
- **Total Lines**: 348 lines
- **Instruction Variants**: ~30 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 6 bits of opcode

## Addressing Modes

XNOR supports the same addressing modes as AND, OR, XOR, NAND, and NOR:

### 1. Register-to-Register Operations

- `XNOR r, n` - XNOR 8-bit register with immediate value
- `XNOR r, r` - XNOR 8-bit register with 8-bit register
- `XNOR rr, nn` - XNOR 16-bit register with immediate value
- `XNOR rr, rr` - XNOR 16-bit register with 16-bit register
- `XNOR rrr, nnn` - XNOR 24-bit register with immediate value
- `XNOR rrr, rrr` - XNOR 24-bit register with 24-bit register
- `XNOR rrrr, nnnn` - XNOR 32-bit register with immediate value
- `XNOR rrrr, rrrr` - XNOR 32-bit register with 32-bit register

### 2. Memory-to-Register Operations

- `XNOR r, (nnn)` - XNOR 8-bit register with value from absolute address
- `XNOR r, (rrr)` - XNOR 8-bit register with value from register-indirect address
- Similar patterns for 16, 24, 32-bit operations

### 3. Register-to-Memory Operations

- `XNOR (nnn), n` - XNOR memory at absolute address with immediate value
- `XNOR (nnn), r` - XNOR memory at absolute address with register
- Similar patterns for register-indirect addressing and all data sizes

## Data Size Variations

The XNOR instruction operates on four data sizes:

1. **8-bit operations** (`XNOR`): Operates on single bytes
2. **16-bit operations** (`XNOR16`): Operates on 16-bit words
3. **24-bit operations** (`XNOR24`): Operates on 24-bit values
4. **32-bit operations** (`XNOR32`): Operates on 32-bit double words

## Flag Updates

The XNOR instruction updates CPU flags through the register methods:
- **Zero Flag**: Set when result equals zero
- **Sign Flag**: Set when result is negative (for signed operations)
- **Parity Flag**: Updated based on result value
- **Carry Flag**: Typically cleared for logical operations

## Implementation Details

### Register XNOR Methods

The implementation uses register methods:
- `XNor8Bit(register, value)` - XNORs value with 8-bit register
- `XNor16Bit(register, value)` - XNORs value with 16-bit register
- `XNor24Bit(register, value)` - XNORs value with 24-bit register
- `XNor32Bit(register, value)` - XNORs value with 32-bit register

### Memory XNOR Methods

For memory operations:
- `XNor8BitMem(address, value)` - XNORs memory with value (8-bit)
- `XNorMemTo8BitReg(register, address)` - XNORs register with memory (8-bit)
- Similar patterns for 16, 24, 32-bit operations

## Bitwise XNOR Truth Table

| A | B | A XNOR B |
|---|---|----------|
| 0 | 0 | 1        |
| 0 | 1 | 0        |
| 1 | 0 | 0        |
| 1 | 1 | 1        |

## Key Properties

- **Equivalence Operation**: XNOR is true when operands are equal
- **Complement of XOR**: `A XNOR B = NOT (A XOR B)`
- **Equality Check**: Useful for comparing bit patterns

## Usage Examples

```
XNOR A, B        ; A = A XNOR B (equivalence)
XNOR BC, 0xFF00  ; BC = BC XNOR 0xFF00
XNOR (XYZ), 0xFF ; memory[XYZ] = memory[XYZ] XNOR 0xFF
```

## Comparison with XOR Instruction

- **XNOR**: Result is complement of XOR
- **XOR**: Result bit is 1 if operands differ
- **XNOR**: Result bit is 1 if operands are equal
- **XNOR**: Useful for equality checking

## Conclusion

The XNOR instruction provides comprehensive bitwise XNOR capabilities across all data sizes and addressing modes. The equivalence nature makes it useful for bit pattern comparison and equality checking operations.

