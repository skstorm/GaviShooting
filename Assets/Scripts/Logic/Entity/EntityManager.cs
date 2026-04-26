using System.Collections.Generic;
using GaviShooting.Core;
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

        public int NextId() => _nextId++;

        public void EnqueueSpawn(IEntity entity)
        {
            _spawnQueue.Enqueue(entity);
            Log.Info("EntityManager.EnqueueSpawn id={0} type={1}", entity.Id, entity.GetType().Name);
        }

        /// <summary>
        /// 라이프사이클 첫 단계: 큐에 대기 중인 엔티티를 활성 목록에 추가한다
        /// </summary>
        public void ProcessQueue()
        {
            int count = _spawnQueue.Count;
            while (_spawnQueue.Count > 0)
            {
                _entities.Add(_spawnQueue.Dequeue());
            }
            if (count > 0)
                Log.Info("EntityManager.ProcessQueue spawned={0} total={1}", count, _entities.Count);
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
                    Log.Info("EntityManager.RemoveDead id={0}", _entities[i].Id);
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
            Log.Info("EntityManager.Clear");
        }
    }
}
