using System;

namespace Tools.ExcelResolver.Editor
{
    internal class TInt : TType
    {
        internal override string TypeName => "int";

        internal override bool String2TType(string typeText)
        {
            return string.Equals(typeText, TypeName, StringComparison.OrdinalIgnoreCase);
        }

        internal override Type RealType => typeof(int);

        internal override object TryParseFrom(string cellText)
        {
            return int.TryParse(cellText, out var result) ? result : null;
        }
    }
}