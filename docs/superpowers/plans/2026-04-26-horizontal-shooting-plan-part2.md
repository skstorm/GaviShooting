## Task 4: Core — GameLoop

**Files:**
- Create: `Assets/Scripts/Core/IGameLoop.cs`
- Create: `Assets/Scripts/Core/GameLoop.cs`

- [ ] **Step 1: IGameLoop 인터페이스 작성**

Create `Assets/Scripts/Core/IGameLoop.cs`:
```csharp
namespace GaviShooting.Core
{
    public interface IGameLoop
    {
        void LogicUpdate();
        void Render();
    }
}
```

- [ ] **Step 2: GameLoop 구현**

Create `Assets/Scripts/Core/GameLoop.cs`:
```csharp
namespace GaviShooting.Core
{
    public class GameLoop
    {
        private readonly FrameManager _frameManager;
        private readonly IGameLoop _logic;

        public GameLoop(FrameManager frameManager, IGameLoop logic)
        {
            _frameManager = frameManager;
            _logic = logic;
        }

        public void Update(float deltaTime)
        {
            int deltaFrame = _frameManager.CalcDeltaFrame(deltaTime);
            for (int i = 0; i < deltaFrame; i++)
            {
                _logic.LogicUpdate();
            }
            if (deltaFrame > 0)
            {
                _logic.Render();
            }
        }
    }
}
```

- [ ] **Step 3: 커밋**
```bash
git add Assets/Scripts/Core/
git commit -m "add: GameLoop — 고정 프레임 루프"
```

---

## Task 5: Logic — FSM (게임 상태 머신)

**Files:**
- Create: `Assets/Scripts/Logic/GaviShooting.Logic.asmdef`
- Create: `Assets/Scripts/Logic/Fsm/IState.cs`
- Create: `Assets/Scripts/Logic/Fsm/StateMachine.cs`
- Create: `Assets/Scripts/Logic/Fsm/eGameState.cs`

- [ ] **Step 1: Logic asmdef 생성**

Create `Assets/Scripts/Logic/GaviShooting.Logic.asmdef`:
```json
{
    "name": "GaviShooting.Logic",
    "rootNamespace": "GaviShooting.Logic",
    "references": ["GaviShooting.Core"],
    "includePlatforms": [],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "overrideReferences": false,
    "precompiledReferences": [],
    "autoReferenced": true,
    "defineConstraints": [],
    "versionDefines": [],
    "noEngineReferences": true
}
```

- [ ] **Step 2: eGameState 열거형**

Create `Assets/Scripts/Logic/Fsm/eGameState.cs`:
```csharp
namespace GaviShooting.Logic.Fsm
{
    public enum eGameState
    {
        Ready,
        Playing,
        Paused,
        GameOver,
        StageClear
    }
}
```

- [ ] **Step 3: IState 인터페이스**

Create `Assets/Scripts/Logic/Fsm/IState.cs`:
```csharp
namespace GaviShooting.Logic.Fsm
{
    public interface IState
    {
        void Enter();
        void Exit();
        void LogicUpdate();
    }
}
```

- [ ] **Step 4: StateMachine 구현**

Create `Assets/Scripts/Logic/Fsm/StateMachine.cs`:
```csharp
using System.Collections.Generic;

namespace GaviShooting.Logic.Fsm
{
    public class StateMachine
    {
        private readonly Dictionary<eGameState, IState> _states = new();
        private IState _current;
        private eGameState _currentKey;

        public eGameState CurrentState => _currentKey;

        public void AddState(eGameState key, IState state)
        {
            _states[key] = state;
        }

        public void ChangeState(eGameState key)
        {
            _current?.Exit();
            _currentKey = key;
            _current = _states[key];
            _current.Enter();
        }

        public void LogicUpdate()
        {
            _current?.LogicUpdate();
        }
    }
}
```

- [ ] **Step 5: 커밋**
```bash
git add Assets/Scripts/Logic/
git commit -m "add: FSM — 게임 상태 머신"
```

---

## Task 6: Logic — HitBox + CollisionSystem + 테스트

**Files:**
- Create: `Assets/Scripts/Logic/Collision/HitBox.cs`
- Create: `Assets/Scripts/Logic/Collision/CollisionSystem.cs`
- Create: `Assets/Tests/EditMode/Logic/CollisionSystemTest.cs`

- [ ] **Step 1: HitBox 구현**

Create `Assets/Scripts/Logic/Collision/HitBox.cs`:
```csharp
namespace GaviShooting.Logic.Collision
{
    public struct HitBox
    {
        public float X;
        public float Y;
        public float Radius;

        public HitBox(float x, float y, float radius)
        {
            X = x;
            Y = y;
            Radius = radius;
        }

        public bool Overlaps(in HitBox other)
        {
            float dx = X - other.X;
            float dy = Y - other.Y;
            float sumR = Radius + other.Radius;
            return dx * dx + dy * dy <= sumR * sumR;
        }
    }
}
```

