using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Tools.PoolModule2
{
    public abstract class ObjectPoolFactory<T> : IDisposable
        where T : MonoBehaviour, IPoolable
    {
        private readonly Dictionary<string, ObjectPool<T>> _pools = new();
        
        public abstract UniTask InitAsync();
        
        protected async UniTask CreatePool(string fullPath, int initialCapacity = 0, int maxCapacity = 50)
        {
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
            
            string name = item.GetType().Name;
            if (_pools.ContainsKey(name))
            {
                Debug.LogWarning($"对象池已存在: {name}");
                return;
            }
            
            var pool = new ObjectPool<T>(item, initialCapacity, maxCapacity);
            _pools.Add(name, pool);
            Addressables.Release(obj);  // 可以直接释放预制体
        }
        
        public TO Get<TO>()
            where TO : T
        {
            string name = typeof(TO).Name;
            if (_pools.TryGetValue(name, out var pool))
            {
                return pool.Get() as TO;
            }
            
            Debug.LogError($"未找到对象池: {name}");
            return null;
        }

        public TO[] Get<TO>(int count)
            where TO : T
        {
            string name = typeof(TO).Name;
            if (_pools.TryGetValue(name, out var pool))
            {
                return pool.Get(count) as TO[];
            }
            
            Debug.LogError($"未找到对象池: {name}");
            return null;
        }
        
        public void Return(T obj)
        {
            string name = obj.GetType().Name;
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
            foreach (KeyValuePair<string, ObjectPool<T>> pair in _pools)
            {
                var pool = pair.Value;
                var objects = new List<T>();

                // 将所有对象回收到池中
                while (pool.PoolCount > 0)
                {
                    objects.Add(pool.GetWithoutAction());
                }

                // 销毁池中的对象
                foreach (var obj in objects)
                {
                    Object.Destroy(obj.gameObject);
                }
            }

            _pools.Clear();
            Debug.Log("已清理所有对象池并释放资源");
        }
    }
}