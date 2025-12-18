# Audio Processing Unit (APU) Architecture Documentation

## Executive Summary

The Continuum 93 Audio Processing Unit (APU) is a dedicated audio processing system that runs in a separate thread and provides comprehensive sound generation capabilities. The APU supports parameter-based sound synthesis (XSound), WAV file playback, and OGG/Opus streaming. It operates independently from the CPU, allowing asynchronous audio playback.

## APU Architecture

### Thread-Based Design

The APU runs in a dedicated thread (`APUThreadWrapper`):
- **Separate Thread**: Audio processing runs independently from CPU execution
- **Command Queue**: CPU sends commands via a thread-safe queue
- **Non-Blocking**: Audio operations don't block CPU execution
- **Asynchronous Playback**: Multiple sounds can play simultaneously

### APU Components

1. **APU Core** (`APU` class):
   - Manages active sounds and sound library
   - Handles sound storage and playback
   - Tracks sound players

2. **APU Thread Wrapper** (`APUThreadWrapper` class):
   - Manages the APU thread
   - Provides thread-safe interface
   - Handles command queuing
   - Manages WAV and OGG players

3. **XSound System**:
   - Parameter-based sound synthesis
   - Multiple waveform types
   - Envelope, frequency modulation, effects

4. **Streaming Players**:
   - WAV file streaming
   - OGG/Opus file streaming

## Sound Generation: XSound System

### XSound Overview

XSound is a parameter-based sound synthesis system that generates audio from mathematical parameters. Sounds are defined by a set of parameters stored in memory and read by the `PLAY` instruction.

### PLAY Instruction

**Syntax**:
```
PLAY <address>
```

**Addressing Modes**:
- `PLAY nnn` - Play sound with parameters at absolute 24-bit address
- `PLAY rrr` - Play sound with parameters at register-indirect address (24-bit register)

**Operation**:
1. Reads sound parameters from memory at specified address
2. Parses parameters based on flags
3. Creates `XSoundParams` structure
4. Sends sound to APU for playback
5. Returns immediately (non-blocking)

**Example**:
```
.SoundParams
    #DB 0b00000000, 0b00000000    ; Flags (2 bytes)
    #DB 440.0                      ; Frequency (4 bytes, float)
    #DB 0.019                      ; EnvelopeSustain (4 bytes, float)
    #DB 0.5                        ; SoundVolume (4 bytes, float)

.Main
    LD XYZ, .SoundParams
    PLAY XYZ                       ; Play sound
```

### Sound Parameter Structure

Sound parameters are stored in memory as a structured format:

#### Parameter Layout

**Required Parameters** (Always Present):
1. **Flags** (2 bytes, 16-bit): Bit flags indicating which optional parameters are present
2. **Frequency** (4 bytes, float): Base frequency in Hz (default: 440.0)
3. **EnvelopeSustain** (4 bytes, float): Sustain time in seconds (default: 0.019)
4. **SoundVolume** (4 bytes, float): Master volume 0.0-1.0 (default: 0.5)

**Optional Parameters** (Based on Flags):

| Flag Bit | Parameter Group | Size | Parameters |
|----------|----------------|------|------------|
| 15 | WaveType | 1 byte | WaveType (0-255), WaveTypeValues (if COMPLEX) |
| 14 | Envelope | 8 bytes | EnvelopeAttack (4 bytes), EnvelopeDecay (4 bytes) |
| 13 | EnvelopePunch | 4 bytes | EnvelopePunch (4 bytes) |
| 12 | Frequency Modulation | 12 bytes | FreqLimit, FreqRamp, FreqDramp (each 4 bytes) |
| 11 | Vibrato | 8 bytes | VibratoDepth, VibratoSpeed (each 4 bytes) |
| 10 | Arpeggio | 8 bytes | ArpeggioMod, ArpeggioSpeed (each 4 bytes) |
| 9 | Duty Cycle | 8 bytes | DutyCycle, DutyCycleRamp (each 4 bytes) |
| 8 | Repeat | 4 bytes | RepeatSpeed (4 bytes) |
| 7 | Flanger | 8 bytes | FlangerOffset, FlangerRamp (each 4 bytes) |
| 6 | Phaser | 8 bytes | PhaserOffset, PhaserSweep (each 4 bytes) |
| 5 | Low-Pass Filter | 12 bytes | LpfFreq, LpfRamp, LpfResonance (each 4 bytes) |
| 4 | High-Pass Filter | 8 bytes | HpfFreq, HpfRamp (each 4 bytes) |
| 3 | Bit Depth | 1 byte | BitDepth (1-16) |
| 2 | Sample Rate | 4 bytes | SampleRate (float) |

