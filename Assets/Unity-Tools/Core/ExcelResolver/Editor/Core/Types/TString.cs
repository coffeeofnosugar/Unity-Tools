using System;

namespace Tools.ExcelResolver.Editor
{
    public class TString : TType
    {
        public override string TypeName => "string";

        public override Type RealType => typeof(string);

        public override bool TryParseFrom(string s, out object o)
        {
            o = s;
            return true;
        }
    }
}