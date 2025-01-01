namespace Tools.FSM
{
    public interface IState
    {
        bool CanEnterState { get; }
        bool CanExitState { get; }
        void OnEnterState();
        void OnExitState();
    }

    public abstract class State : IState
    {
        public virtual bool CanEnterState => true;
        public virtual bool CanExitState => true;
        public virtual void OnEnterState() { }
        public virtual void OnExitState() { }
    }
}