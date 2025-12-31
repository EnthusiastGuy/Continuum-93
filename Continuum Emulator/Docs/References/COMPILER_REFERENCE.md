# Compiler Reference Guide

## Executive Summary

This document provides essential information for compiler writers targeting Continuum 93 assembly language. It covers instruction encoding, system constants, execution flow, data representations, and compilation processes that are critical for generating correct machine code.

## System Constants and Limits

### Memory Sizes

- **RAM**: 16MB + 3 bytes (16,777,219 bytes)
  - Address range: 0x000000 - 0xFFFFFF (24-bit)
  - Used for: Program code, data, VRAM, palettes

- **RSRAM** (Register Stack): 4MB (4,194,304 bytes)
  - Used for: PUSH/POP operations on registers
  - Pointer: SPR (24-bit)

- **CSRAM** (Call Stack): 1MB (1,048,576 entries × 4 bytes = 4,194,304 bytes)
  - Used for: CALL/RET return addresses
  - Pointer: SPC (24-bit)
  - Entry size: 24-bit address (3 bytes) + 1 byte padding

- **HMEM** (Hardware Memory): 64KB (65,536 entries × 4 bytes = 262,144 bytes)
  - Used for: System variables, GETVAR/SETVAR
  - Entry size: 32-bit unsigned integer
  - Special addresses:
    - `ERROR_HANDLER_ADDRESS`: size - 1
    - `ERROR_ID`: size - 2

### Video Constants

- **V_WIDTH**: 480 pixels
- **V_HEIGHT**: 270 pixels
- **V_SIZE**: 129,600 bytes (480 × 270)
- **Maximum Video Pages**: 8 (layers 0-7)
- **Palette Size**: 768 bytes (256 colors × 3 bytes RGB)
- **Page Size**: 130,368 bytes (129,600 + 768)

### Register Constants

- **General Purpose Registers**: 26 (A-Z)
- **Register Banks**: 256 (0-255)
- **Float Registers**: 16 per bank (F0-F15)
- **Float Register Banks**: 256 (0-255)
- **Register Indices**: 0-25 (A=0, B=1, ..., Z=25)
- **Float Register Indices**: 0-15 (F0=0, F1=1, ..., F15=15)

### Special Registers

- **IPO** (Instruction Pointer): 24-bit address (0x000000 - 0xFFFFFF)
- **SPR** (Stack Pointer for Registers): 24-bit pointer to RSRAM
- **SPC** (Stack Pointer for Calls): 24-bit pointer to CSRAM

### Boolean Constants

- **B_TRUE**: 0xFF
- **B_FALSE**: 0x00

### Address Limits

- **MAX24BIT**: 0xFFFFFF (maximum 24-bit address)
- **Address Wrapping**: Addresses wrap at 0x1000000 boundary

## Instruction Encoding Format

### Opcode Structure

Continuum 93 uses a **two-level opcode system**:

1. **Primary Opcode** (1 byte, 0-255): Identifies the instruction family
2. **Secondary Opcode/Sub-opcode** (1 byte): Identifies the addressing mode or variant

### Primary Opcodes

Primary opcodes are single-byte values (0-255) that identify instruction families:

