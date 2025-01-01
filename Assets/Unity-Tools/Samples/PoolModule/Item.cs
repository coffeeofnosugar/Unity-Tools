using System;
using UnityEngine;

namespace Tools.PoolModule.Sample
{
    public class Item : MonoBehaviour, IPoolable, IPoolableString
    {
        public float health;
        public string name;
        public virtual string Name => name;

        public void OnGet() { }

        public void OnReturn() { }

#if UNITY_EDITOR
        
        private void OnValidate()
        {
            // if (gameObject.IsPrefabDefinition())
            {
                
            }
            name = gameObject.name[^1].ToString();
        }
#endif
    }
}