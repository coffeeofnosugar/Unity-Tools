using System.Threading.Tasks;
using UnityEngine;

namespace Tools.FSM.Sample
{
    public class AttackState : CharacterState
    {
        [SerializeField]
        private PlayerBrain1 _brain;
        private int _animationTime = 1000;
        private void OnEnable()
        {
            Debug.Log("播放Attack动画");
            AnimationDelay();
        }

        // 将攻击状态设置为中优先级，避免被其他状态打断
        public override CharacterStatePriority Priority
            => CharacterStatePriority.Medium;

        private async void AnimationDelay()
        {
            await Task.Delay(_animationTime);               // 模拟动画播放
            _brain.StateMachine.ForceSetDefaultState();     // 在动画播放完毕后强制设置成默认状态
        }
    }
}