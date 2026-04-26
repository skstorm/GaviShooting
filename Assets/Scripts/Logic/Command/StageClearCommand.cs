using GaviShooting.Core;
using GaviShooting.Logic.Fsm;

namespace GaviShooting.Logic.Command
{
    public class StageClearCommand : ICommand
    {
        private readonly StateMachine _fsm;

        public StageClearCommand(StateMachine fsm)
        {
            _fsm = fsm;
        }

        public void Execute()
        {
            Log.Info("StageClearCommand");
            _fsm.ChangeState(eGameState.StageClear);
        }
    }
}
