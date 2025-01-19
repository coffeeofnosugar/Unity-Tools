using System;
using UnityEngine;

namespace Tools.ExcelResolver.Editor
{
    internal class TVector2 : TType
    {
        internal override string FieldWriteFormat => "Vector2";

        internal override bool String2TType(string typeText)
        {
            return string.Equals(typeText, "vector2", StringComparison.OrdinalIgnoreCase);
        }

        internal override Type RealType => typeof(Vector2);

        internal override object TryParseFrom(string cellText)
        {
            cellText = cellText[1..^1];
            var split = cellText.Split(',');
            if (split.Length != 2)
            {
                return null;
            }
            else
            {
                return new Vector2(float.Parse(split[0]), float.Parse(split[1]));
            }
        }
    }
}