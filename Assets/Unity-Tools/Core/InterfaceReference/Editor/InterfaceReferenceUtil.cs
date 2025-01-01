using UnityEditor;
using UnityEngine;

namespace Tools.InterfaceReference.Editor
{
    /// <summary> 在物体选择框的右边显示接口名称 </summary>
    public static class InterfaceReferenceUtil
    {
        private static GUIStyle labelStyle;

        /// 在Drawer中直接调用此方法可以在右边显示接口名称
        public static void OnGUI(Rect position, SerializedProperty property, GUIContent label, InterfaceArgs args)
        {
            InitializeStyleIfNeeded();

            var controlID = GUIUtility.GetControlID(FocusType.Passive) - 1;
            var isHovering = position.Contains(Event.current.mousePosition);
            var displayString = property.objectReferenceValue == null || isHovering
                ? $"({args.InterfaceType.Name})"
                : "*";
            DrawInterfaceNameLabel(position, displayString, controlID);
        }

        /// 初始化样式
        static void InitializeStyleIfNeeded()
        {
            if (labelStyle != null) return;

            var style = new GUIStyle(EditorStyles.label)
            {
                font = EditorStyles.objectField.font,               // Unity系统字体
                fontSize = EditorStyles.objectField.fontSize,       // Unity系统字体大小
                fontStyle = EditorStyles.objectField.fontStyle,     // Unity系统字体样式
                alignment = TextAnchor.MiddleRight,                 // 对齐方式
                padding = new RectOffset(0, 2, 0, 0)                // 内边距
            };
            labelStyle = style;
        }

        /// 设置
        static void DrawInterfaceNameLabel(Rect position, string displayString, int controlID)
        {
            if (Event.current.type == EventType.Repaint)    // 绘制事件时才跟新样式
            {
                const int additionalLeftWidth = 3;
                const int verticalIndent = 1;

                GUIContent content = EditorGUIUtility.TrTextContent(displayString);     // 翻译文本内容
                Vector2 size = labelStyle.CalcSize(content);                            // 测量文本大小
                Rect labelPos = position;                       // 获取位置
                
                labelPos.width = size.x + additionalLeftWidth;  // 调整位置
                labelPos.x += position.width - labelPos.width - 18;
                labelPos.height -= verticalIndent * 2;
                labelPos.y += verticalIndent;
                labelStyle.Draw(labelPos, content, controlID, DragAndDrop.activeControlID  == controlID, false);    // 绘制文本
            }
        }
    }
}