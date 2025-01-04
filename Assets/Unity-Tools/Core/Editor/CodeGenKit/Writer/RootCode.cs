using System.Collections.Generic;

namespace Tools.Editor.CodeGenKit
{
    public sealed class RootCode : ICodeScope
    {
        public void Gen(ICodeWriter writer)
        {
            foreach (var code in Codes)
            {
                code.Gen(writer);
            }
        }

        public List<ICode> Codes { get; } = new();
    }
}