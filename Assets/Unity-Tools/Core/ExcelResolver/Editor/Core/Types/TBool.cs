using System;

namespace Tools.ExcelResolver.Editor
{
    internal class TBool : TType
    {
        internal override string TypeName => "bool";

        internal override bool String2TType(string typeText)
        {
            return string.Equals(typeText, TypeName, StringComparison.OrdinalIgnoreCase);
        }

        internal override Type RealType => typeof(bool);

        internal override object TryParseFrom(string cellText)
        {
            if (bool.TryParse(cellText, out var result))
            {
                return result;
            }
            return null;
        }
    }
}