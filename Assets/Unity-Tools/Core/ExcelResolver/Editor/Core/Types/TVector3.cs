using System;
using UnityEngine;

namespace Tools.ExcelResolver.Editor
{
    internal class TVector3 : TType
    {
        internal override string TypeName => "vector3";

        internal override bool String2TType(string typeText)
        {
            return string.Equals(typeText, TypeName, StringComparison.OrdinalIgnoreCase);
        }

        internal override Type RealType => typeof(Vector3);

        internal override object TryParseFrom(string cellText)
        {
            if (!cellText.StartsWith("(") || !cellText.EndsWith(")"))
            {
                return null;
            }
            
            cellText = cellText[1..^1];
            var ss = cellText.Split(',');
            if (ss.Length != 3)
            {
                return null;
            }
            else
            {
                return new Vector3(float.Parse(ss[0]), float.Parse(ss[1]), float.Parse(ss[2]));
            }
        }
    }
}