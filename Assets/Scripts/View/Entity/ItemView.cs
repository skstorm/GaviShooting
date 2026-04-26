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
