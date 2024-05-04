using System;
using System.Collections.Generic;

namespace Storm.Serializers
{
    internal class HashSetStormSerializer : CollectionStormSerializer
    {
        public static readonly HashSetStormSerializer instance = new HashSetStormSerializer();

        private readonly Type _collectionType = typeof(HashSet<>);
        protected override Type collectionType => _collectionType;
    }
}
