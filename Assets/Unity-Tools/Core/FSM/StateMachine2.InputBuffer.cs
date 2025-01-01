namespace Tools.FSM
{
    public partial class StateMachine<TKey, TState>
    {
        public new class InputBuffer : InputBuffer<StateMachine<TKey, TState>>
        {
            /************************************************************************************************************************/

            public TKey Key { get; set; }

            /************************************************************************************************************************/

            public InputBuffer(StateMachine<TKey, TState> stateMachine) : base(stateMachine) { }

            /************************************************************************************************************************/

            /// <summary> 详细解释查看基类的Buffer </summary>
            public void Buffer(TKey key, float timeOut, System.Func<bool> condition = default, System.Action succeedAction = default)
            {
                if (_stateMachine.TryGetValue(key, out var state))
                {
                    Key = key;
                    Buffer(state, timeOut, condition, succeedAction);
                }
                else throw new System.ArgumentException($"{GetType().FullName} 并未注册 {key} 状态");
            }

            /************************************************************************************************************************/

            protected override bool TryEnterState()
                => _stateMachine.TryResetState(Key);

            /************************************************************************************************************************/

            public override void Clear()
            {
                base.Clear();
                Key = default;
            }
            
            /************************************************************************************************************************/
        }
    }
}