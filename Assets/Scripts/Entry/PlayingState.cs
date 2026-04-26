using System.Collections.Generic;
using GaviShooting.Core;
using GaviShooting.Logic.Collision;
using GaviShooting.Logic.Entity;
using GaviShooting.Logic.Fsm;
using GaviShooting.Logic.Stage;
using GaviShooting.View;

namespace GaviShooting.Entry
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
            Log.Info("PlayingState.Enter");
            _scrollView.SetScrolling(true);
            _uiView.HideAll();
        }

        public void Exit()
        {
            Log.Info("PlayingState.Exit");
            _scrollView.SetScrolling(false);
        }

        /// <summary>
        /// 라이프사이클: ProcessQueue → Move → ProcessCommand → Update → Remove
        /// </summary>
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
            {
                Log.Warn("PlayingState: Player dead → GameOver");
                _fsm.ChangeState(eGameState.GameOver);
            }
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
                        Log.Info("PlayingState: Enemy killed score+={0} total={1}", enemy.Score, _score);
                        if (enemy.EnemyType == eEnemyType.Boss)
                        {
                            Log.Info("PlayingState: Boss killed → StageClear");
                            _fsm.ChangeState(eGameState.StageClear);
                        }
                    }
                    break;
            }
        }
    }
}
