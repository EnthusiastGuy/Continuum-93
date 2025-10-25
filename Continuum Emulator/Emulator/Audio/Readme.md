Continuum audio structure

Continuum benefits from a complementary processing unit, the Audio Processing Unit which has its own dedicated memory (8 mb)
The APU has 16 channels either of them configurable as stereo or mono.

Sound parameters
================

APU is able to play sounds given parameters such as in this example:

Envelope:
- Attack time 0.000 sec
- Sustain time 0.00007955 sec
- Sustain punch +48.66%
- Decay time 0.2129 sec
Frequency:
- Start frequency 2210Hz
- Min freq. cutoff 3.528Hz
- Slide 0.000 8va/sec
- Delta slide 0.0000e+0 8va/s^2
Vibrato:
- Depth OFF
- Speed OFF
Arpeggiation:
- Frequency mult x 1.047
- Change speed 0.06172 sec
Duty Cycle:
- Duty cycle 50.00%
- Sweep 0.000%/sec
Retrigger:
- Rate OFF
Flanger:
- Offset OFF
- Sweep OFF
Low-Pass Filter:
- Cutoff frequency OFF
- Cutoff sweep OFF
- Resonance 45.00%
High-Pass Filter:
- Cutoff frequency OFF
- Cutoff sweep OFF

Also, the signal should be able to chose from square, sawtooth, sine and noise type of base signals.
Sample rate should be configurable between 44k, 22k, 11k and 6k
Sample size could be 16bit or 8 bit


Individual sounds are defined as a collection of parameters, example:

{
  "wave_type": 1,
  "p_env_attack": 0,
  "p_env_sustain": 0.005922980661768707,
  "p_env_punch": 0.4865962056520096,
  "p_env_decay": 0.30642944631428676,
  "p_base_freq": 0.7909057409945753,
  "p_freq_limit": 0,
  "p_freq_ramp": 0,
  "p_freq_dramp": 0,
  "p_vib_strength": 0,
  "p_vib_speed": 0,
  "p_arp_mod": 0.2242132408118785,
  "p_arp_speed": 0.6332296586130074,
  "p_duty": 0,
  "p_duty_ramp": 0,
  "p_repeat_speed": 0,
  "p_pha_offset": 0,
  "p_pha_ramp": 0,
  "p_lpf_freq": 1,
  "p_lpf_ramp": 0,
  "p_lpf_resonance": 0,
  "p_hpf_freq": 0,
  "p_hpf_ramp": 0,
  "sound_vol": 0.25,
  "sample_rate": 44100,
  "sample_size": 8
}

The individual sounds are stored in the memory controlled by the APU and indexed, so for example, for the sound above,
the index 1 might be assigned. Each stored individual sound receives an available index and when deleted, that index is
made available for assigning again.

Storing a sound is done by:

	SNDSTORE AB, .SoundAddress

.SoundAddress
	#DB [sound params here]


The code above stores the sound as a collection of parameters in the memory controlled by the APU and gives it
an available index number which is returned.
AB is a 16 bit register that receives the newly assigned index value and .SoundAddress is an address label
pointing to the parameters of the sound.

As such, several sounds might form a collection of indexes such as: [1, 2, 3, 4, 5, 6, 7, 8].

The CPU can trigger the APU to play either of these sounds by running:

SNDPLAY 2, 5	; Which contacts the APU and signals playing sound index 2 to channel 5.

SNDSTOP 2		; Stops playing sound associated with index 2

So, the APU is capable of playing individual sounds but it should also be able to play some form of composition 
so a composition's data may look like this:

.CompositionData
[
PLAY soundIndex, startTimeIndex, channel,
PLAY soundIndex, startTimeIndex, channel,
...
REPEAT lastNPlayInstructions, times
PLAY soundIndex, startTimeIndex, channel
...
]

And, to store a composition in APU memory:
COMPSTORE EF, .CompositionData	; EF will hold the index to that composition which can be played with:

COMPPLAY EF

Several compositions can be triggered at the same time and they will play in parallel

C# implementation:

Implement a new thread process that we can interract with from another thread by calling functions such as:

