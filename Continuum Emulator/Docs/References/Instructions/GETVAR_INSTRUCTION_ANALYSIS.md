# GETVAR Instruction Execution Analysis

## Executive Summary

The GETVAR (Get Variable) instruction implementation in `ExGETVAR.cs` provides **2 distinct instruction variants** for reading variables from high memory (HMEM). GETVAR reads a 32-bit value from the variable storage area.

## File Statistics

- **File**: `Emulator/Execution/ExGETVAR.cs`
- **Total Lines**: 39 lines
- **Instruction Variants**: 2 unique opcodes
- **Implementation Pattern**: Uses switch statement on secondary opcode

## Addressing Modes

### Variable Reading Operations

- `GETVAR n, rrrr` - Get variable at immediate index, store to 32-bit register
- `GETVAR rrrr, rrrr` - Get variable at index from 32-bit register, store to 32-bit register

## Operation Description

GETVAR performs variable reading:
- **Variable Access**: Reads from `HMEM[varIndex]`
- **32-bit Values**: Variables are 32-bit unsigned integers
- **Register Storage**: Result stored in 32-bit register

## Implementation Details

### Immediate Index

For immediate variable index:
```csharp
uint varIndex = computer.MEMC.Fetch32();
uint value = computer.MEMC.HMEM[varIndex];
byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);

computer.CPU.REGS.Set32BitRegister(regIndex, value);
```

### Register Index

For register-based variable index:
```csharp
byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
uint varIndex = computer.CPU.REGS.Get32BitRegister(regIndex);

uint value = computer.MEMC.HMEM[varIndex];

byte regValueIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
computer.CPU.REGS.Set32BitRegister(regValueIndex, value);
```

## High Memory (HMEM)

GETVAR accesses the high memory area:
- **HMEM**: Separate memory area for variables
- **32-bit Values**: Each variable is 32 bits
- **Index-based**: Variables accessed by index
- **Fast Access**: Optimized for variable access

## Opcode Encoding

GETVAR uses compact encoding:
1. **Primary Opcode**: Opcode for GETVAR
2. **Secondary Opcode**: Determines addressing mode
3. **Variable Index**: Immediate 32-bit value or from register
4. **Destination Register**: 32-bit register index

## Flag Updates

GETVAR does not modify CPU flags.

## Operation Categories Summary

1. **Immediate Index Operations**: 1 variant
2. **Register Index Operations**: 1 variant

## Usage Examples

```
GETVAR 0, ABCD      ; Get variable 0, store to register ABCD
GETVAR XYZ, ABCD    ; Get variable at index in XYZ, store to ABCD
```

## Typical Use Cases

1. **Variable Access**: Read program variables
2. **Global Variables**: Access global variable storage
3. **Dynamic Access**: Access variables by computed index
4. **State Variables**: Read state variables

## Variable System

The variable system provides:
- **High Memory**: Separate storage for variables
- **32-bit Values**: Large value range
- **Index-based**: Simple variable addressing
- **Fast Access**: Optimized variable access

## Comparison with SETVAR Instruction

- **GETVAR**: Reads variable from HMEM
- **SETVAR**: Writes variable to HMEM
- **GETVAR/SETVAR**: Complementary operations for variable access

## Conclusion

The GETVAR instruction provides efficient variable reading capabilities from high memory. The variable system enables fast access to program variables in Continuum 93 programs.