#### WaveType Values

**Standard Wave Types** (0-52):
- `0` - SINE
- `1` - CLIPPEDSINE
- `2` - HALFSINE
- `3` - SQUARE
- `4` - PULSEWAVE
- `5` - TRIANGLE
- `6` - RAMPWAVE
- `7` - SAWTOOTH
- `8` - REVERSESAWTOOTH
- `9` - BREAKER
- `10` - TAN
- `11` - EXPONENTIAL
- `12` - WAVEFOLDING
- `13` - WHISTLE
- `14` - ORGAN
- `15` - FORMANT
- `16` - RINGMODULATION
- `17` - LFO
- `18` - WARPEDSINE
- `19` - FRACTALSAWTOOTH
- `20` - DETUNEDSINE
- `21` - PHASEDISTORDETSINE
- `50` - NOISE
- `51` - PINKNOISE
- `52` - GAUSSIANNOISE

**Complex Wave Type** (100):
- `100` - COMPLEX: Combines multiple wave types
- When WaveType = 100, additional data follows:
  - Count byte (1 byte): Number of wave types to combine
  - Wave type pairs (N × 2 bytes): For each wave type:
    - WaveType (1 byte): Wave type to combine
    - Weight (1 byte): Weight/contribution (0-99)

#### Parameter Reading Order

Parameters are read sequentially from memory:

1. Flags (2 bytes) - Always first
2. Required parameters (12 bytes total):
   - Frequency (4 bytes)
   - EnvelopeSustain (4 bytes)
   - SoundVolume (4 bytes)
3. Optional parameters (in flag bit order, highest to lowest):
   - If bit 15 set: WaveType (1 byte) + WaveTypeValues (if COMPLEX)
   - If bit 14 set: EnvelopeAttack, EnvelopeDecay (8 bytes)
   - If bit 13 set: EnvelopePunch (4 bytes)
   - If bit 12 set: FreqLimit, FreqRamp, FreqDramp (12 bytes)
   - If bit 11 set: VibratoDepth, VibratoSpeed (8 bytes)
   - If bit 10 set: ArpeggioMod, ArpeggioSpeed (8 bytes)
   - If bit 9 set: DutyCycle, DutyCycleRamp (8 bytes)
   - If bit 8 set: RepeatSpeed (4 bytes)
   - If bit 7 set: FlangerOffset, FlangerRamp (8 bytes)
   - If bit 6 set: PhaserOffset, PhaserSweep (8 bytes)
   - If bit 5 set: LpfFreq, LpfRamp, LpfResonance (12 bytes)
   - If bit 4 set: HpfFreq, HpfRamp (8 bytes)
   - If bit 3 set: BitDepth (1 byte)
   - If bit 2 set: SampleRate (4 bytes)

### Sound Parameter Examples

#### Simple Sound (Minimal Parameters)

```
.SimpleSound
    #DB 0x00, 0x00            ; Flags: no optional parameters
    #DB 440.0                  ; Frequency: 440 Hz (A4 note)
    #DB 0.5                    ; EnvelopeSustain: 0.5 seconds
    #DB 0.5                    ; SoundVolume: 50%

; Usage:
    LD XYZ, .SimpleSound
    PLAY XYZ
```

#### Complex Sound (Multiple Parameters)

