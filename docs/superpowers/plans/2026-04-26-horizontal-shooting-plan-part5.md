## Task 13: Logic — StageTimeline + 테스트

**Files:**
- Create: `Assets/Scripts/Logic/Stage/SpawnEvent.cs`
- Create: `Assets/Scripts/Logic/Stage/StageData.cs`
- Create: `Assets/Scripts/Logic/Stage/StageTimeline.cs`
- Create: `Assets/Tests/EditMode/Logic/StageTimelineTest.cs`

- [ ] **Step 1: SpawnEvent + StageData**

Create `Assets/Scripts/Logic/Stage/SpawnEvent.cs`:
```csharp
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
```

Create `Assets/Scripts/Logic/Stage/StageData.cs`:
```csharp
using System.Collections.Generic;

namespace GaviShooting.Logic.Stage
{
    public class StageData
    {
        public string StageName;
        public int TotalFrames;
        public float ScrollSpeed;
        public int BossFrame;
        public List<SpawnEvent> SpawnEvents;
    }
}
```

- [ ] **Step 2: 실패 테스트 작성**

Create `Assets/Tests/EditMode/Logic/StageTimelineTest.cs`:
```csharp
using System.Collections.Generic;
using NUnit.Framework;
using GaviShooting.Logic.Entity;
using GaviShooting.Logic.Stage;

namespace Tests.EditMode.Logic
{
    public class StageTimelineTest
    {
        private StageData createTestData()
        {
            return new StageData
            {
                StageName = "Test",
                TotalFrames = 120,
                ScrollSpeed = 2f,
                BossFrame = 100,
                SpawnEvents = new List<SpawnEvent>
                {
                    new() { Frame = 10, EnemyType = eEnemyType.Straight, X = 10f, Y = 3f },
                    new() { Frame = 10, EnemyType = eEnemyType.Straight, X = 10f, Y = -3f },
                    new() { Frame = 50, EnemyType = eEnemyType.Wave, X = 10f, Y = 0f },
                }
            };
        }

        [Test]
        public void Advance_ReturnsSpawnsAtFrame()
        {
            var timeline = new StageTimeline(createTestData());
            var spawns = timeline.Advance();
            Assert.AreEqual(0, spawns.Count);

            for (int i = 0; i < 9; i++) timeline.Advance();
            spawns = timeline.Advance();
            Assert.AreEqual(2, spawns.Count);
        }

        [Test]
        public void IsBossTime_TrueAtBossFrame()
        {
            var timeline = new StageTimeline(createTestData());
            for (int i = 0; i < 100; i++) timeline.Advance();
            Assert.IsTrue(timeline.IsBossTime);
        }

        [Test]
        public void IsFinished_TrueAtTotalFrames()
        {
            var timeline = new StageTimeline(createTestData());
            for (int i = 0; i < 120; i++) timeline.Advance();
            Assert.IsTrue(timeline.IsFinished);
        }
    }
}
```

- [ ] **Step 3: StageTimeline 구현**

Create `Assets/Scripts/Logic/Stage/StageTimeline.cs`:
```csharp
using System.Collections.Generic;

namespace GaviShooting.Logic.Stage
{
    public class StageTimeline
    {
        private readonly StageData _data;
        private readonly List<SpawnEvent> _result = new();
        private int _currentFrame;

        public StageTimeline(StageData data)
        {
            _data = data;
        }

        public int CurrentFrame => _currentFrame;
        public bool IsBossTime => _currentFrame >= _data.BossFrame;
        public bool IsFinished => _currentFrame >= _data.TotalFrames;
        public float ScrollSpeed => _data.ScrollSpeed;

        public List<SpawnEvent> Advance()
        {
            _result.Clear();
            _currentFrame++;
            for (int i = 0; i < _data.SpawnEvents.Count; i++)
            {
                if (_data.SpawnEvents[i].Frame == _currentFrame)
                {
                    _result.Add(_data.SpawnEvents[i]);
                }
            }
            return _result;
        }

        public void Reset()
        {
            _currentFrame = 0;
        }
    }
}
```

- [ ] **Step 4: 테스트 실행 확인 후 커밋**
```bash
git add Assets/Scripts/Logic/Stage/ Assets/Tests/
git commit -m "add: StageTimeline — 스테이지 타임라인 + 테스트"
```

---

## Task 14: View — asmdef + EntityView 인터페이스 + EntityViewPool

**Files:**
- Create: `Assets/Scripts/View/GaviShooting.View.asmdef`
- Create: `Assets/Scripts/View/Entity/IEntityView.cs`
- Create: `Assets/Scripts/View/Entity/EntityViewPool.cs`

- [ ] **Step 1: View asmdef**

