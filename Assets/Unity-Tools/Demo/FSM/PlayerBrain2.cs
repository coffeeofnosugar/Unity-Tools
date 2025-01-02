using UnityEngine;
using UnityEngine.Serialization;

namespace Tools.FSM.Sample
{
    public sealed class PlayerBrain2 : MonoBehaviour
    {
        public enum State { Idle, Move, Attack }

        public StateMachine<State, CharacterState>.WithDefault StateMachine = new();

        [SerializeField] private CharacterState _idleState;
        [SerializeField] private CharacterState _moveState;
        [SerializeField] private CharacterState _attackState;

        private void Awake()
        {
            // 可以使用枚举初始化
            StateMachine.InitializeAfterDeserialize();
            
            // 需要映射枚举与状态的关系
            StateMachine.AddRange(
                new [] { State.Idle , State.Move, State.Attack },
                new [] { _idleState, _moveState, _attackState });
        }

        private void Update()
        {
            UpdateMovement();
            UpdateAttackAction();
        }
        
        private void UpdateMovement()
        {
            if (Mathf.Abs(Input.GetAxis("Horizontal")) >= 0.1f)
                StateMachine.TrySetState(State.Move);      // 可以使用枚举做状态转换，而不需要类的实例
            else
                StateMachine.TrySetState(State.Idle);
        }
        
        private void UpdateAttackAction()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            StateMachine.TrySetState(State.Attack);
            Debug.Log(StateMachine.CurrentKey);
        }
    }
}