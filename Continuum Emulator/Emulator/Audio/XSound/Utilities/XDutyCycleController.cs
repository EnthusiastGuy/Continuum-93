namespace Continuum93.Emulator.Audio.XSound.Generators
{
    public class XDutyCycleController
    {
        private readonly XSoundParams parameters;
        private float dutyCycle;
        private readonly float dutyCycleSlide;

        public XDutyCycleController(XSoundParams parameters)
        {
            this.parameters = parameters;
            dutyCycle = parameters.DutyCycle;
            dutyCycleSlide = parameters.DutyCycleRamp;
        }

        public float GetDutyCycle()
        {
            dutyCycle += dutyCycleSlide;
            if (dutyCycle < 0) dutyCycle = 0;
            if (dutyCycle > 0.5f) dutyCycle = 0.5f;
            return dutyCycle;
        }
    }

}
