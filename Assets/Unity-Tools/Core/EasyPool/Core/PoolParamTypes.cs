namespace Tools.EasyPoolKit
{
    /// <summary>
    /// 当用户试图获取一个对象，但池达到最大计数<see cref="IAbstractPool{T}.MaxSpawnCount"/>
    /// Default =>创建新对象并返回
    /// RejectNull =>不创建新对象并返回null
    /// Recycleold => 强制回收最老的并返回，注意：该物体不会执行回收方法
    /// </summary>
    public enum PoolReachMaxLimitType
    {
        Default, //无限制
        RejectNull,
        RecycleOldest,
    }

    /// <summary>
    /// 当对象返回给池时，如果池大小大于最大影藏的最大值<see cref="IAbstractPool{T}.MaxDespawnCount"/>
    /// 默认值 => 什么也不做
    /// DestroyToLimit => 销毁对象，使池大小等于限制
    /// </summary>
    public enum PoolDespawnDestroyType
    {
        Default, //Not destroy
        DestroyToLimit,
    }

    /// <summary>
    /// 当清除池时
    /// Default => 清除所有
    /// ClearToLimit => 清楚掉多余的对象，使池大小等于初始化大小<see cref="IAbstractPool{T}.InitCreateCount"/>
    /// </summary>
    public enum PoolClearType
    {
        Default, //Clear all
        ClearToLimit,
    }
}