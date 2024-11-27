using System;

namespace Tools
{
    public class KnobAttribute : Attribute
    {
        public float Min;
        public float Max;

        public KnobAttribute(float min, float max) => (this.Min, this.Max) = (min, max);
    }
}