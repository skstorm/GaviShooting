using GaviShooting.Core;
using GaviShooting.Logic.Entity;

namespace GaviShooting.Logic.Command
{
    public class ShootCommand : ICommand
    {
        private readonly PlayerLogic _player;
        private readonly EntityManager _entityManager;
        private readonly System.Func<float, float, float, float, bool, int, BulletLogic> _bulletFactory;

        private const float BULLET_SPEED = 10f / 60f;
        private const int BULLET_DAMAGE = 10;
        private const float SPREAD_ANGLE = 0.15f;

        public ShootCommand(PlayerLogic player, EntityManager entityManager,
            System.Func<float, float, float, float, bool, int, BulletLogic> bulletFactory)
        {
            _player = player;
            _entityManager = entityManager;
            _bulletFactory = bulletFactory;
        }

        public void Execute()
        {
            int level = _player.WeaponLevel;
            float px = _player.X;
            float py = _player.Y;

            spawnBullet(px, py, BULLET_SPEED, 0f);

            if (level >= 2)
            {
                spawnBullet(px, py, BULLET_SPEED, SPREAD_ANGLE);
            }
            if (level >= 3)
            {
                spawnBullet(px, py, BULLET_SPEED, -SPREAD_ANGLE);
            }
            Log.Info("ShootCommand level={0}", level);
        }

        private void spawnBullet(float x, float y, float speed, float angleY)
        {
            var bullet = _bulletFactory(x + 0.5f, y, speed, angleY, true, BULLET_DAMAGE);
            _entityManager.EnqueueSpawn(bullet);
        }
    }
}
