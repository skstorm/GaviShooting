# 횡스크롤 슈팅 게임 기획서

## 개요

- **프로젝트명**: GaviShooting
- **장르**: 클래식 횡스크롤 슈팅 (그라디우스/R-Type 계열)
- **플랫폼**: PC 우선 (향후 모바일 확장 가능)
- **규모**: 미니멀 프로토타입 — 스테이지 1개, 적 2~3종, 보스 1마리
- **아트**: 프리미티브 도형 (사각형, 원, 삼각형)
- **엔진**: Unity 6, URP 2D
- **접근법**: 게임 주도 점진적 구축 — 설계 원칙은 처음부터 준수하되 필요할 때 만든다

## 설계 원칙

game-programmer 에이전트의 설계 사상을 따른다:

- Logic-View 분리 (Logic은 순수 C#, View는 MonoBehaviour)
- 고정 프레임 루프 (deltaFrame 기반)
- 커맨드 패턴 (모든 입력/통신을 큐로 통일)
- 수동 생성자 주입 (DI 프레임워크/Service Locator 금지)
- UniTask 비동기 처리
- 리플레이 가능 구조만 준비 (기능 미구현)

## 시스템 아키텍처

### 계층 구조

```
GameEntry (MonoBehaviour, 유일한 진입점)
  ↓
FrameManager / GameLoop (코어 프레임워크)
  ↓
CommandQueue / StageTimeline / EntityManager / CollisionSystem (게임 시스템, 순수 C#)
  ↓
PlayerLogic / EnemyLogic / BulletLogic / ItemLogic (엔티티 Logic, 순수 C#)
  ── Logic / View 경계 ──
GameView / EntityViewPool / InputView / UiView / ScrollView (View, MonoBehaviour)
  ↓ 엔티티별 View
PlayerView / EnemyView / BulletView / ItemView
```

### 네임스페이스 의존 방향

```
Entry → View → Logic → Core
```

의존은 항상 한 방향만 허용. Logic이 View 네임스페이스를 참조하지 않음.

### Logic ↔ View 인터페이스

양방향이되 역할 분리:

- **View → Logic**: 읽기 전용 (IPlayerReadOnly, IEnemyReadOnly, IBulletReadOnly, IItemReadOnly)
- **Logic → View**: 쓰기 전용 (IPlayerViewWriter, IEnemyViewWriter, IBulletViewWriter, IItemViewWriter)

두 인터페이스 모두 Logic 네임스페이스에 정의. View가 이를 구현하므로 의존 방향 유지.

## 엔티티 생명주기

한 논리 프레임 내 처리 순서:

```
ProcessQueue(생성) → Move(이동) → ProcessCommand(커맨드) → Update(갱신) → Remove(제거)
```

- **ProcessQueue**: 스폰 큐에 있는 엔티티를 EntityManager에 등록
- **Move**: 모든 엔티티의 위치 갱신
- **ProcessCommand**: 커맨드 큐 일괄 처리 (Move, Shoot, Damage, Spawn, ItemPickup)
- **Update**: 상태 갱신, 충돌 판정, 타임라인 진행
- **Remove**: HP ≤ 0인 적, 화면 밖 탄환, 획득된 아이템 제거

## 게임 상태 (FSM)

```
Ready → (Space) → Playing → (HP ≤ 0) → GameOver → (Space) → Ready
                           → (보스 처치) → StageClear → (Space) → Ready
                           → (Esc) → Paused → (Esc) → Playing
```

상태 전환은 FSM으로 구현. 전환 트리거도 커맨드로 처리 (GameStartCommand, GameOverCommand, StageClearCommand, PauseCommand).

## 스테이지 타임라인

ScriptableObject로 정의:

```
StageData
├── stageName: "Stage 1"
├── totalFrames: 3600 (60fps × 60초)
├── scrollSpeed: 2.0
├── bossFrame: 3000 (50초 시점)
└── spawnEvents[]
    ├── { frame: 60,   enemyType: Straight, pos: (10, 3) }
    ├── { frame: 120,  enemyType: Wave,     pos: (10, 4) }
    └── { frame: 3000, enemyType: Boss,     pos: (8, 0) }
```

bossFrame 도달 시 자동 스크롤 정지, 보스 처치 시 StageClear.

## 충돌 시스템

- 원-원(Circle-Circle) 충돌. Unity 물리 엔진 미사용. 순수 C#으로 계산.
- 모든 히트박스: 중심점 + 반지름

### 충돌 조합

|            | 적           | 적 탄        | 아이템       |
|------------|-------------|-------------|-------------|
| 플레이어    | 데미지       | 데미지       | 획득         |
| 플레이어 탄 | 데미지       | 판정 없음    | 판정 없음    |

## 엔티티 상세

### Player

- HP: 100
- 이동: 전방향 자유 이동, 속도 5.0 유닛/초, 화면 경계 내 제한
- 무기: Lv.1(단발) → Lv.2(2연발) → Lv.3(3연발, 부채꼴)
- 발사 간격: 6프레임 (0.1초)
- 탄 속도: 10.0, 탄 데미지: 10
- 히트박스 반지름: 0.3 (기체보다 작게)

### Enemy — 직진형

- HP: 20, 이동 속도: 3.0 (왼쪽 직진)
- 발사: 없음
- 충돌 데미지: 20, 점수: 100

### Enemy — 웨이브형

- HP: 30, 이동 속도: 2.5 (사인파 이동)
- 발사: 60프레임 간격
- 충돌 데미지: 20, 점수: 200

### Enemy — 보스

- HP: 500, 이동 속도: 1.0 (상하 이동)
- 발사: 30프레임 간격, 3방향
- 충돌 데미지: 30, 점수: 5000

### Item

- 무기 강화: 무기 레벨 +1 (최대 Lv.3), 드랍 확률 15%
- HP 회복: HP +30 회복, 드랍 확률 10%

## UI

- HP 바: 좌상단, 잔량에 따라 색상 변화 (초록→노랑→빨강)
- 스코어: 우상단
- 무기 레벨: 좌상단 HP 아래

## 조작 (키보드)

- WASD / 방향키: 이동
- Space: 사격 (누르고 있으면 연사)
- Esc: 일시정지

## 스테이지 수치

- 논리 프레임레이트: 60fps
- 스테이지 길이: 3600프레임 (60초)
- 스크롤 속도: 2.0 유닛/초
- 보스 등장: 3000프레임 (50초 시점)
- 적 탄 속도: 6.0, 적 탄 데미지: 15
- 모든 수치는 ScriptableObject로 에디터에서 조정 가능

## 디렉토리 구조

```
Assets/
├── Scripts/
│   ├── Core/                    — GaviShooting.Core
│   │   ├── FrameManager.cs
│   │   ├── GameLoop.cs
│   │   ├── IGameLoop.cs
│   │   ├── CommandQueue.cs
│   │   └── ICommandQueue.cs
│   │
│   ├── Logic/                   — GaviShooting.Logic
│   │   ├── Entity/
│   │   │   ├── EntityManager.cs
│   │   │   ├── IEntity.cs
│   │   │   ├── PlayerLogic.cs
│   │   │   ├── IPlayerReadOnly.cs
│   │   │   ├── IPlayerViewWriter.cs
│   │   │   ├── EnemyLogic.cs
│   │   │   ├── IEnemyReadOnly.cs
│   │   │   ├── IEnemyViewWriter.cs
│   │   │   ├── BulletLogic.cs
│   │   │   ├── IBulletReadOnly.cs
│   │   │   ├── IBulletViewWriter.cs
│   │   │   ├── ItemLogic.cs
│   │   │   ├── IItemReadOnly.cs
│   │   │   └── IItemViewWriter.cs
│   │   ├── Command/
│   │   │   ├── ICommand.cs
│   │   │   ├── MoveCommand.cs
│   │   │   ├── ShootCommand.cs
│   │   │   ├── DamageCommand.cs
│   │   │   ├── SpawnCommand.cs
│   │   │   └── ItemPickupCommand.cs
│   │   ├── Collision/
│   │   │   ├── CollisionSystem.cs
│   │   │   └── HitBox.cs
│   │   └── Stage/
│   │       ├── StageTimeline.cs
│   │       └── StageData.cs
│   │
│   ├── View/                    — GaviShooting.View
│   │   ├── GameView.cs
│   │   ├── Entity/
│   │   │   ├── IEntityView.cs
│   │   │   ├── EntityViewPool.cs
│   │   │   ├── PlayerView.cs
│   │   │   ├── EnemyView.cs
│   │   │   ├── BulletView.cs
│   │   │   └── ItemView.cs
│   │   ├── InputView.cs
│   │   ├── ScrollView.cs
│   │   └── UiView.cs
│   │
│   └── Entry/                   — GaviShooting.Entry
│       └── GameEntry.cs
│
├── Resources/
│   └── StageData/
│       └── Stage1.asset
│
├── Prefabs/
│   ├── PlayerView.prefab
│   ├── EnemyView.prefab
│   ├── BulletView.prefab
│   └── ItemView.prefab
│
├── Scenes/
│   └── GameScene.unity
│
└── Tests/
    └── EditMode/
        ├── Core/
        │   ├── FrameManagerTest.cs
        │   └── CommandQueueTest.cs
        └── Logic/
            ├── PlayerLogicTest.cs
            ├── CollisionSystemTest.cs
            └── StageTimelineTest.cs
```
