# POP Instruction Execution Analysis

## Executive Summary

The POP instruction implementation in `ExPOP.cs` provides **approximately 15 distinct instruction variants** for popping values from the register stack. POP supports popping to registers (8, 16, 24, 32-bit), float registers, register ranges, and memory locations. The instruction includes stack underflow detection and error handling.

## File Statistics

- **File**: `Emulator/Execution/ExPOP.cs`
- **Total Lines**: 314 lines
- **Instruction Variants**: ~15 unique opcodes
- **Implementation Pattern**: Uses switch statement on secondary opcode

## Addressing Modes

### 1. Register Pop Operations

- `POP r` - Pop 8-bit value from stack to register
- `POP rr` - Pop 16-bit value from stack to register
- `POP rrr` - Pop 24-bit value from stack to register
- `POP rrrr` - Pop 32-bit value from stack to register

### 2. Register Range Pop Operations

- `POP r, r` - Pop range of 8-bit registers (from second to first, in reverse order)
  - Registers are popped in reverse order (Z to A, wrapping)
  - Example: `POP A, C` pops to C, B, A (last popped goes to first register)

### 3. Float Register Pop Operations

- `POP fr` - Pop float register from stack (4 bytes)
- `POP fr, fr` - Pop range of float registers (F15 to F0, in reverse order)
  - Each float register takes 4 bytes from stack

### 4. Memory Pop Operations

#### Absolute Address Memory
- `POP (nnn)` - Pop 8-bit value from stack to absolute address
- `POP16 (nnn)` - Pop 16-bit value from stack to absolute address
- `POP24 (nnn)` - Pop 24-bit value from stack to absolute address
- `POP32 (nnn)` - Pop 32-bit value from stack to absolute address

#### Register-Indirect Memory
- `POP (rrr)` - Pop 8-bit value from stack to register-indirect address
- `POP16 (rrr)` - Pop 16-bit value from stack to register-indirect address
- `POP24 (rrr)` - Pop 24-bit value from stack to register-indirect address
- `POP32 (rrr)` - Pop 32-bit value from stack to register-indirect address

## Stack Management

### Stack Pointer

POP uses the Stack Pointer for Registers (SPR):
- **SPR**: Points to next available stack position
- **Stack Shrinking**: Stack shrinks downward (SPR decrements)
- **Stack Location**: Uses RSRAM (Register Stack RAM)

### Stack Underflow Detection

POP checks for stack underflow before popping:
```csharp
if (_computer.CPU.REGS.SPR < requiredSpace)
{
    // Handle stack underflow
    Log.WriteLine($"Stack underflow...");
    _computer.MEMC.ResetAllStacks();
    _computer.CPU.REGS.IPO = _computer.MEMC.HMEM[_computer.MEMC.HMEM.ERROR_HANDLER_ADDRESS];
    _computer.MEMC.HMEM[_computer.MEMC.HMEM.ERROR_ID] = SystemMessages.ERR_STACK_UNDERFLOW;
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

### Register Pop Methods

- `Get8BitFromRegStack(SPR)` - Pops 8-bit value, then SPR decremented by 1
- `Get16BitFromRegStack(SPR)` - Pops 16-bit value, then SPR decremented by 2
- `Get24BitFromRegStack(SPR)` - Pops 24-bit value, then SPR decremented by 3
- `Get32BitFromRegStack(SPR)` - Pops 32-bit value, then SPR decremented by 4

### Register Range Pop

For register ranges, values are popped in reverse order:
```csharp
while (reg1Index != reg2Index)
{
    Pop8BitReg(reg2Index);
    reg2Index = (reg2Index == Mnem.A) ? Mnem.Z : (byte)(reg2Index - 1);
}
Pop8BitReg(reg2Index);
```

This ensures LIFO (Last In, First Out) behavior.

### Float Register Pop

Float registers are reconstructed from 32-bit integers:
```csharp
_computer.CPU.REGS.SPR -= 4;
uint stackVal = _computer.MEMC.Get32BitFromRegStack(_computer.CPU.REGS.SPR);
float floatValue = FloatPointUtils.UintToFloat(stackVal);
_computer.CPU.FREGS.SetRegister(fRegIndex, floatValue);
```

## Operation Categories Summary

1. **Single Register Pop**: 4 variants (8, 16, 24, 32-bit)
2. **Register Range Pop**: 1 variant (8-bit registers, reverse order)
3. **Float Register Pop**: 2 variants (single, range)
4. **Memory Pop**: 8 variants (4 data sizes × 2 addressing modes)

## Error Handling

When stack underflow occurs:
1. Error message logged with register name and instruction pointer
2. All stacks reset
3. Instruction pointer set to error handler address
4. Error ID set to `ERR_STACK_UNDERFLOW`
5. Execution continues at error handler

## Usage Examples

### Single Register Pop
```
POP A           ; Pop value to register A (1 byte)
POP BC          ; Pop value to register BC (2 bytes)
POP24 XYZ       ; Pop value to register XYZ (3 bytes)
POP32 ABCD      ; Pop value to register ABCD (4 bytes)
```

### Register Range Pop
```
POP A, C        ; Pop to C, B, A (3 bytes, reverse order)
POP X, Z        ; Pop to Z, Y, X (3 bytes, wraps)
```

### Float Register Pop
```
POP F0          ; Pop float register F0 (4 bytes)
POP F0, F2      ; Pop to F2, F1, F0 (12 bytes, reverse order)
```

### Memory Pop
```
POP (0x1000)    ; Pop to memory[0x1000] (1 byte)
POP16 (XYZ)     ; Pop 16-bit value to memory[XYZ]
```

## LIFO Stack Behavior

POP maintains LIFO (Last In, First Out) order:
- Last value pushed is first value popped
- Register ranges are popped in reverse order
- Ensures correct restoration of register state

## Comparison with PUSH Instruction

- **POP**: Removes values from stack (SPR decrements)
- **PUSH**: Adds values to stack (SPR increments)
- **POP**: Checks for underflow
- **PUSH**: Checks for overflow
- **POP/PUSH**: LIFO (Last In, First Out) stack operations

## Conclusion

The POP instruction provides comprehensive stack pop capabilities with support for all data sizes, register ranges, and memory addressing. The stack underflow detection ensures program safety and enables error recovery. The reverse-order popping for register ranges maintains proper LIFO stack semantics.

