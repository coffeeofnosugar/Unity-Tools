using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Tools.PoolModule
{
    /// <summary>
    /// 本工厂与普通的对象池工厂不同，
    /// 该对象池工厂只有在需要时才会创建对象池。
    /// 字典的键为预制体名称的不同点，如预制体名称为 ItemA ItemB ItemC 等，那么键就是A B C，也可以是ItemA ItemB ItemC，取决于Path的{0}占位符。
    /// 但需要注意的是，需要使用唯一表示区分不同的预制体
    /// <para></para>
    /// 适合一个类型有多个实例的情况
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPoolFactoryString<T> : IDisposable
        where T : MonoBehaviour, IPoolableString
    {
        private readonly Dictionary<string, ObjectPool<T>> _pools = new();

        /// 该路径为预制体的路径，使用{0}占位符
        protected virtual string Path { get; } = "Assets/Unity-Tools/Samples/PoolModule/PoolModule2/Item{0}.prefab";
        protected virtual int InitialCapacity { get; }
        protected virtual int MaxCapacity { get; }

        public ObjectPoolFactoryString(string path, int initialCapacity = 0, int maxCapacity = 50)
        {
            Path = path;
            InitialCapacity = initialCapacity;
            MaxCapacity = maxCapacity;
        }

        protected async UniTask CreatePool(string name, int initialCapacity = 0, int maxCapacity = 50)
        {
            string fullPath = string.Format(Path, name);
            GameObject obj = await Addressables.LoadAssetAsync<GameObject>(fullPath);
            if (obj == null)
            {
                Debug.LogError($"加载失败: 路径 {fullPath}");
                return;
            }
            
            T item = obj.GetComponent<T>();
            
            if (item == null)
            {
                Debug.LogError($"对象不包含类型 {typeof(T).Name}: 路径 {fullPath}");
                return;
            }
            
            if (_pools.ContainsKey(name))
            {
                Debug.LogWarning($"对象池已存在: {name}");
                return;
            }
            
            var pool = new ObjectPool<T>(item, initialCapacity, maxCapacity);
            _pools.Add(name, pool);
            Addressables.Release(obj);  // 可以直接释放预制体
        }

        public async UniTask<T> Get(string name)
        {
            if (!_pools.ContainsKey(name))
            {
                await CreatePool(name, InitialCapacity, MaxCapacity);
            }
            return _pools[name].Get();
        }

        public async UniTask<T[]> Get(string name, int count)
        {
            if (!_pools.ContainsKey(name))
            {
                await CreatePool(name, InitialCapacity, MaxCapacity);
            }
            return _pools[name].Get(count);
        }
        
        public void Return(T obj)
        {
            string name = obj.Name;
            if (_pools.TryGetValue(name, out var pool))
            {
                pool.Return(obj);
            }
            else
            {
                Debug.LogError($"未找到对象池: {name}");
            }
        }
        
        public void ReturnAll()
        {
            foreach (ObjectPool<T> pool in _pools.Values)
            {
                pool.ReturnAll();
            }
        }
        
        public void Dispose()
        {
            foreach (ObjectPool<T> pool in _pools.Values)
            {
                pool.Dispose();
            }

            _pools.Clear();
            Debug.Log("已清理所有对象池并释放资源");
        }
    }
}