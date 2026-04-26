using System.Collections.Generic;

namespace GaviShooting.Core
{
    public class CommandQueue : ICommandQueue
    {
        private readonly Queue<ICommand> _queue = new();

        public int Count => _queue.Count;

        public void Enqueue(ICommand command)
        {
            _queue.Enqueue(command);
            Log.Info("CommandQueue.Enqueue {0} (count={1})", command.GetType().Name, _queue.Count);
        }

        public void ProcessAll()
        {
            int count = _queue.Count;
            while (_queue.Count > 0)
            {
                _queue.Dequeue().Execute();
            }
            Log.Info("CommandQueue.ProcessAll processed={0}", count);
        }
    }
}
