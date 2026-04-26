using System;
using GaviShooting.Core;
using GaviShooting.Logic.Collision;

namespace GaviShooting.Logic.Entity
{
    public class EnemyLogic : IEntity, IEnemyReadOnly
    {
        private const float SCREEN_LEFT = -12f;

        private readonly int _id;
        private readonly eEnemyType _enemyType;
        private readonly float _speed;
        private readonly int _contactDamage;
        private readonly int _score;
        private readonly float _hitRadius;
        private readonly IEnemyViewWriter _viewWriter;
        private float _x;
        private float _y;
        private int _hp;
        private int _frameCount;
        private float _spawnY;
        private bool _dead;

        public EnemyLogic(int id, eEnemyType enemyType, float x, float y, int hp, float speed, int contactDamage, int score, float hitRadius, IEnemyViewWriter viewWriter)
        {
            _id = id;
            _enemyType = enemyType;
            _x = x;
            _y = y;
            _hp = hp;
            _speed = speed;
            _contactDamage = contactDamage;
            _score = score;
            _hitRadius = hitRadius;
            _viewWriter = viewWriter;
            _spawnY = y;
            Log.Info("EnemyLogic created id={0} type={1} pos=({2},{3}) hp={4}", id, enemyType, x, y, hp);
        }

        public int Id => _id;
        public float X => _x;
        public float Y => _y;
        public int Hp => _hp;
        public eEnemyType EnemyType => _enemyType;
        public int ContactDamage => _contactDamage;
        public int Score => _score;
        public bool IsAlive => !_dead;
        public bool ShouldRemove => _dead;
        public HitBox HitBox => new(_x, _y, _hitRadius);
        public eCollisionLayer Layer => eCollisionLayer.Enemy;

        public void Move()
        {
            _frameCount++;
            switch (_enemyType)
            {
                case eEnemyType.Straight:
                    _x -= _speed;
                    break;
                case eEnemyType.Wave:
                    _x -= _speed;
                    _y = _spawnY + (float)Math.Sin(_frameCount * 0.05f) * 2f;
                    break;
                case eEnemyType.Boss:
                    _y = (float)Math.Sin(_frameCount * 0.02f) * 3f;
                    break;
            }
        }

        public void Update()
        {
            if (_enemyType != eEnemyType.Boss && _x < SCREEN_LEFT)
            {
                _dead = true;
            }
        }

        public void TakeDamage(int amount)
        {
            _hp -= amount;
            if (_hp <= 0)
            {
                _hp = 0;
                _dead = true;
                Log.Info("EnemyLogic DEAD id={0} type={1}", _id, _enemyType);
                _viewWriter.PlayDeathEffect();
            }
            else
            {
                Log.Info("EnemyLogic TakeDamage id={0} amount={1} hp={2}", _id, amount, _hp);
            }
        }

        public bool CanShoot(int shootInterval)
        {
            return shootInterval > 0 && _frameCount % shootInterval == 0;
        }
    }
}
