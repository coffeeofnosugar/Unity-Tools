using System;
using UnityEngine;

namespace Tools.ExcelResolver.Editor
{
    internal class TVector2 : TType
    {
        internal override string TypeName => "vector2";

        internal override bool String2TType(string typeText)
        {
            return string.Equals(typeText, TypeName, StringComparison.OrdinalIgnoreCase);
        }

        internal override Type RealType => typeof(Vector2);

        internal override object TryParseFrom(string cellText)
        {
            cellText = cellText[1..^1];
            var ss = cellText.Split(',');
            if (ss.Length != 2)
            {
                return null;
            }
            else
            {
                return new Vector2(float.Parse(ss[0]), float.Parse(ss[1]));
            }
        }
    }
}