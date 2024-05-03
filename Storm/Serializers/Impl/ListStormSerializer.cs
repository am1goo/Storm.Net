using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Storm.Serializers
{
    public class ListStormSerializer : IStormSerializer
    {
        private const char BracketStart = '[';
        private const char BracketEnd   = ']';

        public static readonly ListStormSerializer instance = new ListStormSerializer();

        private static readonly Type _type = typeof(List<>);

        public bool CanConvert(Type type)
        {
            return type.IsGenericType && _type.IsAssignableFrom(type.GetGenericTypeDefinition());
        }

        public bool TryParse(ref int index, string[] lines, out string key, out string text)
        {
            return StormSerializer.TryParseText(ref index, lines, BracketStart, BracketEnd, out key, out text);
        }

        public void Populate(IStormVariable variable, IStormValue value, StormContext ctx)
        {
            var type = variable.type;

            var elementType = type.GetGenericArguments()[0];

            var list = (IList)Activator.CreateInstance(type);

            StormCache<List<IStormValue>>.Pop(out var cache);
            value.GetEntries(cache);
            var elementCount = cache.Count;
            for (int i = 0; i < elementCount; ++i)
            {
                var entry = cache[i];
                var elementVar = new StormListElement(elementType, list, i);
                entry.Populate(elementVar, ctx);
            }
            StormCache<List<IStormValue>>.Push(cache);

            variable.SetValue(list);
        }

        public async Task<IStormValue> DeserializeAsync(string text, StormContext ctx)
        {
            var stormList = new StormArray();
            stormList = await ctx.serializer.DeserializeAsync(stormList, text, ctx);
            return stormList;
        }

        public async Task<string> SerializeAsync(IStormVariable variable, object obj, StormContext ctx)
        {
            var type = variable.type;
            var list = (IList)obj;
            var elementType = type.GetGenericArguments()[0];

            var countMethod = type.GetProperty("Count", BindingFlags.Public | BindingFlags.Instance);
            var itemMethod = type.GetProperty("Item", BindingFlags.Public | BindingFlags.Instance);

            StormCache<StringBuilder>.Pop(out var sb);
            var intent = ctx.settings.GetIntent(ctx.intent);
            sb.Append(intent).AppendKey(variable.name);
            sb.Append(BracketStart).Append(Environment.NewLine);
            ctx.intent++;

            var count = (int)countMethod.GetValue(obj, null);
            for (int i = 0; i < count; ++i)
            {
                var elementValue = itemMethod.GetValue(obj, new object[] { i });
                var elementVar = new StormListElement(elementType, list, i);
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
    }
}
