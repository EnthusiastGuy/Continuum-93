# STREGS Instruction Execution Analysis

## Executive Summary

The STREGS (Store Registers) instruction implementation in `ExSTREGS.cs` provides **2 distinct instruction variants** for storing a range of registers to memory. STREGS stores consecutive registers to a memory address.

## File Statistics

- **File**: `Emulator/Execution/ExSTREGS.cs`
- **Total Lines**: 45 lines
- **Instruction Variants**: 2 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 1 bit of opcode

## Addressing Modes

### Register Range Storing

- `STREGS (rrr), r, r` - Store registers from r1 to r2 to memory at register-indirect address
- `STREGS (nnn), r, r` - Store registers from r1 to r2 to memory at absolute address

## Operation Description

STREGS performs bulk register storing:
- **Range Storing**: Stores registers from r1 to r2 (inclusive)
- **Memory Destination**: Writes to memory starting at specified address
- **Length Calculation**: `len = abs(r1 - r2) + 1`
- **Memory Write**: Uses `LoadMemAt()` for memory access

## Implementation Details

### Register Range Storing

For register-indirect addressing:
```csharp
byte rAdr = (byte)(ldOp >> 2);
byte r1 = (byte)(((ldOp & 0b00000011) << 3) + (mixedReg >> 5));
byte r2 = (byte)(mixedReg & 0b00011111);
uint address = computer.CPU.REGS.Get24BitRegister(rAdr);

byte[] regs = computer.CPU.REGS.GetRegistersBetween(r1, r2);
computer.LoadMemAt(address, regs);
```

### Memory Writing

STREGS uses memory writing:
- **GetRegistersBetween**: Extracts register values in range
- **LoadMemAt**: Writes register values to memory
- **Sequential Write**: Writes registers sequentially to memory

## Opcode Encoding

STREGS uses compact encoding:
1. **Primary Opcode**: Opcode for STREGS
2. **Secondary Opcode**: Upper 1 bit of opcode (`ldOp >> 7`)
3. **Register Range**: r1 and r2 encoded in opcode and mixed register
4. **Address**: Register-indirect or absolute address

## Flag Updates

STREGS does not modify CPU flags.

## Operation Categories Summary

1. **Register-Indirect Memory Operations**: 1 variant
2. **Absolute Memory Operations**: 1 variant

## Usage Examples

```
STREGS (XYZ), 0, 15    ; Store registers 0-15 to memory[XYZ]
STREGS (0x1000), 5, 10 ; Store registers 5-10 to memory[0x1000]
```

## Typical Use Cases

1. **Context Saving**: Save multiple registers to memory
2. **State Storing**: Store processor state to memory
3. **Bulk Operations**: Efficiently store multiple registers
4. **Function Call**: Save registers before function call

## Register Range

STREGS stores registers in a range:
- **Start Register**: r1 (first register to store)
- **End Register**: r2 (last register to store)
- **Direction**: Works regardless of r1 < r2 or r1 > r2
- **Length**: `abs(r1 - r2) + 1` registers stored

## Comparison with LDREGS Instruction

- **STREGS**: Stores registers to memory
- **LDREGS**: Loads registers from memory
- **STREGS/LDREGS**: Complementary operations for register save/restore

## Conclusion

The STREGS instruction provides efficient bulk register storing capabilities. The range-based storing is essential for context saving and state management in Continuum 93 programs.

