# RAM Architecture Documentation

## Executive Summary

The Continuum 93 RAM architecture consists of **four distinct memory regions**: **RAM** (general-purpose memory), **RSRAM** (register stack memory), **CSRAM** (call stack memory), and **HMEM** (hardware memory for variables). The architecture supports 24-bit addressing (16MB address space), big-endian byte ordering, and specialized memory operations including bit-addressed memory and string operations.

## File Structure

- **Memory.cs**: Core memory class with byte array storage and operations
- **MemoryController.cs**: Memory controller managing all memory regions and fetch operations
- **HardwareMemory.cs**: Hardware memory (HMEM) for system variables
- **StackMemory.cs**: Stack memory for call stack (CSRAM)
- **SystemMessages.cs**: System error messages

## Memory Regions

### 1. RAM (General-Purpose Memory)

**Size**: 16MB + 3 bytes (16,777,219 bytes)
**Purpose**: Main program memory, ROM data, VRAM (video RAM)
**Address Range**: 0x000000 - 0xFFFFFF (24-bit addressing)
**Data Type**: Byte array

#### RAM Organization
- **Program Code**: Stored starting at address 0x000000
- **Data Storage**: General-purpose data storage
- **Video RAM (VRAM)**: Located at high addresses (end of RAM)
- **Palette Data**: Stored adjacent to VRAM

#### RAM Operations
- **Byte Access**: `Get8bitFromRAM(address)` / `Set8bitToRAM(address, value)`
- **16-bit Access**: `Get16bitFromRAM(address)` / `Set16bitToRAM(address, value)` (big-endian)
- **24-bit Access**: `Get24bitFromRAM(address)` / `Set24bitToRAM(address, value)` (big-endian)
- **32-bit Access**: `Get32bitFromRAM(address)` / `Set32bitToRAM(address, value)` (big-endian)
- **Float Access**: `GetFloatFromRAM(address)` / `SetFloatToRam(address, value)` (IEEE 754)
- **String Access**: `GetStringAt(address)` / `SetStringAt(str, address)` (UTF-8, null-terminated)

#### Bit-Addressed Memory

RAM supports bit-level addressing for efficient bit manipulation:

- **Get8BitValueFromBitMemoryAt(bitAddress, bits)**: Reads 1-8 bits from bit address
- **Get16BitValueFromBitMemoryAt(bitAddress, bits)**: Reads 1-16 bits from bit address
- **Get24BitValueFromBitMemoryAt(bitAddress, bits)**: Reads 1-24 bits from bit address
- **Get32BitValueFromBitMemoryAt(bitAddress, bits)**: Reads 1-32 bits from bit address
- **Set8BitValueToBitMemoryAt(value, bitAddress, bits)**: Writes 1-8 bits to bit address
- **Set16BitValueToBitMemoryAt(value, bitAddress, bits)**: Writes 1-16 bits to bit address
- **Set24BitValueToBitMemoryAt(value, bitAddress, bits)**: Writes 1-24 bits to bit address
- **Set32BitValueToBitMemoryAt(value, bitAddress, bits)**: Writes 1-32 bits to bit address

**Bit Addressing**:
- **Bit Address**: Addresses individual bits (not bytes)
- **Byte Address**: `byteAddress = bitAddress >> 3`
- **Bit Offset**: `bitInByte = bitAddress & 7`
- **Big-Endian**: Bits read/written in big-endian order

#### Memory Operations
- **Copy**: `Copy(sourceAddress, destAddress, length)` - Copies memory blocks
- **Fill**: `Fill(value, start, count)` - Fills memory with byte value
- **FillRect**: `FillRect(value, start, width, height)` - Fills rectangular region
- **Find**: `Find(address, value)` - Finds byte value starting from address
- **FindPattern**: `FindPattern(address, pattern)` - Finds byte pattern (string search)

### 2. RSRAM (Register Stack Memory)

**Size**: 4MB (4,194,304 bytes)
**Purpose**: Stack for register values (PUSH/POP operations)
**Pointer**: SPR (Stack Pointer for Registers)
**Data Type**: Byte array

#### RSRAM Operations
- **Get8BitFromRegStack(address)**: Gets 8-bit value from register stack
- **Set8BitToRegStack(address, value)**: Sets 8-bit value to register stack
- **Get16BitFromRegStack(address)**: Gets 16-bit value (big-endian)
- **Set16BitToRegStack(address, value)**: Sets 16-bit value (big-endian)
- **Get24BitFromRegStack(address)**: Gets 24-bit value (big-endian)
- **Set24BitToRegStack(address, value)**: Sets 24-bit value (big-endian)
- **Get32BitFromRegStack(address)**: Gets 32-bit value (big-endian)
- **Set32BitToRegStack(address, value)**: Sets 32-bit value (big-endian)

