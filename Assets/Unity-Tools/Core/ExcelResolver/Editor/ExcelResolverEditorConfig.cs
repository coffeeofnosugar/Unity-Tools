using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.ExcelResolver.Editor
{
    [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
    public sealed class ExcelResolverEditorConfig : ScriptableObject
    {
        [ValueDropdown("@Tools.Editor.DirectoryUtil.GetFilePaths()")]
        public string ExcelPath;
        [FolderPath]
        public string JsonPath;
    }
}