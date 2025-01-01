namespace Tools.PoolModule
{
    public interface IPoolable
    {
        void OnGet(params object[] args);
        void OnReturn();
    }
    
    public interface IPoolableString : IPoolable
    {
        string Name { get; }
    }
    
    public interface IPoolableInt : IPoolable
    {
        int Id { get; }
    }
}