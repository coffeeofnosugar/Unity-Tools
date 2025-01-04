using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Tools.EasyPoolKit
{
    public class SimpleGOPoolKit : MonoBehaviour
    {
        public static SimpleGOPoolKit Instance
        {
            get
            {
                if (_instance == null)
                {
                    var poolRoot = new GameObject("SimpleGOPoolKit");
                    var cachedRoot = new GameObject("CachedRoot");
                    cachedRoot.transform.SetParent(poolRoot.transform, false);
                    cachedRoot.gameObject.SetActive(false);
                    _instance = poolRoot.AddComponent<SimpleGOPoolKit>();
                    _instance._cachedRoot = cachedRoot.transform;
                    DontDestroyOnLoad(_instance.gameObject);
                }

                return _instance;
            }
        }

        private static SimpleGOPoolKit _instance;
        
        // 缓存根节点
        private Transform _cachedRoot;
        
        // key: 预制体的HashID, value: 对应的预制体
        private Dictionary<int, GameObject> _prefabTemplates = new Dictionary<int, GameObject>();
        
        // key: 预制体的HashID, value: 对应的对象池
        private Dictionary<int, SimpleGameObjectPool> _gameObjPools = new Dictionary<int, SimpleGameObjectPool>();

        // key: 激活的物体实例的HashID, value: 对应的预制体的HashID
        private Dictionary<int, int> _gameObjRelations = new Dictionary<int, int>();
        
        private List<RecyclablePoolInfo> _poolInfoList = new List<RecyclablePoolInfo>();
        public List<RecyclablePoolInfo> GetPoolsInfo() => _poolInfoList;
        
        private bool _ifAppQuit = false;
        
        /// 检测是否已有对应的对象池
        public bool IsPoolValid(int prefabHash) => _prefabTemplates.ContainsKey(prefabHash) && _gameObjPools.ContainsKey(prefabHash);
        
        private void Awake()
        {
            Debug.Log("Awake");
            if (_instance != null)
            {
                Debug.LogError("EasyPoolKit == 不要绑定SimpleGOPoolKit到任何物体上, 使用SimpleGOPoolKit.Instance!");
                Destroy(gameObject);
            }
        }
        
        private void Update()
        {
            foreach (var poolPair in _gameObjPools)
            {
                var pool = poolPair.Value;
                var deltaTime = pool.IsIgnoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
                pool.OnPoolUpdate(deltaTime);
            }
        }
        
        private void OnApplicationQuit()
        {
            _ifAppQuit = true;
        }

        private void OnDestroy()
        {
            if (!_ifAppQuit)
            {
                ClearAllPools(true);
            }
        }

        public SimpleGameObjectPool RegisterPrefab(GameObject prefabAsset, RecyclablePoolConfig config = null)
        {
            Assert.IsNotNull(prefabAsset);

            var prefabHash = prefabAsset.GetInstanceID();

#if EASY_POOL_DEBUG
            if (_gameObjPools.ContainsKey(prefabHash) || _prefabTemplates.ContainsKey(prefabHash))
            {
                Debug.LogError($"EasyPoolKit == {prefabAsset.name} 已经注册过该预制体了!");
                return null;
            }
#endif
            
            if (config == null)
            {
                config = GetSimpleGOPoolConfig(prefabAsset, null);
            }
            
            if (config.SpawnFunc == null)
            {
                config.SpawnFunc = () => DefaultCreateObjectFunc(prefabHash);   // 这里只是注册了一个方法，并没有直接执行
            }

            if (config.ExtraArgs == null || config.ExtraArgs.Length == 0)
            {
                config.ExtraArgs = new object[] { _cachedRoot };
            }

            _prefabTemplates[prefabHash] = prefabAsset;
            var newPool = new SimpleGameObjectPool(config);
            _gameObjPools[prefabHash] = newPool;
            _poolInfoList.Add(newPool.GetPoolInfoReadOnly());

            return newPool;
        }

        public bool UnRegisterPrefab(int prefabHash)
        {
            _prefabTemplates.Remove(prefabHash);
            RemoveObjectsRelationByAssetHash(prefabHash);
            if (_gameObjPools.TryGetValue(prefabHash, out var pool))
            {
                pool.ClearAll();
                _poolInfoList.Remove(pool.GetPoolInfoReadOnly());
                _gameObjPools.Remove(prefabHash);

                return true;
            }

            return false;
        }
        
        /// <summary>
        /// 简单的生成一个对象
        /// <para></para>
        /// 使用该方法生成的对象时需要设置手动设置父级，否则无法正常取出对象
        /// </summary>
        /// <param name="prefabTemplate"></param>
        /// <returns></returns>
        public GameObject SimpleSpawn(GameObject prefabTemplate)
        {
            Assert.IsNotNull(prefabTemplate);

            var prefabHash = prefabTemplate.GetInstanceID();

            if (!_gameObjPools.TryGetValue(prefabHash, out var pool))
            {
                pool = RegisterPrefab(prefabTemplate);
            }

            var newObj = pool.SpawnObject();

            _gameObjRelations[newObj.GetInstanceID()] = prefabHash;
            
            return newObj;
        }
        
        public bool TrySpawn(int prefabHash, out GameObject newObj)
        {
            newObj = null;
            if (_gameObjPools.TryGetValue(prefabHash, out var pool))
            {
                newObj = pool.SpawnObject();
                _gameObjRelations[newObj.GetInstanceID()] = prefabHash;
            }

            return newObj != null;
        }

        public bool Despawn(GameObject usedObj)
        {
            Assert.IsNotNull(usedObj);

            var objHash = usedObj.GetInstanceID();

            if (!_gameObjRelations.TryGetValue(objHash, out var assetHash))
            {
                return false;
            }

            if (!_gameObjPools.TryGetValue(assetHash, out var pool))
            {
                return false;
            }

            _gameObjRelations.Remove(objHash);
            
            return pool.DespawnObject(usedObj);
        }
        
        public bool ClearPoolByAssetHash(int prefabHash, bool onlyClearUnused = false)
        {
            if (_gameObjPools.TryGetValue(prefabHash, out var pool))
            {
                if (onlyClearUnused)
                {
                    pool.ClearUnusedObjects();   
                    return true;
                }
                else
                {
                    pool.ClearAll();
                    return true;
                }
            }
            
            return false;
        }
        
        public void ClearAllUnusedObjects()
        {
            foreach (var poolPair in _gameObjPools)
            {
                poolPair.Value.ClearUnusedObjects();
            }
        }
        
        /// <summary>
        /// 清空所有对象池
        /// </summary>
        /// <param name="isDestroy">是否摧毁所有对象池，再次需要使用时将重新注册</param>
        public void ClearAllPools(bool isDestroy)
        {
            foreach (var pool in _gameObjPools)
            {
                pool.Value.ClearAll();
            }

            _gameObjRelations.Clear();
            
            if (isDestroy)
            {
                if (_cachedRoot)
                {
                    for (int i = _cachedRoot.childCount - 1; i >= 0; i--)
                    {
                        var child = _cachedRoot.GetChild(i).gameObject;
                        Destroy(child);
                    }
                }

                _prefabTemplates.Clear();
                _poolInfoList.Clear();
                _gameObjPools.Clear();
            }
        }
        
        private List<int> _tempRemovedObjectList = new List<int>();
        private void RemoveObjectsRelationByAssetHash(int assetHash)
        {
            foreach (var relation in _gameObjRelations)
            {
                if (relation.Value == assetHash)
                {
                    _tempRemovedObjectList.Add(relation.Key);
                }
            }

            foreach (var removeItem in _tempRemovedObjectList)
            {
                _gameObjRelations.Remove(removeItem);
            }
            
            _tempRemovedObjectList.Clear();
        }


        public RecyclablePoolConfig GetSimpleGOPoolConfig(GameObject prefabAsset, Func<GameObject> spawnFunc)
        {
            Assert.IsNotNull(prefabAsset);

            var poolConfig = new RecyclablePoolConfig()
            {
                ObjectType = RecycleObjectType.GameObject,
                ReferenceType = typeof(GameObject),
                PoolId = $"SimplePool_{prefabAsset.name}_{prefabAsset.GetInstanceID()}",
                SpawnFunc = spawnFunc,
            };

            return poolConfig;
        }
        
        
        private GameObject DefaultCreateObjectFunc(int prefabHash)
        {
            // 使用委托注册该本方法时_prefabTemplates不一定不一定含有该prefabHash，只有在真正调用该方法含有对应的prefabHash就OK了
            if (_prefabTemplates.TryGetValue(prefabHash, out var prefabAsset))
            {
                if (prefabAsset != null)
                {
                    var gameObj = Instantiate(prefabAsset);
                    return gameObj;
                }
            }
            
            Debug.LogError($"EasyPoolKit == Cannot create object: {prefabHash}");
            return null;
        }
    }
}