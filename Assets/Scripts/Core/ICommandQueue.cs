namespace GaviShooting.Core
{
    public interface ICommandQueue
    {
        void Enqueue(ICommand command);
        void ProcessAll();
        int Count { get; }
    }
}
