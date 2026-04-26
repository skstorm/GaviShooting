namespace GaviShooting.Logic.Fsm
{
    public interface IState
    {
        void Enter();
        void Exit();
        void LogicUpdate();
    }
}
