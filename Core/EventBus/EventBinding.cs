using System;

namespace Tools.EventBus
{
    internal interface IEventBinding<T>
    {
        public Action OnEventNoArgs { get; }
        public Action<T> OnEvent { get; }
    }
    
    /// <summary>
    /// 绑定事件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventBinding<T> : IEventBinding<T> where T : IEvent
    {
        Action onEventNoArgs = default;
        Action<T> onEvent = default;

        public EventBinding(Action onEventNoArgs) => this.onEventNoArgs = onEventNoArgs;
        public EventBinding(Action<T> onEvent) => this.onEvent = onEvent;
        
        public void Add(Action onEvent) => this.onEventNoArgs += onEvent;
        public void Remove(Action onEvent) => this.onEventNoArgs -= onEvent;

        public void Add(Action<T> onEvent) => this.onEvent += onEvent;
        public void Remove(Action<T> onEvent) => this.onEvent -= onEvent;
        
        
        Action IEventBinding<T>.OnEventNoArgs => onEventNoArgs;
        Action<T> IEventBinding<T>.OnEvent => onEvent;
    }
}