using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.PoolModule
{
    public class ObjectPool : MonoBehaviour
    {
        [ReadOnly]
        public List<GameObject> PooledGameObjects;
    }
}