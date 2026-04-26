using System;
using GaviShooting.Logic.Entity;

namespace GaviShooting.Logic.Stage
{
    [Serializable]
    public struct SpawnEvent
    {
        public int Frame;
        public eEnemyType EnemyType;
        public float X;
        public float Y;
    }
}
