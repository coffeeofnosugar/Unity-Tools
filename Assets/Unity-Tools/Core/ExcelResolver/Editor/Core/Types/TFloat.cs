using System;

namespace Tools.ExcelResolver.Editor
{
    internal class TFloat : TType
    {
        internal override string TypeName => "float";

        internal override Type RealType => typeof(float);

        internal override bool TryParseFrom(string s, out object o)
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