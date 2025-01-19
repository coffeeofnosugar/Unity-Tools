using System;

namespace Tools.ExcelResolver.Editor
{
    internal class TInt : TType
    {
        internal override string FieldWriteFormat => "System.Int32";

        internal override bool String2TType(string typeText)
        {
            return string.Equals(typeText, "int", StringComparison.OrdinalIgnoreCase);
        }

        internal override Type RealType => typeof(int);

        internal override object TryParseFrom(string cellText)
        {
            return int.TryParse(cellText, out var result) ? result : null;
        }
    }
}