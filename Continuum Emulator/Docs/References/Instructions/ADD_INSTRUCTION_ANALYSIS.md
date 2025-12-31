# ADD Instruction Execution Analysis

## Executive Summary

The ADD (Add) instruction implementation in `ExADD.cs` contains **approximately 80 distinct instruction variants**, supporting addition operations across multiple data sizes (8, 16, 24, 32 bits) and addressing modes. Unlike the LD instruction, ADD does not support indexing, but it does support comprehensive register-to-register, immediate-to-register, memory-to-register, register-to-memory, and memory-to-memory operations, as well as floating-point operations.

## File Statistics

- **File**: `Emulator/Execution/ExADD.cs`
- **Total Lines**: 1,310 lines
- **Instruction Variants**: ~80 unique opcodes
- **Dispatch Table**: Uses a 256-element array of `Action<Computer>` delegates for O(1) lookup

## Addressing Modes

The ADD instruction supports the following addressing modes:

### 1. Register-to-Register Operations
- `ADD r, n` - Add immediate 8-bit value to 8-bit register
- `ADD r, r` - Add 8-bit register to 8-bit register
- `ADD16 rr, nn` - Add immediate 16-bit value to 16-bit register
- `ADD rr, r` - Add 8-bit register to 16-bit register
- `ADD rr, rr` - Add 16-bit register to 16-bit register
- `ADD24 rrr, nnn` - Add immediate 24-bit value to 24-bit register
- `ADD rrr, r` - Add 8-bit register to 24-bit register
- `ADD rrr, rr` - Add 16-bit register to 24-bit register
- `ADD rrr, rrr` - Add 24-bit register to 24-bit register
- `ADD32 rrrr, nnnn` - Add immediate 32-bit value to 32-bit register
- `ADD rrrr, r` - Add 8-bit register to 32-bit register
- `ADD rrrr, rr` - Add 16-bit register to 32-bit register
- `ADD rrrr, rrr` - Add 24-bit register to 32-bit register
- `ADD rrrr, rrrr` - Add 32-bit register to 32-bit register

### 2. Memory-to-Register Operations
- `ADD r, (nnn)` - Add 8-bit value from absolute address to 8-bit register
- `ADD r, (rrr)` - Add 8-bit value from register-indirect address to 8-bit register
- `ADD16 rr, (nnn)` - Add 16-bit value from absolute address to 16-bit register
- `ADD16 rr, (rrr)` - Add 16-bit value from register-indirect address to 16-bit register
- `ADD24 rrr, (nnn)` - Add 24-bit value from absolute address to 24-bit register
- `ADD24 rrr, (rrr)` - Add 24-bit value from register-indirect address to 24-bit register
- `ADD32 rrrr, (nnn)` - Add 32-bit value from absolute address to 32-bit register
- `ADD32 rrrr, (rrr)` - Add 32-bit value from register-indirect address to 32-bit register

### 3. Register-to-Memory Operations
- `ADD (nnn), n` - Add immediate 8-bit value to memory at absolute address
- `ADD (nnn), r` - Add 8-bit register to memory at absolute address
- `ADD16 (nnn), nn` - Add immediate 16-bit value to memory at absolute address
- `ADD16 (nnn), r` - Add 8-bit register to memory at absolute address (16-bit operation)
- `ADD16 (nnn), rr` - Add 16-bit register to memory at absolute address
- `ADD24 (nnn), nnn` - Add immediate 24-bit value to memory at absolute address
- `ADD24 (nnn), r` - Add 8-bit register to memory at absolute address (24-bit operation)
- `ADD24 (nnn), rr` - Add 16-bit register to memory at absolute address (24-bit operation)
- `ADD24 (nnn), rrr` - Add 24-bit register to memory at absolute address
- `ADD32 (nnn), nnnn` - Add immediate 32-bit value to memory at absolute address
- `ADD32 (nnn), r` - Add 8-bit register to memory at absolute address (32-bit operation)
- `ADD32 (nnn), rr` - Add 16-bit register to memory at absolute address (32-bit operation)
- `ADD32 (nnn), rrr` - Add 24-bit register to memory at absolute address (32-bit operation)
- `ADD32 (nnn), rrrr` - Add 32-bit register to memory at absolute address

### 4. Register-Indirect Memory Operations
- `ADD (rrr), n` - Add immediate 8-bit value to memory at register-indirect address
- `ADD (rrr), r` - Add 8-bit register to memory at register-indirect address
- `ADD16 (rrr), nn` - Add immediate 16-bit value to memory at register-indirect address
- `ADD16 (rrr), r` - Add 8-bit register to memory at register-indirect address (16-bit operation)
- `ADD16 (rrr), rr` - Add 16-bit register to memory at register-indirect address
- `ADD24 (rrr), nnn` - Add immediate 24-bit value to memory at register-indirect address
- `ADD24 (rrr), r` - Add 8-bit register to memory at register-indirect address (24-bit operation)
- `ADD24 (rrr), rr` - Add 16-bit register to memory at register-indirect address (24-bit operation)
- `ADD24 (rrr), rrr` - Add 24-bit register to memory at register-indirect address
- `ADD32 (rrr), nnnn` - Add immediate 32-bit value to memory at register-indirect address
- `ADD32 (rrr), r` - Add 8-bit register to memory at register-indirect address (32-bit operation)
- `ADD32 (rrr), rr` - Add 16-bit register to memory at register-indirect address (32-bit operation)
- `ADD32 (rrr), rrr` - Add 24-bit register to memory at register-indirect address (32-bit operation)
- `ADD32 (rrr), rrrr` - Add 32-bit register to memory at register-indirect address

