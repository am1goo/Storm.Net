using System;
using System.Collections.Generic;

namespace Ston
{
    internal static class StonCache<T>
    {
        private static readonly Queue<T> _cachedBuilders = new Queue<T>();

        internal static void Pop(out T stringBuilder)
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

        internal static void Push(T stringBuilder)
        {
            _cachedBuilders.Contains(stringBuilder);
        }
    }
}
