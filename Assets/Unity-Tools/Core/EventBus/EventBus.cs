using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tools.EventBus
{
    public interface IEventListenerBase { };

    public interface IEventListener<T> : IEventListenerBase
    {
        void OnEvent(T eventType);
    }

    public static class EventRegister
    {
        public delegate void Delegate<T>(T eventType);

        public static void EventStartListening<TEventType>(this IEventListener<TEventType> caller)
            where TEventType : struct
        {
            EventBus.AddListener<TEventType>(caller);
        }

        public static void EventStopListening<TEventType>(this IEventListener<TEventType> caller)
            where TEventType : struct
        {
            EventBus.RemoveListener<TEventType>(caller);
        }
    }
    
    [ExecuteAlways]
    public static class EventBus
    {
        private static Dictionary<Type, List<IEventListenerBase>> _subscribersList;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitializeStatics()
        {
            _subscribersList = new Dictionary<Type, List<IEventListenerBase>>();
        }

        static EventBus()
        {
            _subscribersList = new Dictionary<Type, List<IEventListenerBase>>();
        }

        public static void AddListener<TEvent>(IEventListener<TEvent> listener) where TEvent : struct
        {
            Type eventType = typeof(TEvent);

            if (!_subscribersList.ContainsKey(eventType))
            {
                _subscribersList[eventType] = new List<IEventListenerBase>();
            }

            if (!_subscribersList[eventType].Contains(listener))
            {
                _subscribersList[eventType].Add(listener);
            }
        }

        public static void RemoveListener<TEvent>(IEventListener<TEvent> listener) where TEvent : struct
        {
            Type eventType = typeof(TEvent);
            if (_subscribersList.TryGetValue(eventType, out var subscriberList))
            {
                subscriberList.Remove(listener);

                if (subscriberList.Count == 0)
                {
                    _subscribersList.Remove(eventType);
                }
            }
        }

        public static void TriggerEvent<TEvent>(TEvent newEvent) where TEvent : struct
        {
            if (!_subscribersList.TryGetValue(typeof(TEvent), out var list))
                return;

            for (int i = list.Count - 1; i >= 0; i--)
            {
                (list[i] as IEventListener<TEvent>)?.OnEvent(newEvent);
            }
        }
    }
}