StoreSound, PlaySound, StopSound, StoreComposition, PlayComposition, StopComposition.

The APU implementation will implement all sound generators that describe the sound parameters described above (envelope, frequency etc...)
The channels should be spawned as needed, when a new play is requested, the number of required channels is instantiated and
then the specific sounds/compositions are sent to them to play.


Classification of Sound Parameters into Groups
==============================================

To efficiently use flags and group related parameters, we can classify the parameters into essential and optional groups. Each optional group will be associated with a single flag bit. Here's a suggested grouping, ordered from most essential to least essential:

Essential Parameters (Always Required):
WaveType (1 byte): The type of waveform.
Frequency (4 bytes): The base frequency of the sound.
Envelope Parameters (12 bytes total):
EnvelopeAttack (4 bytes)
EnvelopeSustain (4 bytes)
EnvelopeDecay (4 bytes)
Optional Parameter Groups (Each Associated with a Flag Bit):
Flag Bit 0 - Sound Volume (4 bytes):

SoundVolume
Flag Bit 1 - Envelope Punch (4 bytes):

EnvelopePunch
Flag Bit 2 - Duty Cycle Parameters (8 bytes):

DutyCycle (4 bytes)
DutyCycleRamp (4 bytes)
Flag Bit 3 - Frequency Modulation Parameters (12 bytes):

FreqLimit (4 bytes)
FreqRamp (4 bytes)
FreqDramp (4 bytes)
Flag Bit 4 - Vibrato Parameters (8 bytes):

VibratoDepth (4 bytes)
VibratoSpeed (4 bytes)
Flag Bit 5 - Filter Parameters (20 bytes):

LpfFreq (4 bytes)
LpfRamp (4 bytes)
LpfResonance (4 bytes)
HpfFreq (4 bytes)
HpfRamp (4 bytes)
Flag Bit 6 - Flanger Parameters (8 bytes):

FlangerOffset (4 bytes)
FlangerRamp (4 bytes)
Flag Bit 7 - Phaser Parameters (8 bytes):

PhaserOffset (4 bytes)
PhaserSweep (4 bytes)
Flag Bit 8 - Arpeggio Parameters (8 bytes):

ArpeggioMod (4 bytes)
ArpeggioSpeed (4 bytes)
Flag Bit 9 - Repeat Parameters (4 bytes):

RepeatSpeed
Flag Bit 10 - WaveTypeValues (For COMPLEX WaveType) (Variable Size):

N (1 byte): Number of wave types combined.
WaveTypeValues (N * 5 bytes): For each wave type:
WaveType (1 byte)
Value (4 bytes)
Implementation Details:
Instruction Format:

PLAY <Flags>, <SoundAddress>
Flags: A set of bits indicating which optional parameter groups are included.
SoundAddress: Memory address where parameters are stored sequentially.
Parameter Reading Order:

Read Essential Parameters from SoundAddress.
For each Flag Bit set to 1, read the corresponding Optional Parameter Group in the predefined order.
The sizes of each parameter group are known, allowing the instruction to calculate offsets.
If Flag Bit 10 (WaveTypeValues) is set and WaveType is COMPLEX, read the variable-length WaveTypeValues.
Example Usage:

PLAY 0b0000010011, .SoundAddress
Flags Breakdown:
Bit 0 (SoundVolume): Not set (uses default).
Bit 1 (EnvelopePunch): Set.
Bit 2 (DutyCycle Parameters): Set.
Bit 3 (Frequency Modulation Parameters): Not set.
Bit 4 (Vibrato Parameters): Not set.
Bit 5 (Filter Parameters): Set.
Bits 6-10: Not set.
Parameters to Read:
Essential Parameters.
EnvelopePunch.
DutyCycle Parameters.
Filter Parameters.
Advantages of This Approach:
Efficiency: Developers supply only the parameters they need, conserving memory.
Flexibility: Allows complex sounds by combining different parameter groups.
Simplicity: Fixed order and known sizes for parameter groups simplify the implementation.
Handling WaveType.COMPLEX and WaveTypeValues:

