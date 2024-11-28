using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools
{
    /// <summary>
    /// 懒汉单例，适合不需要获取索引的单例
    /// <para></para>
    /// 如果后续不小心创建了第二个，则会依据<see cref="_initializationTime"/>删除掉旧的单例，保存刚创建的。
    /// 如果先前没有设置物体，而重新创建物体时因为将物体的<see cref="GameObject.hideFlags"/>设置成了HideAndDontSave，所以物体不会显示在Hierarchy窗口。
    /// 但是能正常访问的，能有效的防止用户修改该单例
    /// </summary>
    /// <example> 可以看做是懒汉单例，比较适用于不需要获取索引的单例（可以将其视作非MonoBehaviour类，继承MonoBehaviour只是为拥有Update等便利的方法） </example>
    public abstract class PersistentHumbleSingleton<T> : MonoBehaviour where T : Component
    {
        protected static T _instance;
        public static bool HasInstance => _instance != null;
        public static T Current => _instance;

        [ShowInInspector, ReadOnly, PropertyOrder(-1000)]
        private float _initializationTime;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T> ();
                    // _instance = FindFirstObjectByType<T>();
                    if (_instance == null)
                    {
                        var obj = new GameObject()
                        {
                            hideFlags = HideFlags.HideAndDontSave,
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

            _initializationTime = Time.time;
            DontDestroyOnLoad(gameObject);
            T[] check = FindObjectsOfType<T>();
            // T[] check = FindObjectsByType<T>(FindObjectsSortMode.None);
            foreach (T searched in check)
            {
                if (searched != this && searched.GetComponent<PersistentHumbleSingleton<T>>()._initializationTime < _initializationTime)
                    Destroy(searched.gameObject);
            }

            if (_instance == null)
                _instance = this as T;
        }
    }
}