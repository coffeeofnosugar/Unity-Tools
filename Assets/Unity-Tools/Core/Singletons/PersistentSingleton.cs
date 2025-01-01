using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools
{
    /// <summary>
    /// 饿汉单例，适合在游戏开始时就初始化的单例
    /// <para></para>
    /// 物体会被存放在DontDestroyLoad中不会被摧毁，整个游戏生命周期都永远只有一个
    /// 唯一需要注意的是，若实例化的单例需要引用其他物体，则需要在EditorMode的时候就手动创建物体并拖拽引用（除非你愿意用代码去获取其他物体）
    /// </summary>
    public abstract class PersistentSingleton<T> : MonoBehaviour where T : Component
    {
        [ShowInInspector, DisableInPlayMode]
        private bool automaticallyUnparentOnAwake;
        
        protected static T _instance;
        public static bool HasInstance => _instance != null;
        public static T Current => _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    // _instance = FindFirstObjectByType<T>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject()
                        {
                            name = typeof(T).Name + "_AutoCreated",
                        };
                        _instance = obj.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            InitializeSingleton();
        }

        protected virtual void InitializeSingleton()
        {
            if (!Application.isPlaying) return;

            if (automaticallyUnparentOnAwake)
                transform.SetParent(null);
                
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                if (this != _instance)
                    Destroy(gameObject);
            }
        }
    }
}