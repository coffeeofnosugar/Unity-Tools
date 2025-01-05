using System;

namespace Tools.ExcelResolver.Editor
{
    public class TInt : TType
    {
        public override string TypeName => "int";

        public override Type RealType => typeof(int);

        public override bool TryParseFrom(string s, out object o)
        {
            var b = int.TryParse(s, out var i);
            o = i;
            return b;
        }
    }
}