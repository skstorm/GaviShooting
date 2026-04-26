using GaviShooting.Core;
using GaviShooting.Logic.Fsm;
using GaviShooting.View;

namespace GaviShooting.Entry
{
    public class StageClearState : IState
    {
        private readonly UiView _uiView;

        public StageClearState(UiView uiView)
        {
            _uiView = uiView;
        }

        public void Enter()
        {
            Log.Info("StageClearState.Enter");
            _uiView.ShowStageClear();
        }

        public void Exit() => _uiView.HideAll();
        public void LogicUpdate() { }
    }
}
