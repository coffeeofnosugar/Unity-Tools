using Tools.Editor.CodeGenKit;

namespace Tools.ExcelResolver.Editor
{
    public sealed partial class ExcelResolverEditorWindow
    {
        private enum TableType
        {
            SingleKeyTable,         // 单主键表
            UnionMultiKeyTable,     // 多主键表（联合索引）
            MultiKeyTable,          // 多主键表（独立索引）
            ColumnTable,            // 纵表
        }
        
        
        private void WriteTypeCode()
        {
            var code = new RootCode();
            code.Custom("//代码使用工具生成，请勿随意修改");
            code.Using("System");
            code.Using("System.Collections.Generic");
            code.Using("System.Linq");

            code.NameSpace(config.GenerateDataClassNameSpace, cope =>
            {
                cope.Custom("[Serializable]");
                // cope.Class($"")
            });
        }
    }
}