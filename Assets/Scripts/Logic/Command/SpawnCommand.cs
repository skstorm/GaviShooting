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
