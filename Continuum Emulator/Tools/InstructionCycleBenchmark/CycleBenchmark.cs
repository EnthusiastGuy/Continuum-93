using Continuum93.Emulator;
using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator.AutoDocs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Continuum93.Tools.InstructionCycleBenchmark
{
    public static class CycleBenchmark
    {
        private static List<BenchmarkedInstruction> benchInstructions = new();
        public static void Run()
        {
            BenchAll();
        }

        private static void BenchAll()
        {
            benchInstructions.Clear();
            string pool = DocsInstructionSamples.GenerateForBenchmark();

            string[] lines = pool.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                BenchmarkedInstruction instr = new();
                instr.InstructionName = line;

                if (line.Contains("WAIT") || line.Contains("INT"))
                {
                    continue;   // We don't benchmark those
                }

                Computer computer = new();
                Assembler cp = new();
                computer.Clear();
                cp.Build(line);

                byte[] compiled = cp.GetCompiledCode();

                computer.LoadMemAt(1000, compiled);
                double instructionsPerSecond = computer.RunSingleInstructionRepeatedly();
                instr.RunsPerSecond = instructionsPerSecond;

                benchInstructions.Add(instr);

                Log.WriteLine($"{line}\t|{instructionsPerSecond}|runs per second.");
            }

            ComputeBenchmarkedInstructions();

            Log.WriteLine("\n\nCalibrated output\n------------------------");

            foreach (BenchmarkedInstruction instr in benchInstructions)
            {
                Log.WriteLine(instr.GetStringOutput());
            }

        }

        private static void ComputeBenchmarkedInstructions()
        {
            float fastestInstructionTicks = 2.0f;
            // Get the fastest instruction
            double instructionWithMaxRunsPerSecond = benchInstructions.OrderByDescending(x => x.RunsPerSecond).FirstOrDefault().RunsPerSecond;

            foreach (BenchmarkedInstruction instr in benchInstructions)
            {
                instr.Ticks = (fastestInstructionTicks * instructionWithMaxRunsPerSecond) / instr.RunsPerSecond;
            }
        }
    }
}