#### Stack Pointer (SPR)
- **Initial Value**: 0
- **Increment**: Incremented on PUSH operations
- **Decrement**: Decremented on POP operations
- **Overflow Check**: Stack overflow checked before PUSH
- **Underflow Check**: Stack underflow checked before POP

### 3. CSRAM (Call Stack Memory)

**Size**: 1MB (1,048,576 entries, each 24-bit)
**Purpose**: Stack for return addresses (CALL/RET operations)
**Pointer**: SPC (Stack Pointer for Calls)
**Data Type**: 24-bit address array

#### CSRAM Operations
- **GetFromCallStack(address)**: Gets return address from call stack
- **SetToCallStack(address, value)**: Sets return address to call stack

#### Stack Pointer (SPC)
- **Initial Value**: 0
- **Increment**: Incremented on CALL/CALLR operations
- **Decrement**: Decremented on RET/RETIF operations
- **Underflow Check**: Stack underflow checked before RET/RETIF

### 4. HMEM (Hardware Memory)

**Size**: 64KB (65,536 entries, each 32-bit)
**Purpose**: System variables and hardware state
**Data Type**: 32-bit unsigned integer array

#### HMEM Special Addresses
- **ERROR_HANDLER_ADDRESS**: `size - 1` - Address of error handler
- **ERROR_ID**: `size - 2` - Error ID storage

#### HMEM Operations
- **Direct Access**: `HMEM[address] = value` / `value = HMEM[address]`
- **Variable Storage**: Used by GETVAR/SETVAR instructions
- **System State**: Stores hardware and system state variables

## MemoryController

The `MemoryController` manages all memory regions and provides instruction fetch operations.

### Fetch Operations

The MemoryController provides instruction fetch operations that automatically increment the Instruction Pointer (IPO):

- **Fetch()**: Fetches 8-bit value, increments IPO by 1
- **FetchSigned()**: Fetches signed 8-bit value, increments IPO by 1
- **Fetch16()**: Fetches 16-bit value (big-endian), increments IPO by 2
- **Fetch16Signed()**: Fetches signed 16-bit value, increments IPO by 2
- **Fetch24()**: Fetches 24-bit value (big-endian), increments IPO by 3
- **Fetch24Signed()**: Fetches signed 24-bit value, increments IPO by 3
- **Fetch32()**: Fetches 32-bit value (big-endian), increments IPO by 4
- **Fetch32Signed()**: Fetches signed 32-bit value, increments IPO by 4

### Safe Memory Access

The MemoryController provides safe memory access that wraps addresses:
- **GetSafe8bitFromRAM(address)**: Wraps address to 24-bit range
- **GetSafe16bitFromRAM(address)**: Wraps address to 24-bit range
- **GetSafe24bitFromRAM(address)**: Wraps address to 24-bit range
- **GetSafe32bitFromRAM(address)**: Wraps address to 24-bit range

### String Operations

- **GetStringAt(address)**: Reads null-terminated UTF-8 string from memory
- **SetStringAt(str, address)**: Writes UTF-8 string to memory with null terminator
- **GetStringBytesAt(address)**: Gets string bytes (without null terminator)

### Memory Utilities

- **GetMemoryWrapped(address, length)**: Gets memory with address wrapping
- **DumpMemAt(address, length)**: Dumps memory region to byte array
- **GetMemoryPointedByAllAddressingRegisters()**: Gets memory samples from all 24-bit registers

### Stack Management

- **ResetAllStacks()**: Clears both RSRAM and CSRAM, resets pointers
- **ClearAllRAM()**: Clears main RAM
- **ClearHMEM()**: Clears hardware memory

## Byte Ordering (Endianness)

All multi-byte values use **big-endian** (most significant byte first) ordering:

### 16-bit Values
- **Byte 0**: High byte (bits 15-8)
- **Byte 1**: Low byte (bits 7-0)
- **Example**: 0x1234 stored as [0x12, 0x34]

### 24-bit Values
- **Byte 0**: High byte (bits 23-16)
- **Byte 1**: Middle byte (bits 15-8)
- **Byte 2**: Low byte (bits 7-0)
- **Example**: 0x123456 stored as [0x12, 0x34, 0x56]

