# EX Instruction Execution Analysis

## Executive Summary

The EX (Exchange) instruction implementation in `ExEX.cs` provides **5 distinct instruction variants** for swapping values between two registers. EX exchanges the contents of two registers without using temporary storage.

## File Statistics

- **File**: `Emulator/Execution/ExEX.cs`
- **Total Lines**: 70 lines
- **Instruction Variants**: 5 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 6 bits of opcode

## Addressing Modes

### Register Exchange Operations

- `EX r, r` - Exchange two 8-bit registers
- `EX rr, rr` - Exchange two 16-bit registers
- `EX rrr, rrr` - Exchange two 24-bit registers
- `EX rrrr, rrrr` - Exchange two 32-bit registers
- `EX fr, fr` - Exchange two float registers

## Operation Description

EX performs register exchange:
- **Swap Operation**: `temp = reg1; reg1 = reg2; reg2 = temp`
- **No Temporary**: Uses efficient exchange algorithm
- **Atomic Operation**: Both registers updated simultaneously

## Implementation Details

### Integer Register Exchange

For integer register operations:
```csharp
computer.CPU.REGS.Ex8BitRegisters(
    (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5)),
    (byte)(mixedReg & 0b00011111)
);
```

### Float Register Exchange

For float register operations:
```csharp
computer.CPU.FREGS.ExchangeRegisters(
    (byte)(mixedReg >> 4),
    (byte)(mixedReg & 0b_00001111)
);
```

## Opcode Encoding

EX uses compact encoding:
1. **Primary Opcode**: Opcode for EX
2. **Secondary Opcode**: Upper 6 bits of opcode (`ldOp >> 2`)
3. **Register Indices**: Encoded in mixed register byte
   - First register: `((ldOp << 3) & 0b00011111) + (mixedReg >> 5)`
   - Second register: `mixedReg & 0b00011111`

## Flag Updates

EX does not modify CPU flags.

## Operation Categories Summary

1. **8-bit Operations**: 1 variant
2. **16-bit Operations**: 1 variant
3. **24-bit Operations**: 1 variant
4. **32-bit Operations**: 1 variant
5. **Float Operations**: 1 variant

## Usage Examples

```
EX A, B         ; Swap A and B
EX BC, DE       ; Swap BC and DE
EX XYZ, ABC     ; Swap XYZ and ABC
EX F0, F1       ; Swap float registers F0 and F1
```

## Typical Use Cases

1. **Value Swapping**: Swap two values efficiently
2. **Sorting Algorithms**: Exchange elements in sorting
3. **Register Management**: Reorganize register contents
4. **Optimization**: Avoid temporary storage

## Mathematical Properties

- **Self-Inverse**: `EX(EX(a, b), b) = EX(a, b)` (swapping twice returns to original)
- **Commutative**: `EX(a, b) = EX(b, a)`
- **Identity**: Swapping same register has no effect

## Comparison with Other Instructions

- **EX**: Swaps two registers (atomic operation)
- **LD with temporary**: Requires temporary storage
- **EX**: More efficient for swapping
- **LD**: More flexible but requires extra register

## Conclusion

The EX instruction provides efficient register exchange capabilities for all data sizes including floating-point. The atomic swap operation is essential for sorting algorithms and register management in Continuum 93 programs.
