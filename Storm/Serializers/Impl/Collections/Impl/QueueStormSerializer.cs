using System;
using System.Collections.Generic;
using System.Reflection;

namespace Storm.Serializers
{
    internal class QueueStormSerializer : CollectionStormSerializer
    {
        public static readonly QueueStormSerializer instance = new QueueStormSerializer();

        private readonly Type _collectionType = typeof(Queue<>);
        protected override Type collectionType => _collectionType;

        protected override MethodInfo GetInsertMethod(Type type)
        {
            return type.GetMethod("Enqueue", BindingFlags.Public | BindingFlags.Instance);
        }
    }
}
