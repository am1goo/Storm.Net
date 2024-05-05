using System;
using System.Collections.Generic;
using System.Reflection;

namespace Storm.Serializers
{
    internal class LinkedListStormSerializer : CollectionStormSerializer
    {
        public static readonly LinkedListStormSerializer instance = new LinkedListStormSerializer();

        private readonly Type _collectionType = typeof(LinkedList<>);
        protected override Type collectionType => _collectionType;

        private readonly Type _returnType = typeof(LinkedListNode<>);
        private const string _insertMethodName = "AddLast";

        protected override MethodInfo GetInsertMethod(Type type)
        {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var method in methods)
            {
                if (method.Name != _insertMethodName)
                    continue;

                var returnTypeDef = method.ReturnType.GetGenericTypeDefinition();
                if (!returnTypeDef.IsAssignableFrom(_returnType))
                    continue;

                var args = method.GetParameters();
                if (args.Length != 1)
                    continue;

                return method;
            }
            throw new Exception($"method {_insertMethodName} not found in {type}");
        }
    }
}
