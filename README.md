# Continuum93

*A fantasy retro computer + emulator for building and running assembly-driven games and demos.*

> **Status:** Released / work-in-progress  
> **Community:** Discord — https://discord.gg/6AYkhnzxD9

---

## Table of contents

- [What is Continuum93?](#what-is-continuum93)
- [Features at a glance](#features-at-a-glance)
- [Hardware (virtual) overview](#hardware-virtual-overview)
- [Install & Run](#install--run)
  - [Windows](#windows)
  - [macOS](#macos)
  - [Linux](#linux)
  - [Raspberry Pi OS (64-bit, experimental)](#raspberry-pi-os-64-bit-experimental)
- [Writing code](#writing-code)
  - [Visual Studio Code](#visual-studio-code)
  - [Notepad++](#notepad)
- [Running your programs](#running-your-programs)
- [Configuration](#configuration)
- [Debugging (Continuum Tools)](#debugging-continuum-tools)
- [Documentation](#documentation)
- [Contributing](#contributing)
- [License & assets](#license--assets)
- [Bugs & support](#bugs--support)

---

## What is Continuum93?

Continuum93 is a **fantasy computer** and **emulator** built for retro-style games and experiments. It runs programs written in a custom assembly language and ships with a small operating system (a visual file browser) so you can explore and run `.asm` programs directly.

---

## Features at a glance

- **Virtual 24-bit address space** with **16 MB RAM**
- **Layered video RAM** (VRAM at end of RAM, ~128 KB per layer, up to 8 layers)
- **Fast virtual CPU** — speed scales with your machine’s single-core performance  
  *(roughly from a few MHz on low-end machines to hundreds of MHz on high-end ones)*
- **Cross-platform renderer** via MonoGame **DesktopGL**
- **Built-in OS**: browse the virtual filesystem and run `.asm` programs
- **Configurable** via `Data/init.cfg`

---

## Hardware (virtual) overview

- **CPU**: virtual; instruction set designed for retro game development  
  Speed is proportional to your host’s single-core performance: typically **~2–3 MHz** on the low end and **500 MHz+** on fast machines.
- **Addressing**: **24-bit**
- **RAM**: **16 MB**
- **Video**: VRAM mapped at the end of main RAM; **~128 KB per layer**, up to **8 layers**
- For registers, interrupts, addressing modes, and instruction reference, see the PDF manuals in the **Support** directory.

---

## Install & Run

You can either download a release (when available) or build from source. For detailed build instructions (dependencies, SDL2/OpenAL on macOS/Linux, content pipeline), see **[BUILDING.md](BUILDING.md)**.

### Windows

1. Unzip the release package.
2. Run the emulator executable, or from source:
   ```powershell
   dotnet restore
   dotnet build -c Release
   dotnet run -c Release
   ```
3. The default OS can load and execute `*.asm` files.

### macOS

1. Install runtime deps (see **BUILDING.md** for DesktopGL prerequisites).
2. From source:
   ```bash
   dotnet restore
   dotnet build -c Release
   dotnet run -c Release
   ```

### Linux

1. Install runtime deps (SDL2, OpenAL — see **BUILDING.md**).
2. From source:
   ```bash
   dotnet restore
   dotnet build -c Release
   dotnet run -c Release
   ```

### Raspberry Pi OS (64-bit, experimental)

If you’re using an ARM build artifact:
```bash
sudo chmod +x ./Continuum93
./Continuum93
```
If you see “Permission denied,” ensure the binary is executable. OpenGL driver support on the Pi varies by model/drivers.

---

## Writing code

Create your `.asm` programs and place them under:
```
Data/filesystem/programs/<your-project>/
```

You can keep code split across multiple files; the OS can load and run your entry `.asm`.

### Visual Studio Code

- Recommended for multi-file projects.
- Install the **ASM Code Lens** extension for syntax highlighting.  
  *(Continuum93 uses a custom instruction set, so highlighting won’t be perfect but it helps.)*

### Notepad++

1. Enable the **Vibrant Ink** style (optional).
2. Import the user language file:
   - `Support/Notepad++ user defined language/Continuum93 Assembly.xml`
3. In Notepad++: **Language → User Defined Language → Open User Defined Language folder** and copy the file there.
4. If highlighting seems off, explicitly select **Language → Continuum93 assembly**.

---

## Running your programs

1. Launch Continuum93.
2. In the built-in OS (file browser), navigate to:
   ```
   programs/<your-project>/
   ```
3. Move the selector to your entry `.asm` file and press **Enter** to run.

See the *Operating System* section in the User Manual for details.

---

## Configuration

Global settings live in:
```
Data/init.cfg
```
Review and adjust this file to tweak runtime options.

---

## Debugging (Continuum Tools)

**Continuum Tools** is included alongside the emulator and provides debugging capabilities.  
- Read **“Continuum Tools Manual.pdf”** in the **Support** directory.
- Currently available on **Windows**.

---

## Documentation

PDF manuals are provided in the **Support** directory, including:
- **User Manual** (Operating System, usage)
- **Hardware & Assembly Reference** (registers, addressing, instruction set)
- **Continuum Tools Manual**

Also check the example programs under:
```
Data/filesystem/programs/
```

---

## Contributing

We welcome contributions! Please read:
- **[CONTRIBUTING.md](CONTRIBUTING.md)** for workflow, code style, and PR checklist
- **[CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md)** for community guidelines
- **[ROADMAP.md](ROADMAP.md)** for near-term goals

---

## License & assets

- **Code:** [MIT License](LICENSE) 

---

## Bugs & support

Bugs happen! Please report issues via GitHub or drop by the **#support** channel on Discord:  
https://discord.gg/6AYkhnzxD9

When filing a bug, include:
- OS and GPU
- Steps to reproduce (program or snippet if possible)
- Logs and screenshots if relevant
