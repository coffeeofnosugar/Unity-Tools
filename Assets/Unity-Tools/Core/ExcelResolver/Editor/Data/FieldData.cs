using System;
using System.CodeDom;

namespace Tools.ExcelResolver.Editor
{
    internal class FieldData
    {
        public int colIndex;
        public bool isKey;
        
        public string varName;
        public string typeString;
        public Type type;
        public string info;
        public string description;
        public string path;
        
        public void Dispose()
        {
            varName = null;
            type = null;
            info = null;
            description = null;
            path = null;
        }
    }

    internal static class FieldDataExtension
    {
        internal static CodeMemberField GetCodeField(this FieldData field)
        {
            CodeMemberField codeField = new CodeMemberField
            {
                Attributes = MemberAttributes.Public,
                Name = field.varName,
                Type = new CodeTypeReference(field.type),
                CustomAttributes = new CodeAttributeDeclarationCollection()
                {
                    new CodeAttributeDeclaration("SerializeField")
                },
            };
            return codeField;
        }
    }
}