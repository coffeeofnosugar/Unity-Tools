using System;
using UnityEngine;

namespace Tools.EventBus.Sample
{
    public struct SampleEvent
    {
        public string message;
        
        private static SampleEvent e;
        public static void Trigger(string message)
        {
            e.message = message;
            EventBus.TriggerEvent(e);
        }
    }
    
    public class Listener : MonoBehaviour,
        IEventListener<SampleEvent>
    {
        private void OnEnable()
        {
            this.EventStartListening<SampleEvent>();
        }

        private void OnDisable()
        {
            this.EventStopListening<SampleEvent>();
        }

        public void OnEvent(SampleEvent eventType)
        {
            Debug.Log($"触发事件：{eventType.message}");
        }
    }
}