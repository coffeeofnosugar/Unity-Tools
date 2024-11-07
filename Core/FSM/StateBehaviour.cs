using UnityEngine;

namespace Tools.FSM
{
    public abstract class StateBehaviour : MonoBehaviour, IState
    {
        /************************************************************************************************************************/
        
        public virtual bool CanEnterState => true;
        public virtual bool CanExitState => true;
        
        /************************************************************************************************************************/
        
        public void OnEnterState()
        {
#if UNITY_ASSERTIONS
            if (enabled)
                Debug.LogError(
                    $"{nameof(StateBehaviour)} was already enabled before {nameof(OnEnterState)}: {this}",
                    this);
#endif
#if UNITY_EDITOR
            // Unity的 Inspector窗口 在某些情况下不会自动刷新，尤其是在被折叠的时候
            // 使用该方法强制刷新，使组件显示正确的状态，而不是等到下次自然重绘
            else
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
#endif
            enabled = true;
        }

        public void OnExitState()
        {
            if (this == null)
                throw  new System.NullReferenceException($"{GetType().FullName} 状态并未被实例化");
#if UNITY_ASSERTIONS
            if (!enabled)
                Debug.LogError(
                    $"{nameof(StateBehaviour)} was already disabled before {nameof(OnExitState)}: {this}",
                    this);
#endif
            enabled = false;
        }
        
        /************************************************************************************************************************/
        
#if UNITY_EDITOR
        /// <summary> 在[Editor-Only]状态下禁用，只有当前状态在运行时被启用 </summary>
        protected virtual void OnValidate()
        {
            if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)    // 处于播放模式或即将切换到播放模式
                return;
            
            enabled = false;
        }
#endif
        /************************************************************************************************************************/
    }
}