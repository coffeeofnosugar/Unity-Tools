using System;
using UnityEngine;

namespace Tools.ExcelResolver.Editor
{
    internal class TVector3 : TType
    {
        internal override string TypeName => "vector3";

        internal override Type RealType => typeof(Vector3);

        internal override object TryParseFrom(string s)
        {
            if (!s.StartsWith("(") || !s.EndsWith(")"))
            {
                return null;
            }
            
            s = s[1..^1];
            var ss = s.Split(',');
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