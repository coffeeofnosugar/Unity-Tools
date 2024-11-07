using UnityEditor;
using UnityEngine;

namespace Tools.InterfaceReference.Sample
{
    public class DamageableComponent : MonoBehaviour, IDamageable
    {
        public void Damage(int amount)
        {
            Debug.Log($"DamageableComponent taken {amount} damage {name}");
        }
    }
}