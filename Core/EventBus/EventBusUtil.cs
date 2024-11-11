using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Tools.EventBus.Editor
{
    /// <summary>
    /// 用于处理 Unity 应用中的事件总线和事件类型
    /// 这种处理方式能很好地解决：在开启了不加载Domain，且忘记使用代码注销事件的情况下，再次进入PlayMode时，事件会触发两次
    /// </summary>
    public static class EventBusUtil
    {
        public static IReadOnlyList<Type> EventTypes { get; set; }
        public static IReadOnlyList<Type> EventBusTypes { get; set; }
        
#if UNITY_EDITOR

        private const string name = "Tools/EventBusUtil/Switch";
        private static bool enable { get => EditorPrefs.GetBool(name); set => EditorPrefs.SetBool(name, value); }
        [MenuItem(name, false)] static void Switch() { enable = !enable; Menu.SetChecked(name, enable); }
        
        
        

        public static PlayModeStateChange PlayModeState { get; set; }
        
        [InitializeOnLoadMethod]    // 该属性表示该方法在编辑器启动时自动调用
        public static void InitializeEditor()
        {
            if (!enable) return;
            
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }
        
        /// <summary> 当退出编辑器模式时，清除所有事件总线 </summary>
        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (!enable) return;

            PlayModeState = state;
            if (state == PlayModeStateChange.ExitingPlayMode)
                ClearAllBuses();
        }
#endif

        /// <summary>
        /// 在游戏启动时初始化事件类型和事件总线类型。
        /// 当游戏加载完成但场景还未加载时调用，确保在场景加载前初始化所有事件相关的类型。
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            if (!enable) return;
            
            EventTypes = PredefinedAssemblyUtil.GetTypes(typeof(IEvent));       // 获取所有实现 IEvent 接口的类型
            EventBusTypes = InitializeAllBuses();
        }

        /// <summary> 遍历所有实现了IEvent接口的类型，为每个类型创建相应的EventBus&lt;T&gt;泛型示例，并添加到EventBusTypes中 </summary>
        static List<Type> InitializeAllBuses()
        {
            List<Type> eventBusTypes = new List<Type>();
            
            var typedef = typeof(EventBus<>);
            foreach (var eventType in EventTypes) 
            {
                var busType = typedef.MakeGenericType(eventType);
                eventBusTypes.Add(busType);
                Debug.Log($"Initialized EventBus<{eventType.Name}>");
            }
            
            return eventBusTypes;
        }

        /// <summary>
        /// 清除所有事件总线，在退出PlayMode时会自动调用
        /// </summary>
        public static void ClearAllBuses()
        {
            Debug.Log("Clearing all buses...");
            foreach (var busType in EventBusTypes)
            {
                var clearMethod = busType.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);
                clearMethod?.Invoke(null, null);
            }
        }
    }
}