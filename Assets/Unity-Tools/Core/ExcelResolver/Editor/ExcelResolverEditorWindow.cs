using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using Tools.Editor;
using UnityEditor;
using UnityEngine;

namespace Tools.ExcelResolver.Editor
{
    public sealed partial class ExcelResolverEditorWindow : OdinEditorWindow
    {
        [SerializeField] private ExcelResolverEditorConfig excelResolverConfig;
        
        [MenuItem("\u272dExcelResolver\u272d/ExcelResolverEditorWindow")]
        private static void OpenWindow()
        {
            var window = GetWindow<ExcelResolverEditorWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
            window.titleContent = new GUIContent("ExcelResolverEditorWindow");
        }

        protected override void Initialize()
        {
            if (excelResolverConfig == null)
            {
                string[] assetGuids = AssetDatabase.FindAssets($"ExcelResolverEditorConfig t:ExcelResolverEditorConfig");
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[0]);
                excelResolverConfig = AssetDatabase.LoadAssetAtPath<ExcelResolverEditorConfig>(assetPath);
            }
        }

        [FoldoutGroup("Hide Setting")]
        [Button(SdfIconType.ExclamationDiamond, "删除所有生成的代码和SO"), GUIColor(1f, 0f, 0f)]
        public void DeleteAllScriptsAndSO()
        {
            if (EditorUtility.DisplayDialog("警告", "确定要删除所有生成的代码和SO吗？", "确定", "取消"))
            {
                DirectoryUtil.DeleteDirectory(excelResolverConfig.CodePathRoot);
                DirectoryUtil.DeleteDirectory(excelResolverConfig.SOPathRoot);
                AssetDatabase.Refresh();
            }
        }

        [OnInspectorGUI]
        private void DrawGenerateButton()
        {
            GUILayout.FlexibleSpace(); // 把空白区域推向上方
        }


        [Button(ButtonSizes.Gigantic)]
        [ButtonGroup("Generate")]
        private void GenerateCode() => ReadExcel();
        
        [ButtonGroup("Generate")]
        private void GenerateSO() => WriteSOData();
    }
}