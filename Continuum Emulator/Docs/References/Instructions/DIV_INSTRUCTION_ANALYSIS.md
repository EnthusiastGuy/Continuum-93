# DIV Instruction Execution Analysis

## Executive Summary

The DIV (Divide) instruction implementation in `ExDIV.cs` contains **approximately 40 distinct instruction variants**, supporting division operations across multiple data sizes (8, 16, 24, 32 bits) and floating-point operations. DIV includes both simple division (quotient only) and division with remainder variants. Division by zero is handled by setting the result to the maximum value for the data type and setting the carry flag.

## File Statistics

- **File**: `Emulator/Execution/ExDIV.cs`
- **Total Lines**: 948 lines
- **Instruction Variants**: ~40 unique opcodes
- **Implementation Pattern**: Uses switch statement on upper 6 bits of opcode

## Addressing Modes

The DIV instruction supports register-to-register and immediate-to-register operations:

### 1. Integer Register Operations (No Remainder)

#### 8-bit Operations
- `DIV r, n` - Divide 8-bit register by immediate value
  - Register index encoded in opcode
  - Immediate value: up to 16-bit
  - Result (quotient) stored in register
  - If divisor is 0: result set to `byte.MaxValue`, carry flag set
- `DIV r, r` - Divide 8-bit register by 8-bit register
  - Both register indices encoded in opcode
  - If divisor is 0: result set to `byte.MaxValue`, carry flag set

#### 16-bit Operations
- `DIV rr, n` - Divide 16-bit register by immediate value
- `DIV rr, r` - Divide 16-bit register by 8-bit register
- `DIV rr, rr` - Divide 16-bit register by 16-bit register
  - If divisor is 0: result set to `ushort.MaxValue`, carry flag set

#### 24-bit Operations
- `DIV rrr, n` - Divide 24-bit register by immediate value
- `DIV rrr, r` - Divide 24-bit register by 8-bit register
- `DIV rrr, rr` - Divide 24-bit register by 16-bit register
- `DIV rrr, rrr` - Divide 24-bit register by 24-bit register
  - If divisor is 0: result set to `0xFFFFFF`, carry flag set

#### 32-bit Operations
- `DIV rrrr, n` - Divide 32-bit register by immediate value
- `DIV rrrr, r` - Divide 32-bit register by 8-bit register
- `DIV rrrr, rr` - Divide 32-bit register by 16-bit register
- `DIV rrrr, rrr` - Divide 32-bit register by 24-bit register
- `DIV rrrr, rrrr` - Divide 32-bit register by 32-bit register
  - If divisor is 0: result set to `0xFFFFFFFF`, carry flag set

### 2. Integer Register Operations (With Remainder)

DIV supports operations that store both quotient and remainder:

#### 8-bit Operations
- `DIV r, n, r` - Divide 8-bit register by immediate, store quotient in first register, remainder in third register
- `DIV r, r, r` - Divide 8-bit register by 8-bit register, store quotient in first register, remainder in third register
  - If divisor is 0: quotient set to `byte.MaxValue`, remainder set to 0, carry flag set

#### 16-bit Operations
- `DIV rr, n, rr` - Divide 16-bit register by immediate, store quotient and remainder
- `DIV rr, r, r` - Divide 16-bit register by 8-bit register
  - Quotient stored in 16-bit register
  - Remainder stored in 8-bit register
- `DIV rr, rr, rr` - Divide 16-bit register by 16-bit register
  - Both quotient and remainder stored in 16-bit registers

#### 24-bit Operations
- `DIV rrr, n, rr` - Divide 24-bit register by immediate
  - Quotient stored in 24-bit register
  - Remainder stored in 16-bit register
- `DIV rrr, r, r` - Divide 24-bit register by 8-bit register
  - Quotient stored in 24-bit register
  - Remainder stored in 8-bit register
- `DIV rrr, rr, rr` - Divide 24-bit register by 16-bit register
  - Quotient stored in 24-bit register
  - Remainder stored in 16-bit register
- `DIV rrr, rrr, rrr` - Divide 24-bit register by 24-bit register
  - Both quotient and remainder stored in 24-bit registers

#### 32-bit Operations
- `DIV rrrr, n, rr` - Divide 32-bit register by immediate
  - Quotient stored in 32-bit register
  - Remainder stored in 16-bit register
- `DIV rrrr, r, r` - Divide 32-bit register by 8-bit register
  - Quotient stored in 32-bit register
  - Remainder stored in 8-bit register
- `DIV rrrr, rr, rr` - Divide 32-bit register by 16-bit register
  - Quotient stored in 32-bit register
  - Remainder stored in 16-bit register
- `DIV rrrr, rrr, rrr` - Divide 32-bit register by 24-bit register
  - Quotient stored in 32-bit register
  - Remainder stored in 24-bit register
- `DIV rrrr, rrrr, rrrr` - Divide 32-bit register by 32-bit register
  - Both quotient and remainder stored in 32-bit registers

### 3. Floating-Point Operations

#### Float Register Operations
- `DIV fr, fr` - Divide float register by float register
  - If divisor is 0: reports runtime error "Division by zero"
  - Sign flag set if result is negative
