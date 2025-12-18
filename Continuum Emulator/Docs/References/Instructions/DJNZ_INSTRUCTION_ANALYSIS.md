# DJNZ Instruction Execution Analysis

## Executive Summary

The DJNZ (Decrement and Jump if Not Zero) instruction implementation in `ExDJNZ.cs` provides **approximately 20 distinct instruction variants** for loop control. DJNZ decrements a register or memory location and jumps to a target address if the result is not zero.

## File Statistics

- **File**: `Emulator/Execution/ExDJNZ.cs`
- **Total Lines**: 327 lines
- **Instruction Variants**: ~20 unique opcodes
- **Implementation Pattern**: Uses switch statement on secondary opcode

## Addressing Modes

### 1. Register Decrement and Jump

#### 8-bit Operations
- `DJNZ r, nnn` - Decrement 8-bit register, jump to immediate address if not zero
- `DJNZ r, rrr` - Decrement 8-bit register, jump to address in 24-bit register if not zero

#### 16-bit Operations
- `DJNZ rr, nnn` - Decrement 16-bit register, jump to immediate address if not zero
- `DJNZ rr, rrr` - Decrement 16-bit register, jump to address in 24-bit register if not zero

#### 24-bit Operations
- `DJNZ rrr, nnn` - Decrement 24-bit register, jump to immediate address if not zero
- `DJNZ rrr, rrr` - Decrement 24-bit register, jump to address in 24-bit register if not zero

#### 32-bit Operations
- `DJNZ rrrr, nnn` - Decrement 32-bit register, jump to immediate address if not zero
- `DJNZ rrrr, rrr` - Decrement 32-bit register, jump to address in 24-bit register if not zero

### 2. Memory Decrement and Jump

#### 8-bit Memory Operations
- `DJNZ (nnn), nnn` - Decrement 8-bit value at absolute address, jump to immediate address if not zero
- `DJNZ (nnn), rrr` - Decrement 8-bit value at absolute address, jump to address in register if not zero
- `DJNZ (rrr), nnn` - Decrement 8-bit value at register-indirect address, jump to immediate address if not zero
- `DJNZ (rrr), rrr` - Decrement 8-bit value at register-indirect address, jump to address in register if not zero

#### 16-bit Memory Operations
- `DJNZ16 (nnn), nnn` - Decrement 16-bit value at absolute address, jump to immediate address if not zero
- `DJNZ16 (nnn), rrr` - Decrement 16-bit value at absolute address, jump to address in register if not zero
- `DJNZ16 (rrr), nnn` - Decrement 16-bit value at register-indirect address, jump to immediate address if not zero
- `DJNZ16 (rrr), rrr` - Decrement 16-bit value at register-indirect address, jump to address in register if not zero

#### 24-bit Memory Operations
- `DJNZ24 (nnn), nnn` - Decrement 24-bit value at absolute address, jump to immediate address if not zero
- `DJNZ24 (nnn), rrr` - Decrement 24-bit value at absolute address, jump to address in register if not zero
- `DJNZ24 (rrr), nnn` - Decrement 24-bit value at register-indirect address, jump to immediate address if not zero
- `DJNZ24 (rrr), rrr` - Decrement 24-bit value at register-indirect address, jump to address in register if not zero

#### 32-bit Memory Operations
- `DJNZ32 (nnn), nnn` - Decrement 32-bit value at absolute address, jump to immediate address if not zero
- `DJNZ32 (nnn), rrr` - Decrement 32-bit value at absolute address, jump to address in register if not zero
- `DJNZ32 (rrr), nnn` - Decrement 32-bit value at register-indirect address, jump to immediate address if not zero
- `DJNZ32 (rrr), rrr` - Decrement 32-bit value at register-indirect address, jump to address in register if not zero

## Operation Description

DJNZ performs:
1. **Decrement**: Decrements register or memory value
2. **Check**: Tests if result is not zero
3. **Jump**: If not zero, sets IPO to target address
4. **Continue**: If zero, continues to next instruction

## Implementation Details

### Register DJNZ

For register operations:
```csharp
computer.CPU.REGS.Decrement8Bit(regIndex);
byte regValue = computer.CPU.REGS.Get8BitRegister(regIndex);
if (regValue != 0)
{
    computer.CPU.REGS.IPO = address;
}
```

### Memory DJNZ

For memory operations:
```csharp
computer.CPU.REGS.Decrement8BitMem(targetAddress);
byte value = computer.MEMC.Get8bitFromRAM(targetAddress);
if (value != 0)
{
    computer.CPU.REGS.IPO = jumpAddress;
}
```

## Flag Updates

DJNZ may update CPU flags through register decrement operations, but does not explicitly set flags for the jump condition.

## Operation Categories Summary

1. **Register Operations**: 8 variants (4 data sizes × 2 address types)
2. **Memory Operations**: 12 variants (4 data sizes × 3 address combinations)

## Usage Examples

```
DJNZ A, loop    ; Decrement A, jump to loop if A != 0
DJNZ BC, start  ; Decrement BC, jump to start if BC != 0
DJNZ (XYZ), end ; Decrement memory[XYZ], jump to end if not zero
```

## Typical Use Cases

1. **Loop Control**: Implement counted loops
2. **Iteration**: Iterate a fixed number of times
3. **Counter Decrement**: Decrement and check counter
4. **Loop Optimization**: Efficient loop implementation

## Loop Pattern

DJNZ is commonly used for loops:
```
LD A, 10
loop:
    ; ... loop body ...
    DJNZ A, loop    ; Decrement A, repeat if not zero
```

## Comparison with Other Instructions

- **DJNZ**: Decrement and jump if not zero (combined operation)
- **DEC + JP**: Separate decrement and conditional jump
- **DJNZ**: More efficient for loops
- **DEC + JP**: More flexible but slower

## Conclusion

The DJNZ instruction provides efficient loop control capabilities with support for both register and memory operations. The combined decrement-and-jump operation makes it ideal for implementing counted loops in Continuum 93 programs.