Special Consideration: When WaveType is set to COMPLEX, the WaveTypeValues group becomes essential for defining the combined waveform.
Flag Bit 10: Indicates the inclusion of WaveTypeValues.
Variable Length: The group starts with a count (N) followed by N pairs of wave types and their values.
Example for WaveType.COMPLEX:

Flags: Set Flag Bit 10.
Parameters:
Essential Parameters (including WaveType = COMPLEX).
WaveTypeValues:
N = 3
WaveType[0], Value[0]
WaveType[1], Value[1]
WaveType[2], Value[2]





Prototypes
==========

PLAY .SoundParamsAddress
	
	Plays a specific sound expecting only two parameters positioned at .SoundParamsAddress

	Defaults to volume = 0.5f, wave type = SQUARE
	Expects: frequency (16 bit, 3 -> 4000?), some duration (16 bit, ms)

	Results in playing a simple tone of given frequency over the given duration

PLAY <paramFlags>, .SoundParamsAddress




PLAY 0b0000000000000000, .SoundParamsAddress



Instructions summary
====================

	PLAY nnn
	PLAY rrr
	PLAY nn, nnn	(cancelled)
	PLAY rr, nnn	(cancelled)
	PLAY nn, rrr	(cancelled)
	PLAY rr, rrr	(cancelled)

Flags
=====

	As flags are enabled, their corresponding values must be present in the declaration of the sound.
	Flags can be skipped so no intermediary zeroes all over are needed.

	[default - they always must be there]
	Frequency = 440.0f;			// 4 bytes (float)
	EnvelopeSustain = 0.019f;	// 4 bytes (float)
	SoundVolume = 0.5f;			// 4 bytes (float)

	[bit 15]
	WaveType = SQUARE;			// 1 byte (0 - 255)
		- WaveTypeValues = [];	// if WaveType is 100 (meaning complex) this structure exists
								// starting with a byte meaning the count of valye type bytes (max 255),
								// then those value type bytes, 1 byte each.
								// So, if WaveType is 100 then we check the next byte, and say that's 7
								// we then take additional 7 bytes as waveTypeValue[] before moving on

	[bit 14]
	EnvelopeAttack = 0.0f;		// 4 bytes (float)
	EnvelopeDecay = 0.39f;		// 4 bytes (float)

	[bit 13]
	EnvelopePunch = 0.5f;		// 4 bytes (float)

	[bit 12]
	FreqLimit = 0.0f;			// 4 bytes (float)
	FreqRamp = 0.0f;			// 4 bytes (float)
	FreqDramp = 0.0f;			// 4 bytes (float)

	[bit 11]
	VibratoDepth = 0.0f;		// 4 bytes (float)
	VibratoSpeed = 0.0f;		// 4 bytes (float)

	[bit 10]
	ArpeggioMod = 0.0f;			// 4 bytes (float)
	ArpeggioSpeed = 0.0f;		// 4 bytes (float)

	[bit 9]
	DutyCycle = 0.5f;			// 4 bytes (float)
	DutyCycleRamp = 0.0f;		// 4 bytes (float)

	[bit 8]
	RepeatSpeed = 0.0f;			// 4 bytes (float)

	[bit 7]
	FlangerOffset = 0.0f;		// 4 bytes (float)
	FlangerRamp = 0.0f;			// 4 bytes (float)

	[bit 6]
	PhaserOffset = 0.0f;		// 4 bytes (float)
	PhaserSweep = 0.0f;			// 4 bytes (float)

	[bit 5]
	LpfFreq = 1.0f;				// 4 bytes (float)
	LpfRamp = 0.0f;				// 4 bytes (float)
	LpfResonance = 0.0f;		// 4 bytes (float)

	[bit 4]
	HpfFreq = 0.0f;				// 4 bytes (float)
	HpfRamp = 0.0f;				// 4 bytes (float)

	[bit 3]
	BitDepth = 16;	// [1, 16]	// 1 byte (1 - 16)

	[bit 2]
	SampleRate = 44100;			// 4 bytes (float)

	Example (reversed)

	PLAY .Sound
	
