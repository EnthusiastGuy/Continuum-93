using System;

namespace Continuum93.Emulator.Audio.XSound.Utilities
{
    public class XRepeatController
    {
        private int repeatTime;
        private readonly int repeatLimit;

        public XRepeatController(XSoundParams parameters)
        {
            if (parameters.RepeatSpeed == 0.0f)
            {
                repeatLimit = 0;
            }
            else
            {
                repeatLimit = (int)(Math.Pow(1.0f - parameters.RepeatSpeed, 2.0f) * 20000 + 32);
            }
            repeatTime = 0;
        }

        public bool CheckRepeat()
        {
            if (repeatLimit == 0)
            {
                return false; // No repeat
            }

            repeatTime++;
            if (repeatTime >= repeatLimit)
            {
                repeatTime = 0;
                return true; // Time to repeat
            }

            return false;
        }

        public void Reset()
        {
            repeatTime = 0;
        }
    }

}
