## Task 11: Logic — ItemLogic

**Files:**
- Create: `Assets/Scripts/Logic/Entity/eItemType.cs`
- Create: `Assets/Scripts/Logic/Entity/IItemReadOnly.cs`
- Create: `Assets/Scripts/Logic/Entity/IItemViewWriter.cs`
- Create: `Assets/Scripts/Logic/Entity/ItemLogic.cs`

- [ ] **Step 1: enum + 인터페이스**

Create `Assets/Scripts/Logic/Entity/eItemType.cs`:
```csharp
namespace GaviShooting.Logic.Entity
{
    public enum eItemType
    {
        WeaponUpgrade,
        HpRecover
    }
}
```

Create `Assets/Scripts/Logic/Entity/IItemReadOnly.cs`:
```csharp
namespace GaviShooting.Logic.Entity
{
    public interface IItemReadOnly
    {
        float X { get; }
        float Y { get; }
        eItemType ItemType { get; }
    }
}
```

Create `Assets/Scripts/Logic/Entity/IItemViewWriter.cs`:
```csharp
namespace GaviShooting.Logic.Entity
{
    public interface IItemViewWriter
    {
        void PlayPickupEffect();
    }
}
```

- [ ] **Step 2: ItemLogic 구현**

Create `Assets/Scripts/Logic/Entity/ItemLogic.cs`:
```csharp
using GaviShooting.Logic.Collision;

namespace GaviShooting.Logic.Entity
{
    public class ItemLogic : IEntity, IItemReadOnly
    {
        private const float MOVE_SPEED = 1.5f / 60f;
        private const float HIT_RADIUS = 0.4f;
        private const float SCREEN_LEFT = -12f;

        private readonly int _id;
        private readonly eItemType _itemType;
        private readonly IItemViewWriter _viewWriter;
        private float _x;
        private float _y;
        private bool _dead;

        public ItemLogic(int id, eItemType itemType, float x, float y, IItemViewWriter viewWriter)
        {
            _id = id;
            _itemType = itemType;
            _x = x;
            _y = y;
            _viewWriter = viewWriter;
        }

        public int Id => _id;
        public float X => _x;
        public float Y => _y;
        public eItemType ItemType => _itemType;
        public bool IsAlive => !_dead;
        public bool ShouldRemove => _dead;
        public HitBox HitBox => new(_x, _y, HIT_RADIUS);
        public eCollisionLayer Layer => eCollisionLayer.Item;

        public void Move()
        {
            _x -= MOVE_SPEED;
        }

        public void Update()
        {
            if (_x < SCREEN_LEFT) _dead = true;
        }

        public void Pickup()
        {
            _dead = true;
            _viewWriter.PlayPickupEffect();
        }
    }
}
```

- [ ] **Step 3: 커밋**
```bash
git add Assets/Scripts/Logic/Entity/
git commit -m "add: ItemLogic — 아이템 로직"
```

---

## Task 12: Logic — Commands

**Files:**
- Create: `Assets/Scripts/Logic/Command/MoveCommand.cs`
- Create: `Assets/Scripts/Logic/Command/ShootCommand.cs`
- Create: `Assets/Scripts/Logic/Command/DamageCommand.cs`
- Create: `Assets/Scripts/Logic/Command/SpawnCommand.cs`
- Create: `Assets/Scripts/Logic/Command/ItemPickupCommand.cs`
- Create: `Assets/Scripts/Logic/Command/GameStartCommand.cs`
- Create: `Assets/Scripts/Logic/Command/GameOverCommand.cs`
- Create: `Assets/Scripts/Logic/Command/StageClearCommand.cs`
- Create: `Assets/Scripts/Logic/Command/PauseCommand.cs`

- [ ] **Step 1: MoveCommand**

Create `Assets/Scripts/Logic/Command/MoveCommand.cs`:
```csharp
using GaviShooting.Core;
using GaviShooting.Logic.Entity;

namespace GaviShooting.Logic.Command
{
    public class MoveCommand : ICommand
    {
        private readonly PlayerLogic _player;
        private readonly float _dx;
        private readonly float _dy;

        public MoveCommand(PlayerLogic player, float dx, float dy)
        {
            _player = player;
            _dx = dx;
            _dy = dy;
        }

        public void Execute()
        {
            _player.SetDirection(_dx, _dy);
        }
    }
}
```

- [ ] **Step 2: ShootCommand**

Create `Assets/Scripts/Logic/Command/ShootCommand.cs`:
```csharp
using GaviShooting.Core;
using GaviShooting.Logic.Entity;

namespace GaviShooting.Logic.Command
{
    public class ShootCommand : ICommand
    {
        private readonly PlayerLogic _player;
        private readonly EntityManager _entityManager;
        private readonly IBulletViewWriter _bulletViewWriter;

        private const float BULLET_SPEED = 10f / 60f;
        private const int BULLET_DAMAGE = 10;
        private const float SPREAD_ANGLE = 0.15f;

        public ShootCommand(PlayerLogic player, EntityManager entityManager, IBulletViewWriter bulletViewWriter)
        {
            _player = player;
            _entityManager = entityManager;
            _bulletViewWriter = bulletViewWriter;
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
        }

        private void spawnBullet(float x, float y, float speed, float angleY)
        {
            var bullet = new BulletLogic(0, x + 0.5f, y, speed, angleY, true, BULLET_DAMAGE, _bulletViewWriter);
            _entityManager.EnqueueSpawn(bullet);
        }
    }
}
```

