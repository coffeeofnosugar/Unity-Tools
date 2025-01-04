using Sirenix.OdinInspector;
using Tools.Editor;
using UnityEngine;

namespace Tools.ExcelResolver.Editor
{
    [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
    [CreateAssetMenu(fileName = "ExcelResolverEditorConfig", menuName = "ExcelResolver/ExcelResolverEditorConfig")]
    public sealed class ExcelResolverEditorConfig : ScriptableObject
    {
        [LabelText("Excel路径"), FolderPath]
        public string ExcelPathRoot = "Assets/ExcelResolver/Excel";
        [LabelText("Json路径"), ValueDropdown("@Tools.Editor.DirectoryUtil.GetFilePaths()")]
        public string JsonPathRoot = "Assets/ExcelResolver/Json";
        [LabelText("代码路径"), ValueDropdown("@Tools.Editor.DirectoryUtil.GetFilePaths()")]
        public string CodePathRoot = "Assets/Scripts/Generator/Excel";
        [LabelText("生成代码命名空间")]
        public string GenerateDataClassNameSpace = "Tools.ExcelResolver";
        
        [HideInInspector] public bool isInit;
        
        public void MakeSureDirectory()
        {
            DirectoryUtil.MakeSureDirectory(ExcelPathRoot);
            DirectoryUtil.MakeSureDirectory(JsonPathRoot);
            DirectoryUtil.MakeSureDirectory(CodePathRoot);
        }
    }
}