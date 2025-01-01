using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.ExcelResolver.Editor
{
    [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
    public class ExcelResolverEditorConfig : ScriptableObject
    {
        public string ExcelPath;
        public string JsonPath;
    }
}