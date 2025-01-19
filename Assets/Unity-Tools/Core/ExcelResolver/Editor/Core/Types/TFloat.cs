using System;

namespace Tools.ExcelResolver.Editor
{
    internal class TFloat : TType
    {
        internal override string FieldWriteFormat => "System.Single";

        internal override bool String2TType(string typeText)
        {
            return string.Equals(typeText, "float", StringComparison.OrdinalIgnoreCase);
        }

        internal override Type RealType => typeof(float);

        internal override object TryParseFrom(string cellText)
        {
            if (float.TryParse(cellText, out float result))
            {
                return result;
            }
            return null;
        }
    }
}