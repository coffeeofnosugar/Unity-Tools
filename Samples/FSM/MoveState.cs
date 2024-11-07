using UnityEngine;

namespace Tools.FSM.Sample
{
    public class MoveState : CharacterState
    {
        private void OnEnable()
        {
            Debug.Log("播放Move动画");
        }
    }
}