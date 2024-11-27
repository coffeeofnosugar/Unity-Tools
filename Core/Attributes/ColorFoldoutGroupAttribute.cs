#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
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

}
#endif