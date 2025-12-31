# MUL Instruction Execution Analysis

## Executive Summary

The MUL (Multiply) instruction implementation in `ExMUL.cs` contains **approximately 30 distinct instruction variants**, supporting multiplication operations across multiple data sizes (8, 16, 24, 32 bits) and floating-point operations. MUL uses a compact encoding scheme where register indices are encoded in the opcode itself, allowing for efficient instruction encoding.

## File Statistics

- **File**: `Emulator/Execution/ExMUL.cs`
- **Total Lines**: 431 lines
- **Instruction Variants**: ~30 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 6 bits of opcode

## Addressing Modes

The MUL instruction supports register-to-register and immediate-to-register operations:

### 1. Integer Register Operations

#### 8-bit Operations
- `MUL r, n` - Multiply 8-bit register by immediate value (up to 16-bit)
  - Register index encoded in opcode: `((ldOp << 3) & 0b00011111) + (mixedReg >> 5)`
  - Immediate value: `((mixedReg & 0b00011111) << 8) + next_byte`
  - Result stored in register (low 8 bits)
  - Carry flag set if product exceeds 8 bits
- `MUL r, r` - Multiply 8-bit register by 8-bit register
  - Both register indices encoded in opcode
  - Result stored in first register
  - Carry flag set if product exceeds 8 bits

#### 16-bit Operations
- `MUL rr, n` - Multiply 16-bit register by immediate value
- `MUL rr, r` - Multiply 16-bit register by 8-bit register
- `MUL rr, rr` - Multiply 16-bit register by 16-bit register
  - Result stored in first register (low 16 bits)
  - Carry flag set if product exceeds 16 bits

#### 24-bit Operations
- `MUL rrr, n` - Multiply 24-bit register by immediate value
- `MUL rrr, r` - Multiply 24-bit register by 8-bit register
- `MUL rrr, rr` - Multiply 24-bit register by 16-bit register
- `MUL rrr, rrr` - Multiply 24-bit register by 24-bit register
  - Result stored in first register
  - Carry flag set if product exceeds 24 bits (0xFFFFFF)

#### 32-bit Operations
- `MUL rrrr, n` - Multiply 32-bit register by immediate value
- `MUL rrrr, r` - Multiply 32-bit register by 8-bit register
- `MUL rrrr, rr` - Multiply 32-bit register by 16-bit register
- `MUL rrrr, rrr` - Multiply 32-bit register by 24-bit register
- `MUL rrrr, rrrr` - Multiply 32-bit register by 32-bit register
  - Result stored in first register (low 32 bits)
  - Uses 64-bit intermediate for overflow detection
  - Carry flag set if product exceeds 32 bits

### 2. Floating-Point Operations

#### Float Register Operations
- `MUL fr, fr` - Multiply float register by float register
  - Register indices encoded in single byte: `(mixedReg >> 4)` and `(mixedReg & 0b00001111)`
  - Result stored in first float register
  - Sign flag set if result is negative
- `MUL fr, nnn` - Multiply float register by immediate float value (32-bit)
- `MUL fr, r` - Multiply float register by 8-bit integer register (converted to float)
- `MUL fr, rr` - Multiply float register by 16-bit integer register (converted to float)
- `MUL fr, rrr` - Multiply float register by 24-bit integer register (converted to float)
- `MUL fr, rrrr` - Multiply float register by 32-bit integer register (converted to float)
- `MUL fr, (nnn)` - Multiply float register by float value from memory
- `MUL fr, (rrr)` - Multiply float register by float value from register-indirect memory

#### Integer Register from Float Operations
- `MUL r, fr` - Multiply 8-bit register by float register
  - Float value converted to integer and rounded
  - Result stored in integer register
- `MUL rr, fr` - Multiply 16-bit register by float register
- `MUL rrr, fr` - Multiply 24-bit register by float register
  - Uses `Math.Abs()` to ensure positive result
