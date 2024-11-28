using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tools.PoolModule2
{
    public class ObjectPool<T> where T : MonoBehaviour, IPoolable
    {
        private readonly Stack<T> _pool = new();
        private readonly T _prefab; // 预制体
        private readonly Transform _parent; // 父物体
        private readonly int maxCapacity; // 最大容量
        public int Count { get; private set; } // 当前数量
        public int PoolCount => _pool.Count; // 活跃数量
        public int ActiveCount => Count - _pool.Count; // 活跃数量

        public ObjectPool(T prefab, int initialCapacity = 0, int maxCapacity = 50)
        {
            _prefab = prefab;
            _parent = new GameObject($"[ObjectPool] {prefab.name}").transform;
            this.maxCapacity = maxCapacity;
            _pool = new Stack<T>(maxCapacity);
            
            for (int i = 0; i < initialCapacity; i++)
            {
                CreateNewObject();
            }
        }
        
        public T Get()
        {
            if (_pool.Count == 0)
            {
                CreateNewObject();
            }

            var pooledObject = _pool.Pop();
            pooledObject.gameObject.SetActive(true);
            return pooledObject;
        }
        
        public void Return(T obj)
        {
            obj.gameObject.SetActive(false);
            _pool.Push(obj);
        }
        
        private T CreateNewObject()
        {
            Debug.Assert(Count < maxCapacity, $"超出最大容量 {maxCapacity}");
            
            Count++;
            var obj = Object.Instantiate(_prefab, _parent);
            obj.name = $"{_prefab.name}-{Count}";
            obj.gameObject.SetActive(false);
            _pool.Push(obj);
            return obj;
        }
    }
}