using System;
using System.Collections.Generic;

namespace Tools.EasyPoolKit
{
    public abstract class RecyclablePoolBase<T> : IRecyclablePool<T> where T : class, IRecyclable
    {
        protected Queue<T> CachedQueue { get; private set; }

        protected LinkedList<T> UsedList { get; private set; }

        private int _objectCounter = 0;

        private bool _ifInit = false;
        
        public RecycleObjectType ObjectType { get; private set;}
        public Type ReferenceType { get; private set;}
        public string PoolId { get; private set;}
        public int? InitCreateCount { get; private set;}
        public int? MaxSpawnCount { get; private set;}
        public int? MaxDespawnCount { get; private set;}
        public float? AutoClearTime { get; private set;}
        public Func<object> SpawnFunc { get; set; }
        public PoolReachMaxLimitType ReachMaxLimitType { get; private set;}
        public PoolDespawnDestroyType DespawnDestroyType { get; private set;}
        public PoolClearType ClearType { get; private set;}
        public bool IsIgnoreTimeScale { get; private set;}

        public int GetCachedObjectCount() => CachedQueue.Count;
        public int GetUsedObjectCount() => UsedList.Count;
        public int GetTotalObjectCount() => CachedQueue.Count + UsedList.Count;
        
        protected readonly RecyclablePoolInfo PoolInfo;
        public RecyclablePoolInfo GetPoolInfoReadOnly() => PoolInfo;
        
        public RecyclablePoolBase(RecyclablePoolConfig config)
        {
            PoolInfo = InitByConfig(config);
        }

        protected virtual RecyclablePoolInfo InitByConfig(RecyclablePoolConfig config)
        {
            if (_ifInit)
            {
                throw new Exception("EasyEventPool == Already Init!");
            }

            if (config == null)
            {
                throw new Exception("EasyEventPool == config is null!");
            }

            ObjectType = config.ObjectType;
            ReferenceType = config.ReferenceType;
            PoolId = config.PoolId;
            SpawnFunc = config.SpawnFunc ?? throw new Exception("EasyEventPool == config.SpawnFunc is null!");
            ReachMaxLimitType = config.ReachMaxLimitType;
            DespawnDestroyType = config.DespawnDestroyType;
            ClearType = config.ClearType;
            InitCreateCount = config.InitCreateCount;
            MaxSpawnCount = config.MaxSpawnCount;
            MaxDespawnCount = config.MaxDespawnCount;
            AutoClearTime = config.AutoClearTime;
            IsIgnoreTimeScale = config.IsIgnoreTimeScale;

            var poolInfo = new RecyclablePoolInfo(config, GetCachedObjectCount, GetUsedObjectCount, GetTotalObjectCount, this);
            
            CachedQueue = MaxSpawnCount.HasValue
                ? new Queue<T>(MaxSpawnCount.Value + 1)
                : new Queue<T>();

            UsedList = new LinkedList<T>();
            OnInitByParams(config.ExtraArgs);
            InitCachedPool();
            _ifInit = true;
            
#if EASY_POOL_DEBUG
            //Check if params are valid
            if (ReachMaxLimitType == PoolReachMaxLimitType.RejectNull ||
                ReachMaxLimitType == PoolReachMaxLimitType.RecycleOldest)
            {
                if (!(MaxSpawnCount.HasValue && MaxSpawnCount.Value > 0))
                {
                    throw new Exception("MaxSpawnCount.Value should > 0");
                }
            }

            if (DespawnDestroyType == PoolDespawnDestroyType.DestroyToLimit)
            {
                if (!(MaxDespawnCount.HasValue && MaxDespawnCount.Value > 0))
                {
                    throw new Exception("MaxDespawnCount.Value should > 0");
                }
            }

            if (MaxSpawnCount.HasValue && MaxDespawnCount.HasValue)
            {
                if (!(MaxDespawnCount.Value <= MaxSpawnCount.Value))
                {
                    throw new Exception("MaxDespawnCount.Value should <= MaxSpawnCount.Value");
                }
            }

            if (MaxSpawnCount.HasValue && InitCreateCount.HasValue)
            {
                if (!(InitCreateCount.Value <= MaxSpawnCount.Value))
                {
                    throw new Exception("InitCreateCount.Value should <= MaxSpawnCount.Value");
                }
            }
#endif
            return poolInfo;
        }

