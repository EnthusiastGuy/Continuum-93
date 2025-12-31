# Continuum 93 Emulator - Work Context

## Project Overview
**Continuum 93** is a fantasy retro computer emulator written in C# using MonoGame. It emulates a virtual CPU that can run assembly code programs, designed for retro games and demos.

### Key Details
- **Language**: C# (.NET 8.0)
- **Framework**: MonoGame 3.8.1.303
- **Entry Point**: `Program.cs` → `Continuum.cs`
- **Build Status**: ✅ Builds successfully (with minor warnings)

## Project Structure

### Core Components
- **`Program.cs`** - Application entry point
- **`Continuum.cs`** - Main game/emulator class (MonoGame Game class)
- **`Emulator/`** - Core emulator implementation
  - `CPU/` - Virtual CPU, registers, instructions
  - `Graphics.cs` - Graphics rendering
  - `Interrupts/` - System interrupts (video, filesystem, etc.)
  - `Machine.cs` - Main machine state
  - `Audio/` - Audio processing (APU, OGG, WAV support)
- **`Data/filesystem/programs/`** - Example assembly programs
- **`Data/os/`** - Built-in operating system (file browser)
- **`ServiceModule/`** - UI/service layer
- **`CodeAnalysis/`** - Debugging and analysis tools

### Virtual Hardware Specs
- **CPU**: Virtual CPU, frequency proportional to host CPU (2-3 MHz to 500+ MHz)
- **RAM**: 16 MB (24-bit addressing)
- **Video RAM**: 128k per layer (up to 8 layers)
- **Registers**: Multiple register pages

## Work Log

### 2024 - Initial Setup
- ✅ Project loaded and examined
- ✅ Build verified - compiles successfully
- ✅ Emulator executed successfully (`dotnet run`)
- ✅ Context document created

### 2024 - LD Instruction Analysis
- ✅ Analyzed LD instruction execution (`ExLD.cs`)
- ✅ Documented indexing support patterns
- ✅ Created comprehensive analysis document (`LD_INSTRUCTION_ANALYSIS.md`)
- ✅ Identified 245 instruction variants with 193 using indexing (78.8%)

## Current State
- Project is ready for development
- All dependencies restored
- Build system working
- Emulator runs and displays correctly

## Notes
- Minor warnings in build (unused variables, obsolete methods) - non-critical
- Audio library cleanup warning on exit - harmless
- Example programs available in `Data/filesystem/programs/`

## Recent Analysis

### LD Instruction Indexing Support
- **File**: `Emulator/Execution/ExLD.cs` (3,891 lines)
- **Total Variants**: 245 instruction opcodes
- **Indexing Variants**: 193 (78.8% of all LD variants)
- **Key Feature**: Two-level indexing support (base address + offset/index)
  - Base: Immediate (`nnn`) or Register (`rrr`)
  - Index: Immediate (`nnn`), 8-bit reg (`r`), 16-bit reg (`rr`), or 24-bit reg (`rrr`)
- **Analysis Document**: See `LD_INSTRUCTION_ANALYSIS.md` for full details

## Next Steps
- [To be determined based on user requests]

