# CALL Instruction Execution Analysis

## Executive Summary

The CALL instruction implementation in `ExCALL.cs` provides **4 distinct instruction variants** for unconditional and conditional subroutine calls. CALL saves the return address to the call stack and jumps to the target address, enabling function/subroutine invocation.

## File Statistics

- **File**: `Emulator/Execution/ExCALL.cs`
- **Total Lines**: 62 lines
- **Instruction Variants**: 4 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 3 bits of opcode

## Addressing Modes

### 1. Unconditional Calls

- `CALL nnn` - Call subroutine at absolute 24-bit address
  - Fetches 24-bit address from instruction stream
  - Saves current IPO to call stack: `SPC++`, then `callStack[SPC] = IPO`
  - Sets `IPO = address`
  - Enables return via RET instruction

- `CALL rrr` - Call subroutine at address stored in 24-bit register
  - Register index encoded in lower 5 bits of opcode
  - Reads address from register
  - Saves current IPO to call stack
  - Sets `IPO = register_value`

### 2. Conditional Calls

- `CALL ff, nnn` - Call subroutine at absolute address if flag is set
  - Flag index encoded in lower 5 bits of opcode
  - Fetches 24-bit address from instruction stream
  - Only calls if `FLAGS.GetValueByIndex(flagIndex)` is true
  - Saves return address only if condition is met

- `CALL ff, rrr` - Call subroutine at address in register if flag is set
  - Flag index encoded in lower 5 bits of opcode
  - Register index fetched from next byte
  - Only calls if flag is set
  - Address read from 24-bit register

## Opcode Encoding

CALL uses the same encoding scheme as JP:
1. **Primary Opcode**: Opcode 21 (CALL)
2. **Secondary Opcode**: Upper 3 bits of next byte (`ldOp >> 5`)
3. **Flag/Register Index**: Lower 5 bits of opcode (`ldOp & 0b00011111`)

## Call Stack Management

CALL uses the Stack Pointer for Calls (SPC) register:
```csharp
computer.MEMC.SetToCallStack(computer.CPU.REGS.SPC++, computer.CPU.REGS.IPO);
computer.CPU.REGS.IPO = address;
```

The call stack stores return addresses, allowing the RET instruction to restore execution to the instruction following CALL.

## Flag Usage

CALL does not modify flags, but uses them for conditional calls:
- Same flag indices as JP/JR instructions
- Flag Index 0-31 available for conditional calls

## Implementation Details

### Stack Operations

The call stack is managed through:
- **SPC Register**: Stack Pointer for Calls (incremented before storing)
- **SetToCallStack**: Stores the return address at the current stack position
- **Stack Growth**: Stack grows upward (SPC increments)

### Address Fetching

Same as JP instruction:
- Immediate addresses: `Fetch24()`
- Register addresses: `Get24BitRegister(registerIndex)`

### Conditional Execution

Conditional calls only save return address if condition is met:
```csharp
if (computer.CPU.FLAGS.GetValueByIndex(flagIndex))
{
    computer.MEMC.SetToCallStack(computer.CPU.REGS.SPC++, computer.CPU.REGS.IPO);
    computer.CPU.REGS.IPO = address;
}
```

## Operation Categories Summary

1. **Unconditional Calls**: 2 variants
   - Immediate address: 1 variant
   - Register address: 1 variant

2. **Conditional Calls**: 2 variants
   - Immediate address: 1 variant
   - Register address: 1 variant

## Comparison with CALLR Instruction

- **CALL**: Absolute addressing (exact address)
- **CALLR**: Relative addressing (offset from current position)
- **CALL**: More flexible for long-distance calls
- **CALLR**: More compact for short-range calls

## Comparison with JP Instruction

- **CALL**: Saves return address to call stack
- **JP**: Does not save return address
- **CALL**: Enables subroutine return via RET
- **JP**: One-way jump only

## Usage Examples

### Unconditional Calls
```
CALL 0x1000        ; Call subroutine at address 0x1000
CALL XYZ           ; Call subroutine at address stored in register XYZ
```

### Conditional Calls
```
CALL Z, 0x1000     ; Call subroutine at 0x1000 if zero flag is set
CALL C, XYZ        ; Call subroutine at address in XYZ if carry flag is set
CALL GT, func      ; Call function if greater than flag is set
```

## Typical Use Cases

1. **Function Calls**: Invoke functions/subroutines
2. **Library Calls**: Call system/library functions
3. **Conditional Function Calls**: Call functions based on conditions
4. **Dynamic Dispatch**: Call functions via register-stored addresses

## Conclusion

The CALL instruction provides essential subroutine invocation capabilities with both unconditional and conditional variants. The automatic return address management enables structured programming with function calls and returns.

