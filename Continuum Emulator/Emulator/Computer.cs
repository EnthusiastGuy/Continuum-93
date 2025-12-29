using Continuum93.Emulator;
using Continuum93.CodeAnalysis;
using Continuum93.Emulator.Audio;
using Continuum93.Emulator.Audio.Ogg;
using Continuum93.Emulator.CPU;
using Continuum93.Emulator.RAM;
using Continuum93.Tools;
using System;
using System.Threading;

namespace Continuum93.Emulator
{
    public class Computer: IDisposable
    {
        private const int StackPointerResetValue = 100;

        public int InstructionDelayIterations { get; set; } = 0;

        public MemoryController MEMC;
        public Microprocessor CPU;
        public Graphics GRAPHICS;
        public APUThreadWrapper APU;

        private bool RUNNING = true;
        private bool PAUSED = false;

        public Computer(bool initWithAudio = false)
        {
            MEMC = new MemoryController(this);
            CPU = new Microprocessor(this);
            GRAPHICS = new Graphics(this);
            APU = new APUThreadWrapper();
            CPU.REGS.IPO = 0;   // Set the instruction pointer at the beginning of the start code
            CPU.REGS.SPR = 0;   // Beginning of the register stack pointer in the RSRAM
            CPU.REGS.SPC = 0;   // Beginning of the call stack pointer in the CSRAM
            //APU.RegisterWav("Data/w98.wav", channel: 0);
            //APU.PlayWav(0);
            //APU.RegisterOgg("Data/long_speech_24k_48000.ogg", channel: 0);
            //APU.PlayOgg(0);

            if (initWithAudio)
            {
                APU.PlayInitializationSounds();    // Play a sound to indicate APU is up and running
            }

    }

        /// <summary>
        /// Loads a sequence of opcode bytes into memory, starting at the beginning.
        /// </summary>
        /// <param name="opCodes">The opcodes to load into memory.</param>
        public void LoadMem(params byte[] opCodes)
        {
            //Array.Copy(opCodes, 0, MEMC.RAM.Data, 0, opCodes.Length); // Upgrade to this later on
            for (uint i = 0; i < opCodes.Length; i++)
                MEMC.Set8bitToRAM(i, opCodes[i]);
        }

        /// <summary>
        /// Loads a sequence of opcode bytes into memory at a specified address.
        /// </summary>
        /// <param name="address">The start address in memory where the opcodes will be loaded.</param>
        /// <param name="opCodes">The opcodes to load into memory.</param>
        public void LoadMemAt(uint address, params byte[] opCodes) => Array.Copy(opCodes, 0, MEMC.RAM.Data, address, opCodes.Length);

        /// <summary>
        /// Loads multiple sequences of opcode bytes into memory sequentially, starting at a specified address.
        /// </summary>
        /// <param name="address">The start address in memory where the first sequence of opcodes will be loaded.</param>
        /// <param name="opCodesArrays">Multiple opcode sequences to be loaded into memory in order.</param>
        public void LoadMemAt(uint address, params byte[][] opCodesArrays)
        {
            foreach (byte[] opCodes in opCodesArrays)
            {
                Array.Copy(opCodes, 0, MEMC.RAM.Data, address, opCodes.Length);
                address += (uint)opCodes.Length; // Update the address for the next array
            }
        }

        /// <summary>
        /// Fill memory starting with given address, of given length with given
        /// byte value
        /// </summary>
        public void FillMemoryAt(int address, int length, byte value) => Array.Fill(MEMC.RAM.Data, value, address, length);

        /// <summary>
        /// Retrieves a sequence of bytes from memory, starting from a specified address and of a specified length.
        /// </summary>
        /// <param name="address">The address in memory from where to start copying.</param>
        /// <param name="length">The number of bytes to copy from memory.</param>
        /// <returns>A byte array containing the copied bytes from memory.</returns>
        public byte[] GetMemFrom(uint address, uint length)
        {
            byte[] response = new byte[length];
            Array.Copy(MEMC.RAM.Data, address, response, 0, length);

            return response;
        }

        public void Clear()
        {
            MEMC.ClearAllRAM();
            MEMC.ClearHMEM();
            CPU.REGS.ClearAll();
            CPU.FREGS.ClearAll();
            GRAPHICS.InitializeVideoPages();
            GRAPHICS.LAYER_VISIBLE_BITS = 0xFF;
            GRAPHICS.LAYER_BUFFER_MODE_BITS = 0xFF;
            GRAPHICS.LAYER_VISIBLE = [true, true, true, true, true, true, true, true];
            GRAPHICS.LAYER_BUFFER_AUTO_MODE = [true, true, true, true, true, true, true, true];
            RUNNING = true;
        }

        public void Start() => RUNNING = true;

        public void Stop() => RUNNING = false;

        public bool IsRunning => RUNNING;

        public bool IsPaused => PAUSED;

        public void Pause() => PAUSED = true;
        
        public void Unpause() => PAUSED = false;

        public void Run(uint address = 0)
        {
            CPU.REGS.IPO = address;
            CPUBenchmark.Start();

            try
            {
                while (IsRunning)
                {
                    if (!IsPaused)
                    {
                        ExecuteNextInstruction();
                        //CPUBenchmark.IncrementInstructions();
                    }

                    DebuggerTrap();
                }
            }
            catch (Exception e)
            {
                Log.WriteLine("Exception met at Machine.Run: " + e.Message);
            }
        }

        public double RunSingleInstructionRepeatedly()
        {
            CPU.REGS.IPO = 0;
            CPUBenchmark.Start();

            int runs = 1_000_000;

            try
            {
                while (runs > 0)
                {
                    if (!IsPaused)
                    {
                        ExecuteNextInstruction(true);
                        CPUBenchmark.IncrementInstructions();
                    }

                    runs--;
                }
                return CPUBenchmark.GetInstructionsPerSecond();
            }
            catch (Exception e)
            {
                Log.WriteLine("Exception met at Machine.RunSingleInstructionRepeatedly: " + e.Message);
            }

            return 0;
        }

        private void DebuggerTrap()
        {
            if (DebugState.StepByStep)
            {
                Log.WriteLine($"Debugger trap in step-by-step");
                while (!DebugState.MoveNext)
                {
                    if (!DebugState.StepByStep || !IsRunning)
                    {
                        Log.WriteLine($"Debug trap exited");
                        break;
                    }

                    Thread.Sleep(1); // yield to allow UI / shutdown to proceed
                }
                DebugState.MoveNext = false;
            }
        }

        public void SetIP(uint address)
        {
            CPU.REGS.IPO = address;
        }

        public void ExecuteNextInstruction(bool resetIP = false)
        {
            if (resetIP) CPU.REGS.SPR = StackPointerResetValue;
            byte opcode = MEMC.Fetch();
            Action<Computer> action = InstructionSet.IJT[opcode];

            action(this);

            if (InstructionDelayIterations > 0)
            {
                DelayHelper.AtomicDelay(InstructionDelayIterations - 1);
            }
        }

        public void Dispose()
        {
            APU?.StopAPUThread();  // Safely stop the APU thread
            GRAPHICS.Dispose();
        }
    }
}
