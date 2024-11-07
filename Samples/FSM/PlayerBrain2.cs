using UnityEngine;

namespace Tools.FSM.Sample
{
    public sealed class PlayerBrain2 : MonoBehaviour
    {
        private enum State { Idle, Move }

        [SerializeField]
        private StateMachine<State, CharacterState>.WithDefault _stateMachine = new();

        [SerializeField] private CharacterState _idleState;
        [SerializeField] private CharacterState _moveState;

        private void Awake()
        {
            // 需要映射枚举与状态的关系
            _stateMachine.AddRange(
                new [] { State.Idle , State.Move },
                new [] { _idleState, _moveState});
        }

        private void Start()
        {
            // 可以使用枚举初始化
            _stateMachine.InitializeAfterDeserialize(State.Idle);
        }

        private void Update()
        {
            UpdateMovement();
        }
        
        private void UpdateMovement()
        {
            if (Mathf.Abs(Input.GetAxis("Horizontal")) >= 0.1f)
                _stateMachine.TrySetState(State.Move);      // 可以使用枚举做状态转换，而不需要类的实例
            else
                _stateMachine.TrySetState(State.Idle);
        }
    }
}