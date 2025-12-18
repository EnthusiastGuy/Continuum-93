# JR Instruction Execution Analysis

## Executive Summary

The JR (Jump Relative) instruction implementation in `ExJR.cs` provides **2 distinct instruction variants** for unconditional and conditional relative jumps. JR adds a signed offset to the current instruction pointer, making it ideal for short-range jumps within code.

## File Statistics

- **File**: `Emulator/Execution/ExJR.cs`
- **Total Lines**: 37 lines
- **Instruction Variants**: 2 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 3 bits of opcode
- **Instruction Size**: 5 bytes (used for offset calculation)

## Addressing Modes

### 1. Unconditional Relative Jump

- `JR nnn` - Jump relative by signed 24-bit offset
  - Fetches signed 24-bit offset from instruction stream
  - Calculates: `IPO = IPO + offset - 5`
  - The `-5` accounts for the instruction size itself
  - Offset can be positive (forward) or negative (backward)

### 2. Conditional Relative Jump

- `JR ff, nnn` - Jump relative if flag is set
  - Flag index encoded in lower 5 bits of opcode
  - Fetches signed 24-bit offset from instruction stream
  - Only jumps if `FLAGS.GetValueByIndex(flagIndex)` is true
  - Calculates: `IPO = IPO + offset - 5` (if condition met)

## Opcode Encoding

JR uses a compact encoding scheme:
1. **Primary Opcode**: Opcode 24 (JR)
2. **Secondary Opcode**: Upper 3 bits of next byte (`ldOp >> 5`)
3. **Flag Index**: Lower 5 bits of opcode (`ldOp & 0b00011111`)

## Offset Calculation

The relative jump calculation accounts for instruction size:
```csharp
int JR_InstructionSize = 5;
int addressOffset = computer.MEMC.Fetch24Signed();
computer.CPU.REGS.IPO = (uint)(computer.CPU.REGS.IPO + addressOffset - JR_InstructionSize);
```

The `-5` ensures that the jump target is relative to the instruction following JR, not the JR instruction itself.

## Flag Usage

JR does not modify flags, but uses them for conditional jumps:
- Same flag indices as JP instruction
- Flag Index 0-31 available for conditional jumps

## Implementation Details

### Signed Offset

The offset is fetched as a signed 24-bit value:
- Range: -8,388,608 to +8,388,607 bytes
- Allows jumping both forward and backward in code
- Negative offsets enable backward jumps (loops)

### Conditional Execution

Conditional jumps check the flag before updating IPO:
```csharp
byte flagIndex = (byte)(ldOp & 0b00011111);
if (computer.CPU.FLAGS.GetValueByIndex(flagIndex))
    computer.CPU.REGS.IPO = (uint)(computer.CPU.REGS.IPO + addressOffset - JR_InstructionSize);
```

## Operation Categories Summary

1. **Unconditional Relative Jumps**: 1 variant
2. **Conditional Relative Jumps**: 1 variant

## Comparison with JP Instruction

- **JR**: Relative addressing (offset from current position)
- **JP**: Absolute addressing (exact address)
- **JR**: More compact for short-range jumps
- **JP**: More flexible for long-distance jumps
- **JR**: Position-independent code friendly
- **JP**: Requires knowing exact addresses

## Usage Examples

### Unconditional Relative Jumps
```
JR 100          ; Jump forward 100 bytes
JR -50          ; Jump backward 50 bytes
```

### Conditional Relative Jumps
```
JR Z, 20        ; Jump forward 20 bytes if zero flag is set
JR C, -10       ; Jump backward 10 bytes if carry flag is set
JR GT, loop     ; Jump to loop label if greater than
```

## Typical Use Cases

1. **Loops**: Backward jumps to loop start
2. **Conditional Branches**: Short-range if/else constructs
3. **Function Calls**: Short-range subroutine calls (though CALL is preferred)
4. **Error Handling**: Jump to error handlers nearby

## Conclusion

The JR instruction provides efficient relative jumping capabilities, ideal for position-independent code and short-range control flow. The signed offset allows both forward and backward jumps, making it perfect for loops and conditional branches.

