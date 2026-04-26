## Task 17: View — GameView (View 루트)

**Files:**
- Create: `Assets/Scripts/View/GameView.cs`

- [ ] **Step 1: GameView 구현**

Create `Assets/Scripts/View/GameView.cs`:
```csharp
using System.Collections.Generic;
using UnityEngine;
using GaviShooting.Logic.Entity;
using GaviShooting.View.Entity;

namespace GaviShooting.View
{
    public class GameView : MonoBehaviour
    {
        private EntityManager _entityManager;
        private readonly Dictionary<int, IEntityView> _activeViews = new();
        private readonly List<int> _removeBuffer = new();

        private EntityViewPool<PlayerView> _playerPool;
        private EntityViewPool<EnemyView> _enemyPool;
        private EntityViewPool<BulletView> _bulletPool;
        private EntityViewPool<ItemView> _itemPool;

        private ScrollView _scrollView;
        private UiView _uiView;

        public void Init(
            EntityManager entityManager,
            PlayerView playerPrefab,
            EnemyView enemyPrefab,
            BulletView bulletPrefab,
            ItemView itemPrefab,
            ScrollView scrollView,
            UiView uiView)
        {
            _entityManager = entityManager;
            _playerPool = new EntityViewPool<PlayerView>(playerPrefab, transform);
            _enemyPool = new EntityViewPool<EnemyView>(enemyPrefab, transform);
            _bulletPool = new EntityViewPool<BulletView>(bulletPrefab, transform);
            _itemPool = new EntityViewPool<ItemView>(itemPrefab, transform);
            _scrollView = scrollView;
            _uiView = uiView;
        }

        public PlayerView GetPlayerView() => _playerPool.Get();
        public EnemyView GetEnemyView() => _enemyPool.Get();
        public BulletView GetBulletView() => _bulletPool.Get();
        public ItemView GetItemView() => _itemPool.Get();

        public void RegisterView(int entityId, IEntityView view)
        {
            _activeViews[entityId] = view;
        }

        public void Render()
        {
            _removeBuffer.Clear();
            var entities = _entityManager.Entities;

            for (int i = 0; i < entities.Count; i++)
            {
                var e = entities[i];
                if (_activeViews.TryGetValue(e.Id, out var view))
                {
                    view.SyncPosition(e);
                }
            }

            foreach (var kvp in _activeViews)
            {
                bool found = false;
                for (int i = 0; i < entities.Count; i++)
                {
                    if (entities[i].Id == kvp.Key) { found = true; break; }
                }
                if (!found) _removeBuffer.Add(kvp.Key);
            }

            for (int i = 0; i < _removeBuffer.Count; i++)
            {
                var view = _activeViews[_removeBuffer[i]];
                view.Deactivate();
                _activeViews.Remove(_removeBuffer[i]);
            }

            _scrollView.Render();
        }

        public void Clear()
        {
            foreach (var kvp in _activeViews)
            {
                kvp.Value.Deactivate();
            }
            _activeViews.Clear();
        }
    }
}
```

- [ ] **Step 2: 커밋**
```bash
git add Assets/Scripts/View/GameView.cs
git commit -m "add: GameView — View 루트, 엔티티 View 동기화"
```

---

## Task 18: Entry — GameEntry + FSM 상태 구현

**Files:**
- Create: `Assets/Scripts/Entry/GaviShooting.Entry.asmdef`
- Create: `Assets/Scripts/Entry/GameEntry.cs`
- Create: `Assets/Scripts/Logic/Fsm/ReadyState.cs`
- Create: `Assets/Scripts/Logic/Fsm/PlayingState.cs`
- Create: `Assets/Scripts/Logic/Fsm/PausedState.cs`
- Create: `Assets/Scripts/Logic/Fsm/GameOverState.cs`
- Create: `Assets/Scripts/Logic/Fsm/StageClearState.cs`

- [ ] **Step 1: Entry asmdef**

Create `Assets/Scripts/Entry/GaviShooting.Entry.asmdef`:
```json
{
    "name": "GaviShooting.Entry",
    "rootNamespace": "GaviShooting.Entry",
    "references": ["GaviShooting.Core", "GaviShooting.Logic", "GaviShooting.View"],
    "includePlatforms": [],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "overrideReferences": false,
    "precompiledReferences": [],
    "autoReferenced": true,
    "defineConstraints": [],
    "versionDefines": [],
    "noEngineReferences": false
}
```

- [ ] **Step 2: FSM 상태 클래스들**

Create `Assets/Scripts/Logic/Fsm/ReadyState.cs`:
```csharp
using GaviShooting.View;

namespace GaviShooting.Logic.Fsm
{
    public class ReadyState : IState
    {
        private readonly UiView _uiView;
        public ReadyState(UiView uiView) { _uiView = uiView; }
        public void Enter() => _uiView.ShowReady();
        public void Exit() => _uiView.HideAll();
        public void LogicUpdate() { }
    }
}
```

