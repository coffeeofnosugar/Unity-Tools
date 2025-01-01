using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Tools.PoolModule2
{
    /// <summary>
    /// 本工厂与普通的对象池工厂不同，它只会创建一个对象，而不是创建多个对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingleFactory<T> : IDisposable
        where T : MonoBehaviour, IPoolable
    {
        private readonly Transform _parent; // 父物体
        private readonly string prefabPath;
        private readonly Dictionary<string, T> pool = new();
        
        public SingleFactory(string prefabPath)
        {
            _parent = new GameObject($"[ObjectPool] {typeof(T).Name}").transform;
            this.prefabPath = prefabPath;
        }

        public async UniTask<TO> Get<TO>()
            where TO :  T
        {
            string name = typeof(TO).Name;
            if (!pool.TryGetValue(name, out var obj))
            {
                obj = await Create<TO>(name);
            }
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            obj.OnGet();
            return obj as TO;
        }
        
        public void Return(T obj)
        {
            obj.OnReturn();
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(_parent);
        }
        
        public void ReturnAll()
        {
            foreach (T obj in pool.Values)
            {
                Return(obj);
            }
        }

        private async UniTask<TO> Create<TO>(string name)
            where TO : T
        {
            GameObject prefab = await Addressables.LoadAssetAsync<GameObject>(string.Format(prefabPath, name));
            if (prefab == null)
            {
                Debug.LogError($"加载失败: 路径 {string.Format(prefabPath, name)}");
                return null;
            }
            var obj = Object.Instantiate(prefab, _parent);
            Addressables.Release(prefab);       // 可立即释放预制体
            var component = obj.GetComponent<TO>();
            if (component == null)
            {
                Debug.LogError($"对象不包含类型 {typeof(TO).Name}: 路径 {string.Format(prefabPath, name)}");
                return null;
            }
            obj.SetActive(false);
            pool.Add(name, component);
            return component;
        }

        public void Dispose()
        {
            foreach (T obj in pool.Values)
            {
                Object.Destroy(obj);
            }
            Object.Destroy(_parent.gameObject);
            pool.Clear();
        }
    }
}