using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tools.PoolModule2
{
    public class ObjectPool<T> : IDisposable
        where T : MonoBehaviour, IPoolable
    {
        private readonly Stack<T> _pool;
        private readonly T _prefab; // 预制体
        private readonly Transform _parent; // 父物体
        private readonly int maxCapacity; // 最大容量
        public int Count { get; private set; } // 当前数量
        public int PoolCount => _pool.Count; // 活跃数量
        public int ActiveCount => Count - _pool.Count; // 活跃数量

        /// 温馨提示：如果预制体是通过Addressable加载的，可以直接释放预制体
        public ObjectPool(T prefab, int initialCapacity = 0, int maxCapacity = 50)
        {
            _prefab = prefab;
            _parent = new GameObject($"[ObjectPool] {prefab.name}").transform;
            this.maxCapacity = maxCapacity;
            _pool = new Stack<T>(maxCapacity);
                
            for (int i = 0; i < initialCapacity; i++)
                CreateNewObject();
            Debug.Log($"成功创建对象池: {prefab.name} (初始容量: {initialCapacity}, 最大容量: {maxCapacity})");
        }
            
        public T Get()
        {
            if (_pool.Count == 0)
            {
                CreateNewObject();
            }

            var pooledObject = _pool.Pop();
            pooledObject.gameObject.SetActive(true);
            pooledObject.OnGet();
            return pooledObject;
        }

        public T[] Get(int count)
        {
            T[] result = new T[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = Get();
            }
            return result;
        }
        
        /// 一般在Dispose时调用
        internal T GetWithoutAction() => _pool.Pop();
        
        public void Return(T obj)
        {
            obj.OnReturn();
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(_parent);
            _pool.Push(obj);
        }
            
        private T CreateNewObject()
        {
            Debug.Assert(Count < maxCapacity, $"超出最大容量 {maxCapacity}， 此警告只是起提示作用，依然能正常创建物体，但请记得增大该对象池的容量");
                
            Count++;
            var obj = Object.Instantiate(_prefab, _parent);
            obj.name = $"{_prefab.name}-{Count}";
            obj.gameObject.SetActive(false);
            _pool.Push(obj);
            return obj;
        }

        public void Dispose()
        {
            while (_pool.Count > 0)
            {
                var obj = _pool.Pop();
                Object.Destroy(obj.gameObject);
            }
            Object.Destroy(_parent.gameObject);
            _pool.Clear();
        }
    }
}