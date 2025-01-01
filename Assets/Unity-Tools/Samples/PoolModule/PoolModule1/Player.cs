using System;
using UnityEngine;

namespace Tools.PoolModule1.Sample
{
    public class Player : MonoBehaviour
    {
        public SimpleObjectPooler SimplePooler;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                // var bullet = SimpleObjectPooler.Instance.GetPooledGameObject();
                // bullet.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                var bullet =SimplePooler.GetPooledGameObject();
                bullet.SetActive(true);
            }
        }
    }
}