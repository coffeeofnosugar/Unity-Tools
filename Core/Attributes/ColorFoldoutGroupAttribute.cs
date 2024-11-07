using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace Tools
{
    public class ColorFoldoutGroupAttribute : PropertyGroupAttribute
    {
        public float R, G, B, A;
        public ColorFoldoutGroupAttribute(string groupId) : base(groupId) { }

        public ColorFoldoutGroupAttribute(string groupId, float r, float g, float b, float a = 1f) : base(groupId)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        /// <summary>
        /// 如果没有属性没有输入颜色，Odin会忽略，不过也可以使用该方法设置值
        /// </summary>
        protected override void CombineValuesWith(PropertyGroupAttribute other)
        {
            var otherAttr = (ColorFoldoutGroupAttribute)other;
            
            this.R = Mathf.Max(otherAttr.R, this.R);
            this.G = Mathf.Max(otherAttr.G, this.G);
            this.B = Mathf.Max(otherAttr.B, this.B);
            this.A = Mathf.Max(otherAttr.A, this.A);
        }
    }

    public class ColorFoldoutGroupAttributeDrawer : OdinGroupDrawer<ColorFoldoutGroupAttribute>
    {
        // 使用简单的bool值控制展开会导致，每次编译的时候都会重置为false
        // public bool isOpen;

        private LocalPersistentContext<bool> isExpanded;

        protected override void Initialize()
        {
            isExpanded = this.GetPersistentValue("ColorFoldoutGroupAttributeDrawer.isExpanded",
                GeneralDrawerConfig.Instance.ExpandFoldoutByDefault);
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            GUIHelper.PushColor(new Color(this.Attribute.R, this.Attribute.G, this.Attribute.B, this.Attribute.A));
            SirenixEditorGUI.BeginBox();
            SirenixEditorGUI.BeginBoxHeader();
            GUIHelper.PopColor();
            
            isExpanded.Value = SirenixEditorGUI.Foldout(isExpanded.Value, label);
            SirenixEditorGUI.EndBoxHeader();
            if (SirenixEditorGUI.BeginFadeGroup(this, isExpanded.Value))
            {
                foreach (var c in this.Property.Children)
                {
                    c.Draw();
                }
            }
            SirenixEditorGUI.EndFadeGroup();
            SirenixEditorGUI.EndBox();
        }
    }
}