# REGS Instruction Execution Analysis

## Executive Summary

The REGS (Register Bank Select) instruction implementation in `ExREGS.cs` provides **4 distinct instruction variants** for selecting the active integer register bank. REGS switches between different banks of integer registers, allowing access to multiple sets of registers.

## File Statistics

- **File**: `Emulator/Execution/ExREGS.cs`
- **Total Lines**: 54 lines
- **Instruction Variants**: 4 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 3 bits of opcode

## Addressing Modes

### Register Bank Selection

- `REGS n` - Select register bank using immediate value
- `REGS r` - Select register bank using value from 8-bit register
- `REGS (nnn)` - Select register bank using value from absolute address
- `REGS (rrr)` - Select register bank using value from register-indirect address

## Operation Description

REGS performs register bank switching:
- **Bank Selection**: `SetRegisterBank(bankIndex)`
- **Bank Index**: 0-255 (8-bit value)
- **Active Bank**: All register operations use the selected bank

## Implementation Details

### Bank Selection

REGS uses the register bank system:
```csharp
byte bankIndex = computer.MEMC.Fetch();
computer.CPU.REGS.SetRegisterBank(bankIndex);
```

The bank index determines which set of registers is active.

## Opcode Encoding

REGS uses compact encoding:
1. **Primary Opcode**: Opcode for REGS
2. **Secondary Opcode**: Upper 3 bits of opcode (`ldOp >> 5`)
3. **Bank Index Source**: Immediate, register, or memory address

## Flag Updates

REGS does not modify CPU flags.

## Operation Categories Summary

1. **Immediate Bank Selection**: 1 variant
2. **Register Bank Selection**: 1 variant
3. **Absolute Address Bank Selection**: 1 variant
4. **Register-Indirect Bank Selection**: 1 variant

## Usage Examples

```
REGS 0          ; Select register bank 0
REGS A          ; Select register bank from register A
REGS (0x1000)   ; Select register bank from memory[0x1000]
REGS (XYZ)      ; Select register bank from memory[XYZ]
```

## Typical Use Cases

1. **Context Switching**: Switch between different register contexts
2. **Function Calls**: Save/restore registers by switching banks
3. **Multi-threading**: Different threads use different register banks
4. **State Management**: Organize registers by state

## Register Bank System

The register bank system allows:
- **Multiple Banks**: Multiple sets of registers
- **Bank Switching**: Fast context switching
- **Isolation**: Different contexts don't interfere
- **Efficiency**: Faster than saving/restoring registers

## Comparison with FREGS Instruction

- **REGS**: Selects integer register bank
- **FREGS**: Selects float register bank
- **REGS**: For integer register operations
- **FREGS**: For float register operations

## Conclusion

The REGS instruction provides efficient register bank selection capabilities. The bank switching system enables fast context switching and state management for integer register operations in Continuum 93 programs.

