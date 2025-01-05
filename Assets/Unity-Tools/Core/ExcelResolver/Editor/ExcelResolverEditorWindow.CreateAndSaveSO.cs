using UnityEditor;
using UnityEngine;

namespace Tools.ExcelResolver.Editor
{
    public sealed partial class ExcelResolverEditorWindow
    {
        private void CreateSO(ClassCodeData classCodeData)
        {
            var so = ScriptableObject.CreateInstance(classCodeData.className + "SO");
            AssetDatabase.CreateAsset(so, $"{excelResolverConfig.SOPathRoot}/{classCodeData.className}SO.asset");
            AssetDatabase.SaveAssets();
        }
    }
}