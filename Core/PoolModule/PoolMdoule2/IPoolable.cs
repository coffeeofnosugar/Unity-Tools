namespace Tools.PoolModule2
{
    public interface IPoolable
    {
        void OnGet();
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