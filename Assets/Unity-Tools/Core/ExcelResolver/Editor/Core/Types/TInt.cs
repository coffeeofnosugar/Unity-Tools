using System;

namespace Tools.ExcelResolver.Editor
{
    internal class TInt : TType
    {
        internal override string TypeName => "int";

        internal override Type RealType => typeof(int);

        internal override object TryParseFrom(string s)
        {
            return int.TryParse(s, out var result) ? result : null;
        }
    }
}