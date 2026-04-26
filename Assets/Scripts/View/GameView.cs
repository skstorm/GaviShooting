using System.Collections.Generic;
using UnityEngine;
using GaviShooting.Core;
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
            Log.Info("GameView.Init");
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
            Log.Info("GameView.Clear");
        }
    }
}
