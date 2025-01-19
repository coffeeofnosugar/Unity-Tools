using System;
using System.Collections.Generic;

namespace Tools.ExcelResolver.Editor
{
    internal class TList : TType
    {
        internal override string TypeName => "list";
        internal override Type RealType => typeof(List<>);
        internal override object TryParseFrom(string s)
        {
            return null;
        }
    }
}