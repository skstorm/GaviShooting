namespace GaviShooting.Core
{
    public class FrameManager
    {
        private readonly float _secPerFrame;
        private float _accumulator;

        public FrameManager(int fps)
        {
            _secPerFrame = 1f / fps;
        }

        /// <summary>
        /// 경과시간 축적 방식으로 계산하여 가변 프레임레이트에서도 일정한 논리 프레임 수를 보장한다
        /// </summary>
        public int CalcDeltaFrame(float deltaTime)
        {
            _accumulator += deltaTime;
            int frames = (int)(_accumulator / _secPerFrame);
            _accumulator -= frames * _secPerFrame;
            return frames;
        }
    }
}
