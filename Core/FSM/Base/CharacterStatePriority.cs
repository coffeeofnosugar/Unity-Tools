namespace Tools.FSM
{
    /// <summary> 状态优先级 </summary>
    /// <example>
    /// 假设优先级为：A &lt; B &lt; C
    /// <para></para>
    /// 则C永远也无法进入A或B，B永远无法进入A，除非使用强制设置状态
    /// <para></para>
    /// 使用场景：攻击状态为Medium，
    /// 那么在进入攻击状态后，除非使用强制进入另外一个状态，否则都将不会进入其他状态。
    /// </example>
    public enum CharacterStatePriority
    {
        Low = 0,
        Medium = 1,
        High = 2,
    }
}