```
.ComplexSound
    #DB 0b11111111, 0b11111111 ; Flags: all parameters enabled
    #DB 220.0                   ; Frequency: 220 Hz
    #DB 0.1                     ; EnvelopeSustain: 0.1 seconds
    #DB 0.8                     ; SoundVolume: 80%
    #DB 3                       ; WaveType: SQUARE
    #DB 0.0                     ; EnvelopeAttack: 0.0 seconds
    #DB 0.3                     ; EnvelopeDecay: 0.3 seconds
    #DB 0.5                     ; EnvelopePunch: 0.5
    #DB 100.0                   ; FreqLimit: 100 Hz
    #DB 0.0                     ; FreqRamp: 0.0
    #DB 0.0                     ; FreqDramp: 0.0
    #DB 0.1                     ; VibratoDepth: 0.1
    #DB 5.0                     ; VibratoSpeed: 5.0 Hz
    #DB 1.0                     ; ArpeggioMod: 1.0
    #DB 0.5                     ; ArpeggioSpeed: 0.5
    #DB 0.5                     ; DutyCycle: 50%
    #DB 0.0                     ; DutyCycleRamp: 0.0
    #DB 0.0                     ; RepeatSpeed: 0.0
    #DB 0.0                     ; FlangerOffset: 0.0
    #DB 0.0                     ; FlangerRamp: 0.0
    #DB 0.0                     ; PhaserOffset: 0.0
    #DB 0.0                     ; PhaserSweep: 0.0
    #DB 1.0                     ; LpfFreq: 1.0
    #DB 0.0                     ; LpfRamp: 0.0
    #DB 0.0                     ; LpfResonance: 0.0
    #DB 0.0                     ; HpfFreq: 0.0
    #DB 0.0                     ; HpfRamp: 0.0
    #DB 16                      ; BitDepth: 16-bit
    #DB 44100.0                 ; SampleRate: 44100 Hz
```

#### Complex Wave Type Example

```
.ComplexWaveSound
    #DB 0b10000000, 0x00        ; Flags: WaveType enabled
    #DB 440.0                   ; Frequency
    #DB 0.5                     ; EnvelopeSustain
    #DB 0.5                     ; SoundVolume
    #DB 100                     ; WaveType: COMPLEX
    #DB 3                       ; Count: 3 wave types
    #DB 3, 50                   ; SQUARE at 50% weight
    #DB 7, 30                   ; SAWTOOTH at 30% weight
    #DB 0, 20                   ; SINE at 20% weight
```

## Sound Effects and Envelopes

### Envelope Parameters

The envelope controls how the sound's amplitude changes over time:

- **EnvelopeAttack** (0.0+): Time for sound to reach full volume (attack phase)
- **EnvelopeSustain** (0.0+): Time at full volume (sustain phase)
- **EnvelopePunch** (0.0-1.0): Boosted sustain level (adds emphasis)
- **EnvelopeDecay** (0.0+): Time for sound to fade out (decay phase)

**Envelope Phases**:
1. **Attack**: Sound rises from 0 to full volume
2. **Sustain**: Sound maintains full volume (with optional punch boost)
3. **Decay**: Sound fades from full volume to 0

### Frequency Modulation

- **FreqLimit** (Hz): Minimum frequency cutoff
- **FreqRamp** (8va/sec): Frequency slide rate (octaves per second)
- **FreqDramp** (8va/s²): Frequency slide acceleration

### Effects

- **Vibrato**: Periodic frequency modulation (depth and speed)
- **Arpeggio**: Frequency stepping (modulation and speed)
- **Duty Cycle**: Pulse width modulation for square waves
- **Flanger**: Comb filter effect (offset and ramp)
- **Phaser**: Phase-shifting effect (offset and sweep)
- **Filters**: Low-pass and high-pass filtering

## WAV File Playback

The APU supports streaming WAV file playback:

### WAV Player Methods

- **RegisterWav(path, channel)**: Register a WAV file on a channel
- **PlayWav(channel)**: Play/resume WAV on channel
- **PauseWav(channel)**: Pause WAV on channel (preserves position)
- **StopWav(channel)**: Stop WAV on channel (resets position)
- **SetWavVolume(channel, volume)**: Set volume (0.0-1.0)
- **SetWavLoop(channel, looped)**: Enable/disable looping

### WAV Format Support

- **PCM uncompressed** WAV files
- **Streaming playback**: Files are streamed, not fully loaded
- **Multiple channels**: Up to 256 channels (byte channel IDs)
- **Independent control**: Each channel can be controlled separately

## OGG/Opus Streaming

The APU supports OGG container with Opus codec:

### OGG Player Methods

