using System;
using System.Collections.Generic;

namespace Tools.ExcelResolver.Editor
{
    internal class TList : TType
    {
        internal override string FieldWriteFormat => "List<int>";
        
        internal override bool String2TType(string typeText)
        {
            var split = typeText.Split('|');
            if (split.Length != 2)
            {
                return false;
            }

            if (!string.Equals(split[0], "list", StringComparison.OrdinalIgnoreCase) || !string.Equals(split[1], "int", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        internal override Type RealType => typeof(List<int>);
        internal override object TryParseFrom(string cellText)
        {
            return null;
        }
    }
}