Create `Assets/Scripts/View/GaviShooting.View.asmdef`:
```json
{
    "name": "GaviShooting.View",
    "rootNamespace": "GaviShooting.View",
    "references": ["GaviShooting.Core", "GaviShooting.Logic"],
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

- [ ] **Step 2: IEntityView + EntityViewPool**

Create `Assets/Scripts/View/Entity/IEntityView.cs`:
```csharp
using GaviShooting.Logic.Entity;

namespace GaviShooting.View.Entity
{
    public interface IEntityView
    {
        void Activate(IEntity entity);
        void SyncPosition(IEntity entity);
        void Deactivate();
    }
}
```

Create `Assets/Scripts/View/Entity/EntityViewPool.cs`:
```csharp
using System.Collections.Generic;
using UnityEngine;

namespace GaviShooting.View.Entity
{
    public class EntityViewPool<TView> where TView : MonoBehaviour, IEntityView
    {
        private readonly TView _prefab;
        private readonly Transform _parent;
        private readonly Stack<TView> _pool = new();

        public EntityViewPool(TView prefab, Transform parent)
        {
            _prefab = prefab;
            _parent = parent;
        }

        public TView Get()
        {
            var view = _pool.Count > 0 ? _pool.Pop() : Object.Instantiate(_prefab, _parent);
            view.gameObject.SetActive(true);
            return view;
        }

        public void Return(TView view)
        {
            view.Deactivate();
            view.gameObject.SetActive(false);
            _pool.Push(view);
        }
    }
}
```

- [ ] **Step 3: 커밋**
```bash
git add Assets/Scripts/View/
git commit -m "add: IEntityView + EntityViewPool — View 기반 인프라"
```

---

## Task 15: View — PlayerView, EnemyView, BulletView, ItemView

**Files:**
- Create: `Assets/Scripts/View/Entity/PlayerView.cs`
- Create: `Assets/Scripts/View/Entity/EnemyView.cs`
- Create: `Assets/Scripts/View/Entity/BulletView.cs`
- Create: `Assets/Scripts/View/Entity/ItemView.cs`

- [ ] **Step 1: PlayerView**

Create `Assets/Scripts/View/Entity/PlayerView.cs`:
```csharp
using GaviShooting.Logic.Entity;
using UnityEngine;

namespace GaviShooting.View.Entity
{
    public class PlayerView : MonoBehaviour, IEntityView, IPlayerViewWriter
    {
        public void Activate(IEntity entity)
        {
            SyncPosition(entity);
        }

        public void SyncPosition(IEntity entity)
        {
            transform.position = new Vector3(entity.X, entity.Y, 0f);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public void PlayDamageEffect() { }
        public void PlayDeathEffect() { }
    }
}
```

- [ ] **Step 2: EnemyView**

Create `Assets/Scripts/View/Entity/EnemyView.cs`:
```csharp
using GaviShooting.Logic.Entity;
using UnityEngine;

namespace GaviShooting.View.Entity
{
    public class EnemyView : MonoBehaviour, IEntityView, IEnemyViewWriter
    {
        public void Activate(IEntity entity)
        {
            SyncPosition(entity);
        }

        public void SyncPosition(IEntity entity)
        {
            transform.position = new Vector3(entity.X, entity.Y, 0f);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public void PlayDeathEffect() { }
        public void PlaySpawnEffect() { }
    }
}
```

- [ ] **Step 3: BulletView**

Create `Assets/Scripts/View/Entity/BulletView.cs`:
```csharp
using GaviShooting.Logic.Entity;
using UnityEngine;

namespace GaviShooting.View.Entity
{
    public class BulletView : MonoBehaviour, IEntityView, IBulletViewWriter
    {
        public void Activate(IEntity entity)
        {
            SyncPosition(entity);
        }

        public void SyncPosition(IEntity entity)
        {
            transform.position = new Vector3(entity.X, entity.Y, 0f);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public void PlayHitEffect() { }
    }
}
```

- [ ] **Step 4: ItemView**

Create `Assets/Scripts/View/Entity/ItemView.cs`:
```csharp
using GaviShooting.Logic.Entity;
using UnityEngine;

namespace GaviShooting.View.Entity
{
    public class ItemView : MonoBehaviour, IEntityView, IItemViewWriter
    {
        public void Activate(IEntity entity)
        {
            SyncPosition(entity);
        }

        public void SyncPosition(IEntity entity)
        {
            transform.position = new Vector3(entity.X, entity.Y, 0f);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public void PlayPickupEffect() { }
    }
}
```

- [ ] **Step 5: 커밋**
```bash
git add Assets/Scripts/View/Entity/
git commit -m "add: PlayerView/EnemyView/BulletView/ItemView — 엔티티 View"
```
