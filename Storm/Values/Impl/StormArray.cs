using Storm.Serializers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storm
{
    public class StormArray : IStormValue, IStormContainer
    {
        private List<IStormValue> _entries;

        public StormArray()
        {
            _entries = new List<IStormValue>();
        }

        public void GetEntries(List<IStormValue> result)
        {
            result.AddRange(_entries);
        }

        public void Add(string key, IStormValue value)
        {
            _entries.Add(value);
        }

        public void Populate(IStormVariable variable, StormContext ctx)
        {
            var type = variable.type;

            if (!ctx.serializer.TryGetSerializer(type, ctx.settings, out var serializer))
                throw new Exception($"serializer not found for type {type}");

            serializer.Populate(variable, this, ctx);
        }

        public override string ToString()
        {
            StormCache<StringBuilder>.Pop(out var sb);
            foreach (var entry in _entries)
            {
                if (sb.Length > 0)
                    sb.Append(Environment.NewLine);
                sb.Append(entry);
            }
            var str = sb.ToString();
            sb.Clear();
            StormCache<StringBuilder>.Push(sb);
            return str;
        }
    }
}
