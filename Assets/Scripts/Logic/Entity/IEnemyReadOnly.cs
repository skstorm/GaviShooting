namespace GaviShooting.Logic.Entity
{
    public interface IEnemyReadOnly
    {
        float X { get; }
        float Y { get; }
        int Hp { get; }
        eEnemyType EnemyType { get; }
        bool IsAlive { get; }
    }
}
