using System;

namespace Tools.ExcelResolver.Editor
{
    public class TBool : TType
    {
        public override string TypeName => "bool";

        public override Type RealType => typeof(bool);

        public override bool TryParseFrom(string s, out object o)
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