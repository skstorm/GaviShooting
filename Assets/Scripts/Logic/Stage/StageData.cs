using System.Collections.Generic;

namespace GaviShooting.Logic.Stage
{
    public class StageData
    {
        public string StageName;
        public int TotalFrames;
        public float ScrollSpeed;
        public int BossFrame;
        public List<SpawnEvent> SpawnEvents;
    }
}
