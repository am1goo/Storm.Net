using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Storm.Serializers
{
    internal class ConcurrentQueueStormSerializer : CollectionStormSerializer
    {
        public static readonly ConcurrentQueueStormSerializer instance = new ConcurrentQueueStormSerializer();

        private readonly Type _collectionType = typeof(ConcurrentQueue<>);
        protected override Type collectionType => _collectionType;

        protected override MethodInfo GetInsertMethod(Type type)
        {
            return type.GetMethod("Enqueue", BindingFlags.Public | BindingFlags.Instance);
        }
    }
}
