# LDF Instruction Execution Analysis

## Executive Summary

The LDF (Load Flags) instruction implementation in `ExLDF.cs` provides **6 distinct instruction variants** for loading CPU flags into registers or memory. LDF reads the flags register and stores it to a destination.

## File Statistics

- **File**: `Emulator/Execution/ExLDF.cs`
- **Total Lines**: 64 lines
- **Instruction Variants**: 6 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 3 bits of opcode

## Addressing Modes

### Flag Loading Operations

- `LDF r` - Load flags byte to 8-bit register
- `LDF (rrr)` - Load flags byte to memory at register-indirect address
- `LDF (nnn)` - Load flags byte to memory at absolute address
- `LDF r, n` - Load flags byte ANDed with mask to 8-bit register
- `LDF (rrr), n` - Load flags byte ANDed with mask to memory at register-indirect address
- `LDF (nnn), n` - Load flags byte ANDed with mask to memory at absolute address

## Operation Description

LDF performs flag loading:
- **Flag Reading**: Reads all flags as a single byte
- **Storage**: Stores flags byte to register or memory
- **Masking**: Optional mask can filter specific flags

## Implementation Details

### Basic Flag Loading

For basic operations:
```csharp
byte flagsValue = computer.CPU.FLAGS.GetFlagsByte();
computer.CPU.REGS.Set8BitRegister(regIndex, flagsValue);
```

### Masked Flag Loading

For masked operations:
```csharp
byte flagsValue = computer.CPU.FLAGS.GetFlagsByte();
byte mask = computer.MEMC.Fetch();
computer.CPU.REGS.Set8BitRegister(regIndex, (byte)(flagsValue & mask));
```

## Flag Byte Format

The flags byte contains all flag values:
- **Bit 0**: Zero flag (Z)
- **Bit 1**: Carry flag (C)
- **Bit 2**: Sign flag (SN)
- **Bit 3**: Overflow flag (OV)
- **Bit 4**: Parity flag (PO)
- **Bit 5**: Equal flag (EQ)
- **Bit 6**: Greater Than flag (GT)
- **Bit 7**: Less Than flag (LT)

## Opcode Encoding

LDF uses compact encoding:
1. **Primary Opcode**: Opcode for LDF
2. **Secondary Opcode**: Upper 3 bits of opcode (`ldOp >> 5`)
3. **Register Index**: Lower 5 bits of opcode (`ldOp & 0b00011111`) for register operations
4. **Mask**: Optional immediate byte for masked operations

## Flag Updates

LDF does not modify CPU flags (read-only operation).

## Operation Categories Summary

1. **Register Operations**: 2 variants (with/without mask)
2. **Register-Indirect Memory Operations**: 2 variants (with/without mask)
3. **Absolute Memory Operations**: 2 variants (with/without mask)

## Usage Examples

```
LDF A           ; A = flags byte
LDF (XYZ)       ; memory[XYZ] = flags byte
LDF A, 0x0F     ; A = flags byte & 0x0F (lower 4 flags)
LDF (0x1000)    ; memory[0x1000] = flags byte
```

## Typical Use Cases

1. **Flag Saving**: Save flags for later restoration
2. **Flag Inspection**: Read flags for conditional logic
3. **Flag Filtering**: Extract specific flags using mask
4. **State Preservation**: Save processor state

## Mask Usage

Masks allow selective flag reading:
- `0x01`: Only Zero flag
- `0x0F`: Lower 4 flags (Z, C, SN, OV)
- `0xF0`: Upper 4 flags (PO, EQ, GT, LT)
- `0xFF`: All flags

## Comparison with Other Instructions

- **LDF**: Loads flags to register/memory (read operation)
- **SETF/RESF**: Sets/resets individual flags (write operations)
- **LDF**: Bulk flag reading
- **SETF/RESF**: Individual flag manipulation

## Conclusion

The LDF instruction provides efficient flag reading capabilities with support for masking. The ability to save and filter flags is essential for state preservation and conditional logic in Continuum 93 programs.

