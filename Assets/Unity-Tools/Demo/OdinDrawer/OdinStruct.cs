#if UNITY_EDITOR && ODIN_INSPECTOR
using System;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Tools.OdinDrawer.Sample
{
    [Serializable]
    public struct OdinStruct
    {
        public string text;
        public int number;
        public Vector3 location;
    }


    /// <summary>
    /// 结构体是值类型，<see cref="IPropertyValueEntry{TValue}.SmartValue"/>类似ref，引用了该结构体，所以可以直接改变结构体的值
    /// </summary>
    public class OdinStructDrawer : OdinValueDrawer<OdinStruct>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var text = ValueEntry.SmartValue.text;
            var number = ValueEntry.SmartValue.number;
            var location = ValueEntry.SmartValue.location;

            Rect rect = EditorGUILayout.GetControlRect();
            if (label != null)
                EditorGUI.PrefixLabel(rect, label);

            SirenixEditorGUI.BeginShakeableGroup();
            text = SirenixEditorFields.TextField("Text", text);
            if (text != null)
            {
                var originLength = text.Length;
                text = new string(text.Where(char.IsDigit).ToArray());  // 过滤非数字字符
                if (originLength != text.Length)
                    SirenixEditorGUI.StartShakingGroup();       // 如果输入了非数字字符，则闪烁
            }
            SirenixEditorGUI.EndShakeableGroup();
            
            number = SirenixEditorFields.IntField("Number", number);
            location = SirenixEditorFields.Vector3Field("Location", location);

            
            this.ValueEntry.SmartValue = new OdinStruct()
            {
                text = text,
                number = number,
                location = location
            };
        }
    }
}
#endif