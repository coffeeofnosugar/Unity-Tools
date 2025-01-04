namespace Tools.Editor.CodeGenKit
{
    public sealed class CustomCode : ICode
    {
        private readonly string _content;

        public CustomCode(string content)
        {
            _content = content;
        }

        public void Gen(ICodeWriter writer)
        {
            writer.WriteLine(_content);
        }
    }

    public static partial class CodeScopeExtensions
    {
        public static ICodeScope Custom(this ICodeScope self, string content)
        {
            self.Codes.Add(new CustomCode(content));
            return self;
        }
    }
}