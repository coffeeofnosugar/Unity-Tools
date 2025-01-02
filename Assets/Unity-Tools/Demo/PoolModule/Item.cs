using System;
using UnityEngine;

namespace Tools.PoolModule.Sample
{
    public class Item : MonoBehaviour, IPoolable, IPoolableString
    {
        public float health;
        public virtual string Name => name;

        public void OnGet(params object[] args)
        {
            Debug.Log(args.Length > 0 ? args[0] : "No args");
        }

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