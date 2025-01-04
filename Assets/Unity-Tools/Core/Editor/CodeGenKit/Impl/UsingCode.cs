namespace Tools.Editor.CodeGenKit
{
    public sealed class UsingCode : ICode
    {
        private readonly string _nameSpace;

        public UsingCode(string nameSpace)
        {
            _nameSpace = nameSpace;
        }

        public void Gen(ICodeWriter writer)
        {
            writer.WriteLine($"using {_nameSpace};");
        }
    }


    public static partial class CodeScopeExtensions
    {
        public static ICodeScope Using(this ICodeScope self, string nameSpace)
        {
            var code = new UsingCode(nameSpace);
            self.Codes.Add(code);
            return self;
        }
    }
}