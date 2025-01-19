using System;
using System.Collections.Generic;

namespace Tools.ExcelResolver.Editor
{
    internal class TList : TType
    {
        internal override string TypeName => "list";
        internal override bool String2TType(string typeText)
        {
            return false;
        }

        internal override Type RealType => typeof(List<>);
        internal override object TryParseFrom(string cellText)
        {
            return null;
        }
    }
}