- `DIV fr, nnn` - Divide float register by immediate float value
- `DIV fr, r` - Divide float register by 8-bit integer register (converted to float)
- `DIV fr, rr` - Divide float register by 16-bit integer register
- `DIV fr, rrr` - Divide float register by 24-bit integer register
- `DIV fr, rrrr` - Divide float register by 32-bit integer register
- `DIV fr, (nnn)` - Divide float register by float value from memory
- `DIV fr, (rrr)` - Divide float register by float value from register-indirect memory

#### Integer Register from Float Operations
- `DIV r, fr` - Divide 8-bit register by float register
  - If divisor is 0: reports runtime error
  - Float result converted to integer and rounded
- `DIV rr, fr` - Divide 16-bit register by float register
- `DIV rrr, fr` - Divide 24-bit register by float register
  - Uses `Math.Abs()` to ensure positive result
- `DIV rrrr, fr` - Divide 32-bit register by float register
  - Uses `Math.Abs()` to ensure positive result

#### Float Memory Operations
- `DIV (nnn), fr` - Divide float value in memory by float register
- `DIV (rrr), fr` - Divide float value in register-indirect memory by float register

## Opcode Encoding

DIV uses the same compact encoding scheme as MUL:

1. **Primary Opcode**: Opcode 4 (DIV)
2. **Secondary Opcode**: Upper 6 bits of next byte (`ldOp >> 2`)
3. **Register Encoding**: Same as MUL
4. **Remainder Variants**: Use different secondary opcodes (e.g., `DIV_r_n_r`)

## Division by Zero Handling

### Integer Operations
- **Result**: Set to maximum value for the data type
  - 8-bit: `byte.MaxValue` (0xFF)
  - 16-bit: `ushort.MaxValue` (0xFFFF)
  - 24-bit: `0xFFFFFF`
  - 32-bit: `0xFFFFFFFF`
- **Remainder**: Set to 0 (for remainder variants)
- **Carry Flag**: Set to true
- **No Exception**: Operation completes without error

### Floating-Point Operations
- **Error Handling**: Calls `ErrorHandler.ReportRuntimeError("Division by zero")`
- **No Result**: Operation does not modify registers
- **Program Behavior**: Depends on error handler implementation

## Flag Updates

- **Carry Flag**: Set when division by zero occurs (integer operations)
- **Sign Flag**: Set for floating-point operations when result is negative
- **Zero Flag**: Updated through register operations (if quotient is zero)

## Implementation Details

### Register Index Extraction

Same as MUL:
```csharp
byte mixedReg = computer.MEMC.Fetch();
byte dividendReg = (byte)(((ldOp << 3) & 0b00011111) + (mixedReg >> 5));
byte divisorReg = (byte)(mixedReg & 0b00011111);
```

### Remainder Calculation

For remainder variants, both quotient and remainder are calculated:
```csharp
quotient = dividend / divisor;
remainder = dividend % divisor;
```

The remainder is stored in a separate register specified in the instruction.

### Float-to-Integer Conversion

When dividing integer registers by float registers:
1. Integer value is converted to float
2. Division performed in float
3. Result rounded to nearest integer
4. For 24-bit and 32-bit operations, absolute value is taken

## Operation Categories Summary

1. **Integer Operations (No Remainder)**: ~12 variants
   - 8-bit: 2 variants
   - 16-bit: 3 variants
   - 24-bit: 4 variants
   - 32-bit: 5 variants

2. **Integer Operations (With Remainder)**: ~12 variants
   - 8-bit: 2 variants
   - 16-bit: 3 variants
   - 24-bit: 4 variants
   - 32-bit: 5 variants

3. **Floating-Point Operations**: ~16 variants
   - Float register operations: 8 variants
   - Integer-to-float operations: 4 variants
   - Float-to-integer operations: 4 variants
   - Float memory operations: 2 variants

## Comparison with MUL

- **Same Encoding**: DIV uses the same compact encoding as MUL
- **Remainder Support**: DIV supports remainder variants, MUL does not
- **Division by Zero**: DIV has explicit handling for division by zero
- **Error Reporting**: Floating-point DIV reports errors, integer DIV uses flags

## Usage Examples

### Basic Division
```
DIV A, 5        ; A = A / 5
DIV BC, DE      ; BC = BC / DE
DIV24 XYZ, 100  ; XYZ = XYZ / 100
```

### Division with Remainder
```
DIV A, 5, B     ; A = A / 5 (quotient), B = A % 5 (remainder)
DIV BC, DE, FG  ; BC = BC / DE (quotient), FG = BC % DE (remainder)
```

### Floating-Point Operations
```
DIV F0, F1          ; F0 = F0 / F1
DIV F0, 3.14        ; F0 = F0 / 3.14
DIV A, F0           ; A = A / (int)F0 (rounded)
```

## Conclusion

The DIV instruction provides comprehensive division capabilities with support for both integer and floating-point operations. The remainder variants are particularly useful for modular arithmetic and integer division operations. The division by zero handling ensures predictable behavior for integer operations while providing error reporting for floating-point operations.

