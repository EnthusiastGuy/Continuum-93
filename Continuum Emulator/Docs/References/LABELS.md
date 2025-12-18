# Labels Documentation

## Executive Summary

Labels in Continuum 93 assembly language provide symbolic names for memory addresses, enabling code to reference locations by name rather than numeric addresses. The assembler supports two types of labels: **absolute labels** (prefixed with `.`) and **relative labels** (prefixed with `~`), though relative labels are currently not fully implemented.

## Label Syntax

### Absolute Labels

**Format**: `.LabelName`

- Starts with a period (`.`)
- Followed by alphanumeric characters and underscores
- Case-sensitive
- Represents an absolute memory address

**Examples**:
```
.Start
.MainLoop
.DataSection
.MyFunction
.Label_123
```

### Relative Labels

**Format**: `~LabelName`

- Starts with a tilde (`~`)
- Followed by alphanumeric characters and underscores
- Case-sensitive
- Intended to represent addresses relative to the current instruction
- **Note**: Currently not fully implemented in the assembler

**Examples**:
```
~LocalLabel
~Temp
```

## Label Definition

### Definition Rules

1. **One label per line**: Only one label can be defined per line
2. **Label position**: Labels must appear at the beginning of a line (before any instruction or directive)
3. **Label format**: Must start with `.` or `~` followed by valid identifier characters
4. **Unique names**: Each label must be unique within the program
5. **Address assignment**: Labels are assigned the address of the instruction or directive on the same line

### Definition Syntax

```
.LabelName
    <instruction or directive>
```

Or on the same line:

```
.LabelName <instruction or directive>
```

**Examples**:
```
.Start
    LD A, 1
    RET

.MainLoop
    CALL .ProcessData
    JR .MainLoop

.DataSection
    #DB "Hello", 0
```

## Label Usage

### In Instructions

Labels can be used as operands in instructions that support absolute addressing:

**Supported Instructions**:
- `CALL`, `JP` - Jump/call to absolute address
- `LD`, `LD16`, `LD24`, `LD32` - Load from/to absolute address
- `ADD`, `SUB`, `MUL`, `DIV` - Arithmetic with absolute address
- `AND`, `OR`, `XOR`, `NAND`, `NOR`, `XNOR`, `IMPLY` - Logical operations
- `MEMC`, `MEMF`, `FIND` - Memory operations
- `PUSH`, `POP` - Stack operations with absolute address
- `VCL`, `VDL` - Video operations
- `LDF`, `LDREGS`, `STREGS` - Register operations
- `RGB2HSL`, `HSL2RGB`, `RGB2HSB`, `HSB2RGB` - Color operations
- `ISQR`, `POW`, `SQR` - Math operations
- `PLAY` - Audio operations
- And many more...

**Examples**:
```
.Start
    LD A, 1
    JP .NextSection

.NextSection
    LD BCD, .DataAddress
    LD A, (BCD)

.DataAddress
    #DB 0x12, 0x34, 0x56
```

### In Relative Instructions

For `CALLR` and `JR` (relative jump/call), labels are converted to relative offsets:

**Examples**:
```
.MainLoop
    CALLR .ProcessData    ; Relative call (offset calculated automatically)
    JR .MainLoop          ; Relative jump (offset calculated automatically)

.ProcessData
    RET
```

**Implementation**:
- Relative instructions calculate: `offset = label_address - current_address`
- The offset is encoded as a signed value in the instruction
- Forward and backward jumps are supported

### In #DB Directives

Labels can be used in `#DB` directives to emit label addresses:

**Examples**:
```
.Start
    LD A, 1

.DataSection
    #DB .Start            ; Emits 24-bit address of .Start (3 bytes)
    #DB .DataSection      ; Emits 24-bit address of .DataSection (3 bytes)
```

**Behavior**:
- Label addresses are emitted as 24-bit values (3 bytes, big-endian)
- Labels are resolved after all labels are defined (second pass)
- If a label is not found, an error is logged

## Label Resolution

### Two-Pass Assembly

The assembler uses a two-pass approach to resolve labels:

**First Pass** (Label Collection):
1. Scans all source lines
2. Identifies label definitions
3. Assigns addresses to labels based on code generation
4. Builds a label dictionary mapping names to addresses

**Second Pass** (Code Generation):
1. Generates machine code
2. Resolves label references to addresses
3. For absolute addressing: Uses label address directly
4. For relative addressing: Calculates offset from current address

### Label Resolution Process

