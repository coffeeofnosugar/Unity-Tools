using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
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

        // [OnInspectorGUI]
        // private void DrawGenerateButton()
        // {
        //     GUILayout.FlexibleSpace(); // 把空白区域推向上方
        //     if (GUILayout.Button("Generate", GUILayout.Height(50)))
        //     {
        //         Generate();
        //     }
        // }

        [Button(ButtonSizes.Gigantic)]
        private void Generate()
        {
            ReadExcel();
        }
    }
}