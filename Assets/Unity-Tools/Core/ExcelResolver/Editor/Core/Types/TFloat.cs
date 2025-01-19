using System;

namespace Tools.ExcelResolver.Editor
{
    internal class TFloat : TType
    {
        internal override string TypeName => "float";

        internal override Type RealType => typeof(float);

        internal override object TryParseFrom(string s)
        {
            if (float.TryParse(s, out float result))
            {
                return result;
            }
            return null;
        }
    }
}