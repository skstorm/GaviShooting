namespace GaviShooting.Logic.Entity
{
    public interface IPlayerReadOnly
    {
        float X { get; }
        float Y { get; }
        int Hp { get; }
        int WeaponLevel { get; }
        bool IsAlive { get; }
    }
}