### 5. Memory-to-Memory Operations
- `ADD (nnn), (nnn)` - Add 8-bit value from one memory location to another
- `ADD16 (nnn), (nnn)` - Add 16-bit value from one memory location to another
- `ADD24 (nnn), (nnn)` - Add 24-bit value from one memory location to another
- `ADD32 (nnn), (nnn)` - Add 32-bit value from one memory location to another

## Floating-Point Operations

The ADD instruction supports floating-point operations:

### Float Register Operations
- `ADD fr, fr` - Add float register to float register
- `ADD fr, nnn` - Add immediate float value (32-bit) to float register
- `ADD fr, r` - Add 8-bit integer register (converted to float) to float register
- `ADD fr, rr` - Add 16-bit integer register (converted to float) to float register
- `ADD fr, rrr` - Add 24-bit integer register (converted to float) to float register
- `ADD fr, rrrr` - Add 32-bit integer register (converted to float) to float register
- `ADD fr, (nnn)` - Add float value from memory to float register
- `ADD fr, (rrr)` - Add float value from register-indirect memory to float register

### Integer Register from Float Operations
- `ADD r, fr` - Add float register (converted to 8-bit integer) to 8-bit register
  - If float is negative, performs subtraction instead
  - Sets overflow flag if conversion exceeds 8-bit range
  - Uses modulo arithmetic for overflow
- `ADD rr, fr` - Add float register (converted to 16-bit integer) to 16-bit register
- `ADD rrr, fr` - Add float register (converted to 24-bit integer) to 24-bit register
- `ADD rrrr, fr` - Add float register (converted to 32-bit integer) to 32-bit register

### Float Memory Operations
- `ADD (nnn), fr` - Add float register to memory location
- `ADD (rrr), fr` - Add float register to register-indirect memory location

## Data Size Variations

The ADD instruction operates on four data sizes:

1. **8-bit operations** (`_r_*`): Operates on single bytes
2. **16-bit operations** (`_rr_*` or `ADD16`): Operates on 16-bit words
3. **24-bit operations** (`_rrr_*` or `ADD24`): Operates on 24-bit values
4. **32-bit operations** (`_rrrr_*` or `ADD32`): Operates on 32-bit double words

## Flag Updates

The ADD instruction updates CPU flags through the register methods:
- **Carry Flag**: Set when addition results in overflow beyond the data size
- **Zero Flag**: Set when result equals zero
- **Sign Flag**: Set when result is negative (for signed operations)
- **Overflow Flag**: Set when float-to-integer conversion exceeds target size

## Implementation Details

### Register Addition Methods

The implementation uses register methods that handle flag updates:
- `AddTo8BitRegister(register, value)` - Adds value to 8-bit register
- `AddTo16BitRegister(register, value)` - Adds value to 16-bit register
- `AddTo24BitRegister(register, value)` - Adds value to 24-bit register
- `AddTo32BitRegister(register, value)` - Adds value to 32-bit register
- `Add8BitValues(a, b)` - Returns sum of two 8-bit values (for memory operations)
- `Add16BitValues(a, b)` - Returns sum of two 16-bit values
- `Add24BitValues(a, b)` - Returns sum of two 24-bit values
- `Add32BitValues(a, b)` - Returns sum of two 32-bit values
- `AddFloatValues(a, b)` - Returns sum of two float values

### Float-to-Integer Conversion

When adding float registers to integer registers:
1. The float value's sign is checked
2. The absolute value is taken
3. The value is rounded to the nearest integer
4. If the value exceeds the target integer size, modulo arithmetic is applied and overflow flag is set
5. If the original float was negative, subtraction is performed instead of addition

### Code Structure

The implementation uses a **dispatch table pattern**:
- 256-element array of `Action<Computer>` delegates
- Each instruction variant is assigned to a specific opcode slot
- Fast O(1) lookup for instruction execution
- All variants are defined in `BuildDispatchTable()` method
- The `Process()` method fetches the opcode and dispatches to the appropriate handler

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

## Comparison with LD Instruction

- **No Indexing Support**: Unlike LD, ADD does not support indexed addressing modes
- **Simpler Addressing**: ADD uses only immediate addresses (`nnn`) and register-indirect addresses (`(rrr)`)
- **Memory-to-Memory**: ADD supports memory-to-memory operations, which LD also supports
- **Float Support**: ADD has comprehensive floating-point support similar to LD

## Usage Examples

### Basic Addition
```
ADD A, 5        ; A = A + 5
ADD BC, DE      ; BC = BC + DE
ADD24 XYZ, 0x123456  ; XYZ = XYZ + 0x123456
```

### Memory Operations
```
ADD A, (0x1000)     ; A = A + memory[0x1000]
ADD (0x2000), A     ; memory[0x2000] = memory[0x2000] + A
ADD (XYZ), 10       ; memory[XYZ] = memory[XYZ] + 10
```

### Floating-Point Operations
```
ADD F0, F1          ; F0 = F0 + F1
ADD F0, 3.14        ; F0 = F0 + 3.14
ADD A, F0           ; A = A + (int)F0 (with overflow handling)
```

## Conclusion

The ADD instruction provides comprehensive addition capabilities across all data sizes and addressing modes. While it lacks the indexing support of the LD instruction, it offers extensive floating-point support and memory-to-memory operations. The dispatch table implementation ensures fast execution while maintaining code clarity and maintainability.

