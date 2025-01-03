using System;
using UnityEngine;

namespace Tools.EasyPoolKit
{
    public interface ISimpleGameObjectPool : IAbstractPool<GameObject> { }
    
    public interface IRecyclablePool<T> : IAbstractPool<T> where T : class, IRecyclable { }
    
    public interface IAbstractPool<T> where T : class
    {
        RecycleObjectType ObjectType { get; }

        Type ReferenceType { get; }

        string PoolId { get; }

        int? InitCreateCount { get; }

        int? MaxSpawnCount { get; }

        int? MaxDespawnCount { get; }

        float? AutoClearTime { get; }

        Func<object> SpawnFunc { get; set; }

        PoolReachMaxLimitType ReachMaxLimitType { get; }

        PoolDespawnDestroyType DespawnDestroyType { get; }
        
        PoolClearType ClearType { get; }

        bool IsIgnoreTimeScale { get; }

        int GetCachedObjectCount();

        int GetUsedObjectCount();

        int GetTotalObjectCount();
        
        T SpawnObject();
        
        bool TrySpawnObject(out T newObj);
        
        bool DespawnObject(T usedObj);
        
        void ClearUnusedObjects();
        
        void ClearAll();
        
        void OnPoolUpdate(float deltaTime);

        RecyclablePoolInfo GetPoolInfoReadOnly();
    }
}