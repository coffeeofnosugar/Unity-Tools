using System;
using UnityEngine;

namespace Tools.ExcelResolver.Editor
{
    internal class TVector3 : TType
    {
        internal override string FieldWriteFormat => "Vector3";

        internal override bool String2TType(string typeText)
        {
            return string.Equals(typeText, "vector3", StringComparison.OrdinalIgnoreCase);
        }

        internal override Type RealType => typeof(Vector3);

        internal override object TryParseFrom(string cellText)
        {
            if (!cellText.StartsWith("(") || !cellText.EndsWith(")"))
            {
                return null;
            }
            
            cellText = cellText[1..^1];
            var split = cellText.Split(',');
            if (split.Length != 3)
            {
                return null;
            }
            else
            {
                return new Vector3(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]));
            }
        }
    }
}