- [ ] **Step 2: 실패 테스트 작성**

Create `Assets/Tests/EditMode/Logic/CollisionSystemTest.cs`:
```csharp
using NUnit.Framework;
using GaviShooting.Logic.Collision;

namespace Tests.EditMode.Logic
{
    public class CollisionSystemTest
    {
        [Test]
        public void HitBox_Overlapping_ReturnsTrue()
        {
            var a = new HitBox(0f, 0f, 1f);
            var b = new HitBox(1.5f, 0f, 1f);
            Assert.IsTrue(a.Overlaps(b));
        }

        [Test]
        public void HitBox_NotOverlapping_ReturnsFalse()
        {
            var a = new HitBox(0f, 0f, 1f);
            var b = new HitBox(3f, 0f, 1f);
            Assert.IsFalse(a.Overlaps(b));
        }

        [Test]
        public void HitBox_Touching_ReturnsTrue()
        {
            var a = new HitBox(0f, 0f, 1f);
            var b = new HitBox(2f, 0f, 1f);
            Assert.IsTrue(a.Overlaps(b));
        }
    }
}
```

- [ ] **Step 3: CollisionSystem 구현**

Create `Assets/Scripts/Logic/Collision/CollisionSystem.cs`:
```csharp
using System.Collections.Generic;

namespace GaviShooting.Logic.Collision
{
    public enum eCollisionLayer
    {
        Player,
        PlayerBullet,
        Enemy,
        EnemyBullet,
        Item
    }

    public interface ICollidable
    {
        HitBox HitBox { get; }
        eCollisionLayer Layer { get; }
        bool IsAlive { get; }
    }

    public struct CollisionPair
    {
        public ICollidable A;
        public ICollidable B;
    }

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
```

- [ ] **Step 4: 테스트 실행 확인 후 커밋**
```bash
git add Assets/Scripts/Logic/Collision/ Assets/Tests/
git commit -m "add: HitBox + CollisionSystem — 충돌 판정 + 테스트"
```

---

## Task 7: Logic — IEntity + EntityManager

**Files:**
- Create: `Assets/Scripts/Logic/Entity/IEntity.cs`
- Create: `Assets/Scripts/Logic/Entity/EntityManager.cs`

- [ ] **Step 1: IEntity 인터페이스**

Create `Assets/Scripts/Logic/Entity/IEntity.cs`:
```csharp
using GaviShooting.Logic.Collision;

namespace GaviShooting.Logic.Entity
{
    public interface IEntity : ICollidable
    {
        int Id { get; }
        float X { get; }
        float Y { get; }
        void Move();
        void Update();
        bool ShouldRemove { get; }
    }
}
```

- [ ] **Step 2: EntityManager 구현**

Create `Assets/Scripts/Logic/Entity/EntityManager.cs`:
```csharp
using System.Collections.Generic;
using GaviShooting.Logic.Collision;

namespace GaviShooting.Logic.Entity
{
    public class EntityManager
    {
        private readonly List<IEntity> _entities = new();
        private readonly Queue<IEntity> _spawnQueue = new();
        private readonly List<ICollidable> _collidableCache = new();
        private int _nextId;

        public IReadOnlyList<IEntity> Entities => _entities;

        public int EnqueueSpawn(IEntity entity)
        {
            _spawnQueue.Enqueue(entity);
            return _nextId++;
        }

        public void ProcessQueue()
        {
            while (_spawnQueue.Count > 0)
            {
                _entities.Add(_spawnQueue.Dequeue());
            }
        }

        public void MoveAll()
        {
            for (int i = 0; i < _entities.Count; i++)
            {
                _entities[i].Move();
            }
        }

        public void UpdateAll()
        {
            for (int i = 0; i < _entities.Count; i++)
            {
                _entities[i].Update();
            }
        }

        public void RemoveDead()
        {
            for (int i = _entities.Count - 1; i >= 0; i--)
            {
                if (_entities[i].ShouldRemove)
                {
                    _entities.RemoveAt(i);
                }
            }
        }

        public List<ICollidable> GetCollidables()
        {
            _collidableCache.Clear();
            for (int i = 0; i < _entities.Count; i++)
            {
                _collidableCache.Add(_entities[i]);
            }
            return _collidableCache;
        }

        public void Clear()
        {
            _entities.Clear();
            _spawnQueue.Clear();
        }
    }
}
```

- [ ] **Step 3: 커밋**
```bash
git add Assets/Scripts/Logic/Entity/
git commit -m "add: IEntity + EntityManager — 엔티티 생명주기 관리"
```
