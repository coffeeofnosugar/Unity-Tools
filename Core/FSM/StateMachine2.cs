using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
#endif

namespace Tools.FSM
{
    public interface IKeyedStateMachine<TKey>
    {
        TKey CurrentKey { get; }
        TKey PreviousKey { get; }
        TKey NextKey { get; }
        bool TrySetState(TKey key);
        bool TryResetState(TKey key);
        void ForceSetState(TKey key);
    }
    
    /// <summary> 使用方法参考<see cref="WithDefault"/> </summary>
    [System.Serializable]
    public partial class StateMachine<TKey, TState> : StateMachine<TState>, IKeyedStateMachine<TKey>, IDictionary<TKey, TState>
        where TState : class, IState
    {
        /************************************************************************************************************************/

        /// <summary> 存储枚举与状态的映射 </summary>
        public IDictionary<TKey, TState> Dictionary { get; set; } = new Dictionary<TKey, TState>();

        [UnityEngine.SerializeField]
        [ReadOnly, HideInEditorMode]
        private TKey _currentKey;
        public TKey CurrentKey => _currentKey;
        public TKey PreviousKey => KeyChange<TKey>.PreviousKey;
        public TKey NextKey => KeyChange<TKey>.NextKey;
        
        /************************************************************************************************************************/

        public override void InitializeAfterDeserialize()
        {
            if (CurrentState != null)
            {
                using (new KeyChange<TKey>(this, default, _currentKey))
                using (new StateChange<TState>(this, null, CurrentState))
                    CurrentState.OnEnterState();
            }
            else if (Dictionary.TryGetValue(_currentKey, out var state))
            {
                ForceSetState(state);
            }
            else
            {
                throw new System.ArgumentException($"{GetType().FullName} 并未注册 {_currentKey} 状态，初始化失败");
            }
        }
        
        /************************************************************************************************************************/
        
        
        public bool TrySetState(TKey key)
        {
            if (EqualityComparer<TKey>.Default.Equals(_currentKey, key))
                return false;
            else
                return TryResetState(key);
        }
        
        /************************************************************************************************************************/
        
        public bool TryResetState(TKey key)
        {
            if (!Dictionary.TryGetValue(key, out var state)) throw new System.ArgumentException($"{GetType().FullName} 并未注册 {key} 状态");

            using (new KeyChange<TKey>(this, _currentKey, key))
            {
                if (!CanSetState(state))
                    return false;

                _currentKey = key;
                ForceSetState(state);
                return true;
            }
        }
        
        /************************************************************************************************************************/
        
        public void ForceSetState(TKey key)
        {
            if (!Dictionary.TryGetValue(key, out var state)) throw new System.ArgumentException($"{GetType().FullName} 并未注册 {key} 状态");
            
            using (new KeyChange<TKey>(this, _currentKey, key))
            {
                _currentKey = key;
                ForceSetState(state);
            }
        }
        
        /************************************************************************************************************************/
        #region Dictionary Wrappers
        /************************************************************************************************************************/

        public TState this[TKey key] { get => Dictionary[key]; set => Dictionary[key] = value; }
        public bool TryGetValue(TKey key, out TState state) => Dictionary.TryGetValue(key, out state);
        public ICollection<TKey> Keys => Dictionary.Keys;
        public ICollection<TState> Values => Dictionary.Values;
        public int Count => Dictionary.Count;
        public void Add(TKey key, TState state) => Dictionary.Add(key, state);
        public void Add(KeyValuePair<TKey, TState> item) => Dictionary.Add(item);
        public bool Remove(TKey key) => Dictionary.Remove(key);
        public bool Remove(KeyValuePair<TKey, TState> item) => Dictionary.Remove(item);
        public void Clear() => Dictionary.Clear();
        public bool Contains(KeyValuePair<TKey, TState> item) => Dictionary.Contains(item);
        public bool ContainsKey(TKey key) => Dictionary.ContainsKey(key);
        public IEnumerator<KeyValuePair<TKey, TState>> GetEnumerator() => Dictionary.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public void CopyTo(KeyValuePair<TKey, TState>[] array, int arrayIndex) => Dictionary.CopyTo(array, arrayIndex);
        bool ICollection<KeyValuePair<TKey, TState>>.IsReadOnly => Dictionary.IsReadOnly;
        
        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/

        /// <summary> 通过键获取状态，如果没有注册返回null </summary>
        public TState GetState(TKey key)
        {
            TryGetValue(key, out var state);
            return state;
        }

        /************************************************************************************************************************/

        /// <summary> 通过数组的形式添加映射关系，两者长度必须一致 </summary>
        public void AddRange(TKey[] keys, TState[] states)
        {
            UnityEngine.Debug.Assert(keys.Length == states.Length,
                $"The '{nameof(keys)}' 与 '{nameof(states)}' 长度不一致");

            for (int i = 0; i < keys.Length; i++)
            {
                Dictionary.Add(keys[i], states[i]);
            }
        }

        /************************************************************************************************************************/
        
        public override string ToString()
            => $"{GetType().FullName} -> {_currentKey} -> {(CurrentState != null ? CurrentState.ToString() : "null")}";

        /************************************************************************************************************************/

#if UNITY_EDITOR && ODIN_INSPECTOR
        /// <summary>
        /// 由于<see cref="StateMachine{TKey, TState}"/>继承了<see cref="IDictionary{TKey, TValue}"/>接口，
        /// 如果项目安装了Odin插件，<see cref="StateMachine{TKey, TState}"/>在Inspector窗口上只会序列化成字典，且不会序列化其他的字段，
        /// 如 <see cref="_currentKey"/> 和 <see cref="WithDefault.DefaultKey"/>。
        /// <para></para>
        /// 定义此类后，Odin不会将<see cref="StateMachine{TKey, TState}"/>序列化成字典，且能序列化其他字段。
        /// <para></para>
        /// <see cref="ProcessedMemberPropertyResolver{T}"/>为一个抽象类，可以控制参数<see cref="T"/>在 Inspector 中如何序列化。
        /// <para></para>
        /// 使用[ResolverPriority(100)]修饰<see cref="SerializableResolver{T}"/>后，由于100大于默认的-5，
        /// <see cref="T"/>将以<see cref="SerializableResolver{T}"/>的形式被序列化，即非字典类型
        /// <remarks>
        /// 该类只是用来定义<see cref="T"/>的序列化规则，告诉Odin插件该如何序列化，无需实例化和使用
        /// </remarks></summary>
        [ResolverPriority(100)]
        private class SerializableResolver<T> : ProcessedMemberPropertyResolver<T> where T : StateMachine<TKey, TState> { }
#endif
        /************************************************************************************************************************/
        
    }
}