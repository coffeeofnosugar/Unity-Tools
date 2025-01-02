using UnityEngine;

namespace Tools.FSM.Sample
{
    /// <summary>
    /// 控制Player行为逻辑，检测Player状态（是否在空中）并监听玩家输入决定Player进入什么状态
    /// <para></para>
    /// 代码规范：<see cref="CharacterState"/>类里只做状态内的行为逻辑处理，最好不要转换状态，而是通过<see cref="FSM.Sample.PlayerBrain2"/>转换状态。
    /// 除非是特殊的事件状态（如攻击、死亡）结束后使用<see cref="StateMachine{State, CharacterState}.WithDefault.TrySetDefaultState"/>返回默认状态。
    /// <para></para>
    /// 注意事项：
    /// 1. 为了使转换更敏捷，一般在<see cref="Update"/>中执行状态转换。
    /// 2. 在Update中转换时，必须要使用if等逻辑判断处理好进入状态的顺序，不要出现成功进入A状态后继续转换，导致进入了B状态。
    /// <example>错误示例：<code>
    /// private void Update()
    /// {
    ///     if (在空中)
    ///         FSM.TrySetState(State.Air);         // 角色在空中，成功进入了空中状态
    ///     FSM.TrySetState(State.Idle);            // 但是还会继续执行Update，导致进入Idle状态。最终表现在游戏中的结果是玩家疯狂在这两个状态中切换
    /// }
    /// </code></example>
    /// <example>正确示例：<code>
    /// private void Update()
    /// {
    ///     if (在空中)
    ///         FSM.TrySetState(State.Air);         // 角色在空中，成功进入了空中状态
    ///     else
    ///         FSM.TrySetState(State.Idle);        // 角色不在空中，则进入Idle状态
    /// }
    /// </code></example>
    /// 3. 如果想要整理代码，打算将不同的逻辑写在不同的方法中，也需要遵循第二项。可以参考<see cref="UpdateAttackAction"/>，如果不满足if直接return。
    /// </summary>
    public sealed class PlayerBrain1 : MonoBehaviour
    {
        public StateMachine<CharacterState>.WithDefault StateMachine = new();

        [SerializeField] private CharacterState _idleState;
        [SerializeField] private CharacterState _moveState;
        [SerializeField] private CharacterState _attackState;
        
        
        private void Awake()
        {
            // 初始化状态机，需要选择默认状态，在使用ForceSetDefaultState等方法时会进入该状态
            // StateMachine.InitializeAfterDeserialize(_idleState);
            StateMachine.InitializeAfterDeserialize();
        }

        private void Update()
        {
            UpdateMovement();
            UpdateAttackAction();
        }
        
        private void UpdateMovement()
        {
            if (Mathf.Abs(Input.GetAxis("Horizontal")) >= 0.1f)
            {
                // 两种方法进入状态
                StateMachine.TrySetState(_moveState);
                // _stateMachine.TrySetState(_moveState);
            }
            else
            {
                StateMachine.TrySetState(_idleState);
            }
        }
        
        private void UpdateAttackAction()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            StateMachine.TrySetState(_attackState);
        }
    }
}