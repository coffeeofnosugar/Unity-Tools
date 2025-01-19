using System;

namespace Tools.ExcelResolver.Editor
{
    internal class TBool : TType
    {
        internal override string TypeName => "bool";

        internal override Type RealType => typeof(bool);

        internal override object TryParseFrom(string s)
        {
            if (bool.TryParse(s, out var result))
            {
                return result;
            }
            return null;
        }
    }
}