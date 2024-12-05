using Cysharp.Threading.Tasks;

namespace Tools.PoolModule2.Sample
{
    public class ItemFactory1 : ObjectPoolFactory1<Item>
    {
        public override async UniTask InitAsync()
        {
            string pathFormat = "Assets/Unity-Tools/Samples/PoolModule/PoolModule2/{0}.prefab";
            await CreatePool(string.Format(pathFormat, "ItemA"), 1, 10);
            await CreatePool(string.Format(pathFormat, "ItemB"), 2, 10);
        }
    }
}