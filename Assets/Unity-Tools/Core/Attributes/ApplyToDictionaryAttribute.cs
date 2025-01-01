#if ODIN_INSPECTOR
using System;
using System.Diagnostics;

namespace Tools
{
    /// <summary> 可以自定义字典键的属性 </summary>
    /// <example><see cref="Tools.Attributes.Samples.ApplyToDictionary.SomeDictionary"/></example>
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Class)]
    public class ApplyToDictionaryKeysAttribute : Attribute { }

    /// <summary> 可以自定义字典值的属性 </summary>
    /// <example><see cref="Tools.Attributes.Samples.ApplyToDictionary.SomeDictionary"/></example>
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Class)]
    public class ApplyToDictionaryValuesAttribute : Attribute { }
}
#endif