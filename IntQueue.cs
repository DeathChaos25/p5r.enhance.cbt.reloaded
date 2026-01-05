using System;
using System.Collections.Concurrent;
using static p5r.enhance.cbt.reloaded.Utils;

namespace p5r.enhance.cbt.reloaded
{
    public class IntQueue
    {
        private readonly ConcurrentQueue<int> _queue = new ConcurrentQueue<int>();
        private readonly string _name;

        public string Name => _name;

        public IntQueue(string name = "Unnamed")
        {
            _name = name;
        }

        public void Enqueue(int value)
        {
            _queue.Enqueue(value);
            LogDebug($"Enqueued value {value} to '{_name}'. Queue size: {Count}");
        }

        public bool TryDequeue(out int value)
        {
            bool result = _queue.TryDequeue(out value);
            if (result)
            {
                LogDebug($"Dequeued value {value} from '{_name}'. Queue remaining: {Count}");
            }
            return result;
        }

        public bool TryPeek(out int value)
        {
            return _queue.TryPeek(out value);
        }

        public bool IsEmpty => _queue.IsEmpty;
        public bool HasItems => !_queue.IsEmpty;

        public int Count => _queue.Count;

        public void Clear()
        {
            while (_queue.TryDequeue(out _)) { }
            LogDebug($"Cleared all items from '{_name}'");
        }

        public List<int> ToList()
        {
            return _queue.ToList();
        }

        public List<int> TakeAll()
        {
            var items = new List<int>();
            while (_queue.TryDequeue(out int item))
            {
                items.Add(item);
            }
            LogDebug($"Took all {items.Count} items from '{_name}'");
            return items;
        }
    }
}
