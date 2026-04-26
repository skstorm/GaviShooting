using GaviShooting.Core;
using GaviShooting.Logic.Fsm;
using GaviShooting.View;

namespace GaviShooting.Entry
{
    public class ReadyState : IState
    {
        private readonly UiView _uiView;

        public ReadyState(UiView uiView)
        {
            _uiView = uiView;
        }

        public void Enter()
        {
            Log.Info("ReadyState.Enter");
            _uiView.ShowReady();
        }

        public void Exit() => _uiView.HideAll();
        public void LogicUpdate() { }
    }
}
