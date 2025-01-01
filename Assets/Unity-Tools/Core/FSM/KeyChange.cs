using System;

namespace Tools.FSM
{
    public struct KeyChange<TKey> : IDisposable
    {
        /************************************************************************************************************************/

        [ThreadStatic]
        private static KeyChange<TKey> _current;

        private IKeyedStateMachine<TKey> _stateMachine;
        private TKey _previousKey;
        private TKey _nextKey;

        /************************************************************************************************************************/

        /// <summary> 当前是否正在改变状态 </summary>
        public static bool IsActive => _current._stateMachine != null;

        /// <summary> 当前正在改变的状态机，如果当前没有正在改变的状态机，则为空 </summary>
        public static IKeyedStateMachine<TKey> StateMachine => _current._stateMachine;

        /************************************************************************************************************************/

        public static TKey PreviousKey
        {
            get
            {
#if UNITY_ASSERTIONS
                if (!IsActive)
                    throw new InvalidOperationException($"当前并没有在切换状态 {typeof(TKey)}, {typeof(StateMachine<,>)}");
#endif
                return _current._previousKey;
            }
        }

        public static TKey NextKey
        {
            get
            {
#if UNITY_ASSERTIONS
                if (!IsActive)
                    throw new InvalidOperationException($"当前并没有在切换状态 {typeof(TKey)}, {typeof(StateMachine<,>)}");
#endif
                return _current._nextKey;
            }
        }

        /************************************************************************************************************************/
        
        
        internal KeyChange(IKeyedStateMachine<TKey> stateMachine, TKey previousKey, TKey nextKey)
        {
            this = _current;

            _current._stateMachine = stateMachine;
            _current._previousKey = previousKey;
            _current._nextKey = nextKey;
        }

        /************************************************************************************************************************/
        
        public void Dispose()
        {
            _current = this;
        }

        /************************************************************************************************************************/

        public readonly override string ToString() => IsActive
            ? $"{nameof(KeyChange<TKey>)}<{typeof(TKey).FullName}" +
              $">({nameof(PreviousKey)}={PreviousKey}" +
              $", {nameof(NextKey)}={NextKey})"
            : $"{nameof(KeyChange<TKey>)}<{typeof(TKey).FullName}(Not Currently Active)";

        public static string CurrentToString()
            => _current.ToString();

        /************************************************************************************************************************/
    }
}