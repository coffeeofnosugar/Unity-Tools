using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tools.PoolModule1
{
    /// <summary> 不要使用单例，在每个需要使用对象池的地方实例化一个 </summary>
    public abstract class ObjectPooler : MonoBehaviour
    {
        // public static ObjectPooler Instance;
        /// true：所有物体使用一个对象池
        [DisableInPlayMode, OnValueChanged("@NestUnderThis = false")]
        public bool MutualizeWaitingPools = false;
        /// ture：所有等待和活动对象都会被存放在一个空物体对象下。否则他们都会在顶层
        [DisableInPlayMode]
        public bool NestWaitingPool = true;
        /// true：存放在自己层级下
        [ShowIf("NestWaitingPool"), DisableInPlayMode, DisableIf("MutualizeWaitingPools")]
        public bool NestUnderThis = false;
        protected GameObject _waitingPool;
        protected ObjectPool _objectPool;
        protected const int _initialCapacity = 5;
        
        public static List<ObjectPool> _pools = new List<ObjectPool>(_initialCapacity);
        
        // /// 在每次切换场景时重新初始化单例
        // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        // protected static void InitializeStatics()
        // {
        //     Instance = null;
        // }

        protected void Awake()
        {
            // Instance = this;
            FillObjectPool();
        }


        private void AddPool(ObjectPool pool)
        {
            _pools ??= new List<ObjectPool>(_initialCapacity);
            Debug.Assert(!_pools.Contains(pool), $"重复添加{pool.name}");
            if (!_pools.Contains(pool))
                _pools.Add(pool);
        }

        private void RemovePool(ObjectPool pool)
        {
            _pools?.Remove(pool);
        }

        /// <summary>
        /// 创建新的对象池。
        /// 当MutualizeWaitingPools为false时，直接创建一个新的对象池，不管是否拥有相同的对象池名称。
        /// 当MutualizeWaitingPools为true时，先寻找是否已有同名的对象池，有则共用，没有则创建新的对象池。
        /// </summary>
        /// <returns>
        /// true: 创建成功
        /// false: 创建失败，找到了同名的对象池，并与其共用</returns>
        protected virtual bool CreateWaitingPool()
        {
            if (!MutualizeWaitingPools)
            {
                _waitingPool = new GameObject(DetermineObjectPoolName());
                SceneManager.MoveGameObjectToScene(_waitingPool, this.gameObject.scene);    // 确保物体都在同一个场景内
                _objectPool = _waitingPool.AddComponent<ObjectPool>();
                _objectPool.PooledGameObjects = new List<GameObject>();
                ApplyNesting();
                return true;
            }
            else
            {
                ObjectPool objectPool = ExistingPool(DetermineObjectPoolName());
                if (objectPool != null)
                {
                    _waitingPool = objectPool.gameObject;
                    _objectPool = objectPool;
                    return false;
                }
                else
                {
                    _waitingPool = new GameObject(DetermineObjectPoolName());
                    SceneManager.MoveGameObjectToScene(_waitingPool, this.gameObject.scene);
                    _objectPool = _waitingPool.AddComponent<ObjectPool>();
                    _objectPool.PooledGameObjects = new List<GameObject>();
                    ApplyNesting();
                    AddPool(_objectPool);
                    return true;
                }
            }
        }
        
        public virtual ObjectPool ExistingPool(string poolName)
        {
            _pools ??= new List<ObjectPool>(_initialCapacity);
            if (_pools.Count == 0)
            {
                var pools = FindObjectsOfType<ObjectPool>();
                // var pools = FindObjectsByType<ObjectPool>(FindObjectsSortMode.None);
                if (pools.Length > 0)
                    _pools.AddRange(pools);
            }

            return _pools.FirstOrDefault(pool => pool != null && pool.name == poolName);
        }

        protected virtual string DetermineObjectPoolName() => $"[ObjectPooler] {this.name}";
        
        /// <summary> 设置对象池层级 </summary>
        protected virtual void ApplyNesting()
        {
            if (NestWaitingPool && NestUnderThis && (_waitingPool != null))
            {
                _waitingPool.transform.SetParent(this.transform);
            }
        }
        /// <summary> 重写该方法设置物体 </summary>
        protected virtual void FillObjectPool()
        {
            return;
        }
        
        /// <summary> 重写该方法获取物体 </summary>
        public virtual GameObject GetPooledGameObject()
        {
            return null;
        }
        
        /// <summary> 销毁对象池 </summary>
        public virtual void DestroyObjectPool()
        {
            if (_waitingPool != null)
            {
                Destroy(_waitingPool.gameObject);
            }
        }

        private void OnDestroy()
        {
            if (_objectPool != null && NestUnderThis)
            {
                RemovePool(_objectPool);
            }
        }
    }
}