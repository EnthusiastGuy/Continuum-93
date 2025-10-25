namespace Continuum93.Emulator.Audio.Samples
{
    public class InputParams
    {
        public bool OldParams { get; set; }
        public int Wave_Type { get; set; }
        public double P_Env_Attack { get; set; }
        public double P_Env_Sustain { get; set; }
        public double P_Env_Punch { get; set; }
        public double P_Env_Decay { get; set; }
        public double P_Base_Freq { get; set; }
        public double P_Freq_Limit { get; set; }
        public double P_Freq_Ramp { get; set; }
        public double P_Freq_Dramp { get; set; }
        public double P_Vib_Strength { get; set; }
        public double P_Vib_Speed { get; set; }
        public double P_Arp_Mod { get; set; }
        public double P_Arp_Speed { get; set; }
        public double P_Duty { get; set; }
        public double P_Duty_Ramp { get; set; }
        public double P_Repeat_Speed { get; set; }
        public double P_Pha_Offset { get; set; }
        public double P_Pha_Ramp { get; set; }
        public double P_Lpf_Freq { get; set; }
        public double P_Lpf_Ramp { get; set; }
        public double P_Lpf_Resonance { get; set; }
        public double P_Hpf_Freq { get; set; }
        public double P_Hpf_Ramp { get; set; }
        public double Sound_Vol { get; set; }
        public int Sample_Rate { get; set; }
        public int Sample_Size { get; set; }
    }
}
