using System;
using System.Collections.Generic;

namespace Ston
{
    internal static class StonCache<T>
    {
        private static readonly Queue<T> _cachedBuilders = new Queue<T>();

        internal static void Pop(out T stringBuilder)
        {
            lock (_cachedBuilders)
            {
                if (_cachedBuilders.Count > 0)
                {
                    stringBuilder = _cachedBuilders.Dequeue();
                }
                else
                {
                    stringBuilder = Activator.CreateInstance<T>();
                }
            }
        }

        internal static void Push(T stringBuilder)
        {
            lock (_cachedBuilders)
            {
                if (_cachedBuilders.Contains(stringBuilder))
                    return;

                _cachedBuilders.Enqueue(stringBuilder);
            }
        }
    }
}
