using System.Collections.Generic;

namespace Tools.FSM
{
    public interface IStateMachine
    {
        IState CurrentState { get; }
        IState PreviousState { get; }
        IState NextState { get; }
        bool CanSetState(IState state);
        IState CanSetState(IList<IState> states);
        bool TrySetState(IState state);
        bool TrySetState(IList<IState> states);
        bool TryResetState(IState state);
        bool TryResetState(IList<IState> states);
        void ForceSetState(IState state);
    }
    
    /// <summary> 使用方法参考<see cref="WithDefault"/> </summary>
    [System.Serializable]
    public partial class StateMachine<TState> : IStateMachine
        where TState : class, IState
    {
        /************************************************************************************************************************/
        
        [UnityEngine.SerializeField]
        protected TState _currentState;
        public TState CurrentState => _currentState;    // 这么处理主要是为了能在 Inspector窗口 上实例化，实质上可以简单的写成一个属性: TState CurrentState { get; private set; };
        public TState PreviousState => StateChange<TState>.PreviousState;
        public TState NextState => StateChange<TState>.NextState;

        /************************************************************************************************************************/
        
        /// <summary> 初始化——进入初始状态 </summary>
        public virtual void InitializeAfterDeserialize()
        {
            if (_currentState == null) return;
            using (new StateChange<TState>(this, null, CurrentState))
                CurrentState.OnEnterState();
        }
        
        /************************************************************************************************************************/

        /// <summary> 判断当前状态是否能退出，目标状态是否能进入 </summary>
        /// <returns> true: 当前状态能退出，且目标状态能进入 </returns>
        /// <remarks> 一般用作在类的内部判断 </remarks>
        public bool CanSetState(TState state)
        {
            using (new StateChange<TState>(this, CurrentState, state))
            {
                if (CurrentState != null && !CurrentState.CanExitState)
                    return false;

                if (state != null && !state.CanEnterState)
                    return false;

                return true;
            }
        }
        
        /// <summary> 判断列表中是否有符合条件的状态 </summary>
        /// <remarks>
        /// 这里简单看过去可能觉得能优化代码减少判断，比如只用判断一次当前状态是否能退出
        /// <para></para>
        /// 但其实里面的判断一个不能少，不能优化。例如，当前状态的 <see cref="IState.CanExitState"/> 里有关于 <see cref="PreviousState"/> 的判断
        /// </remarks>
        public TState CanSetState(IList<TState> states)
        {
            var count = states.Count;
            for (int i = 0; i < count; i++)
            {
                var state = states[i];
                if (CanSetState(state))
                    return state;
            }

            return null;
        }
        
        /************************************************************************************************************************/

        /// <summary> 若当前状态能退出，且目标状态能进入，则进入目标状态  </summary>
        /// <returns> 如果目标状态为当前状态，直接返回true，即不能再次进入自己 </returns>
        public bool TrySetState(TState state)
        {
            if (CurrentState == state)
                return true;
            
            return TryResetState(state);
        }
        
        /// <summary> 以 <see cref="TrySetState(TState)"/> 的规则按索引号顺序进入目标状态，即目标状态不能是当前状态 </summary>
        /// <remarks> 循环执行到当前目标（如果有的话）会直接退出 </remarks>
        public bool TrySetState(IList<TState> states)
        {
            var count = states.Count;
            for (int i = 0; i < count; i++)
                if (TrySetState(states[i]))
                    return true;
            
            return false;
        }
        
        /************************************************************************************************************************/

        /// <summary> 若当前状态能退出，且目标状态能进入，则进入目标状态 </summary>
        /// <param name="state"> 当目标状态为当前状态时，会重复进入该状态，即会再次执行一遍<see cref="IState.OnEnterState"/> </param>
        public bool TryResetState(TState state)
        {
            if (!CanSetState(state))
                return false;

            ForceSetState(state);
            return true;
        }
        
        /// <summary> 以 <see cref="TryResetState(TState)"/> 的规则按索引号顺序进入目标状态，即目标状态可以是当前状态 </summary>
        public bool TryResetState(IList<TState> states)
        {
            var count = states.Count;
            for (int i = 0; i < count; i++)
                if (TryResetState(states[i]))
                    return true;

            return false;
        }
        
        /************************************************************************************************************************/

        /// <summary> 强制进入目标状态，无论当前状态、目标状态是否满足条件 </summary>
        public void ForceSetState(TState state)
        {
#if UNITY_ASSERTIONS
            if (state == null)
            {
                throw new System.ArgumentNullException(nameof(state), "不允许为空");
            }
#endif
            using (new StateChange<TState>(this, CurrentState, state))
            {
                CurrentState?.OnExitState();

                _currentState = state;

                state?.OnEnterState();
            }
        }

        /************************************************************************************************************************/
        #region IStateMachine
        /************************************************************************************************************************/
        
        IState IStateMachine.CurrentState => CurrentState;
        IState IStateMachine.PreviousState => PreviousState;
        IState IStateMachine.NextState => NextState;

        bool IStateMachine.CanSetState(IState state) => CanSetState((TState)state);
        IState IStateMachine.CanSetState(IList<IState> states) => CanSetState((IList<TState>)states);

        bool IStateMachine.TrySetState(IState state) => TrySetState((TState)state);
        bool IStateMachine.TrySetState(IList<IState> states) => TrySetState((IList<TState>)states);

        bool IStateMachine.TryResetState(IState state) => TryResetState((TState)state);
        bool IStateMachine.TryResetState(IList<IState> states) => TryResetState((IList<TState>)states);

        void IStateMachine.ForceSetState(IState state) => ForceSetState((TState)state);

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
    }
}