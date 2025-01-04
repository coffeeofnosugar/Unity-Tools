namespace Tools.Editor.CodeGenKit
{
    public sealed class CloseBraceCode : ICode
    {
        private readonly bool _semicolon;

        public CloseBraceCode(bool semicolon)
        {
            _semicolon = semicolon;
        }

        public void Gen(ICodeWriter writer)
        {
            writer.WriteLine("}" + (_semicolon ? ";" : string.Empty));
        }
    }
}