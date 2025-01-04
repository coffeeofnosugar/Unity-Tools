using System.Collections.Generic;

namespace Tools.Editor.CodeGenKit
{
    public abstract class CodeScope : ICodeScope
    {
        public bool Semicolon { get; set; }

        public virtual void Gen(ICodeWriter writer)
        {
            GenFirstLine(writer);

            new OpenBraceCode().Gen(writer);

            writer.IndentCount++;

            foreach (var code in Codes)
            {
                code.Gen(writer);
            }

            writer.IndentCount--;

            new CloseBraceCode(Semicolon).Gen(writer);
        }

        protected abstract void GenFirstLine(ICodeWriter writer);

        public List<ICode> Codes { get; } = new();
    }
}