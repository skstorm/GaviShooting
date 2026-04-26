using GaviShooting.Core;
using GaviShooting.Logic.Fsm;
using GaviShooting.View;

namespace GaviShooting.Entry
{
    public class GameOverState : IState
    {
        private readonly UiView _uiView;

        public GameOverState(UiView uiView)
        {
            _uiView = uiView;
        }

        public void Enter()
        {
            Log.Info("GameOverState.Enter");
            _uiView.ShowGameOver();
        }

        public void Exit() => _uiView.HideAll();
        public void LogicUpdate() { }
    }
}
