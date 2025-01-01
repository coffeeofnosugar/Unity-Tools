using System;

namespace Tools.FSM
{
    /// <summary>
    /// 该结构体只有在转换状态的时候才会创建，并且在转换完成之后会销毁
    /// <para></para>
    /// 该结构体的作用：规范状态转换方法，在调用 <see cref="IState"/> 的方法时都必须在该结构体的作用域中。
    /// 换言之，可以在 <see cref="IState.CanEnterState"/>, <see cref="IState.CanExitState"/>, <see cref="IState.OnEnterState"/>和<see cref="IState.OnExitState"/>中访问当前状态和下一个状态
    /// <para></para>
    /// </summary>
    /// <remarks><strong>示例:</strong><code>
    /// 
    /// public class RunState : State
    /// {
    ///     StateMachine&lt;State&gt;.WithDefault _stateMachine;
    ///     public RunState(StateMachine&lt;State&gt;.WithDefault stateMachine)
    ///     {
    ///         _stateMachine = stateMachine;
    ///     }
    ///
    ///     // 当上一个状态不为AttackState时，才能进入本状态
    ///     public override bool CanEnterState =&gt; _stateMachine.PreviousState is not AttackState;
    ///
    ///     public override void OnEnterState()
    ///     {
    ///         // 两种访问方法都可以，但最好还是使用第一种
    ///         // 因为如果有多个角色在不同线程的同一时间转换状态的话，可能会读取到不是自己状态的StateChange&lt;TState&gt;
    ///         Debug.Log($"进入 {_stateMachine.NextState}");     // 输出：进入 RunState
    ///         Debug.Log($"进入 {StateChange&lt;State&gt;.NextState}");
    ///     }
    /// }
    ///
    /// </code></remarks>
    public struct StateChange<TState> : IDisposable
        where TState : class, IState
    {
        /************************************************************************************************************************/
        
        /// <summary>
        /// 线程静态成员的特点：
        /// <para></para>
        /// 1. 多个线程访问并改变 _current 的值时，每个线程看到的是它自己的 _current 副本，因此一个线程对 _current 的修改不会影响其他线程。
        /// <para></para>
        /// 2. 每个线程在其生命周期内对 _current 的任何修改只对其自身有效。当线程执行完毕后，该线程的 _current 副本就会被销毁。
        /// <para></para>
        /// 3. 当所有线程都执行完毕后，_current 的最终值取决于最后一个修改它的线程的状态，如果没有任何线程正在进行状态更改 _current 将保持其默认值（通常是 null 或者初始状态）。
        /// </summary>
        /// <remarks> 不过Unity是单线程，一般都不会碰到多线程，所以这个都不重要，重要的是 _current 与 this 的配合可以确保在嵌套改变状态时，能正常切换回上一层
        /// <para></para>
        /// <strong>假设当前是从 FallState 进入到 RunState，示例:</strong><code>
        ///
        /// public class RunState : State
        /// {
        ///     // 在进入 RunState 时，可能需要判断是否应该返回 DefaultState 或者进入 HitState
        ///     public override void OnEnterState()         // 第一层嵌套，FallState -> RunState
        ///     {
        ///         Debug.Log($"第一层嵌套开始 {_stateMachine.NextState}");    // 输出：第一层嵌套开始 RunState
        ///
        ///         if (true)
        ///         {
        ///             _stateMachine.TrySetDefaultState();     // 第二层嵌套一号，RunState -> DefaultState
        ///         }
        /// 
        ///         Debug.Log($"第二层嵌套结束，继续第一次嵌套 {_stateMachine.NextState}");    // 输出：第二层嵌套结束，继续第一次嵌套 RunState
        ///
        ///         if (true)
        ///         { 
        ///             _stateMachine.TrySetState(_brain.hitState);     // 第二层嵌套二号，RunState -> HitState
        ///         }
        ///     }
        /// }
        /// 
        /// </code><para></para>则上面的嵌套可以表示成：<code>
        /// 
        /// using (new StateChange&lt;TState&gt;(stateMachine, FallState, RunState))
        /// {
        ///     using (new StateChange&lt;TState&gt;(stateMachine, RunState, DefaultState))
        ///     {
        ///     }
        /// 
        ///     using (new StateChange&lt;TState&gt;(stateMachine, RunState, HitState))
        ///     {
        ///     }
        /// }
        ///
        /// </code>
        /// <para></para>
        /// 若以上状态都满足进入和退出的条件，则最终会进入 HitState
        /// <para></para>
        /// 注意：这里为了展示代码并不规范，如果成功进入了其他状态，则应该使用`return`退出嵌套，不再进行后续同层级的判断
        /// </remarks>
        [ThreadStatic]  // 标记为线程静态
        public static StateChange<TState> _current;

        private StateMachine<TState> _stateMachine;
        private TState _previousState;
        private TState _nextState;
        
        /************************************************************************************************************************/
        
        /// <summary> 当前是否正在改变状态 </summary>
        public static bool IsActive => _current._stateMachine != null;
        
        /// <summary> 当前正在改变的状态机，如果当前没有正在改变的状态机，则为空 </summary>
        public static StateMachine<TState> StateMachine => _current._stateMachine;
        
        /************************************************************************************************************************/
        
        public static TState PreviousState
        {
            get
            {
#if UNITY_ASSERTIONS
                if (!IsActive)
                    throw new InvalidOperationException($"当前并没有在切换状态 {typeof(TState)}, {typeof(StateMachine<>)}");
#endif
                return _current._previousState;
            }
        }
        
        
        public static TState NextState
        {
            get
            {
#if UNITY_ASSERTIONS
                if (!IsActive)
                    throw new InvalidOperationException($"当前并没有在切换状态 {typeof(TState)}, {typeof(StateMachine<>)}");
#endif
                return _current._nextState;
            }
        }
        
        /************************************************************************************************************************/
        
        internal StateChange(StateMachine<TState> stateMachine, TState previousState, TState nextState)
        {
            this = _current;    // 嵌套循环中上一层的 StateChange 暂存在 this 中，方便在 using 结束时复原

            _current._stateMachine = stateMachine;
            _current._previousState = previousState;
            _current._nextState = nextState;
        }
        
        /************************************************************************************************************************/

        public void Dispose()
        {
            _current = this;        // 在退出 using 时复原嵌套循环上一层的 StateChange
        }
        
        /************************************************************************************************************************/

        public readonly override string ToString()
            => IsActive
                ? $"{nameof(StateChange<TState>)}<{typeof(TState).FullName}" +
                  $">({nameof(PreviousState)}='{_previousState}'" +
                  $", {nameof(NextState)}='{_nextState}')"
                : $"{nameof(StateChange<TState>)}<{typeof(TState).FullName}(Not Currently Active)";

        public static string CurrentToString()
            => _current.ToString();

        /************************************************************************************************************************/
    }
}