using UnityEngine;

namespace Tools.Observable.Sample
{
    public class Player : MonoBehaviour
    {
        public Observable<float> Health;

        private void Awake()
        {
            // Health = 10;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                Health.Value--;
            }
        }
    }
}