### 32-bit Values
- **Byte 0**: High byte (bits 31-24)
- **Byte 1**: Second byte (bits 23-16)
- **Byte 2**: Third byte (bits 15-8)
- **Byte 3**: Low byte (bits 7-0)
- **Example**: 0x12345678 stored as [0x12, 0x34, 0x56, 0x78]

## Memory Addressing

### Address Space

- **Total Addressable**: 16MB (24-bit addressing: 0x000000 - 0xFFFFFF)
- **Address Type**: 24-bit unsigned integer
- **Address Wrapping**: Addresses wrap at 0x1000000 boundary

### Addressing Modes

1. **Absolute Addressing**: Direct 24-bit address
   - Example: `LD A, (0x1000)`

2. **Register-Indirect Addressing**: Address stored in 24-bit register
   - Example: `LD A, (XYZ)`

3. **Indexed Addressing**: Base address + offset
   - Example: `LD A, (XYZ + 10)`

## Memory Layout

### Typical Memory Layout

```
0x000000 - 0x0FFFFF: Program Code and Data
0x100000 - 0xEFFFFF: General Data Storage
0xF00000 - 0xFFFFFF: Video RAM and Palettes
  - VRAM Pages: Video frame buffers
  - Palette Data: Color palettes (768 bytes per palette)
```

### Video RAM Layout

Video RAM is located at the end of main RAM:
- **VRAM Offset**: `0x1000000 - V_SIZE * VRAM_PAGES`
- **Page Size**: 480 × 270 = 129,600 bytes per page
- **Palette Size**: 256 colors × 3 bytes = 768 bytes per palette
- **Total Page Size**: 129,600 + 768 = 130,368 bytes per page

## Memory Operations

### Copy Operations

```csharp
RAM.Copy(sourceAddress, destAddress, length)
```
- Copies `length` bytes from `sourceAddress` to `destAddress`
- Handles overlapping regions safely
- Uses `Array.Copy()` for efficiency

### Fill Operations

```csharp
RAM.Fill(value, start, count)
```
- Fills `count` bytes starting at `start` with `value`
- Uses `Array.Fill()` for efficiency

### Search Operations

```csharp
RAM.Find(startAddress, value)
```
- Finds first occurrence of `value` starting from `startAddress`
- Returns address of match or end address if not found

```csharp
RAM.FindPattern(startAddress, pattern)
```
- Finds first occurrence of byte pattern (string) starting from `startAddress`
- Returns address of match or 0xFFFFFF if not found

## Memory Safety

### Bounds Checking

- **RAM**: Bounds checked on bit-addressed operations
- **RSRAM**: Stack overflow/underflow checked
- **CSRAM**: Stack underflow checked
- **HMEM**: Bounds checked on array access

### Error Handling

- **Stack Overflow**: Triggers error handler interrupt
- **Stack Underflow**: Triggers error handler interrupt
- **Out of Bounds**: Throws `IndexOutOfRangeException` or `ArgumentOutOfRangeException`

## Performance Considerations

- **Volatile Arrays**: RAM uses `volatile byte[]` for thread safety
- **Aggressive Inlining**: Fetch operations use `[MethodImpl(MethodImplOptions.AggressiveInlining)]`
- **Efficient Copying**: Uses `Array.Copy()` and `Unsafe.CopyBlockUnaligned()` for fast memory operations
- **Bit Operations**: Optimized bit-addressed memory operations

## Usage Examples

```
; Memory access
LD A, (0x1000)      ; Load byte from absolute address
LD A, (XYZ)         ; Load byte from register-indirect address
LD BC, (0x1000)     ; Load 16-bit value (big-endian)

; Stack operations
PUSH A              ; Push register A to register stack
POP A               ; Pop register A from register stack

; Variable access
GETVAR 0, ABCD      ; Get variable 0, store in ABCD
SETVAR 0, ABCD      ; Set variable 0 from ABCD

; Memory operations
MEMC XYZ, ABC, DEF  ; Copy memory from XYZ to ABC, length from DEF
MEMF 0x1000, 100, 0 ; Fill memory at 0x1000 with 0, length 100
FIND (XYZ), 0x00    ; Find null byte starting from memory[XYZ]
```

## Conclusion

The Continuum 93 RAM architecture provides comprehensive memory management with four distinct regions, big-endian byte ordering, bit-addressed memory, and efficient memory operations. The architecture supports 24-bit addressing, stack management, and specialized memory operations for video and hardware state.

