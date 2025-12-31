# MEMC Instruction Execution Analysis

## Executive Summary

The MEMC (Memory Copy) instruction implementation in `ExMEMC.cs` provides **9 distinct instruction variants** for copying memory blocks. MEMC copies a specified number of bytes from a source address to a destination address.

## File Statistics

- **File**: `Emulator/Execution/ExMEMC.cs`
- **Total Lines**: 103 lines
- **Instruction Variants**: 9 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 3 bits of opcode

## Addressing Modes

### Memory Copy Operations

All combinations of source address, destination address, and length:
- `MEMC rrr, rrr, rrr` - Copy from register-indirect source to register-indirect dest, length from register
- `MEMC nnn, rrr, rrr` - Copy from absolute source to register-indirect dest, length from register
- `MEMC rrr, nnn, rrr` - Copy from register-indirect source to absolute dest, length from register
- `MEMC nnn, nnn, rrr` - Copy from absolute source to absolute dest, length from register
- `MEMC rrr, rrr, nnn` - Copy from register-indirect source to register-indirect dest, length immediate
- `MEMC nnn, rrr, nnn` - Copy from absolute source to register-indirect dest, length immediate
- `MEMC rrr, nnn, nnn` - Copy from register-indirect source to absolute dest, length immediate
- `MEMC nnn, nnn, nnn` - Copy from absolute source to absolute dest, length immediate

## Operation Description

MEMC performs memory block copying:
- **Source Address**: Starting address of source memory block
- **Destination Address**: Starting address of destination memory block
- **Length**: Number of bytes to copy (24-bit value)
- **Copy Operation**: `RAM.Copy(sourceAddress, destAddress, length)`

## Implementation Details

### Memory Copy

MEMC uses the RAM copy method:
```csharp
computer.MEMC.RAM.Copy(sourceAddress, destAddress, length);
```

The copy operation handles:
- **Overlapping Regions**: Safe copying even if regions overlap
- **Memory Bounds**: Respects memory boundaries
- **Efficient Copy**: Optimized memory copying

## Opcode Encoding

MEMC uses compact encoding:
1. **Primary Opcode**: Opcode for MEMC
2. **Secondary Opcode**: Upper 3 bits of opcode (`op >> 5`)
3. **Source Address**: Register-indirect (24-bit register) or absolute (24-bit immediate)
4. **Destination Address**: Register-indirect (24-bit register) or absolute (24-bit immediate)
5. **Length**: Register (24-bit register) or immediate (24-bit immediate)

## Flag Updates

MEMC does not modify CPU flags.

## Operation Categories Summary

1. **All Register Addressing**: 1 variant
2. **Mixed Addressing**: 6 variants (combinations of register/immediate)
3. **All Immediate Addressing**: 1 variant

## Usage Examples

```
MEMC XYZ, ABC, DEF    ; Copy from memory[XYZ] to memory[ABC], length from DEF
MEMC 0x1000, XYZ, 100 ; Copy from memory[0x1000] to memory[XYZ], length 100
MEMC XYZ, 0x2000, DEF ; Copy from memory[XYZ] to memory[0x2000], length from DEF
MEMC 0x1000, 0x2000, 100 ; Copy from memory[0x1000] to memory[0x2000], length 100
```

## Typical Use Cases

1. **Memory Block Copying**: Copy data blocks
2. **Buffer Operations**: Copy buffers
3. **Data Movement**: Move data in memory
4. **String Operations**: Copy strings

## Memory Copy Properties

- **Overlapping Safe**: Handles overlapping source and destination
- **Efficient**: Optimized for large block copies
- **Flexible Addressing**: Supports all address combinations
- **24-bit Length**: Supports large copy operations

## Comparison with MEMF Instruction

- **MEMC**: Copies memory from source to destination
- **MEMF**: Fills memory with a value
- **MEMC**: Two-address operation
- **MEMF**: Single-address operation

## Conclusion

The MEMC instruction provides efficient memory block copying capabilities with flexible addressing modes. The comprehensive addressing combinations make it suitable for various memory operations in Continuum 93 programs.

