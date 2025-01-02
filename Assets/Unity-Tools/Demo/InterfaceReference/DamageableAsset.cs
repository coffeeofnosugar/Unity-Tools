using UnityEngine;

namespace Tools.InterfaceReference.Sample
{
    [CreateAssetMenu(fileName = "DamageableAsset", menuName = "SerializeInterface/Example/IDamageable")]
    public class DamageableAsset : ScriptableObject, IDamageable
    {
        public void Damage(int amount)
        {
            Debug.Log($"DamageableAsset taken {amount} damage {name}");
        }
    }
}