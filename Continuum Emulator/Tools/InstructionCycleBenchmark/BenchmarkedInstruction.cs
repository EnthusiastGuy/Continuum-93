namespace Continuum93.Tools.InstructionCycleBenchmark
{
    public class BenchmarkedInstruction
    {
        public string InstructionName;
        public double RunsPerSecond;
        public double Ticks;

        public string GetStringOutput()
        {
            return $"{InstructionName}\t{Ticks}\t{RunsPerSecond}";
        }
    }
}
