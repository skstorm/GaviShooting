namespace GaviShooting.Core
{
    public class GameLoop
    {
        private readonly FrameManager _frameManager;
        private readonly IGameLoop _logic;

        public GameLoop(FrameManager frameManager, IGameLoop logic)
        {
            _frameManager = frameManager;
            _logic = logic;
        }

        public void Update(float deltaTime)
        {
            int deltaFrame = _frameManager.CalcDeltaFrame(deltaTime);
            for (int i = 0; i < deltaFrame; i++)
            {
                _logic.LogicUpdate();
            }
            if (deltaFrame > 0)
            {
                _logic.Render();
            }
        }
    }
}
