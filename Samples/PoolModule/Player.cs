using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tools.PoolModule.Sample
{
    public class Player : MonoBehaviour
    {
        public SimpleObjectPooler SimplePooler;

        private void Update()
        {
            if (Keyboard.current.aKey.wasPressedThisFrame)
            {
                // var bullet = SimpleObjectPooler.Instance.GetPooledGameObject();
                // bullet.SetActive(true);
            }

            if (Keyboard.current.sKey.wasPressedThisFrame)
            {
                var bullet =SimplePooler.GetPooledGameObject();
                bullet.SetActive(true);
            }
        }
    }
}