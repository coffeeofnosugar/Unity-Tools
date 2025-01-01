using UnityEngine;

namespace Tools.PoolModule1.Sample
{
    public class Bullet : MonoBehaviour
    {
        [Knob(0f, 100f)]
        public float speed = 10f;
        public Vector3 direction = Vector3.forward;
        public float lifeTime = 10f;

        private void OnEnable()
        {
            transform.position = new Vector3(Random.Range(-5f, 5f), 0f, 0f);
            Invoke(nameof(DelayDisable), lifeTime);
        }

        private void FixedUpdate()
        {
            transform.position += direction * speed * Time.fixedDeltaTime;
        }

        private void DelayDisable()
        {
            gameObject.SetActive(false);
        }
    }
}