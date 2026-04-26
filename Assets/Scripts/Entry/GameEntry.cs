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

        private StageData _stageData;
        private IBulletViewWriter _bulletViewWriter;

        private void Start()
        {
            Log.Info("GameEntry.Start");

            _frameManager = new FrameManager(FPS);
            _commandQueue = new CommandQueue();
            _entityManager = new EntityManager();
            _collisionSystem = new CollisionSystem();
            _fsm = new StateMachine();

            _stageData = new StageData
            {
                StageName = "Stage 1",
                TotalFrames = 3600,
                ScrollSpeed = 2f,
                BossFrame = 3000,
                SpawnEvents = createDefaultSpawnEvents()
            };
            _stageTimeline = new StageTimeline(_stageData);

            _gameView = _gameViewRoot.gameObject.AddComponent<GameView>();
            _gameView.Init(_entityManager, _playerPrefab, _enemyPrefab, _bulletPrefab, _itemPrefab, _scrollView, _uiView);

            _scrollView.Init(_stageData.ScrollSpeed);

            spawnPlayer();

            var bulletView = _gameView.GetBulletView();
            bulletView.gameObject.SetActive(false);
            _bulletViewWriter = bulletView;

            _inputView = gameObject.AddComponent<InputView>();
            _inputView.Init(_commandQueue, _player, _entityManager, _fsm, _bulletViewWriter, RestartGame);

            _fsm.AddState(eGameState.Ready, new ReadyState(_uiView));
            _fsm.AddState(eGameState.Playing, new PlayingState(
                _entityManager, _collisionSystem, _commandQueue, _stageTimeline,
                _player, _scrollView, _uiView, _fsm, createEnemy));
            _fsm.AddState(eGameState.Paused, new PausedState(_uiView));
            _fsm.AddState(eGameState.GameOver, new GameOverState(_uiView));
            _fsm.AddState(eGameState.StageClear, new StageClearState(_uiView));
            _fsm.ChangeState(eGameState.Ready);

            Log.Info("GameEntry.Start complete");
        }

        private void spawnPlayer()
        {
            var playerView = _gameView.GetPlayerView();
            _player = new PlayerLogic(0, -6f, 0f, playerView);
            _entityManager.EnqueueSpawn(_player);
            _entityManager.ProcessQueue();
            playerView.Activate(_player);
            _gameView.RegisterView(_player.Id, playerView);
        }

        public void RestartGame()
        {
            Log.Info("GameEntry.RestartGame");
            _gameView.Clear();
            _entityManager.Clear();
            _stageTimeline.Reset();

            spawnPlayer();

            _inputView.Init(_commandQueue, _player, _entityManager, _fsm, _bulletViewWriter, RestartGame);

            _fsm.AddState(eGameState.Playing, new PlayingState(
                _entityManager, _collisionSystem, _commandQueue, _stageTimeline,
                _player, _scrollView, _uiView, _fsm, createEnemy));

            _fsm.ChangeState(eGameState.Playing);
        }

        private void Update()
        {
            _commandQueue.ProcessAll();

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