| Opcode | Hex | Instruction Family |
|--------|-----|-------------------|
| 0 | 0x00 | NOP |
| 1 | 0x01 | LD |
| 2 | 0x02 | ADD, ADD16, ADD24, ADD32 |
| 3 | 0x03 | SUB, SUB16, SUB24, SUB32 |
| 4 | 0x04 | DIV |
| 5 | 0x05 | MUL |
| 6 | 0x06 | SL |
| 7 | 0x07 | SR |
| 8 | 0x08 | RL |
| 9 | 0x09 | RR |
| 10 | 0x0A | SET |
| 11 | 0x0B | RES |
| 12 | 0x0C | BIT |
| 13 | 0x0D | AND, AND16, AND24, AND32 |
| 14 | 0x0E | OR, OR16, OR24, OR32 |
| 15 | 0x0F | XOR, XOR16, XOR24, XOR32 |
| 16 | 0x10 | INV |
| 17 | 0x11 | EX |
| 18 | 0x12 | CP |
| 19 | 0x13 | INC, INC16, INC24, INC32 |
| 20 | 0x14 | DEC, DEC16, DEC24, DEC32 |
| 21 | 0x15 | CALL |
| 22 | 0x16 | CALLR |
| 23 | 0x17 | JP |
| 24 | 0x18 | JR |
| 26 | 0x1A | POP, POP16, POP24, POP32 |
| 28 | 0x1C | PUSH, PUSH16, PUSH24, PUSH32 |
| 29 | 0x1D | INT |
| 30 | 0x1E | RAND |
| 31 | 0x1F | MIN |
| 32 | 0x20 | MAX |
| 33 | 0x21 | DJNZ, DJNZ16, DJNZ24, DJNZ32 |
| 36 | 0x24 | SETF |
| 37 | 0x25 | RESF |
| 38 | 0x26 | INVF |
| 39 | 0x27 | LDF |
| 40 | 0x28 | REGS |
| 41 | 0x29 | WAIT |
| 42 | 0x2A | VDL |
| 43 | 0x2B | VCL |
| 44 | 0x2C | FIND |
| 48 | 0x30 | IMPLY, IMPLY16, IMPLY24, IMPLY32 |
| 49 | 0x31 | NAND, NAND16, NAND24, NAND32 |
| 50 | 0x32 | NOR, NOR16, NOR24, NOR32 |
| 51 | 0x33 | XNOR, XNOR16, XNOR24, XNOR32 |
| 56 | 0x38 | GETBITS |
| 57 | 0x39 | SETBITS |
| 64 | 0x40 | MEMF |
| 65 | 0x41 | MEMC |
| 70 | 0x46 | LDREGS |
| 71 | 0x47 | STREGS |
| 92 | 0x5C | SDIV |
| 93 | 0x5D | SMUL |
| 94 | 0x5E | SCP |
| 95 | 0x5F | FREGS |
| 96 | 0x60 | POW |
| 97 | 0x61 | SQR |
| 98 | 0x62 | CBR |
| 99 | 0x63 | ISQR |
| 100 | 0x64 | ISGN |
| 102 | 0x66 | SIN |
| 103 | 0x67 | COS |
| 104 | 0x68 | TAN |
| 128 | 0x80 | ABS |
| 129 | 0x81 | ROUND |
| 130 | 0x82 | FLOOR |
| 131 | 0x83 | CEIL |
| 160 | 0xA0 | PLAY |
| 190 | 0xBE | SETVAR |
| 191 | 0xBF | GETVAR |
| 192 | 0xC0 | RGB2HSL |
| 193 | 0xC1 | HSL2RGB |
| 194 | 0xC2 | RGB2HSB |
| 195 | 0xC3 | HSB2RGB |
| 252 | 0xFC | DEBUG |
| 253 | 0xFD | BREAK |
| 254 | 0xFE | RETIF |
| 255 | 0xFF | RET |

### Secondary Opcode Encoding

After the primary opcode, a **secondary opcode byte** follows that encodes:
- **Addressing mode** (for instructions with multiple modes)
- **Register indices** (packed in bits)
- **Flag indices** (for conditional instructions)
- **Instruction variants**

#### Common Encoding Patterns

**Pattern 1: Simple Addressing Mode** (e.g., LD, ADD, SUB)
```
[Primary Opcode: 1 byte]
[Secondary Opcode: 1 byte] - Addressing mode constant (0-255)
[Operands: variable length]
```

**Pattern 2: Bit-Packed Registers** (e.g., CP, EX, IMPLY)
```
[Primary Opcode: 1 byte]
[Secondary Opcode: 1 byte]
  - Upper 3 bits: Addressing mode variant
  - Lower 5 bits: First register index or flag index
[Additional bytes: Second register, immediate values, etc.]
```

**Pattern 3: Switch-Based Dispatch** (e.g., CALL, JP, JR)
```
[Primary Opcode: 1 byte]
[Secondary Opcode: 1 byte]
  - Upper 3 bits: Instruction variant (switch case)
  - Lower 5 bits: Flag index (for conditional) or register index
[Operands: address (24-bit) or offset (24-bit signed)]
```

### Addressing Mode Constants

The `Instructions` class defines constants for addressing modes (sub-opcodes):

