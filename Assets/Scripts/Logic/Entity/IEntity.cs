using GaviShooting.Logic.Collision;

namespace GaviShooting.Logic.Entity
{
    public interface IEntity : ICollidable
    {
        int Id { get; }
        float X { get; }
        float Y { get; }
        void Move();
        void Update();
        bool ShouldRemove { get; }
    }
}
