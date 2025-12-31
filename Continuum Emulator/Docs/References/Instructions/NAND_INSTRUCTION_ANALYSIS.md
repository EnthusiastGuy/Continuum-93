# NAND Instruction Execution Analysis

## Executive Summary

The NAND instruction implementation in `ExNAND.cs` contains **approximately 30 distinct instruction variants**, supporting bitwise NAND (NOT AND) operations across multiple data sizes (8, 16, 24, 32 bits). NAND performs bitwise logical NAND between operands, which is the complement of AND.

## File Statistics

- **File**: `Emulator/Execution/ExNAND.cs`
- **Total Lines**: 348 lines
- **Instruction Variants**: ~30 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 6 bits of opcode

## Addressing Modes

NAND supports the same addressing modes as AND, OR, and XOR:

### 1. Register-to-Register Operations

- `NAND r, n` - NAND 8-bit register with immediate value
- `NAND r, r` - NAND 8-bit register with 8-bit register
- `NAND rr, nn` - NAND 16-bit register with immediate value
- `NAND rr, rr` - NAND 16-bit register with 16-bit register
- `NAND rrr, nnn` - NAND 24-bit register with immediate value
- `NAND rrr, rrr` - NAND 24-bit register with 24-bit register
- `NAND rrrr, nnnn` - NAND 32-bit register with immediate value
- `NAND rrrr, rrrr` - NAND 32-bit register with 32-bit register

### 2. Memory-to-Register Operations

- `NAND r, (nnn)` - NAND 8-bit register with value from absolute address
- `NAND r, (rrr)` - NAND 8-bit register with value from register-indirect address
- Similar patterns for 16, 24, 32-bit operations

### 3. Register-to-Memory Operations

- `NAND (nnn), n` - NAND memory at absolute address with immediate value
- `NAND (nnn), r` - NAND memory at absolute address with register
- Similar patterns for register-indirect addressing and all data sizes

## Data Size Variations

The NAND instruction operates on four data sizes:

1. **8-bit operations** (`NAND`): Operates on single bytes
2. **16-bit operations** (`NAND16`): Operates on 16-bit words
3. **24-bit operations** (`NAND24`): Operates on 24-bit values
4. **32-bit operations** (`NAND32`): Operates on 32-bit double words

## Flag Updates

The NAND instruction updates CPU flags through the register methods:
- **Zero Flag**: Set when result equals zero
- **Sign Flag**: Set when result is negative (for signed operations)
- **Parity Flag**: Updated based on result value
- **Carry Flag**: Typically cleared for logical operations

## Implementation Details

### Register NAND Methods

The implementation uses register methods:
- `NAnd8Bit(register, value)` - NANDs value with 8-bit register
- `NAnd16Bit(register, value)` - NANDs value with 16-bit register
- `NAnd24Bit(register, value)` - NANDs value with 24-bit register
- `NAnd32Bit(register, value)` - NANDs value with 32-bit register

### Memory NAND Methods

For memory operations:
- `NAnd8BitMem(address, value)` - NANDs memory with value (8-bit)
- `NandMemTo8BitReg(register, address)` - NANDs register with memory (8-bit)
- Similar patterns for 16, 24, 32-bit operations

## Bitwise NAND Truth Table

| A | B | A NAND B |
|---|---|----------|
| 0 | 0 | 1        |
| 0 | 1 | 1        |
| 1 | 0 | 1        |
| 1 | 1 | 0        |

## Key Properties

- **NAND is Universal**: Can implement all other logical operations
- **Complement of AND**: `A NAND B = NOT (A AND B)`
- **Self-Sufficient**: NAND alone can build any logic circuit

## Usage Examples

```
NAND A, B        ; A = A NAND B
NAND BC, 0xFF00  ; BC = BC NAND 0xFF00
NAND (XYZ), 0xFF ; memory[XYZ] = memory[XYZ] NAND 0xFF
```

## Comparison with AND Instruction

- **NAND**: Result is complement of AND
- **AND**: Result bit is 1 only if both operands have 1
- **NAND**: Result bit is 0 only if both operands have 1
- **NAND**: Universal gate (can implement all logic)

## Conclusion

The NAND instruction provides comprehensive bitwise NAND capabilities across all data sizes and addressing modes. The universal nature of NAND makes it useful for complex bit manipulation operations.

