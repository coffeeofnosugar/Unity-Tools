using System;
using UnityEngine;

namespace Tools.EasyPoolKit.Demo
{
    public class RecyclableGOPoolManager : MonoBehaviour
    {
        [SerializeField] private GameObject buttonPrefab;

        [SerializeField] private Transform parent;

        private void Awake()
        {
            var buttonConfig = new RecyclablePoolConfig()
            {
                ObjectType = RecycleObjectType.RecyclableGameObject,
                ReferenceType = typeof(ImageButton),
                PoolId = "ImageButtonPool",
                InitCreateCount = 5,
                ReachMaxLimitType = PoolReachMaxLimitType.RecycleOldest,
                MaxSpawnCount = 10,
                DespawnDestroyType = PoolDespawnDestroyType.DestroyToLimit,
                MaxDespawnCount = 5,
                ClearType = PoolClearType.ClearToLimit,
                IsIgnoreTimeScale = false,
            };
            RecyclableGOPoolKit.Instance.RegisterPrefab(buttonPrefab, buttonConfig);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                var newImage = RecyclableGOPoolKit.Instance.SimpleSpawn<ImageButton>(buttonPrefab);
                newImage.transform.SetParent(parent);
            }
            // else if (Input.GetKeyDown(KeyCode.S))
            // {
            //     RecyclableGOPoolKit.Instance.Despawn("Image");
            // }
            // else if (Input.GetKeyDown(KeyCode.C))
            // {
            //     RecyclableGOPoolKit.Instance.ClearAllPools(true);
            // }
            // else if (Input.GetKeyDown(KeyCode.Alpha1))
            // {
            //     var newImage = RecyclableGOPoolKit.Instance.Spawn("Cube");
            //     newImage.transform.SetParent(null);
            // }
            // else if (Input.GetKeyDown(KeyCode.Alpha2))
            // {
            //     RecyclableGOPoolKit.Instance.Despawn("Cube");
            // }
        }
    }
}