## Task 8: Logic — PlayerLogic + 테스트

**Files:**
- Create: `Assets/Scripts/Logic/Entity/IPlayerReadOnly.cs`
- Create: `Assets/Scripts/Logic/Entity/IPlayerViewWriter.cs`
- Create: `Assets/Scripts/Logic/Entity/PlayerLogic.cs`
- Create: `Assets/Tests/EditMode/Logic/PlayerLogicTest.cs`

- [ ] **Step 1: 인터페이스 작성**

Create `Assets/Scripts/Logic/Entity/IPlayerReadOnly.cs`:
```csharp
namespace GaviShooting.Logic.Entity
{
    public interface IPlayerReadOnly
    {
        float X { get; }
        float Y { get; }
        int Hp { get; }
        int WeaponLevel { get; }
        bool IsAlive { get; }
    }
}
```

Create `Assets/Scripts/Logic/Entity/IPlayerViewWriter.cs`:
```csharp
namespace GaviShooting.Logic.Entity
{
    public interface IPlayerViewWriter
    {
        void PlayDamageEffect();
        void PlayDeathEffect();
    }
}
```

- [ ] **Step 2: 실패 테스트 작성**

Create `Assets/Tests/EditMode/Logic/PlayerLogicTest.cs`:
```csharp
using NUnit.Framework;
using GaviShooting.Logic.Entity;

namespace Tests.EditMode.Logic
{
    public class PlayerLogicTest
    {
        private class NullPlayerViewWriter : IPlayerViewWriter
        {
            public void PlayDamageEffect() { }
            public void PlayDeathEffect() { }
        }

        [Test]
        public void InitialHp_Is100()
        {
            var p = new PlayerLogic(0, 0f, 0f, new NullPlayerViewWriter());
            Assert.AreEqual(100, p.Hp);
        }

        [Test]
        public void TakeDamage_ReducesHp()
        {
            var p = new PlayerLogic(0, 0f, 0f, new NullPlayerViewWriter());
            p.TakeDamage(20);
            Assert.AreEqual(80, p.Hp);
        }

        [Test]
        public void TakeDamage_ToZero_IsNotAlive()
        {
            var p = new PlayerLogic(0, 0f, 0f, new NullPlayerViewWriter());
            p.TakeDamage(100);
            Assert.IsFalse(p.IsAlive);
        }

        [Test]
        public void SetDirection_Move_UpdatesPosition()
        {
            var p = new PlayerLogic(0, 0f, 0f, new NullPlayerViewWriter());
            p.SetDirection(1f, 0f);
            p.Move();
            Assert.Greater(p.X, 0f);
        }

        [Test]
        public void UpgradeWeapon_MaxLevel3()
        {
            var p = new PlayerLogic(0, 0f, 0f, new NullPlayerViewWriter());
            p.UpgradeWeapon();
            p.UpgradeWeapon();
            p.UpgradeWeapon();
            Assert.AreEqual(3, p.WeaponLevel);
        }

        [Test]
        public void Heal_ClampsToMaxHp()
        {
            var p = new PlayerLogic(0, 0f, 0f, new NullPlayerViewWriter());
            p.TakeDamage(10);
            p.Heal(50);
            Assert.AreEqual(100, p.Hp);
        }
    }
}
```

- [ ] **Step 3: PlayerLogic 구현**

Create `Assets/Scripts/Logic/Entity/PlayerLogic.cs`:
```csharp
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
                _viewWriter.PlayDeathEffect();
            }
            else
            {
                _viewWriter.PlayDamageEffect();
            }
        }

        public void Heal(int amount)
        {
            _hp += amount;
            if (_hp > MAX_HP) _hp = MAX_HP;
        }

        public void UpgradeWeapon()
        {
            if (_weaponLevel < MAX_WEAPON_LEVEL) _weaponLevel++;
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
```

- [ ] **Step 4: 테스트 실행 확인 후 커밋**
```bash
git add Assets/Scripts/Logic/Entity/ Assets/Tests/
git commit -m "add: PlayerLogic — 플레이어 로직 + 테스트"
```

---

## Task 9: Logic — BulletLogic

**Files:**
- Create: `Assets/Scripts/Logic/Entity/IBulletReadOnly.cs`
- Create: `Assets/Scripts/Logic/Entity/IBulletViewWriter.cs`
- Create: `Assets/Scripts/Logic/Entity/BulletLogic.cs`

- [ ] **Step 1: 인터페이스 작성**

Create `Assets/Scripts/Logic/Entity/IBulletReadOnly.cs`:
```csharp
namespace GaviShooting.Logic.Entity
{
    public interface IBulletReadOnly
    {
        float X { get; }
        float Y { get; }
        bool IsPlayerBullet { get; }
    }
}
```

Create `Assets/Scripts/Logic/Entity/IBulletViewWriter.cs`:
```csharp
namespace GaviShooting.Logic.Entity
{
    public interface IBulletViewWriter
    {
        void PlayHitEffect();
    }
}
```

- [ ] **Step 2: BulletLogic 구현**

Create `Assets/Scripts/Logic/Entity/BulletLogic.cs`:
```csharp
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
        }
    }
}
```

- [ ] **Step 3: 커밋**
```bash
git add Assets/Scripts/Logic/Entity/
git commit -m "add: BulletLogic — 탄환 로직"
```

---

## Task 10: Logic — EnemyLogic

**Files:**
- Create: `Assets/Scripts/Logic/Entity/IEnemyReadOnly.cs`
- Create: `Assets/Scripts/Logic/Entity/IEnemyViewWriter.cs`
- Create: `Assets/Scripts/Logic/Entity/eEnemyType.cs`
- Create: `Assets/Scripts/Logic/Entity/EnemyLogic.cs`

- [ ] **Step 1: 인터페이스 + enum 작성**

Create `Assets/Scripts/Logic/Entity/eEnemyType.cs`:
```csharp
namespace GaviShooting.Logic.Entity
{
    public enum eEnemyType
    {
        Straight,
        Wave,
        Boss
    }
}
```

Create `Assets/Scripts/Logic/Entity/IEnemyReadOnly.cs`:
```csharp
namespace GaviShooting.Logic.Entity
{
    public interface IEnemyReadOnly
    {
        float X { get; }
        float Y { get; }
        int Hp { get; }
        eEnemyType EnemyType { get; }
        bool IsAlive { get; }
    }
}
```

Create `Assets/Scripts/Logic/Entity/IEnemyViewWriter.cs`:
```csharp
namespace GaviShooting.Logic.Entity
{
    public interface IEnemyViewWriter
    {
        void PlayDeathEffect();
        void PlaySpawnEffect();
    }
}
```

- [ ] **Step 2: EnemyLogic 구현**

Create `Assets/Scripts/Logic/Entity/EnemyLogic.cs`:
```csharp
using System;
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
                _viewWriter.PlayDeathEffect();
            }
        }

        public bool CanShoot(int shootInterval)
        {
            return shootInterval > 0 && _frameCount % shootInterval == 0;
        }
    }
}
```

- [ ] **Step 3: 커밋**
```bash
git add Assets/Scripts/Logic/Entity/
git commit -m "add: EnemyLogic — 적 로직 (직진/웨이브/보스)"
```