**8-bit Register Operations** (`_r_*`):
- `_r_n = 0`: Register, immediate
- `_r_r = 1`: Register, register
- `_r_InnnI = 2`: Register, absolute address
- `_r_Innn_nnnI = 3`: Register, absolute address + offset
- `_r_Innn_rI = 4`: Register, absolute address + 8-bit register offset
- `_r_Innn_rrI = 5`: Register, absolute address + 16-bit register offset
- `_r_Innn_rrrI = 6`: Register, absolute address + 24-bit register offset
- `_r_IrrrI = 7`: Register, register-indirect address
- `_r_Irrr_nnnI = 8`: Register, register-indirect address + offset
- `_r_Irrr_rI = 9`: Register, register-indirect address + 8-bit register offset
- `_r_Irrr_rrI = 10`: Register, register-indirect address + 16-bit register offset
- `_r_Irrr_rrrI = 11`: Register, register-indirect address + 24-bit register offset
- `_r_fr = 12`: Register, float register

**16-bit Register Operations** (`_rr_*`):
- Similar pattern with `_rr_` prefix (constants 13-26)

**24-bit Register Operations** (`_rrr_*`):
- Similar pattern with `_rrr_` prefix (constants 27-41)

**32-bit Register Operations** (`_rrrr_*`):
- Similar pattern with `_rrrr_` prefix (constants 42-57)

**Memory Destination Operations** (`_InnnI_*`, `_IrrrI_*`):
- Constants 58-227 for various memory destination addressing modes

**Float Register Operations** (`_fr_*`):
- Constants 228-243 for float register operations

### Register Encoding

Registers are encoded as **5-bit indices** (0-25 for A-Z):

| Register | Index | Binary |
|----------|-------|--------|
| A | 0x00 | 00000 |
| B | 0x01 | 00001 |
| C | 0x02 | 00010 |
| ... | ... | ... |
| Z | 0x19 | 11001 |

**Register Index Extraction**:
- Lower 5 bits: `registerIndex = byte & 0b00011111`
- Used in: `mem.Fetch() & 0b00011111`

**Composite Registers**:
- 16-bit: Two consecutive registers (e.g., AB = A(0) + B(1))
- 24-bit: Three consecutive registers (e.g., ABC = A(0) + B(1) + C(2))
- 32-bit: Four consecutive registers (e.g., ABCD = A(0) + B(1) + C(2) + D(3))

**Register Wrapping**:
- After Z (index 25), wraps to A (index 0)
- Example: ZA = Z(25) + A(0), YZA = Y(24) + Z(25) + A(0)

### Instruction Execution Flow

#### Execution Loop

```
1. Fetch primary opcode from RAM[IPO]
2. Increment IPO by 1
3. Lookup instruction handler in InstructionSet.IJT[opcode]
4. Call handler (e.g., ExLD.Process(computer))
5. Handler fetches secondary opcode and operands
6. Handler executes instruction
7. IPO automatically incremented by fetch operations
8. Repeat
```

#### Instruction Pointer (IPO) Behavior

- **Initial Value**: Set to run address (from `#RUN` or first `#ORG`)
- **Auto-Increment**: Automatically incremented by fetch operations
- **Fetch Operations**: `Fetch()`, `Fetch16()`, `Fetch24()`, `Fetch32()` increment IPO
- **Jump Instructions**: JP, JR, CALL, CALLR modify IPO directly
- **Return Instructions**: RET, RETIF restore IPO from call stack

#### Fetch Operations

All fetch operations automatically increment IPO:

- **Fetch()**: Reads 1 byte, IPO += 1
- **FetchSigned()**: Reads 1 signed byte, IPO += 1
- **Fetch16()**: Reads 2 bytes (big-endian), IPO += 2
- **Fetch16Signed()**: Reads 2 signed bytes, IPO += 2
- **Fetch24()**: Reads 3 bytes (big-endian), IPO += 3
- **Fetch24Signed()**: Reads 3 signed bytes, IPO += 3
- **Fetch32()**: Reads 4 bytes (big-endian), IPO += 4
- **Fetch32Signed()**: Reads 4 signed bytes, IPO += 4

## Data Type Representations

### Integer Types

**8-bit Unsigned** (byte):
- Range: 0-255
- Storage: 1 byte
- Example: `0xFF` = 255

