# SETF Instruction Execution Analysis

## Executive Summary

The SETF (Set Flag) instruction implementation in `ExSETF.cs` provides **1 instruction variant** for setting a specific CPU flag to true. SETF sets the value of a flag register bit to 1.

## File Statistics

- **File**: `Emulator/Execution/ExSETF.cs`
- **Total Lines**: 13 lines
- **Instruction Variants**: 1 unique opcode
- **Implementation Pattern**: Direct flag manipulation

## Addressing Modes

### Flag Set Operation

- `SETF n` - Set flag at index n to true (0-31)
  - Flag index encoded in lower 5 bits of opcode
  - Sets flag value to true (1)

## Operation Description

SETF performs flag setting:
- **Flag Set**: `flag[n] = true`
- **Bit Index**: 0-31 (supports up to 32 flags)
- **In-place operation**: Flag value is set directly

## Implementation Details

### Flag Setting

SETF uses the flag system:
```csharp
byte flagIndex = (byte)(ldOp & 0b00011111);
computer.CPU.FLAGS.SetValueByIndex(flagIndex, true);
```

The flag index is extracted from the lower 5 bits of the opcode.

## Opcode Encoding

SETF uses compact encoding:
1. **Primary Opcode**: Opcode for SETF
2. **Flag Index**: Lower 5 bits of opcode (`ldOp & 0b00011111`)
   - Range: 0-31
   - Identifies which flag to set

## Flag Updates

SETF directly modifies the specified flag:
- **Target Flag**: Flag at specified index is set to true
- **Other Flags**: Unchanged

## Operation Categories Summary

1. **Flag Operations**: 1 variant

## Usage Examples

```
SETF 0          ; Set Zero flag
SETF 1          ; Set Carry flag
SETF 2          ; Set Sign flag
SETF 5          ; Set Equal flag
```

## Flag Index Reference

Common flag indices:
- **0 (Z)**: Zero flag
- **1 (C)**: Carry flag
- **2 (SN)**: Sign flag
- **3 (OV)**: Overflow flag
- **4 (PO)**: Parity flag
- **5 (EQ)**: Equal flag
- **6 (GT)**: Greater Than flag
- **7 (LT)**: Less Than flag

## Typical Use Cases

1. **Flag Setting**: Set specific flags explicitly
2. **Condition Setting**: Set condition flags
3. **State Management**: Set state flags
4. **Debug Operations**: Manipulate flags for debugging

## Comparison with RESF/INVF Instructions

- **SETF**: Sets flag to true
- **RESF**: Sets flag to false
- **INVF**: Toggles flag (true ↔ false)
- **SETF**: Explicit flag setting

## Conclusion

The SETF instruction provides efficient flag setting capabilities. The simple set operation is essential for explicit flag manipulation in Continuum 93 programs.

