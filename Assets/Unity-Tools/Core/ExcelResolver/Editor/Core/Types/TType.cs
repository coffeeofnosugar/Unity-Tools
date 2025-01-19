using System;

namespace Tools.ExcelResolver.Editor
{
    internal abstract class TType
    {
        internal abstract string TypeName { get; }
        
        internal abstract bool String2TType(string typeText);
        
        internal abstract Type RealType { get; }

        internal abstract object TryParseFrom(string cellText);
    }
}