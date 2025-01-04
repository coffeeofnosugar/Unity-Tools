namespace Tools.Editor.CodeGenKit
{
    public interface ICodeWriter
    {
        int IndentCount { get; set; }
        void WriteLine(string code = null);
    }
}