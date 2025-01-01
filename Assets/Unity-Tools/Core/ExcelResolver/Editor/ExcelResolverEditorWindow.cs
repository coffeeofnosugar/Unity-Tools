using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Tools.ExcelResolver.Editor
{
    public class ExcelResolverEditorWindow : OdinEditorWindow
    {
        public ExcelResolverEditorConfig config;
        
        [MenuItem("Tools/ExcelResolver")]
        private static void OpenWindow()
        {
            GetWindow<ExcelResolverEditorWindow>().Show();
        }

        [OnInspectorGUI]
        private void DrawGenerateButton()
        {
            GUILayout.FlexibleSpace(); // 把空白区域推向上方
            if (GUILayout.Button("Generate", GUILayout.Height(50)))
            {
                Generate();
            }
        }

        private void Generate()
        {
            // 按钮逻辑
        }
    }
}