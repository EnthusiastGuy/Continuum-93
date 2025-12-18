# LDREGS Instruction Execution Analysis

## Executive Summary

The LDREGS (Load Registers) instruction implementation in `ExLDREGS.cs` provides **2 distinct instruction variants** for loading a range of registers from memory. LDREGS loads consecutive registers from a memory address.

## File Statistics

- **File**: `Emulator/Execution/ExLDREGS.cs`
- **Total Lines**: 45 lines
- **Instruction Variants**: 2 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 1 bit of opcode

## Addressing Modes

### Register Range Loading

- `LDREGS r, r, (rrr)` - Load registers from r1 to r2 from memory at register-indirect address
- `LDREGS r, r, (nnn)` - Load registers from r1 to r2 from memory at absolute address

## Operation Description

LDREGS performs bulk register loading:
- **Range Loading**: Loads registers from r1 to r2 (inclusive)
- **Memory Source**: Reads from memory starting at specified address
- **Length Calculation**: `len = abs(r1 - r2) + 1`
- **Wrapped Memory**: Uses `GetMemoryWrapped()` for memory access

## Implementation Details

### Register Range Loading

For register-indirect addressing:
```csharp
byte r1 = (byte)(ldOp >> 2);
byte r2 = (byte)(((ldOp & 0b00000011) << 3) + (mixedReg >> 5));
byte rAdr = (byte)(mixedReg & 0b00011111);
uint address = computer.CPU.REGS.Get24BitRegister(rAdr);
byte len = (byte)(Math.Abs(r1 - r2) + 1);

computer.CPU.REGS.SetRegistersBetween(r1, r2, computer.MEMC.GetMemoryWrapped(address, len));
```

### Memory Wrapping

LDREGS uses wrapped memory access:
- **GetMemoryWrapped**: Handles memory wrapping at boundaries
- **Circular Buffer**: Memory wraps around if address exceeds bounds
- **Safe Access**: Prevents out-of-bounds errors

## Opcode Encoding

LDREGS uses compact encoding:
1. **Primary Opcode**: Opcode for LDREGS
2. **Secondary Opcode**: Upper 1 bit of opcode (`ldOp >> 7`)
3. **Register Range**: r1 and r2 encoded in opcode and mixed register
4. **Address**: Register-indirect or absolute address

## Flag Updates

LDREGS does not modify CPU flags.

## Operation Categories Summary

1. **Register-Indirect Memory Operations**: 1 variant
2. **Absolute Memory Operations**: 1 variant

## Usage Examples

```
LDREGS 0, 15, (XYZ)    ; Load registers 0-15 from memory[XYZ]
LDREGS 5, 10, (0x1000) ; Load registers 5-10 from memory[0x1000]
```

## Typical Use Cases

1. **Context Restoration**: Restore multiple registers from memory
2. **State Loading**: Load processor state from memory
3. **Bulk Operations**: Efficiently load multiple registers
4. **Function Return**: Restore registers after function call

## Register Range

LDREGS loads registers in a range:
- **Start Register**: r1 (first register to load)
- **End Register**: r2 (last register to load)
- **Direction**: Works regardless of r1 < r2 or r1 > r2
- **Length**: `abs(r1 - r2) + 1` registers loaded

## Comparison with STREGS Instruction

- **LDREGS**: Loads registers from memory
- **STREGS**: Stores registers to memory
- **LDREGS/STREGS**: Complementary operations for register save/restore

## Conclusion

The LDREGS instruction provides efficient bulk register loading capabilities. The range-based loading is essential for context restoration and state management in Continuum 93 programs.

