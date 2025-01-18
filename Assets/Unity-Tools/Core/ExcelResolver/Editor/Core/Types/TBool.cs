using System;

namespace Tools.ExcelResolver.Editor
{
    internal class TBool : TType
    {
        internal override string TypeName => "bool";

        internal override Type RealType => typeof(bool);

        internal override bool TryParseFrom(string s, out object o)
        {
            if (bool.TryParse(s, out var b))
            {
                o = b;
                return true;
            }
            o = null;
            return false;
        }
    }
}