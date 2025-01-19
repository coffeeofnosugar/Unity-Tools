using System;

namespace Tools.ExcelResolver.Editor
{
    internal abstract class TType
    {
        internal abstract string TypeName { get; }
        
        internal abstract Type RealType { get; }

        internal abstract object TryParseFrom(string s);
    }
}