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
        public TType type;
        public string info;
        public string description;
        public string path;
        
        internal CodeMemberField GetCodeField()
        {
            CodeMemberField codeField = new CodeMemberField
            {
                Attributes = MemberAttributes.Public,
                Name = varName,
                Type = new CodeTypeReference(type.FieldWriteFormat),
                Comments =
                {
                    new CodeCommentStatement("<summary>", true),
                    new CodeCommentStatement(info, true),
                },
            };
            if (!string.IsNullOrEmpty(description)) 
                codeField.Comments.Add(new CodeCommentStatement($"<c>{description}</c>", true));
            codeField.Comments.Add(new CodeCommentStatement("</summary>", true));
            
            return codeField;
        }
        
        internal void Dispose()
        {
            varName = null;
            type = null;
            info = null;
            description = null;
            path = null;
        }
    }
}