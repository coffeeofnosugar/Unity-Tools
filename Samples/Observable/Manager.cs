using System;
using UnityEngine;

namespace Tools.Observable.Sample
{
    public class Manager : MonoBehaviour
    {
        public Player player;
        
        private void OnEnable()
        {
            player.Health.OnValueChangedTo += HealthChange;
        }

        private void OnDisable()
        {
            player.Health.OnValueChangedTo -= HealthChange;
        }

        public void HealthChange(float value)
        {
            Debug.Log($"Health changed {value}");
        }
    }
}