namespace Last_Known_Reality.Reality
{
    public class Hue
    {
        private static readonly float MAX_HUE = 360;
        private static readonly float BRIGHTEST = 60;
        private static readonly float DARKEST = 240;

        private float _value;

        public float Value
        {
            get
            {
                return _value;
            }

            set
            {
                _value = value % MAX_HUE;
                if (_value < 0)
                    _value = MAX_HUE + _value;
            }
        }

        public Hue()
        {

        }

        public Hue(float hue)
        {
            Value = hue;
        }

        public static implicit operator Hue(float value)
        {
            return new Hue(value);
        }

        // Addition/subtraction
        public static Hue operator ++(Hue hue)
        {
            hue.Value++;
            return hue;
        }

        public static Hue operator --(Hue hue)
        {
            hue.Value--;
            return hue;
        }

        public static Hue operator +(Hue hue1, Hue hue2)
        {
            return new Hue(hue1.Value + hue2.Value);
        }

        public static Hue operator +(Hue hue, float value)
        {
            return new Hue(hue.Value + value);
        }

        public static Hue operator -(Hue hue1, Hue hue2)
        {
            return new Hue(hue1.Value - hue2.Value);
        }

        public static Hue operator -(Hue hue, float value)
        {
            return new Hue(hue.Value - value);
        }

        // Multiplication/division
        public static Hue operator *(Hue hue1, Hue hue2)
        {
            return new Hue(hue1.Value * hue2.Value);
        }

        public static Hue operator *(Hue hue, float value)
        {
            return new Hue(hue.Value * value);
        }

        public static Hue operator /(Hue hue1, Hue hue2)
        {
            return new Hue(hue1.Value / hue2.Value);
        }

        public static Hue operator /(Hue hue, float value)
        {
            return new Hue(hue.Value / value);
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
