# BASIC Compiler Progress

## Overview
This document tracks the progress of the C93 BASIC compiler implementation.

## Status: In Progress

### Completed
- [x] Project structure created
- [x] Lexer implementation
- [x] Parser implementation
- [x] AST nodes
- [x] Code emitter (basic)
- [x] Variable management (basic)
- [x] Expression compilation (basic)
- [x] Control flow statements (IF, FOR, WHILE, GOTO, GOSUB)
- [x] Graphics commands (CLS, PLOT, LINE)
- [x] I/O commands (PRINT basic)
- [x] Interrupt integration (0xC1)
- [x] Unit tests (basic)

### In Progress
- [x] Complete expression compilation (all operators) - Fixed LD instruction usage
- [x] Complete PRINT implementation (all features)
- [x] Math functions
- [ ] String operations
- [x] More graphics commands (CIRCLE, RECTANGLE, ELLIPSE, etc.)
- [x] Array support (basic)
- [x] Function calls (math functions)
- [x] Error handling improvements
- [x] Fixed obsolete LD16/LD24/LD32 usage - now using LD with correct addressing
- [x] Fixed LDF usage - LDF is for flags, floats use LD fr
- [x] Fixed Previous() bug in all expression parsers
- [x] Fixed function call parsing for keyword-based functions (SIN, PI, etc.)
- [x] Fixed END IF parsing - handles both ENDIF and END IF tokens
- [x] Fixed IF statement parsing - correctly handles THEN block
- [x] All unit tests passing (18/18)
- [x] Fixed register allocation - use 32-bit registers (ABCD, EFGH, etc.) spaced by 4 positions
- [x] Optimized binary expressions - reuse left register instead of allocating new result register
- [x] Implemented register reuse pool - registers are released after operations and assignments for reuse
- [x] Fixed FOR loop - use direct memory operations `LD (addr), value, 4` for 32-bit initialization
- [x] Fixed FOR loop - use 32-bit registers for comparisons and increments (not 8-bit)
- [x] Added float step support for FOR loops (separate integer and float loop implementations)
- [x] Created PRINT helper function with PUSH/POP to preserve registers and avoid code duplication
- [x] Fixed PRINT to handle integer expressions by converting to strings using IntToString interrupt
- [x] Fixed PLOT command - load function ID first, use correct register sizes (16-bit for x/y, 8-bit for color)

## Architecture

### Components
1. **BasicCompiler**: Main compiler class
2. **Lexer**: Tokenizes BASIC source code
3. **Parser**: Builds AST from tokens
4. **Emitter**: Generates assembly code from AST
5. **SymbolTable**: Manages variables and labels
6. **VariableManager**: Allocates memory for variables

### Memory Layout
- Code section: Starts at provided address (default 0x080000)
- Variable section: After code section
- String pool: After variables
- Arrays: Allocated dynamically

### Variable Storage Strategy
- Variables stored in memory using LD memory-to-memory instructions
- No stack usage for variable storage
- Each variable has a fixed memory address
- Arrays stored as contiguous memory blocks

## Implementation Notes

### Current Focus
- Setting up core compiler infrastructure
- Implementing lexer and parser
- Basic expression compilation

### Next Steps
1. Complete lexer for all BASIC tokens
2. Implement parser for statements
3. Implement code emitter
4. Add variable management
5. Wire to interrupt 0xC1

## Issues and Questions
- None yet

