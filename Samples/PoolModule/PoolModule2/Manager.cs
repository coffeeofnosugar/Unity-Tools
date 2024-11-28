using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Tools.PoolModule2.Sample
{
    public class Manager : MonoBehaviour
    {
        public ObjectPool<Item> itemPool;

        private async void Awake()
        {
            GameObject itemObj = await Addressables.LoadAssetAsync<GameObject>("Assets/Unity-Tools/Samples/PoolModule/PoolModule2/Item.prefab");
            Item item = itemObj.GetComponent<Item>();
            itemPool = new ObjectPool<Item>(item, 1, 10);
            Addressables.Release(itemObj);  // 可直接释放资源
        }

        public List<Item> lastItem;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                lastItem.Add(itemPool.Get());
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                if (lastItem.Count == 0) return;
                Item item = lastItem[^1];
                lastItem.RemoveAt(lastItem.Count - 1);
                itemPool.Return(item);
            }
        }
    }
}