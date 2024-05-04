using System;
using System.Collections.Generic;

namespace Storm.Serializers
{
    internal class ListStormSerializer : CollectionStormSerializer
    {
        public static readonly ListStormSerializer instance = new ListStormSerializer();

        private readonly Type _collectionType = typeof(List<>);
        protected override Type collectionType => _collectionType;
    }
}
