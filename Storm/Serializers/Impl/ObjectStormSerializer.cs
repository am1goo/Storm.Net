using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Storm.Serializers
{
    internal class ObjectStormSerializer : IStormSerializer
    {
        public static readonly ObjectStormSerializer instance = new ObjectStormSerializer();

        private const char BraceStart   = '{';
        private const char BraceEnd     = '}';

        public bool CanConvert(Type type)
        {
            if (type.IsArray)
                return false;

            var typeInfo = Type.GetTypeCode(type);
            return !typeInfo.TryToStormType(out var _);
        }

        public bool TryParse(ref int index, string[] lines, out string key, out string text)
        {
            return StormSerializer.TryParseText(ref index, lines, BraceStart, BraceEnd, out key, out text);
        }

        public void Populate(IStormVariable variable, IStormValue value, StormContext ctx)
        {
            var type = variable.type;

            var ignoreCase = ctx.settings.options.HasFlag(StormSettings.Options.IgnoreCase);

            var obj = Activator.CreateInstance(type);
            var pis = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);
            var fis = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.SetField);

            StormCache<List<StormFieldOrProperty>>.Pop(out var cache);
            foreach (var pi in pis)
            {
                if (pi.ShouldBeIgnored())
                    continue;

                if (pi.IsPrivate() && !pi.ShouldBeIncluded())
                    continue;

                var fieldOrProperty = new StormFieldOrProperty(obj, pi);
                cache.Add(fieldOrProperty);
            }
            foreach (var fi in fis)
            {
                if (fi.ShouldBeIgnored())
                    continue;

                if (fi.IsPrivate && !fi.ShouldBeIncluded())
                    continue;

                var fieldOrProperty = new StormFieldOrProperty(obj, fi);
                cache.Add(fieldOrProperty);
            }
            foreach (var var in cache)
            {
                if (value.TryGetEntry(var.name, out var varValue, ignoreCase))
                {
                    varValue.Populate(var, ctx);
                }
            }
            StormCache<List<StormFieldOrProperty>>.Push(cache);

            variable.SetValue(obj);
        }

        public async Task<IStormValue> DeserializeAsync(string text, StormContext ctx)
        {
            return await ctx.serializer.DeserializeAsync(text, ctx);
        }

        public async Task<string> SerializeAsync(IStormVariable variable, object obj, StormContext ctx)
        {
            var type = variable.type;

            ctx.intent++;
            var storm = await ctx.serializer.SerializeAsync(type, obj, ctx);
            ctx.intent--;
            if (string.IsNullOrEmpty(storm))
                return null;

            var intent = ctx.settings.GetIntent(ctx.intent);
            ctx.intent++;
            StormCache<StringBuilder>.Pop(out var sb);
            sb.Append(intent).AppendKey(variable.name);
            sb.Append(BraceStart).Append(Environment.NewLine);
            sb.Append(storm);
            sb.Append(intent).Append(BraceEnd);
            var str = sb.ToString();
            sb.Clear();
            StormCache<StringBuilder>.Push(sb);
            ctx.intent--;

            return str;
        }
    }
}
