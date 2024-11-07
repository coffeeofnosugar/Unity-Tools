using System.Collections.Generic;

namespace Tools.FSM
{
    partial class StateMachine<TKey, TState>
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
        public new class WithDefault : StateMachine<TKey, TState>
        {
            /************************************************************************************************************************/
            [UnityEngine.SerializeField]
            private TKey DefaultKey;

            public System.Action ForceSetDefaultState;
            
            /************************************************************************************************************************/

            public override void InitializeAfterDeserialize(TState defaultState)
            {
                foreach (KeyValuePair<TKey,TState> pair in Dictionary)
                {
                    if (defaultState == pair.Value)
                    {
                        DefaultKey = pair.Key;
                        ForceSetDefaultState = () => ForceSetState(pair.Key);
                        
                        _currentKey = pair.Key;
                        _currentState = defaultState;
                        using (new KeyChange<TKey>(this, default, _currentKey))
                        using (new StateChange<TState>(this, null, CurrentState))
                            CurrentState.OnEnterState();
                        return;
                    }
                }
                throw new System.ArgumentException($"{GetType().FullName} 并未注册 {defaultState} 状态");
            }


            public override void InitializeAfterDeserialize(TKey defaultKey)
            {
                if (!Dictionary.TryGetValue(defaultKey, out var state)) throw new System.ArgumentException($"{GetType().FullName} 并未注册 {defaultKey} 状态");

                DefaultKey = defaultKey;
                ForceSetDefaultState = () => ForceSetState(defaultKey);

                _currentKey = defaultKey;
                _currentState = state;
                using (new KeyChange<TKey>(this, default, _currentKey))
                using (new StateChange<TState>(this, null, CurrentState))
                    CurrentState.OnEnterState();
            }
            
            /************************************************************************************************************************/

            public bool TrySetDefaultState() => TrySetState(DefaultKey);
            public bool TryResetDefaultState() => TryResetState(DefaultKey);
            
            /************************************************************************************************************************/
        }
    }
}