```
1. First Pass:
   - Line 1: .Start at address 0x80000
   - Line 5: .DataSection at address 0x80010
   - Build dictionary: {".Start": 0x80000, ".DataSection": 0x80010}

2. Second Pass:
   - Encounter: JP .Start
   - Lookup: .Start = 0x80000
   - Generate: JP 0x80000
   
   - Encounter: JR .DataSection
   - Lookup: .DataSection = 0x80010
   - Current address: 0x80005
   - Calculate: offset = 0x80010 - 0x80005 = 11
   - Generate: JR 11
```

## Label Validation

### Valid Label Names

- Must start with `.` or `~`
- Followed by alphanumeric characters (`A-Z`, `a-z`, `0-9`) and underscores (`_`)
- Case-sensitive
- Must be at least 2 characters (`.` or `~` plus at least one character)

**Valid Examples**:
```
.Start
.MainLoop
.Data_Section
.Label123
~Local
~Temp_Value
```

**Invalid Examples**:
```
Start          ; Missing prefix
.              ; Too short
.123           ; Starts with number (after prefix)
.Main Loop     ; Contains space
.Main-Loop     ; Contains hyphen
```

### Label Validation Function

The assembler uses `DataConverter.IsLabelValid()` to validate label names:
- Checks prefix (`.` or `~`)
- Validates identifier characters
- Ensures minimum length

## Label Scope

### Global Scope

All labels are global within a program:
- Labels defined in included files are available throughout the program
- Labels must be unique across all included files
- Forward references are supported (labels can be used before definition)

**Example**:
```
    JP .LaterLabel        ; Forward reference (OK)

.EarlyLabel
    LD A, 1

.LaterLabel
    LD A, 2
    JP .EarlyLabel        ; Backward reference (OK)
```

### Label Collision

If a label is defined multiple times, the assembler reports an error:

```
.LabelName
    LD A, 1

.LabelName                 ; Error: Label already defined
    LD A, 2
```

## Label Examples

### Basic Usage

```
#ORG 0x080000

.Start
    LD A, 0x02
    LD B, 3
    INT 0x01, A
    RET

.ClearScreen
    LD A, 0x05
    LD B, 0
    LD C, 0
    INT 0x01, A
    RET
```

### Function Calls

```
.Main
    CALL .Initialize
    CALL .MainLoop
    RET

.Initialize
    LD A, 0x02
    LD B, 1
    INT 0x01, A
    RET

.MainLoop
    CALL .Update
    CALL .Render
    JR .MainLoop

.Update
    RET

.Render
    RET
```

### Data References

```
.Main
    LD BCD, .Message       ; Load address of message
    LD EFG, .Buffer        ; Load address of buffer
    MEMC BCD, EFG, 13      ; Copy message to buffer
    RET

.Message
    #DB "Hello, World!", 0

.Buffer
    #DB [256] 0            ; 256-byte buffer
```

### Relative Jumps

```
.MainLoop
    LD A, 0
    INC A
    CP A, 100
    JR NZ, .MainLoop       ; Relative jump (offset calculated)

.Done
    RET
```

### Label Addresses in Data

```
.DataTable
    #DB .Function1         ; Address of Function1 (3 bytes)
    #DB .Function2         ; Address of Function2 (3 bytes)
    #DB .Function3         ; Address of Function3 (3 bytes)

.Function1
    RET

.Function2
    RET

.Function3
    RET
```

## Best Practices

1. **Use descriptive names**: Choose clear, meaningful label names
2. **Consistent naming**: Use consistent naming conventions (e.g., `.FunctionName`, `.DataName`)
3. **Local vs Global**: Use absolute labels (`.`) for global symbols, relative labels (`~`) for local scope (when implemented)
4. **Avoid conflicts**: Ensure label names are unique across all included files
5. **Document labels**: Use comments to explain the purpose of labels
6. **Organize by function**: Group related labels together

## Implementation Details

### Label Storage

Labels are stored in `AssemblerStats.Labels` dictionary:
- Key: Label name (string)
- Value: Memory address (24-bit unsigned integer)

### Label Lookup

During code generation:
- Absolute addressing instructions: `_stats.Labels[label]` → address
- Relative addressing instructions: `_stats.Labels[label] - current_address` → offset
- `#DB` directives: `_stats.Labels[label]` → 24-bit address (3 bytes)

### Label Processing

1. **Line parsing**: `Interpret.GetLabelFromLine()` extracts label from line
2. **Label assignment**: `CLine.Process()` assigns address to label
3. **Label resolution**: `CLine.ResolveArguments()` resolves label references
4. **Error checking**: Undefined labels generate compilation errors

## Conclusion

Labels provide essential symbolic addressing capabilities in Continuum 93 assembly language. Understanding label definition, usage, and resolution is crucial for writing maintainable assembly programs.

