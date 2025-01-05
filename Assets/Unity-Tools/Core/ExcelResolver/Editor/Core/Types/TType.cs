using System;

namespace Tools.ExcelResolver.Editor
{
    public abstract class TType
    {
        public abstract string TypeName { get; }
        
        public abstract Type RealType { get; }

        public abstract bool TryParseFrom(string s, out object o);
    }
}