# NOR Instruction Execution Analysis

## Executive Summary

The NOR instruction implementation in `ExNOR.cs` contains **approximately 30 distinct instruction variants**, supporting bitwise NOR (NOT OR) operations across multiple data sizes (8, 16, 24, 32 bits). NOR performs bitwise logical NOR between operands, which is the complement of OR.

## File Statistics

- **File**: `Emulator/Execution/ExNOR.cs`
- **Total Lines**: 348 lines
- **Instruction Variants**: ~30 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 6 bits of opcode

## Addressing Modes

NOR supports the same addressing modes as AND, OR, XOR, and NAND:

### 1. Register-to-Register Operations

- `NOR r, n` - NOR 8-bit register with immediate value
- `NOR r, r` - NOR 8-bit register with 8-bit register
- `NOR rr, nn` - NOR 16-bit register with immediate value
- `NOR rr, rr` - NOR 16-bit register with 16-bit register
- `NOR rrr, nnn` - NOR 24-bit register with immediate value
- `NOR rrr, rrr` - NOR 24-bit register with 24-bit register
- `NOR rrrr, nnnn` - NOR 32-bit register with immediate value
- `NOR rrrr, rrrr` - NOR 32-bit register with 32-bit register

### 2. Memory-to-Register Operations

- `NOR r, (nnn)` - NOR 8-bit register with value from absolute address
- `NOR r, (rrr)` - NOR 8-bit register with value from register-indirect address
- Similar patterns for 16, 24, 32-bit operations

### 3. Register-to-Memory Operations

- `NOR (nnn), n` - NOR memory at absolute address with immediate value
- `NOR (nnn), r` - NOR memory at absolute address with register
- Similar patterns for register-indirect addressing and all data sizes

## Data Size Variations

The NOR instruction operates on four data sizes:

1. **8-bit operations** (`NOR`): Operates on single bytes
2. **16-bit operations** (`NOR16`): Operates on 16-bit words
3. **24-bit operations** (`NOR24`): Operates on 24-bit values
4. **32-bit operations** (`NOR32`): Operates on 32-bit double words

## Flag Updates

The NOR instruction updates CPU flags through the register methods:
- **Zero Flag**: Set when result equals zero
- **Sign Flag**: Set when result is negative (for signed operations)
- **Parity Flag**: Updated based on result value
- **Carry Flag**: Typically cleared for logical operations

## Implementation Details

### Register NOR Methods

The implementation uses register methods:
- `NOr8Bit(register, value)` - NORs value with 8-bit register
- `NOr16Bit(register, value)` - NORs value with 16-bit register
- `NOr24Bit(register, value)` - NORs value with 24-bit register
- `NOr32Bit(register, value)` - NORs value with 32-bit register

### Memory NOR Methods

For memory operations:
- `NOr8BitMem(address, value)` - NORs memory with value (8-bit)
- `NorMemTo8BitReg(register, address)` - NORs register with memory (8-bit)
- Similar patterns for 16, 24, 32-bit operations

## Bitwise NOR Truth Table

| A | B | A NOR B |
|---|---|---------|
| 0 | 0 | 1       |
| 0 | 1 | 0       |
| 1 | 0 | 0       |
| 1 | 1 | 0       |

## Key Properties

- **NOR is Universal**: Can implement all other logical operations
- **Complement of OR**: `A NOR B = NOT (A OR B)`
- **Self-Sufficient**: NOR alone can build any logic circuit

## Usage Examples

```
NOR A, B        ; A = A NOR B
NOR BC, 0xFF00  ; BC = BC NOR 0xFF00
NOR (XYZ), 0xFF ; memory[XYZ] = memory[XYZ] NOR 0xFF
```

## Comparison with OR Instruction

- **NOR**: Result is complement of OR
- **OR**: Result bit is 1 if either operand has 1
- **NOR**: Result bit is 1 only if both operands are 0
- **NOR**: Universal gate (can implement all logic)

## Conclusion

The NOR instruction provides comprehensive bitwise NOR capabilities across all data sizes and addressing modes. The universal nature of NOR makes it useful for complex bit manipulation operations.

