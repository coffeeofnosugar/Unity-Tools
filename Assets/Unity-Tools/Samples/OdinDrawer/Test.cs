#if UNITY_EDITOR && ODIN_INSPECTOR
using System;
using UnityEditor;
using UnityEngine;

namespace Tools.OdinDrawer.Sample
{
    [Serializable]
    public class Test
    {
        public string text;
        public int number;
        public Vector3 location;
    }


    [CustomPropertyDrawer(typeof(Test))]
    public class TestDrawer : PropertyDrawer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label">实例化对象的名称</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            
            // Debug.Log(position);
        }
    }
}
#endif