Create `Assets/Scripts/Logic/Fsm/PlayingState.cs`:
```csharp
using GaviShooting.Logic.Entity;
using GaviShooting.Logic.Collision;
using GaviShooting.Logic.Stage;
using GaviShooting.Core;
using GaviShooting.View;
using System.Collections.Generic;

namespace GaviShooting.Logic.Fsm
{
    public class PlayingState : IState
    {
        private readonly EntityManager _entityManager;
        private readonly CollisionSystem _collisionSystem;
        private readonly ICommandQueue _commandQueue;
        private readonly StageTimeline _stageTimeline;
        private readonly PlayerLogic _player;
        private readonly ScrollView _scrollView;
        private readonly UiView _uiView;
        private readonly StateMachine _fsm;
        private readonly System.Func<eEnemyType, float, float, EnemyLogic> _enemyFactory;
        private int _score;

        public int Score => _score;

        public PlayingState(
            EntityManager entityManager,
            CollisionSystem collisionSystem,
            ICommandQueue commandQueue,
            StageTimeline stageTimeline,
            PlayerLogic player,
            ScrollView scrollView,
            UiView uiView,
            StateMachine fsm,
            System.Func<eEnemyType, float, float, EnemyLogic> enemyFactory)
        {
            _entityManager = entityManager;
            _collisionSystem = collisionSystem;
            _commandQueue = commandQueue;
            _stageTimeline = stageTimeline;
            _player = player;
            _scrollView = scrollView;
            _uiView = uiView;
            _fsm = fsm;
            _enemyFactory = enemyFactory;
        }

        public void Enter()
        {
            _scrollView.SetScrolling(true);
            _uiView.HideAll();
        }

        public void Exit()
        {
            _scrollView.SetScrolling(false);
        }

        public void LogicUpdate()
        {
            _entityManager.ProcessQueue();
            _entityManager.MoveAll();
            _commandQueue.ProcessAll();

            var spawns = _stageTimeline.Advance();
            for (int i = 0; i < spawns.Count; i++)
            {
                var s = spawns[i];
                var enemy = _enemyFactory(s.EnemyType, s.X, s.Y);
                _entityManager.EnqueueSpawn(enemy);
            }

            if (_stageTimeline.IsBossTime)
                _scrollView.SetScrolling(false);

            var pairs = _collisionSystem.CheckAll(_entityManager.GetCollidables());
            handleCollisions(pairs);

            _entityManager.UpdateAll();
            _entityManager.RemoveDead();

            _uiView.UpdateHp(_player.Hp, 100);
            _uiView.UpdateScore(_score);
            _uiView.UpdateWeaponLevel(_player.WeaponLevel);

            if (!_player.IsAlive)
                _fsm.ChangeState(eGameState.GameOver);
        }

        private void handleCollisions(List<CollisionPair> pairs)
        {
            for (int i = 0; i < pairs.Count; i++)
            {
                var a = pairs[i].A;
                var b = pairs[i].B;
                resolveCollision(a, b);
            }
        }

        private void resolveCollision(ICollidable a, ICollidable b)
        {
            if (a.Layer > b.Layer) (a, b) = (b, a);

            switch (a.Layer, b.Layer)
            {
                case (eCollisionLayer.Player, eCollisionLayer.Enemy):
                    _player.TakeDamage(((EnemyLogic)b).ContactDamage);
                    break;
                case (eCollisionLayer.Player, eCollisionLayer.EnemyBullet):
                    _player.TakeDamage(((BulletLogic)b).Damage);
                    ((BulletLogic)b).Kill();
                    break;
                case (eCollisionLayer.Player, eCollisionLayer.Item):
                    var item = (ItemLogic)b;
                    if (item.ItemType == eItemType.WeaponUpgrade) _player.UpgradeWeapon();
                    else _player.Heal(30);
                    item.Pickup();
                    break;
                case (eCollisionLayer.PlayerBullet, eCollisionLayer.Enemy):
                    var bullet = (BulletLogic)a;
                    var enemy = (EnemyLogic)b;
                    enemy.TakeDamage(bullet.Damage);
                    bullet.Kill();
                    if (!enemy.IsAlive)
                    {
                        _score += enemy.Score;
                        if (enemy.EnemyType == eEnemyType.Boss)
                            _fsm.ChangeState(eGameState.StageClear);
                    }
                    break;
            }
        }
    }
}
```

Create `Assets/Scripts/Logic/Fsm/PausedState.cs`:
```csharp
using GaviShooting.View;

namespace GaviShooting.Logic.Fsm
{
    public class PausedState : IState
    {
        private readonly UiView _uiView;
        public PausedState(UiView uiView) { _uiView = uiView; }
        public void Enter() => _uiView.ShowPause();
        public void Exit() => _uiView.HideAll();
        public void LogicUpdate() { }
    }
}
```

Create `Assets/Scripts/Logic/Fsm/GameOverState.cs`:
```csharp
using GaviShooting.View;

namespace GaviShooting.Logic.Fsm
{
    public class GameOverState : IState
    {
        private readonly UiView _uiView;
        public GameOverState(UiView uiView) { _uiView = uiView; }
        public void Enter() => _uiView.ShowGameOver();
        public void Exit() => _uiView.HideAll();
        public void LogicUpdate() { }
    }
}
```

Create `Assets/Scripts/Logic/Fsm/StageClearState.cs`:
```csharp
using GaviShooting.View;

namespace GaviShooting.Logic.Fsm
{
    public class StageClearState : IState
    {
        private readonly UiView _uiView;
        public StageClearState(UiView uiView) { _uiView = uiView; }
        public void Enter() => _uiView.ShowStageClear();
        public void Exit() => _uiView.HideAll();
        public void LogicUpdate() { }
    }
}
```

- [ ] **Step 3: 커밋**
```bash
git add Assets/Scripts/Logic/Fsm/ Assets/Scripts/Entry/
git commit -m "add: FSM 상태 클래스 + Entry asmdef"
```
