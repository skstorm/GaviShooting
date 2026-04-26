using GaviShooting.Core;
using GaviShooting.Logic.Fsm;

namespace GaviShooting.Logic.Command
{
    public class GameStartCommand : ICommand
    {
        private readonly StateMachine _fsm;

        public GameStartCommand(StateMachine fsm)
        {
            _fsm = fsm;
        }

        public void Execute()
        {
            Log.Info("GameStartCommand");
            _fsm.ChangeState(eGameState.Playing);
        }
    }
}
