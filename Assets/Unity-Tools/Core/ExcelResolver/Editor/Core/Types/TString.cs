using System;

namespace Tools.ExcelResolver.Editor
{
    internal class TString : TType
    {
        internal override string TypeName => "string";

        internal override bool String2TType(string typeText)
        {
            return string.Equals(typeText, TypeName, StringComparison.OrdinalIgnoreCase);
        }

        internal override Type RealType => typeof(string);

        internal override object TryParseFrom(string cellText)
        {
            return cellText;
        }
    }
}