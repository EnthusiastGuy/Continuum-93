# SUB Instruction Execution Analysis

## Executive Summary

The SUB (Subtract) instruction implementation in `ExSUB.cs` contains **approximately 80 distinct instruction variants**, supporting subtraction operations across multiple data sizes (8, 16, 24, 32 bits) and addressing modes. SUB shares the same opcode structure as ADD but performs subtraction instead of addition. It supports comprehensive register-to-register, immediate-to-register, memory-to-register, register-to-memory, and memory-to-memory operations, as well as floating-point operations.

## File Statistics

- **File**: `Emulator/Execution/ExSUB.cs`
- **Total Lines**: 1,033 lines
- **Instruction Variants**: ~80 unique opcodes
- **Implementation Pattern**: Uses switch statement on secondary opcode

## Addressing Modes

The SUB instruction supports the same addressing modes as ADD:

### 1. Register-to-Register Operations
- `SUB r, n` - Subtract immediate 8-bit value from 8-bit register
- `SUB r, r` - Subtract 8-bit register from 8-bit register
- `SUB16 rr, nn` - Subtract immediate 16-bit value from 16-bit register
- `SUB rr, r` - Subtract 8-bit register from 16-bit register
- `SUB rr, rr` - Subtract 16-bit register from 16-bit register
- `SUB24 rrr, nnn` - Subtract immediate 24-bit value from 24-bit register
- `SUB rrr, r` - Subtract 8-bit register from 24-bit register
- `SUB rrr, rr` - Subtract 16-bit register from 24-bit register
- `SUB rrr, rrr` - Subtract 24-bit register from 24-bit register
- `SUB32 rrrr, nnnn` - Subtract immediate 32-bit value from 32-bit register
- `SUB rrrr, r` - Subtract 8-bit register from 32-bit register
- `SUB rrrr, rr` - Subtract 16-bit register from 32-bit register
- `SUB rrrr, rrr` - Subtract 24-bit register from 32-bit register
- `SUB rrrr, rrrr` - Subtract 32-bit register from 32-bit register

### 2. Memory-to-Register Operations
- `SUB r, (nnn)` - Subtract 8-bit value from absolute address from 8-bit register
- `SUB r, (rrr)` - Subtract 8-bit value from register-indirect address from 8-bit register
- `SUB16 rr, (nnn)` - Subtract 16-bit value from absolute address from 16-bit register
- `SUB16 rr, (rrr)` - Subtract 16-bit value from register-indirect address from 16-bit register
- `SUB24 rrr, (nnn)` - Subtract 24-bit value from absolute address from 24-bit register
- `SUB24 rrr, (rrr)` - Subtract 24-bit value from register-indirect address from 24-bit register
- `SUB32 rrrr, (nnn)` - Subtract 32-bit value from absolute address from 32-bit register
- `SUB32 rrrr, (rrr)` - Subtract 32-bit value from register-indirect address from 32-bit register

### 3. Register-to-Memory Operations
- `SUB (nnn), n` - Subtract immediate 8-bit value from memory at absolute address
- `SUB (nnn), r` - Subtract 8-bit register from memory at absolute address
- `SUB16 (nnn), nn` - Subtract immediate 16-bit value from memory at absolute address
- `SUB16 (nnn), r` - Subtract 8-bit register from memory at absolute address (16-bit operation)
- `SUB16 (nnn), rr` - Subtract 16-bit register from memory at absolute address
- `SUB24 (nnn), nnn` - Subtract immediate 24-bit value from memory at absolute address
- `SUB24 (nnn), r` - Subtract 8-bit register from memory at absolute address (24-bit operation)
- `SUB24 (nnn), rr` - Subtract 16-bit register from memory at absolute address (24-bit operation)
- `SUB24 (nnn), rrr` - Subtract 24-bit register from memory at absolute address
- `SUB32 (nnn), nnnn` - Subtract immediate 32-bit value from memory at absolute address
- `SUB32 (nnn), r` - Subtract 8-bit register from memory at absolute address (32-bit operation)
- `SUB32 (nnn), rr` - Subtract 16-bit register from memory at absolute address (32-bit operation)
- `SUB32 (nnn), rrr` - Subtract 24-bit register from memory at absolute address (32-bit operation)
- `SUB32 (nnn), rrrr` - Subtract 32-bit register from memory at absolute address

