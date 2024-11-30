using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Tools.PoolModule2.Sample
{
    public class Manager : MonoBehaviour
    {
        public ObjectPool<Item> itemPool;
        [ShowInInspector] public readonly ItemFactory itemFactory = new();

        private async void Awake()
        {
            string pathFormat = "Assets/Unity-Tools/Samples/PoolModule/PoolModule2/{0}.prefab";
            GameObject itemObj = await Addressables.LoadAssetAsync<GameObject>(string.Format(pathFormat, "Item"));
            Item item = itemObj.GetComponent<Item>();
            itemPool = new ObjectPool<Item>(item, 3, 10);
            Addressables.Release(itemObj);  // 可直接释放资源

            await itemFactory.InitAsync();
        }

        public List<Item> lastItem;
        public List<ItemA> lastItemA;
        public List<ItemB> lastItemB;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                lastItem.Add(itemPool.Get());
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (lastItem.Count == 0) return;
                Item item = lastItem[^1];
                lastItem.RemoveAt(lastItem.Count - 1);
                itemPool.Return(item);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                lastItemA.Add(itemFactory.Get<ItemA>());
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                if (lastItemA.Count == 0) return;
                ItemA item = lastItemA[^1];
                lastItemA.RemoveAt(lastItemA.Count - 1);
                itemFactory.Return(item);
            }
            else if (Input.GetKeyDown(KeyCode.B))
            {
                lastItemB.Add(itemFactory.Get<ItemB>());
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                if (lastItemB.Count == 0) return;
                ItemB item = lastItemB[^1];
                lastItemB.RemoveAt(lastItemB.Count - 1);
                itemFactory.Return(item);
            }
        }
    }
}