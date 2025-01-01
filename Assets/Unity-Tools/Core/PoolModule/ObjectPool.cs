using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tools.PoolModule
{
    public class ObjectPool<T> : IDisposable
        where T : MonoBehaviour, IPoolable
    {
        private readonly Stack<T> _pool;
        private readonly List<T> _allObject;
        public readonly IReadOnlyList<T> AllObject;
        private readonly List<T> _activeObject;
        public readonly IReadOnlyList<T> ActiveObject;
        private readonly T _prefab;                     // 预制体
        private readonly Transform _parent;             // 父物体
        private readonly int maxCapacity;               // 最大容量
        public int Count { get; private set; }          // 总数量
        public int PoolCount => _pool.Count;            // 对象池中数量
        public int ActiveCount => _activeObject.Count;  // 活跃数量

        /// 温馨提示：如果预制体是通过Addressable加载的，可以直接释放预制体
        public ObjectPool(T prefab, int initialCapacity = 0, int maxCapacity = 50, Transform parent = null)
        {
            _prefab = prefab;
            _parent = parent ?? new GameObject($"[ObjectPool] {prefab.name}").transform;
            this.maxCapacity = maxCapacity;
            _pool = new Stack<T>(maxCapacity);
            _allObject = new List<T>(maxCapacity);
            AllObject = _allObject;
            _activeObject = new List<T>(maxCapacity);
            ActiveObject = _activeObject;
            for (int i = 0; i < initialCapacity; i++)
                CreateNewObject();
            // 排序
            Stack<T> temp = new Stack<T>(_pool.Count);
            foreach (var o in _pool)
                temp.Push(o);
            _pool = temp;
            Debug.Log($"成功创建对象池: {prefab.name} (初始容量: {initialCapacity}, 最大容量: {maxCapacity})");
        }
            
        public T Get()
        {
            if (_pool.Count == 0)
                CreateNewObject();

            var pooledObject = _pool.Pop();
            _activeObject.Add(pooledObject);
            pooledObject.gameObject.SetActive(true);
            pooledObject.OnGet();
            return pooledObject;
        }

        public T[] Get(int count)
        {
            T[] result = new T[count];
            for (int i = 0; i < count; i++)
                result[i] = Get();
            return result;
        }
        
        public void Return(T obj)
        {
            obj.OnReturn();
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(_parent);
            _pool.Push(obj);
            _activeObject.Remove(obj);
        }

        public void ReturnAll()
        {
            for (int i = _activeObject.Count - 1; i >= 0; i--)
                Return(_activeObject[i]);
        }
            
        private void CreateNewObject()
        {
            Debug.Assert(Count < maxCapacity, $"{_parent.name} 超出最大容量 {maxCapacity}， 此警告只是起提示作用，依然能正常创建物体，但请记得增大该对象池的容量");
            
            Count++;
            var obj = Object.Instantiate(_prefab, _parent);
            obj.name = $"{_prefab.name}-{Count}";
            obj.gameObject.SetActive(false);
            _pool.Push(obj);
            _allObject.Add(obj);
        }

        public void Dispose()
        {
            while (_pool.Count > 0)
                Object.Destroy(_pool.Pop().gameObject);

            for (int i = _activeObject.Count - 1; i >= 0; i--)
                Object.Destroy(_activeObject[i].gameObject);
            
            _pool.Clear();
            _activeObject.Clear();
            Object.Destroy(_parent.gameObject);
        }
    }
}