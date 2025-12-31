# CPU Architecture Documentation

## Executive Summary

The Continuum 93 CPU architecture consists of a **Microprocessor** that manages three main components: **Registers** (integer registers with bank switching), **FloatRegisters** (floating-point registers with bank switching), and **Flags** (CPU status flags). The architecture supports 26 general-purpose registers (A-Z) that can be combined into 16-bit, 24-bit, and 32-bit registers, with support for 256 register banks.

## File Structure

- **Microprocessor.cs**: Main CPU class that coordinates registers, float registers, and flags
- **Registers.cs**: Integer register management with bank switching
- **FloatRegisters.cs**: Floating-point register management with bank switching
- **Flags.cs**: CPU status flags management
- **Instructions.cs**: Instruction addressing mode constants

## Microprocessor

The `Microprocessor` class is the central CPU component that provides access to:
- **REGS**: Integer register system
- **FREGS**: Float register system
- **FLAGS**: CPU flags system

### Initialization

```csharp
public Microprocessor(Computer computer)
{
    _computer = computer;
    _regs = new Registers(_computer);
    _fregs = new FloatRegisters(_computer);
    _flags = new Flags();
}
```

## Registers Architecture

### Register Organization

The register system uses a **flat array** with **bank switching**:
- **Total Storage**: 256 banks × 26 registers = 6,656 bytes
- **Active Bank**: One bank active at a time (26 registers visible)
- **Bank Offset**: `bankOffset = registerBank * 26`

### Register Sizes

Registers can be accessed in multiple sizes:

#### 8-bit Registers (26 registers)
- **Names**: A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z
- **Index Range**: 0-25
- **Storage**: Single byte per register

#### 16-bit Registers (26 combinations)
- **Composition**: Two consecutive 8-bit registers
- **Examples**: AB, BC, CD, DE, EF, FG, GH, HI, IJ, JK, KL, LM, MN, NO, OP, PQ, QR, RS, ST, TU, UV, VW, WX, XY, YZ, ZA
- **Byte Order**: Big-endian (high byte in first register, low byte in second)
- **Next Register Calculation**: Uses `Next1[]` lookup table for register wrapping

#### 24-bit Registers (26 combinations)
- **Composition**: Three consecutive 8-bit registers
- **Examples**: ABC, BCD, CDE, DEF, EFG, FGH, GHI, HIJ, IJK, JKL, KLM, LMN, MNO, NOP, OPQ, PQR, QRS, RST, STU, TUV, UVW, VWX, WXY, XYZ, YZA, ZAB
- **Byte Order**: Big-endian (high byte in first register, low byte in third)
- **Next Register Calculation**: Uses `Next1[]`, `Next2[]` lookup tables

#### 32-bit Registers (26 combinations)
- **Composition**: Four consecutive 8-bit registers
- **Examples**: ABCD, BCDE, CDEF, DEFG, EFGH, FGHI, GHIJ, HIJK, IJKL, JKLM, KLMN, LMNO, MNOP, NOPQ, OPQR, PQRS, QRST, RSTU, STUV, TUVW, UVWX, VWXY, WXYZ, XYZA, YZAB, ZABC
- **Byte Order**: Big-endian (high byte in first register, low byte in fourth)
- **Next Register Calculation**: Uses `Next1[]`, `Next2[]`, `Next3[]` lookup tables

### Register Bank System

- **Total Banks**: 256 (0-255)
- **Registers per Bank**: 26
- **Bank Selection**: `SetRegisterBank(byte bankIndex)`
- **Bank Offset Calculation**: `bankOffset = bankIndex * 26`
- **Active Registers**: Only registers in the active bank are accessible

### Special Registers

The CPU has three special registers that are not part of the bank system:

1. **IPO (Instruction Pointer)**: 24-bit address of current instruction
   - **Range**: 0x000000 - 0xFFFFFF
   - **Auto-increment**: Incremented by instruction fetch operations

2. **SPR (Stack Pointer for Registers)**: 24-bit pointer to register stack
   - **Stack Type**: Register stack (RSRAM)
   - **Used By**: PUSH/POP instructions for register values

