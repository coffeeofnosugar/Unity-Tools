using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Tools.FSM
{
    public partial class StateMachine<TState>
    {
        /// <summary>
        /// 附带默认状态的状态机，可以在需要的时候使用 <see cref="TrySetDefaultState"/> 或 <see cref="TryResetDefaultState"/>进入默认状态
        /// <para></para>
        /// </summary>
        /// <remarks><strong>示例:</strong><code>
        ///
        /// // 初始化状态机
        /// private StateMachine&lt;State&gt;.WithDefault stateMachine = new();
        ///
        /// private IdleState idleState;
        /// private RunState runState;
        ///
        /// private void Awake()
        /// {
        ///     // 初始化状态
        ///     idleState = new IdleState(stateMachine);
        ///     runState = new RunState(stateMachine);
        ///
        ///     // 设置并进入idleState状态
        ///     stateMachine.Initialize(idleState);
        /// }
        ///
        /// </code></remarks>
        [System.Serializable]
        // [InlineProperty(LabelWidth = 90)]
        [FoldoutGroup("State Machine"), HideLabel]
        public class WithDefault : StateMachine<TState>
        {
            /************************************************************************************************************************/
            [ReadOnly, HideInEditorMode]
            [SerializeField] private TState _defaultState;

            public System.Action ForceSetDefaultState;
            
            /************************************************************************************************************************/
            
            public override void InitializeAfterDeserialize()
            {
                if (_currentState == null) return;
                _defaultState = _currentState;
                ForceSetDefaultState = () => ForceSetState(_defaultState);
                using (new StateChange<TState>(this, null, CurrentState))
                    CurrentState.OnEnterState();
            }

            /************************************************************************************************************************/

            public bool TrySetDefaultState() => TrySetState(_defaultState);
            public bool TryResetDefaultState() => TryResetState(_defaultState);
            
            /************************************************************************************************************************/
        }
    }
}