        public T SpawnObject()
        {
            T cachedObj = null;

            if (GetCachedObjectCount() > 0)
            {
                cachedObj = CachedQueue.Dequeue();
            }
            else
            {
                bool ifReachLimit = false;

                if (ReachMaxLimitType != PoolReachMaxLimitType.Default && MaxSpawnCount.HasValue)
                {
                    ifReachLimit = GetTotalObjectCount() >= MaxSpawnCount.Value;
                }

                if (!ifReachLimit)
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
                        //Impossible
                        case PoolReachMaxLimitType.RejectNull:
                        default:
                            //do nothing
                            break;
                    }
                }
            }

            if (cachedObj != null)
            {
                OnObjectDequeue(cachedObj);
                cachedObj.OnObjectSpawn();
                UsedList.AddLast(cachedObj);
            }

            return cachedObj;
        }

        public bool TrySpawnObject(out T newObj)
        {
            newObj = SpawnObject();
            return newObj != null;
        }

        public bool DespawnObject(T usedObj)
        {
            if (usedObj == null)
            {
                return false;
            }
            
            var usedNode = UsedList.Find(usedObj);

            if (usedNode == null)
            {
                return false;
            }

            bool ifReachLimit = false;
            
            if (DespawnDestroyType == PoolDespawnDestroyType.DestroyToLimit)
            {
                if (MaxDespawnCount.HasValue && GetTotalObjectCount() > MaxDespawnCount.Value)
                {
                    ifReachLimit = true;
                }
            }

            UsedList.Remove(usedNode);
            usedObj.OnObjectDespawn();

            if (ifReachLimit)
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
                var beginNode = UsedList.First;
                float collectTime = AutoClearTime ?? -1f;
                
                while (beginNode != null)
                {
                    var currentNode = beginNode;
                    beginNode = beginNode.Next;
                    var usedObj = currentNode.Value;
                    usedObj.OnObjectUpdate(deltaTime);

                    if (collectTime > 0 && usedObj.UsedTime >= collectTime)
                    {
                        DespawnObject(usedObj);
                    }
                }
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
        

        private T CreateObject()
        {
            _objectCounter++;

            if (SpawnFunc?.Invoke() is T cachedObj)
            {
                cachedObj.PoolId = PoolId;
                cachedObj.ObjectId = _objectCounter;
            
#if EASY_POOL_DEBUG
                cachedObj.Name = $"{PoolId}_{cachedObj.ObjectId}";
#endif
                OnObjectInit(cachedObj);
                cachedObj.OnObjectInit();    
            }
#if EASY_POOL_DEBUG
            else
            {
                throw new Exception($"EasyPoolKit == GenerateObject {PoolId} Should not be null!");
            }
#endif

            return cachedObj;
        }

        protected virtual void OnInitByParams(object[] args){}
        protected abstract void OnObjectInit(T usedObj);
        protected abstract void OnObjectEnqueue(T usedObj);
        protected abstract void OnObjectDequeue(T usedObj);
        protected abstract void OnObjectDestoryInit(T usedObj);
        
        private T RecycleOldestObject()
        {
            var firstNode = UsedList.First;
            UsedList.Remove(firstNode);
            var oldestObj = firstNode.Value;

            if (oldestObj != null)
            {
                oldestObj.OnObjectDespawn();
            }

            //As it is a free node, need not to add it to cachedQueue 
            return oldestObj;
        }
        
        private void DestroyPoolObject(T poolObj)
        {
            if (poolObj != null)
            {
                poolObj.OnObjectDestroyInit();
                OnObjectDestoryInit(poolObj);   
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