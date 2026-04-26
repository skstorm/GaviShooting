using System.Collections.Generic;
using GaviShooting.Core;

namespace GaviShooting.Logic.Stage
{
    public class StageTimeline
    {
        private readonly StageData _data;
        private readonly List<SpawnEvent> _result = new();
        private int _currentFrame;

        public StageTimeline(StageData data)
        {
            _data = data;
        }

        public int CurrentFrame => _currentFrame;
        public bool IsBossTime => _currentFrame >= _data.BossFrame;
        public bool IsFinished => _currentFrame >= _data.TotalFrames;
        public float ScrollSpeed => _data.ScrollSpeed;

        public List<SpawnEvent> Advance()
        {
            _result.Clear();
            _currentFrame++;
            for (int i = 0; i < _data.SpawnEvents.Count; i++)
            {
                if (_data.SpawnEvents[i].Frame == _currentFrame)
                {
                    _result.Add(_data.SpawnEvents[i]);
                    Log.Info("StageTimeline spawn frame={0} type={1}", _currentFrame, _data.SpawnEvents[i].EnemyType);
                }
            }
            return _result;
        }

        public void Reset()
        {
            _currentFrame = 0;
            Log.Info("StageTimeline Reset");
        }
    }
}