### 4. Register-Indirect Memory Operations
- `SUB (rrr), n` - Subtract immediate 8-bit value from memory at register-indirect address
- `SUB (rrr), r` - Subtract 8-bit register from memory at register-indirect address
- `SUB16 (rrr), nn` - Subtract immediate 16-bit value from memory at register-indirect address
- `SUB16 (rrr), r` - Subtract 8-bit register from memory at register-indirect address (16-bit operation)
- `SUB16 (rrr), rr` - Subtract 16-bit register from memory at register-indirect address
- `SUB24 (rrr), nnn` - Subtract immediate 24-bit value from memory at register-indirect address
- `SUB24 (rrr), r` - Subtract 8-bit register from memory at register-indirect address (24-bit operation)
- `SUB24 (rrr), rr` - Subtract 16-bit register from memory at register-indirect address (24-bit operation)
- `SUB24 (rrr), rrr` - Subtract 24-bit register from memory at register-indirect address
- `SUB32 (rrr), nnnn` - Subtract immediate 32-bit value from memory at register-indirect address
- `SUB32 (rrr), r` - Subtract 8-bit register from memory at register-indirect address (32-bit operation)
- `SUB32 (rrr), rr` - Subtract 16-bit register from memory at register-indirect address (32-bit operation)
- `SUB32 (rrr), rrr` - Subtract 24-bit register from memory at register-indirect address (32-bit operation)
- `SUB32 (rrr), rrrr` - Subtract 32-bit register from memory at register-indirect address

### 5. Memory-to-Memory Operations
- `SUB (nnn), (nnn)` - Subtract 8-bit value from one memory location from another
- `SUB16 (nnn), (nnn)` - Subtract 16-bit value from one memory location from another
- `SUB24 (nnn), (nnn)` - Subtract 24-bit value from one memory location from another
- `SUB32 (nnn), (nnn)` - Subtract 32-bit value from one memory location from another

## Floating-Point Operations

The SUB instruction supports floating-point operations similar to ADD:

### Float Register Operations
- `SUB fr, fr` - Subtract float register from float register
- `SUB fr, nnn` - Subtract immediate float value (32-bit) from float register
- `SUB fr, r` - Subtract 8-bit integer register (converted to float) from float register
- `SUB fr, rr` - Subtract 16-bit integer register (converted to float) from float register
- `SUB fr, rrr` - Subtract 24-bit integer register (converted to float) from float register
- `SUB fr, rrrr` - Subtract 32-bit integer register (converted to float) from float register
- `SUB fr, (nnn)` - Subtract float value from memory from float register
- `SUB fr, (rrr)` - Subtract float value from register-indirect memory from float register

### Integer Register from Float Operations
- `SUB r, fr` - Subtract float register (converted to 8-bit integer) from 8-bit register
  - If float is negative, performs addition instead
  - Sets overflow flag if conversion exceeds 8-bit range
  - Uses modulo arithmetic for overflow
- `SUB rr, fr` - Subtract float register (converted to 16-bit integer) from 16-bit register
- `SUB rrr, fr` - Subtract float register (converted to 24-bit integer) from 24-bit register
- `SUB rrrr, fr` - Subtract float register (converted to 32-bit integer) from 32-bit register

### Float Memory Operations
- `SUB (nnn), fr` - Subtract float register from memory location
- `SUB (rrr), fr` - Subtract float register from register-indirect memory location

## Data Size Variations

The SUB instruction operates on four data sizes, identical to ADD:

