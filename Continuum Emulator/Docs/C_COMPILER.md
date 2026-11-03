# Continuum 93 C Compiler

A C compiler that translates C source into Continuum 93 assembly, which is then built by the regular assembler.
Source: `Emulator/Compilers/C93C/`. Tests: `ContinuumUnitTests/CCompilerTests/`.

## Using it

**From a program** (interrupt `INT 0x00`, function `0xC2` - *Build C*), same register contract as *Build* (`0xC0`):

```asm
    LD A, 0xC2              ; Build C
    LD BCD, .path           ; path relative to the filesystem root
    INT 0x00, A
    CP E, 0                 ; E = number of errors
    JR NZ, .failed
    CALL BCD                ; BCD = run address; main's return value comes back in ABCD
.path
    #DB "programs/my-game/main.c", 0
```

On failure `BCD` is `0xFFFFFF`. A `debug/` folder next to the source receives `<file>.c.log` (compiler
messages), `<file>.generated.asm` and the assembler log. `#include "x.h"` is resolved relative to the including file.

**From C#**: `new CCompiler().Compile(source, "main.c")` returns assembly text; `Errors`, `ErrorList` and `Log`
describe problems. Set `IncludeResolver` to supply `#include` files.

## Language

Supported:

- Types: `char` (signed), `short`, `int`, `long` (all with `unsigned`), `_Bool`, `void`, pointers, arrays (incl.
  multi-dimensional), `struct`, `union` (incl. anonymous members), `enum`, `typedef`, function pointers.
- Sizes: `char` 1, `short` 2, `int`/`long`/pointers 4 bytes. Integers are big-endian in memory and wrap
  (two's complement). Structs are **packed** (no padding) - the machine has no alignment requirements.
- Every operator, with the usual arithmetic conversions, integer promotion and pointer arithmetic.
- `if`/`else`, `while`, `do`/`while`, `for` (with declarations), `switch`/`case`/`default`, `break`, `continue`,
  `goto`, `return`; block scoping and shadowing.
- Global and `static` local variables with constant initializers (including addresses of globals, functions and
  string literals); brace initializers with elision; `char s[] = "..."`.
- Preprocessor: `#define` (object- and function-like, `#`, `##`, `__VA_ARGS__`), `#undef`, `#include`,
  `#if`/`#ifdef`/`#ifndef`/`#elif`/`#else`/`#endif`, `defined`, `#error`, `#pragma once`. `__C93__` is predefined.
- Inline assembly: `asm("LD ABCD, (.c_counter)\nADD32 ABCD, 1\nLD (.c_counter), ABCD");`.
  C globals and functions are labels named `.c_<name>`.

Not supported (rejected with a clear error): floating point, `long long`, passing/returning structs by value
(use pointers), defining variadic functions, bit-fields, designated initializers, compound literals,
variable-length arrays.

## Built-in headers and library

`stddef.h`, `stdint.h`, `stdbool.h`, `limits.h`, `string.h` (`mem*`, `str*`), `stdlib.h` (`malloc`, `calloc`,
`free`, `abs`, `atoi`, `rand`, `srand`), `ctype.h`. The library is written in C and compiled with your program;
only functions you use are emitted, and your own definition of a library function replaces it.
`malloc` is a bump allocator starting after the program image; `free` does nothing.

## Machine model (ABI)

| Item | Value |
|---|---|
| Code origin | `0x080000` (outside the OS's 512 KB) |
| C stack | grows down from `0xE00000` |
| Heap | starts at the end of the program image |
| Registers | `ABCD` accumulator/return value, `QRS` frame pointer, `UVW` C stack pointer, others scratch |
| Calls | arguments pushed right-to-left on the C stack (4 bytes each), `CALL`/`RET` on the hardware call stack |

The program's entry stub sets up the stack, calls `main(0, 0)` and returns with `RET`, so a compiled program
can be started with `CALL` from the OS. Falling off the end of `main` returns 0.
