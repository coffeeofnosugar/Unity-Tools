using System;

namespace Tools.ExcelResolver.Editor
{
    public class TFloat : TType
    {
        public override string TypeName => "float";

        public override Type RealType => typeof(float);

        public override bool TryParseFrom(string s, out object o)
        {
            if (float.TryParse(s, out float f))
            {
                o = f;
                return true;
            }
            o = null;
            return false;
        }
    }
}