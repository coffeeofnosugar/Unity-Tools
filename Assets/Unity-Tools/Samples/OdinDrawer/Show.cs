#if UNITY_EDITOR && ODIN_INSPECTOR
using UnityEngine;

namespace Tools.OdinDrawer.Sample
{
    public class Show : MonoBehaviour
    {
        // public Test testInstance;
        public OdinStruct odinStructInstance;
        public OdinClass odinClassInstance;

        [ColorFoldoutGroup("group1", 1f, 0f, 0f)]
        public string top;
        [ColorFoldoutGroup("group1")]
        public string middle;
        [ColorFoldoutGroup("group1")]
        public string bottom;
        
        [ColorFoldoutGroup("group2", 0f, 1f, 0f)]
        public int first = 1;
        [ColorFoldoutGroup("group2")]
        public int second = 2;
        [ColorFoldoutGroup("group2")]
        public int third = 3;
    }
}
#endif