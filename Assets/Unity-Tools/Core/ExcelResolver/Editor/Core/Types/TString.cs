using System;

namespace Tools.ExcelResolver.Editor
{
    internal class TString : TType
    {
        internal override string TypeName => "string";

        internal override Type RealType => typeof(string);

        internal override object TryParseFrom(string s)
        {
            return s;
        }
    }
}