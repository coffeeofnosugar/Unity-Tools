#if ODIN_INSPECTOR
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Attributes.Sample
{
    // 当DictionaryValueDropdownAttribute修饰字典时，则PreviewField会修饰其键
    [ApplyToDictionaryKeys]
    [PreviewField]
    public class DictionaryPreviewFieldAttribute : Attribute { }

    // 当DictionaryValueDropdownAttribute修饰字典时，则ValueDropdown和DisableInPlayMode会修饰其值
    [ApplyToDictionaryValues]
    [ValueDropdown("@ApplyToDictionary.GetDropdownValues()")]
    [DisableInPlayMode]
    public class DictionaryValueDropdownDisableInPlayModeAttribute : Attribute { }

    /// <summary> 必须继承SerializedMonoBehaviour，这样字典才能显示在Inspector上 </summary>
    public class ApplyToDictionary : SerializedMonoBehaviour
    {
        [DictionaryPreviewField]
        [DictionaryValueDropdownDisableInPlayMode]
        public Dictionary<Sprite, string> SomeDictionary = new Dictionary<Sprite, string>();
    
        public static List<string> GetDropdownValues() => new List<string>
        {
            "Value 1",
            "Value 2",
            "Value 3",
        };
    }
}
#endif