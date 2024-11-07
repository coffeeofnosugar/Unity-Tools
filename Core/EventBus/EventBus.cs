using System.Collections.Generic;
using UnityEngine;

namespace Tools.EventBus
{
    public static class EventBus<T> where T : IEvent
    {
        private static readonly HashSet<IEventBinding<T>> bindings = new();
        
        public static void Register(EventBinding<T> binding) => bindings.Add(binding);
        public static void Deregister(EventBinding<T> binding) => bindings.Remove(binding);

        public static void Raise(T @event)
        {
            foreach (var binding in bindings)
            {
                binding.OnEventNoArgs?.Invoke();
                binding.OnEvent?.Invoke(@event);
            }
        }

        public static void Clear()
        {
            Debug.Log($"Clearing {typeof(T).Name} event bus.");
            bindings.Clear();
        }
    }
}