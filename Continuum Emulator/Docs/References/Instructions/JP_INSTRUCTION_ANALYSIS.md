# JP Instruction Execution Analysis

## Executive Summary

The JP (Jump) instruction implementation in `ExJP.cs` provides **4 distinct instruction variants** for unconditional and conditional absolute jumps. JP sets the instruction pointer (IPO) to an absolute address, enabling program control flow.

## File Statistics

- **File**: `Emulator/Execution/ExJP.cs`
- **Total Lines**: 58 lines
- **Instruction Variants**: 4 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 3 bits of opcode

## Addressing Modes

### 1. Unconditional Jumps

- `JP nnn` - Jump to absolute 24-bit address
  - Fetches 24-bit address from instruction stream
  - Sets `IPO = address`
  - No condition check

- `JP rrr` - Jump to address stored in 24-bit register
  - Register index encoded in lower 5 bits of opcode
  - Reads address from register
  - Sets `IPO = register_value`

### 2. Conditional Jumps

- `JP ff, nnn` - Jump to absolute address if flag is set
  - Flag index encoded in lower 5 bits of opcode
  - Fetches 24-bit address from instruction stream
  - Only jumps if `FLAGS.GetValueByIndex(flagIndex)` is true
  - Uses flag index 0-31

- `JP ff, rrr` - Jump to address in register if flag is set
  - Flag index encoded in lower 5 bits of opcode
  - Register index fetched from next byte
  - Only jumps if flag is set
  - Address read from 24-bit register

## Opcode Encoding

JP uses a compact encoding scheme:
1. **Primary Opcode**: Opcode 23 (JP)
2. **Secondary Opcode**: Upper 3 bits of next byte (`ldOp >> 5`)
3. **Flag/Register Index**: Lower 5 bits of opcode (`ldOp & 0b00011111`)

## Flag Usage

JP does not modify flags, but uses them for conditional jumps:
- **Flag Index 0**: Zero flag (Z)
- **Flag Index 1**: Carry flag (C)
- **Flag Index 2**: Sign flag (SN)
- **Flag Index 3**: Overflow flag (OV)
- **Flag Index 4**: Parity flag (PO)
- **Flag Index 5**: Equal flag (EQ)
- **Flag Index 6**: Greater Than flag (GT)
- **Flag Index 7**: Less Than flag (LT)
- **Flag Indices 8-31**: Additional flags

## Implementation Details

### Address Fetching

For immediate addresses:
```csharp
uint address = computer.MEMC.Fetch24();
computer.CPU.REGS.IPO = address;
```

For register addresses:
```csharp
byte registerIndex = (byte)(ldOp & 0b00011111);
uint address = computer.CPU.REGS.Get24BitRegister(registerIndex);
computer.CPU.REGS.IPO = address;
```

### Conditional Execution

Conditional jumps check the flag before updating IPO:
```csharp
byte flagIndex = (byte)(ldOp & 0b00011111);
if (computer.CPU.FLAGS.GetValueByIndex(flagIndex))
{
    computer.CPU.REGS.IPO = address;
}
```

## Operation Categories Summary

1. **Unconditional Jumps**: 2 variants
   - Immediate address: 1 variant
   - Register address: 1 variant

2. **Conditional Jumps**: 2 variants
   - Immediate address: 1 variant
   - Register address: 1 variant

## Comparison with JR Instruction

- **JP**: Absolute addressing (sets IPO to exact address)
- **JR**: Relative addressing (adds offset to current IPO)
- **JP**: More flexible for long-distance jumps
- **JR**: More compact for short-range jumps

## Usage Examples

### Unconditional Jumps
```
JP 0x1000        ; Jump to address 0x1000
JP XYZ           ; Jump to address stored in register XYZ
```

### Conditional Jumps
```
JP Z, 0x1000     ; Jump to 0x1000 if zero flag is set
JP C, XYZ        ; Jump to address in XYZ if carry flag is set
JP GT, loop_start ; Jump if greater than flag is set
```

## Conclusion

The JP instruction provides essential control flow capabilities with both unconditional and conditional variants. The ability to jump to either immediate addresses or register-stored addresses provides flexibility for dynamic program control.

