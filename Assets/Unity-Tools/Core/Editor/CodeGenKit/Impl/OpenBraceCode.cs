namespace Tools.Editor.CodeGenKit
{
    public sealed class OpenBraceCode : ICode
    {
        public void Gen(ICodeWriter writer)
        {
            writer.WriteLine("{");
        }
    }
}