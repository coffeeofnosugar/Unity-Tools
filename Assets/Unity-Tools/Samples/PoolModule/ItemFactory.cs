using Cysharp.Threading.Tasks;

namespace Tools.PoolModule.Sample
{
    public class ItemFactory : ObjectPoolFactory<Item>
    {
        public override async UniTask InitAsync()
        {
            string pathFormat = "Assets/Unity-Tools/Samples/PoolModule/PoolModule2/{0}.prefab";
            await CreatePool(string.Format(pathFormat, "ItemA"), 5, 10);
            await CreatePool(string.Format(pathFormat, "ItemB"), 2, 10);
        }
    }
}