namespace GaviShooting.Logic.Collision
{
    public interface ICollidable
    {
        HitBox HitBox { get; }
        eCollisionLayer Layer { get; }
        bool IsAlive { get; }
    }
}
