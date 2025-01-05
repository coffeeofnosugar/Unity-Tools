using System;
using Tools.Editor.CodeGenKit;

namespace Tools.ExcelResolver.Editor
{
    internal class FieldData
    {
        public string name;
        public Type type;
        public string description;
        
    }
    
    public sealed partial class ExcelResolverEditorWindow
    {
        private int[] keyIndex;
        
        private void WriteTypeCode()
        {
            var code = new RootCode();
            code.Custom("// 代码使用工具生成，请勿随意修改");
            code.Using("System");
            code.Using("System.Collections.Generic");
            code.Using("System.Linq");

            code.NameSpace(excelResolverConfig.GenerateDataClassNameSpace, cope =>
            {
                cope.Custom("[Serializable]");
                // cope.Class($"")
            });
        }
    }
}