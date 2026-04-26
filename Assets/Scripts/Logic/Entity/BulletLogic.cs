using GaviShooting.Core;
using GaviShooting.Logic.Collision;

namespace GaviShooting.Logic.Entity
{
    public class BulletLogic : IEntity, IBulletReadOnly
    {
        private const float HIT_RADIUS = 0.15f;
        private const float SCREEN_MIN = -12f;
        private const float SCREEN_MAX = 12f;

        private readonly int _id;
        private readonly float _speedX;
        private readonly float _speedY;
        private readonly bool _isPlayerBullet;
        private readonly int _damage;
        private readonly IBulletViewWriter _viewWriter;
        private float _x;
        private float _y;
        private bool _dead;

        public BulletLogic(int id, float x, float y, float speedX, float speedY, bool isPlayerBullet, int damage, IBulletViewWriter viewWriter)
        {
            _id = id;
            _x = x;
            _y = y;
            _speedX = speedX;
            _speedY = speedY;
            _isPlayerBullet = isPlayerBullet;
            _damage = damage;
            _viewWriter = viewWriter;
            Log.Info("BulletLogic created id={0} player={1} pos=({2},{3})", id, isPlayerBullet, x, y);
        }

        public int Id => _id;
        public float X => _x;
        public float Y => _y;
        public bool IsPlayerBullet => _isPlayerBullet;
        public int Damage => _damage;
        public bool IsAlive => !_dead;
        public bool ShouldRemove => _dead;
        public HitBox HitBox => new(_x, _y, HIT_RADIUS);
        public eCollisionLayer Layer => _isPlayerBullet ? eCollisionLayer.PlayerBullet : eCollisionLayer.EnemyBullet;

        public void Move()
        {
            _x += _speedX;
            _y += _speedY;
        }

        public void Update()
        {
            if (_x < SCREEN_MIN || _x > SCREEN_MAX || _y < SCREEN_MIN || _y > SCREEN_MAX)
            {
                _dead = true;
            }
        }

        public void Kill()
        {
            _dead = true;
            _viewWriter.PlayHitEffect();
            Log.Info("BulletLogic Kill id={0}", _id);
        }
    }
}
