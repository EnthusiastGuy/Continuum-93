# Continuum Emulator Roadmap (highly DRAFT)

This roadmap outlines the planned evolution across three major phases: **Near-Term (Stability & Cleanup)**, **Mid-Term (Feature Expansion)**, and **Long-Term (Vision & Ecosystem)**.

---

## Phase 1: Near-Term (Stability & Cleanup)

*Focus: Finalizing the core architecture, establishing robust testing, and improving developer experience.*

### Architecture & Core
- **Namespace Migration:** Finish namespace migration to `Continuum93.*` and remove legacy references.
- **CPU instructions:** Complete implementation of all documented CPU instructions and addressing modes.
- **Register Refactoring:** Refactor register model and flags handling for improved clarity and correctness.
- **Deterministic Stepper:** Implement deterministic single-instruction stepper necessary for reliable test fixtures.
- **I/O Cleanup:** Clean up and document the interrupt vector and file system I/O handlers.

### Audio
- **Native/Managed Fallback:** Provide native Ogg/Opus decoding for Linux/macOS or managed fallback via `IAudioDecoder` interface to ensure cross-platform audio support.

### Testing
- **Complete coverage** for CPU instruction set, memory controllers, and I/O devices.

---

## Phase 2: Mid-Term (Feature Expansion & Usability)

*Focus: Delivering a smoother user experience, expanding the development ecosystem, and simplifying sound integration.*

### Sound System Usability & Refactoring
- **API Simplification:** Refactor the public sound API (`SoundSystem` class) to be more intuitive, decoupling playback commands from low-level mixing logic.
- **Synthesizer Mixer:** Clean up the internal mixer and synthesizer components to enable easier integration of new sound channels (e.g., dedicated PCM, noise, or custom wave channels).
- **Runtime Controls:** Implement a way to control basic sound properties (volume, pan) live during emulation via a standardized command or API call.

### MOSAIC Compiler (NEW)
- **Language Specification:** Define a simple, high-level language (MOSAIC) similar to BASIC with structured control flow.
- **Compiler Front-End:** Develop a compiler that accepts MOSAIC source code.
- **Native ASM Generation:** The compiler must output **native Continuum Assembly (ASM) code** suitable for execution on the emulated machine. This will significantly lower the bar for new content creation.


---

## Phase 3: Long-Term (Vision & Ecosystem)

*Focus: Building a sustainable, advanced platform and community around the emulator.*

### Advanced Debugging & Profiling
- **Remote Protocol:** Finalize the optional remote debugging protocol (e.g., leveraging WebSockets or gRPC) to allow external IDEs to connect.
- **Instruction Profiler:** Implement a cycle-accurate instruction counter and profiler to identify performance bottlenecks in emulated programs.
- **Memory Map Viewer:** Create a graphical tool within the debugger to visualize the emulated machine's memory map and allocation.

### Content Distribution & Community
- **Asset Packager:** Develop a simple tool to bundle assembly code, graphics, and sound assets into a single, distributable `.cont` file.
- **Online Registry/Gallery:** Create a basic web presence or GitHub repository to showcase user-contributed programs and assets, fostering a content ecosystem.

### Emulation Advancement
- **Hardware Abstraction:** Further generalize hardware interfaces (e.g., `IMemoryController`, `IProcessor`) to support emulating *different generations* or variants of the Continuum machine architecture in the future.
- **JIT Compilation (Stretch Goal):** Explore a simple Just-In-Time (JIT) compiler for heavily used blocks of machine code to significantly boost emulation performance.
