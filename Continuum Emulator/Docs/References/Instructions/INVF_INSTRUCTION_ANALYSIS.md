# INVF Instruction Execution Analysis

## Executive Summary

The INVF (Invert Flag) instruction implementation in `ExINVF.cs` provides **1 instruction variant** for inverting a specific CPU flag. INVF toggles the value of a flag register bit.

## File Statistics

- **File**: `Emulator/Execution/ExINVF.cs`
- **Total Lines**: 14 lines
- **Instruction Variants**: 1 unique opcode
- **Implementation Pattern**: Direct flag manipulation

## Addressing Modes

### Flag Inversion Operation

- `INVF n` - Invert flag at index n (0-31)
  - Flag index encoded in lower 5 bits of opcode
  - Toggles flag value (true ↔ false)

## Operation Description

INVF performs flag inversion:
- **Flag Toggle**: `flag[n] = !flag[n]`
- **Bit Index**: 0-31 (supports up to 32 flags)
- **In-place operation**: Flag value is toggled directly

## Implementation Details

### Flag Inversion

INVF uses the flag system:
```csharp
byte flagIndex = (byte)(ldOp & 0b00011111);
computer.CPU.FLAGS.InvertValueByIndex(flagIndex);
```

The flag index is extracted from the lower 5 bits of the opcode.

## Opcode Encoding

INVF uses compact encoding:
1. **Primary Opcode**: Opcode for INVF
2. **Flag Index**: Lower 5 bits of opcode (`ldOp & 0b00011111`)
   - Range: 0-31
   - Identifies which flag to invert

## Flag Updates

INVF directly modifies the specified flag:
- **Target Flag**: Flag at specified index is toggled
- **Other Flags**: Unchanged

## Operation Categories Summary

1. **Flag Operations**: 1 variant

## Usage Examples

```
INVF 0          ; Toggle Zero flag
INVF 1          ; Toggle Carry flag
INVF 2          ; Toggle Sign flag
INVF 5          ; Toggle Equal flag
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

1. **Flag Toggling**: Toggle specific flags
2. **Condition Inversion**: Invert condition flags
3. **State Management**: Toggle state flags
4. **Debug Operations**: Manipulate flags for debugging

## Comparison with SETF/RESF Instructions

- **INVF**: Toggles flag (true ↔ false)
- **SETF**: Sets flag to true
- **RESF**: Sets flag to false
- **INVF**: More flexible for toggling

## Conclusion

The INVF instruction provides efficient flag toggling capabilities. The simple inversion operation is essential for conditional logic and state management in Continuum 93 programs.

