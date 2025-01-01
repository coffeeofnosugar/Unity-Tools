using UnityEngine;

namespace Tools.InterfaceReference.Sample
{
    public class Example : MonoBehaviour
    {
        public InterfaceReference<IDamageable> damageable;
        public InterfaceReference<IDamageable, MonoBehaviour> damageableMonoBehaviour;

        // private void Awake()
        // {
        //     damageable = GetComponent<DamageableComponent>();
        // }

        private void Start()
        {
            damageable.Value.Damage(10);

            IDamageable d = damageable.Value;
            d.Damage(20);
        }
    }
}