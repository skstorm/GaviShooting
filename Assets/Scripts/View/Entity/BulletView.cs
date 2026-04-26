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
