using System.Collections.Generic;
using GaviShooting.Core;

namespace GaviShooting.Logic.Collision
{
    public class CollisionSystem
    {
        private readonly List<CollisionPair> _results = new();

        public List<CollisionPair> CheckAll(List<ICollidable> entities)
        {
            _results.Clear();
            for (int i = 0; i < entities.Count; i++)
            {
                for (int j = i + 1; j < entities.Count; j++)
                {
                    var a = entities[i];
                    var b = entities[j];
                    if (!a.IsAlive || !b.IsAlive) continue;
                    if (!shouldCheck(a.Layer, b.Layer)) continue;
                    var ha = a.HitBox;
                    var hb = b.HitBox;
                    if (ha.Overlaps(hb))
                    {
                        _results.Add(new CollisionPair { A = a, B = b });
                        Log.Info("Collision: {0} vs {1}", a.Layer, b.Layer);
                    }
                }
            }
            return _results;
        }

        private bool shouldCheck(eCollisionLayer a, eCollisionLayer b)
        {
            if (a > b) (a, b) = (b, a);
            return (a, b) switch
            {
                (eCollisionLayer.Player, eCollisionLayer.Enemy) => true,
                (eCollisionLayer.Player, eCollisionLayer.EnemyBullet) => true,
                (eCollisionLayer.Player, eCollisionLayer.Item) => true,
                (eCollisionLayer.PlayerBullet, eCollisionLayer.Enemy) => true,
                _ => false
            };
        }
    }
}
