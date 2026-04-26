## Task 19: Entry — GameEntry (진입점, 모든 것을 조립)

**Files:**
- Create: `Assets/Scripts/Entry/GameEntry.cs`

- [ ] **Step 1: GameEntry 구현**

Create `Assets/Scripts/Entry/GameEntry.cs`:
```csharp
using UnityEngine;
using GaviShooting.Core;
using GaviShooting.Logic.Collision;
using GaviShooting.Logic.Entity;
using GaviShooting.Logic.Fsm;
using GaviShooting.Logic.Stage;
using GaviShooting.View;
using GaviShooting.View.Entity;

namespace GaviShooting.Entry
{
    public class GameEntry : MonoBehaviour
    {
        [SerializeField] private PlayerView _playerPrefab;
        [SerializeField] private EnemyView _enemyPrefab;
        [SerializeField] private BulletView _bulletPrefab;
        [SerializeField] private ItemView _itemPrefab;
        [SerializeField] private ScrollView _scrollView;
        [SerializeField] private UiView _uiView;
        [SerializeField] private Transform _gameViewRoot;

        private FrameManager _frameManager;
        private CommandQueue _commandQueue;
        private EntityManager _entityManager;
        private CollisionSystem _collisionSystem;
        private StateMachine _fsm;
        private StageTimeline _stageTimeline;
        private PlayerLogic _player;
        private GameView _gameView;
        private InputView _inputView;

        private const int FPS = 60;

        private void Start()
        {
            _frameManager = new FrameManager(FPS);
            _commandQueue = new CommandQueue();
            _entityManager = new EntityManager();
            _collisionSystem = new CollisionSystem();
            _fsm = new StateMachine();

            var stageData = new StageData
            {
                StageName = "Stage 1",
                TotalFrames = 3600,
                ScrollSpeed = 2f,
                BossFrame = 3000,
                SpawnEvents = createDefaultSpawnEvents()
            };
            _stageTimeline = new StageTimeline(stageData);

            _gameView = _gameViewRoot.gameObject.AddComponent<GameView>();
            _gameView.Init(_entityManager, _playerPrefab, _enemyPrefab, _bulletPrefab, _itemPrefab, _scrollView, _uiView);

            _scrollView.Init(stageData.ScrollSpeed);

            var playerView = _gameView.GetPlayerView();
            _player = new PlayerLogic(0, -6f, 0f, playerView);
            _entityManager.EnqueueSpawn(_player);
            _entityManager.ProcessQueue();
            playerView.Activate(_player);
            _gameView.RegisterView(_player.Id, playerView);

            var bulletViewWriter = _gameView.GetBulletView();
            bulletViewWriter.gameObject.SetActive(false);

            _inputView = gameObject.AddComponent<InputView>();
            _inputView.Init(_commandQueue, _player, _entityManager, _fsm, bulletViewWriter);

            _fsm.AddState(eGameState.Ready, new ReadyState(_uiView));
            _fsm.AddState(eGameState.Playing, new PlayingState(
                _entityManager, _collisionSystem, _commandQueue, _stageTimeline,
                _player, _scrollView, _uiView, _fsm, createEnemy));
            _fsm.AddState(eGameState.Paused, new PausedState(_uiView));
            _fsm.AddState(eGameState.GameOver, new GameOverState(_uiView));
            _fsm.AddState(eGameState.StageClear, new StageClearState(_uiView));
            _fsm.ChangeState(eGameState.Ready);
        }

        private void Update()
        {
            int deltaFrame = _frameManager.CalcDeltaFrame(Time.deltaTime);
            for (int i = 0; i < deltaFrame; i++)
            {
                _fsm.LogicUpdate();
            }
            if (deltaFrame > 0)
            {
                _gameView.Render();
            }
        }

        private EnemyLogic createEnemy(eEnemyType type, float x, float y)
        {
            var view = _gameView.GetEnemyView();
            int hp, score, contactDamage;
            float speed, hitRadius;

            switch (type)
            {
                case eEnemyType.Straight:
                    hp = 20; speed = 3f / 60f; contactDamage = 20; score = 100; hitRadius = 0.4f; break;
                case eEnemyType.Wave:
                    hp = 30; speed = 2.5f / 60f; contactDamage = 20; score = 200; hitRadius = 0.4f; break;
                case eEnemyType.Boss:
                    hp = 500; speed = 1f / 60f; contactDamage = 30; score = 5000; hitRadius = 1.0f; break;
                default:
                    hp = 20; speed = 3f / 60f; contactDamage = 20; score = 100; hitRadius = 0.4f; break;
            }

            var enemy = new EnemyLogic(0, type, x, y, hp, speed, contactDamage, score, hitRadius, view);
            view.Activate(enemy);
            _gameView.RegisterView(enemy.Id, view);
            return enemy;
        }

        private System.Collections.Generic.List<SpawnEvent> createDefaultSpawnEvents()
        {
            var events = new System.Collections.Generic.List<SpawnEvent>();
            for (int i = 1; i <= 8; i++)
            {
                events.Add(new SpawnEvent { Frame = i * 60, EnemyType = eEnemyType.Straight, X = 10f, Y = 3f - i % 3 * 2f });
            }
            for (int i = 1; i <= 5; i++)
            {
                events.Add(new SpawnEvent { Frame = 480 + i * 90, EnemyType = eEnemyType.Wave, X = 10f, Y = i % 2 == 0 ? 2f : -2f });
            }
            events.Add(new SpawnEvent { Frame = 3000, EnemyType = eEnemyType.Boss, X = 8f, Y = 0f });
            return events;
        }
    }
}
```

