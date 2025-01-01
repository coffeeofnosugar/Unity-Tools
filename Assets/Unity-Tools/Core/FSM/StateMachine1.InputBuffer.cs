namespace Tools.FSM
{
    public partial class StateMachine<TState>
    {
        /// <summary> 该类的作用只是为了简化初始化，美观一点 </summary>
        public class InputBuffer : InputBuffer<StateMachine<TState>>
        {
            public InputBuffer(StateMachine<TState> stateMachine) : base(stateMachine) { }
        }
        
        /// <summary>
        /// 输入缓冲器：在触发缓存器后的几秒内，每帧会都尝试进入目标状态
        /// <para></para>
        /// </summary>
        /// <remarks>
        /// <strong>示例:</strong><code>
        ///
        /// private void Awake()
        /// {
        ///     inputBuffer = new StateMachine&lt;State&gt;.InputBuffer(stateMachine);      // 初始化缓冲器
        /// }
        ///
        /// private void Update()
        /// {
        ///     inputBuffer.Update();       // 在Update中更新缓冲器
        ///     ChangeState();              // 监听输入
        /// }
        ///
        /// private void ChangeState()
        /// {
        ///     if (Input.GetKeyDown(KeyCode.Space))
        ///     {
        ///         // 在按下空格后的两秒内，每帧都会尝试进入runState，进入规则默认为：当前状态可退出，且目标状态可进入
        ///         inputBuffer.Buffer(runState, 2);
        ///     }
        /// }
        /// 
        /// </code></remarks>
        public class InputBuffer<TStateMachine> where TStateMachine : StateMachine<TState>
        {
            /************************************************************************************************************************/
            
            protected readonly TStateMachine _stateMachine;
            /// <summary>
            /// 如果是<see cref="WithDefault"/>状态机，则获取<see cref="WithDefault.ForceSetDefaultState"/>的引用，方便类中其他方法访问
            /// <para></para>
            /// 如果不是<see cref="WithDefault"/>状态机，则不做处理
            /// </summary>
            private System.Action _forceDefaultState;
            private System.Action _succeedAction;
            private System.Func<bool> _condition;
            
            public TState State { get; set; }
            public float TimeOut { get; set; }
            public bool IsActive => State != null;
            
            /************************************************************************************************************************/
            
            public InputBuffer(TStateMachine stateMachine)
            {
                _stateMachine = stateMachine;
                TryRegisterForceSetDefaultState();
            }

            /// <summary>
            /// 为防止被<see cref="WithDefault.ForceSetDefaultState"/>偷跑，将其改为先进入<see cref="State"/>，再进入默认状态
            /// </summary>
            private void TryRegisterForceSetDefaultState()
            {
                if (_stateMachine is WithDefault withDefault)
                {
                    _forceDefaultState = withDefault.ForceSetDefaultState;
                    withDefault.ForceSetDefaultState = TryEnterStateOrForceDefault;
                }
            }

            /// <summary> 进入缓冲目标状态的规则 </summary>
            protected virtual bool TryEnterState() => _stateMachine.TryResetState(State);
            
            public void TryEnterStateOrForceDefault()
            {
                if (IsActive &&
                    TryEnterState())
                    return;
                
                _forceDefaultState();
            }
            
            /************************************************************************************************************************/

            /// <summary>
            /// 开始缓冲<see cref="TryEnterState"/>进入目标状态
            /// </summary>
            /// <param name="state">目标状态</param>
            /// <param name="timeOut">缓冲持续时间</param>
            /// <param name="condition"></param>
            /// <param name="succeedAction">成功后执行事件，若超时则直接释放掉</param>
            /// <remarks> 你可能会好奇，明明已经有<see cref="IState.OnEnterState"/>了，为什么还需要特意传递一个事件：
            /// <para></para>
            /// 如玩家跳跃，在玩家起跳时需要给玩家一个向上的速度，然后进入浮空状态。
            /// 但是并不是每次进入浮空状态都需要给玩家一个向上的速度（如玩家从悬崖跌落），所以并没有在浮空状态的OnEnterState中提供向上的速度。
            /// 所以我们需要给缓冲器提供一个成功进入的委托事件。
            /// <para></para>
            /// 当然，除了给缓冲器提供委托事件外，还有其他的实现方法，如：
            /// 方法一：设置一个公共bool值，记录玩家是否是通过输入键主动进入浮空状态。然后在进入浮空状态时，根据该bool值来决定是否给玩家一个向上的速度。
            /// 方法二：可以将浮空拆分为多个状态，如起跳状态、下落状态等。玩家进入起跳状态时，提供一个向上的速度
            /// </remarks>
            public void Buffer(TState state, float timeOut, System.Func<bool> condition = default, System.Action succeedAction = default)
            {
                State = state;
                TimeOut = timeOut;
                _condition = condition;
                _succeedAction = succeedAction;
            }
            
            /************************************************************************************************************************/
            
            public bool Update() => Update(UnityEngine.Time.deltaTime);
            public bool Update(float deltaTime)     // 若间隔不为Time.deltaTime时可以使用该方法
            {
                if (IsActive)
                {
                    bool isConditionMet = _condition?.Invoke() ?? true;
                    if (isConditionMet && TryEnterState())
                    {
                        _succeedAction.Invoke();
                        Clear();
                        return true;
                    }
                    else
                    {
                        TimeOut -= deltaTime;

                        if (TimeOut < 0)        // 如果超时就结束缓冲
                            Clear();
                    }
                }

                return false;
            }
            
            public virtual void Clear()
            {
                State = null;
                TimeOut = default;
                _succeedAction = null;
            }
            
            /************************************************************************************************************************/
        }
    }
}