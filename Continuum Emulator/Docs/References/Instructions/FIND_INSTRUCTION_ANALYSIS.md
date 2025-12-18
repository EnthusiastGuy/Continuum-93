# FIND Instruction Execution Analysis

## Executive Summary

The FIND instruction implementation in `ExFIND.cs` provides **3 distinct instruction variants** for searching memory for byte values or byte patterns. FIND searches memory starting from a specified address and returns the address where the value or pattern is found.

## File Statistics

- **File**: `Emulator/Execution/ExFIND.cs`
- **Total Lines**: 56 lines
- **Instruction Variants**: 3 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 3 bits of opcode

## Addressing Modes

### Memory Search Operations

- `FIND (rrr), n` - Find byte value in memory starting from register-indirect address
- `FIND (rrr), (nnn)` - Find byte pattern (string) in memory starting from register-indirect address, pattern at absolute address
- `FIND (rrr), (rrr)` - Find byte pattern (string) in memory starting from register-indirect address, pattern at register-indirect address

## Operation Description

FIND performs memory searching:
- **Start Address**: 24-bit address in register (search begins here)
- **Search Value**: Single byte value or byte pattern (string)
- **Result**: Address where value/pattern found (stored back in register)
- **Not Found**: Returns address beyond search range if not found

## Implementation Details

### Single Byte Search

For single byte search:
```csharp
uint startAddress = computer.CPU.REGS.Get24BitRegister(addressRegIndex);
byte value = computer.MEMC.Fetch();

computer.CPU.REGS.Set24BitRegister(addressRegIndex,
    computer.MEMC.RAM.Find((int)startAddress, value));
```

### Pattern Search

For pattern (string) search:
```csharp
uint startAddress = computer.CPU.REGS.Get24BitRegister(addressRegIndex);
uint stringAddress = computer.MEMC.Fetch24();
byte[] stringData = computer.MEMC.GetStringBytesAt(stringAddress);

computer.CPU.REGS.Set24BitRegister(addressRegIndex,
    computer.MEMC.RAM.FindPattern((int)startAddress, stringData));
```

## Opcode Encoding

FIND uses compact encoding:
1. **Primary Opcode**: Opcode for FIND
2. **Secondary Opcode**: Upper 3 bits of opcode (`ldOp >> 5`)
3. **Start Address Register**: Lower 5 bits of opcode (`ldOp & 0b00011111`)
4. **Search Value**: Immediate byte or memory address for pattern

## Flag Updates

FIND does not modify CPU flags.

## Operation Categories Summary

1. **Single Byte Search**: 1 variant
2. **Pattern Search (Absolute)**: 1 variant
3. **Pattern Search (Register-Indirect)**: 1 variant

## Usage Examples

```
FIND (XYZ), 0x00        ; Find null byte starting from memory[XYZ]
FIND (XYZ), (0x1000)    ; Find string at memory[0x1000] starting from memory[XYZ]
FIND (XYZ), (ABC)       ; Find string at memory[ABC] starting from memory[XYZ]
```

## Typical Use Cases

1. **String Searching**: Find substrings in memory
2. **Byte Searching**: Find specific byte values
3. **Pattern Matching**: Locate data patterns
4. **Memory Scanning**: Scan memory for values

## Search Behavior

FIND searches forward from start address:
- **Direction**: Forward (increasing addresses)
- **Termination**: Finds first match or reaches end of memory
- **Result**: Address of match (or end address if not found)
- **In-place Update**: Result stored back in start address register

## Comparison with Other Instructions

- **FIND**: Searches memory for values/patterns
- **LD**: Loads values from memory
- **FIND**: Returns address of match
- **LD**: Returns value at address

## Conclusion

The FIND instruction provides efficient memory searching capabilities for both single bytes and byte patterns. The search functionality is essential for string operations and memory scanning in Continuum 93 programs.

