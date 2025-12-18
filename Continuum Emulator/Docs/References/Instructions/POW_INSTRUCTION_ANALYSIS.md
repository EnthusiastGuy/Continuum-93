# POW Instruction Execution Analysis

## Executive Summary

The POW (Power) instruction implementation in `ExPOW.cs` provides **approximately 15 distinct instruction variants** for computing exponentiation (raising a number to a power). POW supports both integer and floating-point operations with various operand combinations.

## File Statistics

- **File**: `Emulator/Execution/ExPOW.cs`
- **Total Lines**: 201 lines
- **Instruction Variants**: ~15 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 6 bits of opcode

## Addressing Modes

### 1. Float-to-Float Operations

- `POW fr, fr` - Float register raised to power of float register
- `POW fr, nnn` - Float register raised to power of immediate float value
- `POW fr, (nnn)` - Float register raised to power of float value from absolute address
- `POW fr, (rrr)` - Float register raised to power of float value from register-indirect address

### 2. Integer-to-Float Operations

- `POW fr, r` - Float register raised to power of 8-bit integer register
- `POW fr, rr` - Float register raised to power of 16-bit integer register
- `POW fr, rrr` - Float register raised to power of 24-bit integer register
- `POW fr, rrrr` - Float register raised to power of 32-bit integer register

### 3. Float-to-Integer Operations

- `POW r, fr` - 8-bit integer register raised to power of float register (result truncated to 8-bit)
- `POW rr, fr` - 16-bit integer register raised to power of float register (result truncated to 16-bit)
- `POW rrr, fr` - 24-bit integer register raised to power of float register (result truncated to 24-bit)
- `POW rrrr, fr` - 32-bit integer register raised to power of float register (result truncated to 32-bit)

### 4. Memory Operations

- `POW fr, (rrr)` - Float register raised to power of float value from register-indirect address
- `POW (rrr), fr` - Float value at register-indirect address raised to power of float register
- `POW (nnn), fr` - Float value at absolute address raised to power of float register

## Mathematical Operation

POW computes exponentiation using `MathF.Pow()`:
- **Power Function**: `result = base^exponent`
- **Float Precision**: Uses single-precision floating-point for calculations
- **Integer Conversion**: Results are truncated when stored in integer registers

## Implementation Details

### Float-to-Float Power

For float operations:
```csharp
float fReg1Value = computer.CPU.FREGS.GetRegister(fReg1);
float fReg2Value = computer.CPU.FREGS.GetRegister(fReg2);
computer.CPU.FREGS.SetRegister(fReg1, MathF.Pow(fReg1Value, fReg2Value));
```

### Integer-to-Float Power

For integer base with float exponent:
```csharp
byte reg1Val = computer.CPU.REGS.Get8BitRegister(r1Index);
float fRegVal = computer.CPU.FREGS.GetRegister(fRegIndex);
computer.CPU.FREGS.SetRegister(fRegIndex, MathF.Pow(fRegVal, reg1Val));
```

### Float-to-Integer Power

For float base with integer result:
```csharp
byte reg1Val = computer.CPU.REGS.Get8BitRegister(r1Index);
float fRegVal = computer.CPU.FREGS.GetRegister(fRegIndex);
computer.CPU.REGS.Set8BitRegister(r1Index, (byte)MathF.Pow(reg1Val, fRegVal));
```

## Flag Updates

POW does not modify CPU flags.

## Operation Categories Summary

1. **Float-to-Float Operations**: ~4 variants
2. **Integer-to-Float Operations**: 4 variants
3. **Float-to-Integer Operations**: 4 variants
4. **Memory Operations**: ~3 variants

## Usage Examples

```
POW F0, F1      ; F0 = F0^F1
POW F0, 2.0     ; F0 = F0^2.0
POW F0, A       ; F0 = F0^A
POW A, F0       ; A = A^F0 (truncated)
```

## Mathematical Properties

- **Exponentiation**: base raised to exponent power
- **Special Cases**: 
  - `x^0 = 1` (for x ≠ 0)
  - `x^1 = x`
  - `0^x = 0` (for x > 0)
- **Domain**: Base and exponent can be any real numbers
- **Range**: Depends on base and exponent

## Typical Use Cases

1. **Mathematical Calculations**: Exponentiation operations
2. **Scientific Computing**: Power calculations
3. **Graphics**: Scaling, transformations
4. **Game Development**: Damage calculations, scaling

## Conclusion

The POW instruction provides comprehensive exponentiation capabilities with support for both integer and floating-point operations. The flexible operand combinations enable various mathematical calculations.

