using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tools.InterfaceReference
{
    /// <summary>
    /// 作用：可以拖拽获取任何实现了TInterface接口的对象
    /// 实际上，可以直接使用Odin插件的SerializedMonoBehaviour，这样可以直接序列化接口
    /// 跟着网上学习这种方法只是为了扩展知识
    /// </summary>
    /// <typeparam name="TInterface"> 需要引用的接口 </typeparam>
    /// <typeparam name="TObject"> 限制实例化条件，只能引用ScriptableObject或MonoBehaviour </typeparam>
    [Serializable]
    public class InterfaceReference<TInterface, TObject> where TObject : Object where TInterface : class
    {
        [SerializeField] private TObject underlyingValue;

        public TInterface Value
        {
            get => underlyingValue switch
            {
                null => null,
                TInterface @interface => @interface,
                _ => throw new InvalidOperationException($"{underlyingValue} 需要实现 {nameof(TInterface)} 接口")
            };
            set => underlyingValue = value switch
            {
                null => null,
                TObject newValue => newValue,
                _ => throw new ArgumentNullException($"{value} 需要是 {typeof(TObject)} 类型")
            };
        }

        public TObject UnderlyingValue
        {
            get => underlyingValue;
            set => underlyingValue = value;
        }
        
        public InterfaceReference(){ }
        public InterfaceReference(TObject target) => underlyingValue = target;
        public InterfaceReference(TInterface @interface) => UnderlyingValue = @interface as TObject;
    }
    
    [Serializable]
    public class InterfaceReference<TInterface> : InterfaceReference<TInterface, Object> where TInterface : class { }
}