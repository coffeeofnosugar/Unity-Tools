using UnityEngine;

namespace Tools.PoolModule2.Sample
{
    public class Item : MonoBehaviour, IPoolable
    {
        public float health;
        
        public void Reset()
        {
            Debug.Log($"回收 {name}");
        }
    }
}