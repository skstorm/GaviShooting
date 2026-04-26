using GaviShooting.Core;
using GaviShooting.Logic.Entity;
using UnityEngine;

namespace GaviShooting.View.Entity
{
    public class PlayerView : MonoBehaviour, IEntityView, IPlayerViewWriter
    {
        public void Activate(IEntity entity)
        {
            SyncPosition(entity);
            Log.Info("PlayerView.Activate");
        }

        public void SyncPosition(IEntity entity)
        {
            transform.position = new Vector3(entity.X, entity.Y, 0f);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public void PlayDamageEffect() { }
        public void PlayDeathEffect() { }
    }
}
