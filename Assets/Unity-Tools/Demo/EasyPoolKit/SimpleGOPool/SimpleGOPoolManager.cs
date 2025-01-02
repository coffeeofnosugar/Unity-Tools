using UnityEngine;

namespace Tools.EasyPoolKit.Demo
{
    public class SimpleGOPoolManager : MonoBehaviour
    {
        public GameObject prefab;
        private void Start()
        {
            var config = new RecyclablePoolConfig();
            config.SpawnFunc = () => Instantiate(prefab);
            config.ObjectType = RecycleObjectType.GameObject;
            config.ExtraArgs = new object[] { transform };
            config.InitCreateCount = 5;
            var pool = new SimpleGameObjectPool(config);
        }
    }
}
