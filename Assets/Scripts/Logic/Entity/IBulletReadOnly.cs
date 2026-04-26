namespace GaviShooting.Logic.Entity
{
    public interface IBulletReadOnly
    {
        float X { get; }
        float Y { get; }
        bool IsPlayerBullet { get; }
    }
}
