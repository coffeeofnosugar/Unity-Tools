using UnityEngine;

namespace Tools
{
    /// <summary>
    /// 最简洁的单例，适合跟随场景的单例
    /// <para></para>
    /// 缺点：如果不小心创建了第二个，则同时会存在两个单例
    /// </summary>
    /// <example> 因为在切场景时会被摧毁，所以适合于与场景绑定的单例 </example>
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        protected static T _instance;
        public static bool HasInstance => _instance != null;
        public static T TryGetInstance() => HasInstance ? _instance : null;
        public static T Current => _instance;

        public static T Instance
        {
            get
            {
                if (_instance ==null)
                {
                    _instance = FindObjectOfType<T>();
                    // _instance = FindFirstObjectByType<T>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject()
                        {
                            name = typeof(T).Name + "_AutoCreated"
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

            _instance = this as T;
        }
    }
}