- `MUL rrrr, fr` - Multiply 32-bit register by float register
  - Uses `Math.Abs()` to ensure positive result

#### Float Memory Operations
- `MUL (nnn), fr` - Multiply float value in memory by float register
- `MUL (rrr), fr` - Multiply float value in register-indirect memory by float register

## Opcode Encoding

MUL uses a compact encoding scheme:

1. **Primary Opcode**: Opcode 5 (MUL)
2. **Secondary Opcode**: Upper 6 bits of next byte (`ldOp >> 2`)
3. **Register Encoding**: 
   - For integer operations: Register indices encoded in opcode bits
   - First register: `((ldOp << 3) & 0b00011111) + (mixedReg >> 5)`
   - Second register: `(mixedReg & 0b00011111)`
4. **Immediate Values**: Follow register encoding byte

## Flag Updates

- **Carry Flag**: Set when multiplication result exceeds the target register size
  - 8-bit: Set if product > 0xFF
  - 16-bit: Set if product > 0xFFFF
  - 24-bit: Set if product > 0xFFFFFF
  - 32-bit: Set if product > 0xFFFFFFFF
- **Sign Flag**: Set for floating-point operations when result is negative
- **Zero Flag**: Updated through register operations (if result is zero)

## Implementation Details

### Register Index Extraction

The implementation uses bit manipulation to extract register indices from the opcode:
```csharp
byte mixedReg = computer.MEMC.Fetch();
byte multiplicandReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
byte multiplierReg = (byte)(mixedReg & 0b00011111);
```

This allows encoding two register indices in a single byte, making instructions more compact.

### Overflow Handling

For integer operations:
- The product is calculated in a larger type (uint for 8/16/24-bit, ulong for 32-bit)
- Only the low-order bits are stored in the target register
- Carry flag indicates if overflow occurred

For floating-point operations:
- Standard floating-point multiplication
- Sign flag indicates if result is negative
- No overflow flag (floating-point handles infinity)

### Float-to-Integer Conversion

When multiplying integer registers by float registers:
1. Integer value is converted to float
2. Multiplication performed in float
3. Result rounded to nearest integer
4. For 24-bit and 32-bit operations, absolute value is taken (ensures positive result)

## Operation Categories Summary

1. **Integer Register Operations**: ~12 variants
   - 8-bit: 2 variants (immediate, register)
   - 16-bit: 3 variants (immediate, 8-bit reg, 16-bit reg)
   - 24-bit: 4 variants (immediate, 8-bit reg, 16-bit reg, 24-bit reg)
   - 32-bit: 5 variants (immediate, 8-bit reg, 16-bit reg, 24-bit reg, 32-bit reg)

2. **Floating-Point Operations**: ~18 variants
   - Float register operations: 8 variants
   - Integer-to-float operations: 4 variants
   - Float-to-integer operations: 4 variants
   - Float memory operations: 2 variants

## Comparison with ADD/SUB

- **Compact Encoding**: MUL uses more compact encoding than ADD/SUB
- **No Memory-to-Memory**: MUL does not support memory-to-memory operations
- **No Memory-to-Register**: MUL does not support loading from memory and multiplying
- **Register-Only**: MUL focuses on register operations for performance

## Usage Examples

### Basic Multiplication
```
MUL A, 5        ; A = A * 5
MUL BC, DE      ; BC = BC * DE
MUL24 XYZ, 100  ; XYZ = XYZ * 100
```

### Floating-Point Operations
```
MUL F0, F1          ; F0 = F0 * F1
MUL F0, 3.14        ; F0 = F0 * 3.14
MUL A, F0           ; A = A * (int)F0 (rounded)
```

## Conclusion

The MUL instruction provides efficient multiplication capabilities with compact encoding. While it lacks the comprehensive memory addressing modes of ADD/SUB, it offers excellent performance for register-based arithmetic operations and comprehensive floating-point support.

