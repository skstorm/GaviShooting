using GaviShooting.Core;
using GaviShooting.Logic.Fsm;

namespace GaviShooting.Logic.Command
{
    public class GameOverCommand : ICommand
    {
        private readonly StateMachine _fsm;

        public GameOverCommand(StateMachine fsm)
        {
            _fsm = fsm;
        }

        public void Execute()
        {
            Log.Info("GameOverCommand");
            _fsm.ChangeState(eGameState.GameOver);
        }
    }
}
