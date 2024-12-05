namespace Tools.PoolModule2
{
    public interface IPoolable
    {
        void OnGet();
        void OnReturn();
    }
    
    public interface IPoolable2 : IPoolable
    {
        string Name { get; }
    }
}