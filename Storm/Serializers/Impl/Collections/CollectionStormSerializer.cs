using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Storm.Serializers
{
    internal abstract class CollectionStormSerializer : IStormSerializer
    {
        private const char CharStart = StormConstants.BracketStart;
        private const char CharEnd = StormConstants.BracketEnd;

        protected abstract Type collectionType { get; }

        protected virtual MethodInfo GetInsertMethod(Type type)
        {
            return type.GetMethod("Add", BindingFlags.Public | BindingFlags.Instance);
        }

        public bool CanConvert(Type type)
        {
            if (!type.IsGenericType)
                return false;

            var typeDef = type.GetGenericTypeDefinition();
            return collectionType.IsAssignableFrom(typeDef);
        }

        public bool TryParse(ref int index, string[] lines, out string key, out string text)
        {
            return StormSerializer.TryParseText(ref index, lines, CharStart, CharEnd, out key, out text);
        }

        public void Populate(IStormVariableW variable, IStormValue value, StormContext ctx)
        {
            var type = variable.type;

            var elementType = type.GetGenericArguments()[0];

            var insertMethod = GetInsertMethod(type);

            var collection = Activator.CreateInstance(type);

            StormCache<List<IStormValue>>.Pop(out var cache);
            value.GetEntries(cache);
            var elementCount = cache.Count;
            for (int i = 0; i < elementCount; ++i)
            {
                var entry = cache[i];
                var elementVar = new ElementVariable(elementType, collection, insertMethod);
                entry.Populate(elementVar, ctx);
            }
            StormCache<List<IStormValue>>.Push(cache);

            variable.SetValue(collection);
        }

        public async Task<IStormValue> DeserializeAsync(string text, StormContext ctx)
        {
            var stormList = new StormArray();
            stormList = await ctx.serializer.DeserializeAsync(stormList, text, ctx);
            return stormList;
        }

        public async Task<string> SerializeAsync(IStormVariable variable, object obj, StormContext ctx)
        {
            var collection = (IEnumerable)obj;
            if (collection == null)
                return null;

            var type = variable.type;
            var elementType = type.GetGenericArguments()[0];

            StormCache<StringBuilder>.Pop(out var sb);
            var intent = ctx.settings.GetIntent(ctx.intent);
            sb.Append(intent).AppendKey(variable.name);
            sb.Append(CharStart).Append(Environment.NewLine);
            ctx.intent++;

            foreach (var elementValue in collection)
            {
                var elementVar = new ElementVariable(elementType, obj, null);
                var elementStr = await ctx.serializer.SerializeAsync(elementVar, elementValue, ctx);
                sb.Append(elementStr).Append(Environment.NewLine);
            }
            ctx.intent--;
            sb.Append(intent).Append(CharEnd);
            var str = sb.ToString();
            sb.Clear();
            StormCache<StringBuilder>.Push(sb);

            return str;
        }

        private struct ElementVariable : IStormVariableW
        {
            private string _name;
            public string name => _name;

            private Type _type;
            public Type type => _type;

            private object _collection;
            private MethodInfo _insertMethod;

            public ElementVariable(Type type, object collection, MethodInfo insertMethod)
            {
                _name = string.Empty;
                _type = type;
                _collection = collection;
                _insertMethod = insertMethod;
            }

            public void GetAttributes(List<Attribute> result)
            {
                //do nothing
            }

            public void SetValue(object value)
            {
                _insertMethod.Invoke(_collection, new object[] { value });
            }
        }
    }
}
