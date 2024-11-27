namespace Tools.FSM
{
    /// <summary> 代码规范：<see cref="CharacterState"/>类里只做状态类的行为逻辑处理，最好不要转换状态，而是通过<see cref="PlayerBrain"/>转换状态
    /// 除非是事件状态（如攻击）结束后使用<see cref="StateMachine{State, CharacterState}.WithDefault.TrySetDefaultState"/>返回默认状态。 </summary>
    public abstract class CharacterState : StateBehaviour
    {
        /************************************************************************************************************************/
        
        // [SerializeField]
        // private Character _character;
        // public Character Character => _character;
        
        /************************************************************************************************************************/
        
        /// <summary> 很少的情况下才会使用该属性，平时无视即可 </summary>
        public virtual CharacterStatePriority Priority => CharacterStatePriority.Low;
        
        /// <summary>
        /// 是否能再次进入自己
        /// <para></para>
        /// 虽然<see cref="StateMachine{TState}.TrySetState(TState)"/>和<see cref="StateMachine{TState}.TryResetState(TState)"/>
        /// 已经实现了本属性的逻辑，但是在调用这两个方法的时候难免会混淆用法，所以这里单独设置成状态的属性，直接在属性中定义
        /// </summary>
        /// <param name="true">如攻击状态，在攻击动作还未播放完毕时，可再次进入攻击状态</param>
        /// <param name="false">如idle，无法从idle再次进入到idle</param>
        public virtual bool CanInterruptSelf => false;

        /// <summary> 注意：若子类复写了此属性，需要调用`return base.CanExitState` </summary>
        public override bool CanExitState
        {
            get
            {
                // 有两种获取`StateChange`方式
                CharacterState nextState = StateChange<CharacterState>.NextState;
                // CharacterState nextState = _character.StateMachine.NextState;
                if (nextState == this)
                    return CanInterruptSelf;
                else if (Priority == CharacterStatePriority.Low)
                    return true;
                else
                    return nextState.Priority > Priority;
            }
        }

        /************************************************************************************************************************/
        
    #if UNITY_EDITOR
        // protected override void OnValidate()
        // {
        //     base.OnValidate();
        //
        //     if (_character is not null) return;     // 下方的获取组件方法在预制体中并不生效，若已经在Scene中获取到组件了，则返回
        //         
        //     _character = gameObject.GetComponentInParent<Character>();
        //     if (_character == null)
        //         gameObject.GetComponentInChildren<Character>();
        // }
    #endif

        /************************************************************************************************************************/
    }
}
