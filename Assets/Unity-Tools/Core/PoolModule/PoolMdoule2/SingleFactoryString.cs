﻿using System;
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
    public class SingleFactoryString<T> : IDisposable
        where T : MonoBehaviour, IPoolable
    {
        private readonly Transform _parent; // 父物体
        private readonly string prefabPath;
        private readonly Dictionary<string, T> pool = new();
        
        public SingleFactoryString(string prefabPath)
        {
            _parent = new GameObject($"[ObjectPool] {typeof(T).Name}").transform;
            this.prefabPath = prefabPath;
        }

        public async UniTask<T> Get(string name)
        {
            if (!pool.TryGetValue(name, out var obj))
            {
                obj = await Create(name);
            }
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            obj.OnGet();
            return obj;
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

        private async UniTask<T> Create(string name)
        {
            GameObject prefab = await Addressables.LoadAssetAsync<GameObject>(string.Format(prefabPath, name));
            if (prefab == null)
            {
                Debug.LogError($"加载失败: 路径 {string.Format(prefabPath, name)}");
                return null;
            }
            var obj = Object.Instantiate(prefab, _parent);
            Addressables.Release(prefab);       // 可立即释放预制体
            var component = obj.GetComponent<T>();
            if (component == null)
            {
                Debug.LogError($"对象不包含类型 {typeof(T).Name}: 路径 {string.Format(prefabPath, name)}");
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