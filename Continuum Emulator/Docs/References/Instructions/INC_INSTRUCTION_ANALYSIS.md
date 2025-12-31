# INC Instruction Execution Analysis

## Executive Summary

The INC (Increment) instruction implementation in `ExINC.cs` provides **12 distinct instruction variants** for incrementing registers and memory locations by 1. INC supports 8, 16, 24, and 32-bit operations on both registers and memory.

## File Statistics

- **File**: `Emulator/Execution/ExINC.cs`
- **Total Lines**: 94 lines
- **Instruction Variants**: 12 unique opcodes
- **Implementation Pattern**: Uses switch statement on secondary opcode

## Addressing Modes

### 1. Register Increment Operations

- `INC r` - Increment 8-bit register by 1
- `INC rr` - Increment 16-bit register by 1
- `INC rrr` - Increment 24-bit register by 1
- `INC rrrr` - Increment 32-bit register by 1

### 2. Memory Increment Operations

#### Register-Indirect Memory
- `INC (rrr)` - Increment 8-bit value at register-indirect address by 1
- `INC16 (rrr)` - Increment 16-bit value at register-indirect address by 1
- `INC24 (rrr)` - Increment 24-bit value at register-indirect address by 1
- `INC32 (rrr)` - Increment 32-bit value at register-indirect address by 1

#### Absolute Address Memory
- `INC (nnn)` - Increment 8-bit value at absolute address by 1
- `INC16 (nnn)` - Increment 16-bit value at absolute address by 1
- `INC24 (nnn)` - Increment 24-bit value at absolute address by 1
- `INC32 (nnn)` - Increment 32-bit value at absolute address by 1

## Data Size Variations

The INC instruction operates on four data sizes:

1. **8-bit operations** (`INC`): Increments single bytes
2. **16-bit operations** (`INC16`): Increments 16-bit words
3. **24-bit operations** (`INC24`): Increments 24-bit values
4. **32-bit operations** (`INC32`): Increments 32-bit double words

## Flag Updates

The INC instruction updates CPU flags through the register methods:
- **Carry Flag**: Set when increment results in overflow (wraps around)
- **Zero Flag**: Set when result equals zero (after wrapping)
- **Sign Flag**: Updated based on result value
- **Overflow Flag**: Set when signed overflow occurs

## Implementation Details

### Register Increment Methods

The implementation uses register methods that handle flag updates:
- `Increment8Bit(register)` - Increments 8-bit register
- `Increment16Bit(register)` - Increments 16-bit register
- `Increment24Bit(register)` - Increments 24-bit register
- `Increment32Bit(register)` - Increments 32-bit register

### Memory Increment Methods

For memory operations:
- `Increment8BitMem(address)` - Increments 8-bit value in memory
- `Increment16BitMem(address)` - Increments 16-bit value in memory
- `Increment24BitMem(address)` - Increments 24-bit value in memory
- `Increment32BitMem(address)` - Increments 32-bit value in memory

### Address Fetching

For register-indirect addressing:
```csharp
byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
uint regValue = computer.CPU.REGS.Get24BitRegister(regIndex);
computer.CPU.REGS.Increment8BitMem(regValue);
```

For absolute addressing:
```csharp
uint address = computer.MEMC.Fetch24();
computer.CPU.REGS.Increment8BitMem(address);
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

## Comparison with DEC Instruction

- **INC**: Increments by 1
- **DEC**: Decrements by 1
- **INC**: Same addressing modes and data sizes
- **INC/DEC**: Complementary operations

## Usage Examples

### Register Increments
```
INC A        ; A = A + 1
INC BC       ; BC = BC + 1
INC24 XYZ    ; XYZ = XYZ + 1
```

### Memory Increments
```
INC (0x1000)     ; memory[0x1000] = memory[0x1000] + 1
INC (XYZ)        ; memory[XYZ] = memory[XYZ] + 1
INC16 (0x2000)  ; 16-bit value at 0x2000 incremented
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

1. **Loop Counters**: Increment/decrement loop indices
2. **Array Indexing**: Increment array pointers
3. **Counters**: Increment various counters
4. **Pointer Arithmetic**: Move pointers forward/backward

## Conclusion

The INC instruction provides efficient increment operations for both registers and memory. The support for multiple data sizes and addressing modes makes it versatile for various programming tasks, particularly loop control and pointer manipulation.

