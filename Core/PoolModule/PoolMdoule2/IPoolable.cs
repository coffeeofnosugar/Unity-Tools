namespace Tools.PoolModule2
{
    public interface IPoolable
    {
        void OnGet();
        void OnReturn();
    }
}