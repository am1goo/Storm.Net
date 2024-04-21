using System;
using System.Collections;
using System.Collections.Generic;

namespace Storm
{
    internal static class StormCache<T>
    {
        private static readonly Queue<T> _cache = new Queue<T>();

        internal static void Pop(out T obj)
        {
            lock (_cache)
            {
                if (_cache.Count > 0)
                {
                    obj = _cache.Dequeue();
                }
                else
                {
                    obj = Activator.CreateInstance<T>();
                }
            }
        }

        internal static void Push(T obj)
        {
            lock (_cache)
            {
                if (_cache.Contains(obj))
                    return;

                if (obj is IList list && list.Count > 0)
                    list.Clear();
                else if (obj is Queue queue && queue.Count > 0)
                    queue.Clear();

                _cache.Enqueue(obj);
            }
        }
    }
}
