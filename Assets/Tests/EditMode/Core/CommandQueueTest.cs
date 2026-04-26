using NUnit.Framework;
using GaviShooting.Core;

namespace Tests.EditMode.Core
{
    public class CommandQueueTest
    {
        private class StubCommand : ICommand
        {
            public int ExecuteCount;
            public void Execute() => ExecuteCount++;
        }

        [Test]
        public void Enqueue_IncreasesCount()
        {
            var queue = new CommandQueue();
            queue.Enqueue(new StubCommand());
            Assert.AreEqual(1, queue.Count);
        }

        [Test]
        public void ProcessAll_ExecutesAllCommands()
        {
            var queue = new CommandQueue();
            var cmd1 = new StubCommand();
            var cmd2 = new StubCommand();
            queue.Enqueue(cmd1);
            queue.Enqueue(cmd2);

            queue.ProcessAll();

            Assert.AreEqual(1, cmd1.ExecuteCount);
            Assert.AreEqual(1, cmd2.ExecuteCount);
        }

        [Test]
        public void ProcessAll_ClearsQueue()
        {
            var queue = new CommandQueue();
            queue.Enqueue(new StubCommand());
            queue.ProcessAll();
            Assert.AreEqual(0, queue.Count);
        }

        [Test]
        public void ProcessAll_EmptyQueue_DoesNothing()
        {
            var queue = new CommandQueue();
            Assert.DoesNotThrow(() => queue.ProcessAll());
        }
    }
}
