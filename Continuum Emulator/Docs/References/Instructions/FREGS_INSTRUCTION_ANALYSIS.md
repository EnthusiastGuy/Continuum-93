# FREGS Instruction Execution Analysis

## Executive Summary

The FREGS (Float Register Bank Select) instruction implementation in `ExFREGS.cs` provides **4 distinct instruction variants** for selecting the active float register bank. FREGS switches between different banks of float registers, allowing access to multiple sets of float registers.

## File Statistics

- **File**: `Emulator/Execution/ExFREGS.cs`
- **Total Lines**: 53 lines
- **Instruction Variants**: 4 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 3 bits of opcode

## Addressing Modes

### Register Bank Selection

- `FREGS n` - Select float register bank using immediate value
- `FREGS r` - Select float register bank using value from 8-bit register
- `FREGS (nnn)` - Select float register bank using value from absolute address
- `FREGS (rrr)` - Select float register bank using value from register-indirect address

## Operation Description

FREGS performs register bank switching:
- **Bank Selection**: `SetRegisterBank(bankIndex)`
- **Bank Index**: 0-255 (8-bit value)
- **Active Bank**: All float register operations use the selected bank

## Implementation Details

### Bank Selection

FREGS uses the float register bank system:
```csharp
byte bankIndex = computer.MEMC.Fetch();
computer.CPU.FREGS.SetRegisterBank(bankIndex);
```

The bank index determines which set of float registers is active.

## Opcode Encoding

FREGS uses compact encoding:
1. **Primary Opcode**: Opcode for FREGS
2. **Secondary Opcode**: Upper 3 bits of opcode (`ldOp >> 5`)
3. **Bank Index Source**: Immediate, register, or memory address

## Flag Updates

FREGS does not modify CPU flags.

## Operation Categories Summary

1. **Immediate Bank Selection**: 1 variant
2. **Register Bank Selection**: 1 variant
3. **Absolute Address Bank Selection**: 1 variant
4. **Register-Indirect Bank Selection**: 1 variant

## Usage Examples

```
FREGS 0         ; Select float register bank 0
FREGS A         ; Select float register bank from register A
FREGS (0x1000)  ; Select float register bank from memory[0x1000]
FREGS (XYZ)     ; Select float register bank from memory[XYZ]
```

## Typical Use Cases

1. **Context Switching**: Switch between different float register contexts
2. **Function Calls**: Save/restore float registers by switching banks
3. **Multi-threading**: Different threads use different register banks
4. **State Management**: Organize float registers by state

## Register Bank System

The float register bank system allows:
- **Multiple Banks**: Multiple sets of float registers
- **Bank Switching**: Fast context switching
- **Isolation**: Different contexts don't interfere
- **Efficiency**: Faster than saving/restoring registers

## Comparison with REGS Instruction

- **FREGS**: Selects float register bank
- **REGS**: Selects integer register bank
- **FREGS**: For float register operations
- **REGS**: For integer register operations

## Conclusion

The FREGS instruction provides efficient float register bank selection capabilities. The bank switching system enables fast context switching and state management for float register operations in Continuum 93 programs.