- [ ] **Step 2: 커밋**
```bash
git add Assets/Scripts/Entry/
git commit -m "add: GameEntry — 진입점, 전체 조립"
```

---

## Task 20: 씬 셋업 + Prefab 생성 + 플레이 테스트

**Files:**
- Modify: `Assets/Scenes/GameScene.unity` (새 씬 또는 SampleScene 활용)
- Create: `Assets/Prefabs/PlayerView.prefab`
- Create: `Assets/Prefabs/EnemyView.prefab`
- Create: `Assets/Prefabs/BulletView.prefab`
- Create: `Assets/Prefabs/ItemView.prefab`

- [ ] **Step 1: Prefab 생성 (Unity 에디터에서)**

각 Prefab은 프리미티브 도형으로 구성:

- **PlayerView.prefab**: 빈 GameObject + PlayerView.cs + 자식으로 파란 삼각형 (Sprite 또는 3D quad)
- **EnemyView.prefab**: 빈 GameObject + EnemyView.cs + 자식으로 빨간 사각형
- **BulletView.prefab**: 빈 GameObject + BulletView.cs + 자식으로 작은 원
- **ItemView.prefab**: 빈 GameObject + ItemView.cs + 자식으로 초록 원

- [ ] **Step 2: 씬 구성 (Unity 에디터에서)**

GameScene에:
1. 빈 GameObject "GameEntry" — GameEntry.cs 컴포넌트 추가
2. 빈 GameObject "GameViewRoot" — GameEntry의 _gameViewRoot에 할당
3. ScrollView 오브젝트 — 배경 스프라이트를 자식으로
4. Canvas + UiView — HP 바(Image), Score(Text), WeaponLevel(Text), 패널 4개 (Ready/GameOver/StageClear/Pause)
5. SerializeField에 Prefab과 컴포넌트 할당

- [ ] **Step 3: 플레이 테스트**

Run: Unity Play Mode
확인 사항:
- Space로 게임 시작
- WASD/방향키로 이동
- Space 사격 + 연사
- 적 등장 + 파괴 + 스코어 증가
- 아이템 드랍/획득
- 보스 등장 (스크롤 정지) + 처치 → StageClear
- 피격 → HP 감소 → GameOver
- Esc → Pause/Resume

- [ ] **Step 4: 커밋**
```bash
git add Assets/Prefabs/ Assets/Scenes/
git commit -m "add: 씬 셋업 + Prefab + 플레이 가능 프로토타입 완성"
```

---

## 태스크 요약

| Task | 내용 | 유형 |
|------|------|------|
| 1 | UniTask 설치 | 셋업 |
| 2 | FrameManager + 테스트 | Core |
| 3 | CommandQueue + 테스트 | Core |
| 4 | GameLoop | Core |
| 5 | FSM (StateMachine) | Logic |
| 6 | HitBox + CollisionSystem + 테스트 | Logic |
| 7 | IEntity + EntityManager | Logic |
| 8 | PlayerLogic + 테스트 | Logic |
| 9 | BulletLogic | Logic |
| 10 | EnemyLogic | Logic |
| 11 | ItemLogic | Logic |
| 12 | Commands (전체) | Logic |
| 13 | StageTimeline + 테스트 | Logic |
| 14 | EntityViewPool + IEntityView | View |
| 15 | PlayerView/EnemyView/BulletView/ItemView | View |
| 16 | InputView/ScrollView/UiView | View |
| 17 | GameView | View |
| 18 | FSM 상태 클래스 + Entry asmdef | Logic+Entry |
| 19 | GameEntry (전체 조립) | Entry |
| 20 | 씬 셋업 + Prefab + 플레이 테스트 | Integration |
