# DEC Instruction Execution Analysis

## Executive Summary

The DEC (Decrement) instruction implementation in `ExDEC.cs` provides **12 distinct instruction variants** for decrementing registers and memory locations by 1. DEC supports 8, 16, 24, and 32-bit operations on both registers and memory.

## File Statistics

- **File**: `Emulator/Execution/ExDEC.cs`
- **Total Lines**: 94 lines
- **Instruction Variants**: 12 unique opcodes
- **Implementation Pattern**: Uses switch statement on secondary opcode

## Addressing Modes

### 1. Register Decrement Operations

- `DEC r` - Decrement 8-bit register by 1
- `DEC rr` - Decrement 16-bit register by 1
- `DEC rrr` - Decrement 24-bit register by 1
- `DEC rrrr` - Decrement 32-bit register by 1

### 2. Memory Decrement Operations

#### Register-Indirect Memory
- `DEC (rrr)` - Decrement 8-bit value at register-indirect address by 1
- `DEC16 (rrr)` - Decrement 16-bit value at register-indirect address by 1
- `DEC24 (rrr)` - Decrement 24-bit value at register-indirect address by 1
- `DEC32 (rrr)` - Decrement 32-bit value at register-indirect address by 1

#### Absolute Address Memory
- `DEC (nnn)` - Decrement 8-bit value at absolute address by 1
- `DEC16 (nnn)` - Decrement 16-bit value at absolute address by 1
- `DEC24 (nnn)` - Decrement 24-bit value at absolute address by 1
- `DEC32 (nnn)` - Decrement 32-bit value at absolute address by 1

## Data Size Variations

The DEC instruction operates on four data sizes:

1. **8-bit operations** (`DEC`): Decrements single bytes
2. **16-bit operations** (`DEC16`): Decrements 16-bit words
3. **24-bit operations** (`DEC24`): Decrements 24-bit values
4. **32-bit operations** (`DEC32`): Decrements 32-bit double words

## Flag Updates

The DEC instruction updates CPU flags through the register methods:
- **Carry Flag**: Set when decrement results in underflow (wraps around)
- **Zero Flag**: Set when result equals zero
- **Sign Flag**: Updated based on result value
- **Overflow Flag**: Set when signed underflow occurs

## Implementation Details

### Register Decrement Methods

The implementation uses register methods that handle flag updates:
- `Decrement8Bit(register)` - Decrements 8-bit register
- `Decrement16Bit(register)` - Decrements 16-bit register
- `Decrement24Bit(register)` - Decrements 24-bit register
- `Decrement32Bit(register)` - Decrements 32-bit register

### Memory Decrement Methods

For memory operations:
- `Decrement8BitMem(address)` - Decrements 8-bit value in memory
- `Decrement16BitMem(address)` - Decrements 16-bit value in memory
- `Decrement24BitMem(address)` - Decrements 24-bit value in memory
- `Decrement32BitMem(address)` - Decrements 32-bit value in memory

### Address Fetching

For register-indirect addressing:
```csharp
byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
uint regValue = computer.CPU.REGS.Get24BitRegister(regIndex);
computer.CPU.REGS.Decrement8BitMem(regValue);
```

For absolute addressing:
```csharp
uint address = computer.MEMC.Fetch24();
computer.CPU.REGS.Decrement8BitMem(address);
```

## Operation Categories Summary

1. **Register Operations**: 4 variants
   - 8-bit: 1 variant
   - 16-bit: 1 variant
   - 24-bit: 1 variant
   - 32-bit: 1 variant

2. **Memory Operations**: 8 variants
   - Register-indirect: 4 variants (8, 16, 24, 32-bit)
   - Absolute address: 4 variants (8, 16, 24, 32-bit)

## Comparison with INC Instruction

- **DEC**: Decrements by 1
- **INC**: Increments by 1
- **DEC**: Same addressing modes and data sizes
- **DEC/INC**: Complementary operations

## Usage Examples

### Register Decrements
```
DEC A        ; A = A - 1
DEC BC       ; BC = BC - 1
DEC24 XYZ    ; XYZ = XYZ - 1
```

### Memory Decrements
```
DEC (0x1000)     ; memory[0x1000] = memory[0x1000] - 1
DEC (XYZ)        ; memory[XYZ] = memory[XYZ] - 1
DEC16 (0x2000)  ; 16-bit value at 0x2000 decremented
```

### Loop Counters
```
LD A, 10
loop:
    ; ... loop body ...
    DEC A
    JP NZ, loop  ; Continue if A != 0
```

## Typical Use Cases

1. **Loop Counters**: Decrement loop indices (common pattern)
2. **Array Indexing**: Decrement array pointers
3. **Counters**: Decrement various counters
4. **Pointer Arithmetic**: Move pointers backward

## Conclusion

The DEC instruction provides efficient decrement operations for both registers and memory. The support for multiple data sizes and addressing modes makes it versatile for various programming tasks, particularly loop control and pointer manipulation. DEC is commonly used in loop constructs where a counter is decremented until it reaches zero.

