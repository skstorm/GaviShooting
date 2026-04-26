using GaviShooting.Logic.Entity;

namespace GaviShooting.View.Entity
{
    public interface IEntityView
    {
        void Activate(IEntity entity);
        void SyncPosition(IEntity entity);
        void Deactivate();
    }
}
