using Tools.PoolModule.Sample;
using UnityEngine;

namespace Tools.PoolModule.Sample_New
{
    public class Manager_New : MonoBehaviour
    {
        [SerializeField] private Item prefab;
        private ObjectPool<Item> _pool;

        private void Start()
        {
            _pool = new ObjectPool<Item>(prefab, 5, 10);
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var item = _pool.Get(2);
            }
        }
    }
}
