using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Tools.FSM
{
    public partial class StateMachine<TKey, TState>
    {
        /// <summary>
        /// 附带默认状态的状态机，可以在需要的时候使用 <see cref="TrySetDefaultState"/> 或 <see cref="TryResetDefaultState"/>进入默认状态
        /// <para></para>
        /// </summary>
        /// <remarks><strong>示例:</strong><code>
        ///
        /// public class Brain3 : MonoBehaviour
        /// {
        ///     public enum StateEnum { Idle, Run, Attack, Hit }
        ///     public StateMachine&lt;StateEnum, StateBehaviour&gt;.WithDefault stateMachine = new();
        ///
        ///     // 序列化获取状态 
        ///     public StateBehaviour[] states;
        ///     private AttackState attackState;
        /// 
        ///     private void Start()
        ///     {
        ///         stateMachine.AddRange(new [] { StateEnum.Idle, StateEnum.Run }, states);    // 批量添加
        ///         stateMachine.Add(StateEnum.Attack, attackState);                            // 也可以一个一个添加
        ///         
        ///         stateMachine.InitializeAfterDeserialize(StateEnum.Idle);
        ///     }
        /// }
        /// 
        /// </code></remarks>
        [System.Serializable]
        // [InlineProperty(LabelWidth = 90)]
        [FoldoutGroup("State Machine"), HideLabel]
        public new class WithDefault : StateMachine<TKey, TState>
        {
            /************************************************************************************************************************/
            [ReadOnly, HideInEditorMode]
            [SerializeField] private TKey _defaultKey;

            public System.Action ForceSetDefaultState;
            
            /************************************************************************************************************************/

            public override void InitializeAfterDeserialize()
            {
                if (_currentState != null)
                {
                    _defaultKey = _currentKey;
                    ForceSetDefaultState = () => ForceSetState(_defaultKey);
                    using (new KeyChange<TKey>(this, default, _currentKey))
                    using (new StateChange<TState>(this, null, CurrentState))
                        CurrentState.OnEnterState();
                }
                else if (Dictionary.TryGetValue(_currentKey, out var state))
                {
                    ForceSetDefaultState = () => ForceSetState(_defaultKey);
                    ForceSetState(state);
                }
                else
                {
                    throw new System.ArgumentException($"{GetType().FullName} 并未注册 {_currentKey} 状态，初始化失败");
                }
            }

            /************************************************************************************************************************/

            public bool TrySetDefaultState() => TrySetState(_defaultKey);
            public bool TryResetDefaultState() => TryResetState(_defaultKey);
            
            /************************************************************************************************************************/
        }
    }
}