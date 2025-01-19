using Sirenix.OdinInspector;
using Tools.Editor;
using UnityEngine;

namespace Tools.ExcelResolver.Editor
{
    [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
    public sealed class ExcelResolverEditorConfig : ScriptableObject
    {
        [LabelText("Excel路径"), FolderPath]
        public string ExcelPathRoot = "Assets/ExcelResolver/Excel";
        [LabelText("C#代码路径"), ValueDropdown("@Tools.Editor.DirectoryUtil.GetFilePaths()")]
        public string CodePathRoot = "Assets/Scripts/Generator/Excel";
        [LabelText("SO存放路径"), ValueDropdown("@Tools.Editor.DirectoryUtil.GetFilePaths()")]
        public string SOPathRoot = "Assets/ScriptableObject/Excel";
        [LabelText("生成代码命名空间")]
        public string GenerateDataClassNameSpace = "Tools.ExcelResolver";
        
        public void MakeSureDirectory()
        {
            DirectoryUtil.MakeSureDirectory(ExcelPathRoot);
            DirectoryUtil.MakeSureDirectory(SOPathRoot);
            DirectoryUtil.MakeSureDirectory(CodePathRoot);
        }
    }
}