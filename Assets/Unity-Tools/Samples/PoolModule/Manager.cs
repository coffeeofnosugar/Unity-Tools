using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Tools.PoolModule.Sample
{
    public class Manager : MonoBehaviour
    {
        [ShowInInspector] public ObjectPool<Item> itemPool;
        [ShowInInspector] public readonly ItemFactory itemFactory = new();
        [ShowInInspector] public SingleFactory<Item> itemSingleFactory;
        [ShowInInspector] public readonly ObjectPoolFactoryString<Item> itemFactoryString = new("Assets/Unity-Tools/Samples/PoolModule/PoolModule2/ItemFactory2/Item_{0}.prefab", 1, 10);

        private async void Awake()
        {
            itemSingleFactory = new SingleFactory<Item>("Assets/Unity-Tools/Samples/PoolModule/PoolModule2/{0}.prefab");
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
        public ItemA itemA;
        public ItemB itemB;
        public List<Item> lastItem2A;
        public List<Item> lastItem2B;
        
        private async void Update()
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
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                itemPool.ReturnAll();
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
            else if (Input.GetKeyDown(KeyCode.C))
            {
                itemFactory.ReturnAll();
            }
            else if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                itemA = await itemSingleFactory.Get<ItemA>();
            }
            else if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                itemSingleFactory.Return(itemA);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                itemB = await itemSingleFactory.Get<ItemB>();
            }
            else if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                itemSingleFactory.Return(itemB);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                itemSingleFactory.ReturnAll();
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                Item temp = await itemFactoryString.Get("A");
                lastItem2A.Add(temp);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                itemFactoryString.Return(lastItem2A[^1]);
                lastItem2A.RemoveAt(lastItem2A.Count - 1);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                itemFactoryString.ReturnAll();
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                Item temp = await itemFactoryString.Get("B");
                lastItem2B.Add(temp);
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                itemFactoryString.Return(lastItem2B[^1]);
                lastItem2B.RemoveAt(lastItem2B.Count - 1);
            }
            else if (Input.GetKeyDown(KeyCode.Y))
            {
                itemFactoryString.ReturnAll();
            }
        }
    }
}