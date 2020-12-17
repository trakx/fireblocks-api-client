using System.Collections.Concurrent;

namespace Trakx.Fireblocks.ApiClient.Utils
{
    public class LimitedConcurrentQueue<T> : ConcurrentQueue<T>
    {
        public readonly int Limit;

        public LimitedConcurrentQueue(int limit)
        {
            Limit = limit;
        }

        public new void Enqueue(T element)
        {
            base.Enqueue(element);
            if (Count <= Limit) return;
            TryDequeue(out T discard);
        }
    }
}
