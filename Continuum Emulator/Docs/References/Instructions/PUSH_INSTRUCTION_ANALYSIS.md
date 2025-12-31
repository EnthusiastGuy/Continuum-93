# PUSH Instruction Execution Analysis

## Executive Summary

The PUSH instruction implementation in `ExPUSH.cs` provides **approximately 15 distinct instruction variants** for pushing values onto the register stack. PUSH supports pushing registers (8, 16, 24, 32-bit), float registers, register ranges, and memory values. The instruction includes stack overflow detection and error handling.

## File Statistics

- **File**: `Emulator/Execution/ExPUSH.cs`
- **Total Lines**: 355 lines
- **Instruction Variants**: ~15 unique opcodes
- **Implementation Pattern**: Uses switch statement on secondary opcode

## Addressing Modes

### 1. Register Push Operations

- `PUSH r` - Push 8-bit register onto stack
- `PUSH rr` - Push 16-bit register onto stack
- `PUSH rrr` - Push 24-bit register onto stack
- `PUSH rrrr` - Push 32-bit register onto stack

### 2. Register Range Push Operations

- `PUSH r, r` - Push range of 8-bit registers (from first to second, inclusive)
  - Registers are pushed in order (A to Z, wrapping)
  - Example: `PUSH A, C` pushes A, B, C

### 3. Float Register Push Operations

- `PUSH fr` - Push float register onto stack (4 bytes)
- `PUSH fr, fr` - Push range of float registers (F0 to F15, wrapping)
  - Each float register takes 4 bytes on stack

### 4. Memory Push Operations

#### Absolute Address Memory
- `PUSH (nnn)` - Push 8-bit value from absolute address
- `PUSH16 (nnn)` - Push 16-bit value from absolute address
- `PUSH24 (nnn)` - Push 24-bit value from absolute address
- `PUSH32 (nnn)` - Push 32-bit value from absolute address

#### Register-Indirect Memory
- `PUSH (rrr)` - Push 8-bit value from register-indirect address
- `PUSH16 (rrr)` - Push 16-bit value from register-indirect address
- `PUSH24 (rrr)` - Push 24-bit value from register-indirect address
- `PUSH32 (rrr)` - Push 32-bit value from register-indirect address

## Stack Management

### Stack Pointer

PUSH uses the Stack Pointer for Registers (SPR):
- **SPR**: Points to next available stack position
- **Stack Growth**: Stack grows upward (SPR increments)
- **Stack Location**: Uses RSRAM (Register Stack RAM)

### Stack Overflow Detection

PUSH checks for stack overflow before pushing:
```csharp
if (_computer.CPU.REGS.SPR >= _computer.MEMC.RSRAM.Size - requiredSpace)
{
    // Handle stack overflow
    Log.WriteLine($"Stack overflow...");
    computer.MEMC.ResetAllStacks();
    computer.CPU.REGS.IPO = computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_HANDLER_ADDRESS];
    computer.MEMC.HMEM[computer.MEMC.HMEM.ERROR_ID] = SystemMessages.ERR_STACK_OVERFLOW;
    return;
}
```

### Stack Space Requirements

- **8-bit value**: 1 byte
- **16-bit value**: 2 bytes
- **24-bit value**: 3 bytes
- **32-bit value**: 4 bytes
- **Float register**: 4 bytes

## Implementation Details

### Register Push Methods

- `Set8BitToRegStack(SPR, value)` - Pushes 8-bit value, increments SPR by 1
- `Set16BitToRegStack(SPR, value)` - Pushes 16-bit value, increments SPR by 2
- `Set24BitToRegStack(SPR, value)` - Pushes 24-bit value, increments SPR by 3
- `Set32BitToRegStack(SPR, value)` - Pushes 32-bit value, increments SPR by 4

### Register Range Calculation

For register ranges, the distance is calculated:
```csharp
private static byte CalculateRegisterDistance(byte start, byte end)
{
    return (byte)(((end - start + 26) % 26) + 1);
}
```

This handles wrapping from Z to A (26 registers total).

### Float Register Push

Float registers are converted to 32-bit integers before pushing:
```csharp
float floatValue = _computer.CPU.FREGS.GetRegister(fRegIndex);
uint floatUintValue = FloatPointUtils.FloatToUint(floatValue);
_computer.MEMC.Set32BitToRegStack(_computer.CPU.REGS.SPR, floatUintValue);
_computer.CPU.REGS.SPR += 4;
```

## Operation Categories Summary

1. **Single Register Push**: 4 variants (8, 16, 24, 32-bit)
2. **Register Range Push**: 1 variant (8-bit registers)
3. **Float Register Push**: 2 variants (single, range)
4. **Memory Push**: 8 variants (4 data sizes × 2 addressing modes)

## Error Handling

When stack overflow occurs:
1. Error message logged with register name and instruction pointer
2. All stacks reset
3. Instruction pointer set to error handler address
4. Error ID set to `ERR_STACK_OVERFLOW`
5. Execution continues at error handler

## Usage Examples

### Single Register Push
```
PUSH A          ; Push register A (1 byte)
PUSH BC         ; Push register BC (2 bytes)
PUSH24 XYZ      ; Push register XYZ (3 bytes)
PUSH32 ABCD     ; Push register ABCD (4 bytes)
```

### Register Range Push
```
PUSH A, C       ; Push A, B, C (3 bytes total)
PUSH X, Z       ; Push X, Y, Z (3 bytes total, wraps)
```

### Float Register Push
```
PUSH F0         ; Push float register F0 (4 bytes)
PUSH F0, F2     ; Push F0, F1, F2 (12 bytes total)
```

### Memory Push
```
PUSH (0x1000)   ; Push memory[0x1000] (1 byte)
PUSH16 (XYZ)    ; Push 16-bit value from memory[XYZ]
```

## Comparison with POP Instruction

- **PUSH**: Adds values to stack (SPR increments)
- **POP**: Removes values from stack (SPR decrements)
- **PUSH**: Checks for overflow
- **POP**: Checks for underflow
- **PUSH/POP**: LIFO (Last In, First Out) stack operations

## Conclusion

The PUSH instruction provides comprehensive stack push capabilities with support for all data sizes, register ranges, and memory addressing. The stack overflow detection ensures program safety and enables error recovery.

