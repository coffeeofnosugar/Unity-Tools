using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Tools.EasyPoolKit
{
    public class SimpleGameObjectPool : ISimpleGameObjectPool
    {
        private Transform _cachedRoot;  // 影藏位置
        protected Queue<GameObject> CachedQueue { get; private set; }   // 影藏队列
        protected LinkedList<GameObject> UsedList { get; private set; }
        protected Dictionary<int, float> UsedTimeDic { get; private set; }
        private int _objectCounter = 0;
        private bool _ifInit = false;

        public RecycleObjectType ObjectType => RecycleObjectType.GameObject;
        public Type ReferenceType { get; private set; }
        public string PoolId { get; private set; }
        public int? InitCreateCount { get; private set; }
        public int? MaxSpawnCount { get; private set; }
        public int? MaxDespawnCount { get; private set; }
        public float? AutoClearTime { get; private set; }
        public Func<object> SpawnFunc { get; set; }
        public PoolReachMaxLimitType ReachMaxLimitType { get; private set; }
        public PoolDespawnDestroyType DespawnDestroyType { get; private set; }
        public PoolClearType ClearType { get; private set; }
        public bool IsIgnoreTimeScale { get; private set; }
        
        public int GetCachedObjectCount() => CachedQueue.Count;
        public int GetUsedObjectCount() => UsedList.Count;
        public int GetTotalObjectCount() => CachedQueue.Count + UsedList.Count;
        
        protected RecyclablePoolInfo PoolInfo;
        public RecyclablePoolInfo GetPoolInfoReadOnly() => PoolInfo;

        public SimpleGameObjectPool(RecyclablePoolConfig config)
        {
            InitByConfig(config);
        }

        protected RecyclablePoolInfo InitByConfig(RecyclablePoolConfig config)
        {
            Assert.IsTrue(!_ifInit);
            Assert.IsNotNull(config);
            Assert.IsNotNull(config.SpawnFunc);
            Assert.IsTrue(config.ObjectType == RecycleObjectType.GameObject);
            
            ReferenceType = config.ReferenceType;
            PoolId = config.PoolId;
            SpawnFunc = config.SpawnFunc;
            ReachMaxLimitType = config.ReachMaxLimitType;
            DespawnDestroyType = config.DespawnDestroyType;
            ClearType = config.ClearType;
            InitCreateCount = config.InitCreateCount;
            MaxSpawnCount = config.MaxSpawnCount;
            MaxDespawnCount = config.MaxDespawnCount;
            AutoClearTime = config.AutoClearTime;
            IsIgnoreTimeScale = config.IsIgnoreTimeScale;
            
            PoolInfo = new RecyclablePoolInfo(config, GetCachedObjectCount, GetUsedObjectCount, GetTotalObjectCount, this);
            
            CachedQueue = MaxSpawnCount.HasValue
                ? new Queue<GameObject>(MaxSpawnCount.Value + 1)
                : new Queue<GameObject>();
            UsedList = new LinkedList<GameObject>();
            UsedTimeDic = new Dictionary<int, float>();
            
            OnInitByParams(config.ExtraArgs);
            InitCachedPool();
            _ifInit = true;

#if EASY_POOL_DEBUG
            Debug.Log($"EasyPoolKit == Create:Root:\n{GetDebugConfigInfo()}");

            // 检测配置是否合法
            if (ReachMaxLimitType is PoolReachMaxLimitType.RecycleOldest or PoolReachMaxLimitType.RecycleOldest)
            {
                Assert.IsTrue(MaxSpawnCount is > 0);
            }

            if (DespawnDestroyType is PoolDespawnDestroyType.DestroyToLimit)
            {
                Assert.IsTrue(MaxDespawnCount is > 0);
            }

            if (MaxDespawnCount.HasValue && MaxSpawnCount.HasValue)
            {
                Assert.IsTrue(MaxDespawnCount.Value <= MaxSpawnCount.Value);
            }

            if (InitCreateCount.HasValue && MaxSpawnCount.HasValue)
            {
                Assert.IsTrue(InitCreateCount.Value <= MaxSpawnCount.Value);
            }
#endif
            return PoolInfo;
        }
        
        protected virtual void OnInitByParams(object[] args)
        {
            var extraArgs = args;

            if (extraArgs != null && extraArgs.Length > 0 && extraArgs[0] is Transform root)
            {
                _cachedRoot = root;
            }
            else
            {
                Debug.LogError("EasyPoolKit == 创建Mono物体时，extraArgs[0]应该输入Transform当做父节点");
            }
        }

        private void InitCachedPool()
        {
            if (!InitCreateCount.HasValue)
            {
                return;
            }

            var initCount = InitCreateCount.Value;

            for (int i = 0; i < initCount; i++)
            {
                var cachedObj = CreateObject();
                CachedQueue.Enqueue(cachedObj);
                OnObjectEnqueue(cachedObj);
            }
        }
        
        protected virtual void OnObjectInit(GameObject usedObj){ }
        protected virtual void OnObjectEnqueue(GameObject usedObj) => usedObj.transform.SetParent(_cachedRoot, true);
        protected virtual void OnObjectDequeue(GameObject usedObj) => usedObj.transform.SetParent(null, true);
        protected virtual void OnObjectDestroyInit(GameObject usedObj)
        {
            if (usedObj)
            {
                UnityEngine.Object.Destroy(usedObj);
            }
        }
        
        
        // 内部方法，快速创建对象，不做繁琐的检查
        private GameObject CreateObject()
        {
            _objectCounter++;
            var cachedObj = SpawnFunc?.Invoke() as GameObject;

            if (cachedObj)
            {
#if EASY_POOL_DEBUG
                cachedObj.name = $"{PoolId}_{_objectCounter}";
#endif
                OnObjectInit(cachedObj);
            }
#if EASY_POOL_DEBUG
            else
            {
                Debug.LogError($"EasyPoolKit == GenerateObject {PoolId} 创建的物体不该为空!");
            }
#endif

            return cachedObj;
        }
        
        
        public GameObject SpawnObject()
        {
            GameObject cachedObj = null;

            if (GetCachedObjectCount() > 0)
            {
                cachedObj = CachedQueue.Dequeue();
            }
            else
            {
                bool isReachLimit = false;

                if (ReachMaxLimitType != PoolReachMaxLimitType.Default && MaxSpawnCount.HasValue)
                {
                    isReachLimit = GetTotalObjectCount() >= MaxSpawnCount.Value;
                }

                if (!isReachLimit)
                {
                    cachedObj = CreateObject();
                }
                else
                {
                    switch (ReachMaxLimitType)
                    {
                        case PoolReachMaxLimitType.RecycleOldest:
                            cachedObj = RecycleOldestObject();
                            break;
                        case PoolReachMaxLimitType.Default:
                            // 不可达到
                        case PoolReachMaxLimitType.RejectNull:
                        default:
                            // 什么都不做
                            break;
                    }
                }
            }

            if (cachedObj != null)
            {
                OnObjectEnqueue(cachedObj);
                UsedList.AddLast(cachedObj);
                UsedTimeDic[cachedObj.GetInstanceID()] = 0;
            }

            return cachedObj;

            GameObject RecycleOldestObject()
            {
#if EASY_POOL_DEBUG
                Assert.IsTrue(GetUsedObjectCount() > 0, $"EasyPoolKit == RecycleOldestObject {PoolId} 回收最老的物体时 UsedCount should > 0");
#endif
                var firstNode = UsedList.First;
                UsedList.Remove(firstNode);
                var oldestObj = firstNode.Value;

#if EASY_POOL_DEBUG
                if (!oldestObj)
                {
                    Debug.LogError($"EasyPoolKit == RecycleOldestObject {PoolId} 回收最老的物体时 OldestObj should not be null");
                }
#endif
                
                // 直接将该物体作为新物体使用，不添加到CachedQueue，也不执行OnObjectEnqueue
                return oldestObj;
            }
        }

        public bool TrySpawnObject(out GameObject newObj)
        {
            newObj = SpawnObject();
            return newObj != null;
        }

        public bool DespawnObject(GameObject usedObj)
        {
#if EASY_POOL_DEBUG
            if (usedObj == null)
            {
                Debug.LogError("EasyPoolKit == 回收的物体不能为空");
                return false;
            }
#endif       
            var usedNode = UsedList.Find(usedObj);
            if (usedNode == null)
            {
                Debug.LogError($"EasyPoolKit == 无法回收该物体，该物体并不在使用物体队列中:{usedObj.name}");
            }

            bool isReachLimit = false;

            if (DespawnDestroyType == PoolDespawnDestroyType.DestroyToLimit)
            {
                // if (MaxDespawnCount.HasValue && GetCachedObjectCount() > MaxDespawnCount.Value)
                if (MaxDespawnCount.HasValue && GetCachedObjectCount() > MaxDespawnCount.Value)
                {
                    isReachLimit = true;
                }
            }

            UsedList.Remove(usedNode);
            UsedTimeDic.Remove(usedObj.GetInstanceID());

            if (isReachLimit)
            {
                DestroyPoolObject(usedObj);
            }
            else
            {
                CachedQueue.Enqueue(usedObj);
                OnObjectEnqueue(usedObj);
            }

            return true;
        }

        public void OnPoolUpdate(float deltaTime)
        {
            if (UsedList.Count > 0)
            {
                var node = UsedList.First;
                float collectTime = AutoClearTime ?? -1f;
                while (node != null)
                {
                    var currentNode = node;
                    node = node.Next;
                    var usedObj = currentNode.Value;
                    
                    var objId = usedObj.GetInstanceID();
                    
                    var usedTime = UsedTimeDic[objId];
                    UsedTimeDic[objId] = usedTime + deltaTime;
                    
                    if (collectTime > 0 && usedTime >= collectTime)
                    {
                        DespawnObject(usedObj);
                    }
                }
            }
        }

        private void DestroyPoolObject(GameObject poolObj)
        {
            if (poolObj)
            {
                OnObjectDestroyInit(poolObj);
            }
        }

        #region Clear

        public void ClearUnusedObjects()
        {
            foreach (var cachedItem in CachedQueue)
            {
                DestroyPoolObject(cachedItem);
            }
            CachedQueue.Clear();
        }

        public void ClearAll()
        {
            switch (ClearType)
            {
                case PoolClearType.Default:
                    poolClearAll();
                    break;
                case PoolClearType.ClearToLimit:
                    poolClearToLimit();
                    break;
            }
        
            void poolClearAll()
            {
                foreach (var cachedItem in CachedQueue)
                {
                    DestroyPoolObject(cachedItem);
                }
                CachedQueue.Clear();

                while (UsedList.Count > 0)
                {
                    var firstNode = UsedList.First;
                    var firstObj = firstNode.Value;
                    UsedList.Remove(firstNode);
                    DestroyPoolObject(firstObj);
                }

                UsedList.Clear();
            }
        
            void poolClearToLimit()
            {
                if (!InitCreateCount.HasValue)
                {
                    poolClearAll();
                    return;
                }

                poolClearRetain();

                int removeCount = GetCachedObjectCount() - InitCreateCount.Value;

                while (removeCount > 0)
                {
                    removeCount--;
                    var cachedObj = CachedQueue.Dequeue();
                    DestroyPoolObject(cachedObj);
                }
            
                // 回收正在使用的物体
                void poolClearRetain()
                {
                    while (UsedList.Count > 0)
                    {
                        var firstNode = UsedList.First;
                        var firstObj = firstNode.Value;
                        DespawnObject(firstObj);
                    }
                }
            }
        }
        

        #endregion


        #region Debug
        public string GetDebugConfigInfo() => PoolInfo?.GetDebugConfigInfo() ?? string.Empty;
        public string GetDebugRunningInfo() => PoolInfo?.GetDebugRunningInfo() ?? string.Empty;
        #endregion
    }
}