1. **8-bit operations** (`_r_*`): Operates on single bytes
2. **16-bit operations** (`_rr_*` or `SUB16`): Operates on 16-bit words
3. **24-bit operations** (`_rrr_*` or `SUB24`): Operates on 24-bit values
4. **32-bit operations** (`_rrrr_*` or `SUB32`): Operates on 32-bit double words

## Flag Updates

The SUB instruction updates CPU flags through the register methods:
- **Carry Flag**: Set when subtraction results in borrow (underflow)
- **Zero Flag**: Set when result equals zero
- **Sign Flag**: Set when result is negative (for signed operations)
- **Overflow Flag**: Set when float-to-integer conversion exceeds target size

## Implementation Details

### Register Subtraction Methods

The implementation uses register methods that handle flag updates:
- `SubtractFrom8BitRegister(register, value)` - Subtracts value from 8-bit register
- `SubtractFrom16BitRegister(register, value)` - Subtracts value from 16-bit register
- `SubtractFrom24BitRegister(register, value)` - Subtracts value from 24-bit register
- `SubtractFrom32BitRegister(register, value)` - Subtracts value from 32-bit register
- `Sub8BitValues(a, b)` - Returns difference of two 8-bit values (for memory operations)
- `Sub16BitValues(a, b)` - Returns difference of two 16-bit values
- `Sub24BitValues(a, b)` - Returns difference of two 24-bit values
- `Sub32BitValues(a, b)` - Returns difference of two 32-bit values
- `SubFloatValues(a, b)` - Returns difference of two float values

### Float-to-Integer Conversion

When subtracting float registers from integer registers:
1. The float value's sign is checked
2. The absolute value is taken
3. The value is rounded to the nearest integer
4. If the value exceeds the target integer size, modulo arithmetic is applied and overflow flag is set
5. If the original float was negative, addition is performed instead of subtraction

### Code Structure

The implementation uses a **switch statement pattern**:
- Fetches the secondary opcode
- Uses switch statement to dispatch to appropriate handler
- Each case handles a specific instruction variant
- Similar structure to ADD but uses subtraction methods

## Operation Categories Summary

1. **Integer Register Operations**: ~40 variants
   - Register-to-register: 13 variants
   - Immediate-to-register: 4 variants
   - Memory-to-register: 8 variants
   - Register-to-memory: 15 variants

2. **Memory Operations**: ~20 variants
   - Memory-to-memory: 4 variants
   - Register-indirect memory: 16 variants

3. **Floating-Point Operations**: ~20 variants
   - Float register operations: 8 variants
   - Float-to-integer operations: 4 variants
   - Integer-to-float operations: 4 variants
   - Float memory operations: 4 variants

## Comparison with ADD Instruction

- **Same Opcode Structure**: SUB uses the same opcodes as ADD (opcode 3 vs 2)
- **Opposite Operation**: SUB performs subtraction where ADD performs addition
- **Same Addressing Modes**: SUB supports all the same addressing modes as ADD
- **Flag Behavior**: SUB sets carry flag on underflow (borrow) instead of overflow

## Usage Examples

### Basic Subtraction
```
SUB A, 5        ; A = A - 5
SUB BC, DE      ; BC = BC - DE
SUB24 XYZ, 0x123456  ; XYZ = XYZ - 0x123456
```

### Memory Operations
```
SUB A, (0x1000)     ; A = A - memory[0x1000]
SUB (0x2000), A     ; memory[0x2000] = memory[0x2000] - A
SUB (XYZ), 10       ; memory[XYZ] = memory[XYZ] - 10
```

### Floating-Point Operations
```
SUB F0, F1          ; F0 = F0 - F1
SUB F0, 3.14        ; F0 = F0 - 3.14
SUB A, F0           ; A = A - (int)F0 (with overflow handling)
```

## Conclusion

The SUB instruction provides comprehensive subtraction capabilities across all data sizes and addressing modes, mirroring the ADD instruction's structure but performing subtraction operations. It offers extensive floating-point support and memory-to-memory operations, making it a versatile instruction for arithmetic operations in the Continuum 93 instruction set.