3. **SPC (Stack Pointer for Calls)**: 24-bit pointer to call stack
   - **Stack Type**: Call stack (CSRAM)
   - **Used By**: CALL/CALLR/RET/RETIF instructions

### Register Operations

The register system provides comprehensive operations:

#### Arithmetic Operations
- **ADD**: Addition (8, 16, 24, 32-bit)
- **SUB**: Subtraction (8, 16, 24, 32-bit)
- **MUL**: Multiplication (with overflow detection)
- **DIV**: Division (with remainder support)
- **INC**: Increment (8, 16, 24, 32-bit)
- **DEC**: Decrement (8, 16, 24, 32-bit)

#### Logical Operations
- **AND**: Bitwise AND
- **OR**: Bitwise OR
- **XOR**: Bitwise XOR
- **NAND**: Bitwise NAND
- **NOR**: Bitwise NOR
- **XNOR**: Bitwise XNOR
- **IMPLY**: Logical implication
- **INV**: Bitwise inversion (NOT)

#### Bit Manipulation
- **SET**: Set bit at position
- **RES**: Reset (clear) bit at position
- **BIT**: Test bit at position

#### Shift and Rotate
- **SL**: Shift left (8, 16, 24, 32-bit)
- **SR**: Shift right (8, 16, 24, 32-bit)
- **RL**: Rotate left (8, 16, 24, 32-bit)
- **RR**: Rotate right (8, 16, 24, 32-bit)

#### Memory Operations
- **Register-to-Memory**: Operations that write register values to memory
- **Memory-to-Register**: Operations that read memory values into registers

### Register Composition/Decomposition

Registers are composed and decomposed using helper methods:

#### 16-bit Composition
```csharp
private ushort Compose16Bit(byte reg1Pointer, byte reg2Pointer)
{
    return (ushort)((_gpDataFlat[bankOffset + reg1Pointer] << 8) + 
                    _gpDataFlat[bankOffset + reg2Pointer]);
}
```

#### 24-bit Composition
```csharp
private uint Compose24Bit(byte reg1Pointer, byte reg2Pointer, byte reg3Pointer)
{
    return (uint)((_gpDataFlat[bankOffset + reg1Pointer] << 16) + 
                  (_gpDataFlat[bankOffset + reg2Pointer] << 8) + 
                  _gpDataFlat[bankOffset + reg3Pointer]);
}
```

#### 32-bit Composition
```csharp
private uint Compose32Bit(byte reg1Pointer, byte reg2Pointer, byte reg3Pointer, byte reg4Pointer)
{
    return (uint)((_gpDataFlat[bankOffset + reg1Pointer] << 24) + 
                  (_gpDataFlat[bankOffset + reg2Pointer] << 16) + 
                  (_gpDataFlat[bankOffset + reg3Pointer] << 8) + 
                  _gpDataFlat[bankOffset + reg4Pointer]);
}
```

### Signed Integer Support

The register system supports signed integers:
- **8-bit Signed**: `Get8BitRegisterSigned()` / `Set8BitRegisterSigned()`
- **16-bit Signed**: `Get16BitRegisterSigned()` / `Set16BitRegisterSigned()`
- **24-bit Signed**: `Get24BitRegisterSigned()` / `Set24BitRegisterSigned()` (with sign extension)
- **32-bit Signed**: `Get32BitRegisterSigned()` / `Set32BitRegisterSigned()`

### Register Range Operations

The register system supports bulk operations on register ranges:
- **SetRegistersBetween(r1, r2, data)**: Sets registers from r1 to r2 (inclusive) with data array
- **GetRegistersBetween(r1, r2)**: Gets registers from r1 to r2 (inclusive) as byte array
- **Direction**: Works for both ascending (r1 ≤ r2) and descending (r1 > r2) ranges

## FloatRegisters Architecture

### Float Register Organization

The float register system uses a **2D array** with **bank switching**:
- **Total Storage**: 256 banks × 16 registers = 4,096 float values
- **Active Bank**: One bank active at a time (16 registers visible)
- **Register Names**: F0, F1, F2, ..., F15
- **Data Type**: Single-precision floating-point (32-bit IEEE 754)