- [ ] **Step 3: DamageCommand**

Create `Assets/Scripts/Logic/Command/DamageCommand.cs`:
```csharp
using GaviShooting.Core;
using GaviShooting.Logic.Entity;

namespace GaviShooting.Logic.Command
{
    public class DamageCommand : ICommand
    {
        private readonly PlayerLogic _player;
        private readonly int _amount;

        public DamageCommand(PlayerLogic player, int amount)
        {
            _player = player;
            _amount = amount;
        }

        public void Execute()
        {
            _player.TakeDamage(_amount);
        }
    }
}
```

- [ ] **Step 4: SpawnCommand**

Create `Assets/Scripts/Logic/Command/SpawnCommand.cs`:
```csharp
using GaviShooting.Core;
using GaviShooting.Logic.Entity;

namespace GaviShooting.Logic.Command
{
    public class SpawnCommand : ICommand
    {
        private readonly EntityManager _entityManager;
        private readonly IEntity _entity;

        public SpawnCommand(EntityManager entityManager, IEntity entity)
        {
            _entityManager = entityManager;
            _entity = entity;
        }

        public void Execute()
        {
            _entityManager.EnqueueSpawn(_entity);
        }
    }
}
```

- [ ] **Step 5: ItemPickupCommand**

Create `Assets/Scripts/Logic/Command/ItemPickupCommand.cs`:
```csharp
using GaviShooting.Core;
using GaviShooting.Logic.Entity;

namespace GaviShooting.Logic.Command
{
    public class ItemPickupCommand : ICommand
    {
        private readonly PlayerLogic _player;
        private readonly ItemLogic _item;

        private const int HEAL_AMOUNT = 30;

        public ItemPickupCommand(PlayerLogic player, ItemLogic item)
        {
            _player = player;
            _item = item;
        }

        public void Execute()
        {
            switch (_item.ItemType)
            {
                case eItemType.WeaponUpgrade:
                    _player.UpgradeWeapon();
                    break;
                case eItemType.HpRecover:
                    _player.Heal(HEAL_AMOUNT);
                    break;
            }
            _item.Pickup();
        }
    }
}
```

- [ ] **Step 6: FSM 전환 커맨드들**

Create `Assets/Scripts/Logic/Command/GameStartCommand.cs`:
```csharp
using GaviShooting.Core;
using GaviShooting.Logic.Fsm;

namespace GaviShooting.Logic.Command
{
    public class GameStartCommand : ICommand
    {
        private readonly StateMachine _fsm;
        public GameStartCommand(StateMachine fsm) { _fsm = fsm; }
        public void Execute() => _fsm.ChangeState(eGameState.Playing);
    }
}
```

Create `Assets/Scripts/Logic/Command/GameOverCommand.cs`:
```csharp
using GaviShooting.Core;
using GaviShooting.Logic.Fsm;

namespace GaviShooting.Logic.Command
{
    public class GameOverCommand : ICommand
    {
        private readonly StateMachine _fsm;
        public GameOverCommand(StateMachine fsm) { _fsm = fsm; }
        public void Execute() => _fsm.ChangeState(eGameState.GameOver);
    }
}
```

Create `Assets/Scripts/Logic/Command/StageClearCommand.cs`:
```csharp
using GaviShooting.Core;
using GaviShooting.Logic.Fsm;

namespace GaviShooting.Logic.Command
{
    public class StageClearCommand : ICommand
    {
        private readonly StateMachine _fsm;
        public StageClearCommand(StateMachine fsm) { _fsm = fsm; }
        public void Execute() => _fsm.ChangeState(eGameState.StageClear);
    }
}
```

Create `Assets/Scripts/Logic/Command/PauseCommand.cs`:
```csharp
using GaviShooting.Core;
using GaviShooting.Logic.Fsm;

namespace GaviShooting.Logic.Command
{
    public class PauseCommand : ICommand
    {
        private readonly StateMachine _fsm;
        public PauseCommand(StateMachine fsm) { _fsm = fsm; }

        public void Execute()
        {
            if (_fsm.CurrentState == eGameState.Playing)
                _fsm.ChangeState(eGameState.Paused);
            else if (_fsm.CurrentState == eGameState.Paused)
                _fsm.ChangeState(eGameState.Playing);
        }
    }
}
```

- [ ] **Step 7: 커밋**
```bash
git add Assets/Scripts/Logic/Command/
git commit -m "add: Commands — 모든 커맨드 구현"
```
