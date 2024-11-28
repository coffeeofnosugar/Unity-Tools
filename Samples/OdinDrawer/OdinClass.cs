#if UNITY_EDITOR && ODIN_INSPECTOR
using System;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Tools.OdinDrawer.Sample
{
    [Serializable]
    public class OdinClass
    {
        public string text;
        public int number;
        public Vector3 location;
    }

    /// <summary>
    /// <b>事实上，如果你想要控制类的显示方式，最好使用Odin属性，而不是Drawer</b>
    /// <para></para>
    /// class是引用类型
    /// 使用`this.ValueEntry.SmartValue.text = odinText;`直接改变值确实能生效。
    /// 但会有一些问题：
    /// 1. 改变值后重启unity，OdinClass所有的值都会丢失。
    /// 2. 同时选中多个物体，无法同时改变所有的属性
    /// <para></para>
    /// 使用`this.ValueEntry.SmartValue = new OdinClass() {}`确实能解决以上两个问题，但会产生大量的GC，并且会要求a你一直保存场景
    /// </summary>
    public class OdinClassDrawer : OdinValueDrawer<OdinClass>
    {
        /* 错误方法
        protected override void DrawPropertyLayout(GUIContent label)
        {
            string text = this.ValueEntry.SmartValue.text;
            int number = this.ValueEntry.SmartValue.number;
            Vector3 location = this.ValueEntry.SmartValue.location;
            
            Rect rect = EditorGUILayout.GetControlRect();   // 初始化rect，获取一个新的rect空间
            if (label != null)
                rect = EditorGUI.PrefixLabel(rect, label);      // 用刚获取的空间显示实例化的名称
            rect = EditorGUILayout.GetControlRect();            // 重新获取一个新的rect空间
            GUIHelper.PushLabelWidth(75);                   // 设置显示名称的宽度
            text = SirenixEditorFields.TextField(rect.Split(0, 2), "Text", text);       // 依次展示各个属性
            number = SirenixEditorFields.IntField(rect.Split(1, 2), "Number", number);  // 将前两个属性放在一行
            location = SirenixEditorFields.Vector3Field("Location", location);          // 将第三个属性放在新的一行（没有空间时会自动换行）
            GUIHelper.PopLabelWidth();

            // this.ValueEntry.SmartValue.text = text;
            // this.ValueEntry.SmartValue.number = number;
            // this.ValueEntry.SmartValue.location = location;
            this.ValueEntry.SmartValue = new OdinClass()
            {
                text = text,
                number = number,
                location = location
            };
        }
        */
        
        // 正确方法：
        private InspectorProperty text;
        private InspectorProperty number;
        private InspectorProperty location;
        
        /// <summary> 每次编译时会执行一此 </summary>
        protected override void Initialize()
        {
            text = this.Property.Children["text"];
            number = this.Property.Children["number"];
            location = this.Property.Children["location"];
        }
        
        protected override void DrawPropertyLayout(GUIContent label)
        {
            Rect rect = EditorGUILayout.GetControlRect();
            if (label != null) rect = EditorGUI.PrefixLabel(rect, label);
            rect = EditorGUILayout.GetControlRect();
            GUIHelper.PushLabelWidth(75);
            
            // 使用SirenixEditorFields定制渲染规则
            text.ValueEntry.WeakSmartValue = SirenixEditorFields.TextField(rect.Split(0, 2), "Text", (string)text.ValueEntry.WeakSmartValue);
            number.ValueEntry.WeakSmartValue = SirenixEditorFields.IntField(rect.Split(1, 2), "Number", (int)number.ValueEntry.WeakSmartValue);
            location.Draw();
            
            // 如果没有特殊的渲染需求，可以直接使用Draw()方法
            // text.Draw();
            // number.Draw();
            // location.Draw();
            GUIHelper.PopLabelWidth();
            // 结束，无需赋值，因为是引用类型
        }
    }
}
#endif