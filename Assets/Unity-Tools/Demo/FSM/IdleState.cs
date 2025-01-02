using UnityEngine;

namespace Tools.FSM.Sample
{
    /// <summary>
    /// 状态类里的逻辑越简单越好，只需要做状态类的逻辑判断，<font color="red">不要做状态转换</font>
    /// </summary>
    public class IdleState : CharacterState
    {
        private void OnEnable()
        {
            Debug.Log("播放Idle动画");
        }

        private void Update()
        {
            // 不要出现如下状态转换的逻辑，这种逻辑应该在PlayerBrain里做
            // if (Input.GetKeyDown(KeyCode.Space))
            // {
            //     _stateMachine.TrySetState(State.Air);
            // }
        }
    }
}