.Sound
	#DB 0b00000000, 0b00000000, 440, 0, 100.0, 0.5

.Sound1
	#DB 0b10000000, 0b00000000, 440, 0, 100.0, 0.5, 7





PCM uncompressed WAV Player
===============================

OGG -> Opus player
===================

Design Goals
-------------------------------------------------------------------------------------------------------
Feature		Vorbis											Opus
Purpose		General-purpose lossy audio compression.		Versatile codec for real-time communication 
			Focused on music.								and general audio.
Focus		High-quality audio for media playback, 			Low-latency, adaptive streaming for both 
			especially for music.							music and speech.
Released	2000											2012

Audio Quality
-------------------------------------------------------------------------------------------------------
Feature			Vorbis											Opus
Sampling Rates	Supports 8 kHz to 192 kHz.						Supports 8 kHz to 48 kHz.
Bitrate Range	Typically 16 kbps to 500 kbps.					6 kbps to 510 kbps.
Quality			Excellent quality for music at 128–192 kbps. 	Exceptional quality across the entire 
				Performs well at high bitrates but degrades		bitrate range, even at low bitrates 
				at lower ones.									like 32 kbps.
Specialization	Optimized for high-bitrate audio,				Handles both speech and music equally
				especially music								well, making it highly versatile.

Compression Efficiency
-------------------------------------------------------------------------------------------------------
Feature					Vorbis										Opus
Algorithm Efficiency	Good compression, better than MP3 at		Superior compression, especially
						equivalent bitrates.						at low to medium bitrates.
File Size				Larger files at lower bitrates				Smaller file sizes due to improved
						compared to Opus.							compression algorithms.

Latency
-------------------------------------------------------------------------------------------------------
Feature			Vorbis										Opus
Latency			Designed for non-real-time applications		Extremely low latency (< 20 ms), suitable
				(playback), with higher latency.			for real-time audio streaming and VoIP.
Applications	Better suited for media files, not ideal 	Ideal for live-streaming, VoIP, gaming,
				for live-streaming or interactive			and interactive applications.
				applications.

Complexity and Processing Requirements
-------------------------------------------------------------------------------------------------------
Feature				Vorbis										Opus
Decoding Complexity	Moderate. Requires more resources than		Lower decoding complexity than Vorbis 
					MP3 but less than AAC.						at similar bitrates, optimized for
																real-time applications.
Encoding Complexity	Higher encoding complexity than decoding.	Efficient and fast, optimized for
																adaptive streaming.

Supported Bitrates and Channels
-------------------------------------------------------------------------------------------------------
Feature				Vorbis										Opus
Bitrate Modes		Supports variable (VBR), average (ABR),		Supports VBR, CBR, and hybrid encoding.
					and constant (CBR) bitrate encoding.
Channel Support		Up to 255 channels.							Up to 255 channels.
Stereo Support		Very efficient stereo encoding.				Excellent stereo and surround sound
																encoding.

Flexibility
-------------------------------------------------------------------------------------------------------
Feature				Vorbis										Opus
Use Cases			Primarily for music files					Designed for both music and speech.
					(e.g., games, streaming audio, etc.).		Suitable for games, live streaming,
																and VoIP.
Real-Time Encoding	Not optimized for real-time.				Designed for real-time encoding and
																decoding.
Low Bitrate Use		Degrades at very low bitrates (< 48 kbps).	Maintains good quality even at extremely
																low bitrates.

Ogg's Streaming Nature:

Ogg is inherently a streaming-friendly container format. It stores audio data in small "pages" that can be
parsed sequentially. This means you can open an Ogg file and read it in chunks without needing the entire
file in memory.

Opus Decoder Design:

The Opus codec supports low-latency decoding, making it ideal for real-time streaming applications.
You can decode a small amount of compressed audio data (e.g., one Opus frame at a time, typically 20 ms
or less) into raw PCM, then immediately play it while preparing the next chunk.

Low RAM Footprint:

Since decoding is done on small chunks sequentially, the memory usage is minimal. Only the buffer for the
current chunk and some decoder state needs to remain in memory.