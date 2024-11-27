#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace Tools.Editor
{
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
#endif