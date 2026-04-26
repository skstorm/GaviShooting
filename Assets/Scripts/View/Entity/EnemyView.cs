using GaviShooting.Core;
using GaviShooting.Logic.Entity;
using UnityEngine;

namespace GaviShooting.View.Entity
{
    public class EnemyView : MonoBehaviour, IEntityView, IEnemyViewWriter
    {
        public void Activate(IEntity entity)
        {
            SyncPosition(entity);
            Log.Info("EnemyView.Activate");
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
