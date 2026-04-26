namespace GaviShooting.Logic.Entity
{
    public interface IItemReadOnly
    {
        float X { get; }
        float Y { get; }
        eItemType ItemType { get; }
    }
}
