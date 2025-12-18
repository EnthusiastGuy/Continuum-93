# INV Instruction Execution Analysis

## Executive Summary

The INV (Invert) instruction implementation in `ExINV.cs` provides **4 distinct instruction variants** for inverting (bitwise NOT) register values. INV performs bitwise complement operation, flipping all bits in the register.

## File Statistics

- **File**: `Emulator/Execution/ExINV.cs`
- **Total Lines**: 47 lines
- **Instruction Variants**: 4 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 3 bits of opcode

## Addressing Modes

### Register Inversion Operations

- `INV r` - Invert 8-bit register (bitwise NOT)
- `INV rr` - Invert 16-bit register (bitwise NOT)
- `INV rrr` - Invert 24-bit register (bitwise NOT)
- `INV rrrr` - Invert 32-bit register (bitwise NOT)

## Operation Description

INV performs bitwise complement:
- **Bitwise NOT**: `result = ~value`
- **All bits flipped**: 0 becomes 1, 1 becomes 0
- **In-place operation**: Result stored in same register

## Implementation Details

### Register Inversion Methods

The implementation uses register methods:
- `Inv8BitRegister(regIndex)` - Inverts 8-bit register
- `Inv16BitRegister(regIndex)` - Inverts 16-bit register
- `Inv24BitRegister(regIndex)` - Inverts 24-bit register
- `Inv32BitRegister(regIndex)` - Inverts 32-bit register

### Opcode Encoding

INV uses compact encoding:
1. **Primary Opcode**: Opcode for INV
2. **Secondary Opcode**: Upper 3 bits of opcode (`ldOp >> 5`)
3. **Register Index**: Lower 5 bits of opcode (`ldOp & 0b00011111`)

## Flag Updates

INV updates CPU flags through the register methods:
- **Zero Flag**: Set when result equals zero
- **Sign Flag**: Set when result is negative (for signed operations)
- **Parity Flag**: Updated based on result value
- **Carry Flag**: Typically cleared for logical operations

## Operation Categories Summary

1. **8-bit Operations**: 1 variant
2. **16-bit Operations**: 1 variant
3. **24-bit Operations**: 1 variant
4. **32-bit Operations**: 1 variant

## Usage Examples

```
INV A           ; A = ~A
INV BC          ; BC = ~BC
INV XYZ         ; XYZ = ~XYZ
INV ABCD        ; ABCD = ~ABCD
```

## Bitwise Inversion Examples

- `0x00` → `0xFF` (8-bit)
- `0xFF` → `0x00` (8-bit)
- `0x1234` → `0xEDCB` (16-bit)
- `0x000000` → `0xFFFFFF` (24-bit)

## Mathematical Properties

- **Self-Inverse**: `~(~x) = x`
- **Complement**: All bits flipped
- **Two's Complement**: `~x = -x - 1` (for signed integers)

## Typical Use Cases

1. **Bit Masking**: Invert bit masks
2. **Complement Operations**: Get complement of values
3. **Bit Manipulation**: Flip all bits
4. **Two's Complement**: Calculate negative values

## Comparison with Other Instructions

- **INV**: Bitwise NOT (unary operation)
- **AND/OR/XOR**: Binary logical operations
- **INV**: Operates on single operand
- **AND/OR/XOR**: Operate on two operands

## Conclusion

The INV instruction provides efficient bitwise inversion capabilities for all register sizes. The simple complement operation is essential for bit manipulation and two's complement arithmetic.
