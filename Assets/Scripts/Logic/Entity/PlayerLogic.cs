using GaviShooting.Core;
using GaviShooting.Logic.Collision;

namespace GaviShooting.Logic.Entity
{
    public class PlayerLogic : IEntity, IPlayerReadOnly
    {
        private const int MAX_HP = 100;
        private const int MAX_WEAPON_LEVEL = 3;
        private const float MOVE_SPEED = 5f / 60f;
        private const float HIT_RADIUS = 0.3f;

        private readonly int _id;
        private readonly IPlayerViewWriter _viewWriter;
        private float _x;
        private float _y;
        private int _hp;
        private int _weaponLevel;
        private float _dirX;
        private float _dirY;

        public PlayerLogic(int id, float x, float y, IPlayerViewWriter viewWriter)
        {
            _id = id;
            _x = x;
            _y = y;
            _hp = MAX_HP;
            _weaponLevel = 1;
            _viewWriter = viewWriter;
            Log.Info("PlayerLogic created id={0} pos=({1},{2})", id, x, y);
        }

        public int Id => _id;
        public float X => _x;
        public float Y => _y;
        public int Hp => _hp;
        public int WeaponLevel => _weaponLevel;
        public bool IsAlive => _hp > 0;
        public bool ShouldRemove => false;
        public HitBox HitBox => new(_x, _y, HIT_RADIUS);
        public eCollisionLayer Layer => eCollisionLayer.Player;

        public void SetDirection(float dx, float dy)
        {
            _dirX = dx;
            _dirY = dy;
        }

        public void Move()
        {
            _x += _dirX * MOVE_SPEED;
            _y += _dirY * MOVE_SPEED;
        }

        public void Update() { }

        public void TakeDamage(int amount)
        {
            _hp -= amount;
            if (_hp <= 0)
            {
                _hp = 0;
                Log.Warn("PlayerLogic DEAD");
                _viewWriter.PlayDeathEffect();
            }
            else
            {
                Log.Info("PlayerLogic TakeDamage amount={0} hp={1}", amount, _hp);
                _viewWriter.PlayDamageEffect();
            }
        }

        public void Heal(int amount)
        {
            _hp += amount;
            if (_hp > MAX_HP) _hp = MAX_HP;
            Log.Info("PlayerLogic Heal amount={0} hp={1}", amount, _hp);
        }

        public void UpgradeWeapon()
        {
            if (_weaponLevel < MAX_WEAPON_LEVEL)
            {
                _weaponLevel++;
                Log.Info("PlayerLogic UpgradeWeapon level={0}", _weaponLevel);
            }
        }

        public void ClampPosition(float minX, float maxX, float minY, float maxY)
        {
            if (_x < minX) _x = minX;
            if (_x > maxX) _x = maxX;
            if (_y < minY) _y = minY;
            if (_y > maxY) _y = maxY;
        }
    }
}
