using System;
using UnityEngine;

namespace Tools.EventBus.Sample
{
    public class Trigger : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SampleEvent.Trigger("Hello Unity!");
            }
        }
    }
}