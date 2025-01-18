using System;

namespace Tools.ExcelResolver.Editor
{
    internal class TString : TType
    {
        internal override string TypeName => "string";

        internal override Type RealType => typeof(string);

        internal override bool TryParseFrom(string s, out object o)
        {
            o = s;
            return true;
        }
    }
}