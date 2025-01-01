using System.Collections.Generic;

namespace Tools.FSM
{
    public interface IPrioritizable : IState
    {
        float Priority { get; }
    }
    
    public partial class StateMachine<TState>
    {
        /// <summary>
        /// 继承 <see cref="SortedList{TKey, TValue}"/> 基类，继承后可将本类视作字典与列表融合的类型，
        /// Key为 <see cref="IPrioritizable.Priority"/>，Value为 <see cref="TState"/>，
        /// 并且在 <see cref="SortedList{TKey, TValue}.Add(TKey, TValue)"/> 添加元素时会直接根据定义的规则排序
        /// <para></para>
        /// 由于并未使用到TKey，所以该类的效果同样适用于 <see cref="StateMachine{TState, TStateBehaviour}"/>
        /// </summary>
        /// <remarks><strong>示例:</strong><code>
        ///
        /// // 定义一个状态选择器
        /// private StateMachine&lt;State&gt;.StateSelector stateSelector = new();
        ///
        /// private void Awake()
        /// {
        ///     // 初始化状态选择器
        ///     stateSelector.Add(idleState);           // 如果状态实现了 <see cref="IPrioritizable"/> 接口，可不传入优先级，假如idleState实现了且值为100
        ///     stateSelector.Add(1, runState);         // 设置优先级
        ///     stateSelector.Add(3, attackState);      // 数值越大，优先级越高
        /// }
        ///
        /// private void ChangeState()
        /// {
        ///     if (Input.GetKeyDown(KeyCode.Space))
        ///     {
        ///         // 在按下空格键后，按idleState, attackState, runState的顺序尝试进入这些状态
        ///         stateMachine.TrySetState(stateSelector.Values);
        ///     }
        /// }
        /// 
        /// </code></remarks>
        public class StateSelector : SortedList<float, TState>
        {
            /// <summary> 初始化设置排序规则 </summary>
            public StateSelector() : base(ReverseComparer<float>.Instance) { }
            
            /// <summary> 如果需要添加的状态实现了 <see cref="IPrioritizable"/> 接口，则根据 <see cref="IPrioritizable.Priority"/> 排序 </summary>
            public void Add<TPrioritizable>(TPrioritizable state)
                where TPrioritizable : TState, IPrioritizable
                => Add(state.Priority, state);
        }
    }
    
    /// <summary> 定义一个从大到小的排序规则 </summary>
    public class ReverseComparer<T> : IComparer<T>
    {
        /// <summary> 饿汉单例 </summary>
        public static readonly ReverseComparer<T> Instance = new();

        /// <summary> 构造函数私有化——不需要用户再实例化其他的 </summary>
        private ReverseComparer() { }

        /// <summary> 使用从大到小的排列方式 </summary>
        public int Compare(T x, T y) => Comparer<T>.Default.Compare(y, x);
    }
}