- **RegisterOgg(path, channel)**: Register an OGG file on a channel
- **PlayOgg(channel)**: Play/resume OGG on channel
- **PauseOgg(channel)**: Pause OGG on channel
- **StopOgg(channel)**: Stop OGG on channel (resets position)
- **SetOggVolume(channel, volume)**: Set volume (0.0-1.0)
- **SetOggLoop(channel, looped)**: Enable/disable looping

### OGG/Opus Features

- **Low latency**: Optimized for real-time streaming
- **Efficient compression**: Superior compression at low bitrates
- **Streaming-friendly**: Reads in small chunks, minimal memory usage
- **Versatile**: Handles both music and speech well
- **Sample rates**: 8 kHz to 48 kHz
- **Bitrates**: 6 kbps to 510 kbps

## APU Thread Management

### Thread Safety

All APU operations are thread-safe:
- Commands are queued in `ConcurrentQueue<Action>`
- APU thread processes commands sequentially
- CPU thread enqueues commands without blocking

### Command Processing

```
CPU Thread                    APU Thread
    |                             |
    |-- PlaySound() ------------>|
    |                             |-- Process command
    |                             |-- Generate audio
    |                             |-- Play sound
    |<-- Returns immediately -----|
    |                             |
```

### Thread Lifecycle

1. **Initialization**: APU thread starts when `APUThreadWrapper` is created
2. **Command Processing**: Thread continuously processes queued commands
3. **Cleanup**: Thread stops when `Dispose()` is called
4. **Resource Management**: All players and sounds are disposed on shutdown

## Sound Library Management

The APU maintains a sound library for stored sounds:

- **StoreSound(parameters)**: Store sound parameters, returns index
- **PlaySound(index)**: Play stored sound by index
- **StopSound(index)**: Stop stored sound by index
- **ReplaceSoundAtIndex(index, parameters)**: Replace sound at index

**Sound Library Features**:
- Automatic index assignment
- Index reuse when sounds are deleted
- Persistent storage until explicitly removed

## Performance Considerations

### Asynchronous Operation

- **Non-blocking**: Audio operations don't block CPU
- **Parallel processing**: Audio and CPU run concurrently
- **Low latency**: Commands processed quickly via queue

### Memory Management

- **Streaming**: WAV/OGG files streamed, not fully loaded
- **Parameter-based**: XSound uses minimal memory (parameters only)
- **Automatic cleanup**: Finished sounds are automatically cleaned up

### Resource Usage

- **Thread overhead**: Single dedicated thread for all audio
- **Queue-based**: Minimal synchronization overhead
- **Efficient playback**: Direct audio buffer access

## Usage Examples

### Basic Sound Playback

```
.SoundParams
    #DB 0x00, 0x00        ; Flags
    #DB 440.0              ; Frequency: A4
    #DB 0.5                ; Sustain: 0.5s
    #DB 0.5                ; Volume: 50%

.Main
    LD XYZ, .SoundParams
    PLAY XYZ               ; Play sound
    RET
```

### Multiple Sounds

```
.Sound1
    #DB 0x00, 0x00
    #DB 440.0, 0.5, 0.5

.Sound2
    #DB 0x00, 0x00
    #DB 880.0, 0.3, 0.7

.Main
    LD XYZ, .Sound1
    PLAY XYZ               ; Play first sound
    
    LD XYZ, .Sound2
    PLAY XYZ               ; Play second sound (overlaps)
    RET
```

### Sound with Effects

```
.EffectSound
    #DB 0b11110000, 0x00   ; Flags: WaveType, Envelope, Vibrato
    #DB 440.0               ; Frequency
    #DB 0.2                 ; Sustain
    #DB 0.6                 ; Volume
    #DB 3                   ; WaveType: SQUARE
    #DB 0.1                 ; Attack
    #DB 0.4                 ; Decay
    #DB 0.2                 ; VibratoDepth
    #DB 5.0                 ; VibratoSpeed

.Main
    LD XYZ, .EffectSound
    PLAY XYZ
    RET
```

## Conclusion

The Continuum 93 APU provides comprehensive audio capabilities through parameter-based synthesis, file streaming, and thread-based asynchronous operation. The XSound system enables flexible sound generation, while WAV and OGG support provide traditional audio playback. Understanding the APU architecture is essential for implementing audio in Continuum 93 programs.

