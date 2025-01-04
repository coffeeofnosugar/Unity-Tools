using System.Collections.Generic;

namespace Tools.Editor.CodeGenKit
{
    public interface ICodeScope : ICode
    {
        List<ICode> Codes { get; }
    }
}