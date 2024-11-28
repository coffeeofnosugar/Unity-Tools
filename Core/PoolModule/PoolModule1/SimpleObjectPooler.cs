using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tools.PoolModule1
{
    /// <summary>
    /// 一个实例化SimpleObjectPooler对象只能创建一种物体，也就是说如果只使用单例访问该对象池，那么只能创建一个物体
    /// 除非实例化多个SimpleObjectPooler对象，才可以创建多个物体
    /// </summary>
    /// <example>
    /// 一般<see cref="ObjectPooler.MutualizeWaitingPools"/>设置为true
    /// <see cref="ObjectPooler.NestWaitingPool"/>设置为true
    /// <see cref="ObjectPooler.NestUnderThis"/>设置为false
    /// <see cref="PoolCanExpand"/>设置为true
    /// </example>
    public class SimpleObjectPooler : ObjectPooler
    {
        /// 母体
        [DisableInPlayMode]
        public GameObject GameObjectToPool;
        /// 对象池大小
        [DisableInPlayMode]
        public int PoolSize = 20;
        /// true：当物体全部被取出，而继续获取物体时，允许对象池扩容
        [DisableInPlayMode]
        public bool PoolCanExpand = true;

        /// <summary> 填充对象池 </summary>
        protected override void FillObjectPool()
        {
            if (GameObjectToPool == null) return;
            // 如果已经创建了对象池，则退出
            if (_objectPool != null && _objectPool.PooledGameObjects.Count > PoolSize) return;
            
            CreateWaitingPool();

            int objectsTpSpawn = _objectPool == null ? PoolSize : PoolSize - _objectPool.PooledGameObjects.Count;
            for (int i = 0; i < objectsTpSpawn; i++)
            {
                AddOneObjectToThePool();
            }
        }

        protected override string DetermineObjectPoolName() => $"[SimpleObjectPooler] {GameObjectToPool.name}";

        public override GameObject GetPooledGameObject()
        {
            // 在待机池中寻找一个空闲的对象
            foreach (var o in _objectPool.PooledGameObjects.Where(o => !o.activeInHierarchy))
            {
                return o;
            }
            // 如果没有空闲的对象，并且允许扩容，则创建一个新对象
            if (PoolCanExpand)
            {
                return AddOneObjectToThePool();
            }
            // 没有空闲的对象，并且不允许扩容，则返回null
            return null;
        }

        /// <summary>
        /// 添加一个新对象到对象池
        /// </summary>
        private GameObject AddOneObjectToThePool()
        {
            if (GameObjectToPool == null) throw new System.NullReferenceException("GameObjectToPool is null");

            bool initialStatus = GameObjectToPool.activeSelf;
            GameObjectToPool.SetActive(false);
            GameObject newGameObject = Instantiate(GameObjectToPool);
            GameObjectToPool.SetActive(initialStatus);
            SceneManager.MoveGameObjectToScene(newGameObject, this.gameObject.scene);
            if (NestWaitingPool)
                newGameObject.transform.SetParent(_waitingPool.transform);
            newGameObject.name = $"{GameObjectToPool.name}-{_objectPool.PooledGameObjects.Count}";
            _objectPool.PooledGameObjects.Add(newGameObject);
            return newGameObject;
        }
    }
}