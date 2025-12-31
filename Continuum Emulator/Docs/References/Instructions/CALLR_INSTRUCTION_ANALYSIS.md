# CALLR Instruction Execution Analysis

## Executive Summary

The CALLR (Call Relative) instruction implementation in `ExCALLR.cs` provides **2 distinct instruction variants** for unconditional and conditional relative subroutine calls. CALLR saves the return address to the call stack and jumps relative to the current instruction pointer, making it ideal for short-range subroutine calls.

## File Statistics

- **File**: `Emulator/Execution/ExCALLR.cs`
- **Total Lines**: 40 lines
- **Instruction Variants**: 2 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 3 bits of opcode
- **Instruction Size**: 5 bytes (used for offset calculation)

## Addressing Modes

### 1. Unconditional Relative Call

- `CALLR nnn` - Call subroutine relative by signed 24-bit offset
  - Fetches signed 24-bit offset from instruction stream
  - Saves current IPO to call stack: `SPC++`, then `callStack[SPC] = IPO`
  - Calculates: `IPO = IPO + offset - 5`
  - The `-5` accounts for the instruction size itself
  - Offset can be positive (forward) or negative (backward)

### 2. Conditional Relative Call

- `CALLR ff, nnn` - Call subroutine relative if flag is set
  - Flag index encoded in lower 5 bits of opcode
  - Fetches signed 24-bit offset from instruction stream
  - Only calls if `FLAGS.GetValueByIndex(flagIndex)` is true
  - Saves return address and calculates jump only if condition is met
  - Calculates: `IPO = IPO + offset - 5` (if condition met)

## Opcode Encoding

CALLR uses a compact encoding scheme:
1. **Primary Opcode**: Opcode 22 (CALLR)
2. **Secondary Opcode**: Upper 3 bits of next byte (`ldOp >> 5`)
3. **Flag Index**: Lower 5 bits of opcode (`ldOp & 0b00011111`)

## Offset Calculation

The relative call calculation accounts for instruction size:
```csharp
int CALLR_InstructionSize = 5;
int addressOffset = computer.MEMC.Fetch24Signed();
computer.MEMC.SetToCallStack(computer.CPU.REGS.SPC++, computer.CPU.REGS.IPO);
computer.CPU.REGS.IPO = (uint)(computer.CPU.REGS.IPO + addressOffset - CALLR_InstructionSize);
```

The `-5` ensures that the call target is relative to the instruction following CALLR, not the CALLR instruction itself.

## Call Stack Management

CALLR uses the same call stack mechanism as CALL:
- **SPC Register**: Stack Pointer for Calls (incremented before storing)
- **SetToCallStack**: Stores the return address at the current stack position
- **Stack Growth**: Stack grows upward (SPC increments)

## Flag Usage

CALLR does not modify flags, but uses them for conditional calls:
- Same flag indices as JP/JR/CALL instructions
- Flag Index 0-31 available for conditional calls

## Implementation Details

### Signed Offset

The offset is fetched as a signed 24-bit value:
- Range: -8,388,608 to +8,388,607 bytes
- Allows calling both forward and backward in code
- Negative offsets enable calling earlier subroutines

### Conditional Execution

Conditional calls only save return address if condition is met:
```csharp
if (computer.CPU.FLAGS.GetValueByIndex(flagIndex))
{
    computer.MEMC.SetToCallStack(computer.CPU.REGS.SPC++, computer.CPU.REGS.IPO);
    computer.CPU.REGS.IPO = (uint)(computer.CPU.REGS.IPO + addressOffset - CALLR_InstructionSize);
}
```

## Operation Categories Summary

1. **Unconditional Relative Calls**: 1 variant
2. **Conditional Relative Calls**: 1 variant

## Comparison with CALL Instruction

- **CALLR**: Relative addressing (offset from current position)
- **CALL**: Absolute addressing (exact address)
- **CALLR**: More compact for short-range calls
- **CALL**: More flexible for long-distance calls
- **CALLR**: Position-independent code friendly
- **CALL**: Requires knowing exact addresses

## Comparison with JR Instruction

- **CALLR**: Saves return address to call stack
- **JR**: Does not save return address
- **CALLR**: Enables subroutine return via RET
- **JR**: One-way jump only

## Usage Examples

### Unconditional Relative Calls
```
CALLR 100          ; Call subroutine 100 bytes forward
CALLR -50          ; Call subroutine 50 bytes backward
```

### Conditional Relative Calls
```
CALLR Z, 20        ; Call subroutine 20 bytes forward if zero flag is set
CALLR C, -10       ; Call subroutine 10 bytes backward if carry flag is set
CALLR GT, func     ; Call function if greater than flag is set
```

## Typical Use Cases

1. **Local Subroutines**: Short-range function calls within same code section
2. **Position-Independent Code**: Calls that work regardless of code location
3. **Conditional Function Calls**: Call functions based on conditions
4. **Compact Code**: Reduce code size for nearby function calls

## Conclusion

The CALLR instruction provides efficient relative subroutine calling capabilities, ideal for position-independent code and short-range function calls. The signed offset allows both forward and backward calls, making it perfect for local subroutines and compact code generation.

