using System;
using System.Collections.Generic;
using System.Reflection;

namespace Storm.Serializers
{
    internal class SortedSetStormSerializer : CollectionStormSerializer
    {
        public static readonly SortedSetStormSerializer instance = new SortedSetStormSerializer();

        private readonly Type _collectionType = typeof(SortedSet<>);
        protected override Type collectionType => _collectionType;

        protected override MethodInfo GetInsertMethod(Type type)
        {
            return type.GetMethod("Add", BindingFlags.Public | BindingFlags.Instance);
        }
    }
}
