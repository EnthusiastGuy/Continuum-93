# INT Instruction Execution Analysis

## Executive Summary

The INT (Interrupt) instruction implementation in `ExINT.cs` provides **1 instruction variant** for triggering software interrupts. INT invokes interrupt handlers based on an interrupt number and register value.

## File Statistics

- **File**: `Emulator/Execution/ExINT.cs`
- **Total Lines**: 30 lines
- **Instruction Variants**: 1 unique opcode
- **Implementation Pattern**: Uses switch statement on upper 3 bits of opcode

## Addressing Modes

### Software Interrupt Trigger

- `INT n, r` - Trigger interrupt number n with value from register r
  - Interrupt number: 8-bit immediate value
  - Register value: 8-bit register value passed to interrupt handler
  - Register index: Encoded in lower 5 bits of opcode

## Implementation Details

### Interrupt Handling

INT uses the interrupt handler system:
```csharp
byte regIndex = (byte)(ldOp & 0b00011111);
byte number = computer.MEMC.Fetch();
InterruptHandler.HandleInterrupt(number, computer.CPU.REGS.Get8BitRegister(regIndex), regIndex, computer);
```

The interrupt handler receives:
- **Interrupt Number**: Identifies which interrupt to handle
- **Register Value**: Value from specified register
- **Register Index**: Index of the register
- **Computer**: Reference to computer state

## Opcode Encoding

INT uses compact encoding:
1. **Primary Opcode**: Opcode 29 (INT)
2. **Secondary Opcode**: Upper 3 bits of opcode (`ldOp >> 5`)
3. **Register Index**: Lower 5 bits of opcode (`ldOp & 0b00011111`)
4. **Interrupt Number**: 8-bit immediate value

## Flag Updates

INT does not directly modify CPU flags, but interrupt handlers may modify flags.

## Operation Categories Summary

1. **Software Interrupts**: 1 variant

## Usage Examples

```
INT 0, A        ; Trigger interrupt 0 with value from register A
INT 1, B        ; Trigger interrupt 1 with value from register B
INT 5, C        ; Trigger interrupt 5 with value from register C
```

## Typical Use Cases

1. **System Calls**: Invoke operating system functions
2. **Service Routines**: Call service routines
3. **Hardware Emulation**: Simulate hardware interrupts
4. **Debugging**: Trigger debug handlers
5. **Error Handling**: Invoke error handlers

## Interrupt System

The interrupt system allows:
- **Software Interrupts**: Triggered by INT instruction
- **Hardware Interrupts**: Triggered by external events
- **Interrupt Handlers**: Routines that process interrupts
- **Interrupt Numbers**: Identify specific interrupt types

## Comparison with CALL Instruction

- **INT**: Invokes interrupt handler (system-level)
- **CALL**: Invokes subroutine (user-level)
- **INT**: May change execution context
- **CALL**: Standard function call

## Conclusion

The INT instruction provides software interrupt capabilities, enabling system calls and service routine invocation. The interrupt system allows programs to interact with the operating system and hardware emulation layer.

