namespace Continuum93.Emulator.Audio.XSound.Generators
{
    public class XEnvelopeController
    {
        private readonly XSoundParams parameters;
        private readonly float attackTime;
        private readonly float sustainTime;
        private readonly float decayTime;
        private readonly float sustainPunch;
        private float currentTime;
        private bool isPlaying;

        public XEnvelopeController(XSoundParams parameters)
        {
            this.parameters = parameters;
            attackTime = parameters.EnvelopeAttack * parameters.SampleRate;
            sustainTime = parameters.EnvelopeSustain * parameters.SampleRate;
            decayTime = parameters.EnvelopeDecay * parameters.SampleRate;
            sustainPunch = parameters.EnvelopePunch;
            currentTime = 0;
            isPlaying = true;
        }

        public float GetEnvelope()
        {
            if (!isPlaying)
            {
                return 0.0f;
            }

            currentTime++;

            // Attack stage
            if (currentTime < attackTime)
            {
                return currentTime / attackTime;
            }

            // Sustain stage
            if (currentTime < attackTime + sustainTime)
            {
                float punchEffect = 1.0f + (1.0f - ((currentTime - attackTime) / sustainTime)) * 2.0f * sustainPunch;
                return punchEffect;
            }

            // Decay stage
            if (currentTime < attackTime + sustainTime + decayTime)
            {
                return 1.0f - ((currentTime - attackTime - sustainTime) / decayTime);
            }

            // If past decay, stop playing
            isPlaying = false;
            return 0.0f;
        }

        public void Reset()
        {
            currentTime = 0;
            isPlaying = true;
        }
    }

}
