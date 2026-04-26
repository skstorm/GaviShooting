using GaviShooting.Core;
using GaviShooting.Logic.Fsm;
using GaviShooting.View;

namespace GaviShooting.Entry
{
    public class PausedState : IState
    {
        private readonly UiView _uiView;

        public PausedState(UiView uiView)
        {
            _uiView = uiView;
        }

        public void Enter()
        {
            Log.Info("PausedState.Enter");
            _uiView.ShowPause();
        }

        public void Exit() => _uiView.HideAll();
        public void LogicUpdate() { }
    }
}