**8-bit Signed** (sbyte):
- Range: -128 to 127
- Storage: 1 byte (two's complement)
- Example: `0xFF` = -1, `0x7F` = 127

**16-bit Unsigned** (ushort):
- Range: 0-65535
- Storage: 2 bytes (big-endian)
- Example: `0x1234` stored as `[0x12, 0x34]`

**16-bit Signed** (short):
- Range: -32768 to 32767
- Storage: 2 bytes (big-endian, two's complement)
- Example: `0xFFFF` = -1

**24-bit Unsigned** (uint, masked):
- Range: 0-16777215 (0xFFFFFF)
- Storage: 3 bytes (big-endian)
- Example: `0x123456` stored as `[0x12, 0x34, 0x56]`

**24-bit Signed** (int, masked):
- Range: -8388608 to 8388607
- Storage: 3 bytes (big-endian, two's complement with sign extension)
- Sign extension: If bit 23 is set, upper byte is 0xFF

**32-bit Unsigned** (uint):
- Range: 0-4294967295
- Storage: 4 bytes (big-endian)
- Example: `0x12345678` stored as `[0x12, 0x34, 0x56, 0x78]`

**32-bit Signed** (int):
- Range: -2147483648 to 2147483647
- Storage: 4 bytes (big-endian, two's complement)
- Example: `0xFFFFFFFF` = -1

### Floating-Point Types

**32-bit Float** (IEEE 754 single-precision):
- Range: Approximately ±3.4 × 10³⁸
- Precision: 7 decimal digits
- Storage: 4 bytes (big-endian)
- Special values: ±Infinity, NaN
- Example: `3.14` stored as IEEE 754 bytes

**Byte Order**: Big-endian (most significant byte first)

## Addressing Modes

### Notation

- `r`, `rr`, `rrr`, `rrrr`: 8, 16, 24, 32-bit register
- `n`, `nn`, `nnn`, `nnnn`: 8, 16, 24, 32-bit immediate value
- `(nnn)`: Absolute memory address (24-bit)
- `(rrr)`: Register-indirect memory address (24-bit register)
- `(nnn, nnn)`: Absolute address + offset
- `(nnn, r)`: Absolute address + 8-bit register offset
- `(nnn, rr)`: Absolute address + 16-bit register offset
- `(nnn, rrr)`: Absolute address + 24-bit register offset
- `(rrr, nnn)`: Register-indirect address + offset
- `(rrr, r)`: Register-indirect address + 8-bit register offset
- `(rrr, rr)`: Register-indirect address + 16-bit register offset
- `(rrr, rrr)`: Register-indirect address + 24-bit register offset
- `fr`: Float register (F0-F15)

### Addressing Mode Examples

**LD r, n**:
```
[Opcode: 0x01]
[Sub-opcode: 0x00] (_r_n)
[Register: 1 byte] (0-25)
[Immediate: 1 byte] (0-255)
```

**LD r, (nnn)**:
```
[Opcode: 0x01]
[Sub-opcode: 0x02] (_r_InnnI)
[Register: 1 byte]
[Address: 3 bytes] (big-endian, 24-bit)
```

**LD r, (nnn, rrr)**:
```
[Opcode: 0x01]
[Sub-opcode: 0x06] (_r_Innn_rrrI)
[Register: 1 byte]
[Base Address: 3 bytes]
[Offset Register: 1 byte] (24-bit register index)
```

**LD rrr, (rrr)**:
```
[Opcode: 0x01]
[Sub-opcode: 0x36] (_rrr_IrrrI)
[Register: 1 byte] (24-bit register index)
[Address Register: 1 byte] (24-bit register index)
```

## Instruction Size Calculation

### Variable-Length Instructions

Instructions have variable lengths based on addressing mode:

**Minimum Size**: 2 bytes (opcode + sub-opcode)
**Maximum Size**: ~15+ bytes (complex addressing with multiple operands)

**Common Sizes**:
- **Simple register operations**: 2-4 bytes
  - `LD r, n`: 3 bytes (opcode + sub-opcode + register + immediate)
  - `LD r, r`: 3 bytes (opcode + sub-opcode + register1 + register2)
  
- **Memory operations**: 4-6 bytes
  - `LD r, (nnn)`: 5 bytes (opcode + sub-opcode + register + 24-bit address)
  - `LD r, (rrr)`: 4 bytes (opcode + sub-opcode + register + address register)
  
- **Indexed operations**: 5-7 bytes
  - `LD r, (nnn, nnn)`: 8 bytes (opcode + sub-opcode + register + base + offset)
  - `LD r, (nnn, r)`: 6 bytes (opcode + sub-opcode + register + base + offset register)
  
- **Jump/Call operations**: 4-5 bytes
  - `JP nnn`: 4 bytes (opcode + sub-opcode + 24-bit address)
  - `JR nnn`: 5 bytes (opcode + sub-opcode + 24-bit signed offset)
  - `CALL nnn`: 4 bytes (opcode + sub-opcode + 24-bit address)
  - `CALLR nnn`: 5 bytes (opcode + sub-opcode + 24-bit signed offset)

### Relative Jump Offset Calculation

For `JR` and `CALLR` (relative instructions):

```
offset = target_address - current_address
```

Where:
- `current_address`: Address of the JR/CALLR instruction
- `target_address`: Address of the target label

**Note**: The instruction size (5 bytes) is automatically accounted for because the instruction pointer (IPO) has already advanced past the instruction when the offset is applied. The JR/CALLR implementation calculates: `IPO = IPO + offset - instruction_size`, where IPO already points to the byte after the instruction.

**Example**:
```
Address 0x1000: JR .Target
Address 0x1005: (next instruction)
Address 0x1020: .Target
```

Calculation:
- Current address: 0x1000
- Target address: 0x1020
- Offset: 0x1020 - 0x1000 = 0x20 (32 decimal)

Execution:
- IPO starts at 0x1000
- After fetching opcode and offset: IPO = 0x1005
- Calculation: IPO = 0x1005 + 0x20 - 5 = 0x1020 ✓

## Compilation Process

### Two-Pass Assembly

The assembler uses a **two-pass** compilation process:

#### Pass 1: Label Collection and Address Assignment

1. **Preprocessing**: Process `#include` directives
2. **Line Parsing**: Parse each line into labels, mnemonics, arguments
3. **Label Collection**: Collect all label definitions
4. **Address Assignment**: Assign addresses to labels based on code generation
5. **Directive Processing**: Process `#ORG`, `#RUN`, `#DB` directives
6. **Address Tracking**: Update global address pointer as code/data is generated

**Key Operations**:
- Labels are assigned addresses: `Labels[label] = GlobalAddressPointer`
- `#ORG` sets `GlobalAddressPointer` to specified address
- `#DB` advances `GlobalAddressPointer` by data length
- Instructions advance `GlobalAddressPointer` by instruction size

#### Pass 2: Code Generation and Label Resolution

1. **Label Resolution**: Resolve all label references to addresses
2. **Code Generation**: Generate machine code bytes
3. **Address Fixup**: Update label references in `#DB` directives
4. **Block Assembly**: Assemble code blocks for each `#ORG` directive

**Key Operations**:
- Absolute addressing: `label_address` → 24-bit address (3 bytes)
- Relative addressing: `label_address - current_address` → signed 24-bit offset
- `#DB` label values refreshed after label resolution

### Code Block Management

Multiple `#ORG` directives create separate code blocks:

- **Block 0**: Default block (address 0)
- **Block 1+**: Blocks created by `#ORG` directives
- Each block has its own origin address
- Blocks are assembled independently
- Final output combines all blocks

### Instruction Encoding Process

For each instruction:

1. **Emit Primary Opcode**: Write instruction's primary opcode byte
2. **Emit Secondary Opcode**: Write addressing mode sub-opcode byte
3. **Emit Operands**: Write register indices, immediate values, addresses
4. **Label Resolution**: If operands contain labels, resolve to addresses/offsets

**Example: Encoding `LD A, .Label`**

```
Pass 1:
- Label .Label found at address 0x1000
- Current instruction at address 0x0500
- Store pending label reference

Pass 2:
- Resolve .Label → 0x1000
- Emit: [0x01] (LD opcode)
- Emit: [0x02] (_r_InnnI sub-opcode)
- Emit: [0x00] (Register A = 0)
- Emit: [0x10, 0x00, 0x00] (Address 0x1000, big-endian)
```

## Stack Operations

### Register Stack (RSRAM)

**Pointer**: SPR (Stack Pointer for Registers)
**Size**: 4MB
**Purpose**: Store register values via PUSH/POP

**Stack Growth**: Upward (SPR increments on PUSH, decrements on POP)
**Initial Value**: 0
**Reset Value**: 100 (when instruction delay iterations reset)

**PUSH Operation**:
```
1. Check stack overflow (SPR + data_size >= RSRAM.Size)
2. If overflow: Set IPO to error handler address
3. Write data to RSRAM[SPR]
4. Increment SPR by data size
```

**POP Operation**:
```
1. Check stack underflow (SPR < data_size)
2. If underflow: Set IPO to error handler address
3. Decrement SPR by data size
4. Read data from RSRAM[SPR]
```

**Data Sizes**:
- 8-bit: 1 byte
- 16-bit: 2 bytes
- 24-bit: 3 bytes
- 32-bit: 4 bytes
- Float: 4 bytes

### Call Stack (CSRAM)

**Pointer**: SPC (Stack Pointer for Calls)
**Size**: 1MB entries (each 24-bit address)
**Purpose**: Store return addresses for CALL/RET

**Stack Growth**: Upward (SPC increments on CALL, decrements on RET)
**Initial Value**: 0

**CALL Operation**:
```
1. Push current IPO to CSRAM[SPC]
2. Increment SPC
3. Set IPO to target address
```

**RET Operation**:
```
1. Check stack underflow (SPC == 0)
2. If underflow: Stop computer
3. Decrement SPC
4. Pop return address from CSRAM[SPC]
5. Set IPO to return address
```

**Entry Format**: 24-bit address (3 bytes, big-endian)

## Error Handling

### Stack Overflow/Underflow

**Register Stack Overflow**:
- Triggered when: `SPR + data_size >= RSRAM.Size`
- Action: Set `IPO = HMEM[ERROR_HANDLER_ADDRESS]`
- Error logged with instruction pointer and register information

**Call Stack Underflow**:
- Triggered when: `SPC == 0` on RET/RETIF
- Action: Stop computer execution
- Prevents invalid return address access

### Division by Zero

**Integer Division**:
- Triggered when: Divisor is zero in DIV/SDIV
- Action: Set `IPO = HMEM[ERROR_HANDLER_ADDRESS]`
- Error ID stored in `HMEM[ERROR_ID]`

### Error Handler Mechanism

- **ERROR_HANDLER_ADDRESS**: Stored at `HMEM[size - 1]`
- **ERROR_ID**: Stored at `HMEM[size - 2]`
- When error occurs: IPO is set to error handler address
- Program can handle errors by checking ERROR_ID

## Flag System Details

### Flag Indices

Flags support **dual representation** (inverted and non-inverted):

| Index | Name (Non-Inverted) | Name (Inverted) | Meaning |
|-------|---------------------|-----------------|---------|
| 0 | Z | NZ | Zero / Not Zero |
| 1 | C | NC | Carry / No Carry |
| 2 | SN | SP | Sign Negative / Sign Positive |
| 3 | OV | NO | Overflow / No Overflow |
| 4 | PO | PE | Parity Odd / Parity Even |
| 5 | EQ | NE | Equal / Not Equal |
| 6 | GT | LTE | Greater Than / Less Than or Equal |
| 7 | LT | GTE | Less Than / Greater Than or Equal |

**Flag Access**:
- Indices 0-7: Inverted logic (false = set, true = clear)
- Indices 8-15: Non-inverted logic (true = set, false = clear)
- Index 8 = Z, 9 = C, 10 = SN, 11 = OV, 12 = PO, 13 = EQ, 14 = GT, 15 = LT

**Flag Storage**: 8 boolean values (bits 0-7)
**Flag Byte**: Single byte with bits set for true flags

## Instruction Dispatch

### Jump Table (IJT)

The emulator uses a **256-entry jump table** for instruction dispatch:

```csharp
Action<Computer>[] IJT = new Action<Computer>[256];
IJT[opcode] = InstructionHandler.Process;
```

**Dispatch Process**:
1. Fetch primary opcode (1 byte)
2. Lookup handler: `IJT[opcode]`
3. Call handler: `handler(computer)`
4. Handler processes secondary opcode and operands

**Benefits**:
- O(1) instruction dispatch
- No switch-case overhead
- Fast execution

## Compiler Implementation Guidelines

### Instruction Encoding

When encoding instructions:

1. **Emit Primary Opcode**: Write the instruction's primary opcode byte
2. **Emit Secondary Opcode**: Write the addressing mode sub-opcode
3. **Emit Register Indices**: Write register indices (5 bits each, masked with `0b00011111`)
4. **Emit Immediate Values**: Write immediate values in appropriate size (1, 2, 3, or 4 bytes)
5. **Emit Addresses**: Write addresses as 24-bit big-endian (3 bytes)
6. **Emit Offsets**: Write relative offsets as signed 24-bit big-endian (3 bytes)

### Label Resolution

**Absolute Labels** (for JP, CALL, LD with address, etc.):
```
label_address → 24-bit address (3 bytes, big-endian)
```

**Relative Labels** (for JR, CALLR):
```
offset = label_address - current_address
→ signed 24-bit offset (3 bytes, big-endian, two's complement)
```

Note: The instruction size is automatically accounted for during execution, so the offset is simply the difference between the target and current addresses.

**Instruction Size for Relative Jumps**:
- `JR`: 5 bytes
- `CALLR`: 5 bytes

### Register Allocation

**Register Mapping**:
- Map source language variables to registers A-Z
- Use register banks for context switching
- Consider register lifetimes for optimization

**Register Spilling**:
- Use PUSH/POP for register preservation
- Use register stack (RSRAM) for temporary storage
- Consider register bank switching for function contexts

### Memory Layout

**Typical Layout**:
```
0x000000 - 0x07FFFF: Program code (512KB reserved for OS)
0x080000 - 0xEFFFFF: User code and data
0xF00000 - 0xFFFFFF: Video RAM and palettes
```

**Code Organization**:
- Use `#ORG` to set code section addresses
- Use separate `#ORG` for data sections
- Consider memory alignment for performance

### Optimization Opportunities

1. **Register Reuse**: Reuse registers within basic blocks
2. **Register Bank Switching**: Use banks for function contexts (avoid PUSH/POP)
3. **Relative Jumps**: Prefer JR/CALLR for short-range jumps (smaller encoding)
4. **Immediate Values**: Use immediate addressing when possible (faster than memory)
5. **Register-Indirect**: Use register-indirect for array access
6. **Instruction Selection**: Choose optimal addressing mode for each operation

## Example: Compiling a Simple Statement

**Source**: `x = y + 10`

**Compiler Steps**:

1. **Allocate Registers**:
   - `x` → Register A
   - `y` → Register B (assume in memory at .y_address)

2. **Load y**:
   ```
   LD A, (.y_address)
   ```
   Encoded as:
   ```
   [0x01] [0x02] [0x00] [0x12, 0x34, 0x56]
   ^      ^      ^      ^
   LD     _r_    A      .y_address (0x123456)
   InnnI
   ```

3. **Add 10**:
   ```
   ADD A, 10
   ```
   Encoded as:
   ```
   [0x02] [0x00] [0x00] [0x0A]
   ^      ^      ^      ^
   ADD    _r_n   A      10
   ```

4. **Store x**:
   ```
   LD (.x_address), A
   ```
   Encoded as:
   ```
   [0x01] [0x24] [0x00] [0x12, 0x34, 0x50]
   ^      ^      ^      ^
   LD     _InnnI A      .x_address (0x123450)
   _r
   ```

## Conclusion

This reference guide provides the essential information needed to implement a compiler for Continuum 93 assembly language. Key areas to focus on:

1. **Instruction Encoding**: Understand opcode structure and addressing modes
2. **Label Resolution**: Implement two-pass assembly with proper offset calculation
3. **Register Management**: Map variables to registers efficiently
4. **Memory Layout**: Organize code and data sections appropriately
5. **Stack Management**: Handle PUSH/POP and CALL/RET correctly
6. **Error Handling**: Implement proper stack overflow/underflow checks

Refer to the individual instruction documentation files for detailed instruction-specific encoding requirements.

