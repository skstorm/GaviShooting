using GaviShooting.Core;
using GaviShooting.Logic.Fsm;

namespace GaviShooting.Logic.Command
{
    public class PauseCommand : ICommand
    {
        private readonly StateMachine _fsm;

        public PauseCommand(StateMachine fsm)
        {
            _fsm = fsm;
        }

        public void Execute()
        {
            if (_fsm.CurrentState == eGameState.Playing)
            {
                Log.Info("PauseCommand → Paused");
                _fsm.ChangeState(eGameState.Paused);
            }
            else if (_fsm.CurrentState == eGameState.Paused)
            {
                Log.Info("PauseCommand → Resume");
                _fsm.ChangeState(eGameState.Playing);
            }
        }
    }
}
