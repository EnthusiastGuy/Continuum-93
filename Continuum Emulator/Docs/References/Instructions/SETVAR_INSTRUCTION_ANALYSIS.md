# SETVAR Instruction Execution Analysis

## Executive Summary

The SETVAR (Set Variable) instruction implementation in `ExSETVAR.cs` provides **4 distinct instruction variants** for writing variables to high memory (HMEM). SETVAR writes a 32-bit value to the variable storage area.

## File Statistics

- **File**: `Emulator/Execution/ExSETVAR.cs`
- **Total Lines**: 58 lines
- **Instruction Variants**: 4 unique opcodes
- **Implementation Pattern**: Uses switch statement on secondary opcode

## Addressing Modes

### Variable Writing Operations

- `SETVAR n, n` - Set variable at immediate index to immediate value
- `SETVAR n, rrrr` - Set variable at immediate index to value from 32-bit register
- `SETVAR rrrr, n` - Set variable at index from 32-bit register to immediate value
- `SETVAR rrrr, rrrr` - Set variable at index from 32-bit register to value from 32-bit register

## Operation Description

SETVAR performs variable writing:
- **Variable Access**: Writes to `HMEM[varIndex]`
- **32-bit Values**: Variables are 32-bit unsigned integers
- **Value Source**: Value from immediate or register

## Implementation Details

### Immediate Index and Value

For immediate variable index and value:
```csharp
uint varIndex = computer.MEMC.Fetch32();
uint value = computer.MEMC.Fetch32();

computer.MEMC.HMEM[varIndex] = value;
```

### Immediate Index, Register Value

For immediate variable index with register value:
```csharp
uint varIndex = computer.MEMC.Fetch32();
byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
uint value = computer.CPU.REGS.Get32BitRegister(regIndex);

computer.MEMC.HMEM[varIndex] = value;
```

### Register Index, Immediate Value

For register-based variable index with immediate value:
```csharp
byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
uint varIndex = computer.CPU.REGS.Get32BitRegister(regIndex);
uint value = computer.MEMC.Fetch32();

computer.MEMC.HMEM[varIndex] = value;
```

### Register Index and Value

For register-based variable index and value:
```csharp
byte regIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
uint varIndex = computer.CPU.REGS.Get32BitRegister(regIndex);

byte regValueIndex = (byte)(computer.MEMC.Fetch() & 0b00011111);
uint value = computer.CPU.REGS.Get32BitRegister(regValueIndex);

computer.MEMC.HMEM[varIndex] = value;
```

## High Memory (HMEM)

SETVAR accesses the high memory area:
- **HMEM**: Separate memory area for variables
- **32-bit Values**: Each variable is 32 bits
- **Index-based**: Variables accessed by index
- **Fast Access**: Optimized for variable access

## Opcode Encoding

SETVAR uses compact encoding:
1. **Primary Opcode**: Opcode for SETVAR
2. **Secondary Opcode**: Determines addressing mode
3. **Variable Index**: Immediate 32-bit value or from register
4. **Value**: Immediate 32-bit value or from register

## Flag Updates

SETVAR does not modify CPU flags.

## Operation Categories Summary

1. **Immediate Index and Value**: 1 variant
2. **Immediate Index, Register Value**: 1 variant
3. **Register Index, Immediate Value**: 1 variant
4. **Register Index and Value**: 1 variant

## Usage Examples

```
SETVAR 0, 100       ; Set variable 0 to 100
SETVAR 0, ABCD      ; Set variable 0 to value in ABCD
SETVAR XYZ, 200     ; Set variable at index in XYZ to 200
SETVAR XYZ, ABCD    ; Set variable at index in XYZ to value in ABCD
```

## Typical Use Cases

1. **Variable Assignment**: Set program variables
2. **Global Variables**: Update global variable storage
3. **Dynamic Access**: Set variables by computed index
4. **State Variables**: Update state variables

## Variable System

The variable system provides:
- **High Memory**: Separate storage for variables
- **32-bit Values**: Large value range
- **Index-based**: Simple variable addressing
- **Fast Access**: Optimized variable access

## Comparison with GETVAR Instruction

- **SETVAR**: Writes variable to HMEM
- **GETVAR**: Reads variable from HMEM
- **SETVAR/GETVAR**: Complementary operations for variable access

## Conclusion

The SETVAR instruction provides efficient variable writing capabilities to high memory. The variable system enables fast access to program variables in Continuum 93 programs.