### Float Register Bank System

- **Total Banks**: 256 (0-255)
- **Registers per Bank**: 16
- **Bank Selection**: `SetRegisterBank(byte bankIndex)`
- **Active Registers**: Only registers in the active bank are accessible

### Float Register Operations

- **GetRegister(index)**: Gets float value from register
- **SetRegister(index, value)**: Sets float value to register
- **ExchangeRegisters(index1, index2)**: Swaps two float registers
- **AddFloatValues(v1, v2)**: Adds two float values with flag updates
- **SubFloatValues(v1, v2)**: Subtracts two float values with flag updates

### Float Register Data Format

Float registers store values as:
- **Format**: IEEE 754 single-precision (32-bit)
- **Range**: Approximately ±3.4 × 10³⁸
- **Precision**: 7 decimal digits
- **Special Values**: Supports ±Infinity, NaN

## Flags Architecture

The CPU flags system tracks processor state:

### Flag Indices

Common flag indices (0-31 supported):
- **0 (Z)**: Zero flag - Set when result equals zero
- **1 (C)**: Carry flag - Set on arithmetic overflow/underflow
- **2 (SN)**: Sign flag - Set when result is negative
- **3 (OV)**: Overflow flag - Set on signed arithmetic overflow
- **4 (PO)**: Parity flag - Set based on result parity
- **5 (EQ)**: Equal flag - Set when comparison values are equal
- **6 (GT)**: Greater Than flag - Set when first value > second value
- **7 (LT)**: Less Than flag - Set when first value < second value

### Flag Operations

- **GetValueByIndex(index)**: Gets flag value (true/false)
- **SetValueByIndex(index, value)**: Sets flag value
- **InvertValueByIndex(index)**: Toggles flag value
- **GetFlagsByte()**: Gets all flags as a single byte

### Flag Updates

Flags are automatically updated by:
- **Arithmetic Operations**: ADD, SUB, MUL, DIV update flags based on results
- **Comparison Operations**: CP, SCP update comparison flags
- **Bit Operations**: AND, OR, XOR update Zero flag
- **Shift/Rotate Operations**: Update Carry flag based on shifted bits

## Register Addressing

Registers are addressed by index (0-25 for integer, 0-15 for float):
- **Direct Access**: `Get8BitRegister(index)`
- **Next Register Calculation**: `GetNextRegister(regIndex, distance)` calculates `(regIndex + distance) % 26`
- **Lookup Tables**: `Next1[]`, `Next2[]`, `Next3[]` pre-calculate next register indices

## Memory Integration

Registers integrate with memory operations:
- **Register-to-Memory**: Register values can be written to RAM
- **Memory-to-Register**: RAM values can be loaded into registers
- **Stack Operations**: Registers can be pushed/popped to/from stacks
- **Indirect Addressing**: Registers can hold memory addresses for indirect access

## Performance Considerations

- **Bank Switching**: Fast context switching without register save/restore
- **Lookup Tables**: Pre-calculated next register indices for efficient composition
- **In-place Operations**: Many operations modify registers in-place
- **Flag Updates**: Flags updated efficiently during operations

## Usage Examples

```
; Set active register bank
REGS 1          ; Switch to register bank 1

; Access 8-bit register
LD A, 100       ; Load 100 into register A

; Access 16-bit register
LD BC, 0x1234   ; Load 0x1234 into BC (B=0x12, C=0x34)

; Access 24-bit register
LD XYZ, 0x123456 ; Load 0x123456 into XYZ

; Access 32-bit register
LD ABCD, 0x12345678 ; Load 0x12345678 into ABCD

; Float register operations
LDF F0, 3.14    ; Load 3.14 into float register F0
ADD F0, F1      ; Add F1 to F0
```

## Conclusion

The Continuum 93 CPU architecture provides a flexible register system with bank switching, multiple data sizes, and comprehensive operations. The architecture supports efficient context switching, multi-size register access, and seamless integration with memory operations.

