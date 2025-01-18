using System;

namespace Tools.ExcelResolver.Editor
{
    internal class TInt : TType
    {
        internal override string TypeName => "int";

        internal override Type RealType => typeof(int);

        internal override bool TryParseFrom(string s, out object o)
        {
            var b = int.TryParse(s, out var i);
            o = i;
            return b;
        }
    }
}