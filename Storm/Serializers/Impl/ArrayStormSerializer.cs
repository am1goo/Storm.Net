using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Storm.Serializers
{
    public class ArrayStormSerializer : IStormSerializer
    {
        private const char BracketStart = '[';
        private const char BracketEnd   = ']';

        public static readonly ArrayStormSerializer instance = new ArrayStormSerializer();

        public bool CanConvert(Type type)
        {
            return type.IsSZArray;
        }

        public bool TryParse(ref int index, string[] lines, out string key, out string text)
        {
            return StormSerializer.TryParseText(ref index, lines, BracketStart, BracketEnd, out key, out text);
        }

        public void Populate(IStormVariable variable, IStormValue value, StormContext ctx)
        {
            var type = variable.type;
            if (!type.IsArray)
                throw new Exception($"target type {variable.type} should be an array");

            const int expectedRank = 1;
            var actualRank = type.GetArrayRank();
            if (actualRank != expectedRank)
                throw new Exception($"unsupported array rank, actual={actualRank}, expected={expectedRank}");

            if (!type.HasElementType)
                throw new Exception($"array should to have element type");

            StormCache<List<IStormValue>>.Pop(out var cache);
            value.GetEntries(cache);
            var elementType = type.GetElementType();
            var elementCount = cache.Count;
            var array = Array.CreateInstance(elementType, elementCount);

            for (int i = 0; i < elementCount; ++i)
            {
                var entry = cache[i];
                var elementVar = new ElementVariable(elementType, array, i);
                entry.Populate(elementVar, ctx);
            }
            StormCache<List<IStormValue>>.Push(cache);

            variable.SetValue(array);
        }

        public async Task<IStormValue> DeserializeAsync(string text, StormContext ctx)
        {
            var stormArray = new StormArray();
            stormArray = await ctx.serializer.DeserializeAsync(stormArray, text, ctx);
            return stormArray;
        }

        public async Task<string> SerializeAsync(IStormVariable variable, object obj, StormContext ctx)
        {
            var type = variable.type;

            const int expectedRank = 1;
            var actualRank = type.GetArrayRank();
            if (actualRank != expectedRank)
                return null;

            if (!type.HasElementType)
                return null;

            var array = obj as Array;
            if (array == null)
                return null;

            var elementType = type.GetElementType();

            StormCache<StringBuilder>.Pop(out var sb);
            var intent = ctx.settings.GetIntent(ctx.intent);
            sb.Append(intent).AppendKey(variable.name);
            sb.Append(BracketStart).Append(Environment.NewLine);
            ctx.intent++;
            for (int i = 0; i < array.Length; ++i)
            {
                var elementValue = array.GetValue(i);
                var elementVar = new ElementVariable(elementType, array, i);
                var elementStr = await ctx.serializer.SerializeAsync(elementVar, elementValue, ctx);
                sb.Append(elementStr).Append(Environment.NewLine);
            }
            ctx.intent--;
            sb.Append(intent).Append(BracketEnd);
            var str = sb.ToString();
            sb.Clear();
            StormCache<StringBuilder>.Push(sb);

            return str;
        }

        private struct ElementVariable : IStormVariable
        {
            private string _name;
            public string name => _name;

            private Type _type;
            public Type type => _type;

            private Array _array;
            private int _index;

            public ElementVariable(Type type, Array array, int index)
            {
                _name = string.Empty;
                _type = type;
                _array = array;
                _index = index;
            }

            public void GetAttributes(List<Attribute> result)
            {
                //do nothing
            }

            public void SetValue(object value)
            {
                _array.SetValue(value, _index);
            }
        }
    }
}
